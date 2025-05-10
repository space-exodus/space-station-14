using Content.Server.Engineering.Components;
using Content.Server.Exodus.WallPoster.Components;
using Content.Shared.Engineering.Systems;

namespace Content.Server.Exodus.WallPoster.Systems;
public sealed class WallPosterSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WallPosterComponent, DisassembleEntityEvent>(OnItemDisassembled);
    }

    private void OnItemDisassembled(EntityUid uid, WallPosterComponent comp, DisassembleEntityEvent args)
    {
        var spawnComp = EnsureComp<SpawnAfterInteractComponent>(args.CreatedEntity);
        spawnComp.Prototype = args.DisassembledEntProto;
    }
}
