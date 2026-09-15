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
        new KeyBinding(new[] { InputKey.Control, InputKey.Space }, InputAction.PlaybackPauseRollback),
        new KeyBinding(new[] { InputKey.Space }, InputAction.PlaybackPause),
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
        new KeyBinding(new[] { InputKey.Alt, InputKey.MouseWheelDown }, InputAction.VolumeDown),
        new KeyBinding(new[] { InputKey.Control, InputKey.S }, InputAction.Save),
        new KeyBinding(new[] { InputKey.Control, InputKey.W }, InputAction.Close),
        new KeyBinding(new[] { InputKey.Control, InputKey.Q }, InputAction.Quit),
        new KeyBinding(new[] { InputKey.Control, InputKey.Z }, InputAction.Undo),
        new KeyBinding(new[] { InputKey.Control, InputKey.Y }, InputAction.Redo),
        new KeyBinding(new[] { InputKey.Number1 }, InputAction.Select1),
        new KeyBinding(new[] { InputKey.Number2 }, InputAction.Select2),
        new KeyBinding(new[] { InputKey.Number3 }, InputAction.Select3),
        new KeyBinding(new[] { InputKey.Number4 }, InputAction.Select4),
        new KeyBinding(new[] { InputKey.Number5 }, InputAction.Select5),
        new KeyBinding(new[] { InputKey.Number6 }, InputAction.Select6),
        new KeyBinding(new[] { InputKey.Number7 }, InputAction.Select7),
        new KeyBinding(new[] { InputKey.Number8 }, InputAction.Select8),
        new KeyBinding(new[] { InputKey.Number9 }, InputAction.Select9),
        new KeyBinding(new[] { InputKey.Number0 }, InputAction.Select10),
    ];
}
