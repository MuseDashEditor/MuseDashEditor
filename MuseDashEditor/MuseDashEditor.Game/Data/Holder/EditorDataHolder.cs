using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Screens.Editor.SubScreens;
using osu.Framework.Bindables;

namespace MuseDashEditor.Game.Data.Holder;

public class EditorDataHolder
{
    private readonly Bindable<Chart.Chart> currentChart = new();

    public IBindable<Chart.Chart> CurrentChart => currentChart;
    public Bindable<DifficultyType> SelectedDifficulty { get; } = new();
    public Bindable<EditorSubscreenType> SelectedSubscreen { get; } = new();
};
