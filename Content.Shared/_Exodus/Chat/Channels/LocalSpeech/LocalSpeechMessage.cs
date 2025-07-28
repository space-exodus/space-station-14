namespace Content.Shared.Exodus.Chat.Channels.LocalSpeech;

/// <summary>
/// Structure made for simplicity of working with LocalSpeechSystem
/// containing minimal set of data needed for creating message
/// to not duplicate arguments in methods when passing data.
/// 
/// Shouldn't be editable, if you wanna to edit message then create a new structure
/// </summary>
public struct LocalSpeechMessage
{
    public readonly EntityUid Sender;
    public readonly LocalSpeechType Type;
    public readonly string Content;

    public LocalSpeechMessage(EntityUid sender, LocalSpeechType type, string content)
    {
        Sender = sender;
        Type = type;
        Content = content;
    }

    public static LocalSpeechMessage FromClientMessage(LocalSpeechClientMessage message, EntityUid sender)
    {
        return new LocalSpeechMessage(sender, message.Type, message.Message);
    }

    public LocalSpeechMessage WithContent(string content)
    {
        return new LocalSpeechMessage(Sender, Type, content);
    }
}
