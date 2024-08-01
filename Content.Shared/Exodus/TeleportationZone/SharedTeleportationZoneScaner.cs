using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.TeleportationZone;

[Serializable, NetSerializable]
public enum TeleportationZoneScanerUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneScanerUiState : BoundUserInterfaceState
{
    public string Name = "no data available";
    public float X = 0f;
    public float Y = 0f;
    public string Status = "no data available";
    public string Color = "white";

    public TeleportationZoneScanerUiState(string name, float x, float y, string status, string color)
    {
        Name = name;
        X = x;
        Y = y;
        Status = status;
        Color = color;
    }
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneScanerRefreshButtonPressedMessage : BoundUserInterfaceMessage
{
}
