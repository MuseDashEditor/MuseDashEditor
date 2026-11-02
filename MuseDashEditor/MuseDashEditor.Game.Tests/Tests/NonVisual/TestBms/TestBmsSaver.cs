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

using System;
using System.IO;
using System.Text.Json;
using MuseDashEditor.Game.Conversion.BmsFormat;
using MuseDashEditor.Game.Conversion.MdeFormat;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Tests.Resources;
using MuseDashEditor.Game.Utils;
using NUnit.Framework;

namespace MuseDashEditor.Game.Tests.Tests.NonVisual.TestBms;

public class TestBmsSaver
{
    [Theory]
    public void Parse([Values(
                          "empty_map",
                          "single_object_at_start",
                          "64th_track_split",
                          "single_object_at_1_bar",
                          "two_objects_different_track",
                          "bpm_change_simple",
                          "bpm_change_divisor",
                          "two_bpm_change_in_one_track",
                          "two_objects_different_lanes",
                          "triplets_and_quadruplets",
                          "triplets_and_quadruplets_same_track"
                      )]
                      string mapFile)
    {
        using var inputBmsStream = TestResources.OpenResource($"TestBmsSaver/{mapFile}.bms");
        using var inputJsonStream = TestResources.OpenResource($"TestBmsSaver/{mapFile}.json");

        Assert.NotNull(inputBmsStream);
        Assert.NotNull(inputJsonStream);

        using var inputBmsStreamReader = new StreamReader(inputBmsStream);
        using var inputJsonStreamReader = new StreamReader(inputJsonStream);

        var inputBmsContent = inputBmsStreamReader.ReadToEnd().ReplaceLineEndings(Environment.NewLine);
        var inputJsonContent = inputJsonStreamReader.ReadToEnd().ReplaceLineEndings(Environment.NewLine);

        var chartFormat = JsonSerializer.Deserialize<MdeChartFormat>(inputJsonContent);
        Assert.NotNull(chartFormat);

        Map map = new(null!, MapType.Bms);

        MdeMapParser.ParseMapContent(chartFormat, map);
        MapUtils.PreProcessMap(map);

        var saveContent = BmsMapParser.GetSaveContent(map);

        Assert.AreEqual(inputBmsContent.Trim(), saveContent.Trim());
    }
}
