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

using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components;

public partial class LaneCircleContainer : Container<LaneCircle>
{
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Children =
        [
            new LaneCircle
            {
                InnerColor = MdeColors.TopLaneColor,
                Position = new Vector2(EditorConstants.HIT_CIRCLE_X, EditorConstants.TOP_LANE_Y),
            },
            new LaneCircle
            {
                InnerColor = MdeColors.BottomLaneColor,
                Position = new Vector2(EditorConstants.HIT_CIRCLE_X, EditorConstants.BOTTOM_LANE_Y),
            },
            new LaneCircle
            {
                InnerColor = MdeColors.BossLaneColor,
                Position = new Vector2(EditorConstants.HIT_CIRCLE_X, EditorConstants.BOSS_LANE_Y),
            },
        ];
    }
}
