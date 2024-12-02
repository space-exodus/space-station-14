using Content.Shared.Exodus.TeleportationZone;
using JetBrains.Annotations;
using Robust.Client.GameObjects;

namespace Content.Client.Exodus.TeleportationZone.UI.Console;

[UsedImplicitly]
public sealed class TeleportationZoneConsoleBoundUi : BoundUserInterface
{
    [ViewVariables]
    private TeleportationZoneConsoleWindow? _window;

    public TeleportationZoneConsoleBoundUi(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = new TeleportationZoneConsoleWindow();
        _window.OpenCentered();

        // Updating lists
        _window.RefreshBluespaceZonesButtonPressed += OnRefreshBluespaceZonesButtonPressed;
        _window.RefreshArrivalObjectsButtonPressed += OnRefreshArrivalObjectsButtonPressed;
        // Increase/Reducing the amount of matter consumed
        _window.MinusMatterButtonPressed += OnMinusMatterButtonPressed;
        _window.PlusMatterButtonPressed += OnPlusMatterButtonPressed;
        // Start teleporting the object
        _window.StartTeleportingButtonPressed += OnStartTeleportingButtonPressed;
        // Selecting points
        _window.BluespaceZoneSelected += OnBluespaceZoneSelected;
        _window.ArrivalObjectSelected += OnArrivalObjectSelected;

        _window.CoordXChanged += OnCoordXChanged;
        _window.CoordYChanged += OnCoordYChanged;

        _window.OnClose += Close;
    }

    private void OnRefreshBluespaceZonesButtonPressed()
    {
        SendMessage(new TeleportationZoneConsoleRefreshBluespaceZonesMessage());
    }

    private void OnRefreshArrivalObjectsButtonPressed()
    {
        SendMessage(new TeleportationZoneConsoleRefreshArrivalObjectsMessage());
    }

    private void OnMinusMatterButtonPressed(int matter)
    {
        SendMessage(new TeleportationZoneConsoleMinusMatterMessage(matter));
    }

    private void OnPlusMatterButtonPressed(int matter)
    {
        SendMessage(new TeleportationZoneConsolePlusMatterMessage(matter));
    }

    private void OnStartTeleportingButtonPressed()
    {
        SendMessage(new TeleportationZoneConsoleStartTeleportingMessage());
    }

    private void OnBluespaceZoneSelected(string zone)
    {
        SendMessage(new TeleportationZoneConsoleBluespaceZoneSelectedMessage(zone));
    }

    private void OnArrivalObjectSelected(NetEntity _object)
    {
        SendMessage(new TeleportationZoneConsoleArrivalObjectSelectedMessage(_object));
    }

    private void OnCoordXChanged(float coordX)
    {
        SendMessage(new TeleportationZoneConsoleCoordXChangedMessage(coordX));
    }

    private void OnCoordYChanged(float coordY)
    {
        SendMessage(new TeleportationZoneConsoleCoordYChangedMessage(coordY));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (_window == null || state is not TeleportationZoneConsoleUiState cast)
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
