using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Screens.Editor.Components;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Timing.Components;

public partial class TimingTrack : Container
{
    [Resolved] protected EditorDataHolder DataHolder { get; private set; } = null!;

    private ZoomableScrollContainer zoomableScrollContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren =
        [
            new Box
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Size = new Vector2(1, 250),
                Colour = Color4.White,
                Depth = 10,
            },
            zoomableScrollContainer = new ZoomableScrollContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.X,
                Width = 0.8f,
                Height = 250,
                Depth = 20,
            }
        ];

        Schedule(loadTrack);
    }

    private void loadTrack()
    {
        zoomableScrollContainer.Add(new WaveformGraph
        {
            RelativeSizeAxes = Axes.Both,
            Waveform = new Waveform(DataHolder.CurrentTrackStream.Value),
            HighColour = Color4.Green,
            LowColour = Color4.Red,
            MidColour = Color4.Aqua,
            BaseColour = Color4.Aquamarine
        });
        zoomableScrollContainer.SetupZoom(10, 1, 500);
    }
}
