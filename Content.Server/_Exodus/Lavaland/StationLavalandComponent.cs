using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;

namespace Content.Server.Exodus.Lavaland;

/**
  * Component which is added to stations that has access to lavaland
  * If lavaland enabled in serer config then will generate it if it wasn't
  * before
  */
[RegisterComponent]
public sealed partial class StationLavalandComponent : Component
{
}
