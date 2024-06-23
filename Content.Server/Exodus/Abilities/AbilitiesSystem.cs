using Content.Server.Exodus.MassSpawn;
using Content.Server.Exodus.MassSpawn.Methods;
using Content.Shared.Actions;
using Content.Shared.Exodus.Abilities;
using Content.Shared.Exodus.Abilities.Events;
using Robust.Shared.Map;
using System.Numerics;

namespace Content.Server.Exodus.Abilities;

public sealed partial class AbilitiesSystem : SharedAbilitiesSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AbilitiesComponent, WorldLineSpawnEvent>(OnWorldLineSpawnEvent);
    }

    private void OnWorldLineSpawnEvent(EntityUid uid, AbilitiesComponent component, WorldLineSpawnEvent args)
    {
        var perfXform = Transform(args.Performer);

        var worldTargetPoint = _transform.ToMapCoordinates(args.Target).Position;
        var worldBeginPoint = _transform.GetWorldPosition(perfXform);

        var gridRel = perfXform.GridUid;

        var direction = Vector2.Zero;

        if (gridRel == null)
            direction = worldTargetPoint - worldBeginPoint;
        else
        {
            var relXform = Transform(gridRel.Value);
            direction = Vector2.Transform(worldTargetPoint, relXform.InvLocalMatrix) -
                        Vector2.Transform(worldBeginPoint, relXform.InvLocalMatrix);
        }


        var ent = SpawnAtPosition(null, perfXform.Coordinates);
        var massSpawn = AddComp<MassSpawnerComponent>(ent);
        massSpawn.SpawnProtoID = args.SpawnProto;
        massSpawn.AddGroup(new MassSpawnGroup()
        {
            Method = new AttackCellMethodLine()
            {
                InitGroup = new MassSpawnGroup() { GroupProto = "Cell" },
                StartCell = new Vector2(0, 0),
                FinishCell = direction * args.LenMultiply,
                StepDelay = args.StepDelay,
                Step = args.Step
            }
        });
    }


}
