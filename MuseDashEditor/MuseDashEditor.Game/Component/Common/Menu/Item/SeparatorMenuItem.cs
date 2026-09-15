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

using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace MuseDashEditor.Game.Component.Common.Menu.Item;

public class SeparatorMenuItem() : MenuItem("");

public sealed partial class SeparatorDrawableMenuItem : BaseDrawableMenuItem
{
    public SeparatorDrawableMenuItem(SeparatorMenuItem item)
        : base(item)
    {
        Scale = new Vector2(1, 0.6f);

        AddInternal(new Box
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Colour = BackgroundColourHover,
            RelativeSizeAxes = Axes.X,
            Height = 2f,
            Width = 0.9f,
        });
    }

    protected override bool OnHover(HoverEvent e) => true;

    protected override bool OnClick(ClickEvent e) => true;
}
