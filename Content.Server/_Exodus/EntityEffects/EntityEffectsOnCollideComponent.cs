using Content.Shared.EntityEffects;

namespace Content.Server.Exodus.EntityEffects.Components;

[RegisterComponent]
public sealed partial class EntityEffectsOnCollideComponent : Component
{
    [DataField("effects")]
    public List<EntityEffect> Effects = new();

    [DataField]
    public bool OnlyForMob = false;

    [DataField]
    public bool DeleteAfterCollide = false;
}
