using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.TeleportationZone;

[Serializable, NetSerializable]
public enum TeleportationZoneConsoleUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneConsoleUiState : BoundUserInterfaceState
{
    public bool CanStartTeleporting { get; }
    public List<string> BluespaceZones = new ();
    public Dictionary<NetEntity, string> ArrivalObjects = new ();
    public int Matter { get; }

    public TeleportationZoneConsoleUiState(bool canStartTeleporting, List<string> bluespaceZones, Dictionary<NetEntity, string> arrivalObjects, int matter)
    {
        Matter = matter;
        CanStartTeleporting = canStartTeleporting;

        BluespaceZones.Clear();
        foreach(var bluespaceZone in bluespaceZones)
        {
            BluespaceZones.Add(bluespaceZone);
        }

        ArrivalObjects.Clear();
        foreach (var arrivalObject in arrivalObjects)
        {
            ArrivalObjects.Add(arrivalObject.Key, arrivalObject.Value);
        }
    }
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneConsoleRefreshBluespaceZonesMessage : BoundUserInterfaceMessage
{
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneConsoleRefreshArrivalObjectsMessage : BoundUserInterfaceMessage
{
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneConsoleMinusMatterMessage : BoundUserInterfaceMessage
{
    public int Matter { get; }

    public TeleportationZoneConsoleMinusMatterMessage(int matter)
    {
        Matter = matter;
    }
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneConsolePlusMatterMessage : BoundUserInterfaceMessage
{
    public int Matter { get; }

    public TeleportationZoneConsolePlusMatterMessage(int matter)
    {
        Matter = matter;
    }
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneConsoleStartTeleportingMessage : BoundUserInterfaceMessage
{
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneConsoleBluespaceZoneSelectedMessage : BoundUserInterfaceMessage
{
    public string Zone { get; }

    public TeleportationZoneConsoleBluespaceZoneSelectedMessage(string zone)
    {
        Zone = zone;
    }
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneConsoleArrivalObjectSelectedMessage : BoundUserInterfaceMessage
{
    public NetEntity Object { get; }

    public TeleportationZoneConsoleArrivalObjectSelectedMessage(NetEntity _object)
    {
        Object = _object;
    }
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneConsoleCoordXChangedMessage : BoundUserInterfaceMessage
{
    public float CoordX { get; }

    public TeleportationZoneConsoleCoordXChangedMessage(float coordX)
    {
        CoordX = coordX;
    }
}

[Serializable, NetSerializable]
public sealed class TeleportationZoneConsoleCoordYChangedMessage : BoundUserInterfaceMessage
{
    public float CoordY { get; }

    public TeleportationZoneConsoleCoordYChangedMessage(float coordY)
    {
        CoordY = coordY;
    }
}
