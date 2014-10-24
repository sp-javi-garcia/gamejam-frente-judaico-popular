using UnityEngine;
using System.Collections;

public class Timer
{
	float _endTime;

	public void WaitForSeconds(float sec)
	{
		_endTime = Time.timeSinceLevelLoad + sec;
	}

	public bool IsFinished()
	{
		return Time.timeSinceLevelLoad > _endTime;
	}

	public bool IsWaitting()
	{
		return !IsFinished();
	}
}
