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
using MuseDashEditor.Game.Component;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components.LaneObject;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Utils;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components;

public partial class LaneContentContainer() : AutoRefreshContainer<BaseLaneObject>(BaseLaneObject.BASE_SIZE * 5)
{
    [Resolved] private EditorDataHolder dataHolder { get; set; } = null!;
    [Resolved] private EditorClock editorClock { get; set; } = null!;

    private double lastPlayedTickOffset;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
    }

    protected override void RegenerateContent()
    {
        // TODO: merge all, to iterate only once over all the objects
        generateBasicObjects();
        generateHoldObjects();
        generateGeminiObjects();
        generateDesignObjects();

        if (CurrentTickIndex != 0)
            return;

        NextMinTick = CurrentMinRange;
        NextMaxTick = CurrentMaxRange;
    }

    private void generateDesignObjects()
    {
        foreach (var gameObject in dataHolder.CurrentMap.Value.GameObjects)
        {
            var designObjectData = gameObject.DesignObjectData;
            if (designObjectData == null)
                continue;

            var tickOffset = gameObject.Offset.Value;
            var tickPosition = ScrollContainer.PositionAtTime(tickOffset);

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
            laneObject.Offset = tickOffset;
            laneObject.X = tickPosition;
            laneObject.Y = EditorConstants.GetLaneY(gameObject.LaneType);

            laneObject.GameObject = gameObject;
            laneObject.SceneType = SceneType.SpaceStation; // TODO: scene at time
            laneObject.LaneType = gameObject.LaneType;
            laneObject.LaneModifier = gameObject.LaneModifier;
        }
    }

    private void generateBasicObjects()
    {
        foreach (var gameObject in dataHolder.CurrentMap.Value.GameObjects)
        {
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

            var tickOffset = gameObject.Offset.Value;
            var tickPosition = ScrollContainer.PositionAtTime(tickOffset);

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
            laneObject.Offset = tickOffset;
            laneObject.X = tickPosition;
            laneObject.Y = EditorConstants.GetLaneY(gameObject.LaneType);

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
        HashSet<(LaneType, LaneModifierType)> placing = [];

        for (var index = 0; index < gameObjects.Count; index++)
        {
            var gameObject = gameObjects[index];

            var objectType = gameObject.ObjectType;
            if (objectType != allowedObjectType || gameObject.LaneModifier == LaneModifierType.Landmine)
                continue;

            var gameObjectData = gameObject.GameObjectData;
            if (gameObjectData == null)
                continue;

            if (placing.Remove((gameObject.LaneType, gameObject.LaneModifier)))
            {
                continue;
            }

            placing.Add((gameObject.LaneType, gameObject.LaneModifier));

            var tickOffset = gameObject.Offset.Value;
            var tickPosition = ScrollContainer.PositionAtTime(tickOffset);
            var nextObject = getNextObjectOfType(gameObjects, allowedObjectType, index, gameObject.LaneType, gameObject.LaneModifier);

            if (nextObject == null)
            {
                // TODO: popup for the user : the imported map has issues
                Logger.Log("Cannot find next object of type " + allowedObjectType, level: LogLevel.Error);
                continue;
            }

            var endPosition = ScrollContainer.PositionAtTime(nextObject.Offset.Value);

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
            laneObject.Offset = tickOffset;
            laneObject.X = tickPosition;
            laneObject.Y = EditorConstants.GetLaneY(gameObject.LaneType);

            laneObject.GameObject = gameObject;
            laneObject.SceneType = SceneType.SpaceStation; // TODO: scene at time
            laneObject.MovementType = gameObjectData.MovementType;
            laneObject.LaneType = gameObject.LaneType;
            laneObject.LaneModifier = gameObject.LaneModifier;
            laneObject.HoldLength = endPosition - tickPosition;
        }
    }

    private GameObject? getNextObjectOfType(List<GameObject> gameObjects, ObjectType objectType, int startIndex,
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

    private void generateGeminiObjects()
    {
        var gameObjects = dataHolder.CurrentMap.Value.GameObjects;

        foreach (var gameObject in gameObjects)
        {
            var objectType = gameObject.ObjectType;
            if (objectType is not ObjectType.Gemini)
                continue;

            if (gameObject.LaneType is not LaneType.Air and not LaneType.Air2)
                continue;

            var gameObjectData = gameObject.GameObjectData;
            if (gameObjectData == null) continue;

            var tickOffset = gameObject.Offset.Value;
            var tickPosition = ScrollContainer.PositionAtTime(tickOffset);

            var otherGemini = findOtherGemini(gameObjects, gameObject);
            if (otherGemini == null)
            {
                // TODO: popup for the user : the imported map has issues
                Logger.Log("Cannot find pairing gemini", level: LogLevel.Error);
                continue;
            }

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

            var laneObjectY = EditorConstants.GetLaneY(gameObject.LaneType);
            var otherLaneObjectY = EditorConstants.GetLaneY(otherGemini.LaneType);

            var laneObject = getOrCreateObject();
            laneObject.Offset = tickOffset;
            laneObject.X = tickPosition;
            laneObject.Y = (laneObjectY + otherLaneObjectY) / 2;
            laneObject.Height = MathF.Abs(otherLaneObjectY - laneObjectY) + BaseLaneObject.BASE_SIZE;

            laneObject.GameObject = gameObject;
            laneObject.SceneType = SceneType.SpaceStation; // TODO: scene at time
            laneObject.MovementType = gameObjectData.MovementType;
            laneObject.LaneType = gameObject.LaneType;
            laneObject.LaneModifier = gameObject.LaneModifier;
            laneObject.SetGeminiPairLane(otherGemini.LaneType, otherGemini.LaneModifier);
        }
    }

    private static GameObject? findOtherGemini(List<GameObject> gameObjects, GameObject gameObject)
    {
        var offsetValue = gameObject.Offset.Value;
        var isAir = gameObject.LaneType is LaneType.Air or  LaneType.Air2;

        foreach (var other in gameObjects)
        {
            if (other.ObjectType != ObjectType.Gemini)
                continue;

            if (!Precision.AlmostEquals(offsetValue, other.Offset.Value))
                continue;

            var isOtherAir = other.LaneType is LaneType.Air or  LaneType.Air2;
            if (isAir && isOtherAir || !isAir && !isOtherAir)
                continue;

            return other;
        }

        return null;
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
        baseLaneObject.Reset();

        CurrentTickIndex++;
        return baseLaneObject;
    }

    protected override void Update()
    {
        base.Update();

        if (editorClock.IsRunning)
            playSound();
        else
        {
            lastPlayedTickOffset = editorClock.CurrentTime;
        }
    }

    private void playSound()
    {
        var currentTime = editorClock.CurrentTime;
        if (currentTime < 0)
            return;

        var localLastPlayedTickOffset = lastPlayedTickOffset;

        foreach (var obj in Children)
        {
            var tickOffset = obj.Offset;

            if (tickOffset > currentTime) continue;
            if (!(lastPlayedTickOffset < tickOffset) || !(tickOffset < currentTime)) continue;

            obj.PlaySound();

            localLastPlayedTickOffset = tickOffset;
        }

        lastPlayedTickOffset = localLastPlayedTickOffset;
    }
}
