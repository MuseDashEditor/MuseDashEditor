using MuseDashEditor.Game.Data.Holder;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Box = osu.Framework.Graphics.Shapes.Box;
using Color4 = osuTK.Graphics.Color4;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class Toolbar : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 50;
        Width = 1.0f;
        Origin = Anchor.TopLeft;
        Anchor = Anchor.TopLeft;

        Children =
        [
            // Background
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(1, 1),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = Color4.AliceBlue,
            },

            // Left part // TODO

            // Right part
            new SubscreenSwitcher()
        ];
    }
}
