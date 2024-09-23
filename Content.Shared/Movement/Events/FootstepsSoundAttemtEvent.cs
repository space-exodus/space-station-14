namespace Content.Shared.Movement.Events
{
    public sealed class FootstepsSoundAttemtEvent(EntityUid uid) : CancellableEntityEventArgs
    {
        public EntityUid Uid { get; } = uid;
    }
}
