using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Data.Type;
using MuseDashEditor.Game.Screens.Open.Components;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;
using osuTK.Graphics;

namespace MuseDashEditor.Game.Screens.Open;

public partial class DifficultySelectorScreen : Screen
{
    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder)
    {
        // if (dataHolder.CurrentChart.Value == null) return; // TODO

        InternalChildren =
        [
            new FillFlowContainer
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,

                Children =
                [
                    new SpriteText
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Text = "Difficulty Selector",
                        Font = FontUsage.Default.With(size: 50),
                    },
                    new SpriteText
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Text = "CHART NAME", // TODO
                        Font = FontUsage.Default.With(size: 50),
                    },
                    new FillFlowContainer<DifficultyDisplay>
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,

                        Children =
                        [
                            new DifficultyDisplay
                            {
                                StarColour = Color4.Green,
                                Difficulty = DifficultyType.Easy
                            },
                            new DifficultyDisplay
                            {
                                StarColour = Color4.Aqua,
                                Difficulty = DifficultyType.Hard
                            },
                            new DifficultyDisplay
                            {
                                StarColour = Color4.Magenta,
                                Difficulty = DifficultyType.Master
                            },
                            new DifficultyDisplay
                            {
                                StarColour = Color4.Gray,
                                Difficulty = DifficultyType.Hidden
                            },
                        ]
                    }
                ]
            }
        ];
    }
}
