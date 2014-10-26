using UnityEngine;
using System.Collections.Generic;

public class BrainAudioManager : MonoBehaviour {

	AudioSource _audioSource;
	public List<AudioClip> hits;
	
	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
	}
	
	public void PlayHit()
	{
		AudioClip clip = hits[Random.Range(0, hits.Count)];
		
		_audioSource.clip = clip;
		_audioSource.Play();
	}
}
