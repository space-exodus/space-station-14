// using Content.Server.Database;
// using Discord;
// using Discord.Interactions;
// using Discord.WebSocket;
// using Robust.Shared.IoC;
// using Robust.Shared.Log;
// using Robust.Shared.Maths;
// using Robust.Shared.Network;
// using Color = Robust.Shared.Maths.Color;

// namespace Content.DiscordBot;

// public sealed partial class UsersModule : InteractionModuleBase<SocketInteractionContext>
// {
//     [Dependency] private readonly IServerDbManager _db = default!;
//     public static readonly ISawmill Log = LogHelper.GetLogger("discord.users_module");

//     public override void BeforeExecute(ICommandInfo command)
//     {
//         DiscordIoC.InjectDependencies(this);
//     }

//     [SlashCommand("привязать", "Привяжите свой аккаунт Discord к аккаунту SS14")]
//     [RequireContext(ContextType.Guild)]
//     public async Task VerifyCommand([Summary("код-верификации", "Данный код вы должны получить зайдя на наш сервер SS14")] string verificationCode)
//     {
//         var player = await _db.VerifyDiscordVerificationCode(verificationCode);

//         if (player == null)
//         {
//             await RespondAsync("К сожалению, не можем найти игрока с данным кодом. Проверьте правильность введёного кода и попробуйте ещё раз.", ephemeral: true);
//             return;
//         }

//         await _db.LinkDiscord(new NetUserId((Guid) player), Context.User.Id);
//         await RespondAsync("Ваш аккаунт Discord успешно привязан!", ephemeral: true);
//     }

//     [SlashCommand("призвать", "Призовите игрока")]
//     [RequireContext(ContextType.Guild)]
//     [RequireUserPermission(GuildPermission.MentionEveryone)]
//     public async Task SummonCommand(string ckey)
//     {
//         var player = await _db.GetPlayerRecordByUserName(ckey);

//         if (player == null)
//         {
//             await RespondAsync("❗ Пользователя с таким CKey не существует в нашей базе данных", ephemeral: true);
//             return;
//         }

//         if (player.DiscordId == null)
//         {
//             await RespondAsync("❗ У данного пользователя отсутствует привязанный профиль Discord", ephemeral: true);
//             return;
//         }

//         await RespondAsync($"👉 <@!{player.DiscordId}>");
//     }

//     [SlashCommand("получить-ckey", "Узнать CKey игрока")]
//     [RequireContext(ContextType.Guild)]
//     [RequireBotPermission(GuildPermission.MentionEveryone)]
//     public async Task GetCKey(SocketGuildUser user)
//     {
//         var player = await _db.GetPlayerRecordByDiscordId(user.Id);

//         if (player == null)
//         {
//             await RespondAsync("❗ У данного пользователя отсутствует привязанный профиль Space Station 14", ephemeral: true);
//             return;
//         }

//         await RespondAsync($"> CKey игрока: {player.LastSeenUserName}", ephemeral: true);
//     }

//     [SlashCommand("выдать-спонсора", "Выдать привелегии спонсора игроку")]
//     [RequireContext(ContextType.Guild)]
//     [RequireUserPermission(GuildPermission.Administrator)]
//     public async Task PromoteSponsor(string ckey)
//     {
//         var player = await _db.GetPlayerRecordByUserName(ckey);

//         if (player == null)
//         {
//             await RespondAsync("❗ Пользователя с таким CKey не существует в нашей базе данных", ephemeral: true);
//             return;
//         }

//         if (player.IsPremium)
//         {
//             await RespondAsync("❗ Пользователь уже обладает привелегиями спонсора", ephemeral: true);
//             return;
//         }

//         await _db.PromoteSponsor(player.UserId);
//         await RespondAsync($"✅ Привелегии спонсора успешно были выданы игроку `{ckey}`!", ephemeral: true);
//     }

//     [SlashCommand("забрать-спонсора", "Забрать привелегии спонсора у игрока")]
//     [RequireContext(ContextType.Guild)]
//     [RequireUserPermission(GuildPermission.Administrator)]
//     public async Task UnpromoteSponsor(string ckey)
//     {
//         var player = await _db.GetPlayerRecordByUserName(ckey);

//         if (player == null)
//         {
//             await RespondAsync("❗ Пользователя с таким CKey не существует в нашей базе данных", ephemeral: true);
//             return;
//         }

//         if (!player.IsPremium)
//         {
//             await RespondAsync("❗ У данного пользователя нет привелегий спонсора", ephemeral: true);
//             return;
//         }

//         await _db.UnpromoteSponsor(player.UserId);
//         await RespondAsync($"✅ Привелегии спонсора успешно были забраны у игрока `{ckey}`!", ephemeral: true);
//     }

//     [SlashCommand("установить-цвет-ooc", "Установить цвет своего OOC (только для спонсоров!)")]
//     [RequireContext(ContextType.Guild)]
//     public async Task SetPremiumOOCColor([Summary("цвет")] PremiumOOCColor color)
//     {
//         var player = await _db.GetPlayerRecordByDiscordId(Context.User.Id);

//         if (player == null || !player.IsPremium)
//         {
//             await RespondAsync("> ❗ Вы не являетесь спонсором!", ephemeral: true);
//             return;
//         }

//         if (!OOCColorsDict.TryGetValue(color, out var selectedColor))
//         {
//             await RespondAsync($"> ❗ Был выбран неправильный цвет", ephemeral: true);
//             return;
//         }

//         await _db.SetPremiumOOCColor(player.UserId, selectedColor.ToHex());
//         await RespondAsync("> ✅ Новый цвет был успешно установлен! Для применения изменений настроек перезайдите на сервер.", ephemeral: true);
//     }

//     public static readonly Dictionary<PremiumOOCColor, Color> OOCColorsDict = new()
//     {
//         { PremiumOOCColor.Red, Color.Red },
//         { PremiumOOCColor.Orange, Color.Orange },
//         { PremiumOOCColor.Yellow, Color.Yellow },
//         { PremiumOOCColor.Green, Color.Green },
//         { PremiumOOCColor.Blue, Color.FromHex("#1864ab") },
//         { PremiumOOCColor.Purple, Color.Purple },
//     };

//     public enum PremiumOOCColor
//     {
//         [ChoiceDisplay("Красный 🔴")]
//         Red,
//         [ChoiceDisplay("Оранжевый 🟠")]
//         Orange,
//         [ChoiceDisplay("Жёлтый 🟡")]
//         Yellow,
//         [ChoiceDisplay("Зелёный 🟢")]
//         Green,
//         [ChoiceDisplay("Синий 🔵")]
//         Blue,
//         [ChoiceDisplay("Фиолетовый 🟣")]
//         Purple,
//     }
// }
