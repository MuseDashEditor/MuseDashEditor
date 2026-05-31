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

using System.Linq;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;

public partial class TimingPointsTableContainer : TableContainer
{
    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder, EditorClock clock)
    {
        RowSize = new Dimension(GridSizeMode.Absolute, 30);
        RelativeSizeAxes = Axes.Both;
        Columns =
        [
            new TimingPointsTableColumn(dimension: new Dimension(GridSizeMode.Absolute, 40)),
            new TimingPointsTableColumn("Point name", Anchor.CentreLeft),
            new TimingPointsTableColumn("Offset"),
            new TimingPointsTableColumn("Bpm"),
            new TimingPointsTableColumn("Actions")
        ];
        Content = buildRows(dataHolder, clock);
    }

    private static Drawable[,] buildRows(EditorDataHolder dataHolder, EditorClock editorClock)
    {
        return dataHolder.CurrentMap.Value.TimingPoints.Select((timingPoint, index) =>
        {
            var icon = new SpriteIcon
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(0.5f, 0.5f),
                Icon = FontAwesome.Solid.AngleRight,
                Alpha = 0
            };

            editorClock.OnTimeChanged += (time) =>
            {
                var nextPoint = index < dataHolder.CurrentMap.Value.TimingPoints.Count - 1 ? dataHolder.CurrentMap.Value.TimingPoints[index + 1] : null;

                if (time >= timingPoint.Offset && time < (nextPoint?.Offset ?? editorClock.TrackLength))
                    icon.FadeTo(1, 100);
                else
                    icon.FadeTo(0, 100);
            };

            return (Drawable[])
            [
                icon,
                new SpriteText
                {
                    Text = $"Timing point #{index + 1}"
                },
                new SpriteText
                {
                    Text = timingPoint.Offset.ToString("0.0000")
                },
                new SpriteText
                {
                    Text = timingPoint.NewBpm.ToString("0.0000")
                },
                new Box() // TODO
            ];
        }).ToArray().ToRectangular();
    }

    protected override Drawable CreateHeader(int index, TableColumn column)
    {
        return new Container
        {
            RelativeSizeAxes = Axes.X,
            Height = 40,
            Children =
            [
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = MdeColors.Background5
                },
                new SpriteText
                {
                    Anchor = column?.Anchor ?? Anchor.CentreLeft,
                    Origin = column?.Anchor ?? Anchor.CentreLeft,
                    Text = column?.Header ?? string.Empty,
                    Padding = new MarginPadding(10)
                }
            ]
        };
    }
}
