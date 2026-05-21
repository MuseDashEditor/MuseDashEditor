using MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Timing;

public partial class TimingSubscreen : PlayableEditorSubscreen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Children =
        [
            // Background
            new Box
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Colour = MdeColors.Background6,
                Depth = float.MaxValue
            },

            new TimingTrack()
        ];
    }
}
