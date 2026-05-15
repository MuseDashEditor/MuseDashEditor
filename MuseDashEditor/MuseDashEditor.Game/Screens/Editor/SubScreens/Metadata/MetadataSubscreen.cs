using MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata.Components;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata;

public partial class MetadataSubscreen : EditorSubscreen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new MetadataContainer
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(0.8f, 0.8f)
        };
    }
}
