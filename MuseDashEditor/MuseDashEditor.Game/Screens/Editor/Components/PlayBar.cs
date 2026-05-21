using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Editor.Clock;
using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class PlayBar : CompositeDrawable
{
    private SpriteText percentText;
    private SpriteText timerText;
    private BasicSliderBar<float> slider;

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder, EditorClock clock)
    {
        RelativeSizeAxes = Axes.X;
        Height = 65;
        Width = 1.0f;
        Origin = Anchor.BottomLeft;
        Anchor = Anchor.BottomLeft;

        InternalChildren =
        [
            // Background
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(1f, 1f),
                Colour = MdeColors.Background5,
            },

            // Text
            timerText = new SpriteText
            {
                Text = "00:00.0000",
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                Position = new Vector2(125, 17.5f),
                Font = FontUsage.Default.With(size: 28f)
            },
            percentText = new SpriteText()
            {
                Text = "0.00 %",
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                Position = new Vector2(125, -12.5f),
                Font = FontUsage.Default.With(size: 28f)
            },

            // Slider
            slider = new BasicSliderBar<float>
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(1320, 20),

                Current = new BindableNumber<float> { MinValue = 0, MaxValue = 1, Precision = .01f }
            }

            // Play button
            // TODO
        ];
    }
}
