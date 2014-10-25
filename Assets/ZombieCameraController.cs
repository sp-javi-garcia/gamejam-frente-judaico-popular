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
	Vector3 _distanceToCamera;

	Vector3 _zoomPos;
	float _zoomFactor;

    public Transform[] IntroInterestingPoints;
    public string[] IntroMessages;

	enum State
	{
		DEFAULT,
		ZOOM_TO_POSITION,
        INTRO
	}
	State _state = State.INTRO;

	Timer _waitInZoom = new Timer();

	void Awake()
	{
		_squad = GetComponent<ZombieSquad>();
	}

	void Update()
	{
        if(!_isInit)
        {
            _distanceToCamera = _cameraToControl.transform.position - _squad.AveragePosition;
            _isInit = true;
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
            Vector3 currentCameraDestination = IntroInterestingPoints[_currentInterestingPoint].position + _distanceToCamera * 0.5f;
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
		Vector3 newPosition = Vector3.Lerp(_cameraToControl.transform.position, _squad.AveragePosition + _distanceToCamera + _bias, Time.deltaTime * _speed);
		_cameraToControl.transform.position = newPosition;
	}

	void ZoomToPositionState()
	{
		Vector3 newPosition = Vector3.Lerp(_cameraToControl.transform.position, _zoomPos + _distanceToCamera * _zoomFactor, Time.deltaTime * _zoomSpeed);
		_cameraToControl.transform.position = newPosition;

		if(_waitInZoom.IsFinished())
		{
			_state = State.DEFAULT;
            UI3dController.Instance.Show();
		}
	}
}
