using System.Collections.Generic;
using System.IO;
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Bindables;

namespace MuseDashEditor.Game.Data.Chart;

public class Chart(
    DirectoryInfo directory,
    FileInfo musicFile,
    FileInfo demoFile,
    ChartInfo info,
    Dictionary<DifficultyType, Map> maps
)
{
    public DirectoryInfo Directory => directory;
    public ChartInfo ChartInfo => info;

    public Dictionary<DifficultyType, Map> Maps => maps;
    public Map EasyMap = maps.GetValueOrDefault(DifficultyType.Easy);
    public Map HardMap = maps.GetValueOrDefault(DifficultyType.Hard);
    public Map MasterMap = maps.GetValueOrDefault(DifficultyType.Master);
    public Map HiddenMap = maps.GetValueOrDefault(DifficultyType.Hidden);

    private readonly Bindable<FileInfo> musicFileBindableInternal = new(musicFile);
    public IBindable<FileInfo> MusicFileBindable => musicFileBindableInternal;

    private readonly Bindable<FileInfo> demoFileBindableInternal = new(demoFile);
    public IBindable<FileInfo> DemoFileBindable => demoFileBindableInternal;
}
