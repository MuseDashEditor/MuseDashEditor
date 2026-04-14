using MuseDashEditor.Game.Screen;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace MuseDashEditor.Game;

public partial class MuseDashEditorGame : MuseDashEditorGameBase
{
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
