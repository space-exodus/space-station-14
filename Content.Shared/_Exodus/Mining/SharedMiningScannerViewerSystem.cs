// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Mining.Components;
using Robust.Shared.GameStates;

namespace Content.Shared.Exodus.Mining;

public abstract partial class SharedMiningScannerViewerSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MiningScannerViewerComponent, ComponentGetState>(GetState);
        SubscribeLocalEvent<MiningScannerViewerComponent, ComponentHandleState>(HandleState);
    }

    private void GetState(EntityUid uid, MiningScannerViewerComponent comp, ref ComponentGetState args)
    {
        args.State = new MiningScannerViewerComponentState()
        {
            Records = comp.Records,
        };
    }

    private void HandleState(EntityUid uid, MiningScannerViewerComponent comp, ref ComponentHandleState args)
    {
        if (args.Current is not MiningScannerViewerComponentState state)
            return;

        comp.Records = state.Records;
    }
}
