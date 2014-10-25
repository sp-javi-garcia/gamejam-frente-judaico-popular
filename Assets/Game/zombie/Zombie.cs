using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ZombieAI))]
[RequireComponent (typeof (ZombieMover))]
public class Zombie : MonoBehaviour
{
	ZombieAI _zombieAI;
	ZombieMover _zombieMover;
	public Animator Animator;

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

		Animator = GetAnimatorFromChildren(transform);
		if(Animator == null)
		{
			Debug.LogWarning("no animator found");
		}
	}

	Animator GetAnimatorFromChildren(Transform trans)
	{
		Animator anim = trans.GetComponent<Animator>();
		if(anim != null)
		{
			return anim;
		}
		else
		{
			foreach (Transform child in trans)
			{
				anim = GetAnimatorFromChildren(child);
				if(anim != null)
				{
					return anim;
				}
			}
		}

		return null;
	}

	public void Seek(Vector3 targetPosition)
	{
		_zombieAI.Seek(targetPosition);
	}

	public void SeekBrain(Vector3 targetPosition)
	{
		_zombieAI.SeekBrain(targetPosition);
	}

	public void OnBeingOverwhelm(Vector3 position, float forceMagnitude)
	{
		_zombieAI.BeingOverwhelm(position, forceMagnitude);
	}

	public void EatBrain()
	{
		_zombieAI.EatBrain();
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
			_zombieMover.MovementParameters.DefaultMaxVelocity = TwoLegMaxVelocity;
			break;

        case ZombieMode.ONE_LEGS:
			_zombieMover.MovementParameters.MaxVelocity = OneLegMaxVelocity;
			_zombieMover.MovementParameters.DefaultMaxVelocity = OneLegMaxVelocity;
			break;

		case ZombieMode.ZERO_LEGS:
			_zombieMover.MovementParameters.MaxVelocity = ZeroLegMaxVelocity;
			_zombieMover.MovementParameters.DefaultMaxVelocity = ZeroLegMaxVelocity;
            break;
        }
	}
}
