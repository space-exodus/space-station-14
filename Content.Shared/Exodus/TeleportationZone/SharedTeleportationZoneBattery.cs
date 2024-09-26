using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.TeleportationZone;

[Serializable, NetSerializable]
public enum TeleportationZoneBatteryUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneBatteryUiState : BoundUserInterfaceState
{
    public bool CanDownload { get; }
    public bool CanUnload { get; }
    public string BatteryTexturePath { get; }
    public string SmallBatteryTexturePath { get; }
    public int AmountMatterBattery { get; }
    public int AmountMatterSmallBattery { get; }

    public TeleportationZoneBatteryUiState(
        bool canDownload,
        bool canUnload,
        string batteryTexturePath,
        string smallBatteryTexturePath,
        int amountMatterBattery,
        int amountMatterSmallBattery)
    {
        CanDownload = canDownload;
        CanUnload = canUnload;
        BatteryTexturePath = batteryTexturePath;
        SmallBatteryTexturePath = smallBatteryTexturePath;
        AmountMatterBattery = amountMatterBattery;
        AmountMatterSmallBattery = amountMatterSmallBattery;
    }
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneBatteryDownloadMatterMessage : BoundUserInterfaceMessage
{
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneBatteryUnloadMatterMessage : BoundUserInterfaceMessage
{
}
