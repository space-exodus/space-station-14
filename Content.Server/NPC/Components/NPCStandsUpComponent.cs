using Content.Shared.DoAfter;

namespace Content.Server.NPC.Components;

/// <summary>
/// Added to NPC when it's trying to get up (while do after is active)
/// </summary>
[RegisterComponent]
public sealed partial class NPCStandsUpComponent : Component
{
    [DataField]
    public DoAfterId DoAfter;
}
