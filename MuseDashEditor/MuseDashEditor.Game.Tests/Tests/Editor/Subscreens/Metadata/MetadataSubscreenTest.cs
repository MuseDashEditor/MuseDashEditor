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
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata;
using MuseDashEditor.Game.Tests.Resources;
using NUnit.Framework;
using osu.Framework.Allocation;

namespace MuseDashEditor.Game.Tests.Tests.Editor.Subscreens.Metadata;

[TestFixture]
public partial class MetadataSubscreenTest : MuseDashEditorTestScene
{
    [Cached] protected readonly EditorDataHolder EditorDataHolder = new();

    [Test]
    public void TestScreenLoad()
    {
        var loadChartTask = TestResources.GetTestChart();

        AddUntilStep("Load test chart", () => loadChartTask.IsCompletedSuccessfully);
        AddStep("Test metadata screen load", () =>
        {
            if (!loadChartTask.IsCompletedSuccessfully)
                throw new InvalidOperationException("Test chart failed to load");

            EditorDataHolder.CurrentChart.Value = loadChartTask.Result;

            Child = new MetadataSubscreen();
            Child.Show();

            // TODO: validate screen content
        });
    }
}
