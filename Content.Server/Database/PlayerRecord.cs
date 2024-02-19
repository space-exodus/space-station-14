using System.Collections.Immutable;
using System.Net;
using Robust.Shared.Network;

namespace Content.Server.Database
{
    public sealed class PlayerRecord
    {
        public NetUserId UserId { get; }
        public ImmutableArray<byte>? HWId { get; }
        public DateTimeOffset FirstSeenTime { get; }
        public string LastSeenUserName { get; }
        public DateTimeOffset LastSeenTime { get; }
        public IPAddress LastSeenAddress { get; }
        public ulong? DiscordId { get; } // Exodus-Discord
        public bool IsPremium { get; } // Exodus-Sponsorship
        public string? PremiumOOCColor { get; } // Exodus-Sponsorship

        public PlayerRecord(
            NetUserId userId,
            DateTimeOffset firstSeenTime,
            string lastSeenUserName,
            DateTimeOffset lastSeenTime,
            IPAddress lastSeenAddress,
            ImmutableArray<byte>? hwId,
            ulong? discordId, // Exodus-Discord
            bool isPremium, // Exodus-Sponsorship
            string? premiumOOCColor) // Exodus-Sponsorship
        {
            UserId = userId;
            FirstSeenTime = firstSeenTime;
            LastSeenUserName = lastSeenUserName;
            LastSeenTime = lastSeenTime;
            LastSeenAddress = lastSeenAddress;
            HWId = hwId;
            DiscordId = discordId; // Exodus-Discord
            IsPremium = isPremium; // Exodus-Sponsorship
            PremiumOOCColor = premiumOOCColor; // Exodus-Sponsorship
        }
    }
}
