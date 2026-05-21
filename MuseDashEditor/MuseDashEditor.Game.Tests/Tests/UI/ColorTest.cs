using MuseDashEditor.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace MuseDashEditor.Game.Tests.Tests.UI;

public partial class ColorTest : MuseDashEditorTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Children =
        [
            new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(1, 1),
                Direction = FillDirection.Vertical,
                Children =
                [
                    new SpriteText
                    {
                        Text = "Foreground"
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Children =
                        [
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Foreground1
                            },
                        ]
                    },
                    new SpriteText
                    {
                        Text = "Backgrounds"
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Children =
                        [
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Background1
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Background2
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Background3
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Background4
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Background5
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Background6
                            }
                        ]
                    },
                    new SpriteText
                    {
                        Text = "Dark"
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Children =
                        [
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Dark1
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Dark2
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Dark3
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Dark4
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Dark5
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Dark6
                            }
                        ]
                    },
                    new SpriteText
                    {
                        Text = "Light"
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Children =
                        [
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Light1
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Light2
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Light3
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Light4
                            }
                        ]
                    },
                    new SpriteText
                    {
                        Text = "Content"
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Children =
                        [
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Content1
                            },
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Content2
                            }
                        ]
                    },
                    new SpriteText
                    {
                        Text = "Highlight"
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Children =
                        [
                            new Box
                            {
                                Size = new Vector2(100, 100),
                                Colour = MdeColors.Highlight1
                            }
                        ]
                    }
                ]
            }
        ];
    }
}
