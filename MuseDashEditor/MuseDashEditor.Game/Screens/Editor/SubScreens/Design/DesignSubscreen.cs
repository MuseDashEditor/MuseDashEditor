using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Design;

public partial class DesignSubscreen : Screen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            new SpriteText
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Text = "Design subscreen"
            }
        ];
    }
}
