using System.Collections.Generic;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Utils;
using osu.Framework.Bindables;

namespace MuseDashEditor.Game.Data.Chart;

// ReSharper disable InconsistentNaming
public record ChartInfoRaw(
    string name,
    string name_romanized,
    string author,
    string bpm,
    string scene,
    string levelDesigner,
    string levelDesigner1,
    string levelDesigner2,
    string levelDesigner3,
    string levelDesigner4,
    string difficulty1,
    string difficulty2,
    string difficulty3,
    string difficulty4,
    string hideBmsMode,
    string hideBmsDifficulty,
    string hideBmsMessage,
    List<string> searchTags
);
// ReSharper restore InconsistentNaming

public class ChartInfo(ChartInfoRaw raw)
{
    // Global chart info
    public Bindable<string> NameBindable { get; } = new(raw.name);
    public Bindable<string> NameRomanizedBindable { get; } = new(raw.name_romanized);
    public Bindable<string> AuthorBindable { get; } = new(raw.author);
    public Bindable<string> BpmBindable { get; } = new(raw.bpm);

    // Current difficulty info
    public Bindable<string> LevelDesignerBindable { get; } = new(raw.levelDesigner);
    public Bindable<string> LevelDifficultyBindable { get; } = new();
    public Bindable<SceneType> DefaultSceneBindable { get; } = new(SceneUtils.GetSceneType(raw.scene));

    // TODO
    // public string HideBmsMode { get; set; } = hideBmsMode; // TODO: enum
    // public string HideBmsDifficulty { get; set; } = hideBmsDifficulty;
    // public string HideBmsMessage { get; set; } = hideBmsMessage;
    // public List<string> SearchTags { get; set; } = searchTags;

    public void LoadDataFromMap(int mapId)
    {
        LevelDesignerBindable.Value = mapId switch
        {
            1 => raw.levelDesigner1,
            2 => raw.levelDesigner2,
            3 => raw.levelDesigner3,
            4 => raw.levelDesigner4,
            _ => LevelDesignerBindable.Value
        };

        LevelDifficultyBindable.Value = mapId switch
        {
            1 => raw.difficulty1,
            2 => raw.difficulty2,
            3 => raw.difficulty3,
            4 => raw.difficulty4,
            _ => LevelDifficultyBindable.Value
        };
    }
}
