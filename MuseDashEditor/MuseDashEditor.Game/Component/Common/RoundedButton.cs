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
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK.Graphics;

namespace MuseDashEditor.Game.Component.Common;

public partial class RoundedButton : Button
{
    public LocalisableString Text
    {
        get => spriteText.Text;
        set => spriteText.Text = value;
    }

    public Color4? BackgroundColour
    {
        get => background.Colour;
        set => background.Colour = value ?? MdeColors.Dark3;
    }

    private readonly SpriteText spriteText = new()
    {
        Depth = -1,
        Origin = Anchor.Centre,
        Anchor = Anchor.Centre
    };

    private readonly Box background = new()
    {
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
        RelativeSizeAxes = Axes.Both,
        Depth = float.MaxValue,
        Colour = MdeColors.Dark3
    };

    private Box hover = null!;
    private Box flashLayer = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Content.CornerRadius = 10;
        Content.CornerExponent = 2.5f;

        Height = 40;

        AddInternal(new Container
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Masking = true,
            CornerRadius = 5,
            RelativeSizeAxes = Axes.Both,
            Children =
            [
                background,
                hover = new Box
                {
                    Alpha = 0,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.White,
                    Blending = BlendingParameters.Additive,
                    Depth = float.MinValue
                },
                spriteText,
                flashLayer = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Blending = BlendingParameters.Additive,
                    Depth = float.MinValue,
                    Colour = Color4.White.Opacity(0.5f),
                    Alpha = 0,
                }
            ]
        });
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Enabled.Value)
            flashLayer.FadeOutFromOne(800, Easing.OutQuint);

        return base.OnClick(e);
    }

    protected virtual float HoverLayerFinalAlpha => 0.1f;

    protected override bool OnHover(HoverEvent e)
    {
        if (Enabled.Value)
        {
            hover.FadeTo(0.2f, 40, Easing.OutQuint)
                .Then()
                .FadeTo(HoverLayerFinalAlpha, 800, Easing.OutQuint);
        }

        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        base.OnHoverLost(e);

        hover.FadeOut(800, Easing.OutQuint);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        Content.ScaleTo(0.9f, 4000, Easing.OutQuint);
        return base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        Content.ScaleTo(1, 1000, Easing.OutElastic);
        base.OnMouseUp(e);
    }
}
