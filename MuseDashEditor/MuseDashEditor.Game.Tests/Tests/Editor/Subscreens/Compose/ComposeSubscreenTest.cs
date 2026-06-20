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

using MuseDashEditor.Game.Screens.Editor.SubScreens.Compose;
using MuseDashEditor.Game.Tests.Resources;
using NUnit.Framework;

namespace MuseDashEditor.Game.Tests.Tests.Editor.Subscreens.Compose;

[TestFixture]
public partial class ComposeSubscreenTest : MuseDashEditorTestScene
{
    [Test]
    public void TestScreenLoad()
    {
        var loadChartTask = TestResources.GetTestChart();
        var loadEditorDataTask = InitializeEditorData(loadChartTask);

        AddUntilStep("Load test chart", () => loadChartTask.IsCompletedSuccessfully);
        AddUntilStep("Initialize editor data", () => loadEditorDataTask.IsCompletedSuccessfully);
        AddStep("Test compmose screen load", () =>
        {
            var timingSubscreen = new ComposeSubscreen();
            Content.Add(timingSubscreen);
            timingSubscreen.Show();
        });
        RunAllSteps();
    }
}
