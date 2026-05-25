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

using System.Collections.Generic;
using System.IO;
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Bindables;

namespace MuseDashEditor.Game.Data.Chart;

public class Chart(
    DirectoryInfo directory,
    FileInfo musicFile,
    FileInfo demoFile,
    ChartInfo info,
    Dictionary<DifficultyType, Map> maps
)
{
    public DirectoryInfo Directory => directory;
    public ChartInfo ChartInfo => info;

    public Dictionary<DifficultyType, Map> Maps => maps;
    public Map EasyMap => maps.GetValueOrDefault(DifficultyType.Easy);
    public Map HardMap => maps.GetValueOrDefault(DifficultyType.Hard);
    public Map MasterMap => maps.GetValueOrDefault(DifficultyType.Master);
    public Map HiddenMap => maps.GetValueOrDefault(DifficultyType.Hidden);

    private readonly Bindable<FileInfo> musicFileBindableInternal = new(musicFile);
    public IBindable<FileInfo> MusicFileBindable => musicFileBindableInternal;

    private readonly Bindable<FileInfo> demoFileBindableInternal = new(demoFile);
    public IBindable<FileInfo> DemoFileBindable => demoFileBindableInternal;
}
