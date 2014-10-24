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
	public float MaxObstacleRadius = 3f;

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

	public bool EnableDebug = false;

	State _state = State.IDLE;

	Zombie _zombie;
	Vector3 _targetPosition;

	[SerializeField]
	public MovementParameters MovementParameters;

	[SerializeField]
	public BoidParameters BoidParameters;

	List<Zombie> _separationList = new List<Zombie>();
	List<Zombie> _alignmentList = new List<Zombie>();
	List<Zombie> _attractionList = new List<Zombie>();

	public Vector3 SeparationSteering;
	public Vector3 AlignmentSteering;
	public Vector3 AttractionSteering;
	public Vector3 ObstacleForce;
	public Vector3 SeekForce;
	public Vector3 SteeringVelocity;
	public Vector3 BoidForces;

	void Awake()
	{
		_zombie = GetComponent<Zombie>();
	}

	public void Seek(Vector3 targetPos)
	{
		_targetPosition = targetPos;
		_state = State.SEEK;
	}

	public void StopMovement()
	{
		_state = State.IDLE;
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
		SeekForce = SeekSteering();
		SeekForce = Truncate(SeekForce, BoidParameters.MaxSeekSpeed) * BoidParameters.SeekFactor;

		CalculateBoidForces();
		BoidForces = Vector3.zero;
		BoidForces = Truncate(SeparationSteering, BoidParameters.MaxSeparationSpeed) * BoidParameters.SeparationFactor + Truncate(AlignmentSteering, BoidParameters.MaxAlginmentSpeed) * BoidParameters.AlignmentFactor + Truncate (AttractionSteering, BoidParameters.MaxAttractionSpeed) * BoidParameters.AttractionFactor;

		SteeringVelocity = Vector3.zero;
		ObstacleForce = Truncate(CalculateObstacleSteering(), BoidParameters.MaxObstacleSpeed) * BoidParameters.ObstacleFactor;

		if(ObstacleForce.magnitude > 1e-1f)
		{
			SeekForce *= 0.25f;
		}
		SteeringVelocity += (SeekForce + BoidForces + ObstacleForce) * Time.deltaTime;

		Vector3 newVelocity = rigidbody.velocity + SteeringVelocity;
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
		Vector3 velocityNorm = rigidbody.velocity.normalized;
		Vector3 origin =transform.position -  velocityNorm * 0.5f;
		origin.y += 1f;
		Ray ray = new Ray(origin, rigidbody.velocity.normalized);

		Vector3 steeringForce = Vector3.zero;

		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, BoidParameters.MaxObstacleAheadDistance, 1 << LayerMask.NameToLayer("obstacle")))
	    {
			float obstacleRadius = hit.collider.bounds.size.x;
			obstacleRadius = obstacleRadius < hit.collider.bounds.size.y ? hit.collider.bounds.size.y : obstacleRadius;
			obstacleRadius *= 0.60f;

			//Vector3 desiredVelocity = (hit.point - hit.collider.gameObject.transform.position).normalized * obstacleRadius;
			Vector3 desiredVelocity = CalculatePerpendicularVectorToObstacle(hit.collider.gameObject.transform.position, hit.point) * obstacleRadius;
			desiredVelocity.y = 0f;

			steeringForce = desiredVelocity - rigidbody.velocity;
		}
		return steeringForce;
	}

	Vector3 CalculatePerpendicularVectorToObstacle(Vector3 obstaclePosition, Vector3 hitPoint)
	{
		Vector3 dirObstacleHitPoint = (hitPoint - obstaclePosition).normalized;
		Vector3 dirToObstacle = (obstaclePosition - transform.position).normalized;
		Vector3 right = new Vector3(dirToObstacle.z, 0f, -dirToObstacle.x);

		if(dirObstacleHitPoint.sqrMagnitude == 0f)
		{
			dirObstacleHitPoint = right;
        }

		Vector3 perpendicularToObstacle = Vector3.Dot(right, dirObstacleHitPoint) * right;

		return perpendicularToObstacle.normalized;
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

	void OnDrawGizmos()
	{
		if(!EnableDebug)
		{
			return;
		}

		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + ObstacleForce);

		Gizmos.color = Color.gray;
		Gizmos.DrawLine(transform.position, transform.position + SeekForce);

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(transform.position, transform.position + BoidForces);

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + SteeringVelocity);
	}
	#endregion states
}
