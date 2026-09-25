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
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace MuseDashEditor.Game.Component.Cursor;

public partial class MdeCursor : Container<Sprite>
{
    public readonly Bindable<CursorType> CursorType = new(Cursor.CursorType.POINTER);

    [Resolved]
    private TextureStore textures { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Child = new Sprite();

        CursorType.BindValueChanged(onCursorTypeChange, true);
    }

    private void onCursorTypeChange(ValueChangedEvent<CursorType> evt)
    {
        evt.NewValue.Apply(Child, textures);
    }
}
