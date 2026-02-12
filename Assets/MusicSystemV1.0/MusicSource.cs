
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicSource : MonoBehaviour
{
    [SerializeField] private MusicManager _musicManager;
    [SerializeField] private AudioSource _musicSource;

    [SerializeField] private bool isPlaying;
    [SerializeField] private double songStartDspTime;
    [SerializeField] private double songLength;
    [SerializeField] private double songElapsed;
    [Range(0, 100),SerializeField] private double songProgress;
    [SerializeField] private float songDefaultVolume;
    [SerializeField] private float _musicVolume;

    [SerializeField] private float fadeOutDuration = 2.0f;

    public Action IsFinishedPlayingMusicPiece;

    void Update()
    {
        // Safety Check.
        if (!isPlaying)
            return;

        // Update Time-based.
        songElapsed = AudioSettings.dspTime - songStartDspTime;
        songProgress = songElapsed / songLength * 100;
        double timeRemaining = songLength - songElapsed;

        // Apply fade out only in last fadeOutDuration.
        if (timeRemaining <= fadeOutDuration)
        {
            // Goes 1.0 -> 0.0 instead.
            float fadeFactor = Mathf.Clamp01((float)(timeRemaining / fadeOutDuration)); 
            float targetVolume = _musicVolume * songDefaultVolume * fadeFactor;
            _musicSource.volume = targetVolume;
        }
        else
        {
            // Maintain live volume during normal play.
            _musicSource.volume = _musicVolume * songDefaultVolume;
        }

        // Song End.
        if (songElapsed >= songLength)
        {
            isPlaying = false;
            IsFinishedPlayingMusicPiece();
        }
    }

    public void ChangeMusicVolume(float volume)
    {
        _musicVolume = volume;
    }

    private void OnApplicationPause(bool pause)
    {
        PauseMusic(pause);
    }

    public void PauseMusic(bool pause)
    {
        if (pause)
            _musicSource.Pause();
        else
            _musicSource.UnPause();
    }

    public void ChangeMusic(SongSO musicSo)
    {
        songDefaultVolume = musicSo.MusicVolume;

        _musicSource.clip = musicSo.MusicClip;
        _musicSource.volume = songDefaultVolume;

        songLength = _musicSource.clip.length;
        songStartDspTime = AudioSettings.dspTime;

        isPlaying = true;
        _musicSource.PlayScheduled(songStartDspTime);
    }

    public bool CheckIsPlaying()
    {
        if (_musicSource.isPlaying)
            return true;
        else
            return false;
    }
}
