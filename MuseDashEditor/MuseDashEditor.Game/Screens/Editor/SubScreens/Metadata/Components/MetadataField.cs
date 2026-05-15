using System;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Holder;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata.Components;

public partial class MetadataField : CompositeComponent
{
    public string Label { private get; init; }
    public Func<ChartInfo, Bindable<string>> FieldGetter { private get; init; }

    private BasicTextBox textBox;

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder dataHolder)
    {
        Size = new Vector2(120, 50);

        InternalChildren =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FrameworkColour.BlueGreenDark,
                Size = new Vector2(1f, 1f)
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(1f, 1f),
                Direction = FillDirection.Vertical,

                Children = [
                    new SpriteText
                    {
                        Text = Label
                    },
                    textBox = new BasicTextBox
                    {
                        RelativeSizeAxes = Axes.X,
                        Size = new Vector2(1f, 30),
                    }
                ]
            }
        ];

        dataHolder.CurrentChart.BindValueChanged(loadNewChart, true);
    }

    private void loadNewChart(ValueChangedEvent<Chart> evt)
    {
        var chart = evt.NewValue;
        if (chart == null) return;

        var chartInfo = chart.ChartInfo;

        var field = FieldGetter(chartInfo);
        if (field == null) return;

        textBox.Current.BindTo(field);
    }
}
