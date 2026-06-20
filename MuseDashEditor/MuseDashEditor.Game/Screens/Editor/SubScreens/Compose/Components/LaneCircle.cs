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

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components;

public partial class LaneCircle : Container
{
    public Color4 InnerColor;

    [BackgroundDependencyLoader]
    private void load(LargeTextureStore textureStore)
    {
        var circleTexture = textureStore.Get("UI/circle");

        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Width = 50;
        Height = 50;

        Children =
        [
            new Sprite
            {
                Texture = circleTexture,
                Size = new Vector2(50, 50),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            },
            new Sprite
            {
                Texture = circleTexture,
                Size = new Vector2(30, 30),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = InnerColor,
            }
        ];
    }
}
