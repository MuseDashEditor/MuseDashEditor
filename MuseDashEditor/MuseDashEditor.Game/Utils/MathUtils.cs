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

namespace MuseDashEditor.Game.Utils;

public static class MathUtils
{
    public static uint Gcd(uint a, uint b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }

        return a;
    }

    public static int Gcd(int a, int b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }

        return a;
    }

    public static uint Lcm(uint a, uint b)
    {
        return a / Gcd(a, b) * b;
    }

    public static int Lcm(int a, int b)
    {
        return a / Gcd(a, b) * b;
    }

    public static int Lcm(List<int> list)
    {
        if (list.Count == 0)
            return 0;

        var lcm = list[0];

        for (int i = 1; i < list.Count; i++)
        {
            lcm = Lcm(lcm, list[i]);
        }

        return lcm;
    }
}
