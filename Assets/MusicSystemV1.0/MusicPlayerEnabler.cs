using UnityEngine;

public class MusicPlayerEnabler : MonoBehaviour
{
    [SerializeField] private GameObject musicPlayer;

    public void Enabler()
    {
        if (musicPlayer.activeSelf == true)
            musicPlayer.SetActive(false);
        else
            musicPlayer.SetActive(true);
    }
}
