using osu.Framework.Testing;

namespace MuseDashEditor.Game.Tests.Visual;

public abstract partial class MuseDashEditorTestScene : TestScene
{
    protected override ITestSceneTestRunner CreateRunner() => new MuseDashEditorTestSceneTestRunner();

    private partial class MuseDashEditorTestSceneTestRunner : MuseDashEditorGameBase, ITestSceneTestRunner
    {
        private TestSceneTestRunner.TestRunner runner;

        protected override void LoadAsyncComplete()
        {
            base.LoadAsyncComplete();
            Add(runner = new TestSceneTestRunner.TestRunner());
        }

        public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
    }
}
