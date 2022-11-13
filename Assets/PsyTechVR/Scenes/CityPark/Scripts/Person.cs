using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Person : MonoBehaviour {

	private List<GameObject> mAllWaypoints;
	private GameObject[] mWarpWaypoints;
	private GameObject[] mCars;
	private UnityEngine.AI.NavMeshAgent mNavMeshAgent;
	private GameObject mDestination;
	private EState mState;
	private Semaphore mSemRef;
	private GameObject mCrossRef;
	private float mSpeed;
	private float mStoppingDist;
	private GameObject mSubwayRef; // a reference used to get in and out of the subway stations
	
	private const float DIRECTION_EPSILON = 0.5f;
	private const float SIDE_EPSILON = 0.1f;
	private const float AWAY_DIST_RED = 15f;
	private const float AWAY_DIST_GREEN = 6f;
	private const float CAR_PANIC_DIST = 0.8f;
	
	public enum EState
	{
		Walking,
		Stopping,
		Waiting,
		SubwayInOut,
		Panic
	};
	
	private const string WALK_ANIM = "Walk";
	private const string IDLE_ANIM = "Idle";
	private const float ANIM_BLEND_TIME = 0.3f;
	
	
	void OnEnable()
	{		
		const float syncSpeed = 0.7f;
		GetComponent<Animation>()[WALK_ANIM].speed = mNavMeshAgent.speed / syncSpeed;
		
		mSpeed = mNavMeshAgent.speed;
		mStoppingDist = mNavMeshAgent.stoppingDistance;
		
		mAllWaypoints = new List<GameObject>( GameObject.FindGameObjectsWithTag("WaypointsVereda") ); // TODO: Create a static class to hold tag strings
		mWarpWaypoints = GameObject.FindGameObjectsWithTag("WaypointsVeredaSpawn");
		mAllWaypoints.AddRange( mWarpWaypoints ); 
		
		mCars = GameObject.FindGameObjectsWithTag("cars");
		
		GoToRndSpot ();
	}
	
	IEnumerator DelayedGoToRndSpot()
	{
		yield return new WaitForSeconds(0.01f);	// WTF? why the wait for end of frame doesn't work?

		GoToRndSpot();
	}
	
	
	void GoToRndSpot ()
	{
		mNavMeshAgent.enabled = true;
		State = EState.Walking;	
		
		int index = Random.Range(0, mAllWaypoints.Count);
		mDestination = mAllWaypoints[index];
		mNavMeshAgent.SetDestination(mDestination.transform.position);
	}
	
	void Awake () {
	
		mNavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		
		State = EState.Walking;
		mSubwayRef = null;
	}
	
	void SetStartPoint(GameObject sp)
	{
		ObjectRef oref = sp.GetComponent<ObjectRef>();
		if( oref != null )
		{
			mSubwayRef = oref.Ref;
			State = EState.SubwayInOut;
			mNavMeshAgent.enabled = false;
		}
		
		mSpeed = mNavMeshAgent.speed;
	}
	
	void Update () {
		
		if( State == EState.SubwayInOut ) // this is not handled by the nav mesh agent
		{
			Vector3 dir = (mSubwayRef.transform.position - transform.position).normalized;
			transform.Translate( dir * Time.deltaTime * mSpeed, Space.World );
			dir.y = 0;
			transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime*2f);
			
			const float epsilon = 0.07f;
			if(Utils.GetDist(gameObject, mSubwayRef) < epsilon)
			{				
				if(mSubwayRef.tag != "Untagged")
				{
					if( mSubwayRef.tag == "WaypointsBajadaSubte")
						// the person got into the subway station, so just warp it to another location.
						transform.position = mWarpWaypoints[ Random.Range(0, mWarpWaypoints.Length) ].transform.position;
					
					StartCoroutine( DelayedGoToRndSpot() );				
				}				
				
				mSubwayRef = mSubwayRef.GetComponent<ObjectRef>().Ref;
			}
			return;
		}
		
		// temporal: pick a new destination when current is done.
		if( State != EState.Waiting &&
			!mNavMeshAgent.pathPending &&
			mNavMeshAgent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathInvalid &&
			mNavMeshAgent.remainingDistance != Mathf.Infinity && 
			mNavMeshAgent.remainingDistance <= mNavMeshAgent.stoppingDistance )
		{
			if( State == EState.Walking)
			{
				ObjectRef destRef = mDestination.GetComponent<ObjectRef>();
				if( destRef != null )
				{
					mSubwayRef = destRef.Ref;
					State = EState.SubwayInOut;
					mNavMeshAgent.enabled = false;
					return; // we reached a waypoint that will lead us into a subway station.
				}				
				
				if( mDestination.tag == "WaypointsVeredaSpawn" )
				{
					// we entered in a point that the user can't see this person, so lets teleport it to another
					// waypoint (i.e. another building)
					
					transform.position = mWarpWaypoints[ Random.Range(0, mWarpWaypoints.Length) ].transform.position;
				}
				
				GoToRndSpot();
			}
			else if( State == EState.Stopping)
			{
				State = EState.Waiting;
				StartCoroutine(LookAtDestination());
			}
		}
		
		

		// check semaphore to start walking again	
		if( mSemRef != null && State == EState.Waiting )
		{
			// TODO: MOVE INTO A METHOD!
			Semaphore.EState redState = Semaphore.EState.Red;
			if(Mathf.Abs( Vector3.Dot(mSemRef.transform.up, mCrossRef.transform.forward) ) < SIDE_EPSILON )
				redState = Semaphore.EState.Green;
			
			if( mSemRef.GetState() == redState && CarsAreFarAway(AWAY_DIST_GREEN) )
			{
				StartCoroutine(StartWalking());
				State = EState.Walking;
			}
		}
		
		// we're still waiting so check if there is no cars near and start walking again
		if( State == EState.Waiting && CarsAreFarAway(AWAY_DIST_RED))
		{
			StartCoroutine(StartWalking());
			State = EState.Walking;
		}
		
		if( State == EState.Walking && !CarsAreFarAwayAndNotInFront(CAR_PANIC_DIST) ) //we're almost being hit by a car so stop right here!
		{
			mNavMeshAgent.Stop();
			State = EState.Panic;
		}
		else if( State == EState.Panic && CarsAreFarAwayAndNotInFront(CAR_PANIC_DIST) )
		{
			mNavMeshAgent.Resume();
			State = EState.Walking;
		}
	}

	private bool CarsAreFarAway (float dist) {

		foreach(GameObject car in mCars)
			if( !car.GetComponent<CarAi>().mIsStop && Utils.GetDist(car, gameObject) < dist )
				return false;
		
		return true;
	}
	
	private bool CarsAreFarAwayAndNotInFront (float dist) {
		
		foreach(GameObject car in mCars)
		{			
			bool isInFront = transform.InverseTransformPoint(car.transform.position).z > 0;
			if( !car.GetComponent<CarAi>().mIsStop && isInFront && IsInPanicDist(car, dist) )
				return false;
		}
		
		return true;
	}
	
	private bool IsInPanicDist (GameObject obj, float dist) {
		
		Vector3[] borders = new Vector3[5];
		
		MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
		Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
		foreach(MeshFilter filter in meshFilters)
		{
			Matrix4x4 mat = Matrix4x4.TRS(filter.transform.localPosition, filter.transform.localRotation, filter.transform.localScale);
			bounds.Encapsulate( mat * filter.mesh.bounds.min );
			bounds.Encapsulate( mat * filter.mesh.bounds.max );
		}
		
 		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		Vector3 localToObjPos = obj.transform.InverseTransformPoint(gameObject.transform.position);
		borders[0].Set( min.x, localToObjPos.y, min.z);
		borders[1].Set( min.x, localToObjPos.y, max.z);
		borders[2].Set( max.x, localToObjPos.y, min.z);
		borders[3].Set( max.x, localToObjPos.y, max.z);
		borders[4].Set( 0f, 0f, 0f);
		
	
		foreach(Vector3 point in borders)
			if( (localToObjPos - point).magnitude < dist )
				return true;
		
		return false;
	}
	
	// catches the state changes and translate those into animation changes
	public EState State {
		get { return mState; }
		set { 
			
			if( value == mState )
				return;
			
			switch(value)
			{
			case EState.Panic:
			case EState.Waiting:
				GetComponent<Animation>().CrossFade(IDLE_ANIM, ANIM_BLEND_TIME);				
				break;
			case EState.SubwayInOut:
			case EState.Stopping:
			case EState.Walking:
				GetComponent<Animation>().CrossFade(WALK_ANIM, ANIM_BLEND_TIME);
				break;
			}
			
			mState = value;
		}
	}
	
	private Vector3 COLLISION_OFFSET = new Vector3(0f, 0.5f, 0f);
	void OnTriggerStay(Collider other) 
	{	
		if(other.gameObject.layer == LayerMask.NameToLayer("NPC_Cars_Collision") ) // we're colliding with a car
		{	
			Vector3 colPos = other.ClosestPointOnBounds(transform.position + COLLISION_OFFSET);
			Vector3 colVec = transform.position + COLLISION_OFFSET - colPos;
			Vector3 slideVec = colVec;
			if( colVec == Vector3.zero )// in case that we're inside the collider so ClosestPointOnBounds returned the same input point
			{
				slideVec = transform.position - other.transform.position; slideVec.y = 0;
				slideVec.Normalize();
			}
			transform.Translate( slideVec * 0.1f, Space.World );
			
			return;
		}
		
		if(State == EState.Waiting || CarsAreFarAway(AWAY_DIST_RED))
			return; // do not check if we're not moving or there are no near cars
		
		ObjectRef objRef  = other.gameObject.GetComponent<ObjectRef>();
		if(objRef == null)
			return; // just work with gameobjs that have a ObjectRef script.
		
		mSemRef = objRef.Ref.GetComponent<Semaphore>();
		mCrossRef = other.gameObject;
		
		Semaphore.EState redState = Semaphore.EState.Red;
		if(Mathf.Abs( Vector3.Dot(mSemRef.transform.up, mCrossRef.transform.forward) ) < SIDE_EPSILON )
			redState = Semaphore.EState.Green; // if the semaphore is from the perpendicular street, we cross when it is green, not red.
		
		float dotRes = Vector3.Dot(transform.forward, mCrossRef.transform.forward);
		if(mSemRef.GetState() != redState  &&
			dotRes > 0 && 1f - dotRes < DIRECTION_EPSILON ) // mCrossRef points to the cross street direction, so this checks if we're crossing
		{
			//stop walking
			State = EState.Stopping;
			StartCoroutine(StopWalking( mCrossRef.transform ));
		}
	}
	
	IEnumerator StopWalking(Transform waitPlace)
	{
		// lets wait outside the street.
		mNavMeshAgent.destination = waitPlace.position + waitPlace.right * Random.value *2 + waitPlace.forward * Random.Range(0f,1f) *-1;
		mNavMeshAgent.stoppingDistance = 0;
		
		yield return new WaitForSeconds(0); // wtf?
	}
	
	IEnumerator StartWalking()
	{
		mNavMeshAgent.destination = mDestination.transform.position;
		mNavMeshAgent.stoppingDistance = mStoppingDist;
		
		yield return new WaitForSeconds(0);
	}
	
	IEnumerator LookAtDestination()
	{
		Quaternion finalRot = Quaternion.LookRotation( (mDestination.transform.position - transform.position).normalized, Vector3.up );
		Quaternion currRot = transform.rotation;
		
		const int steps = 30;
		float delta = 1f / steps;
		float a = 0f;
		const float delay = 0.05f;
		for(int i = 0; i < steps; i++)
		{
			transform.rotation = Quaternion.Slerp( currRot, finalRot, a );
			a += delta;
			yield return new WaitForSeconds(delay);
		}
	}
}
