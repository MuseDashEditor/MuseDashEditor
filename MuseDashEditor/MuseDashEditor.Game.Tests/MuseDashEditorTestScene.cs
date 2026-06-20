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
using System.Threading.Tasks;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Editor.Clock;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Testing;

namespace MuseDashEditor.Game.Tests;

public abstract partial class MuseDashEditorTestScene : TestScene
{
    [Cached] protected readonly EditorDataHolder EditorDataHolder = new();
    [Resolved] protected GameHost Host { get; private set; }
    [Resolved] protected AudioManager AudioManager { get; private set; }

    private readonly EditorClock editorClock = new();
    protected override Container<Drawable> Content => content;
    private readonly Container<Drawable> content = new Container { RelativeSizeAxes = Axes.Both };

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        base.Content.AddRange([
            editorClock,
            content
        ]);

        dependencies.CacheAs(editorClock);

        return dependencies;
    }

    protected async Task InitializeEditorData(Task<Chart> loadChartTask)
    {
        var loadedChart = await loadChartTask;
        EditorDataHolder.CurrentChart.Value = loadedChart;
        EditorDataHolder.CurrentMap.Value = loadedChart.HardMap;

        // Create backing storage
        var storage = new NativeStorage(loadedChart.Directory.FullName, Host);
        var resourcesStore = new StorageBackedResourceStore(storage);

        var trackStore = AudioManager.GetTrackStore(resourcesStore);

        var musicFile = loadedChart.MusicFileBindable.Value;

        if (musicFile == null)
            throw new InvalidOperationException("Test chart does not contain a music file");

        var loadedTrack = await trackStore.GetAsync(musicFile.Name);

        if (loadedTrack is null)
            throw new InvalidOperationException("Test chart music file could not be loaded");

        while (loadedTrack is { IsLoaded: false }) ;

        EditorDataHolder.CurrentTrack.Value = loadedTrack;
        EditorDataHolder.CurrentTrackStreamGetter.Value = () => trackStore.GetStream(musicFile.Name);
    }

    protected override ITestSceneTestRunner CreateRunner()
    {
        return new MuseDashEditorTestSceneTestRunner();
    }

    private partial class MuseDashEditorTestSceneTestRunner : MuseDashEditorGameBase, ITestSceneTestRunner
    {
        private TestSceneTestRunner.TestRunner runner;

        protected override void LoadAsyncComplete()
        {
            base.LoadAsyncComplete();
            Add(runner = new TestSceneTestRunner.TestRunner());
        }

        public void RunTestBlocking(TestScene test)
        {
            runner.RunTestBlocking(test);
        }
    }
}
