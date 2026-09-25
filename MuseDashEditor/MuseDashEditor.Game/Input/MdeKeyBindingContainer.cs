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
        //@formatter:off
        c(InputAction.Escape,                InputKey.Escape),
        c(InputAction.PlaybackPauseRollback, InputKey.Control, InputKey.Space),
        c(InputAction.PlaybackPause,         InputKey.Space),
        c(InputAction.PlaybackGoToStart,     InputKey.Home),
        c(InputAction.PlaybackGoToEnd,       InputKey.End),
        c(InputAction.NextBeat,              InputKey.Right),
        c(InputAction.NextBeat2,             InputKey.MouseWheelDown),
        c(InputAction.NextFirstBeat,         InputKey.Control, InputKey.Right),
        c(InputAction.PreviousBeat,          InputKey.Left),
        c(InputAction.PreviousBeat2,         InputKey.MouseWheelUp),
        c(InputAction.PreviousFirstBeat,     InputKey.Control, InputKey.Left),
        c(InputAction.NextTimingPoint,       InputKey.PageDown),
        c(InputAction.PreviousTimingPoint,   InputKey.PageUp),
        c(InputAction.SelectAll,             InputKey.Control, InputKey.Shift, InputKey.A),
        c(InputAction.SelectAllVisible,      InputKey.Control, InputKey.A),
        c(InputAction.Copy,                  InputKey.Control, InputKey.C),
        c(InputAction.Cut,                   InputKey.Control, InputKey.X),
        c(InputAction.Paste,                 InputKey.Control, InputKey.V),
        c(InputAction.Delete,                InputKey.Delete),
        c(InputAction.Flip,                  InputKey.F),
        c(InputAction.ZoomIn,                InputKey.Control, InputKey.MouseWheelUp),
        c(InputAction.ZoomOut,               InputKey.Control, InputKey.MouseWheelDown),
        c(InputAction.VolumeUp,              InputKey.Alt, InputKey.MouseWheelUp),
        c(InputAction.VolumeDown,            InputKey.Alt, InputKey.MouseWheelDown),
        c(InputAction.Save,                  InputKey.Control, InputKey.S),
        c(InputAction.Close,                 InputKey.Control, InputKey.W),
        c(InputAction.Quit,                  InputKey.Control, InputKey.Q),
        c(InputAction.Undo,                  InputKey.Control, InputKey.Z),
        c(InputAction.Redo,                  InputKey.Control, InputKey.Y),
        c(InputAction.Select1,               InputKey.Number1),
        c(InputAction.Select2,               InputKey.Number2),
        c(InputAction.Select3,               InputKey.Number3),
        c(InputAction.Select4,               InputKey.Number4),
        c(InputAction.Select5,               InputKey.Number5),
        c(InputAction.Select6,               InputKey.Number6),
        c(InputAction.Select7,               InputKey.Number7),
        c(InputAction.Select8,               InputKey.Number8),
        c(InputAction.Select9,               InputKey.Number9),
        c(InputAction.Select10,              InputKey.Number0),
        c(InputAction.Cancel,                InputKey.MouseRight),
        //@formatter:on
    ];

    private static KeyBinding c(InputAction action, params InputKey[] inputKeys) => new KeyBinding(inputKeys, action);
}
