using osu.Framework.Platform;
using osu.Framework;
using MuseDashEditor.Game;

namespace MuseDashEditor.Desktop;

public static class Program
{
    public static void Main()
    {
        using GameHost host = Host.GetSuitableDesktopHost(@"MuseDashEditor");
        using osu.Framework.Game game = new MuseDashEditorGame();
        host.Run(game);
    }
}
