using MuseDashEditor.Game.Data.Type;

namespace MuseDashEditor.Game.Data.Object.GameObject;

public class GameObject(
    double offset,
    ObjectType objectType,
    LaneType laneType,
    LaneModifierType laneModifier
) : BaseObject(offset)
{
    public ObjectType ObjectType { get; } = objectType;
    public LaneType LaneType { get; set; } = laneType;
    public LaneModifierType LaneModifier { get; set; } = laneModifier;
}
