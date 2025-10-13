// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
using Content.Server.Exodus.EntityEffects.Components;
using Robust.Shared.Physics.Events;
using Content.Shared.Mobs.Components;

namespace Content.Server.Exodus.EntityEffects;

public sealed partial class EntityEffectsOnCollideSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EntityEffectsOnCollideComponent, StartCollideEvent>(OnStartCollide);
    }

    private void OnStartCollide(EntityUid uid, EntityEffectsOnCollideComponent comp, StartCollideEvent args)
    {
        if (comp.Effects.Count == 0)
            return;

        var otherUid = args.OtherEntity;

        if (comp.OnlyForMob && !HasComp<MobStateComponent>(otherUid))
            return;

        foreach (var effect in comp.Effects)
        {
            effect.Effect(new(otherUid, EntityManager));
        }

        if (comp.DeleteAfterCollide && uid != null)
            QueueDel(uid);
    }
}
