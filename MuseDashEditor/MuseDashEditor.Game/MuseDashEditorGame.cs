using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace MuseDashEditor.Game;

public partial class MuseDashEditorGame : MuseDashEditorGameBase
{
    // Editor data
    [Cached] protected readonly EditorDataHolder DataHolder = new();

    // UI data
    [Cached] protected readonly ScreenStack ScreenStack = new() { RelativeSizeAxes = Axes.Both };

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = ScreenStack;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        ScreenStack.Push(new MainScreen());
    }
}
