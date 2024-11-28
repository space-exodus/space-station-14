using System.Linq;
using Robust.Server.Audio;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Player;

namespace Content.Server.Exodus.Audio;

public sealed partial class AdminAudioPanelSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    private (EntityUid Entity, AudioComponent Audio)? _audioStream;
    private List<ICommonSession> _selectedPlayers = new();

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!Playing)
            return;

        if (_audioStream is { } audioStream &&
            audioStream.Entity.IsValid() &&
            (audioStream.Audio.State == AudioState.Playing || audioStream.Audio.State == AudioState.Paused))
            return;

        Queue.TryDequeue(out _);

        if (CurrentTrack != null)
        {
            var newStream = _audio.PlayGlobal(CurrentTrack, Global ? Filter.Broadcast() : Filter.Empty().AddPlayers(_selectedPlayers), Global, AudioParams);

            if (newStream != null)
            {
                _audioStream = newStream.Value;
            }
        }
    }

    private void SetStreamState(AudioState state)
    {
        if (_audioStream != null)
            _audio.SetState(_audioStream.Value.Entity, state, true, _audioStream.Value.Audio);
    }

    /// <summary>
    /// Stops sound and starts its over with current params with playback position of stopped sound.
    /// Used, for example, when need to update selected players.
    /// </summary>
    private void RecreateSound()
    {
        if (_audioStream is not { } audioStream)
            return;

        Playing = false;

        var playback = audioStream.Audio.PlaybackPosition;

        _audio.Stop(audioStream.Entity, audioStream.Audio);

        _audioStream = _audio.PlayGlobal(CurrentTrack, Global ? Filter.Broadcast() : Filter.Empty().AddPlayers(_selectedPlayers), Global, AudioParams);
        _audio.SetPlaybackPosition(_audioStream, playback);

        Playing = true;
    }

    #region Public API
    public readonly Queue<string> Queue = new();
    public string? CurrentTrack
    {
        get
        {
            if (Queue.TryPeek(out var track))
                return track;
            return null;
        }
    }
    public AudioParams AudioParams { get; private set; }
    public bool Playing { get; private set; } = false;
    public bool Global { get; private set; } = false;
    public HashSet<Guid> SelectedPlayers => _selectedPlayers.Select(player => player.UserId.UserId).ToHashSet();
    public float CurrentTrackPlaybackPosition
    {
        get
        {
            if (_audioStream is not { } stream)
                return 0f;
            return stream.Audio.PlaybackPosition;
        }
    }
    public TimeSpan CurrentTrackLength
    {
        get
        {
            if (CurrentTrack == null)
                return new();

            return _audio.GetAudioLength(CurrentTrack);
        }
    }

    public void AddToQueue(string filename)
    {
        Queue.Enqueue(filename);
    }

    public bool Play()
    {
        if (_audioStream != null && _audioStream.Value.Audio.State == AudioState.Paused)
        {
            return Resume();
        }

        Playing = true;
        return Playing;
    }

    public void Pause()
    {
        if (_audioStream != null && _audioStream.Value.Audio.State == AudioState.Playing)
            SetStreamState(AudioState.Paused);

        Playing = false;
    }

    public bool Resume()
    {
        if (_audioStream != null && _audioStream.Value.Audio.State == AudioState.Paused)
            SetStreamState(AudioState.Playing);

        Playing = true;
        return Playing;
    }

    public bool Stop()
    {
        if (_audioStream != null && _audioStream.Value.Audio.State != AudioState.Stopped)
            _audio.Stop(_audioStream.Value.Entity, _audioStream.Value.Audio);

        Playing = false;
        return !Playing;
    }

    public void SetVolume(float volume)
    {
        AudioParams = AudioParams.WithVolume(volume);
        if (_audioStream != null)
            _audio.SetVolume(_audioStream.Value.Entity, volume, _audioStream.Value.Audio);
    }

    public void SelectPlayer(Guid player)
    {
        if (SelectedPlayers.Contains(player))
            return;

        var session = _playerManager.NetworkedSessions.FirstOrDefault(session => session.UserId.UserId == player);

        if (session == null)
            return;

        _selectedPlayers.Add(session);
        RecreateSound();
    }

    public void UnselectPlayer(Guid player)
    {
        if (!SelectedPlayers.Contains(player))
            return;

        var session = _playerManager.NetworkedSessions.FirstOrDefault(session => session.UserId.UserId == player);

        if (session == null)
            return;

        _selectedPlayers.Remove(session);
        RecreateSound();
    }

    /// <summary>
    /// Sets playback position for currently playing sound
    /// </summary>
    /// <param name="position">Is a ratio of desired positon and track length</param>
    public void SetPlaybackPosition(float position)
    {
        if (CurrentTrack != null && _audioStream is { } audioStream)
        {
            var length = (float)_audio.GetAudioLength(CurrentTrack).TotalSeconds;
            var desiredPosition = position * length;
            _audio.SetPlaybackPosition(audioStream, desiredPosition);
        }
    }

    #endregion
}
