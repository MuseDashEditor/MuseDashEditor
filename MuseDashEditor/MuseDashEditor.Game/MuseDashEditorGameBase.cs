using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osuTK;
using MuseDashEditor.Resources;

namespace MuseDashEditor.Game;

public partial class MuseDashEditorGameBase : osu.Framework.Game
{
    protected override Container<Drawable> Content { get; }

    protected MuseDashEditorGameBase()
    {
        base.Content.Add(Content = new DrawSizePreservingFillContainer
        {
            Strategy = DrawSizePreservationStrategy.Maximum,
            TargetDrawSize = new Vector2(1920, 1080)
        });
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Resources.AddStore(new DllResourceStore(typeof(MuseDashEditorResources).Assembly));
    }
}
