using MuseDashEditor.Game.Data.Scene;

namespace MuseDashEditor.Game.Data.Type;

public enum SceneType
{
    Unknown = 0,

    [SceneData(resourcePath: "01_space_station")]
    SpaceStation = 1,

    [SceneData(resourcePath: "02_retrocity")]
    Retrocity = 2,

    [SceneData(resourcePath: "03_castle")]
    Castle = 3,

    [SceneData(resourcePath: "05_candyland")]
    Candyland = 5,
    // TODO: add all scenes
}
