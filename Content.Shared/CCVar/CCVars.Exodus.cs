using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    public static readonly CVarDef<bool> DiscordVerificationEnabled =
        CVarDef.Create("discord.verification_enabled", false, CVar.SERVERONLY);

    public static readonly CVarDef<string> DiscordBanWebhook =
        CVarDef.Create("discord.ban_webhook", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);

    /// <summary>
    ///     If enabled automatically creates preset and map votes when round restarts
    /// </summary>
    public static readonly CVarDef<bool> VoteAutoVoteEnabled =
        CVarDef.Create("vote.auto_vote_enabled", false, CVar.SERVERONLY);
}
