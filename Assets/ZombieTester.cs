using UnityEngine;
using System.Collections;

public class ZombieTester : MonoBehaviour
{
	Zombie[] _zombies = new Zombie[0];

//	public Vector3 TargetPos;
	public Transform Target;
	public Vector3 TargetPos
	{
		get { return Target.position; }
	}

	public bool Start = false;

	void Awake()
	{
		_zombies = FindObjectsOfType<Zombie>();
	}

	void Update()
	{
		if(Start)
		{
			foreach (var item in _zombies)
			{
				item.Seek(TargetPos);
			}
		}
	}


}
