using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.TeleportationZone;

[Serializable, NetSerializable]
public enum TeleportationZoneRepeaterUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneRepeaterUiState : BoundUserInterfaceState
{
    public string Name = "unknow";

    public TeleportationZoneRepeaterUiState(string name)
    {
        Name = name;
    }
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneRepeaterChangedMessage : BoundUserInterfaceMessage
{
    public string Name { get; }

    public TeleportationZoneRepeaterChangedMessage(string name)
    {
        Name = name;
    }
}
