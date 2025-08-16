// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using System.Linq;
using Content.Server.Interaction;
using Content.Shared.Exodus.Chat;
using Content.Shared.Exodus.Chat.Channels.Emote;
using Content.Shared.Exodus.Utils;
using Content.Shared.Ghost;
using Content.Shared.Physics;
using Content.Shared.Whitelist;
using Robust.Shared.GameStates;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Exodus.Chat.Channels.Emote;

public sealed partial class EmoteSystem : SharedEmoteSystem
{
    [Dependency] private readonly ChatIdentitySystem _chatIdentity = default!;
    [Dependency] private readonly ISharedChatManager _chat = default!;
    [Dependency] private readonly InteractionSystem _interaction = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;

    public override void Initialize()
    {
        base.Initialize();

        _chat.OnClientMessage += HandleMessage;
        _prototype.PrototypesReloaded += PrototypesReloaded;

        SubscribeLocalEvent<EmotingComponent, ComponentGetState>(HandleComponentState);
        SubscribeLocalEvent<EmotingComponent, ComponentStartup>(Startup);
    }

    #region Private API
    private void Startup(Entity<EmotingComponent> entity, ref ComponentStartup ev)
    {
        RefreshAvailableEmotes(entity);
    }

    private void HandleComponentState(Entity<EmotingComponent> entity, ref ComponentGetState state)
    {
        if (state.Player?.AttachedEntity is not { } player)
            return;

        var newState = new EmotingComponentState();
        state.State = newState;

        if (player != entity.Owner && !HasComp<GhostComponent>(player))
            return;

        newState.AvailableEmotes = [.. entity.Comp.AvailableEmotes.Select(emote => emote.ID)];
    }

    private void PrototypesReloaded(PrototypesReloadedEventArgs args)
    {
        var query = AllEntityQuery<EmotingComponent>();

        while (query.MoveNext(out var uid, out var emoting))
        {
            RefreshAvailableEmotes((uid, emoting));
        }
    }

    private void HandleMessage(ICommonSession player, BaseChatClientMessage message)
    {
        if (message is not EmoteClientMessage emote)
            return;

        if (player.AttachedEntity is not { } sender)
            return;

        PerformEmote(sender, emote.Message);
    }

    private bool CanSee(EntityUid sender, EntityUid recipient)
    {
        if (HasComp<GhostComponent>(recipient))
            return true;

        if (!_interaction.InRangeUnobstructed(sender, recipient, 15, CollisionGroup.Opaque))
            return false;

        if (this.IsInConscious(recipient, EntityManager))
            return false;

        return true;
    }

    private void SendMessageToOne(EntityUid sender, string senderNameInitial, EntityUid recipient, string content, ProtoId<EmotePrototype>? emote)
    {
        if (CanSee(sender, recipient))
            return;

        var senderName = _chatIdentity.GetSenderNameForRecipient(sender, senderNameInitial, recipient);

        var message = new EmoteServerMessage()
        {
            Message = content,
            SenderName = senderName,
            Emoting = GetNetEntity(recipient),
            Emote = emote,
        };

        _chat.ServerSendMessage(recipient, message);
    }

    private EmotePrototype? GetEmoteByTrigger(Entity<EmotingComponent> emoting, string message)
    {
        foreach (var emote in emoting.Comp.AvailableEmotes)
        {
            foreach (var trigger in emote.Triggers)
            {
                if (message.Contains(trigger))
                    return emote;
            }
        }

        return null;
    }

    #endregion

    #region Public API

    /// <summary>
    /// The specified entity will perform an emote
    /// </summary>
    /// <param name="entity">Sender entity</param>
    /// <param name="message">Message content</param>
    /// <param name="force">Bypass sender checks</param>
    public void PerformEmote(EntityUid entity, string message, bool force = false) // if you ever would need more than 2 message params, for God's sake, use dedicated data-class
    {
        if (!force && !CanEmote(entity))
            return;

        if (!TryComp<EmotingComponent>(entity, out var emoting))
            return;

        var emote = GetEmoteByTrigger((entity, emoting), message);
        var initialSenderName = _chatIdentity.GetSenderNameInitial(entity);
        var recipients = Filter.Pvs(entity, 1).Recipients;

        foreach (var recipient in recipients)
        {
            if (recipient.AttachedEntity is not { } recipientEnt)
                continue;

            SendMessageToOne(entity, initialSenderName, recipientEnt, message, emote?.ID);
        }
    }

    public bool CanEmote(EntityUid sender)
    {
        return HasComp<EmotingComponent>(sender) && this.IsInConscious(sender, EntityManager);
    }

    public HashSet<EmotePrototype> RefreshAvailableEmotes(Entity<EmotingComponent> entity)
    {
        var emotes = new HashSet<EmotePrototype>();

        foreach (var proto in _prototype.EnumeratePrototypes<EmotePrototype>())
        {
            if (_whitelist.IsWhitelistFail(proto.Whitelist, entity) ||
                _whitelist.IsBlacklistFail(proto.Blacklist, entity))
                continue;

            emotes.Add(proto);
        }

        return emotes;
    }
    #endregion
}
