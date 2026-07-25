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
using System.Collections.Generic;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Object.MappingObject;
using osu.Framework.Logging;

namespace MuseDashEditor.Game.Utils;

public static class ExtensionMethods
{
    public static TValue ComputeIfAbsent<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
        Func<TKey, TValue> valueFactory)
    {
        var value = dictionary.TryGetValue(key, out var existingValue) ? existingValue : valueFactory(key);
        dictionary[key] = value;
        return value;
    }

    public static TimingPointObject? GetTimingPointAtTime(this EditorDataHolder dataHolder, double time,
        bool ignoreExact = false)
    {
        var currentMap = dataHolder.CurrentMap.Value;
        if (currentMap == null) return null;

        var timingPoints = currentMap.TimingPoints;
        if (timingPoints.Count == 0) return null;

        TimingPointObject? nearestTimingPoint = null;

        foreach (var timingPointObject in timingPoints)
        {
            if (timingPointObject.Offset.Value > time) continue;
            if (ignoreExact && Math.Abs(timingPointObject.Offset.Value - time) < 1) continue;
            if (nearestTimingPoint != null && timingPointObject.Offset.Value <= nearestTimingPoint.Offset.Value) continue;

            nearestTimingPoint = timingPointObject;
        }

        return nearestTimingPoint;
    }

    public static TimingPointObject? GetNextTimingPointAtTime(this EditorDataHolder dataHolder, double time)
    {
        var currentTimingPointAtTime = dataHolder.GetTimingPointAtTime(time);
        if (currentTimingPointAtTime == null) return null;

        var timingPoints = dataHolder.CurrentMap.Value.TimingPoints;

        TimingPointObject? nextTimingPoint = null;

        foreach (var timingPointObject in timingPoints)
        {
            if (timingPointObject.Offset.Value <= time + 0.5) continue;
            if (nextTimingPoint != null && timingPointObject.Offset.Value >= nextTimingPoint.Offset.Value) continue;

            nextTimingPoint = timingPointObject;
        }

        return nextTimingPoint;
    }
}
