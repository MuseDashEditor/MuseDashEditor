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
using MuseDashEditor.Game.Data.Holder;
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
    private SpriteText percentText;
    private SpriteText timerText;
    private BasicSliderBar<float> slider;

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder, EditorClock clock)
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

            // Text
            timerText = new SpriteText
            {
                Text = "00:00.0000",
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                Position = new Vector2(125, 17.5f),
                Font = FontUsage.Default.With(size: 28f)
            },
            percentText = new SpriteText
            {
                Text = "0.00 %",
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                Position = new Vector2(125, -12.5f),
                Font = FontUsage.Default.With(size: 28f)
            },

            // Slider
            slider = new BasicSliderBar<float>
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(1320, 20),

                BackgroundColour = MdeColors.Dark4,
                SelectionColour = MdeColors.Dark2,
                FocusColour = MdeColors.Dark1,

                Current = new BindableNumber<float> { MinValue = 0, MaxValue = 1, Precision = .01f }
            }

            // Play button
            // TODO
        ];

        clock.OnTimeChanged += time =>
        {
            double minutes = Math.Floor(time / 60000);
            double seconds = Math.Floor(time / 1000) % 60;
            double miliseconds = Math.Floor(time % 1000);

            var timerString = $"{minutes:00}:{seconds:00}.{miliseconds:000}";
            timerText.Text = timerString;

            percentText.Text = $"{(time / clock.TrackLength) * 100:0.00} %";
            var sliderValue = (float)(time / clock.TrackLength);
            if (sliderValue < 0) sliderValue = 0;
            if (sliderValue > 1) sliderValue = 1;
            slider.Current.Value = sliderValue;
        };
    }
}
