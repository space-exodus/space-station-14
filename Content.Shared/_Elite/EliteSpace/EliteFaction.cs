using Robust.Shared.Prototypes;

namespace Content.Shared.Elite.EliteSpace;

[Prototype]
public sealed partial class EliteFactionPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public LocId Name { get; private set; } = "elite-faction-unnamed";

    [DataField]
    public Color MapColor { get; private set; } = Color.White;
}
