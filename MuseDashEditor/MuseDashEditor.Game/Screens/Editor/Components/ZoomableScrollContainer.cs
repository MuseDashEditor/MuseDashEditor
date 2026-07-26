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
using System.Linq;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Layout;
using osu.Framework.Utils;
using osuTK.Input;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class ZoomableScrollContainer : ZoomableScrollContainer<Drawable>
{
    private const float zoom_speed = 10f;
    private const float snap_distance = 50f;

    [Resolved] private EditorDataHolder editorDataHolder { get; set; } = null!;
    [Resolved] private EditorClock editorClock { get; set; } = null!;

    protected override Container<Drawable> Content => zoomedContent;

    public Action OnDrawWidthChanged = () => { };

    private readonly float? xCenter;
    private readonly Container zoomedContent;

    private readonly LayoutValue zoomedContentWidthCache = new(Invalidation.DrawSize);

    private float currentZoom = 1;
    private float minZoom;
    private float maxZoom;
    private bool handlingDragInput;
    private bool trackWasPlaying;
    private double lastScrollPosition;
    private double lastTrackTime;
    private bool isSliding;

    public ZoomableScrollContainer(
        Direction direction = Direction.Horizontal,
        float? xCenter = null
    ) : base(direction)
    {
        this.xCenter = xCenter;

        base.Content.Add(zoomedContent = new Container
        {
            AutoSizeAxes = Axes.Y,
            Alpha = 0
        });

        base.Content.RelativeSizeAxes = Axes.None;
        base.Content.AutoSizeAxes = Axes.Both;

        AddLayout(zoomedContentWidthCache);
    }

    public void SetupZoom(float initial, float minimum, float maximum)
    {
        minZoom = minimum;
        maxZoom = maximum;

        currentZoom = initial;
        zoomedContentWidthCache.Invalidate();

        zoomedContent.Show();
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        if (!e.ControlPressed)
            return false;

        if (editorClock.IsRunning)
            editorClock.Stop();

        var newZoom = Math.Clamp(
            currentZoom + e.ScrollDelta.Y * zoom_speed * (currentZoom > 5000 ? 10 : currentZoom > 1000 ? 5 : 1),
            minZoom,
            maxZoom
        );

        currentZoom = newZoom;
        updateZoomedContentWidth();

        Invalidate(Invalidation.DrawSize);

        Schedule(scrollToTrackTime); // Re-scroll to track time after zooming
        return true;
    }

    protected override void Update()
    {
        base.Update();

        Content.Margin = new MarginPadding { Horizontal = DrawWidth / 2 };

        if (editorClock is { IsRunning: true })
            scrollToTrackTime();

        if (isSliding)
            isSliding &= !Precision.AlmostEquals(Current, Target);
    }

    private void scrollToTrackTime()
    {
        if (editorClock.TrackLength == 0)
            return;

        float position = PositionAtTime(editorClock.CurrentTime);
        ScrollTo(position, false);
    }

    private void seekTrackToCurrent()
    {
        double target = TimeAtPosition(Current);
        editorClock.Seek(target);
    }

    public float GetMaxScrollPosition()
    {
        return PositionAtTime(editorClock.TrackLength);
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (!zoomedContentWidthCache.IsValid)
            updateZoomedContentWidth();

        if (handlingDragInput)
            seekTrackToCurrent();
        else if (!editorClock.IsRunning)
        {
            if (!Precision.AlmostEquals(Current, lastScrollPosition)
                && Precision.AlmostEquals(editorClock.CurrentTime, lastTrackTime)
                || isSliding)
                seekTrackToCurrent();
            else
                scrollToTrackTime();
        }

        lastScrollPosition = Current;
        lastTrackTime = editorClock.CurrentTime;
    }

    private void updateZoomedContentWidth()
    {
        zoomedContent.Width = DrawWidth * currentZoom;
        zoomedContent.X = xCenter ?? 0;
        zoomedContentWidthCache.Validate();

        OnDrawWidthChanged();

        Schedule(() => GC.Collect(0)); // Force GC due to waveform resampling creating a LOT of objects
    }

    protected override void OnUserScroll(double value, bool animated = true, double? distanceDecay = null)
    {
        // Cancel user scroll
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        return e.Button == MouseButton.Left;
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        return false;
    }

    protected override void OnDrag(DragEvent e)
    {
        // Cancel drag
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        // Cancel drag
        return true;
    }

    protected override void OnDragEnd(DragEndEvent e)
    {
        // Cancel drag
    }

    public void ScrollToTime(double time, bool animated = false)
    {
        if (isSliding)
        {
            ScrollTo(Current, false);
        }

        isSliding = true;

        if (time < 0) time = 0;
        if (time > editorClock.TrackLength) time = editorClock.TrackLength;

        editorClock.Seek(time);

        var position = PositionAtTime(editorClock.CurrentTime);
        ScrollTo(position, animated);
    }

    public double TimeAtPosition(double x)
    {
        return x / Content.DrawWidth * editorClock.TrackLength;
    }

    public float PositionAtTime(double time)
    {
        return (float)(time / editorClock.TrackLength * Content.DrawWidth);
    }

    public uint GetCurrentSubBeatDisplayedCount()
    {
        var subBeatCount = (uint)Math.Floor(currentZoom / 50);

        // Bit hack (https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2)
        subBeatCount--;
        subBeatCount |= subBeatCount >> 1;
        subBeatCount |= subBeatCount >> 2;
        subBeatCount |= subBeatCount >> 4;
        subBeatCount |= subBeatCount >> 8;
        subBeatCount |= subBeatCount >> 16;
        subBeatCount++;

        subBeatCount /= 2;

        if (subBeatCount <= 0)
            subBeatCount = 1;

        return subBeatCount;
    }

    public void SnapToNearestPreviousSubbeat()
    {
        var (nearestPreviousTime, _) = getNearestSubbeatBounds();
        ScrollToTime(nearestPreviousTime);
    }

    public void SnapToNearestSubbeat()
    {
        var currentPosition = TimeAtPosition(Current);
        var (nearestPreviousTime, nearestNextTime) = getNearestSubbeatBounds();

        var deltaToPrevious = currentPosition - nearestPreviousTime;
        var deltaToNext = nearestNextTime - currentPosition;

        if (deltaToPrevious < deltaToNext)
        {
            if (deltaToPrevious < snap_distance)
                ScrollToTime(nearestPreviousTime, true);
        }
        else if (deltaToNext < snap_distance)
            ScrollToTime(nearestNextTime, true);
    }

    private (double, double) getNearestSubbeatBounds()
    {
        var currentTime = TimeAtPosition(Current);
        if (currentTime < 0)
            currentTime = 0;

        var nearestTimingPoint = editorDataHolder.GetTimingPointAtTime(currentTime);
        if (nearestTimingPoint == null)
            return (0, editorClock.TrackLength);

        double beatLength = 60_000 / nearestTimingPoint.NewBpm.Value;

        var subBeatCount = GetCurrentSubBeatDisplayedCount();
        var subBeatLength = beatLength / subBeatCount;

        double nearestPreviousTime = nearestTimingPoint.Offset.Value +
                                     Math.Floor((currentTime - nearestTimingPoint.Offset.Value) / subBeatLength) *
                                     subBeatLength;

        if (nearestPreviousTime < nearestTimingPoint.Offset.Value)
            nearestPreviousTime = nearestTimingPoint.Offset.Value;

        double nearestNextTime = nearestPreviousTime + subBeatLength;

        var nextTimingPoint = editorDataHolder.GetNextTimingPointAtTime(nearestNextTime);

        if (nextTimingPoint != null && nearestNextTime > nextTimingPoint.Offset.Value)
            nearestNextTime = nextTimingPoint.Offset.Value;

        return (nearestPreviousTime, nearestNextTime);
    }

    public double GetCurrentOrTargetTime()
    {
        return TimeAtPosition(isSliding ? Target : Current);
    }
}

public partial class ZoomableScrollContainer<T> : ScrollContainer<T>
    where T : Drawable
{
    protected ZoomableScrollContainer(Direction direction) : base(direction)
    {
    }

    protected override ScrollbarContainer CreateScrollbar(Direction direction)
    {
        return new ZoomableScrollbarContainer(direction);
    }

    protected partial class ZoomableScrollbarContainer(Direction direction) : ScrollbarContainer(direction)
    {
        public override void ResizeTo(float val, int duration = 0, Easing easing = Easing.None)
        {
        }
    }
}
