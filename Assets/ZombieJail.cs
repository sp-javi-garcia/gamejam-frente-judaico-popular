using UnityEngine;
using System.Collections.Generic;

public class ZombieJail : MonoBehaviour
{
	[SerializeField]
	List<Zombie> _zombies;

	ZombieSquad _squad;

	enum State
	{
		IDLE,
		LIBERATED,
		OPENING,
		OPENED,
	}

	State _state = State.IDLE;

	[SerializeField]
	Animation _animation;

	public int _life = 50;

	public float _openingTime = 0.1f;

	Timer _openingTimer = new Timer();

	void Awake()
	{
		_animation = GetComponentInChildren<Animation>();
		_squad = ZombieSquad.Instance;
		_zombies.Clear();
		_zombies = new List<Zombie>(gameObject.GetComponentsInChildren<Zombie>());
	}

	public void LiberateZombies()
	{
		if(_life <= 0)
		{
			return;
		}

		_life --;

		if(_life <= 0)
		{
			_openingTimer.WaitForSeconds(_openingTime);

			_animation.Play();

			_state = State.OPENING;
		}
	}

	public bool IsLiberated()
	{
		return _life <= 0;
	}

	void Update()
	{
		switch (_state)
		{
		case State.IDLE:
			IdleState();
			break;

		case State.OPENING:
			OpeningState();
            break;
            
		case State.LIBERATED:
			LiberatedState();
			break;
		}
	}

	void IdleState()
	{
		// Do nothing
	}

	void LiberatedState()
	{
		// Do nothing
	}

	void OpeningState()
	{
		// Do nothing
		if(_openingTimer.IsFinished())
		{
			for (int i = 0; i < _zombies.Count; ++i)
			{
				LiberateZombie(_zombies[i]);
            }

			_state = State.OPENED;
        }
    }

	void LiberateZombie(Zombie zombie)
	{
		zombie.gameObject.transform.parent = ZombieSquad.Instance.gameObject.transform;
		ZombieSquad.Instance.AddZombie(zombie);
	}
}
