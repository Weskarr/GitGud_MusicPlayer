using UnityEngine;

[CreateAssetMenu(fileName = "SongSO", menuName = "Scriptable Objects/SongSO")]
public class SongSO : ScriptableObject
{
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private string _musicTitle;
    [SerializeField] private string _musicArtist;
    [SerializeField] private string _musicLink;
    [SerializeField] private string _musicLicense;
    [SerializeField] private float _musicVolume;

    public AudioClip MusicClip => _musicClip;
    public string MusicTitle => _musicTitle;
    public string MusicArtist => _musicArtist;
    public string MusicLink => _musicLink;
    public string MusicLicense => _musicLicense;
    public float MusicVolume => _musicVolume;
}
