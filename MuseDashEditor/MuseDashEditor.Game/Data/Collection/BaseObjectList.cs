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

using MuseDashEditor.Game.Data.Comparer;
using MuseDashEditor.Game.Data.Object;
using osu.Framework.Lists;

namespace MuseDashEditor.Game.Data.Collection;

/**
 * This custom list is sorted by offset and cannot contain the same object multiple times (based on its guid)
 */
public class BaseObjectList<T>() : SortedList<T>(BaseObjectComparer.INSTANCE) where T : BaseObject
{
    public override int Add(T value)
    {
        var index = BinarySearch(value);
        return index >= 0 ? index : base.Add(value);
    }
}
