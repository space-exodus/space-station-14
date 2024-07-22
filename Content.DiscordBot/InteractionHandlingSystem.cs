// using Discord;
// using Discord.Interactions;
// using Discord.WebSocket;
// using Microsoft.Extensions.Hosting;
// using Robust.Shared.Log;
// using System.Reflection;

// namespace Content.DiscordBot
// {
//     public class InteractionHandlingService : IHostedService
//     {
//         private readonly DiscordSocketClient _client;
//         private readonly InteractionService _interactions;
//         private readonly IServiceProvider _services;
//         private readonly ISawmill _logger = LogHelper.GetLogger("discord.interaction_handling");

//         public InteractionHandlingService(
//             DiscordSocketClient client,
//             InteractionService interactions,
//             IServiceProvider services)
//         {
//             _client = client;
//             _interactions = interactions;
//             _services = services;

//             _interactions.Log += (message) => LogHelper.LogAsync(message, _logger);
//         }

//         public async Task StartAsync(CancellationToken token)
//         {
//             _client.Ready += () => _interactions.RegisterCommandsGloballyAsync(true);
//             _client.InteractionCreated += OnInteractionAsync;

//             await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
//         }

//         public Task StopAsync(CancellationToken token)
//         {
//             _interactions.Dispose();
//             return Task.CompletedTask;
//         }

//         private async Task OnInteractionAsync(SocketInteraction interaction)
//         {
//             try
//             {
//                 var context = new SocketInteractionContext(_client, interaction);
//                 var result = await _interactions.ExecuteCommandAsync(context, _services);

//                 if (!result.IsSuccess)
//                 {
//                     _logger.Error(result.ErrorReason);
//                 }
//             }
//             catch
//             {
//                 if (interaction.Type == InteractionType.ApplicationCommand)
//                 {
//                     await interaction.GetOriginalResponseAsync()
//                         .ContinueWith(msg => msg.Result.DeleteAsync());
//                 }
//             }
//         }
//     }
// }
