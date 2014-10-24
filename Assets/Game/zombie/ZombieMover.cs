using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BoidParameters
{
	[SerializeField]
	public float SeparationDistance = 5f;
	[SerializeField]
	public float AttractionDistance = 600f;
	[SerializeField]
	public float AlginmentDistance = 30f;
	[SerializeField]
	public float MaxObstacleAheadDistance = 10f;

	[SerializeField]
	public float MaxSeparationSpeed = 5f;
	[SerializeField]
	public float MaxAlginmentSpeed = 1f;
	[SerializeField]
	public float MaxAttractionSpeed = 1f;
	[SerializeField]
	public float MaxSeekSpeed = 1f;
	[SerializeField]
	public float MaxObstacleSpeed = 10f;
    
	[SerializeField]
	public float SeparationFactor = 1f;
	[SerializeField]
	public float AlignmentFactor = 1f;
	[SerializeField]
	public float AttractionFactor = 1f;
	[SerializeField]
	public float SeekFactor = 1f;
	[SerializeField]
	public float ObstacleFactor = 1f;
}

[System.Serializable]
public class MovementParameters
{
	[SerializeField]
	public float MaxVelocity = 10f;

	[SerializeField]
	public float ArrivingRadius = 10f;

	[SerializeField]
	public float RotationSpeed = 1f;
}

[RequireComponent (typeof (Zombie))]
[RequireComponent (typeof (Rigidbody))]
public class ZombieMover : MonoBehaviour
{
	enum State
	{
		IDLE,
		SEEK
	}

	State _state = State.IDLE;

	Zombie _zombie;
	Vector3 _targetPosition;

	[SerializeField]
	MovementParameters MovementParameters;

	[SerializeField]
	BoidParameters BoidParameters;

	List<Zombie> _separationList = new List<Zombie>();
	List<Zombie> _alignmentList = new List<Zombie>();
	List<Zombie> _attractionList = new List<Zombie>();

	public Vector3 SeparationSteering;
	public Vector3 AlignmentSteering;
	public Vector3 AttractionSteering;

	void Awake()
	{
		_zombie = GetComponent<Zombie>();
	}

	public void Seek(Vector3 targetPos)
	{
		_targetPosition = targetPos;
		_state = State.SEEK;
	}

	void Update()
	{
		switch (_state)
		{
		case State.IDLE:
			IdleState();
			break;

		case State.SEEK:
			SeekState();
			break;
		}
	}

	#region states
	void IdleState()
	{
		// Do nothing
	}

	void SeekState()
	{
		Vector3 seekForce = SeekSteering();
		seekForce = Truncate(seekForce, BoidParameters.MaxSeekSpeed) * BoidParameters.SeekFactor;

		CalculateBoidForces();
		Vector3 boidForces = Vector3.zero;
		boidForces = Truncate(SeparationSteering, BoidParameters.MaxSeparationSpeed) * BoidParameters.SeparationFactor + Truncate(AlignmentSteering, BoidParameters.MaxAlginmentSpeed) * BoidParameters.AlignmentFactor + Truncate (AttractionSteering, BoidParameters.MaxAttractionSpeed) * BoidParameters.AttractionFactor;

		Vector3 steeringVelocity = Vector3.zero;
		Vector3 obstacleForce = Truncate(CalculateObstacleSteering(), BoidParameters.MaxObstacleSpeed) * BoidParameters.ObstacleFactor;

		if(obstacleForce.magnitude > 1e-1f)
		{
			seekForce *= 0.25f;
		}
		steeringVelocity += (seekForce + boidForces + obstacleForce) * Time.deltaTime;

		Vector3 newVelocity = rigidbody.velocity + steeringVelocity;
		newVelocity = Truncate (newVelocity, MovementParameters.MaxVelocity);
		
		rigidbody.velocity = newVelocity;

		// Rotate towards the velocity
		Quaternion desiredQuat = Quaternion.LookRotation(newVelocity);
		Quaternion newQuat = Quaternion.Slerp(transform.rotation, desiredQuat, Time.deltaTime * MovementParameters.RotationSpeed);
		transform.rotation = newQuat;
	}

	Vector3 Truncate(Vector3 vector, float maxMagnitude)
	{
		float vectorMagnitude = vector.magnitude;

		if(vectorMagnitude > maxMagnitude)
		{
			vector = vector.normalized * maxMagnitude;
		}

		return vector;
	}

	void CalculateBoidForces()
	{
		Zombie [] zombies = _zombie.Squad.Zombies;

		_separationList.Clear();
		_alignmentList.Clear(); 
		_attractionList.Clear();

		Vector3 boidSteeringForce = Vector3.zero;

		for (int i = 0; i < zombies.Length; ++i) 
		{
			Zombie currZombie = zombies[i];

			if(currZombie == _zombie)
			{
				continue;
			}

			// Separation
			float distanceToZombie = (currZombie.transform.position - transform.position).magnitude;
			if(distanceToZombie < BoidParameters.SeparationDistance)
			{
				_separationList.Add(currZombie);
			}

			// Alignment
			else if(distanceToZombie < BoidParameters.AlginmentDistance)
			{
				_alignmentList.Add(currZombie);
			}

			// Attraction
			else if(distanceToZombie < BoidParameters.AttractionDistance)
			{
				_attractionList.Add(currZombie);
			}
		}

		// Calculate Boid Forces

		// Separation
		SeparationSteering = Vector3.zero;
		Vector3 desiredSeparationVelocity = Vector3.zero;
		if(_separationList.Count > 0)
		{
			for (int i = 0; i < _separationList.Count; ++i)
			{
				desiredSeparationVelocity += transform.position - _separationList[i].transform.position;
			}
			desiredSeparationVelocity /= _separationList.Count;

			desiredSeparationVelocity = desiredSeparationVelocity.normalized * BoidParameters.MaxSeparationSpeed; // Normalize
			SeparationSteering = desiredSeparationVelocity - rigidbody.velocity;
		}

		// Alignment
		AlignmentSteering = Vector3.zero;
		Vector3 desiredAlignmentVelocity = Vector3.zero;
		if(_alignmentList.Count > 0)
		{
			Vector3 avgVelocity = Vector3.zero;
			for (int i = 0; i < _alignmentList.Count; ++i)
			{
				avgVelocity += rigidbody.velocity;
			}
			avgVelocity = avgVelocity / _alignmentList.Count;
			desiredAlignmentVelocity = avgVelocity;
			desiredAlignmentVelocity = desiredAlignmentVelocity.normalized * BoidParameters.MaxAlginmentSpeed; // Normalize

			AlignmentSteering = desiredAlignmentVelocity - rigidbody.velocity;
		}

		// Attraction
		AttractionSteering = Vector3.zero;
		Vector3 desiredAttractiontVelocity = Vector3.zero;
		if(_attractionList.Count > 0)
		{
			Vector3 avgPosition = Vector3.zero;
			for (int i = 0; i < _attractionList.Count; ++i)
			{
				avgPosition += _attractionList[i].transform.position;
			}
			avgPosition = avgPosition / _attractionList.Count;
			desiredAttractiontVelocity = avgPosition - transform.position;
			desiredAttractiontVelocity = desiredAttractiontVelocity.normalized * BoidParameters.MaxAttractionSpeed; // Normalize
			
			AttractionSteering = desiredAttractiontVelocity - rigidbody.velocity;
		}
	}

	Vector3 CalculateObstacleSteering()
	{
		// Find the closest Obstacle
		Ray ray = new Ray(transform.position, rigidbody.velocity.normalized);

		Vector3 steeringForce = Vector3.zero;

		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, BoidParameters.MaxObstacleAheadDistance, 1 << LayerMask.NameToLayer("obstacle")))
	    {
			float obstacleRadius = hit.collider.bounds.size.x;
			obstacleRadius = obstacleRadius < hit.collider.bounds.size.y ? hit.collider.bounds.size.y : obstacleRadius;
//			obstacleRadius *= 1.25f;

			Vector3 desiredVelocity = (hit.point - hit.collider.gameObject.transform.position).normalized * obstacleRadius;
			desiredVelocity.y = 0f;

			steeringForce = desiredVelocity - rigidbody.velocity;
		}

		return steeringForce;
	}

	Vector3 SeekSteering()
	{
		Vector3 desiredVelocity = _targetPosition - transform.position;
		desiredVelocity = desiredVelocity.normalized * BoidParameters.MaxSeekSpeed;
		Vector3 seekForce = desiredVelocity - rigidbody.velocity;

		float arriveFactor = 1f;
		float distanceToTarget = (_targetPosition - transform.position).magnitude;

		if(distanceToTarget < MovementParameters.ArrivingRadius)
		{
			arriveFactor = distanceToTarget / MovementParameters.ArrivingRadius;
		}

		seekForce = seekForce * arriveFactor;

		return seekForce;
	}
	#endregion states
}
