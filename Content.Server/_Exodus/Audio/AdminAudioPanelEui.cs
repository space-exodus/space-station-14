using Content.Server.Administration.Managers;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Content.Shared.Exodus.Audio;
using Robust.Server.Player;
using Robust.Shared.Enums;
using Robust.Shared.Player;

namespace Content.Server.Exodus.Audio;

public sealed partial class AdminAudioPanelEui : BaseEui
{
    [Dependency] private readonly IAdminManager _adminManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    private Dictionary<Guid, string> _availablePlayers = new();

    private readonly AdminAudioPanelSystem _audioPanel;

    public AdminAudioPanelEui() : base()
    {
        IoCManager.InjectDependencies(this);

        _audioPanel = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<AdminAudioPanelSystem>();

        foreach (var player in Filter.Broadcast().Recipients)
        {
            _availablePlayers.Add(player.UserId.UserId, player.Name);
        }

        _playerManager.PlayerStatusChanged += (object? sender, SessionStatusEventArgs args) =>
        {
            switch (args.NewStatus)
            {
                case SessionStatus.InGame:
                    _availablePlayers.Add(args.Session.UserId.UserId, args.Session.Name);
                    StateDirty();
                    break;
                case SessionStatus.Disconnected:
                    _availablePlayers.Remove(args.Session.UserId.UserId);
                    StateDirty();
                    break;
            }
        };
    }

    public override void Opened()
    {
        StateDirty();
    }

    public override AdminAudioPanelEuiState GetNewState()
    {
        return new(
            _audioPanel.Playing,
            _audioPanel.Queue,
            _audioPanel.AudioParams.Volume,
            (float)_audioPanel.CurrentTrackLength.TotalSeconds,
            _audioPanel.CurrentTrackPlaybackPosition,
            _audioPanel.Global,
            _availablePlayers,
            _audioPanel.SelectedPlayers
        );
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (msg is not AdminAudioPanelEuiMessage.AdminAudioPanelEuiMessageBase)
            return;

        if (!_adminManager.HasAdminFlag(Player, AdminFlags.Fun))
        {
            Close();
            return;
        }

        switch (msg)
        {
            case AdminAudioPanelEuiMessage.Play:
                _audioPanel.Play();
                break;
            case AdminAudioPanelEuiMessage.Pause:
                _audioPanel.Pause();
                break;
            case AdminAudioPanelEuiMessage.Stop:
                _audioPanel.Stop();
                break;
            case AdminAudioPanelEuiMessage.AddTrack addTrack:
                _audioPanel.AddToQueue(addTrack.Filename);
                break;
            case AdminAudioPanelEuiMessage.SetVolume setVolume:
                _audioPanel.SetVolume(setVolume.Volume);
                break;
            case AdminAudioPanelEuiMessage.SetPlaybackPosition setPlayback:
                _audioPanel.SetPlaybackPosition(setPlayback.Position);
                break;
            case AdminAudioPanelEuiMessage.SelectPlayer selectPlayer:
                _audioPanel.SelectPlayer(selectPlayer.Player);
                break;
            case AdminAudioPanelEuiMessage.UnselectPlayer unselectPlayer:
                _audioPanel.UnselectPlayer(unselectPlayer.Player);
                break;
            default:
                return;
        }
        StateDirty();
    }
}
