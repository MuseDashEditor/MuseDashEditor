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
using System.Text.Json;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK;

namespace MuseDashEditor.Game.Screens.Open.Components;

public partial class ProjectRow(Storage projectStorage) : Container
{
    [Resolved] protected EditorDataHolder DataHolder { get; private set; } = null!;
    [Resolved] protected ScreenStack ScreenStack { get; private set; } = null!;

    private Box hoverBox = null!;
    private bool isLoading;

    [BackgroundDependencyLoader]
    private void load(IRenderer renderer, GameHost gameHost, TextureStore textures)
    {
        RelativeSizeAxes = Axes.X;
        Size = new Vector2(1, 150);

        var textureStore = new LargeTextureStore(renderer,
            gameHost.CreateTextureLoaderStore(new StorageBackedResourceStore(projectStorage)));

        var chartInfoFile = projectStorage.GetStream("info.json");
        var chartInfoRaw = JsonSerializer.Deserialize<ChartInfoRaw>(chartInfoFile) ?? ChartInfoRaw.Empty;

        InternalChildren =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = MdeColors.Background6,
                Depth = 2
            },
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions =
                [
                    new Dimension(GridSizeMode.Absolute, 140f),
                    new Dimension(),
                    new Dimension(GridSizeMode.Relative, 0.3f),
                    new Dimension(GridSizeMode.Absolute, 50f)
                ],
                RowDimensions =
                [
                    new Dimension(GridSizeMode.Relative, 1f)
                ],
                Content = new[]
                {
                    new Drawable[]
                    {
                        new Container
                        {
                            Padding = new MarginPadding(5),
                            RelativeSizeAxes = Axes.Both,
                            Child = new Sprite
                            {
                                RelativeSizeAxes = Axes.Both,
                                Texture = textureStore.Get("cover") ?? textures.Get("UI/default_cover"),
                            }
                        },
                        new FillFlowContainer
                        {
                            Direction = FillDirection.Vertical,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Padding = new MarginPadding(5),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Children =
                            [
                                new SpriteText
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Text = chartInfoRaw.name,
                                    Font = FontUsage.Default.With(size: 40, weight: "600"),
                                },
                                new SpriteText
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Text = chartInfoRaw.author,
                                    Font = FontUsage.Default.With(size: 20),
                                },
                                new SpriteText
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Text = chartInfoRaw.bpm + " BPM",
                                    Font = FontUsage.Default.With(size: 20),
                                }
                            ]
                        },
                        new FillFlowContainer
                        {
                            Direction = FillDirection.Vertical,
                            RelativeSizeAxes = Axes.X,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            AutoSizeAxes = Axes.Y,
                            Padding = new MarginPadding(5),
                            Children = tryBuilDifficultyRows(textures, chartInfoRaw)
                        },
                        new ProjectRowButtons(projectStorage)
                        {
                            Depth = 0
                        }
                    }
                }
            },
            hoverBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Colour = Colour4.White,
                Depth = 1,
                Alpha = 0
            }
        ];
    }

    private static List<FillFlowContainer> tryBuilDifficultyRows(TextureStore textures, ChartInfoRaw chartInfoRaw)
    {
        List<FillFlowContainer> list = [];

        if (chartInfoRaw.difficulty1 != "" && chartInfoRaw.difficulty1 != "0")
        {
            list.Add(buildDifficultyRow(textures, chartInfoRaw.difficulty1, chartInfoRaw.levelDesigner1, "Easy"));
        }

        if (chartInfoRaw.difficulty2 != "" && chartInfoRaw.difficulty2 != "0")
        {
            list.Add(buildDifficultyRow(textures, chartInfoRaw.difficulty2, chartInfoRaw.levelDesigner2, "Hard"));
        }

        if (chartInfoRaw.difficulty3 != "" && chartInfoRaw.difficulty3 != "0")
        {
            list.Add(buildDifficultyRow(textures, chartInfoRaw.difficulty3, chartInfoRaw.levelDesigner3, "Master"));
        }

        if (chartInfoRaw.difficulty4 != "" && chartInfoRaw.difficulty4 != "0")
        {
            list.Add(buildDifficultyRow(textures, chartInfoRaw.difficulty4, chartInfoRaw.levelDesigner4, "Hidden"));
        }

        return list;
    }

    private static FillFlowContainer buildDifficultyRow(TextureStore textures, string difficulty, string levelDesigner,
        string difficultyName)
    {
        return new FillFlowContainer
        {
            Direction = FillDirection.Horizontal,
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            AutoSizeAxes = Axes.X,
            Height = 32,
            Spacing = new Vector2(10f),
            Children =
            [
                new Sprite
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Texture = textures.Get($"Icons/difficulty/{difficultyName.ToLowerInvariant()}"),
                    Size = new Vector2(32, 32),
                },
                new SpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Text = $"{difficultyName} ({difficulty}) by {levelDesigner}"
                }
            ]
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        hoverBox.TransformTo("Alpha", 0.1f, 200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        if (isLoading) return;
        hoverBox.TransformTo("Alpha", 0f, 200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (isLoading)
            return true;

        isLoading = true;
        hoverBox.TransformTo("Colour", (ColourInfo)Colour4.Gray, 200);
        hoverBox.TransformTo("Alpha", 0.2f, 200);

        DataHolder.Initialize(projectStorage)
            .GetAwaiter()
            .OnCompleted(() =>
            {
                ScreenStack.CurrentScreen.Exit();
                ScreenStack.Push(new DifficultySelectorScreen());
            });

        return true;
    }
}
