using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions; // for Regex

public class NavigateSubway : MonoBehaviour {

	// Animator
	public Animator anim;							// a reference to the animator on the character
	private AnimatorStateInfo currentLayer1State;	// a reference to the current state of the animator, used for base layer
	private AnimatorStateInfo currentLayer2State;	// a reference to the current state of the animator, used for 2nd layer
	private float animSpeed = 1.0f;					// a public setting for overall animator animation speed

	static int fidgetFeetTrigger = Animator.StringToHash("FidgetFeetTrigger");
	static int sittingStillTrigger = Animator.StringToHash("SittingStillTrigger");
	static int handsOnThighsTrigger = Animator.StringToHash("HandsOnThighsTrigger");
	static int impatientWaitTrigger = Animator.StringToHash("ImpatientWaitTrigger");
	static int crossLegTrigger = Animator.StringToHash("CrossLegTrigger");
	private bool UsingHMD = false;
	static float lookSmoother = 1.0f;                // a smoothing setting for camera motion
	
	public enum EState
	{
		Walking,
		Waiting,
		Entering,
		Travelling,
		Exiting
	}
	
	public enum EStationSide
	{
		Left,
		Right
	}
	
	public float IdleTime = 30f;
	public float MaxDistanceToSubway = 3f;
	public string[] AvailableStations;

	// Private variables
	private float pedestrianSpeed = 1.0f;	// default walk speed when in group
	private float targetSpeed = 0.0f;		// speed to adjust to
	//private float stoppingDistance = 0.1f;	// how close to target can begin to stop
	private float personalWalkSpeed = 0.0f; // when walking alone
	
	private GameObject mTrainControl;
	private UnityEngine.AI.NavMeshAgent mNavMeshAgent;
	private GameObject mDestination;
	private string mDestStation;
	private EState mState;
	private List<GameObject> mExits;
	private List<GameObject> mPlatformWaitSpots;
	private bool mWillGetIntoSubway;
	private float mStopDistance;
	private EStationSide mSide;
	private string mStation;
	private Transform assignedSpot; // seat or standing position in subway car
	//private HeadLookController lookController;
	
	//private const string WALK_ANIM = "Walk";
	//private const string IDLE_ANIM = "Idle";
	private const float ANIM_BLEND_TIME = 0.3f;

	// Private Cache
	private Characteristics characteristics;
	
	private void AdjustMovementSpeed(){
		pedestrianSpeed = targetSpeed / 0.45f;
		if(anim == null) anim = GetComponent<Animator>();
		anim.SetFloat("Speed", pedestrianSpeed * 1.0f * 0.45f); // added 0.45f
	}

	private void DeterminePersonalWalkSpeed(){
		if(characteristics.age < 10 && !characteristics.obese) personalWalkSpeed = 1.6f; //Random.Range(1.3f, 1.6f); // fast - kids
		else if(characteristics.age < 45 && !characteristics.obese) personalWalkSpeed = 1.3f; //Random.Range(1.0f, 1.3f); // normal
		else personalWalkSpeed = 1.0f; //Random.Range(0.7f, 1.0f); // slow - old people, obese people
		personalWalkSpeed = (Mathf.Round (personalWalkSpeed * 10.0f) / 10.0f) / 0.45f;
	}
	
	void OnEnable()
	{
		mTrainControl = GameObject.Find("SubwayAIControl"); // not using a reference because this is a prefab.
		mTrainControl.GetComponent<SubwayAIControl>().Register(this);
		
		mNavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

		// 3/25/2015
		//Configuration.CameraMode mode = GameObject.Find("PHOBOSConfig").GetComponent<Configuration>().getViewDevice();
		//if (mode > 0) UsingHMD = true;

		//mStopDistance = mNavMeshAgent.stoppingDistance;
		
		// normalizing animation speed with translation speed
		//const float syncSpeed = 0.7f;
		//animation[WALK_ANIM].speed = mNavMeshAgent.speed / syncSpeed;

		transform.localScale = new Vector3(0.45f,0.45f,0.45f);
		characteristics = GetComponent<Characteristics>();
		int mask = 1 << UnityEngine.AI.NavMesh.GetAreaFromName("PlataformaSubte");
		mNavMeshAgent.areaMask = mask;
		anim = GetComponent<Animator>();
		anim.applyRootMotion = false;
		anim.speed = animSpeed;								// set the speed of our animator
		//anim.SetLookAtWeight(lookWeight);					// set the Look At Weight - amount to use look at IK vs using the head's animation
		
		// reference layer states for later (if needed)
		currentLayer1State = anim.GetCurrentAnimatorStateInfo(0);	// set to the current state of the base layer of animation
		if(anim.layerCount == 2){
			currentLayer2State = anim.GetCurrentAnimatorStateInfo(1);	// set to the current state of the second layer of animation
			anim.SetLayerWeight(1, 1); // equal weight between layers
			anim.stabilizeFeet = true;
		}
		
		DeterminePersonalWalkSpeed();

		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll; // keep this!!!!

		lookTarget = GameObject.FindWithTag("Player").transform; // initial look at me

		/*lookController = transform.gameObject.AddComponent<HeadLookController>();
		lookController.segments = new BendingSegment[2];
		BendingSegment bs1 = new BendingSegment();
		bs1.firstTransform = anim.GetBoneTransform (HumanBodyBones.Spine);
		bs1.lastTransform = anim.GetBoneTransform (HumanBodyBones.Chest);
		bs1.thresholdAngleDifference = 30.0f;
		bs1.bendingMultiplier = 0.6f;
		bs1.maxAngleDifference = 90.0f;
		bs1.maxBendingAngle = 40.0f;
		bs1.responsiveness = 2.5f;
		lookController.segments[0] = bs1;
		BendingSegment bs2 = new BendingSegment();
		bs2.firstTransform = anim.GetBoneTransform (HumanBodyBones.Chest);
		bs2.lastTransform = anim.GetBoneTransform (HumanBodyBones.Head);
		bs2.thresholdAngleDifference = 0.0f;
		bs2.bendingMultiplier = 0.7f;
		bs2.maxAngleDifference = 30.0f;
		bs2.maxBendingAngle = 80.0f;
		bs2.responsiveness = 4.0f;
		lookController.segments[1] = bs2;
		lookController.nonAffectedJoints = new NonAffectedJoints[2];
		NonAffectedJoints naj1 = new NonAffectedJoints ();
		naj1.joint = anim.GetBoneTransform (HumanBodyBones.LeftUpperArm);
		naj1.effect = 0.3f;
		lookController.nonAffectedJoints [0] = naj1;
		NonAffectedJoints naj2 = new NonAffectedJoints ();
		naj2.joint = anim.GetBoneTransform (HumanBodyBones.RightUpperArm);
		naj2.effect = 0.3f;
		lookController.nonAffectedJoints [1] = naj2;
		lookController.headLookVector = new Vector3 (0,0,1);
		lookController.headUpVector = new Vector3 (0,1,0);*/
		//lookController.target = DefaultLookTarget();
		// don't specify target, effect, override animation, or root node
	}

	/*private Vector3 DefaultLookTarget(){
		//anim.GetBoneTransform (HumanBodyBones.Head).position.y
		return anim.GetBoneTransform (HumanBodyBones.Head).forward * 5.0f; // look straight forward by default
	}*/
	
	void Start () {
		mSide = mDestination.GetComponent<StationTag>().Side == "Right"? EStationSide.Right : EStationSide.Left;
		FillWaypoints( mSide, mDestination.GetComponent<StationTag>().Station );
		
		GoToRndSpotInPlatform ();

		StartCoroutine("Look");
	}

	private Vector3 destPos;
	
	void GoToRndSpotInPlatform ()
	{
		int index = Random.Range(0, mPlatformWaitSpots.Count);
		mDestination = mPlatformWaitSpots[index];
		mWillGetIntoSubway = !mDestination.name.Contains("idle");
		
		Vector3 platformDir = mDestination.transform.forward;
		Vector3 platformRight = mDestination.transform.right;
		if(mWillGetIntoSubway)
			platformRight = Vector3.zero;	
		
		const float stopDist = 0.5f;
		destPos = mDestination.transform.position + 
			platformDir * Random.Range(-1f,1f) * stopDist + 
				platformRight * Random.Range(-1f,1f) * stopDist;
		//destPos.y = 205.8f;
		mNavMeshAgent.SetDestination(destPos);
		
		
		State = EState.Walking;
	}
	
	void SetStartPoint(GameObject sp)
	{
		mDestination = sp;
		mStation = mDestination.GetComponent<StationTag>().Station;
	}
	
	void FillWaypoints(EStationSide currentSide, string currentStation)
	{
		mPlatformWaitSpots = new List<GameObject>( GameObject.FindGameObjectsWithTag( "WaypointsPlataforma" ) );
		mExits = new List<GameObject>( GameObject.FindGameObjectsWithTag( "InnerPlatformExit" ) );
		mExits.AddRange( GameObject.FindGameObjectsWithTag("OuterPlatformExit") );
		
		mPlatformWaitSpots.RemoveAll( delegate(GameObject obj) {
			return obj.GetComponent<StationTag>().Side != currentSide.ToString() ||
				obj.GetComponent<StationTag>().Station != currentStation; // omg, this comparison sucks.
		});	
		mExits.RemoveAll( delegate(GameObject obj) {
			return obj.GetComponent<StationTag>().Side != currentSide.ToString() ||
				obj.GetComponent<StationTag>().Station != currentStation; // omg, this comparison sucks.
		});	
	}

	private void WalkToTarget(){
		float distance = Vector3.Distance(transform.position,destPos);
		targetSpeed = (Mathf.Round (distance * 10.0f) / 10.0f) / 0.45f;
		if(targetSpeed > personalWalkSpeed) targetSpeed = personalWalkSpeed; // cannot go faster than this
		if (targetSpeed < (0.5f / 0.45f)) {
			targetSpeed = 0.0f;
			destPos = transform.position;
		}
	}

	//private Vector3 seatPos;
	
	void FixedUpdate () { // Update
		
		// we reached the end of the path
		if (mNavMeshAgent.enabled && !mNavMeshAgent.pathPending &&
						mNavMeshAgent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathInvalid &&
						mNavMeshAgent.remainingDistance != Mathf.Infinity && 
						mNavMeshAgent.remainingDistance <= 0.1f) {
						if (State == EState.Exiting) {
								//mNavMeshAgent.stoppingDistance = mStopDistance;
				
								// respawn
								GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag ("OuterPlatformExit");
								/*spawnPoints.AddRange( GameObject.FindGameObjectsWithTag("InnerPlatformExit") );
								List<GameObject> filteredSpawnPoints = new List<GameObject> (spawnPoints);
								// Hack; we don't want our npcs to spawn on the left side yet...
								filteredSpawnPoints.RemoveAll (delegate(GameObject sp) {
										return sp.GetComponent<StationTag> ().Side == "Left";
								});*/
								//mDestination = filteredSpawnPoints [Random.Range (0, filteredSpawnPoints.Count)];
								mDestination = spawnPoints [Random.Range (0, spawnPoints.Length)];
								mStation = mDestination.GetComponent<StationTag> ().Station;
				
								mWillGetIntoSubway = false;
								mNavMeshAgent.enabled = false;
								transform.position = mDestination.transform.position;
				
								StartCoroutine (StartNextFrame ());
				
								return;
						} else if (State == EState.Entering) {			
								State = EState.Travelling;
								mNavMeshAgent.enabled = false; // Francis
								if(assignedSpot != null && assignedSpot.parent.tag.Equals("Bench")){
									anim.SetBool("Sitting",true);
									StartCoroutine("SitActions");
								}
								return;
						} else if (State == EState.Walking) {
								State = EState.Waiting;
				
								if (!mWillGetIntoSubway)
										StartCoroutine (WaitAndPickAnotherSpot ());
						}
		}
		else if(State == EState.Walking || State == EState.Exiting || State == EState.Entering){ // walking
			WalkToTarget();
			//mNavMeshAgent.speed = pedestrianSpeed; // change limit on max speed
		}


		AdjustMovementSpeed();

		if(State == EState.Travelling && assignedSpot != null){ // turn person in direction
			transform.rotation = Quaternion.Slerp(transform.rotation,assignedSpot.rotation, 2.0f * Time.deltaTime);
			if(anim.GetBool("Sitting") == true){ // if sitting
				transform.position = Vector3.Lerp(transform.position,assignedSpot.position+(assignedSpot.forward*-0.18f)+(assignedSpot.up*0.03f),Time.deltaTime);
			}
		}

	}

	float seconds = 0.0f;
	float secondspassed = 0.0f;
	public void ResetSeconds(){
		seconds = 0.0f;
		secondspassed = 0.0f;
	}

	IEnumerator SitActions(){
		while(secondspassed < 41.0f) { // 1 minute for each ride

			//if(transform.parent.GetComponent<SubwayCoach>()) continue;

			//if(GetDestStation() == transform.parent.GetComponent<SubwayCoach>().GetCurrentStation()) break;

			//do stuff
			int randInt = UnityEngine.Random.Range (1, 6);
			int triggerHash = 0;
			switch (randInt) {
				case 1:
						triggerHash = fidgetFeetTrigger;
						break;
				case 2:
						triggerHash = sittingStillTrigger;
						break;
				case 3:
						triggerHash = handsOnThighsTrigger;
						break;
				case 4:
						triggerHash = impatientWaitTrigger;
						break;
				case 5:
						if (characteristics.male == false)
							triggerHash = crossLegTrigger; // women only
						else
							triggerHash = sittingStillTrigger;
						break;
			}
			anim.SetTrigger (triggerHash);

			seconds = Random.Range (4.0f, 12.0f);
			secondspassed += seconds;
			yield return new WaitForSeconds(seconds);
		}
	}

	float secLookDir = 0.0f;
	float much = 0.0f;
	float lookWeight = 1.0f;
	bool lookAtTarget = true;
	Transform lookTarget;
	Vector3 lookPosition;
	IEnumerator Look(){

		while (true) {
			lookAtTarget = true; // default is to look

			// decide where look
			int casenum = Random.Range(1,3); // cases
			casenum = 2;

			switch(casenum){
				case 1: // choose any person in subway car, outside subway car, or no one
					//lookTarget = transform.position + transform.forward * 1000.0f; // initial look
					if(transform.parent != null){ // in subway car?

						// TODO look at group member OR

						// choose random passenger to look at
						//List<NavigateSubway> passengers = transform.parent.GetComponent<SubwayCoach>().passengers;
						List<NavigateSubway> passengers = transform.parent.parent.Find("CoachController01").GetComponent<SubwayCoach>().passengers;
						//passengers.Remove(this);
						int randMax = passengers.Count;
						if(randMax > 0){
							int index = Random.Range(0,randMax);
							lookTarget = passengers[index].transform;
							much = Random.Range (0.4f, 0.6f);
						}
						else lookAtTarget = false; // don't look at anyone/anything
					}
					/*else{ // outside subway car on same side in station

						// TODO look at group member OR

						List<NavigateSubway> persons = GameObject.Find("SubwayAIControl").GetComponent<SubwayAIControl>().getPersonsOnSameSideInStation(mSide.ToString(),mStation);
						//persons.Remove(this); // remove self
						int randMax = persons.Count;
						if(randMax > 0){
							int index = Random.Range(0,randMax);
							lookTarget = persons[index].transform;
							much = Random.Range (0.4f, 0.6f);
						}
						else lookAtTarget = false; // don't look at anyone/anything
					}*/
					break;
				case 2: // look at player
					much = 0.0f; // stare at my eyes
					
					//if(UsingHMD) lookTarget = GameObject.Find("First Person Controller/OVRCameraRig/CenterEyeAnchor").transform;
					//else lookTarget = GameObject.Find("First Person Controller/Main Camera").transform;

					if(Vector3.Distance(transform.position,GameObject.FindWithTag("Player").transform.position) > 1.0f){ // 2 is much farther in this subway scale
						if(Random.Range(1,5) == 1){
							much = -1 * Random.Range (0.0f, 0.2f); // check me out from faraway sometimes (1/4 prob)
						}
						else{
							lookAtTarget = false;
						}
					}
					else{ // default is to look at my eyes
						int res = Random.Range(1,4);
						if(res == 1){
							much = -1 * Random.Range (0.0f, 0.2f); // check me out close up (1/3 prob)
						}
						else if(res == 2){ // ignore me
							lookAtTarget = false;
						}
					}
					break;
				//case 3: // interest point
				//	break;
			}


			// choose interest point

			// choose random spot in front

			//Vector3 target = new Vector3 ();
			//anim.SetLookAtPosition (transform.forward*100000.0f); // look approximately

			//anim.SetLookAtPosition(lookTarget);


			// effort in looking
			//float lookAtWeight = Random.Range (0.0f, 1.0f); // don't let looking affect motion to look fully

			//anim.SetLookAtWeight (lookAtWeight);

			secLookDir = Random.Range (1.0f, 10.0f);
			yield return new WaitForSeconds (secLookDir);
		}
	}

	float currentY = 0.0f;

	float currentX = 0.0f;
	float currentZ = 0.0f;

	void OnAnimatorIK(int layerIndex){

		//Vector3 lookPosition = lookTarget.localPosition + lookTarget.up*much;
		Vector3 lookPosition = lookTarget.position+lookTarget.up*much;

		// 1000
		/*currentX = Mathf.Lerp (currentX,lookPosition.x,Time.deltaTime*1.0f); // gradually adjust x position
		currentZ = Mathf.Lerp (currentZ,lookPosition.z,Time.deltaTime*1.0f); // gradually adjust z position
		lookPosition.x = currentX; // overwrite actual value
		lookPosition.z = currentZ;*/ // overwrite actual value

		currentY = Mathf.Lerp (currentY,lookPosition.y,Time.deltaTime); // gradually adjust y position
		lookPosition.y = currentY; // overwrite actual value
		/////////////anim.SetLookAtPosition(lookPosition); /////// enable this again! 3/25/2015

		//lookPosition = Vector3.Slerp (lookPosition,lookTarget.position+lookTarget.up*much,Time.deltaTime*100.0f);
		//lookPosition = lookTarget.position+lookTarget.up*much; // works!
		//anim.SetLookAtPosition(lookPosition);

		// 3/25/2015
		/*if (lookAtTarget) {
			lookWeight = Mathf.Lerp(lookWeight,1.0f,Time.deltaTime*lookSmoother);
			anim.SetLookAtWeight (lookWeight);
		} else {
			lookWeight = Mathf.Lerp(lookWeight,0.0f,Time.deltaTime*lookSmoother);
			anim.SetLookAtWeight (lookWeight);
		}*/

		/*animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootPositionWeight);
		animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootRotationWeight);
		animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootObj.position);
		animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootObj.rotation);*/
	}
	
	// catches the state changes and translate those into animation changes
	private EState State {
		get { return mState; }
		set { 
			
			if( value == mState )
				return;
			
			switch(value)
			{
			case EState.Travelling:
				break; // Francis
			case EState.Waiting:
				targetSpeed = 0.0f;
				destPos = transform.position;
				//animation.CrossFade(IDLE_ANIM, ANIM_BLEND_TIME);				
				break;
			case EState.Exiting:
			case EState.Entering:
			case EState.Walking:
				targetSpeed = personalWalkSpeed;
				//animation.CrossFade(WALK_ANIM, ANIM_BLEND_TIME);
				break;
			}
			
			mState = value;
		}
	}
	
	
	public EStationSide GetSide()
	{
		return mSide;
	}
	
	public string GetStation()
	{
		return mStation;
	}
	
	public string GetDestStation()
	{
		return mDestStation;
	}
	
	
	public bool CanGetIntoSubway(GameObject[] wagons)
	{
		GameObject closest = Utils.GetNearest(wagons, gameObject);
		Vector3 dist = closest.transform.InverseTransformPoint(transform.position);
		return mWillGetIntoSubway && (State == EState.Waiting || 
		                              dist.x < MaxDistanceToSubway );
	}
	
	
	public void GetIntoSubwayCar(GameObject[] wagons)
	{
		mNavMeshAgent.areaMask |= 1 << UnityEngine.AI.NavMesh.GetAreaFromName("PisoSubte");

		Transform wagon = Utils.GetNearest( wagons, gameObject ).transform;	// get the closest wagon
		transform.parent = wagon; // CoachController0X
		
		// pick the destiny station
		List<string> aux = new List<string>( AvailableStations );
		aux.Remove( mDestination.GetComponent<StationTag>().Station );
		
		mDestStation = aux[ Random.Range(0, aux.Count) ];
		
		// actually getting in
		Transform closestSitSectionTransform = null;
		string num = Regex.Match(wagon.name,"[\\+\\-]?\\d+\\.?\\d+").Value;
		Transform vagon = wagon.Find("vagon".Insert(5,num));

		// choose section of subway car to be in
		foreach (Transform child in vagon) {
			if(child.name.Equals("SitSection")){
				if(closestSitSectionTransform == null) closestSitSectionTransform = child;
				else{ // compare distances
					if(Vector3.Distance(child.position,transform.position) < Vector3.Distance(closestSitSectionTransform.position,transform.position)){
						closestSitSectionTransform = child;
					}
				}
			}
		}
		/////////
		// choose sit area
		int select = Random.Range(1,3); // 1 or 2
		//print ("SitArea/BenchPositions"+select);
		Transform bench = closestSitSectionTransform.Find("SitArea/BenchPositions"+select);

		// find seat in bench
		destPos = FindSeatInBench(bench);

		// if no seat, find place to stand next to bench instead
		if (destPos == Vector3.zero) {
			Transform stand = closestSitSectionTransform.Find ("SitArea/StandPositions" + select);

			// find spot near bench
			destPos = FindSpotNearBench (stand);
		}

		if(destPos == Vector3.zero) { // if failed to get spot, go near the doors
			// removing the waypoints that are not on the wagon that we're entering
			List<GameObject> doorPoint = new List<GameObject>( GameObject.FindGameObjectsWithTag( "WaypointsPisoSubte" ) );
			doorPoint.RemoveAll( delegate(GameObject obj) {
				return obj.transform.parent != wagon;
			} );
			destPos = doorPoint [Random.Range (0, doorPoint.Count)].transform.position;
		}

		mNavMeshAgent.SetDestination(destPos);
		State = EState.Entering;
	}

	private Vector3 FindSpotNearBench(Transform Stand){
		List<Transform> Spots = new List<Transform>();
		foreach(Transform spot in Stand) {
			if(spot.tag.Equals("o")) Spots.Add(spot);
		}
		
		while(Spots.Count > 0){
			int selectSeat = Random.Range (0, Spots.Count);
			if (Spots[selectSeat].tag.Equals("o")) { // check again
				Spots[selectSeat].tag = "x";
				assignedSpot = Spots[selectSeat];
				return Spots[selectSeat].position;
			}
			else Spots.Remove(Spots[selectSeat]);
		}
		return Vector3.zero;
	}

	private Vector3 FindSeatInBench(Transform Bench){
		List<Transform> Seats = new List<Transform>();
		foreach(Transform seat in Bench) {
			if(seat.tag.Equals("o")) Seats.Add(seat);
		}

		while(Seats.Count > 0){
			int selectSeat = Random.Range (0, Seats.Count);
			if (Seats[selectSeat].tag.Equals("o")) { // check again
				Seats[selectSeat].tag = "x";
				assignedSpot = Seats[selectSeat];
				return Seats [selectSeat].position;
			}
			else Seats.Remove(Seats[selectSeat]);
		}
		return Vector3.zero;
	}
	
	public void LeaveSubwayCar()
	{
		if (assignedSpot != null) {
			assignedSpot.tag = "o";
			if(assignedSpot.parent.tag.Equals("Bench")){
				anim.SetBool("Sitting",false); // stand up first
				StopCoroutine("SitActions");
				transform.position = transform.position + (assignedSpot.forward*0.09f)+ (assignedSpot.up*-0.03f); // move forward
			}
			assignedSpot = null;
		}
		mStation = mDestStation;
		mWillGetIntoSubway = false;
		
		transform.parent = null;
		mNavMeshAgent.enabled = true;
		//collider.isTrigger = true; ////////
		
		FillWaypoints( mSide, mDestStation );
		destPos = mExits [Random.Range (0, mExits.Count)].transform.position;
		mNavMeshAgent.SetDestination(destPos);
		State = EState.Exiting;
	}
	
	// this is called when the subway car starts its movement
	public void StayInWagon()
	{		
		// hack: we resolve collisions against the user using a collider, we should use a trigger and slide against the user
		// but somehow is not working properly when the wagon is in movement.
		//collider.isTrigger = false; ///////
		//mNavMeshAgent.enabled = false; // Francis
		mStation = "";
		
		//State = EState.Travelling; // Francis
	}
	
	// TODO: Maybe we should add and remove an obstacle component instead.
	public void EnableCollisions()
	{
		//////mNavMeshAgent.enabled = true; /////
	}
	
	
	IEnumerator WaitAndPickAnotherSpot()
	{	
		yield return new WaitForSeconds(IdleTime * Random.value);
		
		GoToRndSpotInPlatform();
	}
	
	IEnumerator StartNextFrame()
	{	
		yield return new WaitForSeconds(5.0f);
		
		mNavMeshAgent.enabled = true;
		Start();
	}
}
