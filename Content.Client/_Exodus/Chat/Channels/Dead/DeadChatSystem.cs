using Content.Shared.Exodus.Chat;

namespace Content.Client.Exodus.Chat.Channels.Dead;

public sealed partial class DeadChatSystem : EntitySystem
{
    [Dependency] private readonly ISharedChatManager _chat = default!;

    public override void Initialize()
    {
        base.Initialize();

        // all logic will be implemented when I will start work on client
    }
}
