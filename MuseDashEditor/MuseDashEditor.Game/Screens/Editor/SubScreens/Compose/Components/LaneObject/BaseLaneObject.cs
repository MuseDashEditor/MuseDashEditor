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

using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components.LaneObject;

public partial class BaseLaneObject : Container
{
    public const float BASE_SIZE = 75;

    [Resolved] private TextureStore textureStore { get; set; } = null!;

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

    private bool isHold;
    private float? holdLength;

    private SimpleLaneObject simpleObject = null!;
    private LongLaneObject longObject = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;
        AutoSizeAxes = Axes.X;
        Height = BASE_SIZE;

        Children =
        [
            simpleObject = new SimpleLaneObject(),
            longObject = new LongLaneObject()
        ];
    }

    private void updateObjectTextures()
    {
        if (laneType == 0)
            return;

        if (isHold)
            longObject.UpdateObjectTextures(gameObject.ObjectType, sceneType, laneType, laneModifier, /* TODO */ LaneModifierType.Normal);
        else
            simpleObject.UpdateObjectTextures(gameObject.ObjectType, sceneType, laneType, laneModifier, movementType);
    }

    private void setGameObject(GameObject value)
    {
        // TODO: Load sound etc.

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
}
