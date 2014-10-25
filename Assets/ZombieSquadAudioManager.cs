using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
public class ZombieSquadAudioManager : MonoBehaviour
{
	AudioSource _audioSource;

	[SerializeField]
	AudioClip _iceZoneClip;

	[SerializeField]
	AudioClip _landZoneClip;

	[SerializeField]
	AudioClip _fireClip;

	AudioClip _currentClip;

	Timer _timer = new Timer();

	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	public void PlayIceZoneClip()
	{
		_timer.WaitForSeconds(2f);

		if(!_audioSource.isPlaying)
		{
			_audioSource.clip = _iceZoneClip;
			_audioSource.Play();
			AudioSource.PlayClipAtPoint(_iceZoneClip, transform.position);
		}
	}

	public void PlayFireZoneClip()
	{
		_timer.WaitForSeconds(2f);

		if(!_audioSource.isPlaying)
		{
            _audioSource.clip = _fireClip;
			_audioSource.Play();
		}
	}

	public void PlayLandZoneClip()
	{
		_timer.WaitForSeconds(2f);
		if(!_audioSource.isPlaying)
		{
			_audioSource.clip = _landZoneClip;
			_audioSource.Play();
		}
	}

	void Update()
	{
		if(_timer.IsFinished())
		{
			_audioSource.Stop();
		}
	}

	public void StopIceZoneClip()
	{
//		if(_audioSource.clip == _iceZoneClip)
//		{
//			_audioSource.Stop();
//		}
	}
	
	public void StopFireZoneClip()
	{
//		if(_audioSource.clip == _fireClip)
//		{
//			_audioSource.Stop();
//		}
	}
	
	public void StopLandZoneClip()
	{
//		if(_audioSource.clip == _landZoneClip)
//        {
//			_audioSource.Stop();
//        }
    }
}
