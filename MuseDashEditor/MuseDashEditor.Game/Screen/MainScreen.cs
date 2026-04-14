using osu.Framework.Allocation;

namespace MuseDashEditor.Game.Screen;

public partial class MainScreen : osu.Framework.Screens.Screen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = [];
    }
}
