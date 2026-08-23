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
using MuseDashEditor.Game.Data.Object;
using MuseDashEditor.Game.Data.Object.GameObject;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components;

public partial class SelectionHandler : Container<Container>
{
    private const float border = 10;

    [Resolved] private EditorDataHolder dataHolder { get; set; } = null!;

    public LaneContentContainer LaneContentContainer { get; set; } = null!;

    private readonly BindableList<BaseObject> selectedObjects = [];
    private int currentIndex;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        RelativeSizeAxes = Axes.Both;
    }

    public void Select(GameObject gameObject, bool addToSelection)
    {
        var wasSelected = gameObject.Selected.Value;

        if (!addToSelection)
        {
            unselectAll();
        }

        if (wasSelected)
        {
            selectedObjects.Remove(gameObject);
            gameObject.Selected.Value = false;
            UpdateSelection();
            return;
        }

        selectedObjects.Add(gameObject);
        gameObject.Selected.Value = true;

        UpdateSelection();
    }

    private void unselectAll()
    {
        foreach (var selectedObject in selectedObjects)
        {
            selectedObject.Selected.Value = false;
        }

        selectedObjects.Clear();

        foreach (var container in Children)
        {
            container.Alpha = 0;
        }
    }

    public void UpdateSelection()
    {
        currentIndex = 0;

        if (selectedObjects.Count > 0)
        {
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
        }

        while (currentIndex < Count)
        {
            Children[currentIndex++].Alpha = 0;
        }
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (e.ControlPressed)
            return false;

        unselectAll();
        UpdateSelection();

        return true;
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
