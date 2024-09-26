using Content.Shared.Exodus.TeleportationZone;
using Robust.Server.GameObjects;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;

namespace Content.Server.Exodus.TeleportationZone.Battery
{
    public sealed class TeleportationZoneBatterySystem : EntitySystem
    {
        [Dependency] private readonly UserInterfaceSystem _userInterface = default!;
        [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<TeleportationZoneBatteryComponent, ComponentInit>(OnComponentInit);
            SubscribeLocalEvent<TeleportationZoneBatteryComponent, BoundUIOpenedEvent>(OnBoundUIOpened);
            SubscribeLocalEvent<TeleportationZoneBatteryComponent, EntInsertedIntoContainerMessage>(OnItemInserted);
            SubscribeLocalEvent<TeleportationZoneBatteryComponent, EntRemovedFromContainerMessage>(OnItemRemoved);
            SubscribeLocalEvent<TeleportationZoneBatteryComponent, TeleportationZoneBatteryDownloadMatterMessage>(OnDownload);
            SubscribeLocalEvent<TeleportationZoneBatteryComponent, TeleportationZoneBatteryUnloadMatterMessage>(OnUnload);
        }

        private void OnComponentInit(EntityUid uid, TeleportationZoneBatteryComponent component, ComponentInit args)
        {
            _itemSlotsSystem.AddItemSlot(uid, "SmallBattery", component.SmallBattery);
        }

        private void OnBoundUIOpened(EntityUid uid, TeleportationZoneBatteryComponent component, BoundUIOpenedEvent args)
        {
            if (component.SmallBattery.ContainerSlot!.ContainedEntity == null)
            {
                component.CurrentTexturePathForBattery = CheckDesiredPath(component.Matter, component.Capacity, component.VariantsTexturePath, component.CurrentTexturePathForBattery);
                var state = new TeleportationZoneBatteryUiState(false, false, component.CurrentTexturePathForBattery, component.CurrentTexturePathForSmallBattery, component.Matter, 0);
                _userInterface.SetUiState(uid, TeleportationZoneBatteryUiKey.Key, state);
            }
            else
            {
                if (!TryComp<TeleportationZoneSmallBatteryComponent>(component.SmallBattery.ContainerSlot!.ContainedEntity, out var smallBatteryComp))
                    return;

                component.CurrentTexturePathForBattery = CheckDesiredPath(component.Matter, component.Capacity, component.VariantsTexturePath, component.CurrentTexturePathForBattery);
                component.CurrentTexturePathForSmallBattery = CheckDesiredPath(smallBatteryComp.Matter, smallBatteryComp.Capacity, component.VariantsTexturePath, component.CurrentTexturePathForSmallBattery);
                var state = new TeleportationZoneBatteryUiState(true, true, component.CurrentTexturePathForBattery, component.CurrentTexturePathForSmallBattery, component.Matter, smallBatteryComp.Matter);
                _userInterface.SetUiState(uid, TeleportationZoneBatteryUiKey.Key, state);
            }
        }

        private void OnItemInserted(EntityUid uid, TeleportationZoneBatteryComponent component, EntInsertedIntoContainerMessage args)
        {
            if (component.SmallBattery.ContainerSlot!.ContainedEntity == null)
                return;

            if (!TryComp<TeleportationZoneSmallBatteryComponent>(component.SmallBattery.ContainerSlot!.ContainedEntity, out var smallBatteryComp))
                return;

            component.CurrentTexturePathForBattery = CheckDesiredPath(component.Matter, component.Capacity, component.VariantsTexturePath, component.CurrentTexturePathForBattery);
            component.CurrentTexturePathForSmallBattery = CheckDesiredPath(smallBatteryComp.Matter, smallBatteryComp.Capacity, component.VariantsTexturePath, component.CurrentTexturePathForSmallBattery);
            var state = new TeleportationZoneBatteryUiState(true, true, component.CurrentTexturePathForBattery, component.CurrentTexturePathForSmallBattery, component.Matter, smallBatteryComp.Matter);
            _userInterface.SetUiState(uid, TeleportationZoneBatteryUiKey.Key, state);
        }

        private void OnItemRemoved(EntityUid uid, TeleportationZoneBatteryComponent component, EntRemovedFromContainerMessage args)
        {
            component.CurrentTexturePathForSmallBattery = component.VariantsTexturePath[0];
            component.CurrentTexturePathForBattery = CheckDesiredPath(component.Matter, component.Capacity, component.VariantsTexturePath, component.CurrentTexturePathForBattery);
            var state = new TeleportationZoneBatteryUiState(false, false, component.CurrentTexturePathForBattery, component.CurrentTexturePathForSmallBattery, component.Matter, 0);
            _userInterface.SetUiState(uid, TeleportationZoneBatteryUiKey.Key, state);
        }

        private void OnDownload(EntityUid uid, TeleportationZoneBatteryComponent component, TeleportationZoneBatteryDownloadMatterMessage mes)
        {
            var dCapacity = component.Capacity - component.Matter;

            if (!TryComp<TeleportationZoneSmallBatteryComponent>(component.SmallBattery.ContainerSlot!.ContainedEntity, out var smallBatteryComp))
                return;

            if (smallBatteryComp.Matter <= dCapacity)
            {
                component.Matter += smallBatteryComp.Matter;
                smallBatteryComp.Matter = 0;
            }
            else
            {
                component.Matter += dCapacity;
                smallBatteryComp.Matter -= dCapacity;
            }

            component.CurrentTexturePathForBattery = CheckDesiredPath(component.Matter, component.Capacity, component.VariantsTexturePath, component.CurrentTexturePathForBattery);
            component.CurrentTexturePathForSmallBattery = CheckDesiredPath(smallBatteryComp.Matter, smallBatteryComp.Capacity, component.VariantsTexturePath, component.CurrentTexturePathForSmallBattery);
            var state = new TeleportationZoneBatteryUiState(true, true, component.CurrentTexturePathForBattery, component.CurrentTexturePathForSmallBattery, component.Matter, smallBatteryComp.Matter);
            _userInterface.SetUiState(uid, TeleportationZoneBatteryUiKey.Key, state);
        }

        private void OnUnload(EntityUid uid, TeleportationZoneBatteryComponent component, TeleportationZoneBatteryUnloadMatterMessage mes)
        {
            if (!TryComp<TeleportationZoneSmallBatteryComponent>(component.SmallBattery.ContainerSlot!.ContainedEntity, out var smallBatteryComp))
                return;

            var dCapacity = smallBatteryComp.Capacity - smallBatteryComp.Matter;

            if (component.Matter <= dCapacity)
            {
                smallBatteryComp.Matter += component.Matter;
                component.Matter = 0;
            }
            else
            {
                smallBatteryComp.Matter += dCapacity;
                component.Matter -= dCapacity;
            }

            component.CurrentTexturePathForBattery = CheckDesiredPath(component.Matter, component.Capacity, component.VariantsTexturePath, component.CurrentTexturePathForBattery);
            component.CurrentTexturePathForSmallBattery = CheckDesiredPath(smallBatteryComp.Matter, smallBatteryComp.Capacity, component.VariantsTexturePath, component.CurrentTexturePathForSmallBattery);
            var state = new TeleportationZoneBatteryUiState(true, true, component.CurrentTexturePathForBattery, component.CurrentTexturePathForSmallBattery, component.Matter, smallBatteryComp.Matter);
            _userInterface.SetUiState(uid, TeleportationZoneBatteryUiKey.Key, state);
        }

        private string CheckDesiredPath(int currentAmount, int capacity, Dictionary<int, string> VariantsTexturePath, string CurrentTexturePath)
        {
            if (VariantsTexturePath.Count == 0)
                return CurrentTexturePath;

            float magic_number = (((float) currentAmount / (float) capacity) * 100.0f) / (100.0f / (float) (VariantsTexturePath.Count - 2));

            if (magic_number == 0)
                return CurrentTexturePath = VariantsTexturePath[0];

            for (int i = 0; i <= magic_number; i++)
            {
                CurrentTexturePath = VariantsTexturePath[i + 1];
            }

            return CurrentTexturePath;
        }

        public bool CheckCorrectPlacement(EntityUid console, EntityUid battery)
        {
            var consoleComp = Transform(console);
            var batteryComp = Transform(battery);

            var dX = consoleComp.Coordinates.X - batteryComp.Coordinates.X;
            var dY = consoleComp.Coordinates.Y - batteryComp.Coordinates.Y;

            if (consoleComp.LocalRotation.Degrees % 360 == 0)
            {
                if (dX == 1 & dY == 0)
                    return true;

                return false;
            }

            if (consoleComp.LocalRotation.Degrees % 360 == 90 || consoleComp.LocalRotation.Degrees % 360 == -90)
            {
                if (dX == 0 & dY == 1)
                    return true;

                return false;
            }

            if (consoleComp.LocalRotation.Degrees % 360 == 180 || consoleComp.LocalRotation.Degrees % 360 == -180)
            {
                if (dX == -1 & dY == 0)
                    return true;

                return false;
            }

            if (consoleComp.LocalRotation.Degrees % 360 == 270 || consoleComp.LocalRotation.Degrees % 360 == -270)
            {
                if (dX == 0 & dY == -1)
                    return true;

                return false;
            }

            return false;
        }
    }
}
