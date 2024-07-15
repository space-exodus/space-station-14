using Content.Server.Engineering.Components;
using Content.Shared.DoAfter;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Verbs;
using JetBrains.Annotations;

namespace Content.Server.Engineering.EntitySystems
{
    [UsedImplicitly]
    public sealed class DisassembleOnAltVerbSystem : EntitySystem
    {
        [Dependency] private readonly SharedHandsSystem _handsSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<DisassembleOnAltVerbComponent, GetVerbsEvent<AlternativeVerb>>(AddDisassembleVerb);
        }
        private void AddDisassembleVerb(EntityUid uid, DisassembleOnAltVerbComponent component, GetVerbsEvent<AlternativeVerb> args)
        {
            if (!args.CanInteract || !args.CanAccess || args.Hands == null)
                return;

            AlternativeVerb verb = new()
            {
                Act = () =>
                {
                    AttemptDisassemble(uid, args.User, args.Target, component);
                },
                Text = Loc.GetString("disassemble-system-verb-disassemble"),
                Priority = 2
            };
            args.Verbs.Add(verb);
        }

        public async void AttemptDisassemble(EntityUid uid, EntityUid user, EntityUid target, DisassembleOnAltVerbComponent? component = null)
        {
            if (!Resolve(uid, ref component))
                return;
            if (string.IsNullOrEmpty(component.Prototype))
                return;

            if (component.DoAfterTime > 0 && TryGet<SharedDoAfterSystem>(out var doAfterSystem))
            {
                var doAfterArgs = new DoAfterArgs(EntityManager, user, component.DoAfterTime, new AwaitedDoAfterEvent(), null)
                {
                    BreakOnMove = true,
                };
                var result = await doAfterSystem.WaitDoAfter(doAfterArgs);

                if (result != DoAfterStatus.Finished)
                    return;
            }

            if (component.Deleted || Deleted(uid))
                return;

            if (!TryComp(uid, out TransformComponent? transformComp))
                return;

            // Exodus-FoldedPoster-Start
            if (!TryComp<MetaDataComponent>(uid, out var metaDataComp) ||
                metaDataComp.EntityPrototype is null)
                return;
            // Exodus-FoldedPoster-End
            var entity = EntityManager.SpawnEntity(component.Prototype, transformComp.Coordinates);

            _handsSystem.TryPickup(user, entity);

            EntityManager.DeleteEntity(uid);

            // Exodus-FoldedPoster-Start
            var ev = new DisassembleEntityEvent(metaDataComp.EntityPrototype.ID, entity);
            RaiseLocalEvent(entity, ev);
            // Exodus-FoldedPoster-End
        }
    }

    // Exodus-FoldedPoster-Start
    public sealed class DisassembleEntityEvent : EntityEventArgs
    {
        public string DisassembledEntProto;

        public EntityUid CreatedEntity;

        public DisassembleEntityEvent(string disassembledEntProto, EntityUid createdEntity)
        {
            DisassembledEntProto = disassembledEntProto;
            CreatedEntity = createdEntity;
        }
    }
    // Exodus-FoldedPoster-End
}
