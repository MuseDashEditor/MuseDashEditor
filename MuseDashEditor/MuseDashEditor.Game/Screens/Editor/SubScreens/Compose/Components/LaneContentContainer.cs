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

using System.Linq;
using MuseDashEditor.Game.Component;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components;

public partial class LaneContentContainer(LaneType[] laneTypes) : AutoRefreshContainer<LaneObject>(50)
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
        var isPlacingHold = false;

        for (var index = 0; index < dataHolder.CurrentMap.Value.GameObjects.Count; index++)
        {
            var gameObject = dataHolder.CurrentMap.Value.GameObjects[index];
            if (!laneTypes.Contains(gameObject.LaneType)) continue;

            var gameObjectData = gameObject.GameObjectData;
            if (gameObjectData == null) continue;

            var tickPosition = ScrollContainer.PositionAtTime(gameObject.Offset);

            var isHold = gameObject.ObjectType == ObjectType.Hold;
            float? holdEnd = null;
            var shouldRender = false;

            if (tickPosition < CurrentMinRange)
            {
                if (NextMinTick == null || tickPosition > NextMinTick)
                    NextMinTick = tickPosition;

                if (!isHold)
                    continue;

                for (int nextIndex = index + 1;
                     nextIndex < dataHolder.CurrentMap.Value.GameObjects.Count;
                     nextIndex++)
                {
                    var nextObject = dataHolder.CurrentMap.Value.GameObjects[nextIndex];
                    if (!laneTypes.Contains(nextObject.LaneType)) continue;

                    if (nextObject.ObjectType != ObjectType.Hold) continue;

                    var nextPosition = ScrollContainer.PositionAtTime(nextObject.Offset);
                    if (nextPosition < CurrentMinRange)
                    {
                        break;
                    }

                    shouldRender = true;
                    holdEnd = nextPosition;
                    break;
                }

                if (!shouldRender)
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
            laneObject.Alpha = 1;
            laneObject.AlwaysPresent = shouldRender;

            laneObject.GameObject = gameObject;
            laneObject.SceneType = SceneType.SpaceStation; // TODO: scene at time
            laneObject.MovementType = gameObject.GameObjectData?.MovementType;
            laneObject.LaneType = gameObject.LaneType;
            laneObject.LaneModifier = gameObject.LaneModifier;
            laneObject.HoldLength = null;

            if (!isHold)
                continue;

            if (isPlacingHold) // End of hold
            {
                isPlacingHold = false;
                continue;
            }

            if (holdEnd == null)
            {
                for (int nextIndex = index + 1;
                     nextIndex < dataHolder.CurrentMap.Value.GameObjects.Count;
                     nextIndex++)
                {
                    var nextObject = dataHolder.CurrentMap.Value.GameObjects[nextIndex];
                    if (!laneTypes.Contains(nextObject.LaneType)) continue;

                    if (nextObject.ObjectType != ObjectType.Hold) continue;

                    var nextPosition = ScrollContainer.PositionAtTime(nextObject.Offset);
                    if (nextPosition < CurrentMinRange)
                    {
                        break;
                    }

                    holdEnd = nextPosition;
                    break;
                }

                if (holdEnd == null)
                {
                    // Single hold
                }
            }

            isPlacingHold = true;
            laneObject.HoldLength = holdEnd - tickPosition;
        }
    }

    private LaneObject getOrCreateObject()
    {
        LaneObject laneObject;

        if (CurrentTickIndex >= Count)
        {
            laneObject = new LaneObject();
            Add(laneObject);
        }
        else
        {
            laneObject = Children[CurrentTickIndex];
        }

        CurrentTickIndex++;
        return laneObject;
    }
}
