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
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace MuseDashEditor.Game.Editor.Clock;

public partial class EditorClock : CompositeComponent, IFrameBasedClock, IAdjustableClock, ISourceChangeableClock
{
    public double CurrentTime => interpolatingClock.CurrentTime;
    public bool IsRunning => interpolatingClock.IsRunning;
    public double ElapsedFrameTime => interpolatingClock.ElapsedFrameTime;
    public double FramesPerSecond => (interpolatingClock as IFrameBasedClock).FramesPerSecond;
    public double TrackLength { get; private set; }

    public Action<double> OnTimeChanged = _ => {};
    public Action OnSeek = () => {};

    private readonly DecouplingFramedClock decouplingClock;
    private readonly InterpolatingFramedClock interpolatingClock;

    public EditorClock()
    {
        decouplingClock = new DecouplingFramedClock();
        interpolatingClock = new InterpolatingFramedClock(decouplingClock)
        {
            AllowableErrorMilliseconds = 1,
            DriftRecoveryHalfLife = 10
        };
    }

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder)
    {
        dataHolder.CurrentTrack.BindValueChanged(track =>
        {
            Stop();
            ChangeSource(track.NewValue);
        }, true);
    }

    public void Start()
    {
        decouplingClock.Start();
        interpolatingClock.ProcessFrame();
    }

    public void Stop()
    {
        decouplingClock.Stop();
        interpolatingClock.ProcessFrame();

        if (CurrentTime > TrackLength)
            Seek(TrackLength);
    }

    public void Reset()
    {
        decouplingClock.Reset();
        interpolatingClock.ProcessFrame();
    }

    public bool Seek(double position)
    {
        if (position < 0)
            return false;
        if (position > TrackLength)
            return false;

        if (IsRunning) Stop(); // Maybe add an option to keep running?

        var result = decouplingClock.Seek(position);
        interpolatingClock.ProcessFrame();

        OnTimeChanged(CurrentTime);
        OnSeek();

        return result;
    }

    public void ResetSpeedAdjustments()
    {
        decouplingClock.ResetSpeedAdjustments();
    }

    public double Rate
    {
        get => decouplingClock.Rate;
        set => decouplingClock.Rate = value;
    }

    double IClock.Rate => decouplingClock.Rate;

    protected override void Update()
    {
        base.Update();

        interpolatingClock.ProcessFrame();

        if (IsRunning)
            OnTimeChanged(CurrentTime);
    }

    public IClock Source => decouplingClock.Source;

    public void ChangeSource(IClock source)
    {
        if (source is Track track)
            TrackLength = track.Length;
        else
            TrackLength = 0;

        decouplingClock.ChangeSource(source);
        interpolatingClock.ProcessFrame();
    }

    public void ProcessFrame()
    {
        // No-op
    }
}
