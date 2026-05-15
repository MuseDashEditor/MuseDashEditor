using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Screens.Editor.Components;
using MuseDashEditor.Game.Screens.Editor.SubScreens;
using osu.Framework.Allocation;
using osu.Framework.Screens;

namespace MuseDashEditor.Game.Screens.Editor;

public partial class EditorScreen : Screen
{
    [Cached] protected readonly EditorClock EditorClock = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            // UI
            new EditorBackground(),
            new Toolbar(),
            new EditorSubscreenContainer(),
            new PlayBar(),

            // Internal
            EditorClock
        ];
    }
}
