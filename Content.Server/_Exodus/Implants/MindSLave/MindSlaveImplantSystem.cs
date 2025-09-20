using Content.Server.Popups;
using Content.Server.Chat.Managers;
using Robust.Server.Player;
using Content.Shared.Ghost;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Content.Shared.Implants;
using Content.Shared.Mindshield.Components;
using Content.Shared.Tag;
using Content.Shared.Exodus.Implants.MindSlave.Components;


namespace Content.Server.Exodus.Implants.MindSlave;

public sealed partial class MindSlaveImplantSystem : EntitySystem
{
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;

    [ValidatePrototypeId<TagPrototype>]
    public const string MindSlaveTag = "MindSlave";
    private readonly Dictionary<EntityUid, EntityUid> _mindSlaveImplantMap = new();
    private readonly Dictionary<EntityUid, EntityUid> _mindSlaveImplantUsers = new();

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MindSlaveImplantComponent, ImplantImplantedEvent>(ImplantCheck);
        SubscribeLocalEvent<MindSlaveImplantComponent, EntGotRemovedFromContainerMessage>(OnImplantDraw);
        SubscribeLocalEvent<MindSlaveImplantComponent, ImplantInjectEvent>(AddMindSlaveAttempt);
    }

    public void ImplantCheck(EntityUid uid, MindSlaveImplantComponent component, ImplantImplantedEvent ev)
    {
        if (ev.Implanted == null)
            return;

        if (MindSlaveRemovalCheck(ev.Implanted.Value, ev.Implant))
            return;

        if (!_mindSlaveImplantMap.TryAdd(ev.Implanted.Value, ev.Implant))
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

        if (!_mindSlaveImplantUsers.TryGetValue(ev.Implanted.Value, out var masterUid))
            return;

        EnsureComp<MindSlaveMasterComponent>(masterUid);

        _playerManager.TryGetSessionByEntity(masterUid, out var playerMasterSession);
        if (playerMasterSession != null)
        {
            _chatManager.DispatchServerMessage(
                playerMasterSession,
                Loc.GetString("mindslave-implant-master-success")
            );
        }

        var masterNetId = EntityManager.GetNetEntity(masterUid);
        if (!TryComp<MindSlaveMasterComponent>(masterUid, out var masterComp))
            return;

        if (!TryComp<MindSlaveComponent>(ev.Implanted.Value, out var slaveComp))
            return;

        var slaveNetId = EntityManager.GetNetEntity(ev.Implanted.Value);

        slaveComp.Master = masterUid;
        Dirty(ev.Implanted.Value, slaveComp);

        if (!masterComp.IconList.Contains(masterNetId))
        {
            masterComp.IconList.Add(masterNetId);
        }

        if (!masterComp.IconList.Contains(slaveNetId))
        {
            masterComp.IconList.Add(slaveNetId);
        }

        Dirty(masterUid, masterComp);
    }

    private void AddMindSlaveAttempt(EntityUid uid, MindSlaveImplantComponent component, ImplantInjectEvent ev)
    {
        if (!_mindSlaveImplantUsers.TryAdd(ev.Target, ev.User))
            return;
    }

    public bool MindSlaveRemovalCheck(EntityUid implanted, EntityUid implant)
    {
        if (!_mindSlaveImplantUsers.TryGetValue(implanted, out var master))
        {
            return true;
        }

        bool shouldRemove =
        HasComp<MindShieldComponent>(implanted) ||
        master == implanted ||
        HasComp<MindSlaveComponent>(master) ||
        HasComp<MindSlaveMasterComponent>(implanted) ||
        HasComp<GhostComponent>(master);

        if (!shouldRemove)
            return false;

        var message = Loc.GetString(
            HasComp<MindShieldComponent>(implanted) ? "implanter-component-mindslave-implant-failed" :
            master == implanted ? "implanter-component-mindslave-implant-for-master-failed" :
            HasComp<MindSlaveComponent>(master) ? "implanter-component-mindslave-implant-for-slave-failed" :
            HasComp<MindSlaveMasterComponent>(implanted) ? "implanter-component-target-master" :
            "implanter-component-mindslave-user-ghost"
            );

        var popupType = HasComp<MindShieldComponent>(implanted) ? PopupType.Large : PopupType.Medium;

        _popup.PopupEntity(message, implanted, popupType);
        _mindSlaveImplantUsers.Remove(implanted);

        if (implant.IsValid())
            QueueDel(implant);

        return true;
    }

    public void RemoveMindSlave(EntityUid uid, MindShieldImplantComponent component, ImplantImplantedEvent ev)
    {
        if (ev.Implanted == null)
            return;

        if (HasComp<MindShieldImplantComponent>(ev.Implant) && HasComp<MindSlaveComponent>(ev.Implanted))
        {
            _playerManager.TryGetSessionByEntity(ev.Implanted.Value, out var playerSession);

            if (playerSession != null)
            {
                _chatManager.DispatchServerMessage(
                playerSession,
                Loc.GetString("mindslave-implant-remove")
                );
            }

            if (!TryComp<MindSlaveComponent>(ev.Implanted.Value, out var slaveComp)
                || !TryComp<MindSlaveMasterComponent>(slaveComp.Master, out var masterComp))
                return;

            var slaveNetId = EntityManager.GetNetEntity(ev.Implanted.Value);
            var masterUid = slaveComp.Master;
            var masterNetId = EntityManager.GetNetEntity(masterUid);

            if (masterComp.IconList.Contains(slaveNetId))
            {
                masterComp.IconList.Remove(slaveNetId);
                Dirty(masterUid, masterComp);
            }

            if (_mindSlaveImplantUsers.ContainsKey(ev.Implanted.Value))
                _mindSlaveImplantUsers.Remove(ev.Implanted.Value);

            if (masterComp.IconList.Contains(masterNetId) && masterComp.IconList.Count == 1)
            {
                _playerManager.TryGetSessionByEntity(masterUid, out var playerMasterSession);
                if (playerMasterSession != null)
                {
                    _chatManager.DispatchServerMessage(
                    playerMasterSession,
                    Loc.GetString("mindslave-implant-master-noslaves")
                    );
                }

                RemComp<MindSlaveMasterComponent>(masterUid);
            }

            RemComp<MindSlaveComponent>(ev.Implanted.Value);

            if (!_mindSlaveImplantMap.TryGetValue(ev.Implanted.Value, out var implantUid))
                return;

            QueueDel(implantUid);
        }
    }


    private void OnImplantDraw(Entity<MindSlaveImplantComponent> ent, ref EntGotRemovedFromContainerMessage args)
    {
        if (!HasComp<MindSlaveComponent>(args.Container.Owner))
            return;

        _playerManager.TryGetSessionByEntity(args.Container.Owner, out var playerSession);
        if (playerSession != null)
        {
            _chatManager.DispatchServerMessage(
            playerSession,
            Loc.GetString("mindslave-implant-remove")
            );
        }

        if (!_mindSlaveImplantUsers.TryGetValue(args.Container.Owner, out var masterUid)
            || !TryComp<MindSlaveMasterComponent>(masterUid, out var masterComp))
            return;

        var masterNetId = EntityManager.GetNetEntity(masterUid);
        var slaveNetId = EntityManager.GetNetEntity(args.Container.Owner);

        if (masterComp.IconList.Contains(slaveNetId))
        {
            masterComp.IconList.Remove(slaveNetId);
            Dirty(masterUid, masterComp);
        }

        if (_mindSlaveImplantUsers.ContainsKey(args.Container.Owner))
            _mindSlaveImplantUsers.Remove(args.Container.Owner);

        if (masterComp.IconList.Contains(masterNetId) && masterComp.IconList.Count == 1)
        {
            _playerManager.TryGetSessionByEntity(masterUid, out var playerMasterSession);
            if (playerMasterSession != null)
            {
                _chatManager.DispatchServerMessage(
                playerMasterSession,
                Loc.GetString("mindslave-implant-master-noslaves")
                );
            }
            RemComp<MindSlaveMasterComponent>(masterUid);
        }

        RemComp<MindSlaveComponent>(args.Container.Owner);
    }
}
