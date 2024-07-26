using Content.Shared.Containers.ItemSlots;

namespace Content.Server.Exodus.TeleportationZone.Console
{
    [RegisterComponent]
    public sealed partial class TeleportationZoneConsoleComponent : Component
    {
        public float top_border = 0f;
        public float bottom_border = 0f;
        public float left_border = 0f;
        public float right_border = 0f;

        public EntityUid FirstBeacon = new();
        public EntityUid SecondBeacon = new();
        public EntityUid LandingPoint = new();
        public float CoordX = 0f;
        public float CoordY = 0f;

        public bool ZoneSelected = false;
        public bool ArrivalObjectSelected = false;
        public bool CoordXEntered = false;
        public bool CoordYEntered = false;
        public bool CanStart = false;

        public int Matter = 0;
        public int RealMatter = 10;

        // This is necessary in order for different lists to be updated through different buttons.
        public List<string> BluespaceZones = new();
        public Dictionary<NetEntity, string> ArrivalObjects = new();

        [DataField("slots")]
        public Dictionary<string, ItemSlot> Slots = new();
    }
}
