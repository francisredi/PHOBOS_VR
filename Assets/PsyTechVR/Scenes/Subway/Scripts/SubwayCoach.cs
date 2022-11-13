using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SubwayCoach: MonoBehaviour {
	
	public SubwayWaypoint currentWaypoint;
	public SubwayAIControl aiControl;
	public string Side; // Left or Right
	public float speed = 0.03f;
	public float minSpeed = 0.001f;
	public float stopDistance = 20.0f;
	public float accelerationRate = 30.0f;
	public float stationStopTime = 10.0f;
	public bool  showDebug = false;
	[HideInInspector]
	public List<NavigateSubway> passengers;
	
	private List<GameObject> _wagons;
	private string currentStation;
	private float acceleration;
	private Vector3 targetPos;
	private float maxSpeed;	
	private bool mFromWaitState;
	EState currentState = EState.Moving;
	public AudioClip stationSound;
	public AudioClip movingSound;
	float stopTime;
	
	enum EState
	{
		Wait,
		OpenDoor,
		CloseDoor,
		Moving
	};
	
	void Awake()
	{
		passengers = new List<NavigateSubway>();
		_wagons = new List<GameObject>();
		foreach(Transform child in transform.parent)
			_wagons.Add(child.gameObject);
	}
	
	
	// Use this for initialization
	void Start () {
		processNextWaypoint();
		maxSpeed = speed;
		acceleration = maxSpeed / accelerationRate;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(currentWaypoint == null) return;
		
		switch( currentState)
		{
		case EState.CloseDoor:
			processCloseDoor();
			break;
		case EState.OpenDoor:
			processOpenDoor();
			break;
		case EState.Wait:
			processWaitAtStation();
			break;
		case EState.Moving:
			processMovingState();
			break;
		};
		
	}

	public string GetCurrentStation(){
		return currentStation;
	}
	
	void processOpenDoor()
	{
		Animation openDoor = gameObject.transform.parent.gameObject.GetComponent<Animation>();
		openDoor["OpenDoorRight"].speed = 1;
		openDoor.Play();
		stopTime = Time.time;
		currentState = EState.Wait;
		
		BorderConstraint[] colliders = gameObject.transform.parent.GetComponentsInChildren<BorderConstraint>();
		foreach(BorderConstraint borderScript in colliders)
		{			
			if(borderScript.SmoothTransition) // disabling the smooth ones that belong to doors
			{
				borderScript.GetComponent<Collider>().enabled = false;
				borderScript.enabled = false;
			}
		}
		
		foreach (NavigateSubway person in passengers) {
			person.ResetSeconds(); // for sitting animations timing
			if (person.GetDestStation () == currentStation)
					person.LeaveSubwayCar ();
			else {
					// when StayInWagon is called, collisions are disabled because the NavAgent is disabled.
					// this allows the agents that are entering the wagon to collide against the ones that are staying on it
					person.EnableCollisions ();
			}
		}
		
		// remove the passengers that went out
		passengers.RemoveAll( delegate(NavigateSubway person) {
			return person.GetDestStation() == currentStation;
		});
				
		
		aiControl.ReachedStation( this, currentStation );
	}
	
	void processCloseDoor()
	{
		if(mFromWaitState)
		{
			Animation openDoor = gameObject.transform.parent.gameObject.GetComponent<Animation>();
			openDoor["OpenDoorRight"].speed = -1;
			openDoor["OpenDoorRight"].time = openDoor["OpenDoorRight"].length;
			openDoor.Play();
			mFromWaitState = false;
			stopTime = Time.time;
			
			BorderConstraint[] colliders = gameObject.transform.parent.GetComponentsInChildren<BorderConstraint>();
			foreach(BorderConstraint borderScript in colliders)
			{
				borderScript.enabled = true;
				borderScript.GetComponent<Collider>().enabled = true;
			}
		}
		if( Time.time - stopTime > 2 )
		{
			currentState = EState.Moving;			
			AudioSource sound = GetComponent<AudioSource>();
			sound.clip = movingSound;
			sound.loop = true;
			sound.time = 0.0f;
			sound.Play();
			foreach(NavigateSubway person in passengers)
				person.StayInWagon();
		
			aiControl.LeftStation( this, currentStation );
			currentStation = "";
		}		
	}
	
	void processWaitAtStation()
	{
		if( Time.time - stopTime > stationStopTime )
		{
			mFromWaitState = true;
			AudioSource sound = GetComponent<AudioSource>();
			sound.time = 22.0f;
			
			currentState = EState.CloseDoor;
		}
	}
	
	void processMovingState()
	{
		
		Vector3 distance = targetPos - transform.position;
		Vector3 forward = transform.forward;
		float angle = SignedAngle(forward, distance);
		if( angle != 0)
		{
			transform.Rotate(Vector3.up, angle/15);
		}
		float len = distance.magnitude;
		if( currentWaypoint.IsStation)
		{
			if(len < stopDistance)
			{
				speed = maxSpeed * ( len / stopDistance);
				if(speed < minSpeed)
				{
					speed = minSpeed;
				}
			}
		}
		else
		{
			if( speed < maxSpeed)
			{
				speed += acceleration;
				if(speed > maxSpeed )
				{
					speed = maxSpeed;
				}
			}
		}
		if( len < 1 * speed * Time.deltaTime)
		{
			if(currentWaypoint.IsStation)
			{
				currentState = EState.OpenDoor;
				AudioSource sound = GetComponent<AudioSource>();
				sound.clip = stationSound;
				sound.loop = false;
				sound.time = 10.0f;
				sound.Play();
				currentStation = currentWaypoint.Station;
			}
			currentWaypoint = currentWaypoint.connectedWaypoints[0];
			processNextWaypoint();
			return;
		}
		//Vector3 translation = transform.forward * speed * Time.deltaTime;
		Vector3 translation = distance.normalized * speed * Time.deltaTime;
		transform.Translate( translation  , Space.World );
		
		foreach(GameObject wagon in _wagons)
			if(wagon != gameObject) // to filter the coach itself
				wagon.GetComponent<CoachFollow>().CustomUpdate(); // We assume that the childs are in order.
	}
	
	void processNextWaypoint()
	{
		if(currentWaypoint == null) return;
		targetPos = currentWaypoint.transform.localToWorldMatrix.GetColumn(3);
		Vector3 forward = transform.forward;
		Vector3 dir = targetPos - transform.position;
		float angle = Vector3.Angle(forward, dir);
		if( showDebug)
		{
			Debug.Log( "Angle: " + angle);
		}
		//transform.LookAt( targetPos);
		
	}
	
	float SignedAngle(Vector3 a, Vector3 b)
	{
		var angle = Vector3.Angle(a, b); // calculate angle
		// assume the sign of the cross product's Y component:
		return angle * Mathf.Sign(Vector3.Cross(a, b).y);
	}
	
	public GameObject[] wagons { get { return _wagons.ToArray(); } }
}
