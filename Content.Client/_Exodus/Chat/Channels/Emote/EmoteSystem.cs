// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat;
using Content.Shared.Exodus.Chat.Channels.Emote;
using Robust.Shared.Prototypes;

namespace Content.Client.Exodus.Chat.Channels.Emote;

public sealed partial class EmoteSystem : SharedEmoteSystem
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        _chat.OnHandleMessage += HandleMessage;
    }

    private void HandleMessage(BaseChatServerMessage message)
    {
        if (message is not EmoteServerMessage emote)
            return;

        var emoting = GetEntity(emote.Emoting);
        _prototype.TryIndex(emote.Emote, out var prototype);

        var ev = new EmoteEvent()
        {
            Emote = prototype,
            Emoting = emoting,
        };
        RaiseLocalEvent(emoting, ref ev);
    }
}
