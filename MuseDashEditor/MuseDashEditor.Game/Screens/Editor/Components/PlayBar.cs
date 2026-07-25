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
using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class PlayBar : CompositeDrawable
{
    [Resolved] protected EditorClock EditorClock { get; private set; } = null!;

    private SpriteText percentText = null!;
    private SpriteText timerText = null!;
    private BasicSliderBar<double> slider = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 65;
        Width = 1.0f;
        Origin = Anchor.BottomLeft;
        Anchor = Anchor.BottomLeft;

        InternalChildren =
        [
            // Background
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(1f, 1f),
                Colour = MdeColors.Background5
            },

            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                Content = new[]
                {
                    new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Direction = FillDirection.Vertical,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Children =
                            [
                                timerText = new SpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Text = "00:00.0000",
                                    Font = FontUsage.Default.With(size: 28f),
                                },
                                percentText = new SpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Text = "0.00 %",
                                    Font = FontUsage.Default.With(size: 28f)
                                }
                            ]
                        },
                        slider = new BasicSliderBar<double>
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(1320, 20),

                            BackgroundColour = MdeColors.Dark4,
                            SelectionColour = MdeColors.Dark2,
                            FocusColour = MdeColors.Dark1,

                            Current = new BindableDouble
                            {
                                MinValue = 0,
                                MaxValue = 1,
                                Value = 0,
                                Precision = 1
                            }
                        },
                        new Box()
                    }
                },
                RowDimensions = [new Dimension(GridSizeMode.Relative, 1)],
                ColumnDimensions =
                [
                    new Dimension(GridSizeMode.Absolute, 125),
                    new Dimension(),
                    new Dimension(GridSizeMode.Absolute, 125)
                ]
            }
        ];

        slider.Current.BindTo(EditorClock.CurrentTimeBindable);

        EditorClock.OnTimeChanged += onClockTimeChanged;
        onClockTimeChanged(0);
    }

    private void onClockTimeChanged(double time)
    {
        double minutes = Math.Floor(time / 60000);
        double seconds = Math.Floor(time / 1000) % 60;
        double miliseconds = Math.Floor(time % 1000);

        var timerString = $"{minutes:00}:{seconds:00}.{miliseconds:000}";
        timerText.Text = timerString;

        var editorClockTrackLength = EditorClock.TrackLength;
        if (editorClockTrackLength == 0) editorClockTrackLength = 1; // Prevent division by zero
        percentText.Text = $"{time / editorClockTrackLength * 100:0.00} %";
    }
}
