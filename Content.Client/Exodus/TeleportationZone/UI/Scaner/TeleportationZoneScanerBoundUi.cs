using Content.Shared.Exodus.TeleportationZone;
using JetBrains.Annotations;
using Robust.Client.GameObjects;

namespace Content.Client.Exodus.TeleportationZone.UI.Scaner;

[UsedImplicitly]
public sealed class TeleportationZoneScanerBoundUi : BoundUserInterface
{
    [ViewVariables]
    private TeleportationZoneScanerWindow? _window;

    public TeleportationZoneScanerBoundUi(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = new TeleportationZoneScanerWindow();
        _window.OpenCentered();
        _window.TeleportationZoneScanerRefreshButtonPressed += OnTeleportationZoneScanerRefreshButtonPressed;
        _window.OnClose += Close;
    }

    private void OnTeleportationZoneScanerRefreshButtonPressed()
    {
        SendMessage(new TeleportationZoneScanerRefreshButtonPressedMessage());
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (_window == null || state is not TeleportationZoneScanerUiState cast)
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
