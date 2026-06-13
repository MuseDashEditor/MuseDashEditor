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

using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;

public sealed partial class TimingPointTick : FastCircle
{
    public const float MAX_WIDTH = 8f;
    public const float TIMING_CHANGE_WIDTH = MAX_WIDTH;
    public const float FIRST_BEAT_POINT_WIDTH = MAX_WIDTH / 2f;
    public const float SUB_BEAT_POINT_WIDTH = MAX_WIDTH / 4f;

    public bool ShouldPlaySound { get; set; }
    public double Offset { get; set; }

    public TimingPointTick()
    {
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;

        // Changed based on the tick type (bar, beat, subbeat)
        Width = MAX_WIDTH;
        Height = 1f;
        Colour = Color4.White;
    }
}
