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

using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;

public partial class TimingPointsTable : Container
{
    [Resolved] protected EditorDataHolder DataHolder { get; private set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Width = 0.9f;
        Height = 1080 - 400 - 65 - 50;
        Origin = Anchor.TopCentre;
        Anchor = Anchor.TopCentre;
        Position = new Vector2(0, 400);

        Children =
        [
            // Background
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = MdeColors.Background4
            },
            new TimingPointsTableContainer()
        ];
    }
}
