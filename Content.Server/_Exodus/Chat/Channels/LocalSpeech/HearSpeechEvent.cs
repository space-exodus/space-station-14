using Content.Shared.Exodus.Chat.Channels.LocalSpeech;

namespace Content.Server.Exodus.Chat.Channels.LocalSpeech;

[ByRefEvent]
public sealed partial class HearSpeechEvent : EntityEventArgs
{
    public readonly EntityUid Recipient;
    public readonly string SenderName;

    public EntityUid Sender => Speech.Sender;
    public readonly LocalSpeechMessage Speech;

    public HearSpeechEvent(EntityUid recipient, string senderName, LocalSpeechMessage speech)
    {
        Recipient = recipient;
        SenderName = senderName;
        Speech = speech;
    }
}
