using Content.Shared.Actions;

namespace Content.Shared.Mobs;

/// <summary>
///     Only applies to mobs in crit capable of ghosting/succumbing
/// </summary>
public sealed partial class CritSuccumbEvent : InstantActionEvent
{
}

/// <summary>
///     Only applies/has functionality to mobs in crit that have <see cref="DeathgaspComponent"/>
/// </summary>
public sealed partial class CritFakeDeathEvent : InstantActionEvent
{
}

// Exodus-CritSpeech-LinesRemoval | Remove Crit Last Words Action
