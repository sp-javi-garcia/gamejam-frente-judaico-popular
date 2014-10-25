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
		BEING_PUSHED,
        DEATH
	}

	State _state = State.IDLE;

	Zombie _zombie;
	ZombieMover _zombieMover;

	Vector3 _target;
    Brain _targetBrain;

	#region hits
	Timer _waitToAnimate = new Timer();

	Timer _overWhelmTimer = new Timer();
	Timer _pushTimer = new Timer();
	Timer _biteTimer = new Timer();
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

		case State.BEING_PUSHED:
			BeingPushedState();
			break;

        case State.DEATH:
            DeathState();
            break;
		}
	}

    public void SetDeath()
    {
        _state = State.DEATH;
    }

    void DeathState()
    {
        // Do Nothing
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

	public void SeekBrain(Vector3 targetPos, Brain brain)
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
        _targetBrain = brain;
		_target = targetPos;
		_state = State.CHASING_BRAIN;
	}

	public void EatBrain()
	{
		if(!CanChangeState(State.EATING_BRAIN))
		{
			return;
		}

		if(_waitToAnimate.IsFinished())
		{
			_zombie.SetAnimatorBool("eat", true);
		}

		_zombieMover.StopMovement();

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
			_zombie.SetAnimatorBool("eat", false);
		}
	}

	public void BeingOverwhelm(Vector3 position, Vector3 force)
	{
		if(_state == State.BEING_OVERWELMED || _state == State.BEING_PUSHED)
		{
			return;
		}

		Debug.Log("Being Overwhelmed");

		_zombieMover.StopMovement();
		rigidbody.AddExplosionForce(force.magnitude, position, 3f, 1f, ForceMode.Impulse);

		_overWhelmTimer.WaitForSeconds(1f);

		_state = State.BEING_OVERWELMED;
	}

    public void BeingPushed(Vector3 position, Vector3 force)
    {
        BeingPushed(position, force.magnitude);
    }

	public void BeingPushed(Vector3 position, float forceMagnitude, float radius = 3f)
	{
		if(_state == State.BEING_OVERWELMED || _state == State.BEING_PUSHED)
		{
			return;
		}

		Debug.Log("Being Pushed");
		
		_zombieMover.StopMovement();
        rigidbody.AddExplosionForce(forceMagnitude, position, radius, 1f, ForceMode.Impulse);
		_pushTimer.WaitForSeconds(1f);
		_state = State.BEING_PUSHED;
	}

	#region States
	void IdleState()
	{
		// Do nothing
	}

	void ChasingState()
	{
		_zombie.SetAnimatorFloat("speed", rigidbody.velocity.magnitude / _zombieMover.MovementParameters.DefaultMaxVelocity);

		// Find the closest obstacle and follow it!
		_zombieMover.Seek(_target);
	}

	void ChasingBrainState()
	{
		float distanceToTarget = (_target - transform.position).magnitude;
		if(distanceToTarget < 3f)
		{
			_zombieMover.StopMovement();
			_zombie.SetAnimatorFloat("speed", 0f);
//			_zombie.Animator.SetBool("eat", true);
			_waitToAnimate.WaitForSeconds(0.5f);
			_state = State.EATING_BRAIN;
            _targetBrain.SetEating();
		}
		else
		{
			_zombie.SetAnimatorFloat("speed", rigidbody.velocity.magnitude / _zombieMover.MovementParameters.DefaultMaxVelocity);
		
			// Find the closest obstacle and follow it!
			_zombieMover.Seek(_target);
		}
	}

	void EatingBrainState()
	{
		if(_waitToAnimate.IsFinished())
		{
			_zombie.SetAnimatorBool("eat", true);
		}

		if((Time.timeSinceLevelLoad - _lastTimeZombieEat) > _minTimeBetweenZoomEfect)
		{
			_cameraController.ZoomToPosition(_target, 0.3f);
			_lastTimeZombieEat = Time.timeSinceLevelLoad;
		}

		if(_biteTimer.IsFinished())
		{
			_biteTimer.WaitForSeconds(1f);
			_zombie.AudioManager.PlayBite();
		}

		Quaternion quat = Quaternion.LookRotation((_target - transform.position).normalized);
		transform.rotation = quat;

		_zombieMover.StopMovement();
		_zombie.SetAnimatorFloat("speed", 0f);
		// Do nothing
	}

	void BeingOverWelmedState()
	{
		if(_overWhelmTimer.IsFinished())
		{
			_state = State.CHASING;
		}
	}

	void BeingPushedState()
	{
		if(_pushTimer.IsFinished())
		{
			_state = State.CHASING;
		}
	}

	#endregion States
}
