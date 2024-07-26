using Content.Shared.Whitelist;  // Exodus-FoldedPoster
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Engineering.Components
{
    [RegisterComponent]
    public sealed partial class SpawnAfterInteractComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite)]  // Exodus-FoldedPoster
        [DataField("prototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string? Prototype { get; set; } // Exodus-FoldedPoster - some systems, like WallPosterSystem, overwrite that

        [ViewVariables(VVAccess.ReadWrite)]  // Exodus-FoldedPoster
        [DataField("ignoreDistance")]
        public bool IgnoreDistance { get; private set; }

        [ViewVariables(VVAccess.ReadWrite)]  // Exodus-FoldedPoster
        [DataField("doAfter")]
        public float DoAfterTime = 0;

        // Exodus-FoldedPoster-Start
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("needClearTile")]
        public bool NeedClearTile = true;

        /// <summary>
        ///     Used to determine whether or not interaction is avaible for current entity
        /// </summary>

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("useWhitelist")]
        public bool UseWhitelist = false;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("whitelist")]
        public EntityWhitelist Whitelist = new();

        [ViewVariables(VVAccess.ReadWrite)]
        // Exodus-FoldedPoster-End
        [DataField("removeOnInteract")]
        public bool RemoveOnInteract = false;
    }
}
