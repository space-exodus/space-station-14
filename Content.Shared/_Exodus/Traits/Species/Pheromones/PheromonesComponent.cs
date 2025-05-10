// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Robust.Shared.GameStates;

namespace Content.Shared.Exodus.Traits.Species.Pheromones;

[RegisterComponent, NetworkedComponent]
public sealed partial class PheromonesComponent : Component
{
    [DataField]
    public Color Color = Color.Yellow;

    [DataField]
    public string Text;

    /// <summary>
    /// Whether or not this entity can be seen by those who cant read pheromones
    /// </summary>
    [DataField]
    public bool Hidden = false;

    [DataField]
    public Color? OldSpriteColor;
}
