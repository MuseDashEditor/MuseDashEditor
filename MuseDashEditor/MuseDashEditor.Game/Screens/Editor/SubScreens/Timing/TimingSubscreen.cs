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

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Timing;

public partial class TimingSubscreen : PlayableEditorSubscreen
{
    [Resolved] private EditorDataHolder dataHolder { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
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

            new TimingTrack(),
            new TimingPointsTable()
        ];
    }

    public override void Show()
    {
        base.Show();

        var currentTrackValue = dataHolder.CurrentTrack.Value;
        if (currentTrackValue == null) return;
        currentTrackValue.Volume.Value = 0.6;
    }

    public override void Hide()
    {
        base.Hide();

        var currentTrackValue = dataHolder.CurrentTrack.Value;
        if (currentTrackValue == null) return;
        currentTrackValue.Volume.Value = 1;
    }
}
