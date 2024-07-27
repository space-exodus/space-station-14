using Content.Shared.Exodus.Abilities.Events;
using Robust.Shared.Prototypes;

namespace Content.Shared.Magic.Events;

public sealed partial class DirectionProjectileSpellEvent : DirectionActionEvent, ISpeakSpell
{
    /// <summary>
    /// What entity should be spawned.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId Prototype;

    [DataField]
    public string? Speech { get; private set; }
}
