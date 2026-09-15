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
using MuseDashEditor.Game.Component.Common.Menu.Item;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using FrameworkUserInterface = osu.Framework.Graphics.UserInterface;

namespace MuseDashEditor.Game.Component.Common.Menu;

public partial class MdeMenu : FrameworkUserInterface.Menu
{
    protected const double DELAY_BEFORE_FADE_OUT = 50;
    protected const double FADE_DURATION = 280;

    public MdeMenu(Direction direction = Direction.Vertical, bool topLevelMenu = false)
        : base(direction, topLevelMenu)
    {
        if (topLevelMenu)
        {
            RelativeSizeAxes = Axes.Y;
            ContentContainer.Masking = true;
        }

        MaskingContainer.CornerRadius = 0;
        ItemsContainer.Padding = new MarginPadding();
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        BackgroundColour = MdeColors.Background4;
    }

    protected override void Update()
    {
        base.Update();

        bool showCheckboxes = false;

        foreach (var drawableItem in ItemsContainer)
        {
            // TODO
            // if (drawableItem.Item is StatefulMenuItem)
            // showCheckboxes = true;
        }

        foreach (var drawableItem in ItemsContainer)
        {
            // TODO
            // if (drawableItem is DrawableOsuMenuItem osuItem)
            // osuItem.ShowCheckbox.Value = showCheckboxes;
        }
    }

    protected override void AnimateOpen()
    {
        this.FadeIn(FADE_DURATION, Easing.OutQuint);
    }

    protected override void AnimateClose()
    {
        this.Delay(DELAY_BEFORE_FADE_OUT)
            .FadeOut(FADE_DURATION, Easing.OutQuint);
    }

    protected override void UpdateSize(Vector2 newSize)
    {
        if (Direction == Direction.Vertical)
        {
            Width = newSize.X;

            if (newSize.Y > 0)
                this.ResizeHeightTo(newSize.Y, 300, Easing.OutQuint);
            else
                // Delay until the fade out finishes from AnimateClose.
                this.Delay(DELAY_BEFORE_FADE_OUT + FADE_DURATION).ResizeHeightTo(0);
        }
        else
        {
            Height = newSize.Y;
            if (newSize.X > 0)
                this.ResizeWidthTo(newSize.X, 300, Easing.OutQuint);
            else
                // Delay until the fade out finishes from AnimateClose.
                this.Delay(DELAY_BEFORE_FADE_OUT + FADE_DURATION).ResizeWidthTo(0);
        }
    }

    protected override FrameworkUserInterface.Menu CreateSubMenu() => new MdeMenu
    {
        Anchor = Direction == Direction.Horizontal ? Anchor.BottomLeft : Anchor.TopRight
    };

    protected override DrawableMenuItem CreateDrawableMenuItem(FrameworkUserInterface.MenuItem item)
    {
        return item switch
        {
            TopMenuMenuItem topMenuMenuItem => new TopMenuDrawableMenuItem(topMenuMenuItem),
            ActionMenuItem actionMenuItem => new ActionDrawableMenuItem(actionMenuItem),
            SubMenuMenuItem subMenuMenuItem => new SubMenuDrawableMenuItem(subMenuMenuItem),
            SeparatorMenuItem separatorMenuItem => new SeparatorDrawableMenuItem(separatorMenuItem),
            _ => throw new ArgumentOutOfRangeException(nameof(item))
        };
    }

    protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new MenuScrollContainer(direction);
}
