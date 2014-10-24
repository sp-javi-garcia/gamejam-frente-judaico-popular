using UnityEngine;
using System.Collections;

public class ZombieSquad : MonoBehaviour
{
	Zombie [] _zombies;
	public Zombie [] Zombies
	{
		get { return _zombies; }
	}

	void Awake()
	{
		_zombies = GetComponentsInChildren<Zombie>();
	}
}
