using Robust.Shared.Audio;

namespace Content.Server.Exodus.Lavaland.Altar;

[RegisterComponent]
public sealed partial class BossAltarComponent : Component
{
    [DataField]
    public bool DespawnAfterActivate = true;

    [DataField]
    public string BossSpawnProto = string.Empty;

    [DataField]
    public SoundSpecifier? ActivateSound = null;
}
