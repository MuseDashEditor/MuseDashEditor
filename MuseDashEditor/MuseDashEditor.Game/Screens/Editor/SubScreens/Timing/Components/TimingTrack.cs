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
using MuseDashEditor.Game.Screens.Editor.Components;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;

public partial class TimingTrack : Container
{
    [Resolved] protected EditorDataHolder DataHolder { get; private set; } = null!;

    private ZoomableScrollContainer zoomableScrollContainer = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 250;
        Origin = Anchor.TopCentre;
        Anchor = Anchor.TopCentre;
        Position = new Vector2(0, 100);

        Masking = true;

        InternalChildren =
        [
            new Box
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Size = new Vector2(1, 250),
                Colour = Color4.White,
                Depth = 10
            },
            zoomableScrollContainer = new ZoomableScrollContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.X,
                Width = 0.9f,
                Height = 250,
                Depth = 20
            }
        ];

        Schedule(loadTrack);
    }

    private void loadTrack()
    {
        zoomableScrollContainer.Add(new WaveformGraph
        {
            RelativeSizeAxes = Axes.Both,
            Waveform = new Waveform(DataHolder.CurrentTrackStream.Value),
            HighColour = MdeColors.Light1,
            LowColour = MdeColors.Light2,
            MidColour = MdeColors.Light3,
            BaseColour = MdeColors.Light4,
            Depth = 10
        });
        zoomableScrollContainer.Add(new TimingTrackTickDisplay
        {
            ScrollContainer = zoomableScrollContainer,
            RelativeSizeAxes = Axes.Both,
            Depth = 0
        });
        zoomableScrollContainer.SetupZoom(100, 1, 500);
    }
}
