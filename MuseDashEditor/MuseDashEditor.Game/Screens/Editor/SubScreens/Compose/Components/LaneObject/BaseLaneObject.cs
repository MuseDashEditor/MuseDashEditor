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
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Object;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Screens.Editor.Components;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components.LaneObject;

public partial class BaseLaneObject(ZoomableScrollContainer scrollContainer) : RefreshableObject
{
    public const float BASE_SIZE = 75;

    [Resolved]
    private MdeSounds mdeSounds { get; set; } = null!;

    [Resolved]
    private SelectionHandler selectionHandler { get; set; } = null!;

    [Resolved]
    private EditorDataHolder editorDataHolder { get; set; } = null!;

    [Resolved]
    private EditorClock editorClock { get; set; } = null!;

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

    public bool IsDragging { get; private set; }
    private Vector2 dragStartPosition;
    private bool isDragCancelled;
    private Vector2 lastDragPosition;

    private TransformSequence<BaseLaneObject>? blinkTransform;
    private Transform fadeOut = null!;
    private Transform fadeIn = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        fadeOut = this.MakeTransform(nameof(Alpha), 0.3f, 750);
        fadeIn = this.MakeTransform(nameof(Alpha), 1f, 750);

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
            geminiObject = new SimpleLaneObject
            {
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Alpha = 0
            }
        ];

        editorClock.OnTimeChanged += _ => updateDragPosition();
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
        if (e.Button == MouseButton.Right)
        {
            if (IsDragging)
            {
                cancelDrag();
                return true;
            }

            // TODO context menu
            return true;
        }

        if (base.OnClick(e))
        {
            return true;
        }

        selectionHandler.Select(gameObject, e.ControlPressed);
        return true;
    }

    private void cancelDrag()
    {
        isDragCancelled = true;
        IsDragging = false;

        cancelBlinkEffect();

        // TODO reset position, lane etc.
    }

    private void cancelBlinkEffect()
    {
        // TODO: fix this
        // RemoveTransform(fadeOut);
        // RemoveTransform(fadeIn);
        // Alpha = 1f;
        blinkTransform?.TransformTo(nameof(Alpha), 1f);
    }

    protected override bool OnDoubleClick(DoubleClickEvent e)
    {
        scrollContainer.ScrollToTime(Offset, true);
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        return true;
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        dragStartPosition = Position;
        IsDragging = true;

        blinkTransform = this.TransformTo(nameof(Alpha), 0.3f, 750)
                             .Then()
                             .TransformTo(nameof(Alpha), 1f, 750)
                             .Loop();

        return true;
    }

    protected override void OnDragEnd(DragEndEvent e)
    {
        if (isDragCancelled)
            IsDragging = false;

        if (!IsDragging)
            return;

        IsDragging = false;
        cancelBlinkEffect();
    }

    protected override void OnDrag(DragEvent e)
    {
        if (isDragCancelled)
            return;

        lastDragPosition = e.ScreenSpaceMousePosition;

        updateGameObjectPosition();
    }

    private void updateGameObjectPosition()
    {
        var positionInScroll = scrollContainer.ToLocalSpace(lastDragPosition);
        var x = MathF.Max(0, (float)(positionInScroll.X - BASE_SIZE / 2 + scrollContainer.Current));

        x = scrollContainer.SnapXToNearestSubBeat(x);

        var lane = EditorConstants.GetLaneAtY(positionInScroll.Y - scrollContainer.Height / 2);

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

        var y = EditorConstants.GetLaneY(lane.Value);
        var offset = scrollContainer.TimeAtPosition(x);

        var otherObject = MapUtils.GetObjectAt(editorDataHolder.CurrentMap.Value.GameObjects, offset, lane.Value);
        if (otherObject is not null && otherObject.Id != gameObject.Id)
            return;

        X = x;
        Y = y;
        laneType = lane.Value;

        gameObject.Offset.Value = offset;
        updateObjectTextures();
    }

    private void updateDragPosition()
    {
        if (!IsDragging)
            return;

        updateGameObjectPosition();
    }
}
