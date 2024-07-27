using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;

namespace Content.Server.Exodus.PlaySound;

[RegisterComponent]
public sealed partial class PlaySoundOnSpawnComponent : Component
{
    [DataField]
    public SoundSpecifier? SpawnSound = null;
}
