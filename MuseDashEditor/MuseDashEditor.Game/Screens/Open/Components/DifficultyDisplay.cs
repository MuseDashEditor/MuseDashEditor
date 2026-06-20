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
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace MuseDashEditor.Game.Screens.Open.Components;

public partial class DifficultyDisplay : BasicButton
{
    public ColourInfo StarColour { get; init; }
    public DifficultyType Difficulty { get; init; }
    public Action OnClickAction { get; init; } = () => { };

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder)
    {
        Margin = new MarginPadding(50);
        AutoSizeAxes = Axes.Both;
        Colour = Colour4.White;

        InternalChild = new FillFlowContainer
        {
            Direction = FillDirection.Vertical,
            AutoSizeAxes = Axes.Both,

            Children =
            [
                new Box
                {
                    Size = new Vector2(100, 100),
                    Colour = StarColour,
                    Margin = new MarginPadding(10)
                },
                new SpriteText
                {
                    Text = "DIFF NAME",
                    Font = FontUsage.Default.With(size: 30)
                }
            ]
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        OnClickAction?.Invoke();
        return true;
    }
}
