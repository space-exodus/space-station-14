// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat;
using Content.Shared.Exodus.Chat.Channels.Emote;
using Robust.Client.Player;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed.Commands.Generic;

namespace Content.Client.Exodus.Chat.Channels.Emote;

public sealed partial class EmoteSystem : SharedEmoteSystem
{
    [Dependency] private readonly ISharedChatManager _chat = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    public override void Initialize()
    {
        base.Initialize();

        _chat.OnServerMessage += HandleMessage;

        SubscribeLocalEvent<EmotingComponent, ComponentHandleState>(HandleComponentState);
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

    private void HandleComponentState(Entity<EmotingComponent> emoting, ref ComponentHandleState state)
    {
        if (state.Current is not EmotingComponentState emote)
            return;

        var emotes = new HashSet<EmotePrototype>();

        foreach (var protoId in emote.AvailableEmotes)
        {
            if (_prototype.TryIndex(protoId, out var proto))
                emotes.Add(proto);
        }

        emoting.Comp.AvailableEmotes = emotes;
    }

    public HashSet<EmotePrototype> GetAvailableEmotes()
    {
        var player = _player.LocalSession?.AttachedEntity;

        if (player == null)
            return new();

        if (!TryComp<EmotingComponent>(player, out var emoting))
            return new();

        return emoting.AvailableEmotes;
    }

    public void PlayEmoteMessage(ProtoId<EmotePrototype> emote)
    {
        var message = new EmoteClientMessage()
        {
            ProtoId = emote.Id,
        };
        _chat.ClientSendMessage(message);
    }

    public void PlayEmoteMessage(string content)
    {
        var message = new EmoteClientMessage()
        {
            Message = content,
        };
        _chat.ClientSendMessage(message);
    }
}
