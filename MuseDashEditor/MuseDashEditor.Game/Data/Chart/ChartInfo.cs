using System.Collections.Generic;
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Bindables;

namespace MuseDashEditor.Game.Data.Chart;

public class ChartInfo(
    string name,
    string nameRomanized,
    string author,
    string bpm,
    SceneType defaultScene,
    Dictionary<int, string> levelDesigner,
    Dictionary<int, string> levelDifficulty,
    string hideBmsMode,
    string hideBmsDifficulty,
    string hideBmsMessage,
    List<string> searchTags
)
{
    private readonly Bindable<string> nameBindableInternal = new(name);
    public IBindable<string> NameBindable => nameBindableInternal;

    private readonly Bindable<string> nameRomanizeBindableInternal = new(nameRomanized);
    public IBindable<string> NameRomanizedBindable => nameRomanizeBindableInternal;

    private readonly Bindable<string> authorBindableInternal = new(author);
    public IBindable<string> AuthorBindable => authorBindableInternal;

    private readonly Bindable<string> bpmBindableInternal = new(bpm);
    public IBindable<string> BpmBindable => bpmBindableInternal;

    private readonly Bindable<SceneType> defaultSceneBindableInternal = new(defaultScene);
    public IBindable<SceneType> DefaultSceneBindable => defaultSceneBindableInternal;

    // TODO
    public Dictionary<int, string> LevelDesigner { get; set; } = levelDesigner;
    public Dictionary<int, string> LevelDifficulty { get; set; } = levelDifficulty;
    public string HideBmsMode { get; set; } = hideBmsMode; // TODO: enum
    public string HideBmsDifficulty { get; set; } = hideBmsDifficulty;
    public string HideBmsMessage { get; set; } = hideBmsMessage;
    public List<string> SearchTags { get; set; } = searchTags;
}
