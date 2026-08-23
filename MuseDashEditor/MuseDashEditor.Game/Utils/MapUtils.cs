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

using System.Collections.Generic;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Logging;
using osu.Framework.Utils;

namespace MuseDashEditor.Game.Utils;

public static class MapUtils
{
    public static void PreProcessMap(Map map)
    {
        var placing = new Dictionary<ObjectType, HashSet<(LaneType, LaneModifierType)>>();
        var gameObjects = map.GameObjects;

        for (var objectIndex = 0; objectIndex < gameObjects.Count; objectIndex++)
        {
            var gameObject = gameObjects[objectIndex];

            var objectType = gameObject.ObjectType;
            var isHoldType = objectType is ObjectType.Hold
                or ObjectType.Masher
                or ObjectType.BossMasher1
                or ObjectType.BossMasher2;
            var isLandmine = gameObject.LaneModifier == LaneModifierType.Landmine;

            if (isHoldType && !isLandmine)
            {
                var pairKey = (gameObject.LaneType, gameObject.LaneModifier);
                if (placing.GetValueOrDefault(objectType, []).Remove(pairKey))
                {
                    gameObject.IsHoldEnd = true;
                    continue;
                }

                placing.ComputeIfAbsent(objectType, () => []).Add(pairKey);

                var nextObject = getNextObjectOfType(gameObjects, objectType, objectIndex, gameObject.LaneType,
                    gameObject.LaneModifier);
                if (nextObject == null)
                {
                    // TODO: popup for the user : the imported map has issues
                    Logger.Log("Cannot find next object of type " + objectType, level: LogLevel.Error);
                    continue;
                }

                gameObject.HoldEndObject = nextObject;
            }

            if (objectType == ObjectType.Gemini)
            {
                var otherGemini = findOtherGemini(gameObjects, gameObject);
                if (otherGemini == null)
                {
                    // TODO: popup for the user : the imported map has issues
                    Logger.Log("Cannot find pairing gemini", level: LogLevel.Error);
                    continue;
                }

                gameObject.GeminiPairObject = otherGemini;
            }
        }
    }

    private static GameObject? getNextObjectOfType(List<GameObject> gameObjects, ObjectType objectType, int startIndex,
        LaneType laneType, LaneModifierType laneModifier)
    {
        for (var index = startIndex + 1; index < gameObjects.Count; index++)
        {
            var gameObject = gameObjects[index];

            if (gameObject.ObjectType != objectType
                || gameObject.LaneType != laneType
                || gameObject.LaneModifier != laneModifier)
                continue;

            return gameObject;
        }

        return null;
    }

    private static GameObject? findOtherGemini(List<GameObject> gameObjects, GameObject gameObject)
    {
        var offsetValue = gameObject.Offset.Value;
        var isAir = gameObject.LaneType is LaneType.Air or LaneType.Air2;

        foreach (var other in gameObjects)
        {
            if (other.ObjectType != ObjectType.Gemini)
                continue;

            if (!Precision.AlmostEquals(offsetValue, other.Offset.Value, 1E-2)) // Should be equal
                continue;

            var isOtherAir = other.LaneType is LaneType.Air or LaneType.Air2;
            if (isAir && isOtherAir || !isAir && !isOtherAir)
                continue;

            return other;
        }

        return null;
    }
}
