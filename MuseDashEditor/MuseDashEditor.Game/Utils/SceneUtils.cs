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
        const string prefix = "scene_";
        if (!sceneName.StartsWith(prefix) || !int.TryParse(sceneName[prefix.Length..], out var sceneId))
            return SceneType.Unknown;

        return (SceneType)sceneId;
    }
}
