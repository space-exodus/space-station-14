// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat.Channels.LocalSpeech;
using Content.Shared.Interaction.Events;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server.Exodus.Chat.Channels.LocalSpeech;

public sealed partial class LocalSpeechSystem
{
    private void HandleSpeechHearing(Entity<SpeechHearingComponent> entity, ref HearSpeechEvent ev)
    {
        var consciousEv = new ConsciousAttemptEvent(entity);
        RaiseLocalEvent(entity, ref consciousEv);

        if (consciousEv.Cancelled)
            return;

        if (!TryComp<ActorComponent>(entity, out var actor))
            return;

        var msg = new LocalSpeechServerMessage()
        {
            Content = ev.Speech.Content,
            SenderName = ev.SenderName,
            SpeechType = ev.Speech.Type,
            SpeechVerb = ev.SpeechVerb,
        };
        _chat.SendNetworkMessage((entity.Owner, actor), msg);
    }
}
