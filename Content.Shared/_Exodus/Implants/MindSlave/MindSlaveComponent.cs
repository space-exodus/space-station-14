using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared.Exodus.Implants.MindSlave.Components;


[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MindSlaveComponent : Component
{
    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public EntityUid Master;

    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public SpriteSpecifier Icon { get; set; } = new SpriteSpecifier.Rsi(new ResPath("/Textures/Exodus/Interface/Misc/JobIcons/mindslave.rsi"), "subordinate");
}
