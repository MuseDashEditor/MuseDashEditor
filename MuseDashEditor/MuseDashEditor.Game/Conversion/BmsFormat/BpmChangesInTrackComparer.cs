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

namespace MuseDashEditor.Game.Conversion.BmsFormat;

public class BpmChangesInTrackComparer : IComparer<(int, int, int)>
{
    public static BpmChangesInTrackComparer Instance = new();

    public int Compare((int, int, int) x, (int, int, int) y)
    {
        var item1Comparison = Comparer<int>.Default.Compare(x.Item1, y.Item1);

        if (item1Comparison != 0)
            return item1Comparison;

        return Comparer<double>.Default.Compare(x.Item2 / (double)x.Item3, y.Item2 / (double)y.Item3);
    }
}
