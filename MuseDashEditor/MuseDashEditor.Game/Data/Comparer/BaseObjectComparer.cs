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

using System.Collections.Generic;
using MuseDashEditor.Game.Data.Object;

namespace MuseDashEditor.Game.Data.Comparer;

public class BaseObjectComparer : IComparer<BaseObject>
{
    public static readonly BaseObjectComparer INSTANCE = new();

    public int Compare(BaseObject? x, BaseObject? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (y is null) return 1;
        if (x is null) return -1;

        if (x.Id.Equals(y.Id))
            return 0;

        var res = x.Offset.Value.CompareTo(y.Offset.Value);
        if (res == 0)
            res = -1;

        return res;
    }
}
