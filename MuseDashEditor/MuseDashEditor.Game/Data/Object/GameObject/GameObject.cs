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

using MuseDashEditor.Game.Data.Object.DesignObject;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components.LaneObject;
using MuseDashEditor.Game.Utils;

namespace MuseDashEditor.Game.Data.Object.GameObject;

public class GameObject : BaseObject
{
    public GameObject(double offset,
                      ObjectType objectType,
                      LaneType laneType,
                      LaneModifierType laneModifier)
        : base(offset)
    {
        ObjectType = objectType;
        LaneType = laneType;
        LaneModifier = laneModifier;

        GameObjectData = GameObjectUtils.GetGameObjectData(ObjectType);
        DesignObjectData = GameObjectUtils.GetDesignObjectData(ObjectType);
    }

    public ObjectType ObjectType { get; }
    public LaneType LaneType { get; set; }
    public LaneModifierType LaneModifier { get; set; }

    public readonly GameObjectData? GameObjectData;
    public readonly DesignObjectData? DesignObjectData;

    public GameObject? HoldEndObject;
    public GameObject? GeminiPairObject;
    public bool IsHoldEnd = false;

    public BaseLaneObject? LaneObject;
}
