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

using MuseDashEditor.Game.Screens.Open;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;

namespace MuseDashEditor.Game.Screens;

public partial class MainScreen : Screen
{
    [Resolved] protected ScreenStack MainScreenStack { get; private set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            new BasicButton
            {
                Text = "Open chart",
                Size = new Vector2(200, 50),
                Colour = Colour4.AliceBlue,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Action = () => MainScreenStack.Push(new FolderSelectorScreen())
            }
        ];
    }
}
