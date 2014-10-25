using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ZombieData
{
	public float AddedTime;
	public Zombie Zombie;

	public ZombieData(float iAddedTime, Zombie iZombie)
	{
		AddedTime = iAddedTime;
		Zombie = iZombie;
	}
}

[RequireComponent (typeof (WaypointMover))]
public class Overwhelming : MonoBehaviour 
{
	[SerializeField]
	float _timeToCleanHItted = 3f;

	[SerializeField]
	float _forceMagnitude = 10f;

	[SerializeField]
	float _distanceToAccelerate = 10f;

	[SerializeField]
	float _maxTimeKilling = 10f;

	[SerializeField]
	int _livesToTakePerKick = 1;

	[SerializeField]
	int _maxLivesToKill = 7;

	int _livesKilled = 0;

	Timer _timer = new Timer();

	WaypointMover _mover;

	List<ZombieData> _zombies = new List<ZombieData>();
	ZombieSquad _squad;

	float _initialMoveSpeed;

	enum State
	{
		WALKING,
		KILLING
	}



	State _state = State.WALKING;

	void Awake()
	{
		_mover = GetComponent<WaypointMover>();
		_squad = GameObject.FindObjectOfType<ZombieSquad>();
		_initialMoveSpeed = _mover.MoveSpeed;
	}

	void Update()
	{
		ClearOldZombies();

		_mover.MoveSpeed = 1f;
		bool squadIsClose = CheckSquadIsClose();
		if(squadIsClose)
		{
			_mover.MoveSpeed = _initialMoveSpeed * 2.0f;
		}

		switch (_state)
		{
		case State.WALKING:
			WalkingState();
			break;

		case State.KILLING:
			KillingState();
			break;
		}
	}

	void WalkingState()
	{
	}

	void KillingState()
	{
		if(_timer.IsFinished())
		{
			_livesKilled = 0;
			_state = State.WALKING;
		}
	}

	bool CheckSquadIsClose()
	{
		float distanceToSquad = (_squad.AveragePosition - transform.position).magnitude;

		if(distanceToSquad < _distanceToAccelerate)
		{
			return true;
		}

		return false;
	}

	void OnTriggerEnter(Collider other)
	{
		if(_state == State.WALKING)
		{
			_state = State.KILLING;
			_timer.WaitForSeconds(_maxLivesToKill);
		}

		int realLifesToKill = Mathf.Min(_livesToTakePerKick, (_maxLivesToKill - _livesKilled));
		_maxLivesToKill -= realLifesToKill;

		_maxLivesToKill = 2;

		if(other.gameObject.layer == LayerMask.NameToLayer("zombie"))
		{
			Zombie zombie = other.gameObject.GetComponent<Zombie>();

			if(zombie != null && !CheckzombieWasOverwelmed(zombie))
			{
				Vector3 dirToZombie = transform.position - other.gameObject.transform.position;

				_zombies.Add(new ZombieData(Time.timeSinceLevelLoad, zombie));
				OverwelmZombie(zombie, dirToZombie, realLifesToKill);
			}
		}
	}

	void ClearOldZombies()
	{
		for (int i = 0; i < _zombies.Count; ++i) 
		{
			ZombieData currZombieData = _zombies[i];
			if((currZombieData.AddedTime - Time.timeSinceLevelLoad) < _timeToCleanHItted)
			{
				_zombies.RemoveAt(i);
			}
		}
	}

	bool CheckzombieWasOverwelmed(Zombie zombie)
	{
		for (int i = 0; i < _zombies.Count; ++i) 
		{
			ZombieData currZombieData = _zombies[i];

			if(currZombieData.Zombie == zombie)
			{
				return true;
			}
		}

		return false;
	}

	void OverwelmZombie(Zombie zombie, Vector3 impactDir, int lifesKilled)
	{
		Debug.Log("OverWhelkming Zomebie...");
		zombie.OnBeingOverwhelm(transform.position, _forceMagnitude, lifesKilled);
	}
}
