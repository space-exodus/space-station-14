using Content.Server.Chat.Systems;
using Content.Shared.Coordinates;
using Content.Shared.Exodus.TeleportationZone;
using Robust.Server.GameObjects;
using System.Linq;

namespace Content.Server.Exodus.TeleportationZone
{
    public sealed class AstroDungeonListener : EntitySystem
    {
        [Dependency] private readonly ChatSystem _chat = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<TeleportationZoneAstroDungeonComponent, ComponentInit>(OnCompInit);
        }

        private void OnCompInit(EntityUid uid, TeleportationZoneAstroDungeonComponent component, ComponentInit args)
        {
            _chat.DispatchGlobalAnnouncement(Loc.GetString(component.Text), "Central Command", true, null, Color.Yellow);
        }
    }
}
