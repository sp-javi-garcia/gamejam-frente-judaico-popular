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
	ZombieJailManager _jailManager;

	float distanceToEatBrain = 5f;

    public static ZombieSquad Instance;

	private Timer _startTimer = new Timer();

    public bool WaitingToStart = true;

	public float MaxDistanceToLiberateJail = 10f;

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

		_jailManager = FindObjectOfType<ZombieJailManager>();

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
        if (WaitingToStart)
        {
            for(int i = 0; i < Zombies.Count; ++i)
            {
                Zombies[i].gameObject.rigidbody.velocity = Vector3.zero;
            }
            return;
        }
		Vector3 position;
        Brain brain;
		bool found = FindClosestBrainFromPosition(AveragePosition, out position, out brain);

		if(!found)
		{
			ZombieJail jail = null;
			found = FindClosestJailFromPosition(AveragePosition, out position, out jail);
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
					
					zombie.LiberateJail(jail);
				}
			}
		}
		else
		{
			for (int i = 0; i < _zombies.Count; ++i)
			{
				Zombie zombie = _zombies[i];
				
                zombie.SeekBrain(position, brain);
			}
            SetLastTargetedBrain(brain);
		}
	}

	bool FindClosestJailFromPosition(Vector3 position, out Vector3 foundPosition, out ZombieJail zombieJail)
	{
		foundPosition = Vector3.zero;
		zombieJail = null;
		float closestDist = float.MaxValue;
		bool found = false;
		
		for (int i = 0; i < _jailManager.Jails.Count; ++i)
		{
			ZombieJail currentJail = _jailManager.Jails[i];

			if(currentJail.IsLiberated())
			{
				continue;
			}
			
			float distance = (currentJail.transform.position - position).magnitude;
			if(distance < MaxDistanceToLiberateJail && distance < closestDist)
			{
				found = true;

				foundPosition = currentJail.transform.position;
				closestDist = distance;
				zombieJail = currentJail;
			}
		}
		
		return found;
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

    Vector3 _brainDirection = Vector3.zero;
    float _remainingBrainDirectionTime = 0f;
    public void SetLastTargetedBrain(Brain brain)
    {
        _brainDirection = Vector3.Normalize(brain.transform.position - AveragePosition);
    }

	Vector3 CalculateSquadAvgForward()
	{
        Vector3 targetVector = _humanBase.transform.position - transform.position;
        if (targetVector.magnitude > 10f)
        {
            targetVector.Normalize();
            float xComponentTarget = targetVector.x - targetVector.z;
            float xComponentBrainDirection = _brainDirection.x - _brainDirection.z;
            if (xComponentTarget + xComponentBrainDirection > 0f)
                targetVector.z = 0f;
            else
                targetVector.x = 0f;
        }
        return Vector3.Normalize(targetVector);
         
        /*
        _remainingBrainDirectionTime -= Time.deltaTime;
        if (_remainingBrainDirectionTime <= 0f)
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
        else
        {
            return _brainDirection;
        }
        */
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
        if (_zombies.Count == 0)
        {
            GameController.Instance.OutOfZombies();
        }
    }
}
