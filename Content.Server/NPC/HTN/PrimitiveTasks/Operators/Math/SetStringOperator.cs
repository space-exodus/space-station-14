using System.Threading;
using System.Threading.Tasks;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Math;

/// <summary>
/// Set <see cref="SetBoolOperator.Value"/> to bool value for the
/// specified <see cref="SetFloatOperator.TargetKey"/> in the <see cref="NPCBlackboard"/>.
/// </summary>
public sealed partial class SetStringOperator : HTNOperator
{
    [DataField(required: true), ViewVariables]
    public string TargetKey = string.Empty;

    [DataField(required: true), ViewVariables(VVAccess.ReadWrite)]
    public string Value = string.Empty;

    public override async Task<(bool Valid, Dictionary<string, object>? Effects)> Plan(NPCBlackboard blackboard,
        CancellationToken cancelToken)
    {
        return (
            true,
            new Dictionary<string, object>
            {
                { TargetKey, Value }
            }
        );
    }
}
