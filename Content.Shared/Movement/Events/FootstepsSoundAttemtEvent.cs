namespace Content.Shared.Movement.Events
{
    public sealed class FootstepsSoundAttemptEvent(EntityUid uid) : CancellableEntityEventArgs
    {
        public EntityUid Uid { get; } = uid;
    }
}
