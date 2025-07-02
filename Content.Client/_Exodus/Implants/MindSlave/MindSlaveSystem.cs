using Content.Shared.StatusIcon.Components;
using Content.Shared.Exodus.Implants.MindSlave.Components;
using Content.Shared.StatusIcon;

namespace Content.Client.Exodus.Implants.MindSlave;

public sealed class MindSlaveSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MindSlaveComponent, GetStatusIconsEvent>(GetMindSlaveIcon);
        SubscribeLocalEvent<MindSlaveMasterComponent, GetStatusIconsEvent>(GetMindSlaveMasterIcon);
    }

    public void GetMindSlaveIcon(Entity<MindSlaveComponent> ent, ref GetStatusIconsEvent args)
    {
        if (!HasComp<MindSlaveComponent>(ent.Owner))
            return;

        if (!TryGetEntity(ent.Comp.Master, out var masterEntity) || !TryComp<MindSlaveMasterComponent>(masterEntity, out var masterComp))
        {
            return;
        }

        var icon = new StatusIconData
        {
            Icon = ent.Comp.Icon,
            ShowToNetEntities = masterComp.IconList
        };

        args.StatusIcons.Add(icon);
    }

    public void GetMindSlaveMasterIcon(Entity<MindSlaveMasterComponent> ent, ref GetStatusIconsEvent args)
    {
        if (!HasComp<MindSlaveMasterComponent>(ent.Owner))
            return;

        var icon = new StatusIconData
        {
            Icon = ent.Comp.Icon,
            ShowToNetEntities = ent.Comp.IconList
        };

        args.StatusIcons.Add(icon);
    }
}
