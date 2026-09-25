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

using MuseDashEditor.Game.Editor.Clock;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK.Graphics;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class PlayBarSlider : Container
{
    [Resolved]
    private EditorClock editorClock { get; set; } = null!;

    private readonly BindableDouble currentTrackTime = new();

    private Box cursor = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        currentTrackTime.BindTo(editorClock.CurrentTimeBindable);

        Children =
        [
            new Box
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.X,
                Height = 5,
                Colour = Colour4.White
            },
            cursor = new Box
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Y,
                Height = 0.8f,
                Width = 2,
                Colour = Color4.LimeGreen
            }
        ];

        currentTrackTime.BindValueChanged(@event =>
        {
            cursor.X = (float)(@event.NewValue / currentTrackTime.MaxValue * DrawWidth);
        });
    }

    protected override bool OnClick(ClickEvent e)
    {
        editorClock.Seek(e.MousePosition.X / DrawWidth * currentTrackTime.MaxValue);
        return true;
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        editorClock.Stop();
        return true;
    }

    protected override void OnDrag(DragEvent e)
    {
        editorClock.Seek(e.MousePosition.X / DrawWidth * currentTrackTime.MaxValue);
    }
}
