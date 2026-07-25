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

using MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;
using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace MuseDashEditor.Game.Tests.Tests.Editor.Subscreens.Timing.Components;

public partial class AdjustableInputTest : MuseDashEditorTestScene
{
    [Test]
    public void Test()
    {
        var bindableValue = new BindableDouble();

        Add(new AdjustableInput
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,

            Value = bindableValue,
            MinValue = 0,
            MaxValue = 100,

            SmallStep = 1,
            LargeStep = 10
        });
    }
}
