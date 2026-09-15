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
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace MuseDashEditor.Game.Component.Common.Menu.Item;

public class ActionMenuItem(LocalisableString text, Action action, MenuItemType itemType = MenuItemType.Standard) : MenuItem(text, action)
{
    public MenuItemType Type => itemType;
}

public partial class ActionDrawableMenuItem(ActionMenuItem item) : BaseDrawableMenuItem(item)
{
    public new ActionMenuItem Item = item;

    protected override Drawable CreateContent() => Text = new TextContainer
    {
        Colour = Item.Type switch
        {
            MenuItemType.Highlighted => Colour4.Gold,
            MenuItemType.Destructive => Colour4.Red,
            _ => Colour4.White
        }
    };
}
