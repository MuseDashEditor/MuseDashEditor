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
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Project;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace MuseDashEditor.Game.Screens.Open;

public partial class FolderSelectorScreen : Screen
{
    [BackgroundDependencyLoader]
    private void load(ScreenStack screenStack, EditorDataHolder dataHolder, ProjectManager projectManager)
    {
        var fileSelector = new BasicFileSelector(null, [".mdm"])
        {
            RelativeSizeAxes = Axes.X,
            Size = new Vector2(1, 1030),
            Origin = Anchor.TopLeft,
            Anchor = Anchor.TopLeft
        };

        InternalChildren =
        [
            fileSelector,
            new BasicButton
            {
                Text = "Open chart",
                Size = new Vector2(200, 50),
                Colour = Colour4.AliceBlue,
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Action = () => Scheduler.Add(async void () =>
                {
                    try
                    {
                        var selectedFile = fileSelector.CurrentFile.Value;
                        if (selectedFile == null)
                        {
                            return;
                        }

                        var pathValue = fileSelector.CurrentFile.Value;
                        if (pathValue == null) return;

                        Logger.Log($"Importing chart from {pathValue.FullName}...");

                        var storage = await projectManager.ImportChart(pathValue.FullName);
                        await dataHolder.Initialize(storage);

                        this.Exit();
                        screenStack.Push(new DifficultySelectorScreen());
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Failed to import chart");
                    }
                })
            }
        ];
    }
}
