// using Discord.Interactions;
// using Robust.Shared.IoC;
// using Robust.Shared.Prototypes;

// namespace Content.DiscordBot;

// public sealed partial class PrototypesModule : InteractionModuleBase<SocketInteractionContext>
// {
//     [Dependency] private readonly IPrototypeManager _prototype = default!;

//     public override void BeforeExecute(ICommandInfo command)
//     {
//         DiscordIoC.InjectDependencies(this);
//     }

//     [SlashCommand("proto", "A Test Command To Show How It Works!")]
//     public async Task ProtoCommand(string prototypeName)
//     {
//         if (_prototype.TryIndex(prototypeName, out var prototype))
//         {
//             await RespondAsync($"## Found prototype!\n**ID:** `{prototype.ID}`\n**Name:** `{prototype.Name}`\n**Description:** `{prototype.Description}`");
//         }
//         else
//         {
//             await RespondAsync("Couldn't find prototype with specified name");
//         }
//     }
// }
