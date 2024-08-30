using System.Diagnostics.CodeAnalysis;
using Content.Shared.Exodus.Sponsors;
using Robust.Shared.Network;

namespace Content.Client.Exodus.Sponsors;

public sealed class SponsorsManager
{
    [Dependency] private readonly IClientNetManager _net = default!;
    private SponsorInfo? _info;

    public void Initialize()
    {
        _net.RegisterNetMessage<MsgSponsorInfo>(OnMessage);
    }

    public void OnMessage(MsgSponsorInfo message)
    {
        _info = message.Info;
    }

    public bool TryGetInfo([NotNullWhen(true)] out SponsorInfo? sponsor)
    {
        sponsor = _info;
        return _info != null;
    }
}
