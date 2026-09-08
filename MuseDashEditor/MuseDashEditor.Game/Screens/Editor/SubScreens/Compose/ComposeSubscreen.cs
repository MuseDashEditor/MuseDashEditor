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

using System.Collections.Generic;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Input;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Compose.Components;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose;

public partial class ComposeSubscreen : PlayableEditorSubscreen
{
    private readonly TimingTrack timingTrack;
    private readonly LaneContentContainer laneContentContainer;

    [Resolved]
    private EditorDataHolder editorDataHolder { get; set; } = null!;

    [Cached]
    private readonly SelectionHandler selectionHandler;

    [Cached]
    private readonly SelectionContainer selectionContainer = new();

    public ComposeSubscreen()
    {
        timingTrack = new TimingTrack(-900)
        {
            AutoSizeAxes = Axes.Y,
            Origin = Anchor.CentreLeft,
            Anchor = Anchor.CentreLeft,
            Depth = 1
        };
        laneContentContainer = new LaneContentContainer
        {
            RelativeSizeAxes = Axes.X,
            Height = EditorConstants.TOTAL_LANES_HEIGHT,
            Origin = Anchor.CentreLeft,
            Anchor = Anchor.CentreLeft,
            ScrollContainer = timingTrack.ZoomableScrollContainer,
            Depth = -20,
        };
        selectionHandler = new SelectionHandler(laneContentContainer);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Container laneBackgrounds;

        InternalChildren =
        [
            laneBackgrounds = new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
                Depth = 2
            },
            timingTrack,
            selectionHandler
        ];

        foreach (var laneType in EditorConstants.ORDERED_LANE_TYPES)
        {
            laneBackgrounds.Add(new Box
            {
                RelativeSizeAxes = Axes.X,
                Height = EditorConstants.LANE_HEIGHT,
                Origin = Anchor.CentreLeft,
                Anchor = Anchor.CentreLeft,
                Y = EditorConstants.GetLaneY(laneType),
                Alpha = 0.1f,
                Colour = MdeColors.GetLaneColor(laneType)
            });
        }

        timingTrack.ZoomableScrollContainer.Add(laneContentContainer);
        timingTrack.ZoomableScrollContainer.Add(selectionContainer);

        timingTrack.ZoomableScrollContainer.Width = 1f;
        timingTrack.WaveformGraph.Alpha = 0; // TODO: add setting
        timingTrack.TimingTrackTickDisplay.Height = 1f;
        timingTrack.TimingTrackTickDisplay.ShouldPlayTickSound = false; // TODO: add setting

        ScrollContainer = timingTrack.ZoomableScrollContainer;
    }

    public override bool OnPressed(KeyBindingPressEvent<InputAction> e)
    {
        List<GameObject> selectedObjects = [];

        foreach (var gameObject in editorDataHolder.CurrentMap.Value.GameObjects)
        {
            if (gameObject.Selected.Value)
                selectedObjects.Add(gameObject);
        }

        switch (e.Action)
        {
            case InputAction.Delete:
                foreach (var selectedObject in selectedObjects)
                {
                    delete(selectedObject);
                }

                laneContentContainer.Invalidate();
                selectionContainer.UpdateSelection();
                break;

            case InputAction.Flip:
                foreach (var selectedObject in selectedObjects)
                {
                    flip(selectedObject);
                }

                laneContentContainer.Invalidate();
                selectionContainer.UpdateSelection();
                break;

            case InputAction.SelectAllVisible:
            case InputAction.SelectAll:
            case InputAction.Copy:
            case InputAction.Cut:
            case InputAction.Paste:
            case InputAction.ZoomIn:
            case InputAction.ZoomOut:
                return true;

            default:
                return base.OnPressed(e);
        }

        return true;
    }

    private void flip(GameObject selectedObject)
    {
        // TODO
    }

    private void delete(GameObject selectedObject, bool secondGemini = false)
    {
        editorDataHolder.CurrentMap.Value.GameObjects.Remove(selectedObject);

        if (selectedObject.LaneObject != null)
        {
            selectedObject.LaneObject.IsUsed = false;
            selectedObject.LaneObject = null;
        }

        if (selectedObject.Selected.Value)
            selectionHandler.Unselect(selectedObject);

        if (!secondGemini && selectedObject.GeminiPairObject != null)
            delete(selectedObject.GeminiPairObject, true);

        if (selectedObject.HoldEndObject != null)
            delete(selectedObject.HoldEndObject);
    }
}
