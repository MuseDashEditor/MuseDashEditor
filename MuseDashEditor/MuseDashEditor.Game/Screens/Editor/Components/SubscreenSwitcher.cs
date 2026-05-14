using System;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Screens.Editor.SubScreens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.Components;

public partial class SubscreenSwitcher : TabControl<EditorSubscreenType>
{
    [BackgroundDependencyLoader]
    private void load(EditorDataHolder editorDataHolder)
    {
        Current = editorDataHolder.SelectedSubscreen;
        Origin = Anchor.CentreRight;
        Anchor = Anchor.CentreRight;
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Height = 1.0f;
        TabContainer.RelativeSizeAxes &= ~Axes.X;
        TabContainer.AutoSizeAxes = Axes.X;

        var values = (EditorSubscreenType[])Enum.GetValues(typeof(EditorSubscreenType));
        Array.Reverse(values);

        foreach (var val in values)
            AddItem(val);
    }

    protected override Dropdown<EditorSubscreenType> CreateDropdown()
    {
        return null; // No dropdown
    }

    protected override TabItem<EditorSubscreenType> CreateTabItem(EditorSubscreenType value)
    {
        return new BasicTabControl<EditorSubscreenType>.BasicTabItem(value)
        {
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            AutoSizeAxes = Axes.X,
            RelativeSizeAxes = Axes.Y,

            Children = [
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(1, 1),
                    Colour = FrameworkColour.BlueGreen,
                },
                new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = value.ToString(),
                    Font = FontUsage.Default.With(size: 20),
                    Colour = Colour4.White,
                    Margin = new MarginPadding(10),
                }
            ]
        };
    }
}
