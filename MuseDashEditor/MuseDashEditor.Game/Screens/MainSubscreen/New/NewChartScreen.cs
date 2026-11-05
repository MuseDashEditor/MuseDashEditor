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
using MuseDashEditor.Game.Component;
using MuseDashEditor.Game.Component.Common;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Project;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata.Components;
using MuseDashEditor.Game.Screens.MainSubscreen.Open;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace MuseDashEditor.Game.Screens.MainSubscreen.New;

public partial class NewChartScreen : MainSubscreen
{
    public static NewChartScreen CreateInstance()
    {
        return new NewChartScreen
        {
            Icon = FontAwesome.Solid.Pen,
            Text = "New project"
        };
    }

    [Resolved]
    private EditorDataHolder editorDataHolder { get; set; } = null!;

    [Resolved]
    private ScreenStack screenStack { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load(ProjectManager projectManager)
    {
        Chart chart = new Chart(null!, null!, null, new ChartInfo(ChartInfoRaw.Empty), new Dictionary<DifficultyType, Map>());
        editorDataHolder.CurrentChart.Value = chart;

        FileSelectorField fileSelectorField;

        Content.Add(new FillFlowContainer
        {
            Direction = FillDirection.Vertical,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Size = new Vector2(300, 400),
            Spacing = new Vector2(0, 30),
            Children =
            [
                fileSelectorField = new FileSelectorField
                {
                    Label = "Select music file"
                },
                new MetadataFieldsContainers()
                {
                    AutoSizeAxes = Axes.Both,
                },
                new RoundedButton()
                {
                    Text = "Create",
                    FontUsage = MDEFonts.ArialUnicodeMs.With(size: 30),
                    RelativeSizeAxes = Axes.X,
                    Size = new Vector2(1, 60),
                    BackgroundColour = MdeColors.Background3,
                    Action = () =>
                    {
                        if (fileSelectorField.selectedFile is null)
                            return;

                        // disable button

                        Schedule(action);
                        return;

                        async void action()
                        {
                            try
                            {
                                var storage = await projectManager.CreateNewProject(fileSelectorField.selectedFile);

                                this.Exit();

                                // Re-load new chart
                                await editorDataHolder.Initialize(storage);

                                screenStack.Push(new DifficultySelectorScreen());
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e, "Failed to create chart");
                            }
                        }
                    }
                }
            ]
        });
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        editorDataHolder.CurrentChart.Value = null;
        return false;
    }
}
