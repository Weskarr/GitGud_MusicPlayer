using UnityEngine;

public class MusicPlayerEnabler : MonoBehaviour
{
    [SerializeField] private GameObject _musicPlayer;

    public void Enabler()
    {
        if (_musicPlayer.activeSelf == true)
            _musicPlayer.SetActive(false);
        else
            _musicPlayer.SetActive(true);
    }
}
