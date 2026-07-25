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

using MuseDashEditor.Game.Screens.Open.Components;
using MuseDashEditor.Game.Tests.Resources;
using NUnit.Framework;
using osu.Framework.Platform;

namespace MuseDashEditor.Game.Tests.Tests.Screens;

public partial class ProjectListScreenTest : MuseDashEditorTestScene
{
    [Test]
    public void TestProjectRowComponent()
    {
        var loadChartTask = TestResources.GetTestChartDirectory();
        AddUntilStep("Load test chart", () => loadChartTask.IsCompletedSuccessfully);
        AddStep("Add component", () =>
        {
            var storage = new NativeStorage(loadChartTask.Result.FullName, Host);
            Add(new ProjectRow(storage));
        });
    }
}
