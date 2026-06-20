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

using System;
using System.Reflection;
using MuseDashEditor.Game.Data.Attribute;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Graphics.Textures;

namespace MuseDashEditor.Game.Utils;

public static class GameObjectUtils
{
    public static GameObjectData? GetGameObjectData(ObjectType objectType)
    {
        return typeof(ObjectType).GetField(objectType.ToString())?.GetCustomAttribute<GameObjectData>();
    }

    public static Texture? GetObjectTexture(this ITextureStore textureStore, ObjectType objectType,
        SceneType currentScene, LaneType laneType)
    {
        var objectData = GetGameObjectData(objectType);
        if (objectData is null) return null;

        var sceneData = SceneUtils.GetSceneData(currentScene);
        if (sceneData is null) return null;

        var ignoreScene = false;
        var ignoreLane = false;

        var specialTextureData = typeof(TextureType).GetField(objectData.TextureType.ToString())?.GetCustomAttribute<SpecialTexture>();
        if (specialTextureData != null)
        {
            ignoreScene = specialTextureData.SceneIndependent;
            ignoreLane = specialTextureData.LaneIndependent;
        }

        string scenePath = ignoreScene ? "Common" : $"scene_{sceneData.ResourcePath}";

        string suffix = ignoreLane ? "" : laneType switch
        {
            LaneType.Air or LaneType.Air2 => "_air",
            LaneType.Ground or LaneType.Ground2 => "_ground",
            LaneType.Special or LaneType.Special2 => "",
            _ => throw new ArgumentOutOfRangeException(nameof(laneType), laneType, null)
        };

        var path = $"Icons/Object/{scenePath}/{objectData.TextureType.ToString().ToLowerInvariant()}{suffix}";
        return textureStore.Get(path);
    }

    public static Texture? GetLaneBackgroundTexture(this ITextureStore textureStore, LaneType laneType)
    {
        const string path_prefix = "Icons/Object/LaneBackground";

        return laneType switch
        {
            LaneType.Air or LaneType.Air2 => textureStore.Get($"{path_prefix}/air"),
            LaneType.Ground or LaneType.Ground2 => textureStore.Get($"{path_prefix}/ground"),
            LaneType.Special or LaneType.Special2 => textureStore.Get($"{path_prefix}/special"),
            _ => throw new ArgumentOutOfRangeException(nameof(laneType), laneType, null)
        };
    }

    public static Texture? GetMovementTypeTexture(this ITextureStore textureStore, MovementType movementType)
    {
        return textureStore.Get($"Icons/Object/MovementType/{movementType.ToString().ToLowerInvariant()}");
    }
}
