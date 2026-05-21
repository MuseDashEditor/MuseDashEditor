using System;
using MuseDashEditor.Game.Data.Type;

namespace MuseDashEditor.Game.Data.Scene;

[AttributeUsage(AttributeTargets.Field)]
public class SceneData(
    string resourcePath
) : Attribute
{
    public string ResourcePath => resourcePath;
}
