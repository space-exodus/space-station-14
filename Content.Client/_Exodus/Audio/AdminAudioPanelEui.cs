using Content.Client.Eui;
using Content.Client.Exodus.Audio.Widgets;
using Content.Shared.Eui;
using Content.Shared.Exodus.Audio;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Utility;

namespace Content.Client.Exodus.Audio;

public sealed partial class AdminAudioPanelEui : BaseEui
{
    private AdminAudioPanelEuiState? _state = null;
    private AdminAudioPanel? _adminAudioPanel = null;

    public AdminAudioPanelEui() : base()
    {
        IoCManager.InjectDependencies(this);
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is AdminAudioPanelEuiState adminAudioPanelState)
        {
            _state = adminAudioPanelState;
            UpdateUI();
        }
    }

    public override void Opened()
    {
        _adminAudioPanel = new AdminAudioPanel();
        _adminAudioPanel.OpenCentered();

        _adminAudioPanel.OnPlayButtonEnabled += () => Play();
        _adminAudioPanel.OnPauseButtonEnabled += () => Pause();
        _adminAudioPanel.OnStopButtonEnabled += () => Stop();
        _adminAudioPanel.OnAddTrackPressed += (track) => AddTrack(track);
        _adminAudioPanel.OnPlaybackReleased += (ratio) => SetPlayback(ratio);
        _adminAudioPanel.OnGlobalCheckboxToggled += (toggled) => ChangeGlobalToggled(toggled);
        _adminAudioPanel.OnVolumeLineTextChanged += (volume) => SetVolume(volume);
    }

    public override void Closed()
    {
        if (_adminAudioPanel != null)
            _adminAudioPanel.Close();
    }

    public void Play()
    {
        var message = new AdminAudioPanelEuiMessage.Play();
        SendMessage(message);
    }

    public void Stop()
    {
        var message = new AdminAudioPanelEuiMessage.Stop();
        SendMessage(message);
    }

    public void Pause()
    {
        var message = new AdminAudioPanelEuiMessage.Pause();
        SendMessage(message);
    }

    public void SetPlayback(float ratio)
    {
        var message = new AdminAudioPanelEuiMessage.SetPlaybackPosition(ratio);
        SendMessage(message);
    }

    public void AddTrack(string track)
    {
        var message = new AdminAudioPanelEuiMessage.AddTrack(track);
        SendMessage(message);
    }

    public void ChangeGlobalToggled(bool toggled)
    {
        var message = new AdminAudioPanelEuiMessage.GlobalToggled(toggled);
        SendMessage(message);
    }

    public void SetVolume(float volume)
    {
        var message = new AdminAudioPanelEuiMessage.SetVolume(volume);
        SendMessage(message);
    }

    private void SelectPlayer(Guid player)
    {
        var message = new AdminAudioPanelEuiMessage.SelectPlayer(player);
        SendMessage(message);
    }

    private void UnselectPlayer(Guid player)
    {
        var message = new AdminAudioPanelEuiMessage.UnselectPlayer(player);
        SendMessage(message);
    }

    private void UpdatePlayersContainer(AdminAudioPanel adminAudioPanel, AdminAudioPanelEuiState state)
    {
        adminAudioPanel.PlayersContainer.RemoveAllChildren();

        foreach (var player in state.Players)
        {
            var newButton = new Button
            {
                ClipText = true,
                ToggleMode = true,
                Text = player.Value,
                HorizontalExpand = true,
                Pressed = state.SelectedPlayers.FirstOrNull(selectedPlayer => selectedPlayer == player.Key) != null
            };
            newButton.OnToggled += (args) =>
            {
                if (args.Pressed)
                {
                    SelectPlayer(player.Key);
                }
                else
                {
                    UnselectPlayer(player.Key);
                }
            };

            adminAudioPanel.PlayersContainer.AddChild(newButton);
        }
    }

    private void UpdatePlayControlButtons(AdminAudioPanel adminAudioPanel, AdminAudioPanelEuiState state)
    {
        adminAudioPanel.PlayButton.Pressed = state.Playing;
    }

    private void UpdatePlaybackPosition(AdminAudioPanel adminAudioPanel, AdminAudioPanelEuiState state)
    {
        adminAudioPanel.PlaybackSlider.MaxValue = state.CurrentTrackLength;
        adminAudioPanel.PlaybackSlider.SetValueWithoutEvent(state.PlaybackPosition);
    }

    private void UpdateDurationLabel(AdminAudioPanel adminAudioPanel, AdminAudioPanelEuiState state)
    {
        adminAudioPanel.DurationLabel.Text = $@"{TimeSpan.FromSeconds(state.PlaybackPosition):mm\:ss} / {TimeSpan.FromSeconds(state.CurrentTrackLength):mm\:ss}";
    }

    private void UpdateCurrentTrackLabel(AdminAudioPanel adminAudioPanel, AdminAudioPanelEuiState state)
    {

    }

    private void UpdateUI()
    {
        if (_adminAudioPanel is not { } adminAudioPanel)
            return;

        if (_state is not { } state)
            return;

        UpdatePlayersContainer(adminAudioPanel, state);
        UpdatePlayControlButtons(adminAudioPanel, state);
        UpdatePlaybackPosition(adminAudioPanel, state);
        UpdateDurationLabel(adminAudioPanel, state);
    }
}
