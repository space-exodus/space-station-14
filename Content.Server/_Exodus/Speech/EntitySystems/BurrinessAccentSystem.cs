using Content.Server.Exodus.Speech.Components;
using Content.Shared.Speech;

namespace Content.Server.Speech.EntitySystems
{
    public sealed class BurrinessAccentSystem : EntitySystem
    {
        public override void Initialize()
        {
            SubscribeLocalEvent<BurrinessAccentComponent, AccentGetEvent>(OnAccent);
        }

        private void OnAccent(EntityUid uid, BurrinessAccentComponent component, AccentGetEvent args)
        {
            args.Message = args.Message.Replace("р", "в").Replace("Р", "В");
        }
    }
}
