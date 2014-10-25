using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof (Animation))]
public class ZombieJail : MonoBehaviour
{
	[SerializeField]
	List<Zombie> _zombies;

	ZombieSquad _squad;

	enum State
	{
		IDLE,
		LIBERATED
	}

	State _state = State.IDLE;
	Animation _animation;

	[SerializeField]
	public int _life = 500;

	void Awake()
	{
		if(_animation == null)
		{
			_animation.GetComponent<Animation>();
		}

		_squad = FindObjectOfType<ZombieSquad>();
		_zombies = new List<Zombie>(gameObject.GetComponentsInChildren<Zombie>());
	}

	public void LiberateZombies()
	{
		if(_state == State.LIBERATED)
		{
			return;
		}

		_life --;

		if(_life <= 0)
		{
			_animation.Play();

			_state = State.LIBERATED;
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
		for (int i = 0; i < _zombies.Count; ++i)
		{
			LiberateZombie(_zombies[i]);
		}
	}

	void LiberateZombie(Zombie zombie)
	{
		_squad.AddZombie(zombie);
		zombie.gameObject.transform.parent = _squad.gameObject.transform;
	}
}
