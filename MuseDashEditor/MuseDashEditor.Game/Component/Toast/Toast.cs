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

using MuseDashEditor.Game.Component.Common;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace MuseDashEditor.Game.Component.Toast;

public partial class Toast : Container
{
    protected virtual Colour4 ContainerColour => MdeColors.Dark6;
    protected virtual IconUsage? Icon => FontAwesome.Regular.Bell;
    protected virtual LocalisableString Message => "Ping!";
    protected virtual bool CanBeClosed => true;
    protected virtual double DurationSeconds => 5;

    internal ToastContainer? ToastContainer;
    private IconRoundedButton? closeButton;
    private Box? durationBar;

    [BackgroundDependencyLoader]
    private void load()
    {
        Origin = Anchor.TopRight;
        Anchor = Anchor.TopRight;

        Masking = true;
        BorderColour = ContainerColour;
        BorderThickness = 2;
        CornerExponent = 2.5f;
        CornerRadius = 10;

        Width = 250;
        AutoSizeAxes = Axes.Y;

        Children =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ContainerColour.Darken(0.2f)
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(10),
                ColumnDimensions =
                [
                    new Dimension(GridSizeMode.Absolute, Icon is not null ? 20 : 0),
                    new Dimension(),
                    new Dimension(GridSizeMode.Absolute, CanBeClosed ? 20 : 0),
                ],
                RowDimensions =
                [
                    new Dimension(GridSizeMode.AutoSize)
                ],
                Content =
                    new[]
                    {
                        new Drawable?[]
                        {
                            Icon is not null
                                ? new SpriteIcon
                                {
                                    Height = 20,
                                    Width = 20,
                                    Icon = Icon!.Value,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                }
                                : null,
                            new SpriteText
                            {
                                RelativeSizeAxes = Axes.X,
                                BypassAutoSizeAxes = Axes.Y,
                                AllowMultiline = true,
                                Padding = new MarginPadding { Horizontal = 5 },
                                Text = Message
                            },
                            CanBeClosed
                                ? closeButton = new IconRoundedButton
                                {
                                    AlwaysPresent = true,
                                    Alpha = 0,
                                    BackgroundColour = Colour4.Transparent,
                                    Icon = FontAwesome.Solid.WindowClose,
                                    Size = new Vector2(20),
                                    Action = Close
                                }
                                : null
                        }
                    }
            }
        ];

        if (!(DurationSeconds > 0))
            return;

        durationBar = new Box
        {
            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft,
            RelativeSizeAxes = Axes.X,
            Height = 5,
            Colour = ContainerColour.Lighten(2f),
            Width = 1
        };
        Add(durationBar);

        durationBar.TransformTo(nameof(Width), 0f, DurationSeconds * 1000)
                   .OnComplete(_ => Close());
    }

    protected void Close()
    {
        ToastContainer?.RemoveToast(this);
    }

    protected override bool OnHover(HoverEvent e)
    {
        suspendTimeout();
        closeButton?.TransformTo(nameof(Alpha), 1f, 150f);

        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        resumeTimeout();
        closeButton?.TransformTo(nameof(Alpha), 0f, 150f);
    }

    private void suspendTimeout()
    {
        if (DurationSeconds == 0 || durationBar is null)
            return;

        durationBar.ClearTransforms(false, nameof(Width));
        durationBar.TransformTo(nameof(Alpha), 0f, 50f);
    }

    private void resumeTimeout()
    {
        if (DurationSeconds == 0 || durationBar is null)
            return;

        durationBar.Width = 1;
        durationBar.TransformTo(nameof(Alpha), 1f, 50f);
        durationBar.TransformTo(nameof(Width), 0f, DurationSeconds * 1000)
                   .OnComplete(_ => Close());
    }
}
