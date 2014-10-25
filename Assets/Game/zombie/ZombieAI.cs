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

		BEING_BURNED,

		SEEKING_JAIL,
		LIBERATING_JAIL,

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
	Timer _burningTimer = new Timer();
	#endregion

	ZombieJail _jail;

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

		case State.BEING_BURNED:
			BeingBurnedState();
			break;

        case State.DEATH:
            DeathState();
            break;

		case State.SEEKING_JAIL:
			SeekingJailState();
			break;

		case State.LIBERATING_JAIL:
			LiberatingJailState();
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
		if(_state == State.BEING_OVERWELMED || _state == State.BEING_PUSHED)
		{
			return;
		}

		OnPreChangeState(State.CHASING);

		_target = targetPos;
		_state = State.CHASING;
	}

	public void SeekBrain(Vector3 targetPos, Brain brain)
	{
		if(_state == State.BEING_OVERWELMED || _state == State.BEING_PUSHED)
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

	public void SeekingJailState()
	{
		float distanceToTarget = (_jail.transform.position - transform.position).magnitude;
		if(distanceToTarget < 3f)
		{
			_zombieMover.StopMovement();
			_zombie.SetAnimatorFloat("speed", 0f);
			_waitToAnimate.WaitForSeconds(0.5f);

			_zombie.SetAnimatorBool("eat", true);
			_state = State.LIBERATING_JAIL;
		}
		else
		{
			_zombie.SetAnimatorFloat("speed", rigidbody.velocity.magnitude / _zombieMover.MovementParameters.DefaultMaxVelocity);
			
			_zombieMover.Seek(_jail.transform.position);
		}
	}

	public void LiberatingJailState()
	{	
		_jail.LiberateZombies();

		if(_jail.IsLiberated())
		{
			_state = State.CHASING;
		}
	}

	public void EatBrain()
	{
		if(_state == State.BEING_OVERWELMED || _state == State.BEING_PUSHED)
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

	void OnPreChangeState(State newState)
	{
		if(_state == State.EATING_BRAIN)
		{
			_zombie.SetAnimatorBool("eat", false);
		}
	}

	public void BeingOverwhelm(Vector3 position, float forceMagnitude, int lifesToKill, float radius = 3f)
	{
		if(_state == State.BEING_OVERWELMED || _state == State.BEING_PUSHED)
		{
			return;
		}

		OnPreChangeState(State.BEING_OVERWELMED);

		_zombie.Life -= lifesToKill;
		bool isDead = _zombie.Life <= 0;

		_zombie.SetAnimatorFloat("speed", 0f);
		_zombie.SetAnimatorBool("dead", isDead);
		_zombie.SetAnimatorBool("hit", true);

		_zombieMover.StopMovement();
		_overWhelmTimer.WaitForSeconds(5f);

		_zombieMover.StopMovement();
		rigidbody.AddExplosionForce(forceMagnitude, position, radius, 1f, ForceMode.Impulse);
		_pushTimer.WaitForSeconds(2f);

		_state = State.BEING_OVERWELMED;
	}

	public void BeingPushed(Vector3 position, float forceMagnitude, int lifesToKill, float radius = 3f)
	{
		if(_state == State.BEING_OVERWELMED || _state == State.BEING_PUSHED)
		{
			return;
		}

		OnPreChangeState(State.BEING_PUSHED);

		_zombie.Life -= lifesToKill;
		bool isDead = _zombie.Life <= 0;

		_zombie.SetAnimatorFloat("speed", 0f);
		_zombie.SetAnimatorBool("dead", isDead);
		_zombie.SetAnimatorBool("hit", true);
		
		_zombieMover.StopMovement();
        rigidbody.AddExplosionForce(forceMagnitude, position, radius, 1f, ForceMode.Impulse);
		_pushTimer.WaitForSeconds(2f);

		_state = State.BEING_PUSHED;
	}

	public void BeingBurned()
	{
		if(_state == State.BEING_OVERWELMED || _state == State.BEING_PUSHED || _state == State.BEING_BURNED)
		{
			return;
		}

		_zombie.Life -= 1;

		_burningTimer.WaitForSeconds(1.25f);

		_state = State.BEING_BURNED;
	}

	public void StopBeingBurned()
	{
		if(_state != State.BEING_BURNED)
		{
			return;
		}
		
		_state = State.CHASING;
	}

	public void LiberateJail(ZombieJail jail)
	{
		Debug.Log("Liberate Jail");
		_jail = jail;

		if(_state == State.BEING_OVERWELMED || _state == State.BEING_PUSHED || _state == State.LIBERATING_JAIL || _state == State.SEEKING_JAIL)
		{
			return;
		}

		_state = State.SEEKING_JAIL;
	}

	#region States
	void IdleState()
	{
		// Do nothing
	}

	void ChasingState()
	{
        _zombie.SetAnimatorFloat("speed", rigidbody.velocity.magnitude / _zombieMover.MovementParameters.DefaultMaxVelocity);

		_zombieMover.Seek(_target);
	}

	void ChasingBrainState()
	{
		float distanceToTarget = (_target - transform.position).magnitude;
		if(distanceToTarget < 3f)
		{
			_zombieMover.StopMovement();
			_zombie.SetAnimatorFloat("speed", 0f);
			_waitToAnimate.WaitForSeconds(0.5f);
			_state = State.EATING_BRAIN;
            _targetBrain.SetEating();
		}
		else
		{
            _zombie.SetAnimatorFloat("speed", rigidbody.velocity.magnitude / _zombieMover.MovementParameters.DefaultMaxVelocity);
		
			_zombieMover.Seek(_target);
		}
	}

	void EatingBrainState()
	{
		_zombieMover.StopMovement();
		_zombie.SetAnimatorFloat("speed", 0f);

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

	}

	void BeingOverWelmedState()
	{
		_zombieMover.StopMovement();
		_zombie.SetAnimatorFloat("speed", 0f);

		if(_overWhelmTimer.IsFinished())
		{
			if(_zombie.Life > 0)
			{
				_state = State.CHASING;
			}
			else
            {
                _zombie.ProcessDie();
			}
		}
	}

	void BeingPushedState()
	{
		_zombieMover.StopMovement();
		_zombie.SetAnimatorFloat("speed", 0f);

		if(_pushTimer.IsFinished())
		{
			if(_zombie.Life > 0)
			{
				_state = State.CHASING;
			}
			else
			{
                _zombie.ProcessDie();
            }
		}
	}

	void BeingBurnedState()
	{
		if(_burningTimer.IsFinished())
		{
			_zombie.Life -= 1;

			if(_zombie.Life > 0)
			{
				_burningTimer.WaitForSeconds(1f);
			}
			else
			{
				_zombie.ProcessDie();
			}
		}
	}

	#endregion States
}
