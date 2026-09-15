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
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using osuTK;

namespace MuseDashEditor.Game.Component.Common.Menu.Item;

public class SubMenuMenuItem(LocalisableString text) : MenuItem(text);

public partial class SubMenuDrawableMenuItem(SubMenuMenuItem item) : BaseDrawableMenuItem(item)
{
    protected override Drawable CreateContent()
    {
        Text = new TextContainer();

        Text.Container.Padding = new MarginPadding
        {
            Left = MARGIN_HORIZONTAL,
            Right = MARGIN_HORIZONTAL + 15,
            Vertical = MARGIN_VERTICAL,
        };
        Text.Add(new SpriteIcon
        {
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            Icon = FontAwesome.Solid.ChevronRight,
            Size = new Vector2(10)
        });

        return Text;
    }
}
