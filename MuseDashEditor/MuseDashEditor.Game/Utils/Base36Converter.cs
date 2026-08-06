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

namespace MuseDashEditor.Game.Utils;

public static class Base36Converter
{
    private static readonly List<char> base36_chars =
    [
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L',
        'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
    ];

    public static int FromBase36(ReadOnlySpan<char> span)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(span.Length, 2);

        if (span.Length < 2)
            span = $"0{span[0]}";

        var char1 = span[0];
        var char2 = span[1];

        return base36_chars.IndexOf(char1) * 36 + base36_chars.IndexOf(char2);
    }

    public static string ToBase36(int input)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(input, 36 * 36);

        var char1 = base36_chars[input / 36];
        var char2 = base36_chars[input % 36];

        return $"{char1}{char2}";
    }
}
