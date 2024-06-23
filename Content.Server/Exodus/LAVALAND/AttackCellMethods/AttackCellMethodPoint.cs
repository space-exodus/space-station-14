using System.Numerics;

namespace Content.Server.Exodus.MassSpawn.Methods;

[DataDefinition]
public sealed partial class AttackCellMethodPoint : AttackCellMethod
{
    public override void Start()
    {
        base.Start();

        if (OwnerGroup is null || _entityManager is null)
        {
            return;
        }

        _entityManager!.SpawnEntity(OwnerGroup!.SpawnProtoID, HeartPositionCoords);
    }

    public override AttackCellMethodPoint Copy()
    {
        return new();
    }
}
