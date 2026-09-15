// Copyright 2026 Axel "Azn9" Joly <contact@azn9.dev>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using Box = osu.Framework.Graphics.Shapes.Box;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class Toolbar : Container
{
    [BackgroundDependencyLoader]
    private void load(LargeTextureStore textureStore)
    {
        RelativeSizeAxes = Axes.X;
        Height = 50;
        Width = 1.0f;
        Origin = Anchor.TopLeft;
        Anchor = Anchor.TopLeft;

        Children =
        [
            // Background
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(1, 1),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = MdeColors.Background5
            },

            // Left part // TODO
            new FillFlowContainer
            {
                Direction = FillDirection.Horizontal,
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X,
                Children =
                [
                    new Sprite
                    {
                        Width = 125,
                        Height = 50,
                        Texture = textureStore.Get("title")
                    },
                    new EditorMenu()
                ]
            },

            // Right part
            new SubscreenSwitcher()
        ];
    }
}
