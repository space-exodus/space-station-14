// Exodus - Stamina Refactor | Protection & Resistance
using Content.Shared.Armor;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Inventory;


namespace Content.Shared.Damage.Systems;

public partial class SharedStaminaSystem
{
    private void InitializeResistance()
    {
        SubscribeLocalEvent<StaminaProtectionComponent, BeforeStaminaDamageEvent>(OnGetResistance);
        SubscribeLocalEvent<StaminaArmorComponent, InventoryRelayedEvent<BeforeStaminaDamageEvent>>(RelayedResistance);
        SubscribeLocalEvent<StaminaArmorComponent, ArmorExamineEvent>(OnArmorExamine);
    }


    private void OnGetResistance(Entity<StaminaProtectionComponent> ent, ref BeforeStaminaDamageEvent args)
    {
        args.Value *= ent.Comp.Coefficient;
    }

    private void RelayedResistance(Entity<StaminaArmorComponent> ent, ref InventoryRelayedEvent<BeforeStaminaDamageEvent> args)
    {
        args.Args.Value *= ent.Comp.Coefficient;
    }

    private void OnArmorExamine(Entity<StaminaArmorComponent> ent, ref ArmorExamineEvent args)
    {
        var value = MathF.Round((1f - ent.Comp.Coefficient) * 100, 1);

        if (value == 0)
            return;

        args.Msg.PushNewline();
        args.Msg.AddMarkupOrThrow(Loc.GetString(ent.Comp.Examine, ("value", value)));
    }
}
