using System.Diagnostics.CodeAnalysis;

namespace Content.Shared.Elite.EliteSpace;

/// <summary>
/// The core system controlling three game space layers: Sector map, Star system map, Local space
/// </summary>
public abstract partial class SharedEliteSpaceSystem : EntitySystem
{
    public bool TryGetStarMap([NotNullWhen(true)] out EliteSectorMapComponent? map)
    {
        var query = EntityQueryEnumerator<EliteSectorMapComponent>();
        return query.MoveNext(out map);
    }
}
