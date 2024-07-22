// using Content.Shared.CCVar;
// using Discord;
// using Discord.WebSocket;
// using Microsoft.Extensions.Hosting;
// using Robust.Server;
// using Robust.Server.GameObjects;
// using Robust.Shared;
// using Robust.Shared.Asynchronous;
// using Robust.Shared.Configuration;
// using Robust.Shared.ContentPack;
// using Robust.Shared.IoC;
// using Robust.Shared.Localization;
// using Robust.Shared.Log;
// using Robust.Shared.Network;
// using Robust.Shared.Prototypes;
// using Robust.Shared.Reflection;
// using Robust.Shared.Serialization;
// using Robust.Shared.Serialization.Manager;
// using Robust.Shared.Threading;

// namespace Content.DiscordBot;

// public sealed class DiscordBot : IHostedService
// {
//     [Dependency] private readonly IServerNetManager _network = default!;
//     [Dependency] private readonly INetConfigurationManagerInternal _config = default!;
//     [Dependency] private readonly ITaskManager _task = default!;
//     [Dependency] private readonly IParallelManagerInternal _parallel = default!;
//     [Dependency] private readonly IResourceManagerInternal _resources = default!;
//     [Dependency] private readonly IModLoaderInternal _modLoader = default!;
//     [Dependency] private readonly ILogManager _logManager = default!;
//     [Dependency] private readonly IPrototypeManagerInternal _prototype = default!;
//     [Dependency] private readonly IRobustSerializer _serializer = default!;
//     [Dependency] private readonly ILocalizationManagerInternal _loc = default!;
//     [Dependency] private readonly IRobustMappedStringSerializer _stringSerializer = default!;
//     [Dependency] private readonly ISerializationManager _serialization = default!;
//     [Dependency] private readonly IReflectionManager _reflection = default!;
//     [Dependency] private readonly IServerEntityManager _entityManager = default!;

//     private readonly DiscordSocketClient _client = new();

//     private static readonly string Token =
//         Environment.GetEnvironmentVariable("EXODUS_DISCORD_TOKEN")
//         ?? throw new NullReferenceException("No ENV Token Found.");

//     private ISawmill _logger = default!;
//     public ServerOptions Options { get; private set; } = new();

//     public DiscordBot(DiscordSocketClient discord)
//     {
//         _client = discord;

//         var deps = DiscordIoC.Init();
//         deps.BuildGraph();
//         Robust.Server.Program.SetupLogging(deps);
//         Robust.Server.Program.InitReflectionManager(deps);
//         deps.InjectDependencies(this);
//     }

//     public void Initialize()
//     {
//         // Past initializations
//         _config.Initialize(true);

//         var path = PathHelpers.ExecutableRelativeFile("server_config.toml");

//         if (File.Exists(path))
//         {
//             _config.LoadFromFile(path);
//         }
//         else
//         {
//             _config.SetSaveFile(path);
//         }

//         _config.LoadCVarsFromAssembly(typeof(BaseServer).Assembly); // Robust.Server
//         _config.LoadCVarsFromAssembly(typeof(IConfigurationManager).Assembly); // Robust.Shared

//         _config.OverrideConVars(EnvironmentVariables.GetEnvironmentCVars());

//         _task.Initialize();
//         _parallel.Initialize();

//         _network.Initialize(true);

//         _logger = _logManager.GetSawmill("srv");

//         var dataDir = Options.LoadConfigAndUserData
//             ? PathHelpers.ExecutableRelativeFile("data")
//             : null;

//         // Set up the VFS
//         _resources.Initialize(dataDir);

//         ProgramShared.DoMounts(_resources, Options.MountOptions, Options.ContentBuildDirectory, Options.AssemblyDirectory,
//             Options.LoadContentResources, Options.ResourceMountDisabled, true);

//         _modLoader.SetUseLoadContext(false);
//         _modLoader.SetEnableSandboxing(Options.Sandboxing);

//         var resourceManifest = ResourceManifestData.LoadResourceManifest(_resources);

//         if (!_modLoader.TryLoadModulesFrom(Options.AssemblyDirectory, resourceManifest.AssemblyPrefix ?? Options.ContentModulePrefix))
//         {
//             _logger.Fatal("Errors while loading content assemblies.");
//             return;
//         }

//         foreach (var loadedModule in _modLoader.LoadedModules)
//         {
//             _config.LoadCVarsFromAssembly(loadedModule);
//         }

//         _logger.Debug("Pre Init run level");
//         _modLoader.BroadcastRunLevel(ModRunLevel.PreInit);

//         // HAS to happen after content gets loaded.
//         // Else the content types won't be included.
//         _serializer.Initialize();

//         _loc.AddLoadedToStringSerializer(_stringSerializer);

//         _prototype.LoadedData += data =>
//             {
//                 if (!_stringSerializer.Locked)
//                 {
//                     _stringSerializer.AddStrings(data.Root);
//                 }
//             };

//         _logger.Debug("Init run level");
//         _modLoader.BroadcastRunLevel(ModRunLevel.Init);

//         _entityManager.Initialize();
//         _serialization.Initialize();

//         _prototype.Initialize();
//         _prototype.LoadDefaultPrototypes();
//         _reflection.Initialize();

//         _config.SetCVar(CCVars.GameDummyTicker, true);

//         _entityManager.Startup();

//         _logger.Debug("Post Init run level");
//         _modLoader.BroadcastRunLevel(ModRunLevel.PostInit);

//         _client.Log += LogHelper.LogAsync;
//     }

//     public async Task StartAsync(CancellationToken token)
//     {
//         Initialize();
//         await _client.LoginAsync(TokenType.Bot, Token);
//         await _client.StartAsync();
//     }

//     public async Task StopAsync(CancellationToken cancellationToken)
//     {
//         await _client.LogoutAsync();
//         await _client.StopAsync();
//     }
// }
