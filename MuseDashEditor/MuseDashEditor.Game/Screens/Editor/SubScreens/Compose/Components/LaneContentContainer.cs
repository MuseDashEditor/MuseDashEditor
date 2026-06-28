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
using System.Linq;
using MuseDashEditor.Game.Component;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components.LaneObject;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components;

public partial class LaneContentContainer(LaneType[] laneTypes) : AutoRefreshContainer<BaseLaneObject>(50)
{
    [Resolved] private EditorDataHolder dataHolder { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
    }

    protected override void RegenerateContent()
    {
        generateBasicObjects();
        generateHoldObjects();
        generateGeminiObjects();
    }

    private void generateBasicObjects()
    {
        foreach (var gameObject in dataHolder.CurrentMap.Value.GameObjects)
        {
            if (!laneTypes.Contains(gameObject.LaneType))
                continue;

            var objectType = gameObject.ObjectType;
            if (objectType is ObjectType.Hold or ObjectType.Gemini or ObjectType.Masher or ObjectType.BossMasher1
                or ObjectType.BossMasher2)
            {
                if (gameObject.LaneModifier != LaneModifierType.Landmine)
                    continue;
            }

            var gameObjectData = gameObject.GameObjectData;
            if (gameObjectData == null)
                continue;

            var tickPosition = ScrollContainer.PositionAtTime(gameObject.Offset);

            if (tickPosition < CurrentMinRange)
            {
                if (NextMinTick == null || tickPosition > NextMinTick)
                    NextMinTick = tickPosition;

                continue;
            }

            if (tickPosition > CurrentMaxRange)
            {
                if (NextMaxTick == null || tickPosition < NextMaxTick)
                    NextMaxTick = tickPosition;

                continue;
            }

            var laneObject = getOrCreateObject();
            laneObject.X = tickPosition;

            laneObject.GameObject = gameObject;
            laneObject.SceneType = SceneType.SpaceStation; // TODO: scene at time
            laneObject.MovementType = gameObjectData.MovementType;
            laneObject.LaneType = gameObject.LaneType;
            laneObject.LaneModifier = gameObject.LaneModifier;
        }
    }

    private void generateHoldObjects()
    {
        generateHoldObjectsOfType(ObjectType.Hold);
        generateHoldObjectsOfType(ObjectType.Masher);
        generateHoldObjectsOfType(ObjectType.BossMasher1);
        generateHoldObjectsOfType(ObjectType.BossMasher2);
    }

    private void generateHoldObjectsOfType(ObjectType allowedObjectType)
    {
        var gameObjects = dataHolder.CurrentMap.Value.GameObjects;
        var isPlacing = false;

        for (var index = 0; index < gameObjects.Count; index++)
        {
            var gameObject = gameObjects[index];
            if (!laneTypes.Contains(gameObject.LaneType))
                continue;

            var objectType = gameObject.ObjectType;
            if (objectType != allowedObjectType || gameObject.LaneModifier == LaneModifierType.Landmine)
                continue;

            var gameObjectData = gameObject.GameObjectData;
            if (gameObjectData == null)
                continue;

            if (isPlacing)
            {
                isPlacing = false;
                continue;
            }

            isPlacing = true;

            var tickPosition = ScrollContainer.PositionAtTime(gameObject.Offset);
            var nextObject = getNextObjectOfType(gameObjects, allowedObjectType, index);

            if (nextObject == null)
            {
                // TODO: popup for the user : the imported map has issues
                Logger.Log("Cannot find next object of type " + allowedObjectType, level: LogLevel.Error);
                continue;
            }

            var endPosition = ScrollContainer.PositionAtTime(nextObject.Offset);

            if (tickPosition < CurrentMinRange)
            {
                if (NextMinTick == null || tickPosition > NextMinTick)
                    NextMinTick = tickPosition;

                if (endPosition < CurrentMinRange)
                {
                    if (endPosition > NextMinTick)
                        NextMinTick = endPosition;

                    continue;
                }
            }

            if (tickPosition > CurrentMaxRange)
            {
                if (NextMaxTick == null || tickPosition < NextMaxTick)
                    NextMaxTick = tickPosition;

                continue;
            }

            var laneObject = getOrCreateObject();
            laneObject.X = tickPosition;

            laneObject.GameObject = gameObject;
            laneObject.SceneType = SceneType.SpaceStation; // TODO: scene at time
            laneObject.MovementType = gameObjectData.MovementType;
            laneObject.LaneType = gameObject.LaneType;
            laneObject.LaneModifier = gameObject.LaneModifier;
            laneObject.HoldLength = endPosition - tickPosition;
        }
    }

    private GameObject? getNextObjectOfType(List<GameObject> gameObjects, ObjectType allowedObjectType, int startIndex)
    {
        for (var index = startIndex + 1; index < gameObjects.Count; index++)
        {
            var gameObject = gameObjects[index];
            if (!laneTypes.Contains(gameObject.LaneType))
                continue;

            if (gameObject.ObjectType != allowedObjectType || gameObject.LaneModifier == LaneModifierType.Landmine)
                continue;

            return gameObject;
        }

        return null;
    }

    private void generateGeminiObjects()
    {
        var gameObjects = dataHolder.CurrentMap.Value.GameObjects;

        for (var index = 0; index < gameObjects.Count; index++)
        {
            var gameObject = gameObjects[index];
            if (!laneTypes.Contains(gameObject.LaneType)) continue;

            var objectType = gameObject.ObjectType;
            if (objectType is not ObjectType.Gemini)
                continue;

            var gameObjectData = gameObject.GameObjectData;
            if (gameObjectData == null) continue;

            var tickPosition = ScrollContainer.PositionAtTime(gameObject.Offset);

            // TODO
        }
    }

    private BaseLaneObject getOrCreateObject()
    {
        BaseLaneObject baseLaneObject;

        if (CurrentTickIndex >= Count)
        {
            baseLaneObject = new BaseLaneObject();
            Add(baseLaneObject);
        }
        else
        {
            baseLaneObject = Children[CurrentTickIndex];
        }

        baseLaneObject.Alpha = 1;
        baseLaneObject.HoldLength = null;

        CurrentTickIndex++;
        return baseLaneObject;
    }
}
