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

using System.IO;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Object.MappingObject;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Screens.Editor;
using MuseDashEditor.Game.Screens.MainSubscreen.Open.Components;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;

namespace MuseDashEditor.Game.Screens.MainSubscreen.Open;

public partial class DifficultySelectorScreen : Screen
{
    [Resolved]
    protected ScreenStack MainScreenStack { get; private set; } = null!;

    [Resolved]
    protected EditorDataHolder DataHolder { get; private set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        if (DataHolder.CurrentChart.Value == null)
            return;

        InternalChildren =
        [
            new FillFlowContainer
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,

                Children =
                [
                    new SpriteText
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Text = "Difficulty Selector",
                        Font = FontUsage.Default.With(size: 50)
                    },
                    new SpriteText
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Text = DataHolder.CurrentChart.Value.ChartInfo.NameBindable.Value,
                        Font = FontUsage.Default.With(size: 50)
                    },
                    new FillFlowContainer<DifficultyDisplay>
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,

                        Children =
                        [
                            new DifficultyDisplay
                            {
                                DifficultyName = "Easy",
                                DifficultyType = DifficultyType.Easy,
                                OnClickAction = () => OnDifficultySelected(DifficultyType.Easy)
                            },
                            new DifficultyDisplay
                            {
                                DifficultyName = "Hard",
                                DifficultyType = DifficultyType.Hard,
                                OnClickAction = () => OnDifficultySelected(DifficultyType.Hard)
                            },
                            new DifficultyDisplay
                            {
                                DifficultyName = "Master",
                                DifficultyType = DifficultyType.Master,
                                OnClickAction = () => OnDifficultySelected(DifficultyType.Master)
                            },
                            new DifficultyDisplay
                            {
                                DifficultyName = "Hidden",
                                DifficultyType = DifficultyType.Hidden,
                                OnClickAction = () => OnDifficultySelected(DifficultyType.Hidden)
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private void OnDifficultySelected(DifficultyType difficulty)
    {
        DataHolder.SelectedDifficulty.Value = difficulty;
        var currentChart = DataHolder.CurrentChart.Value;
        currentChart.ChartInfo.LoadDataFromMap((int)difficulty);

        var isNewMap = !currentChart.Maps.TryGetValue(difficulty, out var map);

        if (isNewMap || map is null)
        {
            map = new Map(
                new FileInfo(Path.Combine(currentChart.Directory.FullName, $"map{(int)difficulty}.mdem")),
                MapType.Mdem
            );
            map.Metadata.InitialBpm.Value = 120;
            map.Metadata.InitialLaneSpeed.Value = LaneSpeed.Medium;
            map.Metadata.InitialScene.Value = SceneType.SpaceStation;
            map.TimingPoints.Add(new TimingPointObject(0, 120));
        }

        DataHolder.CurrentMap.Value = map;
        MapUtils.PreProcessMap(DataHolder.CurrentMap.Value);

        this.Exit();
        MainScreenStack.Push(new EditorScreen());
    }
}
