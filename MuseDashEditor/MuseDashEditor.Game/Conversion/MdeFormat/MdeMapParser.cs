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
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MuseDashEditor.Game.Conversion.Parser;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Object;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Object.MappingObject;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Utils;
using osu.Framework.Logging;

namespace MuseDashEditor.Game.Conversion.MdeFormat;

public class MdeMapParser : MapParser
{
    public override async Task<Map?> Parse(FileInfo file)
    {
        Logger.Log($"Parsing map from file: {file.FullName}...");

        Map map = new(file, MapType.Mdem);

        await ParseMapContent(file, map);

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
        {
            var objectTick = timingPointObject.ObjectTick.Value;

            return new MdeChartTimingPoint(
                timingPointObject.Offset.Value,
                timingPointObject.NewBpm.Value,
                objectTick.TrackIndex,
                objectTick.TrackDivisor,
                objectTick.TrackTickIndex
            );
        }));

        var objects = new List<MdeChartObject>(map.GameObjects.Count);
        objects.AddRange(map.GameObjects.Select(gameObject =>
        {
            var channel =
                $"{Base36Converter.ToBase36((int)gameObject.LaneModifier)[1]}" +
                $"{Base36Converter.ToBase36((int)gameObject.LaneType)[1]}";

            var objectTick = gameObject.ObjectTick.Value;

            return new MdeChartObject(
                gameObject.Offset.Value,
                Base36Converter.ToBase36((int)gameObject.ObjectType),
                channel,
                objectTick.TrackIndex,
                objectTick.TrackDivisor,
                objectTick.TrackTickIndex
            );
        }));

        var chartFormat = new MdeChartFormat(
            chartMetadata,
            timingPoints,
            objects
        );

        var data = JsonSerializer.Serialize(chartFormat);
        await File.WriteAllTextAsync(file.FullName, data, Encoding.UTF8);
    }

    public static async Task ParseMapContent(FileInfo file, Map map)
    {
        var infoFileData = file.OpenText().BaseStream;
        var chartFormat = await JsonSerializer.DeserializeAsync<MdeChartFormat>(infoFileData);
        if (chartFormat == null) return;

        ParseMapContent(chartFormat, map);
    }

    public static void ParseMapContent(MdeChartFormat chartFormat, Map map)
    {
        map.Metadata.InitialLaneSpeed.Value = (LaneSpeed)chartFormat.Metadata.InitialLaneSpeed;
        map.Metadata.InitialScene.Value = (SceneType)chartFormat.Metadata.InitialScene;
        map.Metadata.InitialBpm.Value = chartFormat.TimingPoints[0].Bpm;

        for (var index = 0; index < chartFormat.TimingPoints.Count; index++)
        {
            var (time, bpm, trackIndex, trackDivisor, trackTickIndex) = chartFormat.TimingPoints[index];
            var timingPointObject = new TimingPointObject(index, time, bpm);

            if (trackDivisor != 0)
                timingPointObject.ObjectTick.Value = new ObjectTick(trackIndex, trackDivisor, trackTickIndex);

            map.TimingPoints.Add(timingPointObject);
        }

        foreach (var chartObject in chartFormat.Objects)
        {
            var gameObject = new GameObject(
                chartObject.Offset,
                (ObjectType)Base36Converter.FromBase36(chartObject.ObjectType),
                (LaneType)Base36Converter.FromBase36([chartObject.ChannelType[1]]),
                (LaneModifierType)Base36Converter.FromBase36([chartObject.ChannelType[0]])
            );

            if (chartObject.TrackDivisor != 0)
            {
                gameObject.ObjectTick.Value = new ObjectTick(
                    chartObject.TrackIndex,
                    chartObject.TrackDivisor,
                    chartObject.TrackTickIndex
                );
            }

            map.GameObjects.Add(gameObject);
        }
    }
}
