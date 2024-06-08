// Exodus-GhostRolesProfilesEditor-EntireFile
using Content.Server.EUI;
using Content.Shared.Eui;
using Content.Shared.Ghost.Roles;

namespace Content.Server.Ghost.Roles.UI
{
    public sealed class GhostRolesHumanoidProfileEditorEui : BaseEui
    {
        private readonly GhostRoleSystem _ghostRoleSystem;

        public GhostRolesHumanoidProfileEditorEui()
        {
            _ghostRoleSystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<GhostRoleSystem>();
        }

        public override void HandleMessage(EuiMessageBase msg)
        {
            base.HandleMessage(msg);

            switch (msg)
            {
                case GhostRoleHumanoidProfileMessage profile:
                    _ghostRoleSystem.UpdateHumanoidApperance(Player, profile.Profile);
                    break;
            }
        }
    }
}
