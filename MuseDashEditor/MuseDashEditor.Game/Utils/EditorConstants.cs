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

using System;
using System.Collections.Generic;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components.LaneObject;

namespace MuseDashEditor.Game.Utils;

public static class EditorConstants
{
    public const int PREVIEW_HIT_CIRCLE_X = -557;
    public const int PREVIEW_HIT_CIRCLE_Y = -14;
    public const int PREVIEW_TOP_LANE_Y = -166;
    public const int PREVIEW_BOTTOM_LANE_Y = 137;
    public const int PREVIEW_BOSS_LANE_Y = -14;

    public const float LANE_SPACING = 10;
    public const float LANE_HEIGHT = BaseLaneObject.BASE_SIZE + LANE_SPACING;
    public const float TOTAL_LANES_HEIGHT = LANE_HEIGHT * 6 + LANE_SPACING * 5;
    private const float semi_lane_spacing = LANE_SPACING / 2;
    private const float semi_lane_height = LANE_HEIGHT / 2;

    public const float AIR2_LANE_Y = AIR_LANE_Y - LANE_SPACING - LANE_HEIGHT;
    public const float AIR_LANE_Y = GROUND2_LANE_Y - LANE_SPACING - LANE_HEIGHT;
    public const float GROUND2_LANE_Y = -semi_lane_height - semi_lane_spacing;
    public const float GROUND_LANE_Y = semi_lane_height + semi_lane_spacing;
    public const float SPECIAL2_LANE_Y = GROUND_LANE_Y + LANE_SPACING + LANE_HEIGHT;
    public const float SPECIAL_LANE_Y = SPECIAL2_LANE_Y + LANE_SPACING + LANE_HEIGHT;

    public static readonly List<LaneType> ORDERED_LANE_TYPES =
    [
        LaneType.Air2, LaneType.Air,
        LaneType.Ground2, LaneType.Ground,
        LaneType.Special2, LaneType.Special,
    ];

    public static float GetLaneY(LaneType laneType)
    {
        return laneType switch
        {
            LaneType.Air2 => AIR2_LANE_Y,
            LaneType.Air => AIR_LANE_Y,
            LaneType.Ground2 => GROUND2_LANE_Y,
            LaneType.Ground => GROUND_LANE_Y,
            LaneType.Special2 => SPECIAL2_LANE_Y,
            LaneType.Special => SPECIAL_LANE_Y,
            _ => throw new ArgumentOutOfRangeException(nameof(laneType), laneType, null)
        };
    }
}
