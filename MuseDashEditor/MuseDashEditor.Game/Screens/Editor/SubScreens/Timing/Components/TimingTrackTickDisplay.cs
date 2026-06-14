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
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Object.MappingObject;
using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Screens.Editor.Components;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Caching;
using osu.Framework.Graphics.Containers;
using osuTK.Graphics;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;

public partial class TimingTrackTickDisplay : Container<TimingPointTick>
{
    [Resolved] private EditorDataHolder dataHolder { get; set; } = null!;
    [Resolved] private EditorClock editorClock { get; set; } = null!;

    public required ZoomableScrollContainer ScrollContainer { get; set; }
    public bool ShouldPlayTickSound { get; set; } = true;

    private readonly Cached tickCache = new();

    private float currentMinRange = float.MinValue;
    private float currentMaxRange = float.MaxValue;
    private float? nextMinTick;
    private float? nextMaxTick;
    private int currentTickIndex;

    private Sample? tickSample;
    private double lastPlayedTickOffset;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        tickSample = audio.Samples.Get("tick");
        tickSample.Volume.Value = 2f;

        dataHolder.CurrentMap.ValueChanged += _ => tickCache.Invalidate();

        editorClock.OnSeek += () =>
        {
            tickCache.Invalidate();
            lastPlayedTickOffset = editorClock.CurrentTime;
        };

        ScrollContainer.OnDrawWidthChanged += () =>
        {
            tickCache.Invalidate();
        };

        // TODO: OnTimingPointChanged
    }

    protected override void Update()
    {
        base.Update();

        if (DrawWidth <= 0) return;

        var screenSpacePosTopLeft = ScrollContainer.ScreenSpaceDrawQuad.TopLeft;
        var screenSpacePosTopRight = ScrollContainer.ScreenSpaceDrawQuad.TopRight;

        var localSpacePosTopLeft = ToLocalSpace(screenSpacePosTopLeft);
        var localSpacePosTopRight = ToLocalSpace(screenSpacePosTopRight);

        var minRange = localSpacePosTopLeft.X - TimingPointTick.MAX_WIDTH * 2;
        var maxRange = localSpacePosTopRight.X + TimingPointTick.MAX_WIDTH * 2;

        if ((minRange, maxRange) != (currentMinRange, currentMaxRange))
        {
            currentMinRange = minRange;
            currentMaxRange = maxRange;

            if (nextMinTick == null || nextMaxTick == null || minRange < nextMinTick || maxRange > nextMaxTick)
                tickCache.Invalidate();
        }

        if (!tickCache.IsValid)
            regenerateTicks();
        else if (editorClock.IsRunning)
            playTickSound();
        else
        {
            lastPlayedTickOffset = editorClock.CurrentTime;
        }
    }

    private void playTickSound()
    {
        if (!ShouldPlayTickSound) return;

        var currentTime = editorClock.CurrentTime;
        if (currentTime < 0)
            return;

        foreach (var tick in Children)
        {
            var tickOffset = tick.Offset;

            if (tickOffset > currentTime) continue;
            if (!(lastPlayedTickOffset < tickOffset) || !(tickOffset < currentTime)) continue;

            lastPlayedTickOffset = tickOffset;

            if (tick.ShouldPlaySound)
                tickSample?.Play();

            return;
        }
    }

    private void regenerateTicks()
    {
        currentTickIndex = 0;

        nextMinTick = null;
        nextMaxTick = null;

        for (var i = 0; i < dataHolder.CurrentMap.Value.TimingPoints.Count; i++)
        {
            generateTickForTimingPoint(i, dataHolder.CurrentMap.Value.TimingPoints[i]);
        }

        var usedTicks = currentTickIndex;

        while (currentTickIndex < Math.Min(usedTicks + 16, Count))
            Children[currentTickIndex++].Alpha = 0;

        while (currentTickIndex < Count)
            Children[currentTickIndex++].Expire();

        tickCache.Validate();
    }

    private void generateTickForTimingPoint(int pointIndex, TimingPointObject currentTimingPoint)
    {
        var timingPoints = dataHolder.CurrentMap.Value.TimingPoints;
        var isLastPoint = pointIndex == timingPoints.Count - 1;
        var nextPoint = isLastPoint ? null : timingPoints[pointIndex + 1];

        var startOffset = currentTimingPoint.Offset;
        var endOffset = nextPoint?.Offset ?? dataHolder.CurrentTrack.Value.Length;

        var startPosition = ScrollContainer.PositionAtTime(startOffset);
        var endPosition = ScrollContainer.PositionAtTime(endOffset);

        if (startPosition > currentMaxRange || endPosition < currentMinRange)
            return;

        var beatLength = 60_000 / currentTimingPoint.NewBpm;

        var subBeatCount = 4; // TODO: type of subdivision (4/4, 3/4, 6/8, etc.)
        var subBeatLength = beatLength / subBeatCount;

        var beatIndex = -1;

        for (var tickOffset = startOffset; tickOffset < endOffset; tickOffset += subBeatLength)
        {
            var isFirstBeat = (beatIndex + 1) % subBeatCount == 0;
            beatIndex++;

            var tickPosition = ScrollContainer.PositionAtTime(tickOffset);

            if (tickPosition < currentMinRange)
            {
                if (nextMinTick == null || tickPosition > nextMinTick)
                    nextMinTick = tickPosition;
                continue;
            }

            if (tickPosition > currentMaxRange)
            {
                if (nextMaxTick == null || tickPosition < nextMaxTick)
                    nextMaxTick = tickPosition;
                continue;
            }

            if (pointIndex == 0 && beatIndex == 0)
            {
                nextMinTick = float.MinValue;
            }

            var tick = getOrCreateTick();
            tick.Offset = tickOffset;
            tick.X = tickPosition;
            tick.Alpha = 1;
            tick.ShouldPlaySound = false;

            if (beatIndex == 0)
            {
                tick.Height = 1;
                tick.Width = TimingPointTick.TIMING_CHANGE_WIDTH;
                tick.Colour = Color4.Red;
                tick.ShouldPlaySound = true;
            }
            else if (isFirstBeat)
            {
                tick.Height = 0.8f;
                tick.Width = TimingPointTick.FIRST_BEAT_POINT_WIDTH;
                tick.Colour = Color4.White;
                tick.ShouldPlaySound = true;
            }
            else // TODO: Color per subdivision type
            {
                tick.Height = 0.5f;
                tick.Width = TimingPointTick.SUB_BEAT_POINT_WIDTH;
                tick.Colour = Color4.Blue;
            }
        }
    }

    private TimingPointTick getOrCreateTick()
    {
        TimingPointTick tick;

        if (currentTickIndex >= Count)
        {
            tick = new TimingPointTick();
            Add(tick);
        }
        else
        {
            tick = Children[currentTickIndex];
        }

        currentTickIndex++;
        return tick;
    }
}
