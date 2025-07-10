using Content.Shared.Exodus.Chat.Channels.LocalSpeech;

namespace Content.Server.Exodus.Chat.Channels.LocalSpeech;

[ByRefEvent]
public sealed partial class ModifySpeechHearingMessageEvent : EntityEventArgs
{
    public readonly EntityUid Recipient;
    public string SenderName;
    public LocalSpeechMessage Speech;

    public ModifySpeechHearingMessageEvent(EntityUid recipient, string senderName, LocalSpeechMessage speech)
    {
        Recipient = recipient;
        SenderName = senderName;
        Speech = speech;
    }
}
