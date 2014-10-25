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

	WaypointMover _mover;

	List<ZombieData> _zombies = new List<ZombieData>();
	ZombieSquad _squad;

	float _initialMoveSpeed;

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
		if(other.gameObject.layer == LayerMask.NameToLayer("zombie"))
		{
			Zombie zombie = other.gameObject.GetComponent<Zombie>();

			if(zombie != null && !CheckzombieWasOverwelmed(zombie))
			{
				_zombies.Add(new ZombieData(Time.timeSinceLevelLoad, zombie));
				OverwelmZombie(zombie);
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

	void OverwelmZombie(Zombie zombie)
	{
		zombie.OnBeingOverwhelm(transform.position, _forceMagnitude);
	}
}
