using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Object.MappingObject;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Utils;
using osu.Framework.Logging;

namespace MuseDashEditor.Game.Data.Parser;

public static class BmsParser
{
    private static readonly List<char> base36_chars =
    [
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L',
        'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
    ];

    public static async Task<Map> Parse(FileInfo file)
    {
        Logger.Log($"Parsing map from file: {file.FullName}...");

        Map map = new(file);

        var content = await File.ReadAllTextAsync(file.FullName);

        parseMapContent(content, map);
        processMapContent(map);

        return map;
    }

    private static void processMapContent(Map map)
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
                    {
                        Logger.Log($"Duplicate bpm change at index {globalIndex}", level: LogLevel.Important);
                    }
                }
            }
        }

        if (!bpmChangesInTrack.ContainsKey(0))
        {
            bpmChangesInTrack[0] = map.Metadata.InitialBpm.Value;
        }

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
            var elapsedDuration = (240_000 / bpmAtLastKnownOffset) / maxDataLength * elapsedGlobalIndexes;
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

                    var globalIndex = Math.Round(trackId * maxDataLength + i / (double)dataCount * maxDataLength);

                    // Find last known offset before current index
                    int lastKnownOffsetIndex = 0;
                    foreach (var index in knownOffsets.Keys)
                    {
                        if (index <= globalIndex)
                            lastKnownOffsetIndex = index;
                        else
                            break;
                    }

                    var offsetAtIndex = knownOffsets[lastKnownOffsetIndex];
                    var bpmAtIndex = bpmChangesInTrack[lastKnownOffsetIndex];
                    var elapsedDuration = (240_000 / bpmAtIndex) / maxDataLength * (globalIndex - lastKnownOffsetIndex);
                    var objectOffset = Math.Round(offsetAtIndex + elapsedDuration);

                    var objectType = (ObjectType)objectData;

                    var gameObject = new GameObject(
                        objectOffset,
                        objectType,
                        laneType,
                        laneModifier
                    );
                    map.GameObjects.Add(gameObject);

                    Logger.Log($"Adding object at offset {objectOffset} ({globalIndex} / {trackId}#{i}) with type {objectType} in lane {laneType} ({laneModifier})");
                }
            }
        }

        map.GameObjects.Sort((g1, g2) => g1.Offset.CompareTo(g2.Offset));
        Logger.Log($"Parsed {map.GameObjects.Count} objects");
    }

    private static void parseMapContent(string content, Map map)
    {
        var lines = content.Split(Environment.NewLine);

        Logger.Log($"Parsing {lines.Length} lines...");

        foreach (var line in lines)
        {
            parseLineContent(line, map);
        }
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

        var laneModifierValue = parseBase36(laneDeclaration.AsSpan(3, 1));
        var laneTypeValue = parseBase36(laneDeclaration.AsSpan(4, 1));

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

        for (var i = 0; i < dataCount; i++)
        {
            dataArray[i] = parseBase36(data.AsSpan(i * 2, 2));
        }

        map.RawMapData.ComputeIfAbsent(trackNumber, _ => new Dictionary<(LaneModifierType, LaneType), int[]>())
            .Add((laneModifier, laneType), dataArray);
    }

    private static int parseBase36(ReadOnlySpan<char> span)
    {
        if (span.Length < 2)
            span = $"0{span[0]}";

        var char1 = span[0];
        var char2 = span[1];

        return base36_chars.IndexOf(char1) * 36 + base36_chars.IndexOf(char2);
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
            var bpmKey = parseBase36(key.AsSpan(3, 2));
            map.Metadata.BpmChangeKeys[bpmKey] = bpmValue;
        }
    }
}
