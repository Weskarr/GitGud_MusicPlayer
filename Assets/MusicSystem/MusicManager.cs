using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private float _musicVolumeStart = 0.5f;

    [SerializeField] private GameObject _musicWindow;
    [SerializeField] private MusicSource _musicSource;

    [SerializeField] private SongSO _currentMusicSO;
    [SerializeField] private SongSO _previousMusicSO;
    [SerializeField] private AlbumSO _currentAlbum;

    [SerializeField] private List<SongSO> _musicAvailable;
    [SerializeField] private List<SongSO> _musicUnavailable;

    [SerializeField] private Toggle _pauseToggle;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private OpenLicenseLink _openLicenseLink;

    [SerializeField] private TextMeshProUGUI _titleDisplay;
    [SerializeField] private TextMeshProUGUI _artistDisplay;
    [SerializeField] private TextMeshProUGUI _linkDisplay;
    [SerializeField] private TextMeshProUGUI _licenseDisplay;


    #region Start Function

    private void Start()
    {
        SubscribeToIsFinishedPlayingMusicPiece();
        GoPlayNewMusicPiece();

        _pauseToggle.onValueChanged.AddListener(TriggerPauseMusic);
        _musicVolumeSlider.onValueChanged.AddListener(TriggerMusicVolumeChange);

        _musicVolumeSlider.SetValueWithoutNotify(_musicVolumeStart);
        _musicSource.ChangeMusicVolume(_musicVolumeStart);
    }

    #endregion

    #region Subscription Functions

    private void SubscribeToIsFinishedPlayingMusicPiece()
    {
        _musicSource.IsFinishedPlayingMusicPiece += GoPlayNewMusicPiece;
    }

    private void UnsubscribeToIsFinishedPlayingMusicPiece()
    {
        _musicSource.IsFinishedPlayingMusicPiece -= GoPlayNewMusicPiece;
    }

    #endregion

    #region Section Functions

    private void UpdateCurrentlyPlayingSection()
    {
        _titleDisplay.text = _currentMusicSO.MusicTitle;
        _artistDisplay.text = $"> By {_currentMusicSO.MusicArtist}";
    }

    private void UpdateDetailsSection()
    {
        _linkDisplay.text = _currentMusicSO.MusicLink;
        _licenseDisplay.text = $"> {_currentMusicSO.MusicLicense}";
        _openLicenseLink.ChangeMusicLink(_currentMusicSO.MusicLink);
    }

    #endregion

    #region Playing Functions

    private void GoPlayNewMusicPiece()
    {
        if (_musicAvailable.Count == 0)
            CurrentAlbumTransfer();

        _previousMusicSO = _currentMusicSO;

        // Next
        _currentMusicSO = GetNextAvailableSongSO();

        // Random
        //_currentMusicSO = GetRandomAvailableSongSO();

        _musicSource.ChangeMusic(_currentMusicSO);
        _musicAvailable.Remove(_currentMusicSO);
        _musicUnavailable.Add(_currentMusicSO);

        UpdateCurrentlyPlayingSection();
        UpdateDetailsSection();
    }

    private void CurrentAlbumTransfer()
    {
        _musicAvailable.Clear();
        _musicUnavailable.Clear();

        List<SongSO> songsSo = _currentAlbum.AlbumSongs;
        foreach (SongSO so in songsSo)
            _musicAvailable.Add(so);
    }

    #endregion

    #region Getter Functions

    private SongSO GetNextAvailableSongSO()
    {
        // Safety Check.
        if (_musicAvailable.Count == 0)
            return null;

        return _musicAvailable[0];
    }

    private SongSO GetRandomAvailableSongSO()
    {
        // Safety Check.
        if (_musicAvailable.Count == 0)
            return null;

        int max = _musicAvailable.Count;
        int index = Random.Range(0, max);
        return _musicAvailable[index];
    }

    #endregion

    #region Pause Functions

    private void OnApplicationPause(bool isPaused)
    {
        PauseMusic(isPaused);
    }

    public void PauseMusic(bool isPaused)
    {
        _musicSource.ChangeIsPaused(isPaused);
        _pauseToggle.SetIsOnWithoutNotify(isPaused);
    }

    #endregion

    #region Trigger Functions

    public void TriggerPauseMusic(bool pause)
    {
        PauseMusic(pause);
    }

    public void TriggerMusicVolumeChange(float volume)
    {
        _musicSource.ChangeMusicVolume(volume);
    }

    public void TriggerPlayNextSong()
    {
        // Make sure it isn't paused.
        PauseMusic(false);

        // Go play now.
        GoPlayNewMusicPiece();
    }

    public void TriggerPlayPreviousSong()
    {
        // Safety check.
        if (_previousMusicSO == null)
            return;

        // Return current song to available pool.
        //if (_currentMusicSO != null && !_musicAvailable.Contains(_currentMusicSO))
        //    _musicAvailable.Add(_currentMusicSO);

        // Swap current and previous.
        SongSO temp = _currentMusicSO;
        _currentMusicSO = _previousMusicSO;
        _previousMusicSO = temp;

        // Play the previous music.
        _musicSource.ChangeMusic(_currentMusicSO);

        // Move current to unavailable.
        if (_musicAvailable.Contains(_currentMusicSO))
            _musicAvailable.Remove(_currentMusicSO);

        if (!_musicUnavailable.Contains(_currentMusicSO))
            _musicUnavailable.Add(_currentMusicSO);

        UpdateCurrentlyPlayingSection();
        UpdateDetailsSection();
    }

    #endregion
}