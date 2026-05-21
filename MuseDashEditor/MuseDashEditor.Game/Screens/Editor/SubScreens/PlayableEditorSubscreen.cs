using MuseDashEditor.Game.Data.Holder;
using MuseDashEditor.Game.Editor.Clock;
using osu.Framework.Allocation;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace MuseDashEditor.Game.Screens.Editor.SubScreens;

public partial class PlayableEditorSubscreen : EditorSubscreen
{
    [Resolved] protected EditorClock EditorClock { get; private set; } = null!;

    [BackgroundDependencyLoader]
    private void load(EditorDataHolder editorDataHolder)
    {
        editorDataHolder.SelectedSubscreen.BindValueChanged(screenChangedEvent =>
        {
            if (
                screenChangedEvent.NewValue != EditorSubscreenType.Compose
                && screenChangedEvent.NewValue != EditorSubscreenType.Design
                && screenChangedEvent.NewValue != EditorSubscreenType.Timing
            )
            {
                EditorClock.Stop();
            }
        });
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat) return false;
        if (EditorClock == null) return false;

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (e.Key)
        {
            case Key.Space:
            {
                if (EditorClock.IsRunning)
                {
                    EditorClock.Stop();
                }
                else
                {
                    EditorClock.Start();
                }

                return true;
            }
            case Key.Left:
                var amountLeft = e.ShiftPressed ? 10 : e.ControlPressed ? 10000 : 1000;
                EditorClock.Seek(EditorClock.CurrentTime - amountLeft);
                return true;
            case Key.Right:
                var amountRight = e.ShiftPressed ? 10 : e.ControlPressed ? 10000 : 1000;
                EditorClock.Seek(EditorClock.CurrentTime + amountRight);
                return true;
            default:
                return base.OnKeyDown(e);
        }
    }
}
