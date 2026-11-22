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
        var bpmChangesInTrack = new SortedDictionary<(int, int, int), double>(BpmChangesInTrackComparer.Instance);

        // 1. Parse bpm changes
        foreach (var (trackId, trackDataDictionary) in map.RawMapData)
        {
            foreach (var ((laneModifier, laneType), data) in trackDataDictionary)
            {
                if (laneModifier != LaneModifierType.Music) continue;
                if ((int)laneType != 8) continue;

                var dataCount = data.Length;

                for (var index = 0; index < dataCount; index++)
                {
                    var objectData = data[index];
                    if (objectData == 0) continue;

                    if (!map.Metadata.BpmChangeKeys.TryGetValue(objectData, out var bpmChangeValue))
                        continue;

                    bpmChangesInTrack.Add((trackId, index, dataCount), bpmChangeValue);
                }
            }
        }

        if (!bpmChangesInTrack.ContainsKey((0, 0, 1)))
            bpmChangesInTrack[(0, 0, 1)] = map.Metadata.InitialBpm.Value;

        var knownOffsets = new SortedDictionary<(int, int, int), double> { { (0, 0, 1), 0 } };
        map.TimingPoints.Add(new TimingPointObject(0, 0, map.Metadata.InitialBpm.Value)
        {
            ObjectTick =
            {
                Value = new ObjectTick(
                    0,
                    1,
                    0
                )
            }
        });

        // 2. Add bpm changes to the timing points in order
        var tpIndex = 1;

        foreach (var (offsetData, bpmChangeValue) in bpmChangesInTrack)
        {
            var (trackId, index, divisor) = offsetData;
            Logger.Log($"Adding bpm change at index {index} with value {bpmChangeValue}");

            if (trackId == 0 && index == 0)
                continue;

            // This will always be the previous bpm change offset as we are adding them in order
            var (lastKnownOffsetData, lastKnownOffsetValue) = knownOffsets.Last();
            var (lastKnownTrackId, lastKnownOffsetIndex, lastKnownOffsetDivisor) = lastKnownOffsetData;
            var bpmAtLastKnownOffset = bpmChangesInTrack[lastKnownOffsetData];

            // Timing from TP to end of its track + timing of tracks between + timing between beginning of current track and current TP
            double elapsedDurationBefore = 0;

            if (lastKnownTrackId < trackId)
            {
                var fullTracksBetween = trackId - lastKnownTrackId - 1;
                var remainingTracksTicks = lastKnownOffsetDivisor - lastKnownOffsetIndex;
                var trackLength = 240_000 / bpmAtLastKnownOffset;
                elapsedDurationBefore = fullTracksBetween * trackLength + (remainingTracksTicks / (double)lastKnownOffsetDivisor) * trackLength;
            }

            var elapsedDuration = 240_000 / bpmAtLastKnownOffset * (index / (double)divisor);
            var newOffset = lastKnownOffsetValue + elapsedDurationBefore + elapsedDuration;

            Logger.Log($"Adding offset for index {index} with value {newOffset}");
            knownOffsets.Add(offsetData, newOffset);

            var indexGcd = MathUtils.Gcd(divisor, index);
            var divisorDivided = divisor / indexGcd;
            var dividedObjectIndex = index / indexGcd;

            // 3. Convert back to timing points
            map.TimingPoints.Add(new TimingPointObject(tpIndex++, newOffset, bpmChangeValue)
            {
                ObjectTick =
                {
                    Value = new ObjectTick(
                        trackId,
                        divisorDivided,
                        dividedObjectIndex
                    )
                }
            });
        }

        // 4. Parse everything else now that we can get the timing of any object
        foreach (var (trackId, trackDataDictionary) in map.RawMapData)
        {
            foreach (var ((laneModifier, laneType), data) in trackDataDictionary)
            {
                if (laneModifier == LaneModifierType.Music)
                    continue;

                var dataCount = data.Length;

                for (var index = 0; index < dataCount; index++)
                {
                    var objectData = data[index];

                    if (objectData == 0)
                        continue;

                    // Find last known offset before current index
                    var lastKnownOffsetData = (0, 0, 0);

                    foreach (var offsetData in knownOffsets.Keys)
                    {
                        var (tId, tIndex, tDivisor) = offsetData;

                        if (tId < trackId || (tId == trackId && (tIndex / (double)tDivisor) <= (index / (double)dataCount)))
                        {
                            lastKnownOffsetData = offsetData;
                        }
                        else
                            break;
                    }

                    var (lastKnownOffsetTrackId, lastKnownOffsetIndex, lastKnownOffsetDivisor) = lastKnownOffsetData;
                    var offsetAtIndex = knownOffsets[lastKnownOffsetData];
                    var bpmAtIndex = bpmChangesInTrack[lastKnownOffsetData];

                    double elapsedDurationBefore = 0;

                    if (lastKnownOffsetTrackId < trackId)
                    {
                        var fullTracksBetween = trackId - lastKnownOffsetTrackId - 1;
                        var remainingTracksTicks = lastKnownOffsetDivisor - lastKnownOffsetIndex;
                        var trackLength = 240_000 / bpmAtIndex;
                        elapsedDurationBefore = fullTracksBetween * trackLength + (remainingTracksTicks / (double)lastKnownOffsetDivisor) * trackLength;
                    }

                    var elapsedDuration = 240_000 / bpmAtIndex * (index / (double)dataCount);
                    var objectOffset = offsetAtIndex + elapsedDurationBefore + elapsedDuration;

                    if (!Enum.IsDefined(typeof(ObjectType), objectData))
                    {
                        ExtensionMethods.DebugBreak();
                        Logger.Log($"Unknown object: {objectData}", level: LogLevel.Important);
                        continue;
                    }

                    var objectType = (ObjectType)objectData;

                    var indexGcd = MathUtils.Gcd(dataCount, index);
                    var divisor = dataCount / indexGcd;
                    var dividedObjectIndex = index / indexGcd;

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
                                trackId,
                                divisor,
                                dividedObjectIndex
                            )
                        }
                    };

                    map.GameObjects.Add(gameObject);

                    Logger.Log(
                        $"Adding object at offset {objectOffset} ({trackId}#{index}/{dataCount}) with type {objectType} in lane {laneType} ({laneModifier})");
                }
            }
        }

        Logger.Log($"Parsed {map.GameObjects.Count} objects");
    }

    public static void ParseMapContent(string content, Map map)
    {
        var lines = content.Split(Environment.NewLine);

        Logger.Log($"Parsing {lines.Length} lines...");

        foreach (var line in lines)
            parseLineContent(line, map);
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
            ExtensionMethods.DebugBreak();
            Logger.Log($"Unknown lane modifier: {laneModifierValue}", level: LogLevel.Important);
            return;
        }

        if (!Enum.IsDefined(typeof(LaneType), laneTypeValue))
        {
            ExtensionMethods.DebugBreak();
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
                return;
            }
        }

        if (key.StartsWith("BPM") && int.TryParse(value, out var bpmValue))
        {
            var bpmKey = Base36Converter.FromBase36(key.AsSpan(3, 2));
            map.Metadata.BpmChangeKeys[bpmKey] = bpmValue;
        }
    }
}
