using Content.Shared.Exodus.TeleportationZone;
using Robust.Server.GameObjects;

namespace Content.Server.Exodus.TeleportationZone.Repeater
{
    public sealed class TeleportationZoneRepeaterSystem : EntitySystem
    {
        [Dependency] private readonly UserInterfaceSystem _userInterface = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<TeleportationZoneRepeaterComponent, BoundUIOpenedEvent>(OnBoundUIOpened);
            SubscribeLocalEvent<TeleportationZoneRepeaterComponent, TeleportationZoneRepeaterChangedMessage>(OnNameChanged);
        }

        private void OnBoundUIOpened(EntityUid uid, TeleportationZoneRepeaterComponent component, BoundUIOpenedEvent args)
        {
            UpdateUI(uid, component);
        }

        private void UpdateUI(EntityUid uid, TeleportationZoneRepeaterComponent component)
        {
            var state = new TeleportationZoneRepeaterUiState(component.Name);
            _userInterface.SetUiState(uid, TeleportationZoneRepeaterUiKey.Key, state);
        }

        private void OnNameChanged(EntityUid uid, TeleportationZoneRepeaterComponent component, TeleportationZoneRepeaterChangedMessage message)
        {
            component.Name = message.Name;
            UpdateUI(uid, component);
        }
    }
}
