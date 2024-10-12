using Content.Shared.Exodus.TeleportationZone;
using JetBrains.Annotations;
using Robust.Client.GameObjects;

namespace Content.Client.Exodus.TeleportationZone.UI.Beacon;

[UsedImplicitly]
public sealed class TeleportationZoneBeaconBoundUi : BoundUserInterface
{
    [ViewVariables]
    private TeleportationZoneBeaconWindow? _window;

    public TeleportationZoneBeaconBoundUi(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = new TeleportationZoneBeaconWindow();
        _window.OpenCentered();
        _window.BeaconSaveUINCoreButtonPressed += OnSaveUINCoreButtonPressed;
        _window.OnClose += Close;
    }

    private void OnSaveUINCoreButtonPressed()
    {
        SendMessage(new TeleportationZoneBeaconSaveUINCoreMessage());
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (_window == null || state is not TeleportationZoneBeaconUiState cast)
            return;

        _window.UpdateState(cast);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _window?.Dispose();
    }
}
