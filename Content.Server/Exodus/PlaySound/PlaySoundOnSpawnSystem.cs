using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;

namespace Content.Server.Exodus.PlaySound;

public sealed partial class PlaySoundOnSpawnSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audioSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PlaySoundOnSpawnComponent, MapInitEvent>(OnEntitySpawn);
    }

    private void OnEntitySpawn(EntityUid uid, PlaySoundOnSpawnComponent component, MapInitEvent args)
    {
        _audioSystem.PlayPvs(component.SpawnSound, uid);
    }
}
