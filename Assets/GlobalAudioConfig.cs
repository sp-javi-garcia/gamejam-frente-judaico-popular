using UnityEngine;
using System.Collections;

public class GlobalAudioConfig : MonoBehaviour
{
	void Awake()
	{
		AudioListener.volume = 1f;
	}
}
