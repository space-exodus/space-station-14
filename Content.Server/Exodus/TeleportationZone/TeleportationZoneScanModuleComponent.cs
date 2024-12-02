namespace Content.Server.Exodus.TeleportationZone
{
    [RegisterComponent]
    public sealed partial class TeleportationZoneScanModuleComponent : Component
    {
        [DataField]
        public EntityUid UIN = new();

        public bool DataRefreshed = false;
    }
}
