namespace MuseDashEditor.Game.Data.Object.MappingObject;

public sealed class TimingPointObject(double offset, double newBpm) : MappingObject(offset)
{
    public double NewBpm { get; set; } = newBpm;
}
