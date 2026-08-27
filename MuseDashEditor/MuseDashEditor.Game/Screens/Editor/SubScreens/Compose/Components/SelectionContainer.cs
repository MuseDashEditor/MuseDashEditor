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

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components;

public partial class SelectionContainer : Container<Container>
{
    private const float border = 10;

    public LaneContentContainer LaneContentContainer { get; set; } = null!;

    private int currentIndex;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        RelativeSizeAxes = Axes.Both;
    }

    public void UpdateSelection()
    {
        currentIndex = 0;

        foreach (var child in LaneContentContainer.Children)
        {
            if (child is null)
                continue;

            if (!child.GameObject.Selected.Value)
                continue;

            var container = getOrCreateObject();
            container.Alpha = 1;
            container.Position = child.Position;
            container.Size = child.DrawSize + new Vector2(border, border);
        }

        while (currentIndex < Count)
        {
            Children[currentIndex++].Alpha = 0;
        }
    }

    private Container getOrCreateObject()
    {
        Container child;

        if (currentIndex >= Count)
        {
            child = new Container
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                Masking = true,
                BorderThickness = 4,
                CornerRadius = 10,
                BorderColour = Colour4.Yellow,
                Alpha = 0,
                Depth = -1,
                Children =
                [
                    new Box
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        AlwaysPresent = true,
                        Alpha = 0
                    }
                ]
            };
            Add(child);
        }
        else
        {
            child = Children[currentIndex];
        }

        child.Alpha = 1;

        currentIndex++;

        return child;
    }
}
