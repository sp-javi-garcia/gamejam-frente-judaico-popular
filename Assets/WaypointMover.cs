using UnityEngine;
using System.Collections.Generic;

public class WaypointMover : MonoBehaviour
{
	[SerializeField]
	List<Transform> _waypoints = new List<Transform>();

	enum State
	{
		IDLE,
		START_WAYPOINT,
		NEXT_WAYPOINT,
		MOVE_TO_WAYPOINT
	}
	State _state = State.IDLE;

	int _currentWaypointIdx = 0;

	[SerializeField]
	public float MoveSpeed = 60f;

	[SerializeField]
	float RotationSpeed = 2f;

	public bool startInWaypoint = true;

	void Awake()
	{
		if(startInWaypoint)
		{
			transform.position = _waypoints[0].position;
		}

		Invoke("Start", 0.5f);
	}

	void Start()
	{
		_state = State.START_WAYPOINT;
	}

	void Update()
	{
		switch (_state)
		{
		case State.IDLE:
			IdleState();
			break;

		case State.START_WAYPOINT:
			StartWaypointState();
            break;

		case State.NEXT_WAYPOINT:
			NextWaypointState();
			break;

		case State.MOVE_TO_WAYPOINT:
			MoveToWaypointState();
            break;
        }
	}

	void IdleState()
	{
	}

	void StartWaypointState()
	{
		_currentWaypointIdx = 0;

		_state = State.MOVE_TO_WAYPOINT;
    }

	void NextWaypointState()
	{
		_currentWaypointIdx = (_currentWaypointIdx + 1) % _waypoints.Count;

		_state = State.MOVE_TO_WAYPOINT;
    }

	void MoveToWaypointState()
	{
		Vector3 targetPos = _waypoints[_currentWaypointIdx].position;
		Vector3 currentPosition = Vector3.MoveTowards(transform.position, targetPos, MoveSpeed * Time.deltaTime);

		if((currentPosition - targetPos).sqrMagnitude < 1e-1f)
		{
			_state = State.NEXT_WAYPOINT;
		}

		Quaternion quat = Quaternion.LookRotation(targetPos - transform.position);
		Quaternion newQuat = Quaternion.Slerp(transform.rotation, quat, Time.deltaTime * RotationSpeed);

		transform.rotation = newQuat;
		transform.position = currentPosition;
    }
}
