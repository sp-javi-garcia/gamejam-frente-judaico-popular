using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI3dController : MonoBehaviour
{
    Vector3 _showPosition;
    Vector3 _hidePosition;
    BrainDepot _brainDepot;
    public Text WindSpeedText;
    public Text RemainingTimeText;

    public static UI3dController Instance { get; protected set; }

    void Start()
    {
        Instance = this;
        _brainDepot = GetComponentInChildren<BrainDepot>();
        _showPosition = transform.localPosition;
        _hidePosition = _showPosition - transform.up * 5f;
        _isVisible = false;
        transform.localPosition = _hidePosition;
        Show();
    }

    bool _isVisible;
    const float kTransitionTime = 0.5f;

    public void Show()
    {
        if (!_isVisible)
        {
            iTween.MoveTo(gameObject, iTween.Hash("position", _showPosition,
                                                  "islocal", true,
                                                  "easetype", iTween.EaseType.easeOutQuad,
                                                  "time", kTransitionTime));
            _isVisible = true;
            _brainDepot.HandleTaps = true;
        }
    }

    public void Hide()
    {
        if (_isVisible)
        {
            iTween.MoveTo(gameObject, iTween.Hash("position", _hidePosition,
                                                  "islocal", true,
                                                  "easetype", iTween.EaseType.easeInQuad,
                                                  "time", kTransitionTime));
            _isVisible = false;
            _brainDepot.HandleTaps = false;
        }
    }
}
