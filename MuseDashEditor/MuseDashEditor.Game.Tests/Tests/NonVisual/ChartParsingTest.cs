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

using System.Threading.Tasks;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Tests.Resources;
using NUnit.Framework;

namespace MuseDashEditor.Game.Tests.Tests.NonVisual;

[TestFixture]
public class ChartParsingTest
{
    [Test]
    public void TestParseChart()
    {
        Chart chart = null;

        Assert.DoesNotThrowAsync(parseChart);
        Assert.IsNotNull(chart);
        Assert.IsNotNull(chart.Directory);
        Assert.IsNotNull(chart.ChartInfo);
        Assert.IsNotNull(chart.EasyMap);
        Assert.IsNotNull(chart.HardMap);

        Assert.That(chart.ChartInfo.NameBindable.Value, Is.EqualTo("WACCA ULTRA DREAM MEGAMIX"));
        Assert.That(chart.ChartInfo.AuthorBindable.Value, Is.EqualTo("USAO & Kobaryo"));
        Assert.That(chart.ChartInfo.BpmBindable.Value, Is.EqualTo("95-245"));
        Assert.That(chart.ChartInfo.DefaultSceneBindable.Value, Is.EqualTo(SceneType.SpaceStation));
        Assert.That(chart.ChartInfo.LevelDesignerBindable.Value, Is.EqualTo("MDMC ULTRA DREAM TEAM"));

        Assert.IsNotNull(chart.EasyMap.MapFile);
        Assert.IsNotNull(chart.EasyMap.Metadata);
        Assert.That(chart.EasyMap.Metadata.InitialBpm.Value, Is.EqualTo(200D));
        Assert.That(chart.EasyMap.Metadata.InitialLaneSpeed.Value, Is.EqualTo(LaneSpeed.Fast));
        Assert.That(chart.EasyMap.Metadata.InitialScene.Value, Is.EqualTo(SceneType.SpaceStation));

        Assert.IsNotNull(chart.HardMap.MapFile);
        Assert.IsNotNull(chart.HardMap.Metadata);
        Assert.That(chart.HardMap.Metadata.InitialBpm.Value, Is.EqualTo(200D));
        Assert.That(chart.HardMap.Metadata.InitialLaneSpeed.Value, Is.EqualTo(LaneSpeed.Fast));
        Assert.That(chart.HardMap.Metadata.InitialScene.Value, Is.EqualTo(SceneType.SpaceStation));

        return;

        async Task parseChart() => chart = await TestResources.GetTestChart();
    }
}
