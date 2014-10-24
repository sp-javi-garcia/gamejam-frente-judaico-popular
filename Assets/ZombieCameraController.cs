using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ZombieSquad))]
public class ZombieCameraController : MonoBehaviour
{
	[SerializeField]
	Camera _cameraToControl;

	[SerializeField]
	float _speed = 1f;

	[SerializeField]
	Vector3 _bias;

	ZombieSquad _squad;

	bool _isInit = false;
	Vector3 _distanceToCamera;

	void Awake()
	{
		_squad = GetComponent<ZombieSquad>();
	}

	void Update()
	{
		if(!_isInit)
		{
			_distanceToCamera = _cameraToControl.transform.position - _squad.AveragePosition;
			_isInit = true;
		}
		else
		{
			Vector3 newPosition = Vector3.Lerp(_cameraToControl.transform.position, _squad.AveragePosition + _distanceToCamera + _bias, Time.deltaTime * _speed);
			_cameraToControl.transform.position = newPosition;
		}
	}
}
