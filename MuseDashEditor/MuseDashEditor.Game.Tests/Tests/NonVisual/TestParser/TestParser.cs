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
using System.Linq;
using System.Text.Json;
using MuseDashEditor.Game.Conversion.BmsFormat;
using MuseDashEditor.Game.Conversion.MdeFormat;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Tests.Resources;
using MuseDashEditor.Game.Utils;
using NUnit.Framework;

namespace MuseDashEditor.Game.Tests.Tests.NonVisual.TestParser;

public class TestParser
{
    [Theory]
    public void Parse([Values(
                          "single_object_at_start",
                          "single_object_at_1_bar",
                          "two_objects_different_track",
                          "normal_track_split",
                          "64th_track_split"
                      )]
                      string mapFile)
    {
        using var inputBmsStream = TestResources.OpenResource($"TestParser/{mapFile}.bms");
        using var inputJsonStream = TestResources.OpenResource($"TestParser/{mapFile}.json");

        Assert.NotNull(inputBmsStream);
        Assert.NotNull(inputJsonStream);

        using var inputBmsStreamReader = new StreamReader(inputBmsStream);
        using var inputJsonStreamReader = new StreamReader(inputJsonStream);

        var inputBmsContent = inputBmsStreamReader.ReadToEnd().ReplaceLineEndings("\r\n");
        var inputJsonContent = inputJsonStreamReader.ReadToEnd().ReplaceLineEndings("\r\n");

        Map map = new(null!, MapType.Bms);

        BmsMapParser.ParseMapContent(inputBmsContent, map);
        BmsMapParser.ProcessMapContent(map);

        var chartFormat = JsonSerializer.Deserialize<MdeChartFormat>(inputJsonContent);
        Assert.NotNull(chartFormat);

        foreach (var obj in chartFormat.Objects)
        {
            var objectType = (ObjectType)Base36Converter.FromBase36(obj.ObjectType);
            var laneType = (LaneType)Base36Converter.FromBase36([obj.ChannelType[1]]);
            var laneModifier = (LaneModifierType)Base36Converter.FromBase36([obj.ChannelType[0]]);

            var found = map.GameObjects.Any(mapGameObject => Math.Abs(mapGameObject.Offset.Value - obj.Offset) < 0.01D
                                                             && mapGameObject.ObjectType == objectType
                                                             && mapGameObject.LaneType == laneType
                                                             && mapGameObject.LaneModifier == laneModifier);
            Assert.True(found);
        }
    }
}
