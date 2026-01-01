using Robust.Shared.GameStates;

namespace Content.Shared.Elite.EliteSpace;

/// <summary>
/// The main data store for Elite Space System, shouldn't be existing more than one at once
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class EliteSpaceComponent : Component
{
}
