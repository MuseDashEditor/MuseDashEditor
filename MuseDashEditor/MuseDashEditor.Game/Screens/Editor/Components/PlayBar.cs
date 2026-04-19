using MuseDashEditor.Game.Data.Holder;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using Box = osu.Framework.Graphics.Shapes.Box;
using Color4 = osuTK.Graphics.Color4;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class PlayBar : Box // TODO
{
    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder)
    {
        Colour = Color4.White;
        RelativeSizeAxes = Axes.X;
        Height = 50;
        Width = 1.0f;
        Origin = Anchor.BottomLeft;
        Anchor = Anchor.BottomLeft;
    }
}
