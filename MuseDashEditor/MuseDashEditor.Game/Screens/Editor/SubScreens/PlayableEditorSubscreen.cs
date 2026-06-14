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
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Screens.Editor.Components;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens;

public partial class PlayableEditorSubscreen : EditorSubscreen
{
    [Resolved] protected EditorClock EditorClock { get; private set; } = null!;
    [Resolved] protected EditorDataHolder EditorDataHolder { get; private set; } = null!;

    protected ZoomableScrollContainer? ScrollContainer;

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder editorDataHolder)
    {
        editorDataHolder.SelectedSubscreen.BindValueChanged(screenChangedEvent =>
        {
            if (
                screenChangedEvent.NewValue != EditorSubscreenType.Compose
                && screenChangedEvent.NewValue != EditorSubscreenType.Design
                && screenChangedEvent.NewValue != EditorSubscreenType.Timing
            )
                EditorClock.Stop();
        });
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (ScrollContainer == null) return false;

        bool isPlaying = EditorClock.IsRunning;

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (e.Key)
        {
            case Key.Space:
            {
                if (e.Repeat) return false;

                if (EditorClock.IsRunning)
                    EditorClock.Stop();
                else
                    EditorClock.Start();
                return true;
            }
            case Key.PageUp:
            {
                var currentTime = EditorClock.CurrentTime;
                if (currentTime <= 0) break;

                var nearestTimingPoint = EditorDataHolder.GetTimingPointAtTime(currentTime, true);
                if (nearestTimingPoint == null) break;

                ScrollContainer.ScrollToTime(nearestTimingPoint.Offset);
                break;
            }
            case Key.PageDown:
            {
                var currentTime = EditorClock.CurrentTime;
                if (currentTime >= EditorClock.TrackLength) break;

                var nextTimingPoint = EditorDataHolder.GetNextTimingPointAtTime(currentTime);
                if (nextTimingPoint == null) break;

                ScrollContainer.ScrollToTime(nextTimingPoint.Offset);
                break;
            }
            case Key.Left:
            {
                var currentTime = EditorClock.CurrentTime;
                if (currentTime <= 0) break;

                var nearestTimingPoint = EditorDataHolder.GetTimingPointAtTime(currentTime, true);
                if (nearestTimingPoint == null) break;

                double beatLength = 60_000 / nearestTimingPoint.NewBpm;
                double nearestTime = nearestTimingPoint.Offset +
                                     Math.Floor((currentTime - nearestTimingPoint.Offset) / beatLength) * beatLength;

                if (Math.Abs(nearestTime - currentTime) < 0.01f)
                    nearestTime -= beatLength;

                if (nearestTime < nearestTimingPoint.Offset)
                    nearestTime = nearestTimingPoint.Offset;

                ScrollContainer.ScrollToTime(nearestTime);
                break;
            }
            case Key.Right:
            {
                var currentTime = EditorClock.CurrentTime;
                if (currentTime >= EditorClock.TrackLength) break;

                var nearestTimingPoint = EditorDataHolder.GetTimingPointAtTime(currentTime);
                if (nearestTimingPoint == null) break;

                double beatLength = 60_000 / nearestTimingPoint.NewBpm;
                double nearestTime = nearestTimingPoint.Offset +
                                     (Math.Floor((currentTime - nearestTimingPoint.Offset) / beatLength) + 1) *
                                     beatLength;

                if (Math.Abs(nearestTime - currentTime) < 0.01f)
                    nearestTime += beatLength;

                var nextTimingPoint = EditorDataHolder.GetNextTimingPointAtTime(currentTime);
                if (nextTimingPoint != null && nearestTime > nextTimingPoint.Offset)
                    nearestTime = nextTimingPoint.Offset;

                ScrollContainer.ScrollToTime(nearestTime);
                break;
            }
            case Key.Home:
                if (e.Repeat) return false;
                ScrollContainer.ScrollToTime(0);
                break;
            case Key.End:
                if (e.Repeat) return false;
                ScrollContainer.ScrollToTime(EditorClock.TrackLength);
                break;
            default:
                return base.OnKeyDown(e);
        }

        if (isPlaying)
            EditorClock.Start();

        return true;
    }
}
