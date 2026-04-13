using osu.Framework.Graphics.Cursor;
using osu.Framework.Platform;
using osu.Framework.Testing;

namespace MuseDashEditor.Game.Tests
{
    public partial class MuseDashEditorTestBrowser : MuseDashEditorGameBase
    {
        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddRange([
                new TestBrowser("MuseDashEditor"),
                new CursorContainer()
            ]);
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
        }
    }
}
