namespace Content.Server.Exodus.FTLKey;

[RegisterComponent]
public sealed partial class FTLKeyComponent : Component
{
    [DataField("access"), ViewVariables(VVAccess.ReadWrite)]
    public List<string>? FTLKeys = [];
}
