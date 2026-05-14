using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens;

public abstract partial class EditorSubscreen : VisibilityContainer
{
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Size = new Vector2(1, 1);
    }

    protected override void PopIn() => this.FadeIn();
    protected override void PopOut() => this.FadeOut();
}
