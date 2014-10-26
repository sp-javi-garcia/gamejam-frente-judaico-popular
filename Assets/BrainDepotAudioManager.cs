using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof (AudioSource))]
public class BrainDepotAudioManager : MonoBehaviour
{
	AudioSource _audioSource;
	public List<AudioClip> throws;

	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	public void PlayThrow()
	{
		AudioClip clip = throws[Random.Range(0, throws.Count)];

		_audioSource.clip = clip;
		_audioSource.Play();
	}
}
