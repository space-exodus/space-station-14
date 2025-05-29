using Content.Shared.StatusIcon;
using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;

namespace Content.Shared.Exodus.Implants.MindSlave.Components;


[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MindSlaveComponent : Component
{
    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public NetEntity Master;
}
