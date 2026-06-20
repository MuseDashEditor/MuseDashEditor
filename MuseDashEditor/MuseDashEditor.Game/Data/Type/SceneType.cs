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

using MuseDashEditor.Game.Data.Scene;

namespace MuseDashEditor.Game.Data.Type;

public enum SceneType
{
    Unknown = 0,

    [SceneData("01_space_station")] SpaceStation = 1,
    [SceneData("02_retrocity")] Retrocity = 2,
    [SceneData("03_castle")] Castle = 3,
    [SceneData("04_rainy_night")] RainyNight = 4,
    [SceneData("05_candyland")] Candyland = 5,
    [SceneData("06_oriental")] Oriental = 6,
    [SceneData("07_lets_groove")] LetsGroove = 7,
    [SceneData("08_touhou")] Touhou = 8,
    [SceneData("09_djmax")] DjMax = 9,
}
