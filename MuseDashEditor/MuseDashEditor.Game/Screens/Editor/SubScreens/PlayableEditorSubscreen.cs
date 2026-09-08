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
using MuseDashEditor.Game.Input;
using MuseDashEditor.Game.Screens.Editor.Components;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens;

public partial class PlayableEditorSubscreen : EditorSubscreen, IKeyBindingHandler<InputAction>
{
    [Resolved]
    protected EditorClock EditorClock { get; private set; } = null!;

    [Resolved]
    protected EditorDataHolder EditorDataHolder { get; private set; } = null!;

    protected ZoomableScrollContainer? ScrollContainer;
    private double playInitialTime;

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

    private void scrollToNextBeat(bool isLargeJump)
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

        var subBeatCount = isLargeJump ? 1 : ScrollContainer.GetCurrentSubBeatDisplayedCount();
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

    private void scrollToPreviousBeat(bool isLargeJump)
    {
        if (ScrollContainer == null) return;

        var currentTime = ScrollContainer.GetCurrentOrTargetTime();
        if (currentTime <= 0) return;

        var nearestTimingPoint = EditorDataHolder.GetTimingPointAtTime(currentTime, true);
        if (nearestTimingPoint == null) return;

        double beatLength = 60_000 / nearestTimingPoint.NewBpm.Value;

        var subBeatCount = isLargeJump ? 1 : ScrollContainer.GetCurrentSubBeatDisplayedCount();
        var subBeatLength = beatLength / subBeatCount;

        double nearestTime = nearestTimingPoint.Offset.Value +
                             Math.Floor((currentTime - nearestTimingPoint.Offset.Value) / subBeatLength) * subBeatLength;

        if (Math.Abs(nearestTime - currentTime) < 1)
            nearestTime -= subBeatLength;

        if (nearestTime < nearestTimingPoint.Offset.Value)
            nearestTime = nearestTimingPoint.Offset.Value;

        ScrollContainer.ScrollToTime(nearestTime, true);
    }

    public virtual bool OnPressed(KeyBindingPressEvent<InputAction> e)
    {
        if (ScrollContainer == null)
            return false;

        bool isPlaying = EditorClock.IsRunning;

        switch (e.Action)
        {
            case InputAction.PlaybackPlayPause:
                if (!isPlaying)
                {
                    playInitialTime = EditorClock.CurrentTime;
                    EditorClock.Start();
                }
                else
                {
                    EditorClock.Stop();
                    ScrollContainer.ScrollToTime(playInitialTime);
                }

                break;

            case InputAction.PlaybackPauseNoBack:
                if (isPlaying)
                    EditorClock.Stop();
                break;

            case InputAction.PlaybackGoToStart:
                ScrollContainer.ScrollToTime(0);
                break;

            case InputAction.PlaybackGoToEnd:
                ScrollContainer.ScrollToTime(EditorClock.TrackLength);
                break;

            case InputAction.NextBeat:
            case InputAction.NextBeat2:
                scrollToNextBeat(false);
                break;

            case InputAction.NextFirstBeat:
                scrollToNextBeat(true);
                break;

            case InputAction.PreviousBeat:
            case InputAction.PreviousBeat2:
                scrollToPreviousBeat(false);
                break;

            case InputAction.PreviousFirstBeat:
                scrollToPreviousBeat(true);
                break;

            case InputAction.NextTimingPoint:
                ScrollContainer.ScrollToTime(
                    EditorDataHolder.GetNextTimingPointAtTime(
                        ScrollContainer.GetCurrentOrTargetTime()
                    )?.Offset.Value ?? EditorClock.TrackLength,
                    true
                );
                break;

            case InputAction.PreviousTimingPoint:
                ScrollContainer.ScrollToTime(
                    EditorDataHolder.GetTimingPointAtTime(
                        ScrollContainer.GetCurrentOrTargetTime(),
                        true
                    )?.Offset.Value ?? 0,
                    true
                );
                break;

            default:
                return false;
        }

        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<InputAction> e)
    {
    }
}
