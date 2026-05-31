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

using System.Collections.Generic;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Screens.Editor.Components;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Logging;
using osuTK;
using osuTK.Graphics;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;

public partial class TimingTrack : Container
{
    [Resolved] protected EditorDataHolder DataHolder { get; private set; } = null!;
    [Resolved] protected EditorClock EditorClock { get; private set; } = null!;

    private readonly List<Box> timingPointBoxes = [];
    private ZoomableScrollContainer zoomableScrollContainer;

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
        zoomableScrollContainer.SetupZoom(100, 1, 500);
        zoomableScrollContainer.OnDrawWidthChanged += () =>
        {
            Logger.Log("TimingTrack: Draw width changed, reloading timing points");

            // TODO: This is a very basic approach, it will generate a lot of drawables
            // As osu!, we should:
            // 1. Use FastCircle instead of Box
            // 2. Not generate drawables that are not visible
            // 3. Cache drawables and reuse them
            // 4. Update the drawables when the time change

            foreach (var timingPointBox in timingPointBoxes)
            {
                timingPointBox.RemoveAndDisposeImmediately();
            }

            timingPointBoxes.Clear();

            var timingPoints = DataHolder.CurrentMap.Value.TimingPoints;

            // Timing change points
            foreach (var timingPointObject in timingPoints)
            {
                float pixelOffset = zoomableScrollContainer.PositionAtTime(timingPointObject.Offset);

                var timingPointBox = new Box
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.CentreLeft,
                    Size = new Vector2(5, 250),
                    Position = new Vector2(pixelOffset, 0),
                    Colour = Color4.Red,
                    Depth = 0
                };
                timingPointBoxes.Add(timingPointBox);
                zoomableScrollContainer.Add(timingPointBox);
            }

            // Timing bars | white = beat, blue = subdivision
            for (var index = 0; index < timingPoints.Count; index++)
            {
                var timingPointObject = timingPoints[index];

                double startOffset = timingPointObject.Offset;
                double nextOffset = index == timingPoints.Count - 1
                    ? EditorClock.TrackLength
                    : timingPoints[index + 1].Offset;

                double beatLength = 60_000 / timingPointObject.NewBpm;

                for (var currentOffset = startOffset;
                     currentOffset < nextOffset;
                     currentOffset += beatLength)
                {
                    var pixelOffset = zoomableScrollContainer.PositionAtTime(currentOffset);

                    var timingPointBox = new Box
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.CentreLeft,
                        Size = new Vector2(5, 200),
                        Position = new Vector2(pixelOffset, 0),
                        Colour = Color4.White,
                        Depth = 0
                    };
                    timingPointBoxes.Add(timingPointBox);
                    zoomableScrollContainer.Add(timingPointBox);

                    // TODO: type of subdivision (4/4, 3/4, 6/8, etc.)
                    for (var currentSubdivision = 1; currentSubdivision < 4; currentSubdivision++)
                    {
                        pixelOffset = zoomableScrollContainer.PositionAtTime(currentOffset + beatLength / 4 * currentSubdivision);

                        timingPointBox = new Box
                        {
                            Origin = Anchor.Centre,
                            Anchor = Anchor.CentreLeft,
                            Size = new Vector2(3, 200),
                            Position = new Vector2(pixelOffset, 0),
                            Colour = Color4.Blue,
                            Depth = 0
                        };
                        timingPointBoxes.Add(timingPointBox);
                        zoomableScrollContainer.Add(timingPointBox);
                    }
                }
            }
        };
    }
}
