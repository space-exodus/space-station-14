// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared.Exodus.Storage;

namespace Content.Shared.Exodus.Storage.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(StorageComponentTogglerSystem))]
public sealed partial class StorageComponentTogglerComponent : Component
{
    [DataField(required: true)]
    public ComponentRegistry Components = new();

    [DataField]
    public ComponentRegistry? RemoveComponents;
}
