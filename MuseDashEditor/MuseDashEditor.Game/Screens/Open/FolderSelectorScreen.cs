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
using MuseDashEditor.Game.Data.Parser;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK;

namespace MuseDashEditor.Game.Screens.Open;

public partial class FolderSelectorScreen : Screen
{
    [BackgroundDependencyLoader]
    private void load(GameHost host, ScreenStack screenStack, AudioManager audioManager, EditorDataHolder dataHolder)
    {
        var fileSelector = new BasicFileSelector(null, [".bms", ".json", ".ogg", ".mp3"])
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
                        var pathValue = fileSelector.CurrentPath.Value;
                        Logger.Log($"Opening chart from {pathValue.FullName}...");

                        var chart = await ChartParser.Parse(pathValue);
                        if (chart == null) return;

                        // Create backing storage
                        var storage = new NativeStorage(pathValue.FullName, host);
                        var resourcesStore = new StorageBackedResourceStore(storage);

                        var trackStore = audioManager.GetTrackStore(resourcesStore);

                        var musicFile = chart.MusicFileBindable.Value;
                        var demoFile = chart.DemoFileBindable.Value;

                        // Load audio files
                        if (musicFile != null)
                        {
                            var loadedTrack = await trackStore.GetAsync(musicFile.Name);
                            if (loadedTrack != null)
                                dataHolder.CurrentTrack.Value = loadedTrack;

                            var loadedTrackStream = trackStore.GetStream(musicFile.Name);
                            if (loadedTrackStream != null)
                                dataHolder.CurrentTrackStream.Value = loadedTrackStream;
                        }

                        dataHolder.CurrentChart.Value = chart;

                        this.Exit();
                        screenStack.Push(new DifficultySelectorScreen());
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Failed to open chart");
                    }
                })
            }
        ];
    }
}
