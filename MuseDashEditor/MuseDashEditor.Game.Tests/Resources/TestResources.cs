using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using MuseDashEditor.Game.Data.Chart;
using MuseDashEditor.Game.Data.Parser;
using osu.Framework.IO.Stores;
using osu.Framework.Testing;

namespace MuseDashEditor.Game.Tests.Resources;

public class TestResources
{
    private static readonly TemporaryNativeStorage temp_storage = new("TestResources");

    public static DllResourceStore GetStore() => new(typeof(TestResources).Assembly);

    public static Stream OpenResource(string name) => GetStore().GetStream($"Resources/{name}");

    public static async Task<Chart> GetTestChart()
    {
        var directory = await GetTestChartDirectory();
        return await ChartParser.Parse(directory);
    }

    public static async Task<DirectoryInfo> GetTestChartDirectory()
    {
        var dirPath = temp_storage.GetFullPath(Guid.NewGuid().ToString());
        var directory = new DirectoryInfo(dirPath);
        if (!directory.Exists)
            directory.Create();

        await using var sourceStream = OpenResource("WACCA_ULTRA_DREAM_MEGAMIX.mdm");
        ZipFile.ExtractToDirectory(sourceStream, dirPath);

        return directory;
    }
}
