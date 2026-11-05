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
using MuseDashEditor.Game.Conversion.BmsFormat;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Tests.Resources;
using NUnit.Framework;

namespace MuseDashEditor.Game.Tests.Tests.NonVisual.TestBms;

public class TestBmsParserAndSaver
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
                          "two_bpm_change_in_two_tracks"
                      )]
                      string mapFile)
    {
        using var inputBmsStream = TestResources.OpenResource($"TestBmsParserAndSaver/{mapFile}.bms");

        Assert.NotNull(inputBmsStream);

        using var inputBmsStreamReader = new StreamReader(inputBmsStream);

        var inputBmsContent = inputBmsStreamReader.ReadToEnd().ReplaceLineEndings(Environment.NewLine);

        Map map = new(null!, MapType.Bms);

        BmsMapParser.ParseMapContent(inputBmsContent, map);
        BmsMapParser.ProcessMapContent(map);

        var saveContent = BmsMapParser.GetSaveContent(map);

        Assert.AreEqual(inputBmsContent.Trim(), saveContent.Trim());
    }
}
