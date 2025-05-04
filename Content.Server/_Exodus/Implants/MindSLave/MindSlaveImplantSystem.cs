using Content.Server.Implants.Components;
using Content.Server.Popups;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Content.Shared.Implants;
using Content.Shared.Mindshield.Components;
using Content.Shared.Tag;
using Content.Shared.Exodus.Implants.MindSlave.Components;
using Content.Server.Chat.Managers;
using Robust.Server.Player;


namespace Content.Server.Exodus.Implants.MindSlave;

public sealed partial class MindSlaveImplantSystem : EntitySystem
{
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;

    [ValidatePrototypeId<TagPrototype>]
    public const string MindSlaveTag = "MindSlave";

    private EntityUid _mindSlaveImplanted;
    private EntityUid _mindSlaveImplant;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MindShieldImplantComponent, ImplantImplantedEvent>(RemoveMindSlave);
        SubscribeLocalEvent<MindSlaveImplantComponent, ImplantImplantedEvent>(ImplantCheck);
        SubscribeLocalEvent<MindSlaveImplantComponent, EntGotRemovedFromContainerMessage>(OnImplantDraw);
    }

    public void ImplantCheck(EntityUid uid, MindSlaveImplantComponent component, ImplantImplantedEvent ev)
    {
        if (ev.Implanted == null)
            return;

        _mindSlaveImplanted = ev.Implanted.Value;
        _mindSlaveImplant = ev.Implant;

        if (_tag.HasTag(ev.Implant, MindSlaveTag))
        {

            if (MindSlaveRemovalCheck(ev.Implanted.Value, ev.Implant))
                return;

            EnsureComp<MindSlaveComponent>(ev.Implanted.Value);
            _playerManager.TryGetSessionByEntity(ev.Implanted.Value, out var playerSession);
            if (playerSession != null)
            {
                _chatManager.DispatchServerMessage(
                playerSession,
                Loc.GetString("mindslave-implant-success")
                );
            }
        }

    }

    public bool MindSlaveRemovalCheck(EntityUid implanted, EntityUid implant)
    {
        if (HasComp<MindShieldComponent>(implanted))
        {
            _popup.PopupEntity(Loc.GetString("implanter-component-mindslave-implant-failed"), implanted, PopupType.Large);
            QueueDel(implant);
            return true;
        }

        return false;
    }

    public void RemoveMindSlave(EntityUid uid, MindShieldImplantComponent component, ImplantImplantedEvent ev)
    {
        if (ev.Implanted == null)
            return;


        if (HasComp<MindShieldImplantComponent>(ev.Implant) && HasComp<MindSlaveComponent>(ev.Implanted))
        {


            RemComp<MindSlaveComponent>(ev.Implanted.Value);
            _playerManager.TryGetSessionByEntity(ev.Implanted.Value, out var playerSession);

            if (playerSession != null)
            {
                _chatManager.DispatchServerMessage(
                playerSession,
                Loc.GetString("mindslave-implant-remove")
                );
            }

            QueueDel(_mindSlaveImplant);

        }
    }


    private void OnImplantDraw(Entity<MindSlaveImplantComponent> ent, ref EntGotRemovedFromContainerMessage args)
    {
        if (!HasComp<MindSlaveComponent>(args.Container.Owner))
            return;

        RemComp<MindSlaveComponent>(args.Container.Owner);

        _playerManager.TryGetSessionByEntity(_mindSlaveImplanted, out var playerSession);
        if (playerSession != null)
        {
            _chatManager.DispatchServerMessage(
            playerSession,
            Loc.GetString("mindslave-implant-remove")
            );
        }
    }
}
