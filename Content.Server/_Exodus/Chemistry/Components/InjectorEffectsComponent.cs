using Content.Shared.EntityEffects;

namespace Content.Server.Exodus.Chemistry.Components;

[RegisterComponent]
public sealed partial class InjectorEffectsComponent : Component
{
    /// <summary>
    /// Which effects is applied to target when injection do after starts
    /// </summary>
    [DataField]
    public EntityEffect[] InjectionStarted = [];

    /// <summary>
    /// Which effects is applied to target after injection
    /// </summary>
    [DataField]
    public EntityEffect[] AfterInjection = [];
}
