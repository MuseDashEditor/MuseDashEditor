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
using MuseDashEditor.Game.Component.Common.Menu;
using MuseDashEditor.Game.Component.Common.Menu.Item;
using MuseDashEditor.Game.Component.Toast;
using MuseDashEditor.Game.Component.Toast.Impl;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.History;
using MuseDashEditor.Game.Project;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class EditorMenu() : MdeMenu(Direction.Horizontal, true)
{
    [Resolved]
    private EditorDataHolder editorDataHolder { get; set; } = null!;

    [Resolved]
    private ToastContainer toastContainer { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load(HistoryManager historyManager)
    {
        Items =
        [
            new TopMenuMenuItem("File")
            {
                Items =
                [
                    new SubMenuMenuItem("Open difficulty")
                    {
                        Items =
                        [
                            new ActionMenuItem("Easy", () => { }),
                            new ActionMenuItem("Hard", () => { }),
                            new ActionMenuItem("Master", () => { }),
                            new ActionMenuItem("Hidden", () => { })
                        ]
                    },
                    new SeparatorMenuItem(),
                    new ActionMenuItem("Save", save),
                    new ActionMenuItem("Save copy as...", () => { }),
                    new ActionMenuItem("Reset to last save", () => { }, MenuItemType.Destructive),
                    new SeparatorMenuItem(),
                    new SubMenuMenuItem("Export")
                    {
                        Items =
                        [
                            new ActionMenuItem("Export map", () => { }),
                            new ActionMenuItem("Export map as BMS", () => { }),
                            new ActionMenuItem("Export chart", () => { }),
                            new ActionMenuItem("Export chart as BMS", () => { })
                        ]
                    },
                    new SeparatorMenuItem(),
                    new ActionMenuItem("Close project", () => { }),
                    new ActionMenuItem("Close project without saving", () => { }, MenuItemType.Destructive),
                    new ActionMenuItem("Remove difficulty from project", () => { }, MenuItemType.Destructive),
                    new SeparatorMenuItem(),
                    new ActionMenuItem("Quit", () => { }),
                    new ActionMenuItem("Quit without saving", () => { }, MenuItemType.Destructive)
                ]
            },
            new TopMenuMenuItem("Edit")
            {
                Items =
                [
                    new ActionMenuItem("Undo", historyManager.Undo),
                    new ActionMenuItem("Redo", historyManager.Redo),
                    new SeparatorMenuItem(),
                    new ActionMenuItem("Copy", () => { }),
                    new ActionMenuItem("Cut", () => { }),
                    new ActionMenuItem("Paste", () => { }),
                    new ActionMenuItem("Select all visible", () => { }),
                    new ActionMenuItem("Select all", () => { }),
                    new SeparatorMenuItem(),
                    new ActionMenuItem("Snap selection", () => { }, MenuItemType.Destructive)
                ]
            }
        ];
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
}
