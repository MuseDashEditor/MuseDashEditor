using MuseDashEditor.Game.Data.Holder;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata;

public partial class MetadataSubscreen : EditorSubscreen
{
    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder)
    {
        InternalChildren =
        [
            new SpriteText
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Text = "Metadata subscreen"
            }
        ];
    }
}
