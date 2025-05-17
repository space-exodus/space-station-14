using Content.Server.Popups;
using Content.Server.Chat.Managers;
using Robust.Server.Player;
using Content.Shared.Whitelist;
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
    private readonly Dictionary<EntityUid, EntityUid> _mindSLaveImplantMap = new();
    private readonly Dictionary<EntityUid, EntityUid> _mindSlaveImplantUsers = new();

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MindShieldImplantComponent, ImplantImplantedEvent>(RemoveMindSlave);
        SubscribeLocalEvent<MindSlaveImplantComponent, ImplantImplantedEvent>(ImplantCheck);
        SubscribeLocalEvent<MindSlaveImplantComponent, EntGotRemovedFromContainerMessage>(OnImplantDraw);
        SubscribeLocalEvent<MindSlaveImplantComponent, ImplantInjectEvent>(AddMindSlaveAttempt);
    }

    public void ImplantCheck(EntityUid uid, MindSlaveImplantComponent component, ImplantImplantedEvent ev)
    {
        if (ev.Implanted == null)
            return;

        if (_tag.HasTag(ev.Implant, MindSlaveTag))
        {

            if (MindSlaveRemovalCheck(ev.Implanted.Value, ev.Implant))
                return;

            _mindSLaveImplantMap[ev.Implanted.Value] = ev.Implant;

            EnsureComp<MindSlaveComponent>(ev.Implanted.Value);

            _playerManager.TryGetSessionByEntity(ev.Implanted.Value, out var playerSession);
            if (playerSession != null)
            {
                _chatManager.DispatchServerMessage(
                playerSession,
                Loc.GetString("mindslave-implant-success")
                );
            }


            EnsureComp<MindSlaveMasterComponent>(_mindSlaveImplantUsers[ev.Implanted.Value]);
            if (!_mindSlaveImplantUsers.TryGetValue(ev.Implanted.Value, out var master))
                return;

            EnsureComp<MindSlaveMasterComponent>(master);

            _playerManager.TryGetSessionByEntity(_mindSlaveImplantUsers[ev.Implanted.Value], out var playerMasterSession);
            if (playerMasterSession != null)
            {
                _chatManager.DispatchServerMessage(
                    playerMasterSession,
                    Loc.GetString("mindslave-implant-master-success")
                );
            }

            var masterNetId = EntityManager.GetNetEntity(_mindSlaveImplantUsers[ev.Implanted.Value]);
            var masterComp = EntityManager.GetComponent<MindSlaveMasterComponent>(_mindSlaveImplantUsers[ev.Implanted.Value]);


            var slaveComp = EntityManager.GetComponent<MindSlaveComponent>(ev.Implanted.Value);
            slaveComp.Master = masterNetId;
            Dirty(ev.Implanted.Value, slaveComp);

            if (!masterComp.Slaves.Contains(ev.Implanted.Value))
            {
                masterComp.Slaves.Add(ev.Implanted.Value);
                Dirty(_mindSlaveImplantUsers[ev.Implanted.Value], masterComp);
            }

            var iconWhiteList = new List<NetEntity>();
            if (masterComp.Slaves != null)
            {
                foreach (var slave in masterComp.Slaves)
                {
                    iconWhiteList.Add(EntityManager.GetNetEntity(slave));
                }
            }
            iconWhiteList.Add(EntityManager.GetNetEntity(_mindSlaveImplantUsers[ev.Implanted.Value]));

            var whitelistSlaves = new EntityWhitelist
            {
                NetEntities = iconWhiteList,
            };

            masterComp.SlavesWhiteList = whitelistSlaves;

            Dirty(_mindSlaveImplantUsers[ev.Implanted.Value], masterComp);

        }

    }

    private void AddMindSlaveAttempt(EntityUid uid, MindSlaveImplantComponent component, ImplantInjectEvent ev)
    {
        _mindSlaveImplantUsers[ev.Target] = ev.User;
    }

    public bool MindSlaveRemovalCheck(EntityUid implanted, EntityUid implant)
    {
        if (!_mindSlaveImplantUsers.TryGetValue(implanted, out var master))
        {
            return true;
        }

        if (HasComp<MindShieldComponent>(implanted))
        {
            _popup.PopupEntity(Loc.GetString("implanter-component-mindslave-implant-failed"), implanted, PopupType.Large);
            QueueDel(implant);
            _mindSlaveImplantUsers.Remove(implanted);
            return true;
        }
        else if (master == implanted)
        {
            _popup.PopupEntity(Loc.GetString("implanter-component-mindslave-implant-for-master-failed"), implanted, PopupType.Medium);
            _mindSlaveImplantUsers.Remove(implanted);
            QueueDel(implant);
            return true;
        }
        else if (HasComp<MindSlaveComponent>(master))
        {
            _popup.PopupEntity(Loc.GetString("implanter-component-mindslave-implant-for-slave-failed"), implanted, PopupType.Medium);
            _mindSlaveImplantUsers.Remove(implanted);
            QueueDel(implant);
            return true;
        }
        else if (HasComp<MindSlaveMasterComponent>(implanted))
        {
            _popup.PopupEntity(Loc.GetString("implanter-component-target-master"), implanted, PopupType.Medium);
            _mindSlaveImplantUsers.Remove(implanted);
            QueueDel(implant);
            return true;
        }
        else if (HasComp<GhostComponent>(master))
        {
            _popup.PopupEntity(Loc.GetString("implanter-component-mindslave-user-ghost"), implanted, PopupType.Medium);
            _mindSlaveImplantUsers.Remove(implanted);
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
            _playerManager.TryGetSessionByEntity(ev.Implanted.Value, out var playerSession);

            if (playerSession != null)
            {
                _chatManager.DispatchServerMessage(
                playerSession,
                Loc.GetString("mindslave-implant-remove")
                );
            }

            var slaveComp = EntityManager.GetComponent<MindSlaveComponent>(ev.Implanted.Value);
            var masterUid = EntityManager.GetEntity(slaveComp.Master);
            var masterComp = EntityManager.GetComponent<MindSlaveMasterComponent>(masterUid);

            if (masterComp.Slaves.Contains(ev.Implanted.Value))
            {
                masterComp.Slaves.Remove(ev.Implanted.Value);
                Dirty(masterUid, masterComp);
            }


            if (masterComp.SlavesWhiteList.NetEntities.Contains(EntityManager.GetNetEntity(ev.Implanted.Value)))
            {
                masterComp.SlavesWhiteList.NetEntities.Remove(EntityManager.GetNetEntity(ev.Implanted.Value));
                Dirty(masterUid, masterComp);
            }

            if (masterComp.Slaves.Count == 0)
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

            if (!_mindSLaveImplantMap.TryGetValue(ev.Implanted.Value, out var implantUid))
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

        if (!_mindSlaveImplantUsers.TryGetValue(args.Container.Owner, out var masterUid))
            return;

        var masterComp = EntityManager.GetComponent<MindSlaveMasterComponent>(masterUid);

        if (masterComp.Slaves.Contains(args.Container.Owner))
        {
            masterComp.Slaves.Remove(args.Container.Owner);
            Dirty(masterUid, masterComp);
        }

        if (masterComp.SlavesWhiteList.NetEntities.Contains(EntityManager.GetNetEntity(args.Container.Owner)))
        {
            masterComp.SlavesWhiteList.NetEntities.Remove(EntityManager.GetNetEntity(args.Container.Owner));
            Dirty(masterUid, masterComp);
        }

        if (masterComp.Slaves.Count == 0)
        {
            _playerManager.TryGetSessionByEntity(_mindSlaveImplantUsers[args.Container.Owner], out var playerMasterSession);
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
