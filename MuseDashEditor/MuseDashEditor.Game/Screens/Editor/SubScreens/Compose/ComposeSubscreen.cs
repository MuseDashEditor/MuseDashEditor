using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Compose;

public partial class ComposeSubscreen : PlayableEditorSubscreen
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
                Text = "Compose subscreen"
            }
        ];
    }
}
