using UnityEngine;
using System.Collections;

public class ZombieAudioManager : MonoBehaviour 
{
	[SerializeField]
	AudioClip _biteClip;

	public void PlayBite()
	{
		AudioSource.PlayClipAtPoint(_biteClip, transform.position);
	}
}
