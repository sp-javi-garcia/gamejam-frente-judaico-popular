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
    CharacterController _characterController;

	BrainDepot _depot;

    public bool CanBeThrown
    {
        get
        {
            return _state == State.WAITING;
        }
    }

    public static Brain CreateBrain(BrainPrefab brainPrefab, BrainDepot depot, Vector3 position)
    {
        GameObject brainGO = (GameObject)GameObject.Instantiate(Resources.Load(brainPrefab.Path));
        Brain brain = brainGO.GetComponent<Brain>();
        brain.Init(position, depot);
        return brain;
    }

    void Init(Vector3 position, BrainDepot depot)
    {
        _characterController = GetComponent<CharacterController>();
        SetAppearing();
		_depot = depot;
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

    float _remainingDraggingTime;
    const float kMaxDraggingTime = 0.5f;
    void SetDragging()
    {
        _state = State.DRAGGING;
        _remainingDraggingTime = kMaxDraggingTime;
    }

    void SetDisappearing()
    {
        _state = State.DISAPPEARING;
        StartCoroutine(DoDisappear());
    }

    IEnumerator DoDisappear()
    {
		_depot.ActiveBrains.Remove(this);

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

    Vector3 _fallingForce;
    void SetFalling(Vector3 force)
    {
        _state = State.FALLING;
        _fallingForce = force;
        _fallingTime = 0f;
        iTween.RotateTo(gameObject, iTween.Hash("rotation", Vector3.zero,
                                      "easetype", iTween.EaseType.linear,
                                      "time", 1f));
        //body.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void DraggingBehavior()
    {
        _remainingDraggingTime -= Time.deltaTime;
        if (_remainingDraggingTime <= 0f)
        {
            ReturnToDepot();
            SetWaiting();
        }
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
    float _fallingTime = 0f;
    void FallingBehavior()
    {
        _fallingTime += Time.deltaTime;
        _characterController.Move((2f * _fallingForce + (40 * _fallingTime * Vector3.down)) * Time.deltaTime);
        //_fallingForce = Vector3.Lerp(_fallingForce, Vector3.zero, 0.1f);
        
        if (transform.position.y < 1.5f) //Physics.Raycast (transform.position, -Vector3.up, 1f))
        {
            _fallingForce = Vector3.Lerp(_fallingForce, Vector3.zero, 0.25f);
            //_fallingForce = Vector3.zero;
            _fallingTimeout -= Time.deltaTime;
        }
        else
        {
            //TODO: WIND!!!
            _fallingTimeout = 1f;
        }

        if (_fallingTimeout <= 0f)
        {
            SetIdle();
        }
    }

    const int kMaxSamples = 5;
    Vector3[] _throwSamples = new Vector3[kMaxSamples];
    int _throwSampleIndex = 0;
    int _throwSamplesCount = 0;

    void AddSample(Vector3 delta)
    {
        _throwSamples[_throwSampleIndex] = delta;
        _throwSampleIndex = (_throwSampleIndex + 1) % kMaxSamples;
        _throwSamplesCount++;
    }

    public float kForceFactor = 600f;
    Vector3 GetThrowForce()
    {
        _throwSamplesCount = _throwSamplesCount < kMaxSamples ? _throwSamplesCount : kMaxSamples;
        Vector3 result = Vector3.zero;
        for (int i = 0; i < _throwSamplesCount; ++i)
        {
            result += _throwSamples[(kMaxSamples + _throwSampleIndex - _throwSamplesCount + i) % kMaxSamples];
        }
        return kForceFactor * result / _throwSamplesCount;
    }

    void IdleBehavior()
    {
        _remainingIdleTime -= Time.deltaTime;
        if (_remainingIdleTime < 0f)
        {
            SetDisappearing();
        }
    }

    public void OnBrainPressed(Vector3 startPosition)
    {
        if (_state == State.WAITING)
        {
            SetDragging();
        }
    }

    public void OnBrainMoved(Vector3 position)
    {
        AddSample(position - transform.position);
        transform.position = Vector3.Lerp(transform.position, position, 0.3f);
    }

    void ReturnToDepot()
    {
        iTween.MoveTo(gameObject, iTween.Hash("position", _depot.GetBrainPositionByBrain(this),
                                              "easetype", iTween.EaseType.easeOutExpo,
                                              "time", kReturnTime));
        _depot.ResetSelectedBrain();
    }

    Vector3 FixThrowForce(Vector3 throwForce)
    {
        float zCorrectionByPosition = transform.position.z - _depot.transform.position.z;
         
        if (throwForce.z > -1f)
            throwForce.z = Mathf.Max(0f, throwForce.z);
        throwForce.z = Mathf.Min(14.5f - zCorrectionByPosition , throwForce.z);
        Debug.Log("Throw force: " + throwForce + " transform.position.x = " + transform.position.x + " - _depot.transform.position.x = " + _depot.transform.position.x);
        float xCorrectionByPosition = 0.2f * (transform.position.x - _depot.transform.position.x);
        if (throwForce.x < 0f)
        {
            // -5 xcorrection
            xCorrectionByPosition = xCorrectionByPosition < 0f ? 0f : xCorrectionByPosition;
            throwForce.x = Mathf.Max(-11f - xCorrectionByPosition, throwForce.x);
        }
        else
        {
            xCorrectionByPosition = xCorrectionByPosition > 0f ? 0f : xCorrectionByPosition;
            throwForce.x = Mathf.Min(11f - xCorrectionByPosition, throwForce.x);
        }


        return throwForce;
    }

    float kReturnTime = 0.5f;
    public bool OnBrainReleased(Vector3 endPosition, bool overBrainDepot)
    {
        AddSample(endPosition - transform.position);
        Vector3 throwForce = FixThrowForce(GetThrowForce());

        if (overBrainDepot || throwForce.z < 0f)
        {
            ReturnToDepot();
            return false;
        }
        else
        {
            SetFalling(throwForce);
            _throwSamplesCount = 0;
            return true;
        }
    }
}
