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

using System.Reflection;
using JetBrains.Annotations;
using MuseDashEditor.Game.Data.Scene;
using MuseDashEditor.Game.Data.Type;

namespace MuseDashEditor.Game.Utils;

public static class SceneUtils
{
    [CanBeNull]
    public static SceneData GetSceneData(SceneType sceneType)
    {
        return typeof(SceneType).GetField(sceneType.ToString())?.GetCustomAttribute<SceneData>();
    }

    public static SceneType GetSceneType(string sceneName)
    {
        sceneName = sceneName.ToLowerInvariant();

        const string prefix = "scene_";
        if (!sceneName.StartsWith(prefix) || !int.TryParse(sceneName[prefix.Length..], out var sceneId))
            return SceneType.Unknown;

        return (SceneType)sceneId;
    }
}
