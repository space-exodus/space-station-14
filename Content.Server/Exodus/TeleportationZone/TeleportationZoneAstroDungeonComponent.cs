namespace Content.Server.Exodus.TeleportationZone
{
    [RegisterComponent]
    public sealed partial class TeleportationZoneAstroDungeonComponent : Component
    {
        [DataField("name")]
        public string Name = "unknown";

        [DataField("coordX")]
        public float X = 0f;

        [DataField("coordY")]
        public float Y = 0f;

        [DataField("status")]
        public string Status = "no data available";

        [DataField("color")]
        public string Color = "white";

        [DataField("text")]
        public string Text = "no data available";
    }
}
