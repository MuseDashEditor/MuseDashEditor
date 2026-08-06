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
using MuseDashEditor.Game.Component.Common;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace MuseDashEditor.Game.Component.Notification;

public partial class SimpleConfirmationNotification : Notification
{
    public SimpleConfirmationNotification(
        LocalisableString title,
        LocalisableString message,
        Action onNoAction,
        Action onYesAction
    )
    {
        // AutoSizeAxes = Axes.Y;
        Width = 600;
        Height = 400;
        Content.CornerRadius = 20;
        Content.CornerExponent = 5f;

        Header = new FillFlowContainer
        {
            Direction = FillDirection.Vertical,
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Padding = new MarginPadding { Top = 15 },
            Children =
            [
                new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.4f, 0.4f),
                    Icon = FontAwesome.Solid.QuestionCircle
                },
                new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Font = FontUsage.Default.With(size: 40f),
                    Padding = new MarginPadding(10),
                    Text = title
                }
            ]
        };
        Body = new SpriteText
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Text = message
        };
        Footer = new GridContainer
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            RelativeSizeAxes = Axes.X,
            Height = 50,
            Padding = new MarginPadding(5),
            RowDimensions =
            [
                new Dimension()
            ],
            ColumnDimensions =
            [
                new Dimension(),
                new Dimension()
            ],
            Content = new[]
            {
                new Drawable[]
                {
                    new RoundedButton
                    {
                        RelativeSizeAxes = Axes.X,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Width = 0.9f,
                        Action = onNoAction,
                        Text = "No"
                    },
                    new RoundedButton
                    {
                        RelativeSizeAxes = Axes.X,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Width = 0.9f,
                        Action = onYesAction,
                        Text = "Yes"
                    }
                }
            }
        };
    }
}
