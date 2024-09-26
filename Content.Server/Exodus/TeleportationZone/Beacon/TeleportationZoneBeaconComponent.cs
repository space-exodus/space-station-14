using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Content.Shared.Containers.ItemSlots;

namespace Content.Server.Exodus.TeleportationZone.Beacon
{
    [RegisterComponent]
    public sealed partial class TeleportationZoneBeaconComponent : Component
    {
        [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
        public TimeSpan NextUpdate;

        [DataField]
        public TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

        public bool SaveCoreProcessing = false;
        public int CurrentSecondProcessing = 0;

        [DataField("scanModule")]
        public ItemSlot ScanModule = new();

        public string CurrentTexturePath = "/Textures/Exodus/TeleportationZone/UI/ProgressBar/progress0.png";

        [DataField("variantsTexturePath")]
        public Dictionary<int, string> VariantsTexturePath = new();

        public TeleportationZoneScanModuleComponent ScanComp;
    }
}
