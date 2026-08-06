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

using System;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace MuseDashEditor.Game.Screens.Open.Components;

public partial class ProjectRowButton : Container, IHasTooltip
{
    public required IconUsage Icon { get; init; }
    public required Action ActionOnClick { get; init; }
    public Colour4? IconColour { get; init; }
    public LocalisableString TooltipText { get; init; }
    private SpriteIcon icon = null!;
    private Box hoverBox = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(50, 50);

        Children =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = MdeColors.Background5
            },
            icon = new SpriteIcon
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(0.6f),
                Icon = Icon,
                Colour = IconColour ?? Colour4.White,
                Alpha = 0.5f
            },
            hoverBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = Colour4.White,
                Alpha = 0
            },
        ];
    }

    protected override bool OnHover(HoverEvent e)
    {
        hoverBox.TransformTo("Alpha", 0.1f, 200);
        icon.TransformTo("Alpha", 1f, 200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hoverBox.TransformTo("Alpha", 0f, 200);
        icon.TransformTo("Alpha", 0.5f, 200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        ActionOnClick();
        return true;
    }
}
