// Copyright 2026 Axel "Azn9" Joly <contact@azn9.dev>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MuseDashEditor.Game.Conversion.Parser;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Object.MappingObject;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Utils;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace MuseDashEditor.Game.Conversion.MdeFormat;

public static class MdeChartLoader
{
    public static async Task<Map?> Parse(FileInfo file)
    {
        Logger.Log($"Parsing map from file: {file.FullName}...");

        Map map = new(file);

        await parseMapContent(file, map);

        return map;
    }

    public static async Task Save(FileInfo file, Map map)
    {
        Logger.Log($"Saving map to {file.FullName}...");

        var chartMetadata = new MdeChartMetadata(
            (int)map.Metadata.InitialLaneSpeed.Value,
            (int)map.Metadata.InitialScene.Value
        );

        var timingPoints = new List<MdeChartTimingPoint>(map.TimingPoints.Count);
        timingPoints.AddRange(map.TimingPoints.Select(timingPointObject =>
            new MdeChartTimingPoint(timingPointObject.Offset.Value, timingPointObject.NewBpm.Value)));

        var objects = new List<MdeChartObject>(map.GameObjects.Count);
        objects.AddRange(map.GameObjects.Select(gameObject =>
        {
            var channel =
                $"{Base36Converter.ToBase36((int)gameObject.LaneModifier)[1]}" +
                $"{Base36Converter.ToBase36((int)gameObject.LaneType)[1]}";

            return new MdeChartObject(
                gameObject.Offset.Value,
                Base36Converter.ToBase36((int)gameObject.ObjectType),
                channel
            );
        }));

        var chartFormat = new MdeChartFormat(
            chartMetadata,
            timingPoints,
            objects
        );

        await using var fileStream = file.OpenWrite();
        await JsonSerializer.SerializeAsync(fileStream, chartFormat);
    }

    private static async Task parseMapContent(FileInfo file, Map map)
    {
        var infoFileData = file.OpenText().BaseStream;
        var chartFormat = await JsonSerializer.DeserializeAsync<MdeChartFormat>(infoFileData);
        if (chartFormat == null) return;

        map.Metadata.InitialLaneSpeed.Value = (LaneSpeed)chartFormat.Metadata.InitialLaneSpeed;
        map.Metadata.InitialScene.Value = (SceneType)chartFormat.Metadata.InitialScene;

        foreach (var (time, bpm) in chartFormat.TimingPoints)
        {
            map.TimingPoints.Add(new TimingPointObject(time, bpm));
        }

        foreach (var chartObject in chartFormat.Objects)
        {
            map.GameObjects.Add(new GameObject(
                chartObject.Offset,
                (ObjectType)Base36Converter.FromBase36(chartObject.ObjectType),
                (LaneType)Base36Converter.FromBase36([chartObject.ChannelType[1]]),
                (LaneModifierType)Base36Converter.FromBase36([chartObject.ChannelType[0]])
            ));
        }
    }

    public static async Task ConvertFromBms(Storage storage)
    {
        Logger.Log("Converting maps from BMS to MDEM...");

        foreach (var filePath in storage.GetFiles(".", "*.bms"))
        {
            var oldFilePath = storage.GetFullPath(filePath);

            Logger.Log($"Converting from {oldFilePath}...");

            var fileInfo = new FileInfo(oldFilePath);
            if (!fileInfo.Exists)
                continue;

            var mapName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
            var newFileName = $"{mapName}.mdem";

            var newFile = new FileInfo(storage.GetFullPath(newFileName));

            await BmsParser.Parse(fileInfo)
                .ContinueWith(task =>
                {
                    if (!task.IsCompletedSuccessfully || task.Result is null)
                        return Task.CompletedTask;
                    return Save(newFile, task.Result);
                });

            storage.Delete(filePath);

            Logger.Log($"Converted to {newFileName}!");
        }
    }
}
