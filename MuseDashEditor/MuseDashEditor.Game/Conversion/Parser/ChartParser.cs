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
using System.Text.Json;
using System.Threading.Tasks;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Logging;

namespace MuseDashEditor.Game.Conversion.Parser;

public static class ChartParser
{
    public static async Task<Chart?> Parse(DirectoryInfo directory)
    {
        if (!directory.Exists) return null;

        var maps = new Dictionary<DifficultyType, Map>();

        foreach (var file in directory.GetFiles())
        {
            if (!int.TryParse(file.Name.AsSpan(3, 1), out var mapNumber))
                continue;

            if (!Enum.IsDefined(typeof(DifficultyType), mapNumber))
                continue;

            var parser = MapParser.GetParser(file);
            if (parser is null)
                continue;

            if (maps.ContainsKey((DifficultyType)mapNumber))
            {
                Logger.Log($"Multiple files found for difficulty {mapNumber}!", LoggingTarget.Runtime, LogLevel.Important);
            }

            var map = await parser.Parse(file);
            if (map == null)
                continue;

            maps[(DifficultyType)mapNumber] = map;
        }

        var chartInfoFile = new FileInfo(Path.Combine(directory.FullName, "info.json"));
        var infoFileData = chartInfoFile.OpenText().BaseStream;
        var chartInfoRaw = await JsonSerializer.DeserializeAsync<ChartInfoRaw>(infoFileData);
        if (chartInfoRaw == null) return null;

        var chartInfo = new ChartInfo(chartInfoRaw);

        var mp3AudioFiles = directory.GetFiles("*.mp3");
        var oggAudioFiles = directory.GetFiles("*.ogg");

        FileInfo musicFile = null!; // TODO
        FileInfo? demoFile = null;

        foreach (var mp3AudioFile in mp3AudioFiles)
        {
            if (mp3AudioFile.Name.Equals("demo.mp3", StringComparison.OrdinalIgnoreCase))
                demoFile = mp3AudioFile;
            else if (mp3AudioFile.Name.Equals("music.mp3", StringComparison.OrdinalIgnoreCase))
                musicFile = mp3AudioFile;
        }

        foreach (var oggAudioFile in oggAudioFiles)
        {
            if (oggAudioFile.Name.Equals("demo.ogg", StringComparison.OrdinalIgnoreCase))
                demoFile = oggAudioFile;
            else if (oggAudioFile.Name.Equals("music.ogg", StringComparison.OrdinalIgnoreCase))
                musicFile = oggAudioFile;
        }

        if (musicFile == null)
        {
            // TODO popup
            throw new Exception("No music file found");
        }

        return new Chart(
            directory,
            musicFile,
            demoFile,
            chartInfo,
            maps
        );
    }
}
