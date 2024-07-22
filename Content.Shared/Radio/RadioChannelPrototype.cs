using Robust.Shared.Prototypes;

namespace Content.Shared.Radio;

[Prototype("radioChannel")]
public sealed partial class RadioChannelPrototype : IPrototype
{
    /// <summary>
    /// Human-readable name for the channel.
    /// </summary>
    [DataField("name")]
    public LocId Name { get; private set; } = string.Empty;

    [ViewVariables(VVAccess.ReadOnly)]
    public string LocalizedName => Loc.GetString(Name);

    /// <summary>
    /// Single-character prefix to determine what channel a message should be sent to.
    /// </summary>
    [DataField("keycode")]
    public char KeyCode { get; private set; } = '\0';

    [DataField("frequency")]
    public int Frequency { get; private set; } = 0;

    [DataField("color")]
    public Color Color { get; private set; } = Color.Lime;

    [IdDataField, ViewVariables]
    public string ID { get; } = default!;

    /// <summary>
    /// If channel is long range it doesn't require telecommunication server
    /// and messages can be sent across different stations
    /// </summary>
    [DataField("longRange"), ViewVariables]
    public bool LongRange = false;

    // Exodus-LocalizedChannels-Start
    /// <summary>
    /// If channel is localized it doesn't require telecommunitcation server
    /// but messages can be received only in specific range from message source.
    /// Cannot be affected by solar flare.
    /// See also <seealso cref="Range"/>
    /// </summary>
    [DataField("localized"), ViewVariables]
    public bool Localized = false;

    /// <summary>
    /// In which range from message source it can be received.
    /// Ignored when <see cref="Localized"/> is false.
    /// </summary>
    [DataField("range"), ViewVariables]
    public int Range = 300;
    // Exodus-LocalizedChannels-End
}
