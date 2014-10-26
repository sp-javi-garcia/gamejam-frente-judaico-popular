using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof (AudioSource))]
public class TorretAudioManager : MonoBehaviour
{
	AudioSource _audioSource;

	[SerializeField]
	List<AudioClip> _shots;

	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	public void Shot()
	{
		_audioSource.clip = _shots[Random.Range(0, _shots.Count)];
		_audioSource.Play();
	}
}
