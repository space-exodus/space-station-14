// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

namespace Content.Server.Exodus.Chat;

// I really don't know where to put theese
public sealed partial class ChatIdentitySystem : EntitySystem
{
    public string GetSenderNameInitial(EntityUid sender)
    {
        return MetaData(sender).EntityName;
    }

    public string GetSenderNameForRecipient(EntityUid sender, string initialSenderName, EntityUid recipient)
    {
        return initialSenderName;
    }
}
