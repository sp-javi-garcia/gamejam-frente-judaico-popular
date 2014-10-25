using UnityEngine;
using System.Collections;


public class ZombieSquadReactions : MonoBehaviour
{
//	ZombieSquad _squad;
//
//	[SerializeField]
//	public float ZeroLegMaxVelocity = 3f;
//	
//	[SerializeField]
//	public float FireVelocityFactor = 1.2f;
//	
//	[SerializeField]
//	public float IceVelocityFactor = 2.0f;
//	
//	[SerializeField]
//	public float  LandVelocityFactor = 3f;
//	
//	public float  DefaultVelocityFactor = 1f;
//	
//	public float _velocityFactor = 1f;
//
//	// Use this for initialization
//	void Awake ()
//	{
//		_squad = transform.parent.GetComponent<ZombieSquad>();
//	}
//
//	void OnTriggerEnter(Collider other)
//	{
//		if(other.tag == "fire" && _velocityFactor != 1)
//		{
//			_squad.AudioManager.PlayFireZoneClip();
//
//			SetVelocityFactor(FireVelocityFactor);
//		}
//		else if(other.tag == "ice" && _velocityFactor != 1)
//		{
//			_squad.AudioManager.PlayIceZoneClip();
//
//			SetVelocityFactor(IceVelocityFactor);
//		}
//		else if(other.tag == "land" && _velocityFactor != 1)
//		{
//			_squad.AudioManager.PlayLandZoneClip();
//
//			SetVelocityFactor(LandVelocityFactor);
//		}
//	}
//
//	void SetVelocityFactor(float factor)
//	{
//		for (int i = 0; i < _squad.Zombies.Count; ++i)
//		{
//			_squad.Zombies[i].ZombieMover.MovementParameters.MaxVelocity *= _velocityFactor;
//			_squad.Zombies[i].ZombieMover.MovementParameters.DefaultMaxVelocity *= _velocityFactor;
//		}
//
//		_velocityFactor = factor;
//	}
//	
//	void OnTriggerExit(Collider other)
//	{
//		if(other.tag == "fire")
//		{
//			ResetVelocityFactor();
//			_squad.AudioManager.StopFireZoneClip();
//			
//			_velocityFactor = 1f;
//		}
//		else if(other.tag == "ice")
//		{
//			ResetVelocityFactor();
//			_squad.AudioManager.StopIceZoneClip();
//			
//			_velocityFactor = 1f;
//		}
//		else if(other.tag == "land")
//		{
//			ResetVelocityFactor();
//			_squad.AudioManager.StopLandZoneClip();
//			
//			_velocityFactor = 1f;
//		}
//	}
//
//	void ResetVelocityFactor()
//	{
//		for (int i = 0; i < _squad.Zombies.Length; ++i)
//		{
//			_squad.Zombies[i].ZombieMover.MovementParameters.MaxVelocity /= _velocityFactor;
//			_squad.Zombies[i].ZombieMover.MovementParameters.DefaultMaxVelocity /= _velocityFactor;
//		}
//
//		_velocityFactor = 1f;
//	}
}
