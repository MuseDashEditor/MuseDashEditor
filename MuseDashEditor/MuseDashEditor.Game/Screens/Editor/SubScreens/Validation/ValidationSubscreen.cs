using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Validation;

public partial class ValidationSubscreen : EditorSubscreen
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
                Text = "Validation subscreen"
            }
        ];
    }
}
