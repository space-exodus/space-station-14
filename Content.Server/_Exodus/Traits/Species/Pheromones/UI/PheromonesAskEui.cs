// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Server.EUI;
using Content.Shared.Eui;
using Content.Shared.Exodus.Traits.Species.Pheromones.UI;
using Robust.Shared.Map;

namespace Content.Server.Exodus.Traits.Species.Pheromones;

public sealed partial class PheromonesAskEui : BaseEui
{
    private PheromonesSystem _pheromones;
    public EntityUid? Target { get; }
    public EntityCoordinates? Coordinates { get; }

    public PheromonesAskEui(PheromonesSystem pheromones, EntityUid? target, EntityCoordinates? coords) : base()
    {
        _pheromones = pheromones;
        Target = target;
        Coordinates = coords;
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (msg is not PheromonesAskEuiConfirmMessage ask)
            return;

        if (Player.AttachedEntity == null)
            return;

        Close();
        var text = ask.Text.Substring(0, Math.Min(ask.Text.Length, 256));

        if (Target != null && Target.Value.IsValid())
        {
            if (!_pheromones.ValidateCanMark(Player.AttachedEntity.Value, Target.Value))
                return;

            _pheromones.MarkTargetWithPheromones(Target.Value, Player.AttachedEntity.Value, text);
        }
        else if (Coordinates != null)
        {
            if (!_pheromones.ValidateCanMark(Player.AttachedEntity.Value, Coordinates.Value))
                return;

            _pheromones.MarkCoordsWithPheromones(Coordinates.Value, Player.AttachedEntity.Value, text);
        }
    }
}
