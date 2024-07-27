using System.Linq;
using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Timing;

namespace Content.Shared.Exodus.Abilities;

public abstract partial class SharedAbilitiesSystem : EntitySystem
{
    [Dependency] protected readonly SharedTransformSystem TransformSystem = default!;
    [Dependency] protected readonly SharedActionsSystem ActionsSystem = default!;
    [Dependency] protected readonly IGameTiming Timing = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<AbilitiesComponent, ComponentStartup>(OnStartUp);
        SubscribeLocalEvent<AbilitiesComponent, DamageChangedEvent>(OnDamageChanged);

        InitializeAbilityCast();
        InitializeDoAbility();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        UpdateDoAbility(frameTime);
    }

    private void OnStartUp(EntityUid uid, AbilitiesComponent component, ComponentStartup args)
    {
        if (!TryComp<ActionsComponent>(uid, out var actionComp))
            return;

        foreach (var act in component.GrantActions)
        {
            var actionUid = ActionsSystem.AddAction(uid, act, uid, actionComp);
            if (actionUid != null)
                component.Actions.Add(act, actionUid.Value);
        }
        Log.Info("Calling RefreshAvailableAbilities");
        RefreshAvailableAbilities(uid, component);
    }

    public void RefreshAvailableAbilities(EntityUid uid, AbilitiesComponent? abilities = null, DamageableComponent? damageable = null)
    {
        Log.Info("Called RefreshAvailableAbilities");
        if (!Resolve(uid, ref abilities, ref damageable))
            return;
        Log.Info($"RefreshAvailableAbilities resolve passed; {damageable.TotalDamage}");

        var currentActions = GetActionsForThreshold(damageable.TotalDamage, abilities);

        foreach (var action in abilities.Actions)
        {
            ActionsSystem.SetEnabled(action.Value, currentActions.Contains(action.Key));
        }
    }

    private List<string> GetActionsForThreshold(FixedPoint2 damage, AbilitiesComponent abilities)
    {
        var result = abilities.GrantActions;

        foreach (var threshold in abilities.ActionsThresholds)
        {
            if (damage >= threshold.Key)
            {
                result = threshold.Value;
            }
        }

        return result;
    }

    private void OnDamageChanged(EntityUid uid, AbilitiesComponent abilities, ref DamageChangedEvent ev)
    {
        RefreshAvailableAbilities(uid, abilities, ev.Damageable);
    }
}
