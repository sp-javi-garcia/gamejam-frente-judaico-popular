using UnityEngine;
using System.Collections;

public class EndCameraController : MonoBehaviour
{
    public enum State
    {
        TRANSLATION,
        ROTATE_AROUND,
        ZOOM
    };

    Vector3 _offset;
    Vector3 _target;
    System.Action _endTransitionCallback;
    public Vector3 _finalOffset;

    bool _callbackCalled = false;

    public void Init(Vector3 offset, Vector3 target, Vector3 finalOffset, System.Action endTransitionCallback)
    {
        _offset = offset;
        _target = target;
        _state = State.TRANSLATION;
        _endTransitionCallback = endTransitionCallback;
        _finalOffset = finalOffset;
    }

    State _state = State.TRANSLATION;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            CallEndTransitionCallback();
        }
        switch(_state)
        {
        case State.TRANSLATION:
            TranslationState();
            break;
        case State.ROTATE_AROUND:
            RotateAroundState();
            break;
        case State.ZOOM:
            ZoomState();
            break;
        }
    }

    float _remainingZoomTime = 2f;
    void ZoomState()
    {

    }

    void CallEndTransitionCallback()
    {
        if (_callbackCalled)
            return;
        _endTransitionCallback();
    }

    float _translationTime = 0.5f;
    void TranslationState()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_target - transform.position), 0.1f);
        _translationTime -= Time.deltaTime;
        if (_translationTime <= 0f)
        {
            _state = State.ROTATE_AROUND;
        }
    }

    float _remainingRotateAroundTime = 3f;
    void RotateAroundState()
    {
        camera.transform.LookAt(_target);
        transform.position = Vector3.Lerp(transform.position, _target + _finalOffset, Time.deltaTime * 0.4f);
        _remainingRotateAroundTime -= Time.deltaTime;
        if (_remainingRotateAroundTime <= 0f)
        {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 15f, 0.007f);
            _remainingZoomTime -= Time.deltaTime;
            if (_remainingZoomTime <= 0f)
            {
                CallEndTransitionCallback();
            }
        }
    }
}
