// © Space Exodus, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/space-exodus/space-station-14/master/CLA.txt

namespace Content.Client.Exodus.UserInterface.Controls;

/*

Меню:
- Слой 1. Buttons; ContextualButtons;
- Слой 2. Buttons; ContextualButtons;

abstract class RadialMenuButton
{
    private RadialMenuController _radialMenu;

    abstract void OnClick();
}

class RadialMenuController
{
    Dictionary<string, RadialMenuLayer> LayersSchema { get; private set; }
    Dictionary<string, RadialMenuLayer> LayersState { get; private set; }

    string CurrentLayerKey { get; private set; }

    RadialMenuLayer CurrentLayer => LayersState[CurrentLayerKey];

    RadialMenuButton Buttons => CurrentLayer.Buttons;
    RadialMenuButton ContextualButtons => CurrentLayer.ContextualButtons;
    DisplayMode DisplayMode => CurrentLayer.DisplayMode;

    bool IsRadialMenuOpen;

    bool TryOpenRadialMenu(RadialMenu menu);
    void CloseRadialMenu(RadialMenu menu);

    // Current Radial Menu Control
    AddButton();
    AddButtons();

    AddContextualButton();
    AddContextualButtons();

    MoveToLayer(int index, bool resetState = false);
}

class RadialMenuLayer
{
    Buttons;
    ContextualButtons;
    DisplayMode;
}

enum DisplayMode
{
    Sectors,
    Floating
}

class RadialMenu
{
    Dictionary<string, RadialMenuLayer> Layers;
    DisplayMode DisplayMode;
}

*/
