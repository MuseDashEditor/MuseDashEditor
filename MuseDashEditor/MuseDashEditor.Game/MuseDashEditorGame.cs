using MuseDashEditor.Game.Screen;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace MuseDashEditor.Game;

public partial class MuseDashEditorGame : MuseDashEditorGameBase
{
    private ScreenStack screenStack;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        screenStack.Push(new MainScreen());
    }
}
