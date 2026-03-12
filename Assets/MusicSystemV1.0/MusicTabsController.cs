using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MusicTabsController : MonoBehaviour
{
    [SerializeField] private Color _iconSpriteNotSelectedColor = Color.black;
    [SerializeField] private Color _iconSpriteSelectedColor = Color.white;

    [SerializeField] private MusicTabData _startTabData;
    [SerializeField] private MusicTabData _currentTabData;

    private void Start()
    {
        SwitchToNewTab(_startTabData);
    }

    public void SwitchToNewTab(MusicTabData newTab)
    {
        if (newTab == _currentTabData)
            return;

        if (_currentTabData != null)
        {
            TabSectionsEnabler(_currentTabData, false);
            _currentTabData.GetIconImage.color = _iconSpriteNotSelectedColor;
        }

        TabSectionsEnabler(newTab, true);
        newTab.GetIconImage.color = _iconSpriteSelectedColor;
        _currentTabData = newTab;
    }

    private void TabSectionsEnabler(MusicTabData tab, bool direction)
    {
        if (tab == null)
            return;

        List<GameObject> sections = tab.GetTabSections;

        foreach (GameObject section in sections)
            section.SetActive(direction);
    }
}
