using UnityEngine;
using System.Collections;

public class ZombieSquad : MonoBehaviour
{
	Zombie[] _zombies;

	public Zombie [] Zombies 
	{
		get { return _zombies; }
	}

	public Vector3 AveragePosition;
	HumanBase _humanBase;

	BrainDepot _brainDepot;

	float distanceToEatBrain = 5f;

	void Awake ()
	{
		_zombies = GetComponentsInChildren<Zombie> ();
		_brainDepot = FindObjectOfType<BrainDepot>();
		if(_brainDepot == null)
		{
			Debug.LogWarning("brainDepot base is null");
		}

		_humanBase = FindObjectOfType<HumanBase>();
		if(_humanBase == null)
		{
			Debug.LogWarning("human base is null");
		}
	}

	void Update ()
	{
		AveragePosition = CalculateSquadAvgPosition();

		SetTargetToZombies();
	}

	void SetTargetToZombies()
	{
		Vector3 position;
		bool found = FindClosestBrainFromPosition(AveragePosition, out position);

		if(!found)
		{
			position = _humanBase.transform.position;

			for (int i = 0; i < _zombies.Length; ++i)
			{
				Zombie zombie = _zombies[i];
				
				zombie.Seek(position);
			}
		}
		else
		{
			float distanceToBrain = (AveragePosition - position).magnitude;
			if(distanceToBrain < distanceToEatBrain)
			{
				for (int i = 0; i < _zombies.Length; ++i)
				{
					Zombie zombie = _zombies[i];
					
					zombie.EatBrain();
				}
			}
			else
			{
				for (int i = 0; i < _zombies.Length; ++i)
				{
					Zombie zombie = _zombies[i];
					
					zombie.SeekBrain(position);
				}
			}
		}
	}

	bool FindClosestBrainFromPosition(Vector3 position, out Vector3 foundPosition)
	{
		foundPosition = Vector3.zero;

		for (int i = 0; i < _brainDepot.ActiveBrains.Count; ++i)
		{
			Brain brain = _brainDepot.ActiveBrains[i];

			float distance = (brain.transform.position - position).magnitude;
			if(distance < brain.Range)
			{
				foundPosition = brain.transform.position;

				return true;
			}
		}

		return false;
	}

	Vector3 CalculateSquadAvgPosition ()
	{
		Vector3 avgPos = Vector3.zero;
		if(_zombies.Length > 0)
		{
			for (int i = 0; i < _zombies.Length; ++i) 
			{
				Zombie zombie = _zombies[i];

				avgPos += zombie.transform.position;
			}

			avgPos /= _zombies.Length;
		}

		return avgPos;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;

		Gizmos.DrawWireSphere(AveragePosition, 10f);
	}
}
