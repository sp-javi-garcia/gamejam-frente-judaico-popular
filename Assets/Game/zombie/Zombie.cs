using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ZombieAI))]
[RequireComponent (typeof (ZombieMover))]
[RequireComponent (typeof (ZombieAudioManager))]
public class Zombie : MonoBehaviour
{
	ZombieAI _zombieAI;
	public ZombieMover _zombieMover;

	enum ZombieMode
	{
		TWO_LEGS,
		ONE_LEGS,
		ZERO_LEGS
	}
	ZombieMode _mode = ZombieMode.TWO_LEGS;

	[SerializeField]
	public float TwoLegMaxVelocity = 2.5f;

	[SerializeField]
	public float OneLegMaxVelocity = 1.9f;

	[SerializeField]
	public float ZeroLegMaxVelocity = 3f;
	
	[SerializeField]
	public float FireVelocityFactor = 1.2f;
	
	[SerializeField]
	public float IceVelocityFactor = 2.0f;
	
	[SerializeField]
	public float  LandVelocityFactor = 3f;
	
	public float  DefaultVelocityFactor = 1f;
	
	public float _velocityFactor = 1f;

	public ZombieAudioManager AudioManager;

	public Animator ZombieTwoLegs;
	public Animator ZombieOneLegs;
	public Animator ZombieZeroLegs;

	bool _initAnimator = false;

	ZombieSquad _squad;
	public ZombieSquad Squad
	{
		get { return _squad; }
	}

    ZombieHealthBar _healthBar;

	public void SetAnimatorBool(string param, bool value)
	{
		ZombieTwoLegs.SetBool(param, value);
		ZombieOneLegs.SetBool(param, value);
		ZombieZeroLegs.SetBool(param, value);
	}

	public void SetAnimatorFloat(string param, float value)
	{
		ZombieTwoLegs.SetFloat(param, value);
		ZombieOneLegs.SetFloat(param, value);
		ZombieZeroLegs.SetFloat(param, value);
	}

    int _life = 3;
    public int Life
    {
        get
        {
            return _life;
        }
        set
        {
            if (_life != value)
                _healthBar.SetHealth(value);
            _life = value;
            switch(_life)
            {
            case 3:
                SetTwoLegMode();
                break;
            case 2:
                SetOneLegMode();
                break;
            case 1:
                SetZeroLegMode();
                break;
            default:
                if (_life <= 0)
                {
				SetZeroLegMode();
                }
                break;
            }
        }
	}

    IEnumerator DeathAnimation()
    {
        // Animator.Play("Death");
        yield return new WaitForSeconds(1f);
        iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.zero,
                                               "easetype", iTween.EaseType.easeInBack,
                                               "time", 0.2));
    }

    IEnumerator DoInstaKill()
    {
        ZombieSquad.Instance.DeathZombie(this);
        _zombieAI.SetDeath();
        iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.zero,
                                               "easetype", iTween.EaseType.easeInBack,
                                               "time", 0.3));
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }

	// NOTE: This method is gonna be called from AI
	public void ProcessDie()
	{
	    ZombieSquad.Instance.DeathZombie(this);
	    StartCoroutine(DeathAnimation());
	}

    public void InstaKill()
    {
        StartCoroutine(DoInstaKill());
    }

	void Awake()
	{
        _healthBar = GetComponentInChildren<ZombieHealthBar>();
		_zombieAI = GetComponent<ZombieAI>();
		_zombieMover = GetComponent<ZombieMover>();
		_squad = transform.parent.GetComponent<ZombieSquad>();

		if(_squad == null)
		{
			_squad = FindObjectOfType<ZombieSquad>();
		}

		AudioManager = GetComponent<ZombieAudioManager>();

		Invoke("UpdateParametersByMode", 0.1f);
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

	public void LiberateJail(ZombieJail jail)
	{
		_zombieAI.LiberateJail(jail);
	}

	public void Seek(Vector3 targetPosition)
	{
		_zombieAI.Seek(targetPosition);
	}

	public void SeekBrain(Vector3 targetPosition, Brain brain)
	{
		AudioManager.PlayEat();

		_zombieAI.SeekBrain(targetPosition, brain);
	}

	public void OnBeingOverwhelm(Vector3 position, float forceMagnitude, int lifesToKill, float range = 3f)
	{
		AudioManager.PlayBusHit();

		_zombieAI.BeingOverwhelm(position, forceMagnitude, lifesToKill, range);
	}

	public void OnBeingPushed(Vector3 position, float forceMagnitude, int lifesToKill, float range = 3f)
    {
		AudioManager.PlayBusHit();

		_zombieAI.BeingPushed(position, forceMagnitude, lifesToKill, range);
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
			ZombieTwoLegs.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;

			ZombieOneLegs.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
			ZombieZeroLegs.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;


			_zombieMover.MovementParameters.MaxVelocity = TwoLegMaxVelocity;
			_zombieMover.MovementParameters.DefaultMaxVelocity = TwoLegMaxVelocity;
			break;

        case ZombieMode.ONE_LEGS:
			ZombieOneLegs.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;

			ZombieTwoLegs.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
			ZombieZeroLegs.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;

			_zombieMover.MovementParameters.MaxVelocity = OneLegMaxVelocity;
			_zombieMover.MovementParameters.DefaultMaxVelocity = OneLegMaxVelocity;
			break;

		case ZombieMode.ZERO_LEGS:
			ZombieZeroLegs.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;

			ZombieTwoLegs.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
			ZombieOneLegs.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;

			_zombieMover.MovementParameters.MaxVelocity = ZeroLegMaxVelocity;
			_zombieMover.MovementParameters.DefaultMaxVelocity = ZeroLegMaxVelocity;
            break;
        }
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "fire")
		{
			_velocityFactor = FireVelocityFactor;
			
			_squad.AudioManager.PlayFireZoneClip();

			_zombieMover.MovementParameters.MaxVelocity *= _velocityFactor;
			_zombieMover.MovementParameters.DefaultMaxVelocity *= _velocityFactor;

			AudioManager.PlayFired();

			_zombieAI.BeingBurned();
		}
		else if(other.tag == "ice")
		{
			_velocityFactor = IceVelocityFactor;
			
			_squad.AudioManager.PlayIceZoneClip();

			_zombieMover.MovementParameters.MaxVelocity *= _velocityFactor;
			_zombieMover.MovementParameters.DefaultMaxVelocity *= _velocityFactor;
		}
		else if(other.tag == "land")
		{
			_velocityFactor = LandVelocityFactor;
			
			_squad.AudioManager.PlayLandZoneClip();

			_zombieMover.MovementParameters.MaxVelocity *= _velocityFactor;
			_zombieMover.MovementParameters.DefaultMaxVelocity *= _velocityFactor;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.tag == "fire")
		{
			_zombieMover.MovementParameters.DefaultMaxVelocity /= _velocityFactor;
			_zombieMover.MovementParameters.MaxVelocity = _zombieMover.MovementParameters.DefaultMaxVelocity;
//			_squad.AudioManager.StopFireZoneClip();

			AudioManager.StopFired();

			_zombieAI.StopBeingBurned();
			
			_velocityFactor = 1f;
		}
		else if(other.tag == "ice")
		{
			_zombieMover.MovementParameters.DefaultMaxVelocity /= _velocityFactor;
			_zombieMover.MovementParameters.MaxVelocity = _zombieMover.MovementParameters.DefaultMaxVelocity;	
//			_squad.AudioManager.StopIceZoneClip();
			
			_velocityFactor = 1f;
		}
		else if(other.tag == "land")
		{
			_zombieMover.MovementParameters.DefaultMaxVelocity /= _velocityFactor;
			_zombieMover.MovementParameters.MaxVelocity = _zombieMover.MovementParameters.DefaultMaxVelocity;
//			_squad.AudioManager.StopLandZoneClip();
			
			_velocityFactor = 1f;
		}
	}
}
