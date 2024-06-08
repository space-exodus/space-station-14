// Exodus-GhostRolesProfilesEditor-EntireFile
using Content.Client.Eui;
using Content.Client.UserInterface.Systems.Ghost.Widgets;
using Content.Shared.Eui;
using Content.Shared.Ghost.Roles;
using Content.Shared.Humanoid.Markings;
using JetBrains.Annotations;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.Client.UserInterface.Systems.Ghost.Controls.Roles
{
    [UsedImplicitly]
    public sealed class GhostRolesHumanoidProfileEditorEui : BaseEui
    {
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly IFileDialogManager _fileDialogManager = default!;
        [Dependency] private readonly ILogManager _logManager = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly MarkingManager _markingManager = default!;
        private readonly GhostRoleHumanoidProfileEditor _window;

        public GhostRolesHumanoidProfileEditorEui()
        {
            _window = new GhostRoleHumanoidProfileEditor(_entityManager, _fileDialogManager, _logManager, _playerManager, _prototypeManager, _markingManager);

            _window.Done += () =>
            {
                _window.Close();

                SendMessage(new GhostRoleHumanoidProfileMessage(_window.Profile!));
            };

            _window.OnClose += () =>
            {
                SendMessage(new GhostRoleHumanoidProfileMessage(_window.Profile!));
                SendMessage(new CloseEuiMessage());
            };
        }

        public override void Opened()
        {
            base.Opened();
            _window.OpenCentered();
        }

        public override void Closed()
        {
            base.Closed();
            _window.Close();
        }
    }
}
