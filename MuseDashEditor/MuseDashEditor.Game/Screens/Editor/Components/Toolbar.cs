using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Box = osu.Framework.Graphics.Shapes.Box;

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
                Colour = MdeColors.Background5,
            },

            // Left part // TODO

            // Right part
            new SubscreenSwitcher()
        ];
    }
}
