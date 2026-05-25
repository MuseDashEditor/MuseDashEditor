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

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata.Components;

public partial class MetadataContainer : CompositeComponent
{
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            // Background
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(1f, 1f),
                Colour = FrameworkColour.BlueGreen
            },

            // Title
            new SpriteText
            {
                Text = "Metadata",
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Font = FontUsage.Default.With(size: 30)
            },

            // Metadata fields
            new MetadataFieldsContainers
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(1f, 1f),
                Padding = new MarginPadding(10)
            }
        ];
    }
}
