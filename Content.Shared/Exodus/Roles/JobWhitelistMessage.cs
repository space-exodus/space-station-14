using System.IO;
using System.Text.Json.Serialization;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Roles;

[Serializable, NetSerializable]
public sealed class RoleWhitelistInfo
{
    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = null!;

    [JsonPropertyName("rolesGroups")]
    public List<string> RolesGroups { get; set; } = null!;
}

public sealed class MsgRoleWhitelistInfo : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public RoleWhitelistInfo? Info;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        var isSponsor = buffer.ReadBoolean();
        buffer.ReadPadBits();
        if (!isSponsor) return;
        var length = buffer.ReadVariableInt32();
        using var stream = new MemoryStream(length);
        buffer.ReadAlignedMemory(stream, length);
        serializer.DeserializeDirect(stream, out Info);
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(Info != null);
        buffer.WritePadBits();
        if (Info == null) return;
        var stream = new MemoryStream();
        serializer.SerializeDirect(stream, Info);
        buffer.WriteVariableInt32((int) stream.Length);
        buffer.Write(stream.AsSpan());
    }
}
