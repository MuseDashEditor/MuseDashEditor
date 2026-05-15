using System.Drawing;
using System.Linq;
using MuseDashEditor.Resources;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Handlers.Tablet;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osuTK;

namespace MuseDashEditor.Game;

public partial class MuseDashEditorGameBase : osu.Framework.Game
{
    protected override Container<Drawable> Content { get; }
    private DependencyContainer dependencies = null!;

    protected MuseDashEditorGameBase()
    {
        base.Content.Add(Content = new DrawSizePreservingFillContainer
        {
            TargetDrawSize = new Vector2(1920, 1080),
            Strategy = DrawSizePreservationStrategy.Minimum
        });
    }

    [BackgroundDependencyLoader]
    private void load(FrameworkConfigManager config, IRenderer renderer, GameHost gameHost)
    {
        Resources.AddStore(new DllResourceStore(typeof(MuseDashEditorResources).Assembly));
        dependencies.CacheAs(new LargeTextureStore(renderer,
            gameHost.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, "Textures"))));

        config.GetBindable<WindowMode>(FrameworkSetting.WindowMode).Value = WindowMode.Windowed;
        config.GetBindable<Size>(FrameworkSetting.WindowedSize).Value = new Size(1920, 1080);
        config.GetBindable<double>(FrameworkSetting.WindowedPositionX).Value = 0.5;
        config.GetBindable<double>(FrameworkSetting.WindowedPositionY).Value = 0.5;
        config.GetBindable<FrameSync>(FrameworkSetting.FrameSync).Value = FrameSync.Limit2x;
        config.GetBindable<ExecutionMode>(FrameworkSetting.ExecutionMode).Value = ExecutionMode.MultiThreaded;
        config.GetBindable<bool>(FrameworkSetting.ShowUnicode).Value = true;

        ITabletHandler tablet = Host.AvailableInputHandlers.OfType<ITabletHandler>().SingleOrDefault();

        if (tablet != null)
        {
            tablet.Enabled.Value = false;
        }
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
