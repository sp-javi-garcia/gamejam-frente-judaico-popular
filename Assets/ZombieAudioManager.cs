using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof(AudioSource))]
public class ZombieAudioManager : MonoBehaviour 
{
	[SerializeField]
	List<AudioClip> _firedClips;

	[SerializeField]
	List<AudioClip> _busClips;

	[SerializeField]
	List<AudioClip> _eatClips;

	AudioSource _audioSource;

	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	public void PlayEat()
	{
		_audioSource.clip = SelectRandomFromList(_eatClips); 

		_audioSource.Play();
	}

	public void PlayFired()
	{
		_audioSource.clip = SelectRandomFromList(_firedClips);
		
		_audioSource.Play();
	}

	public void BusFired()
	{
		_audioSource.clip = SelectRandomFromList(_busClips);
		
		_audioSource.Play();
	}

	AudioClip SelectRandomFromList(List<AudioClip> list)
	{
		int idx = Random.Range(0, list.Count);

		return list[idx];
	}
}
