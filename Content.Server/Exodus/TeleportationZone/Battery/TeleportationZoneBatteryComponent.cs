using Content.Shared.Containers.ItemSlots;

namespace Content.Server.Exodus.TeleportationZone.Battery
{
    [RegisterComponent]
    public sealed partial class TeleportationZoneBatteryComponent : Component
    {
        [DataField("matter")]
        public int Matter = 0;

        [DataField("capacity")]
        public int Capacity = 1000;

        [DataField("smallBattery")]
        public ItemSlot SmallBattery = new();

        public bool SlotOccupied = false;

        public string CurrentTexturePathForBattery = "/Textures/Exodus/TeleportationZone/UI/Battery/battery_0.png";
        public string CurrentTexturePathForSmallBattery = "/Textures/Exodus/TeleportationZone/UI/Battery/battery_0.png";

        [DataField("variantsTexturePath")]
        public Dictionary<int, string> VariantsTexturePath = new();
    }
}
