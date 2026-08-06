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

using MuseDashEditor.Game.Component.Notification;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Platform;

namespace MuseDashEditor.Game.Screens.Open.Components;

public partial class ProjectRowButtons(Storage projectStorage) : FillFlowContainer<ProjectRowButton>
{
    [Resolved] protected NotificationContainer NotificationContainer { get; private set; } = null!;

    [BackgroundDependencyLoader]
    private void load(GameHost gameHost)
    {
        Direction = FillDirection.Vertical;
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.CentreRight;
        Origin = Anchor.CentreRight;

        Children =
        [
            new ProjectRowButton
            {
                Icon = FontAwesome.Solid.Pen,
                TooltipText = "Edit project details",
                ActionOnClick = () => { }
            },
            new ProjectRowButton
            {
                Icon = FontAwesome.Solid.FolderOpen,
                TooltipText = "Open project folder",
                ActionOnClick = () =>
                {
                    gameHost.OpenFileExternally(projectStorage.GetFullPath("."));
                }
            },
            new ProjectRowButton
            {
                Icon = FontAwesome.Solid.Trash,
                TooltipText = "Delete project",
                IconColour = Colour4.Red,
                ActionOnClick = () =>
                {
                    NotificationContainer.SetNotification(new SimpleConfirmationNotification(
                        "Are you sure you want to delete this project?",
                        "This action cannot be undone.",
                        () =>
                        {
                            NotificationContainer.Hide();
                        },
                        () =>
                        {
                            // TODO: delete project & reload list
                            NotificationContainer.Hide();
                        }
                    ));
                }
            }
        ];
    }

    protected override bool OnHover(HoverEvent e)
    {
        return true;
    }
}
