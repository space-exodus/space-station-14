using Content.Shared.Containers.ItemSlots;

namespace Content.Server.Exodus.FTLKey;

[RegisterComponent]
public sealed partial class FTLAccessConsoleComponent : Component
{

    [DataField("slots")]
    public Dictionary<string, ItemSlot> Slots = [];
}
