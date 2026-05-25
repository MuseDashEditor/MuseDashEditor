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

using MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata.Components;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata;

public partial class MetadataSubscreen : EditorSubscreen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new MetadataContainer
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(0.8f, 0.8f)
        };
    }
}
