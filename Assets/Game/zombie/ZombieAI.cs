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
	}

	State _state = State.IDLE;

	Zombie _zombie;
	ZombieMover _zombieMover;

	Vector3 _target;

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
			Chasing();
			break;
		}
	}

	public void Seek(Vector3 targetPos)
	{
		_target = targetPos;
		_state = State.CHASING;
	}

	public void StartChasing()
	{
		_state = State.CHASING;
	}

	#region States
	void IdleState()
	{
		// Do nothing
	}

	void Chasing()
	{
		// Find the closest obstacle and follow it!
		_zombieMover.Seek(_target);
	}

	#endregion States
}
