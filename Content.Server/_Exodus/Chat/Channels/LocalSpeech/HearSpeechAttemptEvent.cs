using Content.Shared.Exodus.Chat.Channels.LocalSpeech;

namespace Content.Server.Exodus.Chat.Channels.LocalSpeech;

[ByRefEvent]
public sealed partial class HearSpeechAttemptEvent : CancellableEntityEventArgs
{
    public readonly EntityUid Recipient;
    public readonly string SenderName;
    public readonly LocalSpeechMessage Speech;

    public HearSpeechAttemptEvent(EntityUid recipient, string senderName, LocalSpeechMessage speech)
    {
        Recipient = recipient;
        SenderName = senderName;
        Speech = speech;
    }
}
