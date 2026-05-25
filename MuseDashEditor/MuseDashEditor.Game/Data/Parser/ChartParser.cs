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
            if (!int.TryParse(file.Name.AsSpan(3, 1), out var mapNumber))
            {
                Logger.Log($"Invalid map number in file name: {file.Name}", level: LogLevel.Important);
                continue;
            }

            if (!Enum.IsDefined(typeof(DifficultyType), mapNumber)) continue;

            var map = await BmsParser.Parse(file);
            if (map == null) continue;

            maps[(DifficultyType)mapNumber] = map;
        }

        var chartInfoFile = new FileInfo(Path.Combine(directory.FullName, "info.json"));
        var infoFileData = chartInfoFile.OpenText().BaseStream;
        var chartInfoRaw = await JsonSerializer.DeserializeAsync<ChartInfoRaw>(infoFileData);

        var chartInfo = new ChartInfo(chartInfoRaw);

        var mp3AudioFiles = directory.GetFiles("*.mp3");
        var oggAudioFiles = directory.GetFiles("*.ogg");

        FileInfo musicFile = null;
        FileInfo demoFile = null;

        foreach (var mp3AudioFile in mp3AudioFiles)
            if (mp3AudioFile.Name.Equals("demo.mp3", StringComparison.OrdinalIgnoreCase))
                demoFile = mp3AudioFile;
            else if (mp3AudioFile.Name.Equals("music.mp3", StringComparison.OrdinalIgnoreCase))
                musicFile = mp3AudioFile;

        foreach (var oggAudioFile in oggAudioFiles)
            if (oggAudioFile.Name.Equals("demo.ogg", StringComparison.OrdinalIgnoreCase))
                demoFile = oggAudioFile;
            else if (oggAudioFile.Name.Equals("music.ogg", StringComparison.OrdinalIgnoreCase))
                musicFile = oggAudioFile;

        return new Chart.Chart(
            directory,
            musicFile,
            demoFile,
            chartInfo,
            maps
        );
    }
}
