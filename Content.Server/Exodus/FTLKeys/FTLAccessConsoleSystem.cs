using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;
using Content.Shared.Tag;
using Content.Server.Shuttles.Systems;
using Content.Shared.Lock;

namespace Content.Server.Exodus.FTLKey
{
    public sealed class FTLAccessConsoleSystem : EntitySystem
    {
        [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly TagSystem _tagSystem = default!;
        [Dependency] private readonly ShuttleConsoleSystem _shuttleConsoleSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<FTLAccessConsoleComponent, ComponentInit>(OnComponentInit);
            SubscribeLocalEvent<FTLAccessConsoleComponent, ComponentRemove>(OnComponentRemove);

            SubscribeLocalEvent<FTLAccessConsoleComponent, EntInsertedIntoContainerMessage>(OnItemInserted);
            SubscribeLocalEvent<FTLAccessConsoleComponent, EntRemovedFromContainerMessage>(OnItemRemoved);
            SubscribeLocalEvent<FTLAccessConsoleComponent, AnchorStateChangedEvent>(OnAnchorChange);

            //Locking
            SubscribeLocalEvent<FTLAccessConsoleComponent, LockToggledEvent>(OnLockToggled);
        }

        private void OnComponentInit(EntityUid uid, FTLAccessConsoleComponent consl, ComponentInit args)
        {
            foreach (var slot in consl.Slots)
            {
                _itemSlotsSystem.AddItemSlot(uid, slot.Key, slot.Value);
            }

            UpdateAccess(uid, consl);
        }

        private void OnComponentRemove(EntityUid uid, FTLAccessConsoleComponent consl, ComponentRemove args)
        {
            RemoveAccess(uid, consl);

            foreach (var slot in consl.Slots.Values)
            {
                _itemSlotsSystem.TryEject(uid, slot, null, out var _);
                _itemSlotsSystem.RemoveItemSlot(uid, slot);
            }
        }

        private void OnItemInserted(EntityUid uid, FTLAccessConsoleComponent consl, EntInsertedIntoContainerMessage args)
        {
            var xform = Transform(uid);
            if (!xform.Anchored) return;

            UpdateAccess(uid, consl);
        }

        private void OnItemRemoved(EntityUid uid, FTLAccessConsoleComponent consl, EntRemovedFromContainerMessage args)
        {
            var xform = Transform(uid);
            if (!xform.Anchored) return;

            RemoveCurrentAccess(uid, args.Entity);
            UpdateAccess(uid, consl);
        }

        private void OnAnchorChange(EntityUid uid, FTLAccessConsoleComponent consl, AnchorStateChangedEvent args)
        {
            if (args.Anchored) UpdateAccess(uid, consl);
            else RemoveAccess(uid, consl);
        }

        private void OnLockToggled(EntityUid uid, FTLAccessConsoleComponent consl, LockToggledEvent args)
        {
            if (args.Locked)
            {
                foreach (var slot in consl.Slots.Values)
                {
                    _itemSlotsSystem.SetLock(uid, slot, true);
                }
            }
            else
            {
                foreach (var slot in consl.Slots.Values)
                {
                    _itemSlotsSystem.SetLock(uid, slot, false);
                }
            }
        }

        /// <summary>
        /// Sets Access from inserted Keys
        /// </summary>
        private void UpdateAccess(EntityUid uid, FTLAccessConsoleComponent consl)
        {
            foreach (var slot in consl.Slots.Values)
            {
                if (slot.ContainerSlot is not null && slot.ContainerSlot.ContainedEntity is not null)
                    AddCurrentAccess(uid, slot.ContainerSlot.ContainedEntity.Value);
            }

            UpdateConsole(uid);
        }

        /// <summary>
        /// Remove access of all inserted keys
        /// </summary>

        private void RemoveAccess(EntityUid uid, FTLAccessConsoleComponent consl)
        {
            foreach (var slot in consl.Slots.Values)
            {
                if (slot.ContainerSlot is not null && slot.ContainerSlot.ContainedEntity is not null)
                    RemoveCurrentAccess(uid, slot.ContainerSlot.ContainedEntity.Value);
            }

            UpdateConsole(uid);
        }

        /// <summary>
        /// Add access of current Key
        /// </summary>
        private void AddCurrentAccess(EntityUid uid, EntityUid added)
        {
            var xform = Transform(uid);
            if (xform.GridUid is null) return;

            if (!TryComp<FTLKeyComponent>(added, out var keyComp) || keyComp.FTLKeys is null) return;
            _tagSystem.AddTags(xform.GridUid.Value, keyComp.FTLKeys);
        }

        /// <summary>
        /// Remove access of current Key
        /// </summary>
        private void RemoveCurrentAccess(EntityUid uid, EntityUid removed)
        {
            var xform = Transform(uid);
            if (xform.GridUid is null) return;

            if (!TryComp<FTLKeyComponent>(removed, out var keyComp) || keyComp.FTLKeys is null) return;
            _tagSystem.RemoveTags(xform.GridUid.Value, keyComp.FTLKeys);
        }

        private void UpdateConsole(EntityUid uid)
        {
            _shuttleConsoleSystem.RefreshShuttleConsoles(uid);
        }
    }
}
