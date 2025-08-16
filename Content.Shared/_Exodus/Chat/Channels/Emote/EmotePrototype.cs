// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Exodus.Chat.Channels.Emote.Animation;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

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

    [DataField(required: true)]
    public string Name = default!;

    [DataField]
    public EmoteCategory Category = EmoteCategory.Unique;

    [DataField]
    public SpriteSpecifier Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/Actions/scream.png"));

    [DataField]
    public EntityWhitelist? Whitelist;

    [DataField]
    public EntityWhitelist? Blacklist;

    [DataField]
    public HashSet<string> Triggers = new();

    [DataField]
    public EmoteAnimation? Animation;
}

[Flags]
[Serializable, NetSerializable]
public enum EmoteCategory : byte
{
    Invalid = 0,
    Vocal = 1 << 0,
    NonVocal = 1 << 1,
    Unique = 1 << 2,
}
