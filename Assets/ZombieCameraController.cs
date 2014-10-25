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

	enum State
	{
		DEFAULT,
		ZOOM_TO_POSITION
	}
	State _state = State.DEFAULT;

	Timer _waitInZoom = new Timer();

	void Awake()
	{
		_squad = GetComponent<ZombieSquad>();
	}

	void Update()
	{
		switch (_state)
		{
		case State.DEFAULT:
			DefaultState();
			break;

		case State.ZOOM_TO_POSITION:
			ZoomToPositionState();
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

	void DefaultState()
	{
		if(!_isInit)
		{
			_distanceToCamera = _cameraToControl.transform.position - _squad.AveragePosition;
			_isInit = true;
		}
		else
		{
			Vector3 newPosition = Vector3.Lerp(_cameraToControl.transform.position, _squad.AveragePosition + _distanceToCamera + _bias, Time.deltaTime * _speed);
			_cameraToControl.transform.position = newPosition;
		}
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
