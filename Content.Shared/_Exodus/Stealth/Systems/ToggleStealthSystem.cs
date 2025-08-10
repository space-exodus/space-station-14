// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Stealth.Components;
using Content.Shared.Item.ItemToggle.Components;

namespace Content.Shared.Exodus.Stealth;

public sealed class ToggleStealthSystem : EntitySystem
{
    [Dependency] private readonly SharedStealthSystem _stealthSystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ToggleStealthComponent, ItemToggledEvent>(OnToggle);
    }

    private void OnToggle(EntityUid uid, ToggleStealthComponent comp, ItemToggledEvent args)
    {
        var target = comp.Parent ? Transform(uid).ParentUid : uid;

        comp.Enabled = args.Activated;

        if (TerminatingOrDeleted(target))
            return;

        if (comp.Enabled)
        {
            if (!_stealthSystem.RequestStealth(target, uid, comp.Stealth))
                return;

            comp.Target = target;
        }
        else
        {
            if (!comp.Target.IsValid())
                return;

            if (!_stealthSystem.RemoveRequest(uid, comp.Target))
                return;
        }
    }
}
