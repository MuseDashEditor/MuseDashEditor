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
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Object.MappingObject;
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Bindables;

namespace MuseDashEditor.Game.Data.Chart;

public record Map(
    FileInfo MapFile
)
{
    public readonly MapMetadata Metadata = new();

    public readonly List<GameObject> GameObjects = [];
    public readonly List<TimingPointObject> TimingPoints = [];

    public readonly SortedDictionary<int, Dictionary<(LaneModifierType, LaneType), int[]>> RawMapData = new();
}

public class MapMetadata
{
    public readonly Bindable<LaneSpeed> InitialLaneSpeed = new();
    public readonly Bindable<SceneType> InitialScene = new();
    public readonly Bindable<double> InitialBpm = new();
    public readonly Bindable<double> MusicStartOffset = new();

    public readonly Dictionary<int, double> BpmChangeKeys = new();
}
