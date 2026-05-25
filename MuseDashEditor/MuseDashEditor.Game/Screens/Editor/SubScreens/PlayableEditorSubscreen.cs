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

using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Editor.Clock;
using osu.Framework.Allocation;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens;

public partial class PlayableEditorSubscreen : EditorSubscreen
{
    [Resolved] protected EditorClock EditorClock { get; private set; } = null!;

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder editorDataHolder)
    {
        editorDataHolder.SelectedSubscreen.BindValueChanged(screenChangedEvent =>
        {
            if (
                screenChangedEvent.NewValue != EditorSubscreenType.Compose
                && screenChangedEvent.NewValue != EditorSubscreenType.Design
                && screenChangedEvent.NewValue != EditorSubscreenType.Timing
            )
                EditorClock.Stop();
        });
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat) return false;
        if (EditorClock == null) return false;

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (e.Key)
        {
            case Key.Space:
            {
                if (EditorClock.IsRunning)
                    EditorClock.Stop();
                else
                    EditorClock.Start();

                return true;
            }
            case Key.Left:
                var amountLeft = e.ShiftPressed ? 10 : e.ControlPressed ? 10000 : 1000;
                EditorClock.Seek(EditorClock.CurrentTime - amountLeft);
                return true;
            case Key.Right:
                var amountRight = e.ShiftPressed ? 10 : e.ControlPressed ? 10000 : 1000;
                EditorClock.Seek(EditorClock.CurrentTime + amountRight);
                return true;
            default:
                return base.OnKeyDown(e);
        }
    }
}
