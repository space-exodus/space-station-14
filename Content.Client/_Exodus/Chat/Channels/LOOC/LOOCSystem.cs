using Content.Shared.Exodus.Chat;

namespace Content.Client.Exodus.Chat.Channels.LOOC;

public sealed partial class LOOCSystem : EntitySystem
{
    [Dependency] private readonly ISharedChatManager _chat = default!;

    public override void Initialize()
    {
        base.Initialize();

        // all logic will be implemented when I will start work on client
    }
}
