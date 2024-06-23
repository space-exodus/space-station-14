using Content.Shared.Actions;

namespace Content.Shared.Exodus.Abilities
{
    [RegisterComponent]
    public sealed partial class DoAbilityComponent : Component
    {
        public List<PerformingAbilityGroup> Groups = [];

        public bool SuppressActions => Groups.Count > 0;
    }

    public struct PerformingAbilityGroup
    {
        public BaseActionEvent? ExecutableEvent;

        public TimeSpan TimeEnd;

        public BaseActionEvent? Target;
    }
}
