// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
using Content.Server.Exodus.RandomTeleport;
using Content.Shared.EntityEffects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using JetBrains.Annotations;


namespace Content.Server.Exodus.EntityEffects.Effects;

[UsedImplicitly]
[DataDefinition]
public sealed partial class RandomTeleport : EntityEffect
{
    [DataField]
    public float Range = 10f;

    [DataField]
    public bool SpaceAllowed = true;

    [DataField]
    public SoundSpecifier TeleportSound = new SoundPathSpecifier("/Audio/Effects/teleport_arrival.ogg");

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    => Loc.GetString("reagent-effect-guidebook-random-teleport",
        ("chance", Probability));

    public override void Effect(EntityEffectBaseArgs args)
    {
        var transformSys = args.EntityManager.System<SharedTransformSystem>();
        var audioSys = args.EntityManager.System<SharedAudioSystem>();
        var randTeleport = args.EntityManager.System<RandomTeleportSystem>();

        var targetCoordinates = randTeleport.GetRandomCoordinates(args.TargetEntity, Range, SpaceAllowed);

        if (targetCoordinates.HasValue)
        {
            transformSys.SetCoordinates(args.TargetEntity, targetCoordinates.Value);
            audioSys.PlayPvs(TeleportSound, args.TargetEntity);
        }

    }
}
