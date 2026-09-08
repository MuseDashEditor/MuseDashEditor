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

using MuseDashEditor.Game.Component.Common;
using MuseDashEditor.Game.Screens.MainSubscreen.New;
using MuseDashEditor.Game.Screens.MainSubscreen.Open;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Screens;
using osuTK;

namespace MuseDashEditor.Game.Screens;

public partial class MainScreen : Screen
{
    [Resolved]
    protected ScreenStack MainScreenStack { get; private set; } = null!;

    [BackgroundDependencyLoader]
    private void load(LargeTextureStore textures)
    {
        InternalChildren =
        [
            new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(1, 1),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0.3f,
                Texture = textures.Get("default_background")
            },
            new Sprite
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = 100,
                Width = 640,
                Height = 256,
                Texture = textures.Get("title")
            },
            new FillFlowContainer
            {
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 10),
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children =
                [
                    new RoundedButton
                    {
                        Text = "New project",
                        FontUsage = MDEFonts.ArialUnicodeMs.With(size: 40),
                        Size = new Vector2(300, 80),
                        BackgroundColour = MdeColors.Background3,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Action = () => MainScreenStack.Push(NewChartScreen.CreateInstance())
                    },
                    new RoundedButton
                    {
                        Text = "Open project",
                        FontUsage = MDEFonts.ArialUnicodeMs.With(size: 40),
                        Size = new Vector2(300, 80),
                        BackgroundColour = MdeColors.Background3,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Action = () => MainScreenStack.Push(ProjectListScreen.CreateInstance())
                    },
                    new RoundedButton // TODO merge into open
                    {
                        Text = "Import project",
                        FontUsage = MDEFonts.ArialUnicodeMs.With(size: 40),
                        Size = new Vector2(300, 80),
                        BackgroundColour = MdeColors.Background3,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Action = () => MainScreenStack.Push(new FolderSelectorScreen())
                    },
                    new RoundedButton
                    {
                        Text = "Settings",
                        FontUsage = MDEFonts.ArialUnicodeMs.With(size: 40),
                        Size = new Vector2(300, 80),
                        BackgroundColour = MdeColors.Background3,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Action = () => { }
                    }
                ]
            },
            new SpriteText
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Font = MDEFonts.ArialUnicodeMs.With(size: 20),
                Alpha = 0.8f,
                Text = "Not affiliated with, endorsed by, or sponsored by PeroPeroGames. MuseDash and all related assets belong to their respective owners."
            }
        ];
    }
}
