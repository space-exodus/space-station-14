// Exodus-GhostRolesProfilesEditor-EntireFile
using Robust.Shared.Prototypes;

namespace Content.Server.Ghost.Roles.Components;

[RegisterComponent]
[Access(typeof(GhostRoleSystem))]
public sealed partial class GhostRoleHumanoidEditorComponent : Component
{
    /// <summary>
    ///     Extra components to add to this entity.
    /// </summary>
    [DataField("components")]
    public ComponentRegistry? Components { get; private set; }
}
