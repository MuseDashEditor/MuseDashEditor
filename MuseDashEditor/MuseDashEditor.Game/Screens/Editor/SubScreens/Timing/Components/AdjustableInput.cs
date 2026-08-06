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

using System;
using System.Globalization;
using MuseDashEditor.Game.Data.Holder;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;

public partial class AdjustableInput : FillFlowContainer
{
    public required BindableDouble Value { get; init; }
    public double MinValue { get; init; }
    public double MaxValue { get; init; }

    public double SmallStep { get; init; }
    public double LargeStep { get; init; }

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder)
    {
        Direction = FillDirection.Horizontal;

        var textBox = new BasicTextBox
        {
            Size = new Vector2(150, 30),
            Text = Value.Value.ToString(CultureInfo.CurrentCulture),
            InputProperties = new TextInputProperties(TextInputType.Decimal),
            CommitOnFocusLost = true
        };

        textBox.OnCommit += (_, newText) =>
        {
            if (!newText) return;

            if (double.TryParse(textBox.Text, out var newValue))
            {
                Value.Value = Math.Clamp(newValue, MinValue, MaxValue);
            }
        };
        Value.BindValueChanged(newValue =>
        {
            textBox.Text = newValue.NewValue.ToString(CultureInfo.CurrentCulture);
            dataHolder.OnTimingPointsChanged();
        }, true);

        Children =
        [
            new BasicButton
            {
                Size = new Vector2(30, 30),
                Action = () => Value.Value = Math.Max(MinValue, Value.Value - LargeStep),
            }.With(b => b.Add(new SpriteIcon
            {
                Icon = FontAwesome.Solid.AngleDoubleLeft,
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(0.5f, 0.5f),
            })),
            new BasicButton
            {
                Size = new Vector2(30, 30),
                Action = () => Value.Value = Math.Max(MinValue, Value.Value - SmallStep),
            }.With(b => b.Add(new SpriteIcon
            {
                Icon = FontAwesome.Solid.AngleLeft,
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(0.4f, 0.4f),
            })),
            textBox,
            new BasicButton
            {
                Size = new Vector2(30, 30),
                Action = () => Value.Value = Math.Min(MaxValue, Value.Value + SmallStep),
            }.With(b => b.Add(new SpriteIcon
            {
                Icon = FontAwesome.Solid.AngleRight,
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(0.4f, 0.4f),
            })),
            new BasicButton
            {
                Size = new Vector2(30, 30),
                Action = () => Value.Value = Math.Min(MaxValue, Value.Value + LargeStep),
            }.With(b => b.Add(new SpriteIcon
            {
                Icon = FontAwesome.Solid.AngleDoubleRight,
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(0.5f, 0.5f),
            })),
        ];
    }

}
