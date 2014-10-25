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

		CHASING_BRAIN,
		EATING_BRAIN,

		BEING_OVERWELMED,
	}

	State _state = State.IDLE;

	Zombie _zombie;
	ZombieMover _zombieMover;

	Vector3 _target;

	#region overwelm
	Timer _overWhelmTimer = new Timer();
	#endregion

	ZombieCameraController _cameraController;

	private static float _lastTimeZombieEat = 0f;
	private static float _minTimeBetweenZoomEfect = 10f;

	void Awake()
	{
		_zombie = GetComponent<Zombie>();
		if(_zombie == null)
		{
			Debug.LogWarning("Zombie is NOT found!!");
		}

		_zombieMover = GetComponent<ZombieMover>();

		_cameraController = FindObjectOfType<ZombieCameraController>();
	}

	void Update()
	{
//		Debug.Log("state: " + _state.ToString());

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

		case State.CHASING_BRAIN:
			ChasingBrainState();
			break;

		case State.EATING_BRAIN:
			EatingBrainState();
			break;
			
		case State.BEING_OVERWELMED:
			BeingOverWelmedState();
			break;
		}
	}

	public void Seek(Vector3 targetPos)
	{
		if(!CanChangeState(State.CHASING))
		{
			return;
		}
		OnPreChangeState(State.CHASING);

		_target = targetPos;
		_state = State.CHASING;
	}

	public void SeekBrain(Vector3 targetPos)
	{
		if(!CanChangeState(State.CHASING_BRAIN))
		{
			return;
		}

		if(targetPos == _target)
		{
			return;
		}

		OnPreChangeState(State.CHASING_BRAIN);

		_target = targetPos;
		_state = State.CHASING_BRAIN;
	}

	public void EatBrain()
	{
		if(!CanChangeState(State.CHASING_BRAIN))
		{
			return;
		}

		_zombieMover.StopMovement();
		_zombie.Animator.SetBool("eat", true);
		_state = State.EATING_BRAIN;
	}

	bool CanChangeState(State newState)
	{
		if(_state == State.BEING_OVERWELMED)
		{
			return false;
		}

		return true;
	}

	void OnPreChangeState(State newState)
	{
		if(_state == State.EATING_BRAIN)
		{
			_zombie.Animator.SetBool("eat", false);
		}
	}

	public void BeingOverwhelm(Vector3 position, float force)
	{
		if(_state == State.BEING_OVERWELMED)
		{
			return;
		}

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
		_zombie.Animator.SetFloat("speed", rigidbody.velocity.magnitude / _zombieMover.MovementParameters.DefaultMaxVelocity);

		// Find the closest obstacle and follow it!
		_zombieMover.Seek(_target);
	}

	void ChasingBrainState()
	{
		float distanceToTarget = (_target - transform.position).magnitude;
		if(distanceToTarget < 3f)
		{
			_zombieMover.StopMovement();
			_zombie.Animator.SetFloat("speed", 0f);
			_zombie.Animator.SetBool("eat", true);
			_state = State.EATING_BRAIN;
		}
		else
		{
			_zombie.Animator.SetFloat("speed", rigidbody.velocity.magnitude / _zombieMover.MovementParameters.DefaultMaxVelocity);
		
			// Find the closest obstacle and follow it!
			_zombieMover.Seek(_target);
		}
	}

	void EatingBrainState()
	{
		if((Time.timeSinceLevelLoad - _lastTimeZombieEat) > _minTimeBetweenZoomEfect)
		{
			_cameraController.ZoomToPosition(_target, 0.3f);
			_lastTimeZombieEat = Time.timeSinceLevelLoad;
		}

		Quaternion quat = Quaternion.LookRotation((_target - transform.position).normalized);
		transform.rotation = quat;

		_zombieMover.StopMovement();
		_zombie.Animator.SetFloat("speed", 0f);
		// Do nothing
	}

	void BeingOverWelmedState()
	{
		if(_overWhelmTimer.IsFinished())
		{
			_state = State.CHASING;
//			_zombie.Animator.SetBool("idle", false);
		}
	}

	#endregion States
}
