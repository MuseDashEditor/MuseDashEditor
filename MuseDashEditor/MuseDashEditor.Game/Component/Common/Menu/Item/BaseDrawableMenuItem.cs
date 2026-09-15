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
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;

namespace MuseDashEditor.Game.Component.Common.Menu.Item;

public partial class BaseDrawableMenuItem : osu.Framework.Graphics.UserInterface.Menu.DrawableMenuItem
{
    protected const int MARGIN_HORIZONTAL = 10;
    protected const int MARGIN_VERTICAL = 4;
    protected const int TEXT_SIZE = 22;
    protected const int TRANSITION_LENGTH = 80;

    protected TextContainer Text = null!;

    protected BaseDrawableMenuItem(MenuItem item)
        : base(item)
    {
        BackgroundColour = Colour4.Transparent;
    }

    protected override Drawable CreateContent() => Text = new TextContainer();

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Foreground.Anchor = Anchor.CentreLeft;
        Foreground.Origin = Anchor.CentreLeft;
        Item.Action.BindDisabledChanged(_ => updateState(), true);
    }

    protected override bool OnHover(HoverEvent e)
    {
        updateState();
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        updateState();
        base.OnHoverLost(e);
    }

    private void updateState()
    {
        Alpha = IsActionable ? 1 : 0.2f;

        if (IsHovered && IsActionable)
        {
            Text.BoldText.FadeIn(TRANSITION_LENGTH, Easing.OutQuint);
            Text.NormalText.FadeOut(TRANSITION_LENGTH, Easing.OutQuint);
        }
        else
        {
            Text.BoldText.FadeOut(TRANSITION_LENGTH, Easing.OutQuint);
            Text.NormalText.FadeIn(TRANSITION_LENGTH, Easing.OutQuint);
        }
    }

    protected override void UpdateBackgroundColour()
    {
        if (State == MenuItemState.Selected)
            Background.FadeColour(BackgroundColourHover);
        else
            base.UpdateBackgroundColour();
    }

    protected override void UpdateForegroundColour()
    {
        if (State == MenuItemState.Selected)
            Foreground.FadeColour(ForegroundColourHover);
        else
            base.UpdateForegroundColour();
    }

    // Copied from https://github.com/ppy/osu/blob/c729c6d7a2a1e3ce0d7118af3dce8ac9192d31c9/osu.Game/Screens/Edit/Components/Menus/EditorMenuBar.cs
    protected partial class TextContainer : Container, IHasText
    {
        public LocalisableString Text
        {
            get => NormalText.Text;
            set
            {
                NormalText.Text = value;
                BoldText.Text = value;
            }
        }

        public readonly SpriteText NormalText;
        public readonly SpriteText BoldText;
        public readonly Container Container;

        public TextContainer()
        {
            AutoSizeAxes = Axes.Both;

            Child = Container = new Container
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,

                AutoSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = MARGIN_HORIZONTAL, Vertical = MARGIN_VERTICAL, },

                Children =
                [
                    NormalText = new SpriteText
                    {
                        AlwaysPresent = true, // ensures that the menu item does not change width when switching between normal and bold text.
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Font = MDEFonts.ArialUnicodeMs.With(size: TEXT_SIZE),
                    },
                    BoldText = new SpriteText
                    {
                        AlwaysPresent = true, // ensures that the menu item does not change width when switching between normal and bold text.
                        Alpha = 0,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Font = MDEFonts.ArialUnicodeMs.With(size: TEXT_SIZE, weight: "Bold"),
                    }
                ]
            };
        }
    }
}
