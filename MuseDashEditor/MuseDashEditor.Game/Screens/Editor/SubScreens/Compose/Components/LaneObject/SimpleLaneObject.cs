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

using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components.LaneObject;

public partial class SimpleLaneObject : Container
{
    [Resolved] private TextureStore textureStore { get; set; } = null!;

    private Sprite circleSprite = null!;
    private Sprite objectSprite = null!;
    private Sprite movementSprite = null!;
    private Sprite laneModifierSprite = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Name = "SimpleLaneObject";
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        RelativeSizeAxes = Axes.Y;
        Width = 75;

        Children =
        [
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
                RelativeSizeAxes = Axes.Both
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
    }

    public void UpdateObjectTextures(ObjectType objectType, SceneType sceneType, LaneType laneType,
        LaneModifierType laneModifier, MovementType movementType)
    {
        circleSprite.Texture = textureStore.GetLaneBackgroundTexture(laneType);
        objectSprite.Texture = textureStore.GetObjectTexture(objectType, sceneType, laneType);

        if (movementType == MovementType.None)
        {
            movementSprite.Alpha = 0;
        }
        else
        {
            movementSprite.Alpha = 1;
            movementSprite.Texture = textureStore.GetMovementTypeTexture(movementType);
        }

        laneModifierSprite.Alpha = laneModifier == LaneModifierType.Heart ? 1 : 0;
    }
}
