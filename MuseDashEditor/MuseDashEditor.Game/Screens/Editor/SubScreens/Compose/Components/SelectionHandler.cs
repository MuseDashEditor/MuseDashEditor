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
using System.Collections.Generic;
using MuseDashEditor.Game.Data.Object;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Editor.Clock;
using osu.Framework.Allocation;
using osu.Framework.Extensions.PolygonExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components;

public partial class SelectionHandler(LaneContentContainer laneContentContainer) : Container
{
    [Resolved]
    private SelectionContainer selectionContainer { get; set; } = null!;

    [Resolved]
    private EditorClock editorClock { get; set; } = null!;

    private readonly HashSet<BaseObject> selectedObjects = [];
    private readonly HashSet<BaseObject> selectedBeforeDrag = [];

    private Vector2 startPosition;
    private Vector2 stopPosition;
    private bool isSelectionActive;
    private double selectionTimeStart;

    private readonly Container selectionBox = new()
    {
        Anchor = Anchor.TopLeft,
        Origin = Anchor.Centre,
        Masking = true,
        Alpha = 0,
        BorderColour = Colour4.Yellow,
        BorderThickness = 4,
        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Yellow.Opacity(0.1f),
        }
    };

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        Child = selectionBox;
    }

    private void unselectAll()
    {
        foreach (var selectedObject in selectedObjects)
        {
            selectedObject.Selected.Value = false;
        }

        selectedObjects.Clear();
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (e.ControlPressed)
            return false;

        if (base.OnClick(e))
            return false;

        unselectAll();
        selectionContainer.UpdateSelection();
        return false;
    }

    protected override void Update()
    {
        if (!isSelectionActive)
            return;

        var hoveredObjects = new HashSet<BaseObject>();
        var selectionQuad = selectionBox.ScreenSpaceDrawQuad;

        foreach (var baseLaneObject in laneContentContainer.Children)
        {
            if (!baseLaneObject.IsUsed)
                continue;

            if (baseLaneObject.ScreenSpaceDrawQuad.Intersects(selectionQuad))
            {
                hoveredObjects.Add(baseLaneObject.GameObject);
            }
        }

        foreach (var baseObject in selectedBeforeDrag)
        {
            hoveredObjects.Add(baseObject);
        }

        unselectAll();

        foreach (var selectedObject in hoveredObjects)
        {
            selectedObject.Selected.Value = true;
            selectedObjects.Add(selectedObject);
        }

        selectionContainer.UpdateSelection();
    }

    protected override void UpdateAfterChildren()
    {
        if (!isSelectionActive)
            return;

        var screenSpacePos = laneContentContainer.ToScreenSpace(new Vector2(
            laneContentContainer.ScrollContainer.PositionAtTime(selectionTimeStart),
            0
        ));
        startPosition.X = ToLocalSpace(screenSpacePos).X;

        var (minX, minY, maxX, maxY) = (
            MathF.Min(startPosition.X, stopPosition.X),
            MathF.Min(startPosition.Y, stopPosition.Y),
            MathF.Max(startPosition.X, stopPosition.X),
            MathF.Max(startPosition.Y, stopPosition.Y)
        );
        var centerPosition = new Vector2(
            minX + (maxX - minX) / 2,
            minY + (maxY - minY) / 2
        );
        var size = new Vector2(
            maxX - minX,
            maxY - minY
        );

        selectionBox.Position = centerPosition;
        selectionBox.Size = size;
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (base.OnDragStart(e))
            return false;

        foreach (var baseLaneObject in laneContentContainer.Children)
        {
            if (baseLaneObject.ScreenSpaceDrawQuad.Contains(e.ScreenSpaceMouseDownPosition))
                return false;
        }

        selectionBox.Alpha = 1;
        selectionBox.Position = startPosition = e.MouseDownPosition;
        selectionBox.Width = 0;
        selectionBox.Height = 0;

        selectedBeforeDrag.Clear();

        if (!e.ControlPressed)
        {
            unselectAll();
            selectionContainer.UpdateSelection();
        }
        else
        {
            foreach (var selectedObject in selectedObjects)
            {
                selectedBeforeDrag.Add(selectedObject);
            }
        }

        selectionTimeStart = laneContentContainer.ScrollContainer.TimeAtPosition(
            laneContentContainer.ToLocalSpace(
                e.ScreenSpaceMouseDownPosition
            ).X
        );
        isSelectionActive = true;

        return true;
    }

    protected override void OnDrag(DragEvent e)
    {
        stopPosition = e.MousePosition;
    }

    protected override void OnDragEnd(DragEndEvent e)
    {
        isSelectionActive = false;

        selectionBox.TransformTo("Alpha", 0f, 100);

        var hoveredObjects = new List<GameObject>();
        var selectionQuad = selectionBox.ScreenSpaceDrawQuad;

        foreach (var drawable in laneContentContainer.Children)
        {
            if (drawable.ScreenSpaceDrawQuad.Intersects(selectionQuad))
            {
                hoveredObjects.Add(drawable.GameObject);
            }
        }

        if (!e.ControlPressed)
        {
            unselectAll();
        }

        foreach (var selectedObject in hoveredObjects)
        {
            selectedObject.Selected.Value = true;
            selectedObjects.Add(selectedObject);
        }

        selectionContainer.UpdateSelection();
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
        }
        else
        {
            selectedObjects.Add(gameObject);
            gameObject.Selected.Value = true;
        }

        selectionContainer.UpdateSelection();
    }

    public void Unselect(GameObject selectedObject)
    {
        selectedObject.Selected.Value = false;
        selectedObjects.Remove(selectedObject);
    }
}
