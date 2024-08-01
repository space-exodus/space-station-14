using Content.Server.Exodus.TeleportationZone;
using Content.Shared.Coordinates;
using Content.Shared.Exodus.TeleportationZone;
using Robust.Server.GameObjects;
using System.Linq;

namespace Content.Server.Exodus.TeleportationZone.Scaner
{
    public sealed class TeleportationZoneScanerSystem : EntitySystem
    {
        [Dependency] private readonly UserInterfaceSystem _userInterface = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<TeleportationZoneScanerComponent, BoundUIOpenedEvent>(OnBoundUIOpened);
            SubscribeLocalEvent<TeleportationZoneScanerComponent, TeleportationZoneScanerRefreshButtonPressedMessage>(OnRefreshButtonPressed);
        }

        private void OnBoundUIOpened(EntityUid uid, TeleportationZoneScanerComponent component, BoundUIOpenedEvent args)
        {
            UpdateUI(uid, component);
        }

        private void OnRefreshButtonPressed(EntityUid uid, TeleportationZoneScanerComponent component, TeleportationZoneScanerRefreshButtonPressedMessage message)
        {
            UpdateUI(uid, component);
        }

        private void UpdateUI(EntityUid uid, TeleportationZoneScanerComponent component)
        {
            component.Name = "no data available";
            component.X = 0f;
            component.Y = 0f;
            component.Status = "no data available";
            component.Color = "white";

            var query = AllEntityQuery<TeleportationZoneAstroDungeonComponent>();
            while (query.MoveNext(out var grid_uid, out var grid_comp))
            {
                component.Name = grid_comp.Name;
                component.X = grid_comp.X;
                component.Y = grid_comp.Y;
                component.Status = grid_comp.Status;
                component.Color = grid_comp.Color;
            }
            var state = new TeleportationZoneScanerUiState(component.Name, component.X, component.Y, component.Status, component.Color);
            _userInterface.SetUiState(uid, TeleportationZoneScanerUiKey.Key, state);
        }
    }
}
