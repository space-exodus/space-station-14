// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat.Channels.LocalSpeech;

namespace Content.Server.Exodus.Chat.Channels.LocalSpeech;

[ByRefEvent]
public sealed partial class HearSpeechEvent : EntityEventArgs
{
    public readonly EntityUid Recipient;
    public readonly string SenderName;
    public LocId? SpeechVerb;

    public EntityUid Sender => Speech.Sender;
    public readonly LocalSpeechMessage Speech;

    public HearSpeechEvent(EntityUid recipient, string senderName, LocalSpeechMessage speech, LocId? speechVerb)
    {
        Recipient = recipient;
        SenderName = senderName;
        Speech = speech;
        SpeechVerb = speechVerb;
    }
}
