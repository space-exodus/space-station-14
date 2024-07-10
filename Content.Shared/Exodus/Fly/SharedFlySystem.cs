using System.Numerics;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Foldable;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Exodus.Fly;

/// <summary>
/// Provides extraction devices that teleports the attached entity after <see cref="FultonDuration"/> elapses to the linked beacon.
/// </summary>
public abstract partial class SharedFlySystem : EntitySystem
{
    [Dependency] protected readonly IGameTiming Timing = default!;
    [Dependency] protected readonly SharedTransformSystem TransformSystem = default!;
    [Dependency] protected readonly SharedAudioSystem Audio = default!;
    [Dependency] protected readonly SharedContainerSystem Container = default!;

    public override void Initialize()
    {
        base.Initialize();


    }

    protected bool CanTakeoff(EntityUid uid, FlyComponent? comp = null)
    {
        if (!Resolve(uid, ref comp))
            return false;

        if (Container.IsEntityInContainer(uid) ||
            comp.DoAnimation)
            return false;

        var xform = Transform(uid);

        if (xform.Anchored)
            return false;

        return true;
    }

    protected bool CanLand(EntityUid uid, FlyComponent? comp = null)
    {
        if (!Resolve(uid, ref comp) ||
            comp.DoAnimation)
            return false;

        return true;
    }


    [Serializable, NetSerializable]
    protected sealed class LandAnimationMessage : EntityEventArgs
    {
        public NetEntity Entity;
    }

    [Serializable, NetSerializable]
    protected sealed class TakeoffAnimationMessage : EntityEventArgs
    {
        public NetEntity Entity;
    }

    [Serializable, NetSerializable]
    protected sealed class LandMessage : EntityEventArgs
    {
        public NetEntity Entity;
    }

    [Serializable, NetSerializable]
    protected sealed class TakeoffMessage : EntityEventArgs
    {
        public NetEntity Entity;
    }
}
