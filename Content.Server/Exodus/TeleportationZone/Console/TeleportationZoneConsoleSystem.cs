using Content.Server.Exodus.TeleportationZone;
using Content.Server.Exodus.TeleportationZone.Battery;
using Content.Server.Exodus.TeleportationZone.Repeater;
using Content.Server.Popups;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Coordinates;
using Content.Shared.Exodus.TeleportationZone;
using Content.Shared.Maps;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using System.Linq;
using Robust.Shared.Timing;
using System;

namespace Content.Server.Exodus.TeleportationZone.Console
{
    public sealed class TeleportationZoneConsoleSystem : EntitySystem
    {
        [Dependency] private readonly IEntityManager _entMan = default!;
        [Dependency] private readonly SharedMapSystem _mapSystem = default!;
        [Dependency] private readonly StationSystem _station = default!;
        [Dependency] private readonly UserInterfaceSystem _userInterface = default!;
        [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
        [Dependency] private readonly EntityLookupSystem _lookup = default!;
        [Dependency] private readonly TeleportationZoneSystem _tpZone = default!;
        [Dependency] private readonly TeleportationZoneBatterySystem _tpBattery = default!;
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        [Dependency] private readonly IGameTiming _gameTiming = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<TeleportationZoneConsoleComponent, ComponentInit>(OnComponentInit);

            // UI
            SubscribeLocalEvent<TeleportationZoneConsoleComponent, BoundUIOpenedEvent>(OnBoundUIOpened);
            // Refresh lists
            SubscribeLocalEvent<TeleportationZoneConsoleComponent, TeleportationZoneConsoleRefreshBluespaceZonesMessage>(OnRefreshBluespaceZonesButtonPressed);
            SubscribeLocalEvent<TeleportationZoneConsoleComponent, TeleportationZoneConsoleRefreshArrivalObjectsMessage>(OnRefreshArrivalObjectsButtonPressed);
            // Increase/Reducing the amount of matter consumed
            SubscribeLocalEvent<TeleportationZoneConsoleComponent, TeleportationZoneConsoleMinusMatterMessage>(OnMinusMatterButtonPressed);
            SubscribeLocalEvent<TeleportationZoneConsoleComponent, TeleportationZoneConsolePlusMatterMessage>(OnPlusMatterButtonPressed);
            // Changing teleportation coordinates
            SubscribeLocalEvent<TeleportationZoneConsoleComponent, TeleportationZoneConsoleCoordXChangedMessage>(OnCoordXChanged);
            SubscribeLocalEvent<TeleportationZoneConsoleComponent, TeleportationZoneConsoleCoordYChangedMessage>(OnCoordYChanged);
            // Selecting points
            SubscribeLocalEvent<TeleportationZoneConsoleComponent, TeleportationZoneConsoleBluespaceZoneSelectedMessage>(OnZoneSelected);
            SubscribeLocalEvent<TeleportationZoneConsoleComponent, TeleportationZoneConsoleArrivalObjectSelectedMessage>(OnArrivalObjectSelected);
            // Start teleporting the object
            SubscribeLocalEvent<TeleportationZoneConsoleComponent, TeleportationZoneConsoleStartTeleportingMessage>(OnStartLandingButtonPressed);

        }

        private void OnComponentInit(EntityUid uid, TeleportationZoneConsoleComponent component, ComponentInit args)
        {
            foreach (var slot in component.Slots)
            {
                _itemSlotsSystem.AddItemSlot(uid, slot.Key, slot.Value);
            }
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            var query = EntityQueryEnumerator<TeleportationZoneConsoleComponent>();
            while (query.MoveNext(out var uid, out var component))
            {
                if (!component.TeleportationProc)
                    continue;

                if (_gameTiming.CurTime < component.NextUpdate)
                    continue;

                if (component.CurrentSecond > component.LastSecond)
                {
                    _popupSystem.PopupEntity(component.CurrentSecond.ToString(), uid);
                    component.CurrentSecond--;
                    component.NextUpdate = _gameTiming.CurTime + component.UpdateInterval;
                    continue;
                }

                if (component.CurrentSecond == component.LastSecond)
                {
                    _tpZone.CheckTils(
                        uid,
                        component,
                        component.Start_station_uid,
                        component.Start_grid_comp,
                        component.End_station_uid,
                        component.End_grid_comp,
                        component.Position);

                    component.CurrentSecond = 10;
                    component.TeleportationProc = false;
                }
            }
        }

        #region Pressing the buttons
        private void OnBoundUIOpened(EntityUid uid, TeleportationZoneConsoleComponent component, BoundUIOpenedEvent args)
        {
            UpdateUI(uid, component);
        }

        private void OnRefreshBluespaceZonesButtonPressed(EntityUid uid, TeleportationZoneConsoleComponent component, TeleportationZoneConsoleRefreshBluespaceZonesMessage message)
        {
            component.BluespaceZones = UpdateListBluespaceZones(component);
            component.ZoneSelected = false;
            UpdateUI(uid, component);
            CheckСonditions(uid, component);
        }

        private void OnRefreshArrivalObjectsButtonPressed(EntityUid uid, TeleportationZoneConsoleComponent component, TeleportationZoneConsoleRefreshArrivalObjectsMessage message)
        {
            component.ArrivalObjects = UpdateListArrivalObjects();
            component.ArrivalObjectSelected = false;
            UpdateUI(uid, component);
            CheckСonditions(uid, component);
        }

        private void OnMinusMatterButtonPressed(EntityUid uid, TeleportationZoneConsoleComponent component, TeleportationZoneConsoleMinusMatterMessage message)
        {
            if ((component.Matter - message.Matter) > 0)
            {
                component.Matter -= message.Matter;
                UpdateUI(uid, component);
            }
        }

        private void OnPlusMatterButtonPressed(EntityUid uid, TeleportationZoneConsoleComponent component, TeleportationZoneConsolePlusMatterMessage message)
        {
            component.Matter += message.Matter;
            UpdateUI(uid, component);
        }

        private void OnCoordXChanged(EntityUid uid, TeleportationZoneConsoleComponent component, TeleportationZoneConsoleCoordXChangedMessage message)
        {
            component.CoordX = message.CoordX;
            component.CoordXEntered = true;
            CheckСonditions(uid, component);
        }

        private void OnCoordYChanged(EntityUid uid, TeleportationZoneConsoleComponent component, TeleportationZoneConsoleCoordYChangedMessage message)
        {
            component.CoordY = message.CoordY;
            component.CoordYEntered = true;
            CheckСonditions(uid, component);
        }

        private void OnZoneSelected(EntityUid uid, TeleportationZoneConsoleComponent component, TeleportationZoneConsoleBluespaceZoneSelectedMessage message)
        {
            int cycle = 0;
            foreach (var slot in component.Slots)
            {
                if (slot.Value.ContainerSlot!.ContainedEntity == null)
                    continue;

                if (slot.Value.Name == message.Zone & cycle == 0)
                {
                    if (!TryComp<TeleportationZoneScanModuleComponent>(slot.Value.ContainerSlot!.ContainedEntity.Value, out var scanComp))
                        return;

                    if (!scanComp.DataRefreshed)
                        return;

                    component.FirstBeacon = scanComp.UIN;
                    cycle++;
                    continue;
                }

                if (slot.Value.Name == message.Zone & cycle == 1)
                {
                    if (!TryComp<TeleportationZoneScanModuleComponent>(slot.Value.ContainerSlot!.ContainedEntity.Value, out var scanComp))
                        return;

                    if (!scanComp.DataRefreshed)
                        return;

                    component.SecondBeacon = scanComp.UIN;
                    break;
                }
            }

            component.ZoneSelected = true;
            CheckСonditions(uid, component);
        }

        private void OnArrivalObjectSelected(EntityUid uid, TeleportationZoneConsoleComponent component, TeleportationZoneConsoleArrivalObjectSelectedMessage message)
        {
            component.LandingPoint = GetEntity(message.Object);
            component.ArrivalObjectSelected = true;
            CheckСonditions(uid, component);
        }

        private void OnStartLandingButtonPressed(EntityUid uid, TeleportationZoneConsoleComponent component, TeleportationZoneConsoleStartTeleportingMessage message)
        {
            bool canStart = false;
            foreach (var battery_uid in _lookup.GetEntitiesInRange<TeleportationZoneBatteryComponent>(Transform(uid).Coordinates, 1))
            {
                if (_tpBattery.CheckCorrectPlacement(uid, battery_uid))
                {
                    canStart = true;

                    if (!TryComp<TeleportationZoneBatteryComponent>(battery_uid, out var batteryComp))
                        continue;

                    if (component.Matter <= batteryComp.Matter)
                    {
                        component.RealMatter = component.Matter;
                        batteryComp.Matter -= component.Matter;
                    }
                    else
                    {
                        component.RealMatter = batteryComp.Matter;
                        batteryComp.Matter = 0;
                    }

                    component.Matter = 0;
                }
            }

            if (!canStart || Transform(component.LandingPoint).GridUid == null || Transform(component.FirstBeacon).GridUid == null)
            {
                _popupSystem.PopupEntity(Loc.GetString("teleportation-zone-console-cant-start"), uid);
                return;
            }

            EntityUid end_station_uid = Transform(component.LandingPoint).GridUid!.Value;
            EntityUid start_station_uid = Transform(component.FirstBeacon).GridUid!.Value;
            var x_form = Transform(component.LandingPoint);
            var Coords = x_form.Coordinates;
            int count = 0;

            component.bottom_border = Transform(component.SecondBeacon).Coordinates.Y;
            component.top_border = Transform(component.FirstBeacon).Coordinates.Y;
            if (Transform(component.FirstBeacon).Coordinates.Y < Transform(component.SecondBeacon).Coordinates.Y)
            {
                component.bottom_border = Transform(component.FirstBeacon).Coordinates.Y;
                component.top_border = Transform(component.SecondBeacon).Coordinates.Y;
            }

            component.left_border = Transform(component.SecondBeacon).Coordinates.X;
            component.right_border = Transform(component.FirstBeacon).Coordinates.X;
            if (Transform(component.FirstBeacon).Coordinates.X < Transform(component.SecondBeacon).Coordinates.X)
            {
                component.left_border = Transform(component.FirstBeacon).Coordinates.X;
                component.right_border = Transform(component.SecondBeacon).Coordinates.X;
            }

            if (!TryComp<MapGridComponent>(end_station_uid, out var end_station_gridComp))
                return;

            if (!TryComp<MapGridComponent>(start_station_uid, out var start_station_gridComp))
                return;

            var tile = new Vector2i((int)(component.CoordX - 0.5f), (int)(component.CoordY - 0.5f));
            var pos = _mapSystem.GridTileToLocal(end_station_uid, end_station_gridComp, tile);

            component.TeleportationProc = true;
            component.Start_station_uid = start_station_uid;
            component.Start_grid_comp = start_station_gridComp;
            component.End_station_uid = end_station_uid;
            component.End_grid_comp = end_station_gridComp;
            component.Position = pos;
            component.NextUpdate = _gameTiming.CurTime + component.UpdateInterval;
            //_tpZone.CheckTils(
            //        uid,
            //        component,
            //        start_station_uid,
            //        start_station_gridComp,
            //        end_station_uid,
            //        end_station_gridComp,
            //        pos);
        }
        #endregion

        #region Auxiliary methods
        private void UpdateUI(EntityUid uid, TeleportationZoneConsoleComponent component)
        {
            var state = new TeleportationZoneConsoleUiState(component.CanStart, component.BluespaceZones, component.ArrivalObjects, component.Matter, component.CoordX, component.CoordY);
            _userInterface.SetUiState(uid, TeleportationZoneConsoleUiKey.Key, state);
        }

        private List<string> UpdateListBluespaceZones(TeleportationZoneConsoleComponent component)
        {
            List<string> ListOfNames = new();
            List<string> BluespaceZones = new();
            foreach (var slot in component.Slots)
            {
                if (slot.Value.ContainerSlot!.ContainedEntity == null)
                    continue;

                ListOfNames.Add(slot.Value.Name);
            }

            foreach (var zone in ListOfNames.Distinct().ToList())
            {
                if ((int)ListOfNames.Count(p => p == zone) == 2)
                    BluespaceZones.Add(zone);
            }

            return BluespaceZones;
        }

        private Dictionary<NetEntity, string> UpdateListArrivalObjects()
        {
            Dictionary<NetEntity, string> ArrivalObjects = new();
            var query = AllEntityQuery<TeleportationZoneRepeaterComponent>();
            while (query.MoveNext(out var item_uid, out var item_comp))
            {
                if (!TryComp<MetaDataComponent>(item_uid, out var item_MetaData))
                    continue;

                var NameLandingPoint = "unknow";

                if (!string.IsNullOrEmpty(item_comp.Name))
                    NameLandingPoint = item_comp.Name;

                ArrivalObjects.Add(_entMan.GetNetEntity(item_uid), NameLandingPoint);
            }

            return ArrivalObjects;
        }

        private void CheckСonditions(EntityUid uid, TeleportationZoneConsoleComponent component)
        {
            if (component.ZoneSelected & component.ArrivalObjectSelected & component.CoordXEntered & component.CoordYEntered)
            {
                component.CanStart = true;
                UpdateUI(uid, component);
                return;
            }

            component.CanStart = false;
            UpdateUI(uid, component);
        }
        #endregion
    }
}
