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

using System;
using MuseDashEditor.Game.Component.Notification;
using MuseDashEditor.Game.Component.Toast;
using MuseDashEditor.Game.Component.Toast.Impl;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Input;
using MuseDashEditor.Game.Project;
using MuseDashEditor.Game.Screens.Editor.Components;
using MuseDashEditor.Game.Screens.Editor.SubScreens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;

namespace MuseDashEditor.Game.Screens.Editor;

public partial class EditorScreen : Screen, IKeyBindingHandler<InputAction>
{
    [Cached]
    protected readonly EditorClock EditorClock = new();

    [Cached]
    protected readonly NotificationContainer NotificationContainer = new();

    [Resolved]
    private EditorDataHolder editorDataHolder { get; set; } = null!;

    [Resolved]
    private ToastContainer toastContainer { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Children =
            [
                // UI
                new EditorBackground(),
                new EditorSubscreenContainer(),
                new Toolbar(),
                new PlayBar(),
                NotificationContainer,

                // Internal
                EditorClock
            ]
        };
    }

    public bool OnPressed(KeyBindingPressEvent<InputAction> e)
    {
        switch (e.Action)
        {
            case InputAction.Save:
                save();
                return true;

            case InputAction.Close:
            case InputAction.Quit:
                return true;
        }

        return false;
    }

    private void save()
    {
        Schedule(async void () =>
        {
            try
            {
                await ProjectManager.Save(editorDataHolder);
                toastContainer.Show(new SavedToast());
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to save map!");
            }
        });
    }

    public void OnReleased(KeyBindingReleaseEvent<InputAction> e)
    {
    }
}
