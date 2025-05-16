namespace Content.Shared.Speech
{
    public sealed class SpeakAttemptEvent : CancellableEntityEventArgs
    {
        public SpeakAttemptEvent(EntityUid uid, bool intrinsic = false) // Exodus-Kidans | Add intrinsic arg
        {
            Uid = uid;
            Intrinsic = intrinsic; // Exodus-Kidans
        }

        public EntityUid Uid { get; }
        public bool Intrinsic { get; } // Exodus-Kidans
    }
}
