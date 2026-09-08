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

using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace MuseDashEditor.Game.Screens.MainSubscreen.Components;

public partial class TopBanner : Container
{
    public required IconUsage Icon { get; init; }
    public required LocalisableString Text { get; init; }
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
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Padding = new MarginPadding(20),
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(20),
                Children =
                [
                    new SpriteIcon
                    {
                        Icon = Icon,
                        Margin = new MarginPadding(5),
                        Size = new Vector2(60, 60),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    },
                    new SpriteText
                    {
                        Margin = new MarginPadding(5),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Font = MDEFonts.Impact.With(size: 60),
                        Text = Text
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
