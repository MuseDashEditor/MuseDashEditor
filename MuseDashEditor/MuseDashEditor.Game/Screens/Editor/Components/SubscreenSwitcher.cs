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
using MuseDashEditor.Game.Screens.Editor.SubScreens;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class SubscreenSwitcher : TabControl<EditorSubscreenType>
{
    [BackgroundDependencyLoader]
    private void load(EditorDataHolder editorDataHolder)
    {
        Current = editorDataHolder.SelectedSubscreen;
        SelectFirstTabByDefault = false;
        Origin = Anchor.CentreRight;
        Anchor = Anchor.CentreRight;
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Height = 1.0f;
        TabContainer.RelativeSizeAxes &= ~Axes.X;
        TabContainer.AutoSizeAxes = Axes.X;

        var values = (EditorSubscreenType[])Enum.GetValues(typeof(EditorSubscreenType));
        Array.Reverse(values);

        foreach (var val in values)
            AddItem(val);
    }

    protected override Dropdown<EditorSubscreenType> CreateDropdown()
    {
        return null; // No dropdown
    }

    protected override TabItem<EditorSubscreenType> CreateTabItem(EditorSubscreenType value)
    {
        return new BasicTabControl<EditorSubscreenType>.BasicTabItem(value)
        {
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            AutoSizeAxes = Axes.X,
            RelativeSizeAxes = Axes.Y,

            Children =
            [
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(1, 1),
                    Colour = MdeColors.Background4
                },
                new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = value.ToString(),
                    Font = FontUsage.Default.With(size: 20),
                    Colour = Colour4.White,
                    Margin = new MarginPadding(10)
                }
            ]
        };
    }
}
