using System;
using MuseDashEditor.Game.Chart.Parser;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Logging;
using osuTK;

namespace MuseDashEditor.Game.Screen;

public partial class OpenChartScreen : osu.Framework.Screens.Screen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var fileSelector = new BasicFileSelector(null, [".bms"])
        {
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(1, 1),
            Origin = Anchor.Centre,
            Anchor = Anchor.Centre,
        };

        fileSelector.CurrentFile.ValueChanged += file =>
        {
            Logger.Log("Selected file: " + file.NewValue.FullName);

            Scheduler.AddOnce(async void () =>
            {
                try
                {
                    var chart = await ChartParser.Parse(file.NewValue);
                    Logger.Log("Chart parsed successfully");
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Failed to parse chart");
                }
            });
        };

        InternalChildren = [fileSelector];
    }
}
