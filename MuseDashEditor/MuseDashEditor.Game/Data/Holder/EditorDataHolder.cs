using System.IO;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Screens.Editor.SubScreens;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;

namespace MuseDashEditor.Game.Data.Holder;

public class EditorDataHolder
{
    public readonly Bindable<Chart.Chart> CurrentChart = new();
    public readonly Bindable<Chart.Map> CurrentMap = new();
    public readonly Bindable<Track> CurrentTrack = new();
    public readonly Bindable<Stream> CurrentTrackStream = new();
    public readonly Bindable<DifficultyType> SelectedDifficulty = new();
    public readonly Bindable<EditorSubscreenType> SelectedSubscreen = new();
    public readonly Bindable<SceneType> CurrentScene = new();
}
