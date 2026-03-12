
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicTabData : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private List<GameObject> _tabSections;

    public Image GetIconImage => _iconImage;

    public List<GameObject> GetTabSections => _tabSections;
}
