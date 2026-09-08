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
using osu.Framework.Input.Bindings;

namespace MuseDashEditor.Game.Input;

public partial class MdeKeyBindingContainer : KeyBindingContainer<InputAction>
{
    public override IEnumerable<IKeyBinding> DefaultKeyBindings =>
    [
        new KeyBinding(new[] { InputKey.Escape }, InputAction.Escape),
        new KeyBinding(new[] { InputKey.Space }, InputAction.PlaybackPlayPause),
        new KeyBinding(new[] { InputKey.Control, InputKey.Space }, InputAction.PlaybackPauseNoBack),
        new KeyBinding(new[] { InputKey.Home }, InputAction.PlaybackGoToStart),
        new KeyBinding(new[] { InputKey.End }, InputAction.PlaybackGoToEnd),
        new KeyBinding(new[] { InputKey.Right }, InputAction.NextBeat),
        new KeyBinding(new[] { InputKey.MouseWheelDown }, InputAction.NextBeat2),
        new KeyBinding(new[] { InputKey.Control, InputKey.Right }, InputAction.NextFirstBeat),
        new KeyBinding(new[] { InputKey.Left }, InputAction.PreviousBeat),
        new KeyBinding(new[] { InputKey.MouseWheelUp }, InputAction.PreviousBeat2),
        new KeyBinding(new[] { InputKey.Control, InputKey.Left }, InputAction.PreviousFirstBeat),
        new KeyBinding(new[] { InputKey.PageDown }, InputAction.NextTimingPoint),
        new KeyBinding(new[] { InputKey.PageUp }, InputAction.PreviousTimingPoint),
        new KeyBinding(new[] { InputKey.Control, InputKey.Shift, InputKey.A }, InputAction.SelectAll),
        new KeyBinding(new[] { InputKey.Control, InputKey.A }, InputAction.SelectAllVisible),
        new KeyBinding(new[] { InputKey.Control, InputKey.C }, InputAction.Copy),
        new KeyBinding(new[] { InputKey.Control, InputKey.X }, InputAction.Cut),
        new KeyBinding(new[] { InputKey.Control, InputKey.V }, InputAction.Paste),
        new KeyBinding(new[] { InputKey.Delete }, InputAction.Delete),
        new KeyBinding(new[] { InputKey.F }, InputAction.Flip),
        new KeyBinding(new[] { InputKey.Control, InputKey.MouseWheelUp }, InputAction.ZoomIn),
        new KeyBinding(new[] { InputKey.Control, InputKey.MouseWheelDown }, InputAction.ZoomOut),
        new KeyBinding(new[] { InputKey.Alt, InputKey.MouseWheelUp }, InputAction.VolumeUp),
        new KeyBinding(new[] { InputKey.Alt, InputKey.MouseWheelDown }, InputAction.VolumeDown)
    ];
}
