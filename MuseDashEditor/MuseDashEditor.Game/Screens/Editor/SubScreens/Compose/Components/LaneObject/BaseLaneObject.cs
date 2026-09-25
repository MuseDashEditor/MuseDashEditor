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
using System.Linq;
using MuseDashEditor.Game.Component;
using MuseDashEditor.Game.Component.Cursor;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Object;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.History;
using MuseDashEditor.Game.History.Actions;
using MuseDashEditor.Game.Screens.Editor.Components;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components.LaneObject;

/**
 * TODO
 * I know this class is becoming a huge mess, and would really need some refactor, but for now I focus on making things work
 */
public partial class BaseLaneObject(ZoomableScrollContainer scrollContainer) : RefreshableObject
{
    public const float BASE_SIZE = 75;

    [Resolved]
    private MdeSounds mdeSounds { get; set; } = null!;

    [Resolved]
    private SelectionHandler selectionHandler { get; set; } = null!;

    [Resolved]
    private SelectionContainer selectionContainer { get; set; } = null!;

    [Resolved]
    private EditorDataHolder editorDataHolder { get; set; } = null!;

    [Resolved]
    private EditorClock editorClock { get; set; } = null!;

    [Resolved]
    private MdeCursorContainer cursorContainer { get; set; } = null!;

    [Resolved]
    private HistoryManager historyManager { get; set; } = null!;

    public double Offset { get; set; }

    public GameObject GameObject
    {
        get => gameObject;
        set => setGameObject(value);
    }

    public MovementType MovementType
    {
        get => movementType;
        set => setMovementType(value);
    }

    public SceneType SceneType
    {
        get => sceneType;
        set => setSceneType(value);
    }

    public LaneType LaneType
    {
        get => laneType;
        set => setLaneType(value);
    }

    public LaneModifierType LaneModifier
    {
        get => laneModifier;
        set => setLaneModifier(value);
    }

    public float? HoldLength
    {
        get => holdLength;
        set => setHoldLength(value);
    }

    public bool IsPlacementObject;

    private GameObject gameObject = null!;
    private MovementType movementType;
    private SceneType sceneType;
    private LaneType laneType;
    private LaneModifierType laneModifier;
    private HitSoundType hitSoundType;

    private bool isHold;
    private float? holdLength;

    private SimpleLaneObject simpleObject = null!;
    private SimpleLaneObject geminiObject = null!;
    private LongLaneObject longObject = null!;

    private bool isDragging { get; set; }
    private bool isMoveLockLane;
    private bool isMoveLockOffset;
    private Vector2 dragDiffPosition;
    private bool isDragCancelled;
    private Vector2 lastDragPosition;
    private Vector2 dragStartPosition;
    private LaneType dragStartLane;
    private double dragStartOffset;

    public bool IsResizing { get; private set; }
    private bool hasSetCursorToResize;
    private bool resizingLeft;
    private double resizeStartDuration;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;
        AutoSizeAxes = Axes.X;
        Height = BASE_SIZE;

        Children =
        [
            simpleObject = new SimpleLaneObject
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft
            },
            longObject = new LongLaneObject(),
            geminiObject = new SimpleLaneObject(true)
            {
                Anchor = Anchor.BottomRight,
                Origin = Anchor.TopRight,
                Alpha = 0
            }
        ];

        editorClock.OnTimeChanged += _ =>
        {
            if (isHold)
                longObject.InvalidateSsdq();

            updateDragPosition();
        };
    }

    private void updateObjectTextures()
    {
        if (laneType == 0)
            return;

        if (isHold && LaneModifier != LaneModifierType.Landmine)
        {
            longObject.UpdateObjectTextures(gameObject.ObjectType, sceneType, laneType, laneModifier, /* TODO */
                LaneModifierType.Normal);
        }
        else
            simpleObject.UpdateObjectTextures(gameObject.ObjectType, sceneType, laneType, laneModifier, movementType);
    }

    private void setGameObject(GameObject value)
    {
        var gameObjectData = GameObjectUtils.GetGameObjectData(value.ObjectType);
        if (gameObjectData is not null)
            hitSoundType = gameObjectData.HitSoundType;

        gameObject = value;
        updateObjectTextures();
    }

    private void setMovementType(MovementType value)
    {
        movementType = value;
        updateObjectTextures();
    }

    private void setSceneType(SceneType value)
    {
        sceneType = value;
        updateObjectTextures();
    }

    private void setLaneType(LaneType value)
    {
        laneType = value;
        updateObjectTextures();
    }

    private void setLaneModifier(LaneModifierType value)
    {
        laneModifier = value;
        updateObjectTextures();
    }

    private void setHoldLength(float? value)
    {
        holdLength = value;

        if (value == null)
        {
            isHold = false;
            longObject.Alpha = 0;
            simpleObject.Alpha = 1;
            return;
        }

        isHold = true;
        simpleObject.Alpha = 0;
        longObject.Alpha = 1;

        X += value.Value / 2;

        longObject.SetHoldLength(value.Value);
        updateObjectTextures();
    }

    public void PlaySound()
    {
        if (!IsPresent)
            return;

        mdeSounds.PlayHitSound(hitSoundType);

        if (laneModifier == LaneModifierType.Heart)
            mdeSounds.PlayHitSound(HitSoundType.Heart);
    }

    public void SetGeminiPairLane(LaneType value, LaneModifierType laneModifierType)
    {
        geminiObject.UpdateObjectTextures(gameObject.ObjectType, sceneType, value, laneModifierType, movementType);
        geminiObject.Alpha = 1;
    }

    public void Reset()
    {
        simpleObject.Alpha = 1;
        geminiObject.Alpha = 0;
        longObject.Alpha = 0;
        Y = 0;
        X = 0;
        holdLength = null;
        isHold = false;
        Height = BASE_SIZE;
        hitSoundType = HitSoundType.None;
        simpleObject.Reset();
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (IsPlacementObject)
            return false;

        selectionHandler.Select(gameObject, e.ControlPressed);
        return true;
    }

    private void cancelDrag()
    {
        isDragCancelled = true;
        isDragging = false;

        cancelBlinkEffect();

        Position = dragStartPosition;
        LaneType = dragStartLane;
        gameObject.Offset.Value = dragStartOffset;

        // TODO cancel resize

        updateObjectTextures();

        selectionContainer.UpdateSelection();
    }

    private void cancelBlinkEffect()
    {
        ClearTransforms(false, nameof(Alpha));
        Alpha = 1f;
    }

    protected override bool OnDoubleClick(DoubleClickEvent e)
    {
        if (IsPlacementObject)
            return false;

        scrollContainer.ScrollToTime(Offset, true);
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (IsPlacementObject)
            return false;

        if (e.Button == MouseButton.Left)
        {
            if (!longObject.IsPresent)
                return false;

            var isInLeftCircle = longObject.IsInLeftCircle(e.ScreenSpaceMousePosition);
            var isInRightCircle = longObject.IsInRightCircle(e.ScreenSpaceMousePosition);

            if (!isInLeftCircle && !isInRightCircle)
                return false;

            IsResizing = true;
            resizingLeft = isInLeftCircle;
        }
        else if (e.Button == MouseButton.Right)
        {
            if (isDragging)
            {
                foreach (var selectedObject in selectionHandler.SelectedObjects)
                {
                    selectedObject.LaneObject?.cancelDrag();
                }

                return true;
            }

            // TODO context menu

            return true;
        }

        return false;
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (IsPlacementObject)
            return false;

        if (IsResizing)
        {
            resizeStartDuration = gameObject.HoldDuration;
            isMoveLockLane = true;
            startMoveObject(e);
            return true;
        }

        if (!gameObject.Selected.Value)
        {
            selectionHandler.Select(gameObject);
        }
        else if (selectionHandler.SelectionCount > 1)
        {
            foreach (var selectedObject in selectionHandler.SelectedObjects)
            {
                selectedObject.LaneObject?.startMoveObject(e);
            }

            return true;
        }

        startMoveObject(e);
        cursorContainer.SetCursorType(CursorType.MOVE);

        return true;
    }

    private void startMoveObject(DragStartEvent e)
    {
        isDragCancelled = false;

        dragStartPosition = Position;
        dragStartLane = LaneType;
        dragStartOffset = gameObject.Offset.Value;

        var parentSpace = ToParentSpace(ToLocalSpace(e.ScreenSpaceMousePosition));
        parentSpace.Y -= Parent?.Height / 2 ?? 0;
        dragDiffPosition = parentSpace - Position;
        isDragging = true;

        this.TransformTo(nameof(Alpha), 0.3f, 750)
            .Then()
            .TransformTo(nameof(Alpha), 1f, 750)
            .Loop();
    }

    protected override void OnDragEnd(DragEndEvent e)
    {
        if (isDragCancelled)
            isDragging = false;

        if (!isDragging)
            return;

        if (IsResizing)
        {
            stopResizingObject();
            return;
        }

        historyManager.StartCompound();

        stopMoveObject();

        foreach (var selectedObject in selectionHandler.SelectedObjects)
        {
            selectedObject.LaneObject?.stopMoveObject();
        }

        historyManager.EndCompound();
    }

    private void stopResizingObject()
    {
        cancelBlinkEffect();

        isDragging = false;
        isMoveLockLane = false;
        isMoveLockOffset = false;
        IsResizing = false;
        hasSetCursorToResize = false;
        cursorContainer.SetCursorType(CursorType.POINTER);

        if (longObject.IsPresent)
            longObject.InvalidateSsdq();

        historyManager.AddAction(new ResizeObject(
            gameObject,
            resizeStartDuration,
            gameObject.HoldDuration,
            resizingLeft
        ));
    }

    private void stopMoveObject()
    {
        cancelBlinkEffect();

        isDragging = false;
        isMoveLockLane = false;
        isMoveLockOffset = false;
        IsResizing = false;
        hasSetCursorToResize = false;
        cursorContainer.SetCursorType(CursorType.POINTER);

        if (longObject.IsPresent)
            longObject.InvalidateSsdq();

        historyManager.AddAction(new MoveObject(
            gameObject,
            dragStartLane,
            laneType,
            dragStartOffset,
            gameObject.Offset.Value
        ));
    }

    protected override void OnDrag(DragEvent e)
    {
        if (isDragCancelled)
            return;

        if (IsResizing)
        {
            updateSizeObject(e);
            return;
        }

        foreach (var selectedObject in selectionHandler.SelectedObjects)
        {
            selectedObject.LaneObject?.updateMoveObject(e);
        }
    }

    private void updateSizeObject(DragEvent e)
    {
        lastDragPosition = e.ScreenSpaceMousePosition;
        var positionInScroll = scrollContainer.ToLocalSpace(lastDragPosition);
        var x = MathF.Max(0, (float)(positionInScroll.X - BASE_SIZE * 1.5 + scrollContainer.Current));
        x = scrollContainer.SnapXToNearestSubBeat(x);

        var movedObject = resizingLeft ? GameObject : GameObject.HoldEndObject!;

        var offset = scrollContainer.TimeAtPosition(x);

        var otherObject = MapUtils.GetObjectAt(editorDataHolder.CurrentMap.Value!.GameObjects, offset, laneType);
        if (otherObject is not null && otherObject.Id != movedObject.Id)
            return;

        if (resizingLeft)
        {
            var currentEndX = scrollContainer.PositionAtTime(GameObject.HoldEndObject!.Offset.Value);
            if (x >= currentEndX)
                return;

            X = x;
            HoldLength = currentEndX - x;
            sceneType = editorDataHolder.GetSceneAtTime(offset);
        }
        else
        {
            var startX = scrollContainer.PositionAtTime(GameObject.Offset.Value);
            if (x <= startX)
                return;

            X = startX;
            HoldLength = x - startX;
        }

        movedObject.Offset.Value = offset;

        gameObject.HoldDuration = gameObject.HoldEndObject!.Offset.Value - gameObject.Offset.Value;

        editorDataHolder.OnGameObjectsChanged();

        if (longObject.IsPresent)
            longObject.InvalidateSsdq();

        if (gameObject.Selected.Value)
            selectionContainer.UpdateSelection();

        updateObjectTextures();
    }

    private void updateMoveObject(DragEvent e)
    {
        lastDragPosition = e.ScreenSpaceMousePosition;

        updateGameObjectPosition();
    }

    private void updateGameObjectPosition()
    {
        var positionInScroll = scrollContainer.ToLocalSpace(lastDragPosition) - dragDiffPosition;
        var x = MathF.Max(0, (float)(positionInScroll.X - BASE_SIZE * 1.5 + scrollContainer.Current));
        var y = positionInScroll.Y - scrollContainer.Height / 2;

        if (MoveObjectTo(x, y))
            return;

        if (gameObject.Selected.Value)
            selectionContainer.UpdateSelection();
    }

    public bool MoveObjectTo(float x, float y)
    {
        x = scrollContainer.SnapXToNearestSubBeat(x);

        if (isHold)
        {
            x += holdLength!.Value / 2;

            var currentDiff = x - X - dragDiffPosition.X;
            var currentStartX = scrollContainer.PositionAtTime(gameObject.Offset.Value);
            var futureStartX = currentStartX + currentDiff;

            if (futureStartX < 0)
            {
                x = (float)(holdLength / 2)!;
            }
            else
            {
                var diffToSnapStart = futureStartX - scrollContainer.SnapXToNearestSubBeat(futureStartX);
                x += diffToSnapStart;
            }
        }

        var lane = EditorConstants.GetLaneAtY(y);

        if (lane is not null)
        {
            ObjectData? objectData = (ObjectData?)gameObject.GameObjectData ?? gameObject.DesignObjectData;

            // Should never be null here
            if (objectData is not null)
            {
                var validLaneTypes = objectData.ValidLaneTypes;
                var isAllowed = validLaneTypes.Length == 0 || validLaneTypes.Contains(lane.Value);

                if (!isAllowed)
                    lane = laneType;
            }
            else
                lane = laneType;
        }
        else
            lane = laneType;

        y = EditorConstants.GetLaneY(lane.Value);

        if (isHold)
        {
            x -= holdLength!.Value / 2;
            var diff = scrollContainer.SnapXToNearestSubBeat(x) - x;
            x += diff;
        }

        var offset = scrollContainer.TimeAtPosition(x);

        var otherObject = MapUtils.GetObjectAt(editorDataHolder.CurrentMap.Value!.GameObjects, offset, lane.Value);
        if (otherObject is not null && otherObject.Id != gameObject.Id)
            return true;

        if (!isMoveLockLane)
        {
            Y = y;
            laneType = lane.Value;
            gameObject.LaneType = laneType;
        }

        if (!isMoveLockOffset)
        {
            if (isHold)
            {
                X = x - holdLength!.Value / 2;
                offset = scrollContainer.TimeAtPosition(X);
            }
            else
                X = x;

            sceneType = editorDataHolder.GetSceneAtTime(offset);
            gameObject.Offset.Value = offset;

            if (gameObject.HoldEndObject is not null)
                gameObject.HoldEndObject.Offset.Value = offset + gameObject.HoldDuration;
        }

        editorDataHolder.OnGameObjectsChanged();

        if (isHold)
            longObject.InvalidateSsdq();

        updateObjectTextures();
        return false;
    }

    private void updateDragPosition()
    {
        if (!isDragging)
            return;

        updateGameObjectPosition();
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (!IsPlacementObject || e.Repeat)
            return false;

        switch (e.Key)
        {
            case Key.AltLeft:
            case Key.AltRight:
            {
                if (!(gameObject.GameObjectData?.ValidLaneModifiers.Contains(LaneModifierType.Heart) ?? false))
                    return true;

                gameObject.LaneModifier = LaneModifierType.Heart;
                laneModifier = LaneModifierType.Heart;
                updateObjectTextures();

                return true;
            }

            case Key.ShiftLeft:
            case Key.ShiftRight:
            case Key.ControlLeft:
            case Key.ControlRight:
            {
                if ((!e.PressedKeys.Contains(Key.ShiftLeft) && !e.PressedKeys.Contains(Key.ShiftRight))
                    || (!e.PressedKeys.Contains(Key.ControlLeft) && !e.PressedKeys.Contains(Key.ControlRight)))
                    return true;

                if (!(gameObject.GameObjectData?.ValidLaneModifiers.Contains(LaneModifierType.Landmine) ?? false))
                    return true;

                gameObject.LaneModifier = LaneModifierType.Landmine;
                laneModifier = LaneModifierType.Landmine;
                updateObjectTextures();

                return true;
            }
        }

        return false;
    }

    protected override void OnKeyUp(KeyUpEvent e)
    {
        if (!IsPlacementObject)
            return;

        switch (e.Key)
        {
            case Key.AltLeft:
            case Key.AltRight:
            {
                if (e.PressedKeys.Contains(Key.AltLeft) || e.PressedKeys.Contains(Key.AltRight))
                    return;

                if (!(gameObject.GameObjectData?.ValidLaneModifiers.Contains(LaneModifierType.Heart) ?? false))
                    return;

                gameObject.LaneModifier = LaneModifierType.Normal;
                laneModifier = LaneModifierType.Normal;
                updateObjectTextures();

                return;
            }

            case Key.ShiftLeft:
            case Key.ShiftRight:
            case Key.ControlLeft:
            case Key.ControlRight:
            {
                if ((e.PressedKeys.Contains(Key.ShiftLeft) || e.PressedKeys.Contains(Key.ShiftRight))
                    && (e.PressedKeys.Contains(Key.ControlLeft) || e.PressedKeys.Contains(Key.ControlRight)))
                    return;

                if (!(gameObject.GameObjectData?.ValidLaneModifiers.Contains(LaneModifierType.Landmine) ?? false))
                    return;

                gameObject.LaneModifier = LaneModifierType.Normal;
                laneModifier = LaneModifierType.Normal;
                updateObjectTextures();

                return;
            }
        }
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        if (IsResizing || isDragging)
            return false;

        if (gameObject.ObjectType is not ObjectType.Hold and not ObjectType.Masher and not ObjectType.BossMasher1 and not ObjectType.BossMasher2)
            return false;

        if (laneModifier == LaneModifierType.Landmine)
            return false;

        var isInLeftCircle = longObject.IsInLeftCircle(e.ScreenSpaceMousePosition);
        var isInRightCircle = longObject.IsInRightCircle(e.ScreenSpaceMousePosition);

        if (isInLeftCircle || isInRightCircle)
        {
            hasSetCursorToResize = true;
            cursorContainer.SetCursorType(CursorType.RESIZE_H);
        }
        else
        {
            hasSetCursorToResize = false;
            cursorContainer.SetCursorType(CursorType.POINTER);
        }

        return false;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        if (!hasSetCursorToResize || IsResizing)
            return;

        hasSetCursorToResize = false;
        cursorContainer.SetCursorType(CursorType.POINTER);
    }
}
