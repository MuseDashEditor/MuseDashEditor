using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Screens.Editor.Components;
using MuseDashEditor.Game.Screens.Editor.SubScreens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;

namespace MuseDashEditor.Game.Screens.Editor;

public partial class EditorScreen : Screen
{
    [Cached] protected readonly EditorClock EditorClock = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Children =
            [
                // UI
                new EditorBackground(),
                new EditorSubscreenContainer(),
                new Toolbar(),
                new PlayBar(),

                // Internal
                EditorClock
            ]
        };
    }
}
