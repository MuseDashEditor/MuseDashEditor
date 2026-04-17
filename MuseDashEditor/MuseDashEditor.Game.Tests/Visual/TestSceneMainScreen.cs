using MuseDashEditor.Game.Screens;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using NUnit.Framework;

namespace MuseDashEditor.Game.Tests.Visual;

[TestFixture]
public partial class TestSceneMainScreen : MuseDashEditorTestScene
{
    public TestSceneMainScreen()
    {
        Add(new ScreenStack(new MainScreen()) { RelativeSizeAxes = Axes.Both });
    }
}
