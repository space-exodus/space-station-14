// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Flash;

namespace Content.Shared.Exodus.Traits.Species.SensitiveEyes;

public sealed partial class SensitiveEyesSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SensitiveEyesComponent, FlashModifiersEvent>(OnFlashModifiersEvent);
    }

    private void OnFlashModifiersEvent(Entity<SensitiveEyesComponent> entity, ref FlashModifiersEvent args)
    {
        args.ModifyDuration(entity.Comp.DurationModifier);
    }
}
