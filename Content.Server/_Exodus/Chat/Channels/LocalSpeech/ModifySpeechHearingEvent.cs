// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

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
