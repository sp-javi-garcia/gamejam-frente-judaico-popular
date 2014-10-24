using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ZombieAI))]
public class Zombie : MonoBehaviour
{
	ZombieAI _zombieAI;

	ZombieSquad _squad;
	public ZombieSquad Squad
	{
		get { return _squad; }
	}

	void Awake()
	{
		_zombieAI = GetComponent<ZombieAI>();
		_squad = transform.parent.GetComponent<ZombieSquad>();

		if(_squad == null)
		{
			Debug.LogWarning("No Squad Found!!!");
		}
	}

	void Start()
	{
		_zombieAI.StartChasing();
	}

	public void StartChasing()
	{
		_zombieAI.StartChasing();
	}

	public void Seek(Vector3 targetPosition)
	{
		_zombieAI.Seek(targetPosition);
	}
}
