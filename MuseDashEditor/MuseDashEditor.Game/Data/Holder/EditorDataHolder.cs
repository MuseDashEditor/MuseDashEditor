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
using System.IO;
using System.Threading.Tasks;
using MuseDashEditor.Game.Conversion.Parser;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Screens.Editor.SubScreens;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace MuseDashEditor.Game.Data.Holder;

public partial class EditorDataHolder : IDependencyInjectionCandidate
{
    [Resolved] protected AudioManager AudioManager { get; private set; } = null!;

    public readonly Bindable<Chart.Chart> CurrentChart = new();
    public readonly Bindable<Chart.Map> CurrentMap = new();
    public readonly Bindable<Track> CurrentTrack = new();
    public readonly Bindable<Func<Stream>> CurrentTrackStreamGetter = new();
    public readonly Bindable<DifficultyType> SelectedDifficulty = new();
    public readonly Bindable<EditorSubscreenType> SelectedSubscreen = new();
    public readonly Bindable<SceneType> CurrentScene = new();

    public Action OnTimingPointsChanged = () => {};
    public Action OnGameObjectsChanged = () => {};

    public async Task Initialize(Storage? storage)
    {
        if (storage == null)
            throw new InvalidOperationException("Storage is null");

        var storagePath = storage.GetFullPath(".");
        var storageDirectory = new DirectoryInfo(storagePath);

        var chart = await ChartParser.Parse(storageDirectory);
        if (chart == null)
            throw new InvalidOperationException("Chart could not be parsed");

        var resourcesStore = new StorageBackedResourceStore(storage);
        var trackStore = AudioManager.GetTrackStore(resourcesStore);

        var musicFile = chart.MusicFileBindable.Value;
        var demoFile = chart.DemoFileBindable.Value;

        if (musicFile != null)
        {
            var loadedTrack = await trackStore.GetAsync(musicFile.Name);
            if (loadedTrack != null)
                CurrentTrack.Value = loadedTrack;

            CurrentTrackStreamGetter.Value = () => trackStore.GetStream(musicFile.Name);
        }

        CurrentChart.Value = chart;
    }
}
