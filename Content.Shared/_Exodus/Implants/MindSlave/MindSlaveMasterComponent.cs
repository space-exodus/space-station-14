using Content.Shared.StatusIcon;
using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;
using Content.Shared.Whitelist;

namespace Content.Shared.Exodus.Implants.MindSlave.Components;


[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MindSlaveMasterComponent : Component
{
    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public HashSet<NetEntity> IconList = new HashSet<NetEntity>();
}
