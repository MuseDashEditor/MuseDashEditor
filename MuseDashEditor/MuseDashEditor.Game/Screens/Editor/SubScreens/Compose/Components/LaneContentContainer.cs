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
    [Resolved]
    private EditorDataHolder dataHolder { get; set; } = null!;

    [Resolved]
    private EditorClock editorClock { get; set; } = null!;

    [Resolved]
    private SelectionContainer selectionContainer { get; set; } = null!;

    private double lastPlayedTickOffset;
    private BaseLaneObject? placementObject;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;

        selectionContainer.LaneContentContainer = this;

        dataHolder.IsInPlacementMode.BindValueChanged(@event =>
        {
            if (@event.NewValue)
                beginPlacementMode();
            else
                stopPlacementMode();
        });

        dataHolder.OnGameObjectsChanged += Invalidate;
    }

    protected override void RegenerateContent()
    {
        foreach (var gameObject in dataHolder.CurrentMap.Value!.GameObjects)
        {
            if (gameObject.IsHoldEnd || gameObject is { GeminiPairObject: not null, LaneType: not LaneType.Air and not LaneType.Air2 })
                continue;

            var tickOffset = gameObject.Offset.Value;
            var tickPosition = ScrollContainer.PositionAtTime(tickOffset);
            var isSelected = gameObject.Selected.Value;

            float? endPosition = ScrollContainer.PositionAtTime(gameObject.HoldEndObject?.Offset.Value);
            GameObject? otherGemini = gameObject.GeminiPairObject;

            var laneObject = gameObject.LaneObject;

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

                        if (!isSelected)
                        {
                            if (laneObject != null)
                            {
                                laneObject.IsUsed = false;
                                gameObject.LaneObject = null;
                            }

                            continue;
                        }
                    }
                }
                else if (!isSelected)
                {
                    if (laneObject != null)
                    {
                        laneObject.IsUsed = false;
                        gameObject.LaneObject = null;
                    }

                    continue;
                }
            }

            if (tickPosition > CurrentMaxRange)
            {
                if (NextMaxTick == null || tickPosition < NextMaxTick)
                    NextMaxTick = tickPosition;

                if (!isSelected)
                {
                    if (laneObject != null)
                    {
                        laneObject.IsUsed = false;
                        gameObject.LaneObject = null;
                    }

                    continue;
                }
            }

            if (laneObject != null)
                continue;

            laneObject = getOrCreateObject();
            laneObject.IsUsed = true;
            laneObject.Offset = tickOffset;
            laneObject.X = tickPosition;
            laneObject.Y = EditorConstants.GetLaneY(gameObject.LaneType);

            laneObject.GameObject = gameObject;
            laneObject.SceneType = dataHolder.GetSceneAtTime(tickOffset);
            laneObject.LaneType = gameObject.LaneType;
            laneObject.LaneModifier = gameObject.LaneModifier;

            gameObject.LaneObject = laneObject;

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

            if (otherGemini != null && gameObject.LaneType is LaneType.Air or LaneType.Air2)
            {
                var laneObjectY = EditorConstants.GetLaneY(gameObject.LaneType);
                var otherLaneObjectY = EditorConstants.GetLaneY(otherGemini.LaneType);

                laneObject.Y = (laneObjectY + otherLaneObjectY) / 2;
                laneObject.Height = MathF.Abs(otherLaneObjectY - laneObjectY) + BaseLaneObject.BASE_SIZE;
                laneObject.SetGeminiPairLane(otherGemini.LaneType, otherGemini.LaneModifier);
            }
        }

        selectionContainer.UpdateSelection();

        if (CurrentTickIndex != 0)
            return;

        NextMinTick = CurrentMinRange;
        NextMaxTick = CurrentMaxRange;
    }

    private BaseLaneObject getOrCreateObject()
    {
        while (true)
        {
            BaseLaneObject baseLaneObject;

            if (CurrentTickIndex >= Count)
            {
                baseLaneObject = new BaseLaneObject(ScrollContainer);
                Add(baseLaneObject);
            }
            else
            {
                baseLaneObject = Children[CurrentTickIndex];
            }

            CurrentTickIndex++;

            if (baseLaneObject.IsUsed)
            {
                continue;
            }

            baseLaneObject.Alpha = 1;
            baseLaneObject.Reset();

            return baseLaneObject;
        }
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

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        if (!dataHolder.IsInPlacementMode.Value)
            return false;

        var pos = ToLocalSpace(e.ScreenSpaceMousePosition);
        placementObject?.MoveObjectTo(pos.X, pos.Y - Height / 2);

        return false;
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (!dataHolder.IsInPlacementMode.Value)
            return false;

        if (e.Button == MouseButton.Left)
        {
            placeObject();
        }

        return base.OnClick(e);
    }

    private void placeObject()
    {
        if (placementObject is null)
            return;

        var templateObject = placementObject.GameObject;

        var newObject = templateObject.Clone();

        dataHolder.AddObject(newObject);
        historyManager.AddAction(new AddObject(newObject));

        // Special cases
        if (templateObject.ObjectType is ObjectType.Hold or ObjectType.Masher)
        {
            // TODO continue placement to next object & draw body (help)
        }
        else if (templateObject.ObjectType is ObjectType.Gemini)
        {
            // TODO place pair on other lane
        }
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (!dataHolder.IsInPlacementMode.Value)
            return false;

        // TODO place at all points
        return base.OnDragStart(e);
    }

    private void beginPlacementMode()
    {
        placementObject = getOrCreateObject();
        placementObject.IsUsed = true;
        placementObject.IsPlacementObject = true;
        placementObject.Alpha = 0.5f;

        var offset = ScrollContainer.GetCurrentOrTargetTime();
        var tickPosition = ScrollContainer.PositionAtTime(offset);

        var objectType = dataHolder.PlacementObjectType;
        var gameObject = new GameObject(
            offset,
            objectType,
            LaneType.Air,
            LaneModifierType.Normal
        );

        var laneType = gameObject.GameObjectData?.ValidLaneTypes.FirstOrDefault() ??
                       gameObject.DesignObjectData?.ValidLaneTypes.FirstOrDefault() ??
                       LaneType.Air;
        gameObject.LaneType = laneType;

        placementObject.Offset = offset;
        placementObject.X = tickPosition;
        placementObject.Y = EditorConstants.GetLaneY(gameObject.LaneType);

        placementObject.GameObject = gameObject;
        placementObject.SceneType = dataHolder.GetSceneAtTime(offset);
        placementObject.LaneType = gameObject.LaneType;
        placementObject.LaneModifier = gameObject.LaneModifier;

        gameObject.LaneObject = placementObject;
    }

    private void stopPlacementMode()
    {
        if (placementObject is null)
            return;

        placementObject.IsUsed = false;
        placementObject.IsPlacementObject = false;
        placementObject.Alpha = 0;
    }
}
