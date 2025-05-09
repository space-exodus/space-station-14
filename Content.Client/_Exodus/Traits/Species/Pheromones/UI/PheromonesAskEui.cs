// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

using Content.Client.Eui;
using Content.Client.Exodus.Traits.Species.Pheromones.UI;
using Content.Shared.Exodus.Traits.Species.Pheromones.UI;

namespace Content.Client.Exodus.Traits.Species.Pheromones;

public sealed partial class PheromonesAskEui : BaseEui
{
    private PheromonesAskWindow? _window = null;

    public override void Opened()
    {
        base.Opened();
        Close();

        _window = new PheromonesAskWindow();
        _window.OnPheromonesTextSent += (text) =>
        {
            SendMessage(new PheromonesAskEuiConfirmMessage(text));
        };
        _window.OpenCentered();
    }

    public void Close()
    {
        if (_window != null)
        {
            _window.Close();
            _window = null;
        }
    }

    public override void Closed()
    {
        base.Closed();
        Close();
    }
}
