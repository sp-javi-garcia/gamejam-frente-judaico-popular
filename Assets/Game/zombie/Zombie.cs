using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ZombieAI))]
[RequireComponent (typeof (ZombieMover))]
public class Zombie : MonoBehaviour
{
	ZombieAI _zombieAI;
	ZombieMover _zombieMover;

	enum ZombieMode
	{
		TWO_LEGS,
		ONE_LEGS,
		ZERO_LEGS
	}
	ZombieMode _mode = ZombieMode.TWO_LEGS;

	[SerializeField]
	public float TwoLegMaxVelocity = 10f;

	[SerializeField]
	public float OneLegMaxVelocity = 6f;

	[SerializeField]
	public float ZeroLegMaxVelocity = 3f;

	ZombieSquad _squad;
	public ZombieSquad Squad
	{
		get { return _squad; }
	}

	void Awake()
	{
		_zombieAI = GetComponent<ZombieAI>();
		_zombieMover = GetComponent<ZombieMover>();

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

	void SetTwoLegMode()
	{
		_mode = ZombieMode.TWO_LEGS;
		
		UpdateParametersByMode();
    }

	void SetOneLegMode()
	{
		_mode = ZombieMode.ONE_LEGS;

		UpdateParametersByMode();
	}

	void SetZeroLegMode()
	{
		_mode = ZombieMode.ZERO_LEGS;
		
		UpdateParametersByMode();
    }

	void UpdateParametersByMode()
	{
		switch (_mode)
		{

		case ZombieMode.TWO_LEGS:
			_zombieMover.MovementParameters.MaxVelocity = TwoLegMaxVelocity;
			break;

        case ZombieMode.ONE_LEGS:
			_zombieMover.MovementParameters.MaxVelocity = OneLegMaxVelocity;
			break;

		case ZombieMode.ZERO_LEGS:
			_zombieMover.MovementParameters.MaxVelocity = ZeroLegMaxVelocity;
            break;
        }
	}
}
