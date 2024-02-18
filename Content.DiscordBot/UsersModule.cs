using Content.Server.Database;
using Discord;
using Discord.Interactions;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;

namespace Content.DiscordBot;

public sealed partial class UsersModule : InteractionModuleBase<SocketInteractionContext>
{
    [Dependency] private readonly IServerDbManager _db = default!;
    public static readonly ISawmill Log = LogHelper.GetLogger("discord.users_module");

    public override void BeforeExecute(ICommandInfo command)
    {
        DiscordIoC.InjectDependencies(this);
    }

    [SlashCommand("привязать", "Привяжите свой аккаунт Discord к аккаунту SS14")]
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

    [SlashCommand("призвать", "Призовите игрока")]
    [RequireContext(ContextType.Guild)]
    [RequireUserPermission(GuildPermission.MentionEveryone)]
    public async Task SummonCommand(string ckey)
    {
        var player = await _db.GetPlayerRecordByUserName(ckey);

        if (player == null)
        {
            await RespondAsync("❗ Пользователя с таким CKey не существует в нашей базе данных");
            return;
        }

        if (player.DiscordId == null)
        {
            await RespondAsync("❗ У данного пользователя отсутствует привязанный профиль Discord");
            return;
        }

        await RespondAsync($"👉 <@!{player.DiscordId}>");
    }
}
