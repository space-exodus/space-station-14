using Content.Shared.Materials;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;

namespace Content.Shared.Exodus.Materials.Components;

[RegisterComponent]
public sealed partial class MaterialClusterComponent : Component
{
    [DataField]
    public Dictionary<string, int> Materials = new();

    [DataField]
    public EntityWhitelist? Whitelist;
}
