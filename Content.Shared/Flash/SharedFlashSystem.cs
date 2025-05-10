using Content.Shared.FixedPoint; // Exodus-SensitiveEyes
using Content.Shared.Flash.Components;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared.Flash;

public abstract class SharedFlashSystem : EntitySystem
{
    public ProtoId<StatusEffectPrototype> FlashedKey = "Flashed";

    public virtual void FlashArea(Entity<FlashComponent?> source, EntityUid? user, float range, float duration, float slowTo = 0.8f, bool displayPopup = false, float probability = 1f, SoundSpecifier? sound = null)
    {
    }
}

// Exodus-SensitiveEyes-Start | Was "FlashAttemptEvent" in the past (server-side only event)
/// <summary>
///     Called before a flash is used to check if the attempt is cancelled by blindness, items or FlashImmunityComponent.
///     Raised on the target hit by the flash, the user of the flash and the flash used.
/// </summary>
public sealed class FlashModifiersEvent : CancellableEntityEventArgs
{
    public readonly EntityUid Target;
    public readonly EntityUid? User;
    public readonly EntityUid? Used;

    public float DurationModifier { get; private set; } = 1.0f;

    public void ModifyDuration(float duration)
    {
        DurationModifier *= duration;
    }

    public FlashModifiersEvent(EntityUid target, EntityUid? user, EntityUid? used)
    {
        Target = target;
        User = user;
        Used = used;
    }
}
// Exodus-SensitiveEyes-End
