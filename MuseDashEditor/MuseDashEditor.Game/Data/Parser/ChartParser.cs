using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Type;

namespace MuseDashEditor.Game.Data.Parser;

public static class ChartParser
{
    public static async Task<Chart.Chart> Parse(DirectoryInfo directory)
    {
        if (!directory.Exists) return null;

        var maps = new Dictionary<DifficultyType, Map>();

        var bmsFiles = directory.GetFiles("*.bms");
        foreach (var file in bmsFiles)
        {
            if (!int.TryParse(file.Name.AsSpan(4, 2), out var mapNumber)) continue;

            if (!Enum.IsDefined(typeof(DifficultyType), mapNumber))
            {
                continue;
            }

            var map = await BmsParser.Parse(file);
            if (map == null) continue;

            maps[(DifficultyType)mapNumber] = map;
        }

        var chartInfoFile = new FileInfo(Path.Combine(directory.FullName, "info.json"));
        ChartInfo chartInfo = null; // TODO: parse chart data file

        var mp3AudioFiles = directory.GetFiles("*.mp3");
        var oggAudioFiles = directory.GetFiles("*.ogg");

        FileInfo musicFile = null;
        FileInfo demoFile = null;

        foreach (var mp3AudioFile in mp3AudioFiles)
        {
            if (mp3AudioFile.Name.Equals("demo.mp3", StringComparison.OrdinalIgnoreCase))
            {
                demoFile = mp3AudioFile;
            }
            else if (mp3AudioFile.Name.Equals("music.mp3", StringComparison.OrdinalIgnoreCase))
            {
                musicFile = mp3AudioFile;
            }
        }

        foreach (var oggAudioFile in oggAudioFiles)
        {
            if (oggAudioFile.Name.Equals("demo.ogg", StringComparison.OrdinalIgnoreCase))
            {
                demoFile = oggAudioFile;
            }
            else if (oggAudioFile.Name.Equals("music.ogg", StringComparison.OrdinalIgnoreCase))
            {
                musicFile = oggAudioFile;
            }
        }

        return new Chart.Chart(
            directory,
            musicFile,
            demoFile,
            chartInfo,
            maps
        );
    }
}
