using UnityEngine;
using System.Collections.Generic;

public class ZombieJailManager : MonoBehaviour
{
	List<ZombieJail> _jails;

	public List<ZombieJail> Jails { get { return _jails; } }

	void Awake()
	{
		_jails = new List<ZombieJail>(GetComponentsInChildren<ZombieJail>());
	}

}
