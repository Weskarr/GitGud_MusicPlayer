using UnityEngine;

public class OpenLicenseLink : MonoBehaviour
{
    [SerializeField] private string _musicLink;

    public void ChangeMusicLink(string newLink)
    {
        _musicLink = newLink;
    }

    public void OpenLicenseURL()
    {
        Application.OpenURL($"https://{_musicLink}/");
    }
}