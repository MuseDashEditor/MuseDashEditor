using System;
using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Screens.Editor.SubScreens.Metadata;
using MuseDashEditor.Game.Tests.Resources;
using NUnit.Framework;
using osu.Framework.Allocation;

namespace MuseDashEditor.Game.Tests.Tests.Editor.Subscreens.Metadata;

[TestFixture]
public partial class MetadataSubscreenTest : MuseDashEditorTestScene
{
    [Cached] protected readonly EditorDataHolder EditorDataHolder = new();

    [Test]
    public void TestScreenLoad()
    {
        var loadChartTask = TestResources.GetTestChart();

        AddUntilStep("Load test chart", () => loadChartTask.IsCompletedSuccessfully);
        AddStep("Test metadata screen load", () =>
        {
            if (!loadChartTask.IsCompletedSuccessfully) throw new InvalidOperationException("Test chart failed to load");

            EditorDataHolder.CurrentChart.Value = loadChartTask.Result;

            Child = new MetadataSubscreen();
            Child.Show();

            // TODO: validate screen content
        });
    }
}
