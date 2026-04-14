using osu.Framework.Allocation;
using NUnit.Framework;

namespace MuseDashEditor.Game.Tests.Visual;

[TestFixture]
public partial class TestSceneMuseDashEditorGame : MuseDashEditorTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        AddGame(new MuseDashEditorGame());
    }
}
