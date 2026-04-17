using osu.Framework.Bindables;

namespace MuseDashEditor.Game.Data.Holder;

public class EditorDataHolder
{
    private readonly Bindable<Chart.Chart> currentChart = new();

    public IBindable<Chart.Chart> CurrentChart => currentChart;
};
