using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Shared.Exodus.Sponsors;
using Robust.Shared.Network;
using System.Linq;

namespace Content.Server.Exodus.Sponsors;

public sealed class SponsorsManager
{
    [Dependency] private readonly IServerNetManager _net = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    private readonly Dictionary<NetUserId, SponsorInfo> _connectedSponsors = new();

    public void Initialize()
    {
        _net.RegisterNetMessage<MsgSponsorInfo>();

        _net.Connecting += OnConnecting;
        _net.Connected += OnConnected;
        _net.Disconnect += OnDisconnect;
    }

    public bool TryGetInfo(NetUserId userId, [NotNullWhen(true)] out SponsorInfo? sponsor)
    {
        return _connectedSponsors.TryGetValue(userId, out sponsor);
    }

    private async Task OnConnecting(NetConnectingArgs e)
    {
        var info = await GetSponsorInfo(e.UserId);

        if (info == null) return;

        Debug.Assert(!_connectedSponsors.ContainsKey(e.UserId), "Found cached sponsor on player connection");

        _connectedSponsors.Add(e.UserId, info);
    }

    private async Task<SponsorInfo?> GetSponsorInfo(NetUserId userId)
    {
        var playerRecord = await _db.GetPlayerRecordByUserId(userId);
        if (playerRecord == null || !playerRecord.IsPremium) return null;

        return new()
        {
            OOCColor = playerRecord.PremiumOOCColor
        };
    }

    public async void PromoteSponsor(NetUserId userId)
    {
        var existingInfo = await GetSponsorInfo(userId);

        Debug.Assert(existingInfo == null, "Tried to promote to sponsor player which is already a sponsor");

        await _db.PromoteSponsor(userId);

        var info = await GetSponsorInfo(userId);

        Debug.Assert(info != null, "Didn't got any sponsor info while promoting sponsor after database update");

        var channel = _net.Channels.First((channel) => channel.UserId == userId);

        if (channel == null)
            return;

        _connectedSponsors.Add(userId, info);
        var msg = new MsgSponsorInfo() { Info = info };
        _net.ServerSendMessage(msg, channel);
    }

    private void OnConnected(object? sender, NetChannelArgs e)
    {
        var info = _connectedSponsors.TryGetValue(e.Channel.UserId, out var sponsor) ? sponsor : null;
        var msg = new MsgSponsorInfo() { Info = info };
        _net.ServerSendMessage(msg, e.Channel);
    }

    private void OnDisconnect(object? sender, NetDisconnectedArgs e)
    {
        _connectedSponsors.Remove(e.Channel.UserId);
    }
}
