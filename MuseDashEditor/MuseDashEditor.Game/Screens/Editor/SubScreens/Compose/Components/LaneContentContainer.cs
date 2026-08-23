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
using MuseDashEditor.Game.Component;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components.LaneObject;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

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
        foreach (var gameObject in dataHolder.CurrentMap.Value.GameObjects)
        {
            if (gameObject.IsHoldEnd)
                continue;

            var tickOffset = gameObject.Offset.Value;
            var tickPosition = ScrollContainer.PositionAtTime(tickOffset);

            float? endPosition = ScrollContainer.PositionAtTime(gameObject.HoldEndObject?.Offset.Value);
            GameObject? otherGemini = gameObject.GeminiPairObject;

            if (tickPosition < CurrentMinRange)
            {
                if (NextMinTick == null || tickPosition > NextMinTick)
                    NextMinTick = tickPosition;

                if (endPosition != null)
                {
                    if (endPosition < CurrentMinRange)
                    {
                        if (endPosition > NextMinTick)
                            NextMinTick = endPosition;

                        continue;
                    }
                }
                else
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

            var gameObjectData = gameObject.GameObjectData;
            if (gameObjectData != null)
            {
                laneObject.MovementType = gameObjectData.MovementType;
            }

            var designObjectData = gameObject.DesignObjectData;
            if (designObjectData != null)
            {
                // TODO
            }

            if (endPosition != null)
            {
                laneObject.HoldLength = endPosition - tickPosition;
            }

            if (otherGemini != null)
            {
                var laneObjectY = EditorConstants.GetLaneY(gameObject.LaneType);
                var otherLaneObjectY = EditorConstants.GetLaneY(otherGemini.LaneType);

                laneObject.Y = (laneObjectY + otherLaneObjectY) / 2;
                laneObject.Height = MathF.Abs(otherLaneObjectY - laneObjectY) + BaseLaneObject.BASE_SIZE;
                laneObject.SetGeminiPairLane(otherGemini.LaneType, otherGemini.LaneModifier);
            }
        }

        selectionHandler.UpdateSelection();

        if (CurrentTickIndex != 0)
            return;

        NextMinTick = CurrentMinRange;
        NextMaxTick = CurrentMaxRange;
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
