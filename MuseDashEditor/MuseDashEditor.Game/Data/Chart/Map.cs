using System.Collections.Generic;
using System.IO;
using MuseDashEditor.Game.Data.Object.GameObject;
using MuseDashEditor.Game.Data.Object.MappingObject;
using MuseDashEditor.Game.Data.Type;
using osu.Framework.Bindables;

namespace MuseDashEditor.Game.Data.Chart;

public record Map(
    FileInfo MapFile
)
{
    public readonly MapMetadata Metadata = new();

    public readonly List<GameObject> GameObjects = [];
    public readonly List<TimingPointObject> TimingPoints = [];

    public readonly SortedDictionary<int, Dictionary<(LaneModifierType, LaneType), int[]>> RawMapData = new();
}

public class MapMetadata
{
    public readonly Bindable<LaneSpeed> InitialLaneSpeed = new();
    public readonly Bindable<SceneType> InitialScene = new();
    public readonly Bindable<double> InitialBpm = new();
    public readonly Bindable<double> MusicStartOffset = new();

    public readonly Dictionary<int, double> BpmChangeKeys = new();
}
