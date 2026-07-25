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

using MuseDashEditor.Game.Project;
using MuseDashEditor.Game.Screens.Open.Components;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;

namespace MuseDashEditor.Game.Screens.Open;

public partial class ProjectListScreen : Screen
{
    [BackgroundDependencyLoader]
    private void load(ProjectManager projectManager)
    {
        var listContainer = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 10),
        };

        foreach (var projectStorage in projectManager.GetAllProjects())
        {
            listContainer.Add(new ProjectRow(projectStorage));
        }

        if (listContainer.Count == 0)
        {
            listContainer.Add(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 50),
                Children =
                [
                    new SpriteText
                    {
                        Text = "No projects found",
                        Font = FontUsage.Default.With(size: 50),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                    new BasicButton
                    {
                        Text = "New chart",
                        Size = new Vector2(200, 50),
                        Colour = Colour4.AliceBlue,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Action = () => {} // TODO
                    }
                ]
            });
        }

        InternalChildren =
        [
            listContainer
        ];
    }
}
