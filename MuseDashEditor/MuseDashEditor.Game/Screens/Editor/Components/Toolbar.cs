using osu.Framework.Allocation;
using osu.Framework.Graphics;
using Box = osu.Framework.Graphics.Shapes.Box;
using Color4 = osuTK.Graphics.Color4;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class Toolbar : Box // TODO
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Colour = Color4.White;
        RelativeSizeAxes = Axes.X;
        Height = 50;
        Width = 1.0f;
        Origin = Anchor.TopLeft;
        Anchor = Anchor.TopLeft;
    }
}
