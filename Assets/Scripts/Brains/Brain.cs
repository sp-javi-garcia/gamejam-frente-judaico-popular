using UnityEngine;
using System.Collections;

public class Brain : MonoBehaviour
{
    public float Range = 15f;
    public float Duration = 30f;

    public enum State
    {
        APPEARING,
        WAITING,
        DRAGGING,
        FALLING,
        IDLE,
        DISAPPEARING
    }

    State _state;

    public static Brain CreateBrain(BrainPrefab brainPrefab, BrainDepot depot, Vector3 position)
    {
        GameObject brainGO = (GameObject)GameObject.Instantiate(Resources.Load(brainPrefab.Path));
        Brain brain = brainGO.GetComponent<Brain>();
        brain.Init(position, depot);
        return brain;
    }

    void Init(Vector3 position, BrainDepot depot)
    {
        SetAppearing();
        transform.parent = depot.transform;
        transform.position = position;
        transform.localRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update ()
    {
        switch(_state)
        {
        case State.APPEARING:
            AppearingBehavior();
            break;
        case State.DRAGGING:
            DraggingBehavior();
            break;
        case State.IDLE:
            IdleBehavior();
            break;
        case State.DISAPPEARING:
            DisappearingBehavior();
            break;
        case State.FALLING:
            FallingBehavior();
            break;
        }
    }

    const float kAppearTime = 0.5f;
    float _appearingRemainingTime;
    void SetAppearing()
    {
        _state = State.APPEARING;
        transform.localScale = Vector3.zero;
        iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one,
                                               "easetype", iTween.EaseType.easeOutBack,
                                               "time", kAppearTime));
        _appearingRemainingTime = kAppearTime;
    }

    void SetDragging()
    {
        _state = State.DRAGGING;
    }

    void SetDisappearing()
    {
        _state = State.DISAPPEARING;
        StartCoroutine(DoDisappear());
    }

    IEnumerator DoDisappear()
    {
        iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.zero,
                                               "easetype", iTween.EaseType.easeInBack,
                                               "time", 0.2));
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    const float DefaultBrainTimeout = 5f;
    const float InaccessibleBrainTimeout = 2f;
    float _remainingIdleTime;
    void SetIdle()
    {
        _remainingIdleTime = transform.position.y > 3f ? InaccessibleBrainTimeout : DefaultBrainTimeout;
        _state = State.IDLE;
    }

    void SetWaiting()
    {
        _state = State.WAITING;
    }

    void SetFalling()
    {
        _state = State.FALLING;
        Rigidbody body = gameObject.AddComponent<Rigidbody>();
        body.mass = 50f;
        //body.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void DraggingBehavior()
    {

    }

    void AppearingBehavior()
    {
        _appearingRemainingTime -= Time.deltaTime;
        if (_appearingRemainingTime <= 0f)
        {
            SetWaiting();
        }
    }

    void DisappearingBehavior()
    {
    }

    float _fallingTimeout = 1f;
    void FallingBehavior()
    {
        if (gameObject.rigidbody.IsSleeping())
        {
            _fallingTimeout -= Time.deltaTime;
        }
        else
        {
            _fallingTimeout = 1f;
        }

        if (_fallingTimeout <= 0f)
        {
            SetIdle();
        }
    }

    void IdleBehavior()
    {
        _remainingIdleTime -= Time.deltaTime;
        if (_remainingIdleTime < 0f)
        {
            SetDisappearing();
        }
    }

    Vector3 _startDragPosition;
    public void OnBrainPressed(Vector3 startPosition)
    {
        if (_state == State.IDLE)
        {
            SetDragging();
            _startDragPosition = transform.localPosition;
        }
    }

    public void OnBrainMoved(Vector3 position)
    {
        transform.position = Vector3.Lerp(transform.position, position, 0.3f);
    }



    float kReturnTime = 0.5f;

    public bool OnBrainReleased(Vector3 endPosition, bool overBrainDepot)
    {
        if (overBrainDepot)
        {
            iTween.MoveTo(gameObject, iTween.Hash("position", _startDragPosition,
                                                  "islocal", true,
                                                  "easetype", iTween.EaseType.easeOutExpo,
                                                  "time", kReturnTime));
            return false;
        }
        else
        {
            SetFalling();
            Debug.Log("Release!");
            return true;
        }
    }
}
