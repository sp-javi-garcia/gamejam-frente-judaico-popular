using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Zombie))]
[RequireComponent (typeof (ZombieMover))]
public class ZombieAI : MonoBehaviour
{
	enum State
	{
		IDLE,

		START_CHASING,
		CHASING,

		BEING_OVERWELMED,
	}

	State _state = State.IDLE;

	Zombie _zombie;
	ZombieMover _zombieMover;

	Vector3 _target;

	#region overwelm
	Timer _overWhelmTimer = new Timer();
	#endregion

	void Awake()
	{
		_zombie = GetComponent<Zombie>();
		_zombieMover = GetComponent<ZombieMover>();
	}

	void Update()
	{
		switch (_state)
		{
		case State.IDLE:
			IdleState();
			break;

		case State.START_CHASING:
			IdleState();
			break;
		
		case State.CHASING:
			ChasingState();
			break;
			
		case State.BEING_OVERWELMED:
			BeingOverWelmedState();
			break;
		}
	}

	public void Seek(Vector3 targetPos)
	{
		if(_state == State.BEING_OVERWELMED)
		{
			return;
		}

		_target = targetPos;
		_state = State.CHASING;
	}

	public void BeingOverwhelm(Vector3 position, float force)
	{
		_zombieMover.StopMovement();
		rigidbody.AddExplosionForce(force, position, 3f, 1f, ForceMode.Impulse);

		_overWhelmTimer.WaitForSeconds(1f);

		_state = State.BEING_OVERWELMED;
	}

	#region States
	void IdleState()
	{
		// Do nothing
	}

	void ChasingState()
	{
		// Find the closest obstacle and follow it!
		_zombieMover.Seek(_target);
	}

	void BeingOverWelmedState()
	{
		if(_overWhelmTimer.IsFinished())
		{
			_state = State.CHASING;
		}
	}

	#endregion States
}
