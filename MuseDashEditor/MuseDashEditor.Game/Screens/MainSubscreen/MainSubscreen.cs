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

using MuseDashEditor.Game.Component.Notification;
using MuseDashEditor.Game.Input;
using MuseDashEditor.Game.Screens.MainSubscreen.Components;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Framework.Screens;

namespace MuseDashEditor.Game.Screens.MainSubscreen;

public partial class MainSubscreen : Screen, IKeyBindingHandler<InputAction>
{
    [Cached]
    private NotificationContainer notificationContainer = new();

    public required IconUsage Icon { get; init; }
    public required LocalisableString Text { get; init; }
    public Drawable? Addition { get; set; } = null;

    public Container Content { get; } = new()
    {
        RelativeSizeAxes = Axes.X,
        Height = 1080 - 240,
        Width = 0.9f,
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
    };

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = MdeColors.Background6
            },
            new TopBanner
            {
                RelativeSizeAxes = Axes.X,
                Height = 120,

                Icon = Icon,
                Text = Text,
                Addition = Addition
            },
            new BottomBanner
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                RelativeSizeAxes = Axes.X,
                Height = 100
            },
            Content,
            notificationContainer
        ];
    }

    public bool OnPressed(KeyBindingPressEvent<InputAction> e)
    {
        if (e.Action != InputAction.Escape)
            return false;

        this.Exit();

        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<InputAction> e)
    {
    }
}
