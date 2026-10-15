// Copyright 2026 Axel "Azn9" Joly <contact@azn9.dev>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuseDashEditor.Game.Conversion.Parser;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Object;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Object.MappingObject;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Utils;
using osu.Framework.Logging;
using osu.Framework.Utils;
using MathUtils = MuseDashEditor.Game.Utils.MathUtils;

namespace MuseDashEditor.Game.Conversion.BmsFormat;

public class BmsMapParser : MapParser
{
    public override async Task<Map?> Parse(FileInfo file)
    {
        Logger.Log($"Parsing map from file: {file.FullName}...");

        Map map = new(file, MapType.Bms);

        var content = await File.ReadAllTextAsync(file.FullName);

        ParseMapContent(content, map);
        ProcessMapContent(map);

        return map;
    }

    public static void ProcessMapContent(Map map)
    {
        var bpmChangesInTrack = new SortedDictionary<int, double>();

        // Max data length in the map, used to calculate timing point indexes across the whole map
        var maxDataLength = map.RawMapData.Values.Max(x => x.Values.Max(y => y.Length));

        // 1. Parse bpm changes
        foreach (var (trackId, trackDataDictionary) in map.RawMapData)
        {
            foreach (var ((laneModifier, laneType), data) in trackDataDictionary)
            {
                if (laneModifier != LaneModifierType.Music) continue;
                if ((int)laneType != 8) continue;

                var dataCount = data.Length;

                for (var i = 0; i < dataCount; i++)
                {
                    var objectData = data[i];
                    if (objectData == 0) continue;

                    var globalIndex = trackId * maxDataLength + i / dataCount * maxDataLength;

                    if (globalIndex == 0)
                    {
                        Logger.Log("Found bpm change at index 0!", level: LogLevel.Error);
                        continue;
                    }

                    if (!map.Metadata.BpmChangeKeys.TryGetValue(objectData, out var bpmChangeValue)) continue;

                    if (!bpmChangesInTrack.TryAdd(globalIndex, bpmChangeValue))
                        Logger.Log($"Duplicate bpm change at index {globalIndex}", level: LogLevel.Important);
                }
            }
        }

        if (!bpmChangesInTrack.ContainsKey(0)) bpmChangesInTrack[0] = map.Metadata.InitialBpm.Value;

        var knownOffsets = new SortedDictionary<int, double> { { 0, 0 } };

        // 2. Add bpm changes to the timing points in order
        foreach (var (index, bpmChangeValue) in bpmChangesInTrack)
        {
            Logger.Log($"Adding bpm change at index {index} with value {bpmChangeValue}");

            if (index == 0) continue;

            // This will always be the previous bpm change offset as we are adding them in order
            var (lastKnownOffsetIndex, lastKnownOffsetValue) = knownOffsets.Last();
            var bpmAtLastKnownOffset = bpmChangesInTrack[lastKnownOffsetIndex];

            var elapsedGlobalIndexes = index - lastKnownOffsetIndex;
            var elapsedDuration = 240_000 / bpmAtLastKnownOffset / maxDataLength * elapsedGlobalIndexes;
            var newOffset = lastKnownOffsetValue + elapsedDuration;

            Logger.Log($"Adding offset for index {index} with value {newOffset}");
            knownOffsets.Add(index, newOffset);
        }

        // 3. Convert back to timing points
        foreach (var (index, offset) in knownOffsets)
        {
            if (index == 0) continue;

            var bpm = bpmChangesInTrack[index];
            map.TimingPoints.Add(new TimingPointObject(offset, bpm));
        }

        // 3. Parse everything else now that we can get the timing of any object
        foreach (var (trackId, trackDataDictionary) in map.RawMapData)
        {
            foreach (var ((laneModifier, laneType), data) in trackDataDictionary)
            {
                if (laneModifier == LaneModifierType.Music) continue;

                var dataCount = data.Length;

                for (var i = 0; i < dataCount; i++)
                {
                    var objectData = data[i];
                    if (objectData == 0) continue;

                    var globalIndex = (int)Math.Round(trackId * maxDataLength + i / (double)dataCount * maxDataLength);

                    // Find last known offset before current index
                    var lastKnownOffsetIndex = 0;
                    var timingPointIndex = -1;

                    foreach (var index in knownOffsets.Keys)
                    {
                        if (index <= globalIndex)
                        {
                            lastKnownOffsetIndex = index;
                            timingPointIndex++;
                        }
                        else
                            break;
                    }

                    var offsetAtIndex = knownOffsets[lastKnownOffsetIndex];
                    var bpmAtIndex = bpmChangesInTrack[lastKnownOffsetIndex];
                    var objectIndexInTimingPoint = globalIndex - lastKnownOffsetIndex;
                    var elapsedDuration = 240_000 / bpmAtIndex / maxDataLength * objectIndexInTimingPoint;
                    var objectOffset = offsetAtIndex + elapsedDuration;

                    var objectType = (ObjectType)objectData;

                    var indexGcd = MathUtils.Gcd(maxDataLength, objectIndexInTimingPoint);
                    var divisor = maxDataLength / indexGcd;
                    var dividedObjectIndex = objectIndexInTimingPoint / indexGcd;

                    var gameObject = new GameObject(
                        objectOffset,
                        objectType,
                        laneType,
                        laneModifier
                    )
                    {
                        ObjectTick =
                        {
                            Value = new ObjectTick(
                                timingPointIndex,
                                divisor,
                                dividedObjectIndex
                            )
                        }
                    };

                    map.GameObjects.Add(gameObject);

                    Logger.Log(
                        $"Adding object at offset {objectOffset} ({globalIndex} / {trackId}#{i}) with type {objectType} in lane {laneType} ({laneModifier})");
                }
            }
        }

        Logger.Log($"Parsed {map.GameObjects.Count} objects");
    }

    public static void ParseMapContent(string content, Map map)
    {
        var lines = content.Split(Environment.NewLine);

        Logger.Log($"Parsing {lines.Length} lines...");

        foreach (var line in lines) parseLineContent(line, map);
    }

    private static void parseLineContent(string line, Map map)
    {
        if (!line.StartsWith('#'))
            return;

        var metadataParts = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var dataParts = line.Split(':', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (metadataParts.Length == 2)
        {
            parseMetadataHeader(metadataParts, map);
            return;
        }

        if (dataParts.Length < 2)
            return;

        parseDataLine(dataParts, map);
    }

    private static void parseDataLine(string[] dataParts, Map map)
    {
        Logger.Log($"Parsing data line: {dataParts[0]}:{dataParts[1]}");

        var laneDeclaration = dataParts[0].TrimStart('#');
        var data = dataParts[1].Trim();

        if (laneDeclaration.Length != 5)
            return;

        if (data.Length % 2 != 0)
            return;

        if (!int.TryParse(laneDeclaration.AsSpan(0, 3), out var trackNumber))
            return;

        var laneModifierValue = Base36Converter.FromBase36(laneDeclaration.AsSpan(3, 1));
        var laneTypeValue = Base36Converter.FromBase36(laneDeclaration.AsSpan(4, 1));

        if (!Enum.IsDefined(typeof(LaneModifierType), laneModifierValue))
        {
            Logger.Log($"Unknown lane modifier: {laneModifierValue}", level: LogLevel.Important);
            return;
        }

        if (!Enum.IsDefined(typeof(LaneType), laneTypeValue))
        {
            Logger.Log($"Unknown lane type: {laneTypeValue}", level: LogLevel.Important);
            return;
        }

        var laneModifier = (LaneModifierType)laneModifierValue;
        var laneType = (LaneType)laneTypeValue;

        var dataCount = data.Length / 2;
        var dataArray = new int[dataCount];

        for (var i = 0; i < dataCount; i++) dataArray[i] = Base36Converter.FromBase36(data.AsSpan(i * 2, 2));

        var trackData =
            map.RawMapData.ComputeIfAbsent(trackNumber, () => new Dictionary<(LaneModifierType, LaneType), int[]>());

        var dataKey = (laneModifier, laneType);

        // We don't attempt to merge etc. to follow what BMS does
        trackData[dataKey] = dataArray;
    }

    private static void parseMetadataHeader(string[] metadataParts, Map map)
    {
        Logger.Log($"Parsing metadata header: {metadataParts[0]} = {metadataParts[1]}");

        var key = metadataParts[0].TrimStart('#').ToUpperInvariant();
        var value = metadataParts[1].Trim();

        switch (key)
        {
            case "PLAYER":
            {
                if (!int.TryParse(value, out var playerNumber))
                {
                    Logger.Log($"Invalid player number: {value}", level: LogLevel.Important);
                    return;
                }

                map.Metadata.InitialLaneSpeed.Value = (LaneSpeed)playerNumber;
                return;
            }

            case "GENRE":
            {
                var initialScene = SceneUtils.GetSceneType(value);

                if (initialScene == SceneType.Unknown)
                {
                    Logger.Log($"Invalid scene name: {value}", level: LogLevel.Important);
                    return;
                }

                map.Metadata.InitialScene.Value = initialScene;
                return;
            }

            case "BPM":
            {
                if (!double.TryParse(value, out var initialBpmValue))
                {
                    Logger.Log($"Invalid BPM value: {value}", level: LogLevel.Important);
                    return;
                }

                map.Metadata.InitialBpm.Value = initialBpmValue;
                map.TimingPoints.Add(new TimingPointObject(0, initialBpmValue));

                return;
            }
        }

        if (key.StartsWith("BPM") && int.TryParse(value, out var bpmValue))
        {
            var bpmKey = Base36Converter.FromBase36(key.AsSpan(3, 2));
            map.Metadata.BpmChangeKeys[bpmKey] = bpmValue;
        }
    }

    public static async Task Save(FileInfo file, Map map)
    {
        var header = buildHeader(map);
        var content = buildContent(map);

        var data = header + "\n" + content;

        await File.WriteAllTextAsync(file.FullName, data, Encoding.UTF8);
    }

    private static string buildHeader(Map map)
    {
        var header = $"""
                      *---------------------- HEADER FIELD

                      #PLAYER {(int)map.Metadata.InitialLaneSpeed.Value}
                      #GENRE scene_{(int)map.Metadata.InitialScene.Value:D2}
                      #TITLE
                      #ARTIST
                      #LEVELDESIGN
                      #BPM {map.Metadata.InitialBpm.Value}
                      #PLAYLEVEL 1
                      #RANK 1
                      #LNTYPE 1


                      """;

        if (map.TimingPoints.Count <= 1)
            return header;

        for (var i = 0; i < map.TimingPoints.Count; i++)
        {
            header += "#BPM" + Base36Converter.ToBase36(i) + " " + map.TimingPoints[i].NewBpm.Value + "\n";
        }

        return header;
    }

    private static string buildContent(Map map)
    {
        var content = """
                      *---------------------- MAIN DATA FIELD
                      #00001:10


                      """;

        var currentTimingPoint = map.TimingPoints[0];
        TimingPointObject? nextTimingPoint = map.TimingPoints.Count > 1 ? map.TimingPoints[1] : null;

        var trackLength = 60 / currentTimingPoint.NewBpm.Value * 4000;
        var elapsedDuration = 0D;

        for (int track = 0; track < 1000; track++)
        {
            var trackStartTime = elapsedDuration;
            var trackEndTime = elapsedDuration + trackLength;

            var offsetValue = nextTimingPoint?.Offset.Value;

            if (offsetValue is not null)
            {
                if (Precision.AlmostEquals(elapsedDuration, offsetValue.Value, 1E-2D) || trackEndTime > offsetValue)
                {
                    return content;
                    ;
                    // TODO
                }
            }

            var objectsInTrack = map.GameObjects.Where(o => o.Offset.Value >= trackStartTime && o.Offset.Value < trackEndTime).ToList();
            Dictionary<(LaneModifierType, LaneType), List<GameObject>> objectsPerChannel = [];

            foreach (var laneModifier in Enum.GetValues<LaneModifierType>())
            {
                foreach (var laneType in Enum.GetValues<LaneType>())
                {
                    foreach (var gameObject in objectsInTrack.Where(o => o.LaneModifier == laneModifier && o.LaneType == laneType))
                    {
                        objectsPerChannel.ComputeIfAbsent((laneModifier, laneType), () => []).Add(gameObject);
                    }
                }
            }

            foreach (var (key, objects) in objectsPerChannel)
            {
                var channel = $"{Base36Converter.ToBase36((int)key.Item1)[1]}" +
                              $"{Base36Converter.ToBase36((int)key.Item2)[1]}";

                var lcm = MathUtils.Lcm([.. objects.Select(o => o.ObjectTick.Value.BeatDivisor)]);
                var tickLength = trackLength / lcm;
                var elapsedTick = (int)(elapsedDuration / tickLength);

                var trackData = new List<string>(lcm);

                for (int i = 0; i < lcm; i++)
                {
                    trackData.Add("00");
                }

                foreach (var obj in objects)
                {
                    var objectTick = obj.ObjectTick.Value;
                    var tick = (objectTick.BeatIndex * lcm) / objectTick.BeatDivisor - elapsedTick;
                    trackData[tick] = Base36Converter.ToBase36((int)obj.ObjectType);
                }

                content += $"#{track:000}{channel}:{string.Join("", trackData)}\n";
            }

            content += "\n";
            elapsedDuration += trackLength;
        }

        return content;
    }
}
