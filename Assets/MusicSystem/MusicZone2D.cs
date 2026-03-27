using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MusicZone2D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private MusicManager _musicManager;
    [SerializeField] private AlbumSO _album;
    [SerializeField] private Image _image;

    [Header("Animation Settings")]
    [SerializeField] private bool _useEnumeratorInstead = false;
    [SerializeField] private float _activeAlpha = 1f;
    [SerializeField] private float _inactiveAlpha = 0.4f;
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _normalScale = 1f;
    [SerializeField] private float _animationSpeed = 2f;

    private bool _isHovered;
    private Coroutine _transitionEnumerator;

    private bool _isUpdateTransitioning;
    private float _updateTransitionProgress;

    private float _startAlpha;
    private float _startScale;

    #region Awake Function

    private void Awake()
    {
        if (_image == null)
            _image = GetComponent<Image>();
    }

    #endregion

    #region Music Functions

    private void ActivateZone()
    {
        // Debugging here atm, for final version optimisation.
        float time = Time.realtimeSinceStartup;

        // !!! Getting a major fps drop on switching !!!
        if (_musicManager != null && _album != null)
            _musicManager.SetAlbum(_album);

        Debug.Log("Change took: " + (Time.realtimeSinceStartup - time));
    }

    #endregion

    #region UI Event Functions

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;
        ActivateZone();

        if (_useEnumeratorInstead)
            StartCoroutineTransition();
        else
            StartUpdateTransition();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;

        if (_useEnumeratorInstead)
            StartCoroutineTransition();
        else
            StartUpdateTransition();
    }

    #endregion

    #region Update Approach Functions

    private void StartUpdateTransition()
    {
        _startAlpha = _image.color.a;
        _startScale = transform.localScale.x;

        _updateTransitionProgress = 0f;
        _isUpdateTransitioning = true;
    }

    private void Update()
    {
        if (_useEnumeratorInstead || !_isUpdateTransitioning)
            return;

        float targetAlpha = _isHovered ? _activeAlpha : _inactiveAlpha;
        float targetScale = _isHovered ? _hoverScale : _normalScale;

        _updateTransitionProgress += Time.deltaTime * _animationSpeed;

        float smooth = Mathf.SmoothStep(0f, 1f, _updateTransitionProgress);

        float newAlpha = Mathf.Lerp(_startAlpha, targetAlpha, smooth);
        float newScale = Mathf.Lerp(_startScale, targetScale, smooth);

        Color c = _image.color;
        c.a = newAlpha;
        _image.color = c;

        transform.localScale = Vector3.one * newScale;

        if (_updateTransitionProgress >= 1f)
        {
            c.a = targetAlpha;
            _image.color = c;

            transform.localScale = Vector3.one * targetScale;

            _isUpdateTransitioning = false;
        }
    }

    #endregion

    #region Enumerator Approach Functions

    private void StartCoroutineTransition()
    {
        if (_transitionEnumerator != null)
            StopCoroutine(_transitionEnumerator);

        _transitionEnumerator = StartCoroutine(TransitionEnumerator());
    }

    private IEnumerator TransitionEnumerator()
    {
        float startAlpha = _image.color.a;
        float targetAlpha = _isHovered ? _activeAlpha : _inactiveAlpha;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.one * (_isHovered ? _hoverScale : _normalScale);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * _animationSpeed;
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            // Alpha
            Color c = _image.color;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, smooth);
            _image.color = c;

            // Scale
            transform.localScale = Vector3.Lerp(startScale, targetScale, smooth);

            yield return null;
        }

        // Final snap (prevents drift)
        Color finalColor = _image.color;
        finalColor.a = targetAlpha;
        _image.color = finalColor;

        transform.localScale = targetScale;

        _transitionEnumerator = null;
    }

    #endregion
}