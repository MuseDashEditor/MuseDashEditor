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

public partial class LongLaneObject : Container
{
    [Resolved] private TextureStore textureStore { get; set; } = null!;

    private Container<Sprite> holdNotesContainer = null!;
    private Sprite holdBodySprite = null!;

    private Sprite leftCircleSprite = null!;
    private Sprite leftObjectSprite = null!;
    private Sprite leftLaneModifierSprite = null!;

    private Sprite rightCircleSprite = null!;
    private Sprite rightObjectSprite = null!;
    private Sprite rightLaneModifierSprite = null!;

    private int currentNotesTickIndex;
    private Texture notesTexture;

    [BackgroundDependencyLoader]
    private void load()
    {
        Name = "LongLaneObject";
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        RelativeSizeAxes = Axes.Y;

        notesTexture = textureStore.Get("Icons/Object/Common/hold_notes");

        Children =
        [
            holdBodySprite = new Sprite
            {
                Name = "Hold body",
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                RelativeSizeAxes = Axes.Y,
                Margin = new MarginPadding { Horizontal = BaseLaneObject.BASE_SIZE / 2 }
            },
            holdNotesContainer = new Container<Sprite>
            {
                Name = "Hold notes",
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                RelativeSizeAxes = Axes.Y,
                Margin = new MarginPadding { Horizontal = BaseLaneObject.BASE_SIZE / 2 },
                Masking = true
            },
            leftCircleSprite = new Sprite
            {
                Name = "Lane background Left",
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                Size = new Vector2(BaseLaneObject.BASE_SIZE),
                Margin = new MarginPadding { Left = BaseLaneObject.BASE_SIZE }
            },
            leftObjectSprite = new Sprite
            {
                Name = "Object Left",
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                Size = new Vector2(BaseLaneObject.BASE_SIZE),
                Margin = new MarginPadding { Left = BaseLaneObject.BASE_SIZE }
            },
            leftLaneModifierSprite = new Sprite
            {
                Name = "Lane modifier Left",
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                Size = new Vector2(BaseLaneObject.BASE_SIZE),
                Texture = textureStore.Get("Icons/Object/Common/heart"), // Default heart icon
                Alpha = 0,
                Margin = new MarginPadding { Left = BaseLaneObject.BASE_SIZE }
            },

            rightCircleSprite = new Sprite
            {
                Name = "Lane background Right",
                Anchor = Anchor.CentreRight,
                Origin = Anchor.Centre,
                Size = new Vector2(BaseLaneObject.BASE_SIZE),
                Margin = new MarginPadding { Right = BaseLaneObject.BASE_SIZE }
            },
            rightObjectSprite = new Sprite
            {
                Name = "Object Right",
                Anchor = Anchor.CentreRight,
                Origin = Anchor.Centre,
                Size = new Vector2(BaseLaneObject.BASE_SIZE),
                Margin = new MarginPadding { Right = BaseLaneObject.BASE_SIZE }
            },
            rightLaneModifierSprite = new Sprite
            {
                Name = "Lane modifier Right",
                Anchor = Anchor.CentreRight,
                Origin = Anchor.Centre,
                Size = new Vector2(BaseLaneObject.BASE_SIZE),
                Texture = textureStore.Get("Icons/Object/Common/heart"), // Default heart icon
                Alpha = 0,
                Margin = new MarginPadding { Right = BaseLaneObject.BASE_SIZE }
            }
        ];
    }

    public void UpdateObjectTextures(ObjectType objectType, SceneType sceneType, LaneType laneType,
        LaneModifierType leftLaneModifier, LaneModifierType rightLaneModifier)
    {
        leftCircleSprite.Texture = rightCircleSprite.Texture = textureStore.GetLaneBackgroundTexture(laneType);
        leftObjectSprite.Texture =
            rightObjectSprite.Texture = textureStore.GetObjectTexture(objectType, sceneType, laneType);

        leftLaneModifierSprite.Alpha = leftLaneModifier == LaneModifierType.Heart ? 1 : 0;
        rightLaneModifierSprite.Alpha = rightLaneModifier == LaneModifierType.Heart ? 1 : 0;

        holdBodySprite.Texture = textureStore.GetObjectTexture(ObjectType.HoldBody, sceneType, laneType);
    }

    public void SetHoldLength(float value)
    {
        Width = value + BaseLaneObject.BASE_SIZE;

        holdBodySprite.Width = value;
        holdNotesContainer.Width = value;

        currentNotesTickIndex = 0;

        var notesOriginalWidth = BaseLaneObject.BASE_SIZE * notesTexture.Width / notesTexture.Height;

        for (float totalLength = 0; totalLength < value; totalLength += notesOriginalWidth)
        {
            var sprite = getOrCreateNotes(notesOriginalWidth);
            sprite.X = (currentNotesTickIndex - 1) * notesOriginalWidth;
        }

        while (currentNotesTickIndex < holdNotesContainer.Count)
            holdNotesContainer.Children[currentNotesTickIndex++].Expire();
    }

    private Sprite getOrCreateNotes(float textureWidth)
    {
        Sprite child;

        if (currentNotesTickIndex >= holdNotesContainer.Count)
        {
            child = new Sprite
            {
                Name = "Hold notes",
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Width = textureWidth,
                RelativeSizeAxes = Axes.Y,
                Texture = notesTexture
            };
            holdNotesContainer.Add(child);
        }
        else
        {
            child = holdNotesContainer.Children[currentNotesTickIndex];
        }

        child.Alpha = 1;

        currentNotesTickIndex++;
        return child;
    }
}
