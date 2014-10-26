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

	enum SoundType
	{
		Eat,
		Fired,
		BusHit
	}
	SoundType _currentSoundType;
	float _maxTime = 99f;
	float _startedTime = 0f;

	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		float deltaTime = Time.timeSinceLevelLoad - _startedTime;
	}

	public void PlayEat(float maxTime = 1f)
	{
		_maxTime = maxTime;
		_startedTime = Time.timeSinceLevelLoad;

		_audioSource.clip = SelectRandomFromList(_eatClips); 

		_audioSource.Play();

		_currentSoundType = SoundType.Eat;
	}

	public void PlayFired(float maxTime = 1f)
	{
		_maxTime = maxTime;
		_startedTime = Time.timeSinceLevelLoad;

		_audioSource.clip = SelectRandomFromList(_firedClips);
		
		_audioSource.Play();

		_currentSoundType = SoundType.Fired;
	}

	public void PlayBusHit(float maxTime = 1f)
	{
		_maxTime = maxTime;
		_startedTime = Time.timeSinceLevelLoad;

		_audioSource.clip = SelectRandomFromList(_busClips);
		
		_audioSource.Play();

		_currentSoundType = SoundType.BusHit;
	}

	public void StopFired(float maxTime = 1f)
	{
		_maxTime = maxTime;
		_startedTime = Time.timeSinceLevelLoad;

		if(_currentSoundType == SoundType.Fired)
		{
			_audioSource.Stop();
		}
	}

	AudioClip SelectRandomFromList(List<AudioClip> list)
	{
		int idx = Random.Range(0, list.Count);

		return list[idx];
	}
}
