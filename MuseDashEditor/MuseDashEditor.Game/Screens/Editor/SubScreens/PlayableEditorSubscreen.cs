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

        if (e.Key != Key.Space) return base.OnKeyDown(e);

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
}
