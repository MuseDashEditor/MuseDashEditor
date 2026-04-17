using System;
using MuseDashEditor.Game.Data.Parser;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace MuseDashEditor.Game.Screens.Open;

public partial class FolderSelectorScreen : Screen
{
    [BackgroundDependencyLoader]
    private void load(ScreenStack screenStack)
    {
        var fileSelector = new BasicFileSelector(null, [".bms", ".json", ".ogg", ".mp3"])
        {
            RelativeSizeAxes = Axes.X,
            Size = new Vector2(1, 1030),
            Origin = Anchor.TopLeft,
            Anchor = Anchor.TopLeft,
        };

        InternalChildren =
        [
            fileSelector,
            new BasicButton()
            {
                Text = "Open chart",
                Size = new Vector2(200, 50),
                Colour = Colour4.AliceBlue,
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Action = () => Scheduler.Add(async void () =>
                {
                    try
                    {
                        Logger.Log($"Opening chart from {fileSelector.CurrentPath.Value.FullName}...");

                        var chart = await ChartParser.Parse(fileSelector.CurrentPath.Value);



                        this.Exit();
                        screenStack.Push(new DifficultySelectorScreen());
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Failed to open chart");
                    }
                })
            }
        ];
    }
}
