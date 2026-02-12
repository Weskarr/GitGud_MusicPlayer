using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
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

    [SerializeField] private float _musicVolumeStart = 0.5f;

    private void SubscribeToIsFinishedPlayingMusicPiece()
    {
        _musicSource.IsFinishedPlayingMusicPiece += GoPlayNewMusicPiece;
    }

    private void UnsubscribeToIsFinishedPlayingMusicPiece()
    {
        _musicSource.IsFinishedPlayingMusicPiece -= GoPlayNewMusicPiece;
    }

    private void Start()
    {
        SubscribeToIsFinishedPlayingMusicPiece();
        GoPlayNewMusicPiece();

        _pauseToggle.onValueChanged.AddListener(TriggerPauseMusic);
        _musicVolumeSlider.onValueChanged.AddListener(TriggerMusicVolumeChange);

        _musicVolumeSlider.SetValueWithoutNotify(_musicVolumeStart);
        _musicSource.ChangeMusicVolume(_musicVolumeStart);
    }

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

    public void TriggerPauseMusic(bool pause)
    {
        _musicSource.PauseMusic(pause);
    }

    public void TriggerMusicVolumeChange(float volume)
    {
        _musicSource.ChangeMusicVolume(volume);
    }

    public void TriggerPlayNextSong()
    {
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
}
