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

    public readonly ZoomableScrollContainer ZoomableScrollContainer;
    public readonly WaveformGraph WaveformGraph;
    public readonly TimingTrackTickDisplay TimingTrackTickDisplay;

    private readonly float xCenter;

    public TimingTrack(float xCenter = 0f)
    {
        this.xCenter = xCenter;

        ZoomableScrollContainer = new ZoomableScrollContainer(xCenter: xCenter)
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            RelativeSizeAxes = Axes.Both,
            Width = 0.9f,
            Height = 1f,
            Depth = 20
        };
        WaveformGraph = new WaveformGraph
        {
            RelativeSizeAxes = Axes.Both,
            HighColour = MdeColors.Light1,
            LowColour = MdeColors.Light2,
            MidColour = MdeColors.Light3,
            BaseColour = MdeColors.Light4,
            Depth = 10
        };
        TimingTrackTickDisplay = new TimingTrackTickDisplay
        {
            ScrollContainer = ZoomableScrollContainer,
            RelativeSizeAxes = Axes.Both,
            Depth = 0
        };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;

        Masking = true;

        InternalChildren =
        [
            new Box
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Size = new Vector2(1, 250),
                Colour = Color4.White,
                Depth = 10,
                X = xCenter
            },
            ZoomableScrollContainer
        ];

        Schedule(loadTrack);
    }

    private void loadTrack()
    {
        WaveformGraph.Waveform = new Waveform(DataHolder.CurrentTrackStreamGetter.Value());

        ZoomableScrollContainer.Add(WaveformGraph);
        ZoomableScrollContainer.Add(TimingTrackTickDisplay);
        ZoomableScrollContainer.SetupZoom(100, 1, 500); // TODO
    }
}
