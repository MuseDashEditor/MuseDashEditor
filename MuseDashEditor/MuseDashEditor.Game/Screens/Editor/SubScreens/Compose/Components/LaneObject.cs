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
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components;

public partial class LaneObject : Container
{
    [Resolved] private TextureStore textureStore { get; set; } = null!;

    public GameObject? GameObject
    {
        get => gameObject;
        set => setGameObject(value);
    }

    public MovementType? MovementType
    {
        get => movementType;
        set => setMovementType(value);
    }

    public SceneType? SceneType
    {
        get => sceneType;
        set => setSceneType(value);
    }

    public LaneType? LaneType
    {
        get => laneType;
        set => setLaneType(value);
    }

    public LaneModifierType? LaneModifier
    {
        get => laneModifier;
        set => setLaneModifier(value);
    }

    public float? HoldLength
    {
        get => holdLength;
        set => setHoldLength(value);
    }

    private GameObject? gameObject;
    private MovementType? movementType;
    private SceneType? sceneType;
    private LaneType? laneType;
    private LaneModifierType? laneModifier;
    private float? holdLength;

    private Sprite holdBodySprite = null!;
    private Sprite holdNotesSprite = null!;
    private Sprite circleSprite = null!;
    private Sprite objectSprite = null!;
    private Sprite movementSprite = null!;
    private Sprite laneModifierSprite = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.Centre;
        Size = new Vector2(75, 75);

        Children =
        [
            holdBodySprite = new Sprite
            {
                Name = "Hold body",
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft,
                RelativeSizeAxes = Axes.Y,
                Alpha = 0
            },
            holdNotesSprite = new Sprite
            {
                Name = "Hold notes",
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft,
                RelativeSizeAxes = Axes.Y,
                Alpha = 0,
                Texture = textureStore.Get("Icons/Object/Common/hold_notes"),
            },
            circleSprite = new Sprite
            {
                Name = "Lane background",
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both
            },
            objectSprite = new Sprite
            {
                Name = "Object",
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
            },
            movementSprite = new Sprite
            {
                Name = "Movement type",
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            laneModifierSprite = new Sprite
            {
                Name = "Lane modifier",
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Texture = textureStore.Get("Icons/Object/Common/heart"), // Default heart icon
                Alpha = 0
            }
        ];
        //
        // // TODO: replace with a sprite
        // Child = new FastCircle
        // {
        //     Size = new Vector2(75, 75),
        //     Colour = Color4.White,
        //     Anchor = Anchor.Centre,
        //     Origin = Anchor.Centre
        // };
    }

    private void updateObjectTextures()
    {
        if (laneType == null || gameObject == null || sceneType == null)
            return;

        circleSprite.Texture = textureStore.GetLaneBackgroundTexture(laneType.Value);
        objectSprite.Texture = textureStore.GetObjectTexture(gameObject.ObjectType, sceneType.Value, laneType.Value);
    }

    private void setGameObject(GameObject? value)
    {
        if (value == null) return;
        // TODO: Load sound etc.

        gameObject = value;
        updateObjectTextures();
    }

    private void setMovementType(MovementType? value)
    {
        if (value == null) return;

        movementType = value;

        if (value == Data.Type.MovementType.None)
        {
            movementSprite.Alpha = 0;
        }
        else
        {
            movementSprite.Alpha = 1;
            movementSprite.Texture = textureStore.GetMovementTypeTexture(movementType.Value);
        }
    }

    private void setSceneType(SceneType? value)
    {
        if (value == null) return;

        sceneType = value;
        updateObjectTextures();
    }

    private void setLaneType(LaneType? value)
    {
        if (value == null) return;

        laneType = value;
        updateObjectTextures();
    }

    private void setLaneModifier(LaneModifierType? value)
    {
        if (value == null) return;

        laneModifier = value;

        laneModifierSprite.Alpha = value == LaneModifierType.Heart ? 1 : 0;
    }

    private void setHoldLength(float? value)
    {
        holdLength = value;

        if (value == null)
        {
            holdBodySprite.Alpha = 0;
            holdNotesSprite.Alpha = 0;
            return;
        }

        if (laneType == null) return;

        holdBodySprite.Alpha = 1;
        holdNotesSprite.Alpha = 1;

        holdBodySprite.Texture = textureStore.GetObjectTexture(ObjectType.HoldBody, Data.Type.SceneType.SpaceStation, laneType!.Value);
        holdBodySprite.Width = value.Value;
        holdNotesSprite.Width = value.Value;
    }
}
