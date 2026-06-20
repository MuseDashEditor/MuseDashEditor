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

namespace MuseDashEditor.Game.Data.Attribute;

public class SpecialTexture(
    bool laneIndependent = false,
    bool sceneIndependent = false
) : System.Attribute
{
    public bool LaneIndependent { get; } = laneIndependent;
    public bool SceneIndependent { get; } = sceneIndependent;
}
