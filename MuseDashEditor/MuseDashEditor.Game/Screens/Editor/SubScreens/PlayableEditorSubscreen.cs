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

    public override void Show()
    {
        base.Show();

        var currentTrackValue = EditorDataHolder.CurrentTrack.Value;
        if (currentTrackValue == null) return;
        currentTrackValue.Volume.Value = 0.7f; // TODO: config
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        if (ScrollContainer == null) return false;

        bool isPlaying = EditorClock.IsRunning;

        switch (e.ScrollDelta.Y) // TODO: Config to invert scroll direction
        {
            case > 0:
                scrollToPreviousBeat(e);
                break;
            case < 0:
                scrollToNextBeat(e);
                break;
        }

        if (isPlaying)
            EditorClock.Start();

        return true;
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
                var currentTime = ScrollContainer.GetCurrentOrTargetTime();
                if (currentTime <= 0) break;

                var nearestTimingPoint = EditorDataHolder.GetTimingPointAtTime(currentTime, true);
                if (nearestTimingPoint == null) break;

                ScrollContainer.ScrollToTime(nearestTimingPoint.Offset.Value, true);
                break;
            }
            case Key.PageDown:
            {
                var currentTime = ScrollContainer.GetCurrentOrTargetTime();
                if (currentTime >= EditorClock.TrackLength) break;

                var nextTimingPoint = EditorDataHolder.GetNextTimingPointAtTime(currentTime);
                if (nextTimingPoint == null) break;

                ScrollContainer.ScrollToTime(nextTimingPoint.Offset.Value, true);
                break;
            }
            case Key.Left:
            {
                scrollToPreviousBeat(e);
                break;
            }
            case Key.Right:
            {
                scrollToNextBeat(e);
                break;
            }
            case Key.Home:
                if (e.Repeat) return false;
                ScrollContainer.ScrollToTime(0, true);
                break;
            case Key.End:
                if (e.Repeat) return false;
                ScrollContainer.ScrollToTime(EditorClock.TrackLength, true);
                break;
            default:
                return base.OnKeyDown(e);
        }

        if (isPlaying)
            EditorClock.Start();

        return true;
    }

    private void scrollToNextBeat(UIEvent e)
    {
        if (ScrollContainer == null) return;

        var currentTime = ScrollContainer.GetCurrentOrTargetTime();
        if (currentTime >= EditorClock.TrackLength) return;

        var nearestTimingPoint = EditorDataHolder.GetTimingPointAtTime(currentTime);
        if (nearestTimingPoint == null) return;

        var nextTimingPoint = EditorDataHolder.GetNextTimingPointAtTime(currentTime);
        if (nextTimingPoint != null && Math.Abs(nextTimingPoint.Offset.Value - currentTime) < 1f)
        {
            nearestTimingPoint = nextTimingPoint;
            nextTimingPoint = EditorDataHolder.GetNextTimingPointAtTime(nextTimingPoint.Offset.Value);
        }

        double beatLength = 60_000 / nearestTimingPoint.NewBpm.Value;

        var subBeatCount = e.ControlPressed ? 1 : ScrollContainer.GetCurrentSubBeatDisplayedCount();
        var subBeatLength = beatLength / subBeatCount;

        double nearestTime = nearestTimingPoint.Offset.Value +
                             (Math.Floor((currentTime - nearestTimingPoint.Offset.Value) / subBeatLength) + 1) *
                             subBeatLength;

        if (Math.Abs(nearestTime - currentTime) < 1f)
            nearestTime += subBeatLength;

        if (nextTimingPoint != null && nearestTime > nextTimingPoint.Offset.Value)
            nearestTime = nextTimingPoint.Offset.Value;

        ScrollContainer.ScrollToTime(nearestTime, true);
    }

    private void scrollToPreviousBeat(UIEvent e)
    {
        if (ScrollContainer == null) return;

        var currentTime = ScrollContainer.GetCurrentOrTargetTime();
        if (currentTime <= 0) return;

        var nearestTimingPoint = EditorDataHolder.GetTimingPointAtTime(currentTime, true);
        if (nearestTimingPoint == null) return;

        double beatLength = 60_000 / nearestTimingPoint.NewBpm.Value;

        var subBeatCount = e.ControlPressed ? 1 : ScrollContainer.GetCurrentSubBeatDisplayedCount();
        var subBeatLength = beatLength / subBeatCount;

        double nearestTime = nearestTimingPoint.Offset.Value +
                             Math.Floor((currentTime - nearestTimingPoint.Offset.Value) / subBeatLength) * subBeatLength;

        if (Math.Abs(nearestTime - currentTime) < 1)
            nearestTime -= subBeatLength;

        if (nearestTime < nearestTimingPoint.Offset.Value)
            nearestTime = nearestTimingPoint.Offset.Value;

        ScrollContainer.ScrollToTime(nearestTime, true);
    }
}
