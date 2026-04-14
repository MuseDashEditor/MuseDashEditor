using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MuseDashEditor.Game.Chart.Type;
using osu.Framework.Logging;

namespace MuseDashEditor.Game.Chart.Parser;

public static class ChartParser
{
    private static readonly List<char> base36_chars =
    [
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L',
        'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
    ];

    public static async Task<Chart> Parse(FileInfo file)
    {
        Logger.Log($"Parsing chart from file: {file.FullName}...");

        Chart chart = new();

        using var stream = file.OpenText();
        var content = await stream.ReadToEndAsync();

        parseChartContent(content, chart);

        return chart;
    }

    private static void parseChartContent(string content, Chart chart)
    {
        var lines = content.Split(Environment.NewLine);

        Logger.Log($"Parsing {lines.Length} lines...");

        foreach (var line in lines)
        {
            parseLineContent(line, chart);
        }
    }

    private static void parseLineContent(string line, Chart chart)
    {
        if (!line.StartsWith('#'))
            return;

        var metadataParts = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var dataParts = line.Split(':', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (metadataParts.Length == 2)
        {
            parseMetadataHeader(metadataParts, chart);
            return;
        }

        if (dataParts.Length < 2)
            return;

        parseDataLine(dataParts, chart);
    }

    private static void parseDataLine(string[] dataParts, Chart chart)
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

        var laneModifier = (LaneModifier)parseBase36(laneDeclaration.AsSpan(3, 1));
        var laneType = (LaneType)parseBase36(laneDeclaration.AsSpan(4, 1));

        var dataCount = data.Length / 2;
        var dataArray = new EnemyType[dataCount];

        for (var i = 0; i < dataCount; i++)
        {
            dataArray[i] = (EnemyType)parseBase36(data.AsSpan(i * 2, 2));
        }

        Logger.Log(
            $"Parsed data: lane n°{trackNumber} with modifier {laneModifier} and type {laneType}: {string.Join(", ", dataArray)}");

        // TODO: save lane data
    }

    private static int parseBase36(ReadOnlySpan<char> asSpan)
    {
        if (asSpan.Length < 2)
            return parseBase36($"0{asSpan[0]}");

        var char1 = asSpan[0];
        var char2 = asSpan[1];

        return base36_chars.IndexOf(char1) * 36 + base36_chars.IndexOf(char2);
    }

    private static void parseMetadataHeader(string[] metadataParts, Chart chart)
    {
        Logger.Log($"Parsing metadata header: {metadataParts[0]} = {metadataParts[1]}");

        var key = metadataParts[0].TrimStart('#');
        var value = metadataParts[1].Trim();

        switch (key.ToUpperInvariant())
        {
            case "PLAYER":
            case "GENRE":
            case "TITLE":
            case "ARTIST":
            case "LEVELDESIGN":
            case "BPM":
            case "PLAYLEVEL":
            case "RANK":
                Logger.Log($"Metadata value: {key} = {value}");
                break;
            default:
                Logger.Log($"Unknown metadata key: {key}");
                return;
        }

        // TODO: save metadata
    }
}
