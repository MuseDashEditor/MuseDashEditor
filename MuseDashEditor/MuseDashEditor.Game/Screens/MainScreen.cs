using MuseDashEditor.Game.Screens.Open;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;

namespace MuseDashEditor.Game.Screens;

public partial class MainScreen : Screen
{
    [Resolved] protected ScreenStack MainScreenStack { get; private set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            new BasicButton
            {
                Text = "Open chart",
                Size = new Vector2(200, 50),
                Colour = Colour4.AliceBlue,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Action = () => MainScreenStack.Push(new FolderSelectorScreen())
            }
        ];
    }
}
