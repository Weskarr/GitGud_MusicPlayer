
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicSource : MonoBehaviour
{
    [SerializeField] private MusicManager _musicManager;
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private bool _isPaused;
    [SerializeField] private double _pausedElapsed;
    [SerializeField] private double _songStartDspTime;
    [SerializeField] private double _songLength;
    [SerializeField] private double _songElapsed;
    [Range(0, 100),SerializeField] private double _songProgress;
    [SerializeField] private float _songDefaultVolume;
    [SerializeField] private float _musicVolume;
    [SerializeField] private float _fadeOutDuration = 2.0f;
    [SerializeField] private float _fadeInDuration = 1.5f;

    [SerializeField] private float _fadeInTimer;
    [SerializeField] private bool _isFadingIn;

    public Action IsFinishedPlayingMusicPiece;

    private bool _hasStarted = false;

    #region Update Function

    private void Update()
    {
        // Safety Check.
        if (_isPaused || !_hasStarted)
            return;

        // Update Time-based.
        _songElapsed = AudioSettings.dspTime - _songStartDspTime;
        _songProgress = _songElapsed / _songLength * 100;
        double timeRemaining = _songLength - _songElapsed;

        // Apply fade in only at the start.
        if (_isFadingIn)
        {
            _fadeInTimer += Time.deltaTime;
            float current = Mathf.Clamp01(_fadeInTimer / _fadeInDuration);
            float fadeInFactor = Mathf.SmoothStep(0f, 1f, current);
            float targetVolume = _musicVolume * _songDefaultVolume;
            _audioSource.volume = targetVolume * fadeInFactor;

            // Check if done fading in.
            if (current >= 1f)
                _isFadingIn = false;

            // Only return early when fading in.
            return;
        }

        // Apply fade out only in last fadeOutDuration.
        if (timeRemaining <= _fadeOutDuration)
        {
            // Goes 1.0 -> 0.0 instead.
            float fadeFactor = Mathf.Clamp01((float)(timeRemaining / _fadeOutDuration));
            float targetVolume = _musicVolume * _songDefaultVolume * fadeFactor;
            _audioSource.volume = targetVolume;
        }
        else
        {
            // Maintain live volume during normal play.
            _audioSource.volume = _musicVolume * _songDefaultVolume;
        }

        // Song End.
        if (_songElapsed >= _songLength)
        {
            _isPaused = true;
            IsFinishedPlayingMusicPiece?.Invoke();
        }
    }

    #endregion

    #region Change Functions

    public void ChangeMusicVolume(float volume)
    {
        _musicVolume = volume;
    }

    public void ChangeMusic(SongSO musicSo)
    {
        _hasStarted = true;

        _pausedElapsed = 0;

        _songDefaultVolume = musicSo.MusicVolume;

        _audioSource.clip = musicSo.MusicClip;
        _audioSource.volume = _songDefaultVolume;

        _songLength = _audioSource.clip.length;
        _songStartDspTime = AudioSettings.dspTime;

        _isPaused = false;

        _fadeInTimer = 0f;
        _isFadingIn = true;

        _audioSource.volume = 0f;

        _audioSource.Play();
    }

    public void ChangeIsPaused(bool isPaused)
    {
        // Check if the state changed.
        if (_isPaused == isPaused)
            return;

        // Act accordingly, only when required.
        if (isPaused)
        {
            // store how much time already passed
            _pausedElapsed = AudioSettings.dspTime - _songStartDspTime;

            _audioSource.Pause();
        }
        else
        {
            // shift start time forward so elapsed stays correct
            _songStartDspTime = AudioSettings.dspTime - _pausedElapsed;

            _audioSource.UnPause();
        }

        // Apply new development.
        _isPaused = isPaused;
    }

    #endregion

    #region Check Functions

    public bool CheckIsPlaying()
    {
        return !_isPaused;
    }

    #endregion

}
