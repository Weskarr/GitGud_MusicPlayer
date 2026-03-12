
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlbumSO", menuName = "Scriptable Objects/AlbumSO")]
public class AlbumSO : ScriptableObject
{
    [SerializeField] private List<SongSO> _albumSongs = new List<SongSO>();

    public List<SongSO> AlbumSongs => _albumSongs;
}
