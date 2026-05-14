using MuseDashEditor.Game.Screens.Editor.Components;
using MuseDashEditor.Game.Screens.Editor.SubScreens;
using osu.Framework.Allocation;
using osu.Framework.Screens;

namespace MuseDashEditor.Game.Screens.Editor;

public partial class EditorScreen : Screen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            new Toolbar(),
            new EditorSubscreenContainer(),
            new PlayBar()
        ];
    }
}
