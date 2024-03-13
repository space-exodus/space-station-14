using Content.Server.Shuttles.Systems;
using Content.Shared.Shuttles.Components;

namespace Content.Server.Exodus.FTLKey;

public class FTLMarkerSystem : EntitySystem
{
    [Dependency] private readonly ShuttleSystem _shuttle = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FTLMarkerComponent, ComponentInit>(OnComponentInit);
    }

    private void OnComponentInit(EntityUid uid, FTLMarkerComponent component, ComponentInit args)
    {
        var xform = Transform(uid);

        if (_shuttle.TryAddFTLDestination(xform.MapID, component.Enabled, out var ftlComp))
        {
            _shuttle.SetFTLWhitelist((xform.MapUid!.Value, ftlComp), component.Whitelist);
        }
    }
}
