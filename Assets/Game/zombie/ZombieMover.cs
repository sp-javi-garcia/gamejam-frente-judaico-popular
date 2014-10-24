using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Zombie))]
[RequireComponent (typeof (Rigidbody))]
public class ZombieMover : MonoBehaviour
{
	enum State
	{
		IDLE,
		SEEK
	}

	State _state = State.IDLE;

	Zombie _zombie;

	Vector3 _targetPosition;

	public float MaxVelocity = 10f;
	public float ArrivingRadius = 10f;

	void Awake()
	{
		_zombie = GetComponent<Zombie>();
	}

	public void Seek(Vector3 targetPos)
	{
		_targetPosition = targetPos;
		_state = State.SEEK;
	}

	void Update()
	{
		switch (_state)
		{
		case State.IDLE:
			IdleState();
			break;

		case State.SEEK:
			SeekState();
			break;
		}
	}

	#region states
	void IdleState()
	{
		// Do nothing
	}

	void SeekState()
	{
		Vector3 seek = SeekSteering();

		Vector3 newVelocity = rigidbody.velocity + seek * Time.deltaTime;

		rigidbody.velocity = newVelocity;
	}

	Vector3 BoidSteering()
	{

	}

	Vector3 SeekSteering()
	{
		Vector3 desiredVelocity = _targetPosition - transform.position;
		desiredVelocity = desiredVelocity.normalized * MaxVelocity;

		Vector3 seekForce = desiredVelocity - rigidbody.velocity;

		float arriveFactor = 1f;
		float desiredVelocityMag = desiredVelocity.magnitude;
		if(desiredVelocityMag < ArrivingRadius)
		{
			arriveFactor = desiredVelocityMag / ArrivingRadius;
		}

		seekForce = seekForce * arriveFactor;

		return seekForce;
	}
	#endregion states
}
