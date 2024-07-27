using System.Linq;
using System.Numerics;
using Robust.Shared.Utility;

namespace Content.Server.Exodus.MassSpawn.Methods;

[Virtual]
[DataDefinition]
public partial class AttackCellMethodRect : AttackCellMethod
{
    [DataField("vertexs")]
    public List<Vector2> Vertexs = [
        new Vector2(-1, -1),
        new Vector2(1, 1)
    ];

    [DataField("initGroup")]
    public MassSpawnGroup? InitGroup = null;

    private float _top = 1;
    private float _bottom = -1;
    private float _left = -1;
    private float _right = 1;

    public override void Start()
    {
        base.Start();

        _top = _bottom = Vertexs[0].Y;
        _left = _right = Vertexs[0].X;

        foreach (var item in Vertexs)
        {
            _right = Math.Max(_right, item.X);
            _left = Math.Min(_left, item.X);
            _top = Math.Max(_top, item.Y);
            _bottom = Math.Min(_bottom, item.Y);
        }

        for (var i = _bottom; i <= _top; i++)
            AddLine(new Vector2(_left, i),
                    new Vector2(_right, i));
    }

    private void AddLine(Vector2 start, Vector2 finish)
    {
        if (InitGroup is null || OwnerGroup is null)
            return;

        var newInitGroup = new MassSpawnGroup();

        var method = new AttackCellMethodLine();
        method.InitGroup = new();
        method.InitGroup.CopyFrom(InitGroup);
        method.StartCell = start + OwnerGroup!.Offset;
        method.FinishCell = finish + OwnerGroup!.Offset;
        newInitGroup.Method = method;

        OwnerComp?.AddGroup(newInitGroup);
    }


    public override AttackCellMethodRect Copy()
    {
        AttackCellMethodRect newMethod = new();

        newMethod.Vertexs = Vertexs;

        if (InitGroup is null)
            return newMethod;

        var newInitGroup = new MassSpawnGroup();
        newInitGroup.CopyFrom(InitGroup);
        newMethod.InitGroup = newInitGroup;

        return newMethod;
    }
}
