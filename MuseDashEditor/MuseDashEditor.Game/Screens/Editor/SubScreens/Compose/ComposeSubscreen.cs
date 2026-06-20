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

using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose;

public partial class ComposeSubscreen : PlayableEditorSubscreen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        TimingTrack timingTrack;

        InternalChildren =
        [
            timingTrack = new TimingTrack(EditorConstants.HIT_CIRCLE_X)
            {
                Height = 450,
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
                Y = EditorConstants.HIT_CIRCLE_Y
            },
            new LaneCircleContainer
            {
                Depth = -10
            }
        ];

        (LaneType[], int)[] types =
        [
            ([LaneType.Air, LaneType.Air2], EditorConstants.TOP_LANE_Y),
            ([LaneType.Ground, LaneType.Ground2], EditorConstants.BOTTOM_LANE_Y),
            ([LaneType.Special, LaneType.Special2], EditorConstants.SPECIAL_LANE_Y),
        ];
        foreach (var (laneTypes, laneY) in types)
        {
            timingTrack.ZoomableScrollContainer.Add(new LaneContentContainer(laneTypes)
            {
                ScrollContainer = timingTrack.ZoomableScrollContainer,
                Depth = -20,
                RelativeSizeAxes = Axes.X,
                Width = 1f,
                Height = 100,
                Y = laneY - EditorConstants.HIT_CIRCLE_Y
            });
        }

        timingTrack.ZoomableScrollContainer.Width = 1f;
        timingTrack.WaveformGraph.Alpha = 0.3f;
        timingTrack.TimingTrackTickDisplay.Height = 1f;
        timingTrack.TimingTrackTickDisplay.ShouldPlayTickSound = false;

        ScrollContainer = timingTrack.ZoomableScrollContainer;
    }
}
