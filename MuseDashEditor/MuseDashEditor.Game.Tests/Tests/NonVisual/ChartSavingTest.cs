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
using System.Threading.Tasks;
using MuseDashEditor.Game.Conversion.BmsFormat;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Tests.Resources;
using NUnit.Framework;

namespace MuseDashEditor.Game.Tests.Tests.NonVisual;

[TestFixture]
public class ChartSavingTest
{
    [Test]
    public void TestParseChart()
    {
        Chart chart = null;

        Assert.DoesNotThrowAsync(parseChart);
        Assert.IsNotNull(chart);

        var map = chart.HardMap;
        Assert.IsNotNull(map);

        var fileInfo = new FileInfo(Path.GetTempFileName());
        Assert.IsNotNull(fileInfo);

        Assert.DoesNotThrowAsync(() => BmsMapParser.Save(fileInfo, map));

        Console.WriteLine(fileInfo.FullName);

        return;

        async Task parseChart() => chart = await TestResources.GetTestChart();
    }
}
