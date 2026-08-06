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

using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace MuseDashEditor.Game.Component.Notification;

public partial class Notification : Container
{
    protected Drawable Header { get; init; } = null!;
    protected Drawable Body { get; init; } = null!;
    protected Drawable Footer { get; init; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;

        Children =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = MdeColors.Dark6,
                EdgeSmoothness = new Vector2(2f)
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions =
                [
                    new Dimension()
                ],
                RowDimensions =
                [
                    new Dimension(GridSizeMode.Relative, 0.3f),
                    new Dimension(GridSizeMode.Relative, 0.55f),
                    new Dimension(GridSizeMode.Relative, 0.15f)
                ],
                Content = new[]
                {
                    new[] { Header },
                    new[] { Body },
                    new[] { Footer }
                }
            }
        ];
    }
}
