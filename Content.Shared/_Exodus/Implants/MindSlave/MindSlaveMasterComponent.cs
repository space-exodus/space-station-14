using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared.Exodus.Implants.MindSlave.Components;


[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MindSlaveMasterComponent : Component
{
    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public HashSet<NetEntity> IconList = new HashSet<NetEntity>();

    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public SpriteSpecifier Icon { get; set; } = new SpriteSpecifier.Rsi(new ResPath("/Textures/Exodus/Interface/Misc/JobIcons/mindslave.rsi"), "subordinating");
}
