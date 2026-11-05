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

using System;
using MuseDashEditor.Game.Component;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata.Components;

public partial class MetadataField(MetadataFieldsContainers fieldsContainer) : CompositeComponent
{
    public string Label { private get; init; } = null!;
    public Func<ChartInfo, Bindable<string>> FieldGetter { private get; init; } = null!;

    private BasicTextBox textBox = null!;

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder)
    {
        AutoSizeAxes = Axes.Y;
        Width = 300f;

        InternalChildren =
        [
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 5,
                CornerExponent = 2.5f,
                BorderThickness = 2.5f,
                Child = new Box
                {
                    Colour = MdeColors.Dark6,
                    RelativeSizeAxes = Axes.Both,
                }
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(9),
                Spacing = new Vector2(0, 4),

                Children =
                [
                    new SpriteText
                    {
                        Text = Label
                    },
                    textBox = new TransparentTextBox
                    {
                        RelativeSizeAxes = Axes.X,
                        Size = new Vector2(1f, 60),
                        TabbableContentContainer = fieldsContainer
                    }
                ]
            }
        ];

        dataHolder.CurrentChart.BindValueChanged(loadNewChart, true);
    }

    protected override bool OnClick(ClickEvent e)
    {
        GetContainingFocusManager()?.ChangeFocus(textBox);
        return true;
    }

    private void loadNewChart(ValueChangedEvent<Chart?> evt)
    {
        var chart = evt.NewValue;
        if (chart == null) return;

        var chartInfo = chart.ChartInfo;

        var field = FieldGetter(chartInfo);

        textBox.Current.BindTo(field);
    }
}
