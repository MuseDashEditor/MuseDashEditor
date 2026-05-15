using MuseDashEditor.Game.Data.Scene;

namespace MuseDashEditor.Game.Data.Type;

public enum SceneType
{
    Unknown = 0,

    [SceneData()]
    SpaceStation = 1,

    [SceneData]
    Retrocity = 2,

    [SceneData]
    Castle = 3,

    [SceneData]
    Candyland = 5,
    // TODO: add all scenes
}
