using Robust.Shared.Configuration;

namespace Content.Shared.Corvax.CCCVars;

/// <summary>
///     Corvax modules console variables
/// </summary>
[CVarDefs]
// ReSharper disable once InconsistentNaming
public sealed class CCCVars
{
    /*
     * Queue
     */

    /// <summary>
    ///     Controls if the connections queue is enabled. If enabled stop kicking new players after `SoftMaxPlayers` cap and instead add them to queue.
    /// </summary>
    public static readonly CVarDef<bool>
        QueueEnabled = CVarDef.Create("queue.enabled", false, CVar.SERVERONLY);

    // Exodus-DisableStationGoal-Start

    /**
     * Station goal
     */

    /// <summary>
    ///     Controls if round start station goal sending is enabled. `sendstationgoal` command still works even if this value set to false
    /// </summary>
    public static readonly CVarDef<bool>
        StationGoalEnabled = CVarDef.Create("game.station_goal", true, CVar.SERVERONLY);

    // Exodus-DisableStationGoal-End
}
