using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;

public class CarAi : MonoBehaviour {
	private enum eCollisionCarOptions
	{ 
		Slow = 1,
		Stop = 2,
		Ignore = 3
	}
	
	public StreetWaypoint target;
	public float		  stopDistance = 1.0f;
	public List<GameObject> Wheels;
	protected int mPathTypeMask = 0;
	public bool followBusPath = true;
	public bool followCarPath = true;
	public bool followBusStop = false;
	public float wheelSpinSpeed = 5.0f;
	private bool mInCollision = false;
	
	
	private float mCollisionCosCheck = 0.4f;
	
	protected UnityEngine.AI.NavMeshAgent agent;
	protected Vector3 wpPos;
	protected List<Collider> blockingVehicles = new List<Collider>();
	protected List<Collider> blockingPeople = new List<Collider>();
	public bool mIsStop { get;set;}
	private float mMaxSpeed;
	private bool mWaitAtSemaphore = false;
	private float mSlowDistance;
	private bool disable = false;
	private bool agentStateSave;

	private UnityEngine.AI.NavMeshHit closestHit;

	public void Awake()
	{
	}
	// Use this for initialization
	virtual public void Start () {
		if( followBusPath) mPathTypeMask |= (int)StreetWaypoint.eWayPointType.Bus;
		if( followCarPath) mPathTypeMask |= (int)StreetWaypoint.eWayPointType.Cars;
		if( followBusStop) mPathTypeMask |= (int)StreetWaypoint.eWayPointType.BusStop;
		mIsStop = false;

		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		int mask = 1 << UnityEngine.AI.NavMesh.GetAreaFromName("Road");
		//mask |= 1 << NavMesh.GetNavMeshLayerFromName("CityWalk");
		agent.areaMask = mask;

		//if( NavMesh.SamplePosition(transform.position, out closestHit, 1.0f, 1 ) ){ // make sure on navigation mesh!
		//	transform.position = closestHit.position;
		//}

		//int walkMask = (1 << layer1);
		//print (walkMask); // gets hardcoded value 48 from using UnityEditor
		//agent.walkableMask = 512; // Street layer only
		mMaxSpeed = agent.speed;
		mSlowDistance = stopDistance + 3.0f;
		goToTarget();
	}
	
	// Update is called once per frame
	virtual public void Update () {
		if(disable) return;
		updateWheels();
		if(mInCollision) 
		{
			ValidateCollisions();
			return;
		}
		
		//string dbgstr = "speed: " + agent.speed + " acc: " + agent.acceleration +" velocity; " + agent.velocity + " desired vel: " + agent.desiredVelocity + "vel lenght: " + agent.velocity.magnitude;
		//Debug.Log( dbgstr );
		Vector3 distance = wpPos - transform.position;
		
		if( !target.isSemaphoreOk() )
		{
			if(distance.magnitude < stopDistance)
			{
				mWaitAtSemaphore = true;
				pauseMovement();
				return;
			}
			if( distance.magnitude  < mSlowDistance)
			{
				agent.speed = mMaxSpeed * (distance.magnitude / mSlowDistance);
			}
		}
		else
		{
			if(mWaitAtSemaphore)
			{
				mWaitAtSemaphore = false;
				resumeMovement();
				agent.speed = mMaxSpeed;
			}
		}
		
		if( agent.remainingDistance != Mathf.Infinity && 
			agent.remainingDistance <= agent.stoppingDistance + 0.2f  &&
			!mWaitAtSemaphore) // TODO: use the same as Person.cs */
		//if( distance.magnitude < 0.6 )
		{
			if( target.isSemaphoreOk() && distance.magnitude < 0.8f)
			{
				pickNewTarget();
				goToTarget();
			}
		}
	}
	
	public void disabled( bool state)
	{
		gameObject.SetActive(!state);
//		disable = state;
//		if(state)
//		{
//			agentStateSave = agent.enabled;
//			Debug.Log("stop agent");
//			agent.Stop();
//		}
//		else
//		{
//			if( agentStateSave)
//			{
//				Debug.Log("resume agent");
//				agent.Resume();
//			}
//		}
	}
	protected void updateWheels()
	{
		if( mIsStop ) return;
		float speed = agent.velocity.magnitude * Time.deltaTime * wheelSpinSpeed;
		
		foreach( GameObject wheel in Wheels)
		{
			wheel.transform.RotateAroundLocal( new Vector3(1.0f,0.0f,0.0f), speed);
			////wheel.transform.Rotate( new Vector3(1.0f,0.0f,0.0f), speed);
		}
	}
	
	virtual protected void pickNewTarget()
	{
		if(target != null)
		{
			target.removeCar(this);
		}
		target = target.getNextWaipointWithMask( mPathTypeMask );
		if(target != null)
		{
			target.addCar(this);
		}
	}
	protected void goToTarget()
	{
		if(target == null) return;
		wpPos = target.transform.position;
		agent.destination = wpPos;	
	}
	
	protected void pauseMovement()
	{
		agent.Stop();
		mIsStop = true;
	}
	
	protected void resumeMovement()
	{
		agent.Resume();
		mIsStop = false;
	}
	
	public Vector3 getVelocity()
	{
		return agent.velocity;
	}
	
	float distanceToTarget()
	{
		return (wpPos - transform.position).magnitude;
	}
	
	void OnTriggerEnter(Collider other) 
	{
//		CarAi car = other.GetComponent<CarAi>();
//		
//        Vector3 forward = transform.TransformDirection(Vector3.forward);
//		Vector3 direction = other.transform.position - transform.position;
//		direction = direction.normalized;
//		if(Vector3.Dot(forward, direction) > mCollisionCosCheck)
//		{
//			mInCollision = true;
//			if( car == null || car.mIsStop )
//			{
//				pauseMovement();
//			}
//			else
//			{
//				agent.speed =  car.getVelocity().magnitude * 0.1f;
//			}
//			if( !blockingVehicles.Contains(other))
//			{
//				blockingVehicles.Add(other);
//			}
//		}
    }
	
	void OnTriggerStay(Collider other)
	{
		if(other.GetType() == typeof(CapsuleCollider)) { // it is a person or other moving object
			mInCollision = true;
			if( !blockingPeople.Contains(other))
			{
				blockingPeople.Add(other);
			}
			pauseMovement();
			return;
		}



//		CarAi car = other.GetComponent<CarAi>();
//		Vector3 forward = transform.TransformDirection(Vector3.forward);
//		Vector3 direction = other.transform.position - transform.position;
//		direction = direction.normalized;
//		if(Vector3.Dot(forward, direction) > mCollisionCosCheck)
//		{
//			mInCollision = true;
//			if( car == null || car.mIsStop )
//			{
//				pauseMovement();
//			}
//			else
//			{
//				agent.speed =  car.getVelocity().magnitude * 0.7f;
//			}
//			if( !blockingVehicles.Contains(other))
//			{
//				blockingVehicles.Add(other);
//			}
//		}
		
		eCollisionCarOptions choice = SelectCarBehaviour(other);
		if( choice == eCollisionCarOptions.Ignore)
		{
			return;
		}
		mInCollision = true;
		if( !blockingVehicles.Contains(other))
		{
			blockingVehicles.Add(other);
		}
		if( choice == eCollisionCarOptions.Slow )
		{
			CarAi car = other.GetComponent<CarAi>();
			agent.speed =  car.getVelocity().magnitude * 0.7f;
		}
		else
		{
			pauseMovement();
		}
		
	}
	
	void OnTriggerExit(Collider other)
	{
		if( !blockingPeople.Contains(other) && !blockingVehicles.Contains(other)) return; 
		Vector3 forward = transform.TransformDirection(Vector3.forward);
		Vector3 direction = other.transform.position - transform.position;
		direction = direction.normalized;
		if(Vector3.Dot(forward, direction) > mCollisionCosCheck)
		{
			resumeMovement();
			agent.speed = mMaxSpeed;
			mInCollision = false;
			blockingVehicles.Remove(other); // may or may not have it
			blockingPeople.Remove(other); // may or may not have it
		}
	}
	
	void ValidateCollisions()
	{
		for(int i = 0; i < blockingVehicles.Count; i++)
		{
			Collider other = blockingVehicles[i];
			Vector3 forward = transform.TransformDirection(Vector3.forward);
			Vector3 direction = other.transform.position - transform.position;
			float distance = direction.magnitude;
			direction = direction.normalized;
			if(Vector3.Dot(forward, direction) > mCollisionCosCheck && distance < 5)
			{
				return;
			}
		}
		blockingVehicles.Clear();
		resumeMovement();
		agent.speed = mMaxSpeed;
		mInCollision = false;
	}
	
	eCollisionCarOptions SelectCarBehaviour(Collider other)
	{
		CarAi car = other.GetComponent<CarAi>();
		if(car == null) // not a car trigger region box
		{
			/*if(other.GetComponent<Person>() != null){ // is a person
				return eCollisionCarOptions.Ignore;
			}
			else{*/ // is not a person
				//bool isInFront = other.transform.InverseTransformPoint(transform.position).z > 0;
				if(Vector3.Angle(other.transform.position - transform.position,transform.forward) > 45.0f){
				//if(!isInFront){
					return eCollisionCarOptions.Ignore; // ignore cars who are not in front side of vehicle (reduces traffic)
				}
			//}
			return eCollisionCarOptions.Stop;
		}
		Vector3 direction = transform.position - other.transform.position;
		Vector3 theirForward = car.transform.TransformDirection(Vector3.forward);
		Vector3 myForward = transform.TransformDirection(Vector3.forward);
		
		if( Vector3.Dot(myForward,theirForward) < 0)  return eCollisionCarOptions.Ignore;
		direction = direction.normalized;
		if(Vector3.Dot(theirForward, direction) > 0)
		{
			Vector3 theirDir = (other.transform.position - transform.position).normalized;
			if( car.target == target &&
				Vector3.Dot(myForward, theirDir) > 0)
			{
				if(car.distanceToTarget() < distanceToTarget())
				{
					return eCollisionCarOptions.Stop;
				}
			}
			return eCollisionCarOptions.Ignore; // this car is in front of them, ignore them, the should stop
		}
		if(Vector3.Dot(myForward, theirForward) > 0.8 )
		{
			return eCollisionCarOptions.Slow; // this car is behind them and have similar direction, slow down
		}
		
		return eCollisionCarOptions.Stop;//this car is behind the other with different direction, lets stop and let them go 
		
	}
}
