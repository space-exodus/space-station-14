namespace Content.Server.Exodus.TeleportationZone
{
    [RegisterComponent]
    public sealed partial class TeleportationZoneListOfPricesComponent : Component
    {
        [DataField("listOfPrices")]
        public Dictionary<string, int> ListOfPrices = new();
    }
}
