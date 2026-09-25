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
using System.Threading;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Logging;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class EditorBackground : Sprite
{
    [Resolved]
    protected LargeTextureStore Textures { get; private set; } = null!;

    private CancellationTokenSource currentSceneChangeTokenSource = new();

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder)
    {
        RelativeSizeAxes = Axes.Both;
        Size = new Vector2(1, 1);
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        Alpha = 0.2f;

        dataHolder.CurrentScene.BindValueChanged(OnSceneChange, true);
    }

    private void OnSceneChange(ValueChangedEvent<SceneType> valueChangedEvent)
    {
        CancellationTokenSource oldSource;

        lock (currentSceneChangeTokenSource)
        {
            currentSceneChangeTokenSource.Cancel();
            oldSource = currentSceneChangeTokenSource;
            currentSceneChangeTokenSource = new CancellationTokenSource();

            var newScene = valueChangedEvent.NewValue;
            var sceneData = SceneUtils.GetSceneData(newScene);

            var cancellationToken = currentSceneChangeTokenSource.Token;

            if (sceneData is null)
            {
                Schedule(async void () =>
                {
                    try
                    {
                        var texture = await Textures.GetAsync("default_background", cancellationToken);
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        Texture = texture;
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Failed to load default scene background");
                    }
                });
            }
            else
            {
                Schedule(async void () =>
                {
                    try
                    {
                        var textureName = $"Scenes/scene_{sceneData.ResourcePath}/background";
                        var texture = await Textures.GetAsync(textureName, cancellationToken);
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        Texture = texture;
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Failed to load scene background");
                    }
                });
            }
        }

        oldSource.Dispose();
    }
}
