using Content.Server.Station.Components;
using Content.Server.GameTicking.Events;
using Content.Shared.Tag;

namespace Content.Server.Shuttles.Components;

/// <summary>
/// Add base station FTL Tag "Station"
/// </summary>
public sealed partial class StationFTLKeysSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StationFTLKeysComponent, ComponentInit>(OnComponentInit);
    }

    private void OnComponentInit(EntityUid uid, StationFTLKeysComponent comp, ComponentInit args)
    {

    }
}
