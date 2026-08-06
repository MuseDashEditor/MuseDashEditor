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
using System.Collections.Generic;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components.LaneObject;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose;

public partial class ComposeSubscreen : PlayableEditorSubscreen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        TimingTrack timingTrack;
        Container laneBackgrounds;

        InternalChildren =
        [
            laneBackgrounds = new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
                Depth = 1
            },
            timingTrack = new TimingTrack(-900)
            {
                AutoSizeAxes = Axes.Y,
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
                Depth = 0
            }
        ];

        foreach (var laneType in EditorConstants.ORDERED_LANE_TYPES)
        {
            laneBackgrounds.Add(new Box
            {
                RelativeSizeAxes = Axes.X,
                Height = EditorConstants.LANE_HEIGHT,
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
                Y = EditorConstants.GetLaneY(laneType),
                Alpha = 0.1f,
                Colour = MdeColors.GetLaneColor(laneType)
            });
        }

        timingTrack.ZoomableScrollContainer.Add(new LaneContentContainer
        {
            RelativeSizeAxes = Axes.X,
            Height = EditorConstants.TOTAL_LANES_HEIGHT,
            Origin = Anchor.CentreLeft,
            Anchor = Anchor.CentreLeft,
            ScrollContainer = timingTrack.ZoomableScrollContainer,
            Depth = -20,
        });

        timingTrack.ZoomableScrollContainer.Width = 1f;
        timingTrack.WaveformGraph.Alpha = 0; // TODO: add setting
        timingTrack.TimingTrackTickDisplay.Height = 1f;
        timingTrack.TimingTrackTickDisplay.ShouldPlayTickSound = false; // TODO: add setting

        ScrollContainer = timingTrack.ZoomableScrollContainer;
    }
}
