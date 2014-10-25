using UnityEngine;
using System.Collections;

public class Timer
{
	float _endTime;
	float _startTime;

	public void WaitForSeconds(float sec)
	{
		_startTime = Time.timeSinceLevelLoad;
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

	public float GetNormalizedTime()
	{
		float deltaTimeNorm = (_endTime - Time.timeSinceLevelLoad) / (_endTime - _startTime);
		deltaTimeNorm = Mathf.Max(deltaTimeNorm, 0f);

		return (1f-deltaTimeNorm);
	}
}
