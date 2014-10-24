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

public class Overwhelming : MonoBehaviour 
{
	[SerializeField]
	float _timeToCleanHItted = 3f;

	[SerializeField]
	float _forceMagnitude = 10f;

	List<ZombieData> _zombies = new List<ZombieData>();

	void Update()
	{
		ClearOldZombies();
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
