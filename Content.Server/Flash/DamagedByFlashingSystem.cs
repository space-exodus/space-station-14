using Content.Server.Flash.Components;
using Content.Shared.Damage;
using Content.Shared.Flash;

namespace Content.Server.Flash;
public sealed class DamagedByFlashingSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DamagedByFlashingComponent, FlashModifiersEvent>(OnFlashAttempt); // Exodus-SensitiveEyes | FlashAttemptEvent -> FlashModifiersEVent
    }
    private void OnFlashAttempt(Entity<DamagedByFlashingComponent> ent, ref FlashModifiersEvent args) // Exodus-SensitiveEyes | FlashAttemptEvent -> FlashModifiersEVent
    {
        _damageable.TryChangeDamage(ent, ent.Comp.FlashDamage);

        //TODO: It would be more logical if different flashes had different power,
        //and the damage would be inflicted depending on the strength of the flash.
    }
}
