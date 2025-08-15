// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat.Channels.Emote.Animation;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;

namespace Content.Shared.Exodus.Chat.Channels.Emote;

[Prototype("emote")]
public sealed partial class EmotePrototype : IPrototype, IInheritingPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [ParentDataField(typeof(EmotePrototype))]
    public string[]? Parents { get; private set; }

    [AbstractDataField]
    public bool Abstract { get; private set; }

    [DataField]
    public EntityWhitelist? Whitelist;

    [DataField]
    public EntityWhitelist? Blacklist;

    [DataField]
    public HashSet<string> Triggers = new();

    [DataField]
    public EmoteAnimation? Animation;
}
