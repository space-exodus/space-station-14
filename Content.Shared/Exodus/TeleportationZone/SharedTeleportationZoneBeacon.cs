using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.TeleportationZone;

[Serializable, NetSerializable]
public enum TeleportationZoneBeaconUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneBeaconUiState : BoundUserInterfaceState
{
    public bool CanSaveUINCore { get; }
    public string TexPath { get; }

    public TeleportationZoneBeaconUiState(bool canSaveUINCore, string texPath)
    {
        CanSaveUINCore = canSaveUINCore;
        TexPath = texPath;
    }
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneBeaconSaveUINCoreMessage : BoundUserInterfaceMessage
{
}
