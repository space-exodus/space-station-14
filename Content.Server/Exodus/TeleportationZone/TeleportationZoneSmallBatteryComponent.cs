namespace Content.Server.Exodus.TeleportationZone
{
    [RegisterComponent]
    public sealed partial class TeleportationZoneSmallBatteryComponent : Component
    {
        [DataField("matter")]
        public int Matter = 0;

        [DataField("capacity")]
        public int Capacity = 200;
    }
}
