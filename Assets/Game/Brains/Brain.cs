﻿using UnityEngine;
using System.Collections;

public class Brain : MonoBehaviour
{
    public float Range = 15f;
    public float Duration = 30f;
    public GameObject BrainGO;

    public enum State
    {
        APPEARING,
        WAITING,
        DRAGGING,
        FALLING,
        IDLE,
        DISAPPEARING,
        EATING
    }

    State _state;
    CharacterController _characterController;

	BrainDepot _depot;
	BrainAudioManager _audioManager;

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

	void Awake()
	{
		_audioManager = GetComponent<BrainAudioManager>();
	}

    void Init(Vector3 position, BrainDepot depot)
    {
        _characterController = GetComponent<CharacterController>();
        SetAppearing();
		_depot = depot;
        transform.parent = depot.transform;
        transform.position = position;
        transform.localRotation = Quaternion.identity;
        BrainGO.transform.localScale = Vector3.one * Random.Range(1.3f, 1.88f);
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
        case State.EATING:
            EatingBehavior();
            break;
        }
    }

    const float kEatTime = 2f;
    float _remainingEatTime;
    bool _beingEat;
    public void SetEating()
    {
        if (!_beingEat)
        {
            _beingEat = true;
            //Particles or animation?
            _remainingEatTime = kEatTime;
            _state = State.EATING;
        }
    }

    void EatingBehavior()
    {
        _characterController.Move(Vector3.down * 2f * Time.deltaTime);
        _remainingEatTime -= Time.deltaTime;
        if (_remainingEatTime <= 0f)
        {
            SetDisappearing();
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
    const float kMaxDraggingTime = 10f;
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
        float sign = Random.Range(0, 2) == 0 ? -1f : 1f;
        float sign2 = Random.Range(0, 2) == 0 ? -1f : 1f;
        _fallingRotationVector = Random.Range(0, 2) == 0 ? new Vector3(0f, sign * 120f, sign2 * 720f) : new Vector3(0f, sign * 120f, sign2 * 720f);
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

    Vector3 _fallingRotationVector = new Vector3(0f, 120f, 720f);
    float _fallingTimeout = 1f;
    float _fallingTime = 0f;
    void FallingBehavior()
    {
        _fallingTime += Time.deltaTime;
        _characterController.Move((2f * _fallingForce + (40 * _fallingTime * Vector3.down)) * Time.deltaTime);
        
        if (transform.position.y < 1.5f)
        {
            _fallingForce = Vector3.Lerp(_fallingForce, Vector3.zero, 0.25f);
            //_fallingForce = Vector3.zero;
            _fallingTimeout -= Time.deltaTime;
        }
        else
        {
            _fallingTimeout = 1f;
            Vector3 currentRotation = BrainGO.transform.rotation.eulerAngles;
            BrainGO.transform.rotation = Quaternion.Euler(currentRotation.x + _fallingRotationVector.x * Time.deltaTime,
                                                          currentRotation.y + _fallingRotationVector.y * Time.deltaTime,
                                                          currentRotation.z + _fallingRotationVector.z * Time.deltaTime);
            _characterController.Move(WindController.WindForce * Mathf.Min(0.1f, 0.05f * _fallingTime) * Time.deltaTime);
        }

        if (_fallingTime > 4f)
        {
            SetDisappearing();
        }

        if (_fallingTimeout <= 0f)
        {
            SetIdle();
        }
    }

	void OnCollisionEnter()
	{
		if(_state == State.FALLING)
		{
			_audioManager.PlayHit();
		}
	}

//	void OnTriggerEnter()
//	{
//		if(_state == State.FALLING)
//		{
//			_audioManager.PlayHit();
//		}
//	}

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
