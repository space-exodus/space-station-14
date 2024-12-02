using Content.Shared.Exodus.TeleportationZone;
using JetBrains.Annotations;
using Robust.Client.GameObjects;

namespace Content.Client.Exodus.TeleportationZone.UI.Battery;

[UsedImplicitly]
public sealed class TeleportationZoneBatteryBoundUi : BoundUserInterface
{
    [ViewVariables]
    private TeleportationZoneBatteryWindow? _window;

    public TeleportationZoneBatteryBoundUi(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = new TeleportationZoneBatteryWindow();
        _window.OpenCentered();
        _window.BatteryDownloadButtonPressed += OnBatteryDownloadButtonPressed;
        _window.BatteryUploadButtonPressed += OnBatteryUploadButtonPressed;
        _window.OnClose += Close;
    }

    private void OnBatteryDownloadButtonPressed()
    {
        SendMessage(new TeleportationZoneBatteryDownloadMatterMessage());
    }

    private void OnBatteryUploadButtonPressed()
    {
        SendMessage(new TeleportationZoneBatteryUnloadMatterMessage());
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (_window == null || state is not TeleportationZoneBatteryUiState cast)
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
