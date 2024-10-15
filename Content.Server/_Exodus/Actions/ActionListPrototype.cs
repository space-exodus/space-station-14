using Robust.Shared.Prototypes;

namespace Content.Server.Exodus.Actions;

[Prototype("actionList")]
public sealed partial class ActionListPrototype : ActionList, IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;
}

[Virtual]
[DataDefinition]
public partial class ActionList
{
    public bool AllAvaibleAbilities = false;
    public List<string> Actions = [];
}
