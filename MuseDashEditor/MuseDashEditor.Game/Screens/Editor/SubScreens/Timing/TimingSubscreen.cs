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

using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Timing;

public partial class TimingSubscreen : PlayableEditorSubscreen
{
    [Resolved] private EditorDataHolder dataHolder { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        var timingTrack = new TimingTrack
        {
            Height = 250,
            Origin = Anchor.TopCentre,
            Anchor = Anchor.TopCentre,
            Position = new Vector2(0, 100)
        };

        Children =
        [
            // Background
            new Box
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Colour = MdeColors.Background6,
                Depth = float.MaxValue
            },
            timingTrack,
            new TimingPointsTable()
        ];

        ScrollContainer = timingTrack.ZoomableScrollContainer;
    }

    public override void Show()
    {
        base.Show();

        var currentTrackValue = dataHolder.CurrentTrack.Value;
        if (currentTrackValue == null) return;
        currentTrackValue.Volume.Value = 0.5f;
    }

    public override void Hide()
    {
        base.Hide();

        var currentTrackValue = dataHolder.CurrentTrack.Value;
        if (currentTrackValue == null) return;
        currentTrackValue.Volume.Value = 1;
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (!e.ControlPressed)
            return base.OnKeyDown(e);

        var amount = e.AltPressed ? 1 : 10;
        var currentTime = EditorClock.CurrentTime;

        switch (e.Key)
        {
            case Key.Left:
                ScrollContainer?.ScrollToTime(currentTime - amount);
                break;
            case Key.Right:
                ScrollContainer?.ScrollToTime(currentTime + amount);
                break;
            default: return base.OnKeyDown(e);
        }

        return true;
    }
}
