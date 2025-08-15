// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Atmos.EntitySystems;
using Content.Server.NPC.Pathfinding;
using Content.Shared.Exodus.Chat;
using Content.Shared.Exodus.Chat.Channels.LocalSpeech;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Exodus.Chat.Channels.LocalSpeech;

/// Asynchronious approach for handling message hearing in this system was implemented
/// because we needed the result of PathfindingSystem which gives the results only in asynchronious approach
/// We get potential recipients and requesting Pathfinding system to find path to every recipient

public sealed partial class LocalSpeechSystem : EntitySystem
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly PathfindingSystem _pathfinding = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly MapSystem _map = default!;
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    private Queue<LocalSpeechRequest> _requests = new();

    public const float SpeechLowerThresholdPressure = 30f;
    public const float SpeechReferencePressure = 101.325f;
    public const float SpeechUpperThresholdPressure = 50000f;

    public override void Initialize()
    {
        base.Initialize();

        _chat.OnHandleMessage += HandleMessage;

        SubscribeLocalEvent<SpeechHearingComponent, HearSpeechEvent>(HandleSpeechHearing);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        while (_requests.TryDequeue(out var request))
        {
            // TODO: Create timeout for Pathfinding system in complex scenarious
            if (!request.Task.IsCompleted)
            {
                _requests.Enqueue(request);
                continue;
            }

            var fadeLevel = 0f;

            // do not perform fade level checks if sender is recipient
            if (request.Sender != request.Recipient)
            {
                var range = GetMaxRange(request.Type);
                var path = request.PathResult.ConfigureAwait(false).GetAwaiter().GetResult().Path;

                if (!GetFadeLevel(request.Sender, request.Recipient, range, path, out fadeLevel))
                    continue;
            }

            SendMessageToOne(request.Message, request.SenderName, request.Recipient, fadeLevel);
        }
    }

    private void HandleMessage(ICommonSession player, BaseChatClientMessage message)
    {
        if (message is not LocalSpeechClientMessage local)
            return;

        // TODO: validation of blockers specific for ICommonSession
        if (player.AttachedEntity is not { } sender)
        {
            // TODO: Exodus-Client-Modification-Detection
            Log.Warning($"{player.Name} tried to send local message without an attached body");
            return;
        }
        // TODO: check of speech component of sender

        SendMessage(LocalSpeechMessage.FromClientMessage(local, sender));
    }

    public void SendMessage(LocalSpeechMessage message)
    {
        // TODO: entire validation of sender, blockers and etc

        var range = GetMaxRange(message.Type);
        var recipients = GetRecipientsInRange(message.Sender, range);

        foreach (var recipient in recipients)
        {
            // TODO: Optimization | implement checking of obstacles in straight line, in case if no obstacles found handle message hearing instantly instead of requests to pathfinding
            var senderName = _chat.GetSenderNameInitial(message.Sender);
            var pathResult = _pathfinding.GetPath(message.Sender, recipient, range, CancellationToken.None);
            var request = new LocalSpeechRequest(message, senderName, recipient, pathResult, CancellationToken.None);
            _requests.Enqueue(request);
        }
    }

    private void SendMessageToOne(LocalSpeechMessage message, string senderName, EntityUid recipient, float fadeLevel)
    {
        var range = GetMaxRange(message.Type);
        var fadeRange = GetFadeRange(message.Type);

        var obfuscationCoefficient = 1 - Math.Min((range - fadeLevel) / (range - fadeRange), 1f); // percent how much message is faded

        // TODO: species languages, replace message.Content in obfuscatedContent with generated message and nothing more, client shouldn't be receiving original message

        var obfuscatedContent = obfuscationCoefficient > 0
            ? ObfuscateMessageReadability(message.Content, obfuscationCoefficient)
            : message.Content;
        var obfuscatedMessage = message.WithContent(obfuscatedContent);

        var hearedSenderName = _chat.GetSenderNameForRecipient(message.Sender, senderName, recipient);

        var attemptEv = new HearSpeechAttemptEvent(recipient, hearedSenderName, obfuscatedMessage);
        RaiseLocalEvent(recipient, ref attemptEv);

        if (attemptEv.Cancelled)
            return;

        var modifyEv = new ModifySpeechHearingMessageEvent(recipient, hearedSenderName, obfuscatedMessage);
        RaiseLocalEvent(recipient, ref modifyEv);

        var speechVerb = GetSpeechVerb(recipient);
        var hearEv = new HearSpeechEvent(recipient, modifyEv.SenderName, modifyEv.Speech, speechVerb);
        RaiseLocalEvent(recipient, ref hearEv);
    }

    public LocId? GetSpeechVerb(EntityUid entity)
    {
        return null;
    }

    private List<EntityUid> GetRecipientsInRange(EntityUid speaker, float range)
    {
        var ents = new List<EntityUid>();

        // We perform basic filter here because any hearing request requires
        // a lot of cpu time due to Pathfinding system

        var sXform = Transform(speaker);

        if (sXform.GridUid == null)
            return new() { speaker };

        var entsInRange = _lookup.GetEntitiesInRange<SpeechHearingComponent>(Transform(speaker).Coordinates, range);
        foreach (var ent in entsInRange)
        {
            var xform = Transform(ent);

            if (sXform.GridUid != xform.GridUid)
                continue;

            ents.Add(ent);
        }

        return ents;
    }

    private bool GetFadeLevel(EntityUid sender, EntityUid recipient, float range, List<PathPoly> path, out float fadeLevel)
    {
        fadeLevel = 0f;

        var pathfindingReached = GetFadeLevelPathfinding(sender, recipient, range, path, out var pathfindingFade);
        var straightReached = GetFadeLevelStraight(sender, recipient, range, out var straightFade);

        if (pathfindingReached && straightReached)
        {
            fadeLevel = Math.Min(pathfindingFade, straightFade);
            return true;
        }
        if (pathfindingReached)
        {
            fadeLevel = pathfindingFade;
            return true;
        }
        if (straightReached)
        {
            fadeLevel = straightFade;
            return true;
        }

        return false;
    }

    private bool GetFadeLevelPathfinding(EntityUid sender, EntityUid recipient, float range, List<PathPoly> path, out float fadeLevel)
    {
        fadeLevel = 0f;
        var sourceXform = Transform(sender);
        var destXform = Transform(recipient);

        // We are in different grids -> vacuum is blocking speech
        if (sourceXform.GridUid is not { } gridUid)
            return false;

        if (sourceXform.GridUid != destXform.GridUid)
            return false;

        if (!TryComp<MapGridComponent>(gridUid, out var grid))
            return false;

        foreach (var poly in path)
        {
            // poly.Coordinates;
            if (!GetTileFadeLevel((gridUid, grid), _transform.ToMapCoordinates(poly.Coordinates), range, out var tileFade))
            {
                fadeLevel += tileFade;
                return false;
            }
            fadeLevel += tileFade;
        }

        return true;
    }

    /// <summary>
    /// Gets fade level and checks is it more than range by straight line calculation
    /// </summary>
    /// <returns>Is speech reached to recipient</returns>
    private bool GetFadeLevelStraight(EntityUid sender, EntityUid recipient, float range, out float fadeLevel)
    {
        fadeLevel = 0f;

        var sourceXform = Transform(sender);
        var source = (Vector2i)_transform.GetWorldPosition(sourceXform);
        var destXform = Transform(recipient);
        var dest = (Vector2i)_transform.GetWorldPosition(recipient);

        // We are in different grids -> vacuum is blocking speech
        if (sourceXform.GridUid is not { } gridUid)
            return false;

        if (sourceXform.GridUid != destXform.GridUid)
            return false;

        if (!TryComp<MapGridComponent>(gridUid, out var grid))
            return false;

        var line = new GridLineEnumerator(source, dest);

        while (line.MoveNext())
        {
            if (!GetTileFadeLevel((gridUid, grid), new MapCoordinates(line.Current, sourceXform.MapID), range, out var tileFade))
            {
                fadeLevel += tileFade;
                return false;
            }
            fadeLevel += tileFade;
        }

        return true;
    }

    private bool GetTileFadeLevel(Entity<MapGridComponent> grid, MapCoordinates coordinates, float range, out float fadeLevel)
    {
        fadeLevel = 0f;
        var ents = _map.GetAnchoredEntities(grid, coordinates);
        var speechBlocking = ents.Where(HasComp<BlockSpeechComponent>);

        foreach (var ent in speechBlocking)
        {
            if (!TryComp<BlockSpeechComponent>(ent, out var blockSpeech))
                continue;

            if (blockSpeech.FullBlock)
                return false;

            fadeLevel += blockSpeech.Resistance;
        }

        // check atmosphere only if no speech-blocking tiles was found
        // cuz most of them do not have any atmosphere
        if (!speechBlocking.Any())
            fadeLevel += GetAtmosphereFadeLevel(grid, (Vector2i)coordinates.Position, range);

        if (fadeLevel >= range)
            return false; // the speech was fully blocked for this recipient

        return true;
    }

    public float GetAtmosphereFadeLevel(EntityUid gridUid, Vector2i tile, float range)
    {
        var coefficient = GetAtmosphereFadeCoefficient(gridUid, tile);

        return Math.Max(range - range * coefficient, 1f); // cannot be less than one as the normal air pressure is used as point of counting
    }

    /// <summary>
    /// Get coefficient of fading by atmosphere admitting air as an ideal sound propagation medium
    /// </summary>
    public float GetAtmosphereFadeCoefficient(EntityUid gridUid, Vector2i tile)
    {
        var mixture = _atmosphere.GetTileMixture(gridUid, null, tile);

        if (mixture == null || _atmosphere.IsTileSpace(gridUid, null, tile))
            return 0f;

        if (mixture.Pressure <= SpeechLowerThresholdPressure || mixture.Pressure >= SpeechUpperThresholdPressure)
            return 0f;

        if (mixture.Pressure < SpeechReferencePressure)
        {
            var coefficient = (mixture.Pressure - SpeechLowerThresholdPressure) / (SpeechReferencePressure - SpeechLowerThresholdPressure);
            return Math.Clamp(coefficient, 0f, 1f);
        }
        if (mixture.Pressure > SpeechReferencePressure)
        {
            var coefficient = (mixture.Pressure - SpeechReferencePressure) / (SpeechUpperThresholdPressure - SpeechReferencePressure);
            return Math.Clamp(coefficient, 0f, 1f);
        }

        return 1f;
    }

    private string ObfuscateMessageReadability(string message, float chance)
    {
        var modifiedMessage = new StringBuilder(message);

        for (var i = 0; i < message.Length; i++)
        {
            if (char.IsWhiteSpace((modifiedMessage[i])))
            {
                continue;
            }

            if (_random.Prob(1 - chance))
            {
                modifiedMessage[i] = '~';
            }
        }

        return modifiedMessage.ToString();
    }

    private float GetMaxRange(LocalSpeechType type)
    {
        return type switch
        {
            LocalSpeechType.Shout => 13f,
            LocalSpeechType.Whisper => 3f,
            _ => 10.5f,
        };
    }
    private float GetFadeRange(LocalSpeechType type)
    {
        return type switch
        {
            LocalSpeechType.Shout => 7f,
            LocalSpeechType.Whisper => 1f,
            _ => 5f,
        };
    }

    private sealed class LocalSpeechRequest
    {
        public Task Task;
        public TaskCompletionSource Tcs;
        public LocalSpeechMessage Message;
        public string SenderName;
        public EntityUid Recipient;
        public Task<PathResultEvent> PathResult;

        public EntityUid Sender => Message.Sender;
        public LocalSpeechType Type => Message.Type;

        public LocalSpeechRequest(LocalSpeechMessage message, string senderName, EntityUid recipient, Task<PathResultEvent> pathResult, CancellationToken cancelToken)
        {
            Recipient = recipient;
            Message = message;
            SenderName = senderName;
            PathResult = pathResult;

            Tcs = new TaskCompletionSource(cancelToken);
            Task = Tcs.Task;
        }
    };
}
