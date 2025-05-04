using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;
using Content.Shared.Exodus.Implants.MindSlave.Components;

namespace Content.Client.Exodus.Implants.MindSlave;

public sealed class MindSlaveSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MindSlaveComponent, GetStatusIconsEvent>(GetMindSlaveIcon);
    }
    public void GetMindSlaveIcon(Entity<MindSlaveComponent> ent, ref GetStatusIconsEvent args)
    {
        if (!HasComp<MindSlaveComponent>(ent))
            return;

        if (_prototype.TryIndex(ent.Comp.StatusIcon, out var iconPrototype))
            args.StatusIcons.Add(iconPrototype);
    }
}
