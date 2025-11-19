// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using System.IO;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Exodus.Chat;

// #RobustEngineAPIisHorrible
// I don't like the decion of adding another wrapper class but it's the best way to not uglify chat system api

[Serializable, NetSerializable]
public sealed partial class ChatClientNetMessage : NetMessage
{
    public BaseChatClientMessage? Message;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        var length = buffer.ReadVariableInt32();
        using var stream = new MemoryStream(length);
        buffer.ReadAlignedMemory(stream, length);
        serializer.DeserializeDirect(stream, out Message);
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        var stream = new MemoryStream();
        serializer.SerializeDirect(stream, Message);
        buffer.WriteVariableInt32((int)stream.Length);
        buffer.Write(stream.AsSpan());
    }
}
