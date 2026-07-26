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
using MuseDashEditor.Game.Component;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Object.MappingObject;
using MuseDashEditor.Game.Editor.Clock;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics.Colour;
using osuTK.Graphics;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;

public partial class TimingTrackTickDisplay() : AutoRefreshContainer<TimingPointTick>(TimingPointTick.MAX_WIDTH * 5)
{
    [Resolved] private EditorDataHolder dataHolder { get; set; } = null!;
    [Resolved] private EditorClock editorClock { get; set; } = null!;

    public bool ShouldPlayTickSound { get; set; } = true;

    private Sample? tickSample;
    private double lastPlayedTickOffset;

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        tickSample = audio.Samples.Get("tick");
        tickSample.Volume.Value = 2f;

        dataHolder.CurrentMap.ValueChanged += e => onTimingPointsChanged();
        dataHolder.CurrentMap.Value.OnTimingPointsChanged += onTimingPointsChanged;
    }

    private void onTimingPointsChanged()
    {
        ContentCache.Invalidate();
    }

    protected override void Update()
    {
        base.Update();

        if (editorClock.IsRunning)
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

    protected override void RegenerateContent()
    {
        for (var i = 0; i < dataHolder.CurrentMap.Value.TimingPoints.Count; i++)
        {
            generateTickForTimingPoint(i, dataHolder.CurrentMap.Value.TimingPoints[i]);
        }
    }

    private void generateTickForTimingPoint(int pointIndex, TimingPointObject currentTimingPoint)
    {
        var timingPoints = dataHolder.CurrentMap.Value.TimingPoints;
        var isLastPoint = pointIndex == timingPoints.Count - 1;
        var nextPoint = isLastPoint ? null : timingPoints[pointIndex + 1];

        var startOffset = currentTimingPoint.Offset.Value;
        var endOffset = nextPoint?.Offset.Value ?? dataHolder.CurrentTrack.Value.Length;

        var startPosition = ScrollContainer.PositionAtTime(startOffset);
        var endPosition = ScrollContainer.PositionAtTime(endOffset);

        if (startPosition > CurrentMaxRange || endPosition < CurrentMinRange)
            return;

        var beatLength = 60_000 / currentTimingPoint.NewBpm.Value;

        var subBeatCount = ScrollContainer.GetCurrentSubBeatDisplayedCount();
        var subBeatLength = beatLength / subBeatCount;

        var beatIndex = -1;

        for (var tickOffset = startOffset; tickOffset < endOffset; tickOffset += subBeatLength)
        {
            beatIndex++;
            var isFirstBeat = beatIndex % subBeatCount == 0;

            var tickPosition = ScrollContainer.PositionAtTime(tickOffset);

            if (tickPosition < CurrentMinRange)
            {
                if (NextMinTick == null || tickPosition > NextMinTick)
                    NextMinTick = tickPosition;
                continue;
            }

            if (tickPosition > CurrentMaxRange)
            {
                if (NextMaxTick == null || tickPosition < NextMaxTick)
                    NextMaxTick = tickPosition;
                continue;
            }

            if (pointIndex == 0 && beatIndex == 0)
            {
                NextMinTick = float.MinValue;
            }

            var tick = getOrCreateTick();
            tick.Offset = tickOffset;
            tick.X = tickPosition;
            tick.Alpha = 1;
            tick.ShouldPlaySound = false;

            if (beatIndex == 0)
            {
                tick.Width = TimingPointTick.TIMING_CHANGE_WIDTH;
                tick.Colour = Color4.Red;
                tick.ShouldPlaySound = true;
            }
            else if (isFirstBeat)
            {
                tick.Width = TimingPointTick.FIRST_BEAT_POINT_WIDTH;
                tick.Colour = Color4.White;
                tick.ShouldPlaySound = true;
            }
            else
            {
                tick.Width = TimingPointTick.SUB_BEAT_POINT_WIDTH;
                tick.Colour = getBeatColor(beatIndex, subBeatCount);
            }
        }
    }

    private static ColourInfo getBeatColor(int beatIndex, uint subBeatCount)
    {
        var inBeatIndex = beatIndex % subBeatCount;

        if (inBeatIndex == 0) return Color4.White;

        var colors = new Dictionary<int, ColourInfo>
        {
            { 2, Color4.Red },
            { 4, Color4.Blue },
            { 8, Color4.Green },
            { 16, Color4.Yellow },
            { 32, Color4.Purple },
            { 64, Color4.Orange },
            { 128, Color4.Cyan },
            { 256, Color4.Magenta }
        };

        var gcd = TimingTrackTickDisplay.gcd((uint)inBeatIndex, subBeatCount);
        var denominator = (int)(subBeatCount / gcd);

        return colors.TryGetValue(denominator, out var color) ? color : Color4.Blue;
    }

    private static uint gcd(uint a, uint b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }
        return a;
    }

    private TimingPointTick getOrCreateTick()
    {
        TimingPointTick tick;

        if (CurrentTickIndex >= Count)
        {
            tick = new TimingPointTick();
            Add(tick);
        }
        else
        {
            tick = Children[CurrentTickIndex];
        }

        CurrentTickIndex++;
        return tick;
    }
}
