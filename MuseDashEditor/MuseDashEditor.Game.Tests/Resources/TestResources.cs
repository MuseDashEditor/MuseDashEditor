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
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Parser;
using osu.Framework.IO.Stores;
using osu.Framework.Testing;

namespace MuseDashEditor.Game.Tests.Resources;

public class TestResources
{
    private static readonly TemporaryNativeStorage temp_storage = new("TestResources");

    public static DllResourceStore GetStore()
    {
        return new DllResourceStore(typeof(TestResources).Assembly);
    }

    public static Stream OpenResource(string name)
    {
        return GetStore().GetStream($"Resources/{name}");
    }

    public static async Task<Chart> GetTestChart()
    {
        var directory = await GetTestChartDirectory();
        return await ChartParser.Parse(directory);
    }

    public static async Task<DirectoryInfo> GetTestChartDirectory()
    {
        var dirPath = temp_storage.GetFullPath(Guid.NewGuid().ToString());
        var directory = new DirectoryInfo(dirPath);
        if (!directory.Exists)
            directory.Create();

        await using var sourceStream = OpenResource("WACCA_ULTRA_DREAM_MEGAMIX.mdm");
        ZipFile.ExtractToDirectory(sourceStream, dirPath);

        return directory;
    }
}
