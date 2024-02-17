using Content.Server.Database;
using Discord.Interactions;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;

namespace Content.DiscordBot;

public sealed partial class VerificationModule : InteractionModuleBase<SocketInteractionContext>
{
    [Dependency] private readonly IServerDbManager _db = default!;
    public static readonly ISawmill Log = LogHelper.GetLogger("discord.interaction_handling");

    public override void BeforeExecute(ICommandInfo command)
    {
        DiscordIoC.InjectDependencies(this);
    }

    [SlashCommand("verify", "Проверифицируйте свой аккаунт")]
    [RequireContext(ContextType.Guild)]
    public async Task VerifyCommand(string verificationCode)
    {
        var player = await _db.VerifyDiscordVerificationCode(verificationCode);

        if (player == null)
        {
            await RespondAsync("К сожалению, не можем найти игрока с данным кодом. Проверьте правильность введёного кода и попробуйте ещё раз.");
            return;
        }

        await _db.LinkDiscord(new NetUserId((Guid) player), Context.User.Id);
        await RespondAsync("Ваш аккаунт Discord успешно привязан!");
    }
}
