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

using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace MuseDashEditor.Game;

public partial class MuseDashEditorGame : MuseDashEditorGameBase
{
    // Editor data
    [Cached] protected readonly EditorDataHolder DataHolder = new();

    // UI data
    [Cached] protected readonly ScreenStack ScreenStack = new() { RelativeSizeAxes = Axes.Both };

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = ScreenStack;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        ScreenStack.Push(new MainScreen());
    }
}
