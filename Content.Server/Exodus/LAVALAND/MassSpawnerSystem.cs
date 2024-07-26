using Content.Server.Exodus.MassSpawn.Methods;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Content.Server.Exodus.MassSpawn;

public sealed partial class MassSpawnerSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly EntityManager _entityManager = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public List<EntityUid> _entDelList = [];

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<MassSpawnerComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            UpdateCompGroups(frameTime, uid, component);
            UpdateComponentGroupsList(frameTime, uid, component);

            if (component.RemoveAfterUsed && component.Groups.Count == 0)
                _entDelList.Add(uid);
        }

        foreach (var uid in _entDelList)
            QueueDel(uid);
        _entDelList.Clear();
    }

    private void UpdateComponentGroupsList(float frameTime, EntityUid uid, MassSpawnerComponent comp)
    {
        comp.Groups.RemoveAll(x => comp.RemovingGroups.Contains(x));
        comp.RemovingGroups = [];

        comp.Groups.AddRange(comp.AddingGroups);
        comp.AddingGroups = [];
    }

    private void UpdateCompGroups(float frameTime, EntityUid uid, MassSpawnerComponent comp)
    {

        foreach (var group in comp.Groups)
        {
            if (!group.GroupInit)
                InitializeGroup(uid, comp, group);

            switch (group.Method?.LifeStage)
            {
                case AttackCeilMethodStage.BeforeStart:
                    if (_gameTiming.CurTime >= group.GroupInitTime + TimeSpan.FromSeconds(group.DelayBehindStart))
                        group.Method.Start();
                    break;
                case AttackCeilMethodStage.Active:
                    group.Method.Update(_gameTiming.CurTime);
                    break;
                case AttackCeilMethodStage.Dead:
                    comp.RemoveGroup(group);
                    break;
                default:
                    Logger.Error("Null Group Method");
                    break;
            }
        }
    }

    private void InitializeGroup(EntityUid uid, MassSpawnerComponent comp, MassSpawnGroup group)
    {
        if (group.GroupProto is not null)
        {
            if (_prototypeManager.TryIndex<MassSpawnGroupPrototype>(group.GroupProto, out var patternProto))
                group.CopyFrom(patternProto.Group);
            else Logger.Error("Cannot index pattern proto");
        }

        if (group.Method is null)
        {
            Logger.Error("Null method");
            return;
        }

        group.GroupInitTime = _gameTiming.CurTime;
        group.Method.MethodInitialize(uid, comp, group, _transform, _entityManager);
        group.GroupInit = true;

    }
}
