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

using osu.Framework.Input.Bindings;

namespace MuseDashEditor.Game.Utils;

public static class DefaultKeybinds
{
    public static KeyCombination PreviousBeat1 => new(InputKey.Left);
    public static KeyCombination PreviousBeat2 => new(InputKey.MouseWheelUp);
    public static KeyCombination NextBeat1 => new(InputKey.Right);
    public static KeyCombination NextBeat2 => new(InputKey.MouseWheelDown);
    public static KeyCombination PreviousTimingPoint => new(InputKey.PageUp);
    public static KeyCombination NextTimingPoint => new(InputKey.PageDown);
    public static KeyCombination JumpToStart => new(InputKey.Home);
    public static KeyCombination JumpToEnd => new(InputKey.End);
    public static KeyCombination ZoomIn1 => new(InputKey.Control, InputKey.MouseWheelUp);
    public static KeyCombination ZoomIn2 => new(InputKey.Control, InputKey.Up);
    public static KeyCombination ZoomOut1 => new(InputKey.Control, InputKey.MouseWheelDown);
    public static KeyCombination ZoomOut2 => new(InputKey.Control, InputKey.Down);
}
