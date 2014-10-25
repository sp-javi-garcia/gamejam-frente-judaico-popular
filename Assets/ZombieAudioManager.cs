using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class ZombieAudioManager : MonoBehaviour 
{
	[SerializeField]
	AudioClip _biteClip;

	AudioSource _audioSource;

	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	public void PlayBite()
	{
		_audioSource.clip = _biteClip; 

		_audioSource.Play();
	}
}
