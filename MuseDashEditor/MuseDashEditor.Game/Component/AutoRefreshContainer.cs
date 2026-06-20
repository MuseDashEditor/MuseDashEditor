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
using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Screens.Editor.Components;
using osu.Framework.Allocation;
using osu.Framework.Caching;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace MuseDashEditor.Game.Component;

public abstract partial class AutoRefreshContainer<T>(float offset) : Container<T> where T : Drawable
{
    public required ZoomableScrollContainer ScrollContainer { get; init; }

    protected readonly Cached ContentCache = new();

    protected float CurrentMinRange = float.MinValue;
    protected float CurrentMaxRange = float.MaxValue;
    protected float? NextMinTick;
    protected float? NextMaxTick;
    protected int CurrentTickIndex;

    [BackgroundDependencyLoader]
    private void load(EditorClock editorClock)
    {
        editorClock.OnSeek += () =>
        {
            ContentCache.Invalidate();
        };
        ScrollContainer.OnDrawWidthChanged += () =>
        {
            ContentCache.Invalidate();
        };
    }

    protected override void Update()
    {
        base.Update();

        if (DrawWidth <= 0) return;

        var screenSpacePosTopLeft = ScrollContainer.ScreenSpaceDrawQuad.TopLeft;
        var screenSpacePosTopRight = ScrollContainer.ScreenSpaceDrawQuad.TopRight;

        var localSpacePosTopLeft = ToLocalSpace(screenSpacePosTopLeft);
        var localSpacePosTopRight = ToLocalSpace(screenSpacePosTopRight);

        var minRange = localSpacePosTopLeft.X - offset;
        var maxRange = localSpacePosTopRight.X + offset;

        if ((minRange, maxRange) != (CurrentMinRange, CurrentMaxRange))
        {
            CurrentMinRange = minRange;
            CurrentMaxRange = maxRange;

            if (NextMinTick == null || NextMaxTick == null || minRange < NextMinTick || maxRange > NextMaxTick)
                ContentCache.Invalidate();
        }

        if (ContentCache.IsValid) return;

        CurrentTickIndex = 0;

        NextMinTick = null;
        NextMaxTick = null;

        RegenerateContent();

        var usedTicks = CurrentTickIndex;

        while (CurrentTickIndex < Math.Min(usedTicks + 16, Count))
            Children[CurrentTickIndex++].Alpha = 0;

        while (CurrentTickIndex < Count)
            Children[CurrentTickIndex++].Expire();

        ContentCache.Validate();
    }

    protected abstract void RegenerateContent();
}
