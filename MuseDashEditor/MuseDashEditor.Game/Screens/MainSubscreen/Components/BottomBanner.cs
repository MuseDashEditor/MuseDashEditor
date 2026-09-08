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

using MuseDashEditor.Game.Component.Common;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osuTK;

namespace MuseDashEditor.Game.Screens.MainSubscreen.Components;

public partial class BottomBanner : Container
{
    [Resolved]
    protected ScreenStack MainScreenStack { get; private set; } = null!;

    public Drawable? Addition { get; init; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Children =
        [
            new Box
            {
                Colour = MdeColors.Background4,
                RelativeSizeAxes = Axes.Both
            },
            new Container
            {
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Padding = new MarginPadding(20),
                Children =
                [
                    new RoundedButton
                    {
                        Text = "Back",
                        FontUsage = MDEFonts.ArialUnicodeMs.With(size: 30),
                        Size = new Vector2(150, 60),
                        BackgroundColour = MdeColors.Background5,
                        Action = () =>
                        {
                            MainScreenStack.CurrentScreen.Exit();
                        }
                    }
                ]
            }
        ];

        if (Addition != null)
        {
            Add(new Container
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                AutoSizeAxes = Axes.Both,
                Padding = new MarginPadding(20),
                Child = Addition
            });
        }
    }
}
