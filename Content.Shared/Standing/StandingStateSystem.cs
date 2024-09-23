using Content.Shared.ActionBlocker;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Physics;
using Content.Shared.Rotation;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared.Standing
{
    public sealed class StandingStateSystem : EntitySystem
    {
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
        [Dependency] private readonly SharedAudioSystem _audio = default!;
        [Dependency] private readonly SharedPhysicsSystem _physics = default!;
        [Dependency] private readonly MovementSpeedModifierSystem _movementSpeedModifier = default!; // Exodus-Crawling
        [Dependency] private readonly ActionBlockerSystem _actionBlocker = default!; // Exodus-Crawling
        [Dependency] private readonly PullingSystem _pulling = default!; // Exodus-Crawling

        // If StandingCollisionLayer value is ever changed to more than one layer, the logic needs to be edited.
        private const int StandingCollisionLayer = (int) CollisionGroup.MidImpassable;

        // Exodus-Crawling-Start
        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<StandingStateComponent, FootstepsSoundAttemtEvent>(OnFootstepsSound);
            SubscribeLocalEvent<StandingStateComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovementSpeedModifiersEvent);
            SubscribeLocalEvent<StandingStateComponent, DownDoAfterEvent>(OnDownDoAfterEvent);
            SubscribeLocalEvent<StandingStateComponent, StandDoAfterEvent>(OnStandDoAfterEvent);
            SubscribeLocalEvent<StandingStateComponent, PullStartedMessage>(OnPull);
            SubscribeLocalEvent<StandingStateComponent, PullStoppedMessage>(OnPull);
            SubscribeLocalEvent<StandingStateComponent, UpdateCanMoveEvent>(OnUpdateCanMove);
        }
        // Exodus-Crawling-End

        // Exodus-Crawling-Start
        private void OnFootstepsSound(EntityUid uid, StandingStateComponent component, FootstepsSoundAttemtEvent ev)
        {
            if (!component.Standing)
                return;

            ev.Cancel();
        }
        // Exodus-Crawling-End

        public bool IsDown(EntityUid uid, StandingStateComponent? standingState = null)
        {
            if (!Resolve(uid, ref standingState, false))
                return false;

            return !standingState.Standing;
        }

        // Exodus-Crawling-Start
        public bool CanCrawl(EntityUid uid, StandingStateComponent? standingState = null)
        {
            if (!Resolve(uid, ref standingState, false))
                return false;

            return standingState.CanCrawl;
        }
        // Exodus-Crawling-End

        public bool Down(EntityUid uid,
            bool playSound = true,
            bool dropHeldItems = true,
            bool force = false,
            bool canStandUp = true, // Exodus-Crawling
            StandingStateComponent? standingState = null,
            AppearanceComponent? appearance = null,
            HandsComponent? hands = null)
        {
            // TODO: This should actually log missing comps...
            if (!Resolve(uid, ref standingState, false))
                return false;

            // Optional component.
            Resolve(uid, ref appearance, ref hands, false);

            // Exodus-Crawling lines deletion

            // This is just to avoid most callers doing this manually saving boilerplate
            // 99% of the time you'll want to drop items but in some scenarios (e.g. buckling) you don't want to.
            // We do this BEFORE downing because something like buckle may be blocking downing but we want to drop hand items anyway
            // and ultimately this is just to avoid boilerplate in Down callers + keep their behavior consistent.
            if (dropHeldItems && hands != null)
            {
                RaiseLocalEvent(uid, new DropHandItemsEvent(), false);
            }

            if (!force)
            {
                // Exodus-Crawling-Start
                if (!standingState.Standing)
                    return true;
                // Exodus-Crawling-End

                var msg = new DownAttemptEvent();
                RaiseLocalEvent(uid, msg, false);

                if (msg.Cancelled)
                    return false;
            }

            standingState.Standing = false;
            // Exodus-Crawling-Start
            standingState.CanStandUp = canStandUp;
            Dirty(uid, standingState);
            // Exodus-Crawling-End

            RaiseLocalEvent(uid, new DownedEvent(), false);
            _movementSpeedModifier.RefreshMovementSpeedModifiers(uid); // Exodus-Crawling

            // Seemed like the best place to put it
            _appearance.SetData(uid, RotationVisuals.RotationState, RotationState.Horizontal, appearance);

            // Change collision masks to allow going under certain entities like flaps and tables
            if (TryComp(uid, out FixturesComponent? fixtureComponent))
            {
                foreach (var (key, fixture) in fixtureComponent.Fixtures)
                {
                    if ((fixture.CollisionMask & StandingCollisionLayer) == 0)
                        continue;

                    standingState.ChangedFixtures.Add(key);
                    _physics.SetCollisionMask(uid, key, fixture, fixture.CollisionMask & ~StandingCollisionLayer, manager: fixtureComponent);
                }
            }

            // check if component was just added or streamed to client
            // if true, no need to play sound - mob was down before player could seen that
            if (standingState.LifeStage <= ComponentLifeStage.Starting)
                return true;

            if (playSound)
            {
                _audio.PlayPredicted(standingState.DownSound, uid, uid);
            }

            return true;
        }

        public bool Stand(EntityUid uid,
            StandingStateComponent? standingState = null,
            AppearanceComponent? appearance = null,
            bool force = false)
        {
            // TODO: This should actually log missing comps...
            if (!Resolve(uid, ref standingState, false))
                return false;

            // Optional component.
            Resolve(uid, ref appearance, false);

            if (standingState.Standing)
                return true;

            if (!force)
            {
                var msg = new StandAttemptEvent();
                RaiseLocalEvent(uid, msg, false);

                if (msg.Cancelled)
                    return false;
            }

            standingState.Standing = true;
            Dirty(uid, standingState);

            // Exodus-Crawling-Start
            // need to refresh movement input for proper handling of standing state update, waddling for example
            if (TryComp<InputMoverComponent>(uid, out var input))
            {
                var moveInputEvent = new MoveInputEvent((uid, input), input.HeldMoveButtons);
                RaiseLocalEvent(uid, ref moveInputEvent, false);
            }
            // Exodus-Crawling-End

            RaiseLocalEvent(uid, new StoodEvent(), false);
            _movementSpeedModifier.RefreshMovementSpeedModifiers(uid); // Exodus-Crawling
            _actionBlocker.UpdateCanMove(uid); // Exodus-Crawling

            _appearance.SetData(uid, RotationVisuals.RotationState, RotationState.Vertical, appearance);

            if (TryComp(uid, out FixturesComponent? fixtureComponent))
            {
                foreach (var key in standingState.ChangedFixtures)
                {
                    if (fixtureComponent.Fixtures.TryGetValue(key, out var fixture))
                        _physics.SetCollisionMask(uid, key, fixture, fixture.CollisionMask | StandingCollisionLayer, fixtureComponent);
                }
            }
            standingState.ChangedFixtures.Clear();

            return true;
        }

        // Exodus-Crawling-Start
        public void SetCanStandUp(EntityUid uid, bool canStandUp, StandingStateComponent? standing = null)
        {
            if (!Resolve(uid, ref standing, true))
                return;

            standing.CanStandUp = canStandUp;
            Dirty(uid, standing);
        }

        private void OnStandDoAfterEvent(EntityUid uid, StandingStateComponent standing, ref StandDoAfterEvent ev)
        {
            if (ev.Cancelled)
                return;

            Stand(uid, standingState: standing);
        }

        private void OnDownDoAfterEvent(EntityUid uid, StandingStateComponent standing, ref DownDoAfterEvent ev)
        {
            if (ev.Cancelled)
                return;

            Down(uid, standingState: standing);
        }

        private void OnRefreshMovementSpeedModifiersEvent(EntityUid uid, StandingStateComponent standing, ref RefreshMovementSpeedModifiersEvent ev)
        {
            if (standing.Standing)
                return;

            ev.ModifySpeed(standing.CrawlingSpeedModifier, standing.CrawlingSpeedModifier);
        }

        private void OnPull(EntityUid uid, StandingStateComponent standing, ref PullStartedMessage ev)
        {
            _actionBlocker.UpdateCanMove(uid);
        }
        private void OnPull(EntityUid uid, StandingStateComponent standing, ref PullStoppedMessage ev)
        {
            _actionBlocker.UpdateCanMove(uid);
        }

        private void OnUpdateCanMove(EntityUid uid, StandingStateComponent standing, ref UpdateCanMoveEvent ev)
        {
            if (ev.Cancelled)
                return;

            if (!standing.Standing && _pulling.IsPulled(uid))
                ev.Cancel();
        }
        // Exodus-Crawling-End
    }

    public sealed class DropHandItemsEvent : EventArgs
    {
    }

    /// <summary>
    /// Subscribe if you can potentially block a down attempt.
    /// </summary>
    public sealed class DownAttemptEvent : CancellableEntityEventArgs
    {
    }

    /// <summary>
    /// Subscribe if you can potentially block a stand attempt.
    /// </summary>
    public sealed class StandAttemptEvent : CancellableEntityEventArgs
    {
    }

    /// <summary>
    /// Raised when an entity becomes standing
    /// </summary>
    public sealed class StoodEvent : EntityEventArgs
    {
    }

    /// <summary>
    /// Raised when an entity is not standing
    /// </summary>
    public sealed class DownedEvent : EntityEventArgs
    {
    }

    // Exodus-Crawling-Start
    [Serializable, NetSerializable]
    public sealed partial class DownDoAfterEvent : SimpleDoAfterEvent
    {
    }

    [Serializable, NetSerializable]
    public sealed partial class StandDoAfterEvent : SimpleDoAfterEvent
    {
    }
    // Exodus-Crawling-End
}
