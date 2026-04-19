using MuseDashEditor.Game.Screens.Editor.Components;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace MuseDashEditor.Game.Screens.Editor;

public partial class EditorScreen : Screen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var subscreenStack = new ScreenStack { RelativeSizeAxes = Axes.Both };

        InternalChildren =
        [
            new Toolbar(),
            subscreenStack,
            new PlayBar()
        ];

        subscreenStack.Push(new MetadataSubscreen());
    }
}
