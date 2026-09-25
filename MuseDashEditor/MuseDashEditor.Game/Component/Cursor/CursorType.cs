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

using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace MuseDashEditor.Game.Component.Cursor;

public class CursorType(string cursorTextureName, Vector2 size, Vector2 offset)
{
    private static readonly Vector2 default_size = new(42);
    private static readonly Vector2 center_offset = new(-21);

    public static readonly CursorType POINTER = new("pointer_b", default_size, new Vector2(-12, -10));
    public static readonly CursorType RESIZE_H = new("resize_c_horizontal", default_size, center_offset);
    public static readonly CursorType DISABLED = new("disabled", default_size, center_offset);
    public static readonly CursorType MOVE = new("resize_a_cross", default_size, center_offset);

    public void Apply(Sprite sprite, TextureStore textures)
    {
        sprite.Size = size;
        sprite.Position = offset;
        sprite.Texture = textures.Get($"Cursor/{cursorTextureName}");
    }
}
