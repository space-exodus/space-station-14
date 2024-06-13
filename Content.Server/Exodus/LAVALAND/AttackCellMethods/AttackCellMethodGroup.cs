using System.Numerics;

namespace Content.Server.Exodus.Lavaland;

[Virtual]
[DataDefinition]
public partial class AttackCellMethodGroup : AttackCellMethod
{
    [DataField("initGroups")]
    public List<MassSpawnGroup> InitGroups = [];

    public override void Start()
    {
        base.Start();

        OwnerComp?.AddGroup(InitGroups);
    }

    public override AttackCellMethodGroup Copy()
    {
        AttackCellMethodGroup newMethod = new();

        foreach (var initGroup in InitGroups)
        {
            var newInitGroup = new MassSpawnGroup();
            newInitGroup.CopyFrom(initGroup);
            newMethod.InitGroups.Add(newInitGroup);
        }
        return newMethod;
    }

    public AttackCellMethodGroup()
    {
    }
}


