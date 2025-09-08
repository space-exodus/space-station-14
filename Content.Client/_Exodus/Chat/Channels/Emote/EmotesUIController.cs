using Content.Client.Exodus.UserInterface.Controls;
using Content.Client.Gameplay;
using Content.Shared.Input;
using Content.Shared.Whitelist;
using JetBrains.Annotations;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input.Binding;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Client.GameObjects;
using Content.Shared.Exodus.Chat.Channels.Emote;

using MenuBar = Content.Client.UserInterface.Systems.MenuBar;

namespace Content.Client.Exodus.Chat.Channels.Emote;

[UsedImplicitly]
public sealed class EmotesUIController : UIController, IOnStateChanged<GameplayState>
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [UISystemDependency] private readonly SpriteSystem _sprite = default!;
    [UISystemDependency] private readonly EmoteSystem _emote = default!;

    private MenuButton? EmotesButton => UIManager.GetActiveUIWidgetOrNull<MenuBar.Widgets.GameTopMenuBar>()?.EmotesButton;
    private HealthyRadialMenu? _menu;

    private static readonly Dictionary<EmoteCategory, (string Tooltip, SpriteSpecifier Sprite)> EmoteGroupingInfo = new Dictionary<EmoteCategory, (string Tooltip, SpriteSpecifier Sprite)>
    {
        [EmoteCategory.Unique] = ("emote-menu-category-unique", new SpriteSpecifier.Texture(new ResPath("/Textures/Clothing/Hands/Gloves/latex.rsi/icon.png"))),
        [EmoteCategory.Vocal] = ("emote-menu-category-vocal", new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/Emotes/vocal.png"))),
        [EmoteCategory.NonVocal] = ("emote-menu-category-nonvocal", new SpriteSpecifier.Texture(new ResPath("/Textures/Clothing/Head/Soft/mimesoft.rsi/icon.png"))),
    };

    public void OnStateEntered(GameplayState state)
    {
        CommandBinds.Builder
            .Bind(ContentKeyFunctions.OpenEmotesMenu,
                InputCmdHandler.FromDelegate(_ => ToggleEmotesMenu(false)))
            .Register<EmotesUIController>();
    }

    public void OnStateExited(GameplayState state)
    {
        CommandBinds.Unregister<EmotesUIController>();
    }

    private void ToggleEmotesMenu(bool centered)
    {
        var player = _playerManager.LocalSession?.AttachedEntity;

        if (_menu == null || player == null)
        {
            // setup window
            var emotes = _emote.GetAvailableEmotes();
            var models = ConvertToButtons(emotes);

            _menu = new HealthyRadialMenu();
            _menu.SetButtons(models);

            _menu.Open();

            _menu.OnClose += OnWindowClosed;
            _menu.OnOpen += OnWindowOpen;

            if (EmotesButton != null)
                EmotesButton.SetClickPressed(true);

            if (centered)
            {
                _menu.OpenCentered();
            }
            else
            {
                _menu.OpenOverMouseScreenPosition();
            }
        }
        else
        {
            _menu.OnClose -= OnWindowClosed;
            _menu.OnOpen -= OnWindowOpen;

            if (EmotesButton != null)
                EmotesButton.SetClickPressed(false);

            CloseMenu();
        }
    }

    public void UnloadButton()
    {
        if (EmotesButton == null)
            return;

        EmotesButton.OnPressed -= ActionButtonPressed;
    }

    public void LoadButton()
    {
        if (EmotesButton == null)
            return;

        EmotesButton.OnPressed += ActionButtonPressed;
    }

    private void ActionButtonPressed(BaseButton.ButtonEventArgs args)
    {
        ToggleEmotesMenu(true);
    }

    private void OnWindowClosed()
    {
        if (EmotesButton != null)
            EmotesButton.Pressed = false;

        CloseMenu();
    }

    private void OnWindowOpen()
    {
        if (EmotesButton != null)
            EmotesButton.Pressed = true;
    }

    private void CloseMenu()
    {
        if (_menu == null)
            return;

        if (_menu.IsOpen)
            _menu.Close();

        _menu = null;
    }

    private IEnumerable<HealthyRadialMenuOption> ConvertToButtons(IEnumerable<EmotePrototype> emotePrototypes)
    {
        var whitelistSystem = EntitySystemManager.GetEntitySystem<EntityWhitelistSystem>();

        Dictionary<EmoteCategory, List<HealthyRadialMenuOption>> emotesByCategory = new();
        foreach (var emote in emotePrototypes)
        {
            if (emote.Category == EmoteCategory.Invalid)
                continue;

            if (!emotesByCategory.TryGetValue(emote.Category, out var list))
            {
                list = new List<HealthyRadialMenuOption>();
                emotesByCategory.Add(emote.Category, list);
            }

            var actionOption = new HealthyRadialMenuActionOption<EmotePrototype>(HandleRadialButtonClick, emote)
            {
                Children = [
                    new TextureRect() {
                        Texture = _sprite.Frame0(emote.Icon),
                    },
                ],
                ToolTip = Loc.GetString(emote.Name)
            };
            list.Add(actionOption);
        }

        var models = new HealthyRadialMenuOption[emotesByCategory.Count];
        var i = 0;
        foreach (var (key, list) in emotesByCategory)
        {
            var tuple = EmoteGroupingInfo[key];

            models[i] = new HealthyRadialMenuNestedLayerOption(list)
            {
                Children = [
                    new TextureRect() {
                        Texture = _sprite.Frame0(tuple.Sprite),
                    },
                ],
                ToolTip = Loc.GetString(tuple.Tooltip)
            };
            i++;
        }

        return models;
    }

    private void HandleRadialButtonClick(EmotePrototype prototype)
    {
        _emote.PlayEmoteMessage(prototype);
    }
}
