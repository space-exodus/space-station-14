// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Exodus.Traits.Species.Pheromones.UI;

[Serializable, NetSerializable]
public sealed partial class PheromonesAskEuiConfirmMessage : EuiMessageBase
{
    public string Text;

    public PheromonesAskEuiConfirmMessage(string text)
    {
        Text = text;
    }
}
