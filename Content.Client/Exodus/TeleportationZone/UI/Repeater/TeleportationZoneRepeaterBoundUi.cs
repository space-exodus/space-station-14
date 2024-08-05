using Content.Shared.Exodus.TeleportationZone;
using JetBrains.Annotations;
using Robust.Client.GameObjects;

namespace Content.Client.Exodus.TeleportationZone.UI.Repeater;

[UsedImplicitly]
public sealed class TeleportationZoneRepeaterBoundUi : BoundUserInterface
{
    [ViewVariables]
    private TeleportationZoneRepeaterWindow? _window;

    public TeleportationZoneRepeaterBoundUi(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = new TeleportationZoneRepeaterWindow();
        _window.OpenCentered();
        _window.NameChanged += OnNameChanged;
        _window.OnClose += Close;
    }

    private void OnNameChanged(string name)
    {
        SendMessage(new TeleportationZoneRepeaterChangedMessage(name));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (_window == null || state is not TeleportationZoneRepeaterUiState cast)
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
