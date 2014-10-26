using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ZombieSquad))]
public class ZombieCameraController : MonoBehaviour
{
	[SerializeField]
	Camera _cameraToControl;

	[SerializeField]
	float _speed = 1f;

	[SerializeField]
	float _zoomSpeed = 4f;

	[SerializeField]
	Vector3 _bias;

	ZombieSquad _squad;

	bool _isInit = false;
	public Vector3 DistanceToCamera;

	Vector3 _zoomPos;
	float _zoomFactor;

    public Transform[] IntroInterestingPoints;
    public string[] IntroMessages;

	State _lastState;

	[SerializeField]
	float _maxCameraDeltaY = 30f;
	float _originalCameraY;

	[SerializeField]
	float _minZoom = 1f;

	[SerializeField]
	float _maxZoom = 5f;

	public Transform centralTransform;
	float _totalZoom = 1f;

	enum State
	{
		DEFAULT,
		ZOOM_TO_POSITION,
        INTRO,
		TWO_TOUCHES
	}
	State _state = State.INTRO;

	Timer _waitInZoom = new Timer();

	void Awake()
	{
		_squad = GetComponent<ZombieSquad>();
		_originalCameraY = _cameraToControl.transform.position.y;
	}

	void Update()
	{
        if(!_isInit)
        {
            DistanceToCamera = _cameraToControl.transform.position - _squad.AveragePosition;
            _isInit = true;
        }

		if(Input.touches != null && Input.touches.Length >= 2)
		{
			_lastState = _state;
			_state = State.TWO_TOUCHES;
		}

		switch (_state)
		{
		case State.DEFAULT:
			DefaultState();
			break;

		case State.ZOOM_TO_POSITION:
			ZoomToPositionState();
			break;

        case State.INTRO:
            IntroState();
            break;

		case State.TWO_TOUCHES:
			TwoTouchesState();
			break;
		}
	}

	public void ZoomToPosition(Vector3 zoomPos, float zoomFactor)
	{
		_zoomPos = zoomPos;
		_zoomFactor = zoomFactor;

		_waitInZoom.WaitForSeconds(1.25f);
		_state = State.ZOOM_TO_POSITION;
        UI3dController.Instance.Hide();
	}

	public void TwoTouchesState()
	{
		if(Input.touches.Length < 2)
		{
//			UI3dController.Instance.Show();
//			_state = _lastState;
			_totalZoom = 1f;
			_state = State.DEFAULT;
		}
		else
		{
//			UI3dController.Instance.Hide();

			Touch touch1 = Input.touches[0];
			Touch touch2 = Input.touches[1];

			float currentDist = (touch1.position - touch2.position).magnitude;
			float prevDist = ((touch1.position - touch1.deltaPosition) - (touch2.position - touch2.deltaPosition)).magnitude;

			float zoomFactor = prevDist/currentDist;

			if(zoomFactor > 1f)
			{
				_totalZoom += (zoomFactor -1f);
			}
			else
			{
				_totalZoom -= (1f - zoomFactor);
			}

			if(_totalZoom > _maxZoom)
			{
				_totalZoom = _maxZoom;
			}
			else if(_totalZoom < _minZoom)
			{
				_totalZoom = _minZoom;
			}

			float normalizedFactor = (_totalZoom - _minZoom) / (_maxZoom - _minZoom);
			Debug.Log("TotalZoom " + _totalZoom + ", _minZoom: " + _minZoom + ", _maxZoom: " + _maxZoom + ", Normalized Factor: " + normalizedFactor);

			Vector3 idealNextPos = _squad.AveragePosition + DistanceToCamera + _bias;
			Vector3 nextPos = Vector3.Lerp(idealNextPos, centralTransform.position, normalizedFactor);

			_cameraToControl.transform.position = nextPos;
		}
	}

	Vector3 TruncateByY(Vector3 v, float maxMagnitude)
	{
		if(v.y > maxMagnitude)
		{
			float scale = v.y / maxMagnitude;
			v /= scale;
		}

		return v;
	}

    int _currentInterestingPoint = 0;
    float _remainingInterestingPointWaitTime = 2f;
    bool _shownIntroMessage = false;
    void IntroState()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            _remainingInterestingPointWaitTime = 1f;
            _currentInterestingPoint = 1000;
            UIManager.Instance.MessagePanel.Hide();
        }

        if (_currentInterestingPoint < IntroInterestingPoints.Length)
        {
            Vector3 currentCameraDestination = IntroInterestingPoints[_currentInterestingPoint].position + DistanceToCamera * 0.5f;
            float distance = Vector3.Distance(_cameraToControl.transform.position, currentCameraDestination);
            _cameraToControl.transform.position = Vector3.Lerp(_cameraToControl.transform.position, currentCameraDestination, 3 * Time.deltaTime * _speed);
            if (distance < 1f)
            {
                if (!_shownIntroMessage && _currentInterestingPoint <  IntroMessages.Length)
                {
                    _shownIntroMessage = true;
                    UIManager.Instance.MessagePanel.Show(IntroMessages[_currentInterestingPoint]);
                }
                _remainingInterestingPointWaitTime -= Time.deltaTime;
                if (_remainingInterestingPointWaitTime <= 0f)
                {
                    _shownIntroMessage = false;
                    UIManager.Instance.MessagePanel.Hide();
                    _remainingInterestingPointWaitTime = 1f;
                    _currentInterestingPoint++;
                }
            }
        }
        else
        {
            _remainingInterestingPointWaitTime -= Time.deltaTime;
            if (_remainingInterestingPointWaitTime <= 0f)
            {
                if (!_shownIntroMessage)
                {
                    _shownIntroMessage = true;
                    UIManager.Instance.MessagePanel.Show("Ready?");
                }
            }
            DefaultState();

            if (_currentInterestingPoint == 1000)
            {
                _squad.WaitingToStart = false;
                _state = State.DEFAULT;
                UI3dController.Instance.Show();
            }
        }
    }

	void DefaultState()
	{
		Vector3 newPosition = Vector3.Lerp(_cameraToControl.transform.position, _squad.AveragePosition + DistanceToCamera + _bias, Time.deltaTime * _speed);
		_cameraToControl.transform.position = newPosition;
	}

	void ZoomToPositionState()
	{
		Vector3 newPosition = Vector3.Lerp(_cameraToControl.transform.position, _zoomPos + DistanceToCamera * _zoomFactor, Time.deltaTime * _zoomSpeed);
		_cameraToControl.transform.position = newPosition;

		if(_waitInZoom.IsFinished())
		{
			_state = State.DEFAULT;
            UI3dController.Instance.Show();
		}
	}
}
