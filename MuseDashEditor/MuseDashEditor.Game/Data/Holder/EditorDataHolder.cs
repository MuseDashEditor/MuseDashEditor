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
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Screens.Editor.SubScreens;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;

namespace MuseDashEditor.Game.Data.Holder;

public class EditorDataHolder
{
    public readonly Bindable<Chart.Chart> CurrentChart = new();
    public readonly Bindable<Chart.Map> CurrentMap = new();
    public readonly Bindable<Track> CurrentTrack = new();
    public readonly Bindable<Func<Stream>> CurrentTrackStreamGetter = new();
    public readonly Bindable<DifficultyType> SelectedDifficulty = new();
    public readonly Bindable<EditorSubscreenType> SelectedSubscreen = new();
    public readonly Bindable<SceneType> CurrentScene = new();
}
