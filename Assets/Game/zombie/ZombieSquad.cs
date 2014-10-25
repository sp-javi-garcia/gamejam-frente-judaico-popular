using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (ZombieSquadAudioManager))]
public class ZombieSquad : MonoBehaviour
{
	List<Zombie> _zombies;
    List<Zombie> _deathZombies = new List<Zombie>();

	public ZombieSquadAudioManager AudioManager;

    public List<Zombie> Zombies
	{
		get { return _zombies; }
	}

	public Vector3 AveragePosition;
	public Vector3 AverateForward;
	HumanBase _humanBase;

	BrainDepot _brainDepot;

	float distanceToEatBrain = 5f;

    public static ZombieSquad Instance;

	private Timer _startTimer = new Timer();

	void Awake ()
	{
		AudioManager = GetComponent<ZombieSquadAudioManager>();
        Instance = this;
		_zombies = new List<Zombie>(GetComponentsInChildren<Zombie>());
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

		_startTimer.WaitForSeconds(3f);
	}

	void Update ()
	{
		AveragePosition = CalculateSquadAvgPosition();

		AverateForward = CalculateSquadAvgForward();

		SetTargetToZombies();
	}

	void SetTargetToZombies()
	{
		Vector3 position;
        Brain brain;
		bool found = FindClosestBrainFromPosition(AveragePosition, out position, out brain);

		if(!found)
		{
			if(_startTimer.IsFinished())
			{
				position = AveragePosition + AverateForward * 600;
			}
			else
			{
				position = _humanBase.transform.position;
			}

			for (int i = 0; i < _zombies.Count; ++i)
			{
				Zombie zombie = _zombies[i];
				
				zombie.Seek(position);
			}
		}
		else
		{
			for (int i = 0; i < _zombies.Count; ++i)
			{
				Zombie zombie = _zombies[i];
				
            zombie.SeekBrain(position, brain);
			}
		}
	}

	bool FindClosestBrainFromPosition(Vector3 position, out Vector3 foundPosition, out Brain foundBrain)
	{
		foundPosition = Vector3.zero;
        foundBrain = null;
		float closestDist = float.MaxValue;
		bool found = false;

		for (int i = 0; i < _brainDepot.ActiveBrains.Count; ++i)
		{
			Brain brain = _brainDepot.ActiveBrains[i];

			float distance = (brain.transform.position - position).magnitude;
			if(distance < brain.Range && distance < closestDist)
			{
				found = true;
				closestDist = distance;
				foundPosition = brain.transform.position;
                foundBrain = brain;
			}
		}

		return found;
	}

	Vector3 CalculateSquadAvgForward()
	{
		Vector3 avgForward = Vector3.zero;
		if(_zombies.Count > 0)
		{
			for (int i = 0; i < _zombies.Count; ++i) 
			{
				Zombie zombie = _zombies[i];
				
				avgForward += zombie.transform.forward;
			}
			
			avgForward /= _zombies.Count;
		}
		else
		{
			avgForward = Vector3.up;
		}
		
		return avgForward.normalized;
	}

	Vector3 CalculateSquadAvgPosition ()
	{
		Vector3 avgPos = Vector3.zero;
		if(_zombies.Count > 0)
		{
			for (int i = 0; i < _zombies.Count; ++i) 
			{
				Zombie zombie = _zombies[i];

				avgPos += zombie.transform.position;
			}

			avgPos /= _zombies.Count;
		}

		return avgPos;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;

		Gizmos.DrawWireSphere(AveragePosition, 10f);
	}

    public void AddZombie(Zombie zombie)
    {
        _zombies.Add(zombie);
    }

    public void DeathZombie(Zombie zombie)
    {
        _zombies.Remove(zombie);
        _deathZombies.Add(zombie);
    }
}
