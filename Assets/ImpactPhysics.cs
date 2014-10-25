using UnityEngine;
using System.Collections;

public class ImpactPhysics : MonoBehaviour
{
	float _reductionSpeed = 100f;

	Vector3 _forceDir;
	float _forceMagnitude;

	Timer _timer = new Timer();

	enum State
	{
		IDLE,
		IMPACTING
	}
	State _state = State.IDLE;

	public void ApplyImpact(Vector3 sourcePosition, float forceMagnitude, float radius, float maxTime)
	{
		Vector3 dir = (transform.position - sourcePosition);
		float distance = dir.magnitude;
		float distanceNorm = distance / radius;

		float strength = Mathf.Max(1f - distanceNorm, 0f);
		Vector3 force = dir * Mathf.Pow(strength, 1f);

		// Apply force
		rigidbody.velocity = force;

		// Time to stop moving
		_timer.WaitForSeconds(maxTime);

		_state = State.IMPACTING;
	}

	void Update()
	{
		switch (_state) 
		{
		case State.IDLE:
			IdleState();
			break;

		case State.IMPACTING:
			ImpactingState();
			break;
		}
	}

	void IdleState()
	{
	}

	void ImpactingState()
	{
		float normalizedVel = (1f - _timer.GetNormalizedTime());
		normalizedVel = Mathf.Pow(normalizedVel, 1f);
		
		rigidbody.velocity *= normalizedVel;

		if(normalizedVel <= 1e-1f)
		{
			_state = State.IDLE;
		}
	}
}
