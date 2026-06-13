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
using MuseDashEditor.Game.Editor.Clock;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Layout;
using osuTK;
using osuTK.Input;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class ZoomableScrollContainer : ZoomableScrollContainer<Drawable>
{
    private const float zoom_speed = 0.02f;

    [Resolved] private EditorClock editorClock { get; set; } = null!;

    public Action OnDrawWidthChanged = () => {};

    private readonly Container zoomedContent;
    protected override Container<Drawable> Content => zoomedContent;

    private readonly LayoutValue zoomedContentWidthCache = new(Invalidation.DrawSize);

    private float? xCenter;
    private float currentZoom = 1;
    private float minZoom;
    private float maxZoom;
    private bool handlingDragInput;
    private bool trackWasPlaying;

    public ZoomableScrollContainer(Direction direction = Direction.Horizontal) : base(direction)
    {
        base.Content.Add(zoomedContent = new Container
        {
            RelativeSizeAxes = Axes.Y,
            Alpha = 0
        });

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
        if (!e.ControlPressed) return base.OnScroll(e);

        if (editorClock?.IsRunning == true) editorClock.Stop();

        var newZoom = Math.Clamp(currentZoom + e.ScrollDelta.Y * (maxZoom - minZoom) * zoom_speed, minZoom, maxZoom);
        var focusPoint = xCenter ?? zoomedContent.ToLocalSpace(ToScreenSpace(new Vector2(DrawWidth / 2, 0))).X;
        var focusOffset = focusPoint - (float)Current;
        var expectedWidth = DrawWidth * newZoom;
        var targetOffset = expectedWidth * (focusPoint / zoomedContent.DrawWidth) - focusOffset;

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
    }

    private void scrollToTrackTime()
    {
        if (editorClock.TrackLength == 0)
            return;

        float position = PositionAtTime(editorClock.CurrentTime);
        ScrollTo(position, false);

        xCenter = position;
        // OnTimeChange(editorClock.CurrentTime);
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (!zoomedContentWidthCache.IsValid)
            updateZoomedContentWidth();
    }

    private void updateZoomedContentWidth()
    {
        zoomedContent.Width = DrawWidth * currentZoom;
        zoomedContentWidthCache.Validate();

        OnDrawWidthChanged();
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        beginUserDrag();
        return e.Button == MouseButton.Left;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        endUserDrag();
        base.OnMouseUp(e);
    }

    private void beginUserDrag()
    {
        if (handlingDragInput) return;

        handlingDragInput = true;
        trackWasPlaying = editorClock.IsRunning;
        editorClock.Stop();
    }

    private void endUserDrag()
    {
        if (!handlingDragInput) return;

        handlingDragInput = false;

        var time = TimeAtPosition(Current);
        if (time < 0) time = 0;
        if (time > editorClock.TrackLength) time = editorClock.TrackLength;

        editorClock.Seek(Math.Min(editorClock.TrackLength, time));

        var position = PositionAtTime(editorClock.CurrentTime);
        ScrollTo(position, false);
        xCenter = position;

        if (trackWasPlaying)
            editorClock.Start();
    }

    public double TimeAtPosition(double x)
    {
        return x / Content.DrawWidth * editorClock.TrackLength;
    }

    public float PositionAtTime(double time)
    {
        return (float)(time / editorClock.TrackLength * Content.DrawWidth);
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
