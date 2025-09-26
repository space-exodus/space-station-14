// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt
using Content.Shared.Exodus.Materials.Components;
using Content.Shared.Interaction;
using Content.Shared.Materials;
using Content.Shared.Tag;
using Content.Shared.Whitelist;

namespace Content.Server.Exodus.Material;

public sealed partial class MaterialClusterSystem : EntitySystem
{
    [Dependency] private readonly SharedMaterialStorageSystem _materialStorageSystem = default!;
    [Dependency] private readonly EntityWhitelistSystem _entityWhitelist = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MaterialClusterComponent, AfterInteractEvent>(OnInteract);
    }

    private void OnInteract(EntityUid uid, MaterialClusterComponent comp, AfterInteractEvent args)
    {
        if (args.Handled)
            return;

        if (args.Target == null)
            return;

        if (!TryComp<MaterialStorageComponent>(args.Target, out var materialStorageComp))
            return;

        foreach (var material in comp.Materials.Keys)
        {
            if (!_materialStorageSystem.IsMaterialWhitelisted(args.Target.Value, material))
                return;
        }

        if (!_materialStorageSystem.TryChangeMaterialAmount(args.Target.Value, comp.Materials))
            return;

        args.Handled = true;

        QueueDel(uid);
    }
}
