using System.Numerics;
using Robust.Shared.Utility;

namespace Content.Server.Exodus.Lavaland;

[Virtual]
[DataDefinition]
public partial class AttackCellMethodLine : AttackCellMethod
{
    [DataField("start")]
    public Vector2 StartCell = Vector2.Zero;

    [DataField("finish")]
    public Vector2 FinishCell = Vector2.Zero;

    [DataField("step")]
    public int Step = 1;

    [DataField("stepDelay")]
    public float StepDelay = 0.0f;

    [DataField("initGroup")]
    public MassSpawnGroup? InitGroup = null;

    private GridLineEnumerator _line = new();

    private TimeSpan _lastUpd = TimeSpan.Zero;

    private int _cellsNumber = 0;


    public override void Start()
    {
        base.Start();

        _line = new((Vector2i) StartCell, (Vector2i) FinishCell);
    }

    public override void Update(TimeSpan curTime)
    {

        while (LifeStage == AttackCeilMethodStage.Active &&
               curTime >= _lastUpd + TimeSpan.FromSeconds(StepDelay))
        {
            _lastUpd = curTime;

            if (_cellsNumber % Step != 0)
                continue;

            if (!_line.MoveNext())
            {
                LifeStage = AttackCeilMethodStage.Dead;
                return;
            }

            AddCell((Vector2) _line.Current);
                
            _cellsNumber++;
        }
    }

    private void AddCell(Vector2 coords)
    {
        if (InitGroup is null || OwnerGroup is null)
            return;

        var newInitGroup = new MassSpawnGroup();
        newInitGroup.CopyFrom(InitGroup);
        newInitGroup.Offset += coords + OwnerGroup!.Offset;
        OwnerComp?.AddGroup(newInitGroup);
    }

    public override AttackCellMethodLine Copy()
    {
        AttackCellMethodLine newMethod = new();

        newMethod.StartCell = StartCell;
        newMethod.FinishCell = FinishCell;
        newMethod.Step = Step;
        newMethod.StepDelay = StepDelay;

        if (InitGroup is null)
            return newMethod;

        var newInitGroup = new MassSpawnGroup();
        newInitGroup.CopyFrom(InitGroup);
        newMethod.InitGroup = newInitGroup;

        return newMethod;
    }


}
