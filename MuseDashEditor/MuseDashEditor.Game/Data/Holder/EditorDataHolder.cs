using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Screens.Editor.SubScreens;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;

namespace MuseDashEditor.Game.Data.Holder;

public class EditorDataHolder
{
    public Bindable<Chart.Chart> CurrentChart { get; } = new();
    public Bindable<Track> CurrentTrack { get; } = new();
    public Bindable<DifficultyType> SelectedDifficulty { get; } = new();
    public Bindable<EditorSubscreenType> SelectedSubscreen { get; } = new();
};
