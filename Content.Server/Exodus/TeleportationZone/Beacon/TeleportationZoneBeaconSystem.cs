using Content.Shared.Containers.ItemSlots;
using Content.Shared.Exodus.TeleportationZone;
using Robust.Server.GameObjects;
using Robust.Shared.Timing;


namespace Content.Server.Exodus.TeleportationZone.Beacon
{
    public sealed class TeleportationZoneBeaconSystem : EntitySystem
    {
        [Dependency] private readonly UserInterfaceSystem _userInterface = default!;
        [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
        [Dependency] private readonly IGameTiming _gameTiming = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<TeleportationZoneBeaconComponent, ComponentInit>(OnComponentInit);
            SubscribeLocalEvent<TeleportationZoneBeaconComponent, BoundUIOpenedEvent>(OnBoundUIOpened);
            SubscribeLocalEvent<TeleportationZoneBeaconComponent, TeleportationZoneBeaconSaveUINCoreMessage>(OnSaveUINCore);
        }

        private void OnComponentInit(EntityUid uid, TeleportationZoneBeaconComponent component, ComponentInit args)
        {
            _itemSlotsSystem.AddItemSlot(uid, "ScanModule", component.ScanModule);
        }

        private void OnBoundUIOpened(EntityUid uid, TeleportationZoneBeaconComponent component, BoundUIOpenedEvent args)
        {
            var state = new TeleportationZoneBeaconUiState(true, component.CurrentTexturePath);
            _userInterface.SetUiState(uid, TeleportationZoneBeaconUiKey.Key, state);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);
            var query = EntityQueryEnumerator<TeleportationZoneBeaconComponent>();
            while (query.MoveNext(out var uid, out var component))
            {
                if (_gameTiming.CurTime < component.NextUpdate)
                    continue;

                if (component.SaveCoreProcessing)
                {
                    if(component.CurrentSecondProcessing < component.VariantsTexturePath.Count)
                    {
                        component.CurrentTexturePath = component.VariantsTexturePath[component.CurrentSecondProcessing];
                        component.CurrentSecondProcessing++;
                        var state = new TeleportationZoneBeaconUiState(false, component.CurrentTexturePath);
                        _userInterface.SetUiState(uid, TeleportationZoneBeaconUiKey.Key, state);
                    }
                    else
                    {
                        component.CurrentSecondProcessing = 0;
                        component.CurrentTexturePath = component.VariantsTexturePath[component.CurrentSecondProcessing];
                        component.SaveCoreProcessing = false;

                        component.ScanComp.UIN = uid;
                        component.ScanComp.DataRefreshed = true;

                        var state = new TeleportationZoneBeaconUiState(true, component.CurrentTexturePath);
                        _userInterface.SetUiState(uid, TeleportationZoneBeaconUiKey.Key, state);
                    }
                }
                component.NextUpdate += component.UpdateInterval;
            }
        }

        private void OnSaveUINCore(EntityUid uid, TeleportationZoneBeaconComponent component, TeleportationZoneBeaconSaveUINCoreMessage mes)
        {
            foreach(var _object in component.ScanModule.ContainerSlot!.ContainedEntities!)
            {
                if (!TryComp<TeleportationZoneScanModuleComponent>(_object, out var scanComp))
                    return;

                component.SaveCoreProcessing = true;
                component.NextUpdate = _gameTiming.CurTime + component.UpdateInterval;
                component.ScanComp = scanComp;
            }
        }
    }
}
