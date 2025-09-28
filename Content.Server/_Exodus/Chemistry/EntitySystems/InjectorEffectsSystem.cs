using Content.Server.Exodus.Chemistry.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.EntityEffects;

namespace Content.Server.Exodus.Chemistry.EntitySystems;

public sealed partial class InjectorEffectsSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<InjectorEffectsComponent, InjectionStartedEvent>(InjectionStarted);
        SubscribeLocalEvent<InjectorEffectsComponent, AfterInjectorUseEvent>(AfterInjectorUse);
    }

    private void InjectionStarted(EntityUid uid, InjectorEffectsComponent comp, ref InjectionStartedEvent ev)
    {
        foreach (var effect in comp.InjectionStarted)
        {
            if (!effect.ShouldApply(new(ev.Target, EntityManager)))
                continue;

            effect.Effect(new(ev.Target, EntityManager));
        }
    }

    private void AfterInjectorUse(EntityUid uid, InjectorEffectsComponent comp, ref AfterInjectorUseEvent ev)
    {
        foreach (var effect in comp.AfterInjection)
        {
            if (!effect.ShouldApply(new(ev.Target, EntityManager)))
                continue;

            effect.Effect(new(ev.Target, EntityManager));
        }
    }
}