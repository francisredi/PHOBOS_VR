using UnityEngine;
using System.Collections;
using RealisticEyeMovements;

namespace CrowdSim
{
	public class NavigateCity : StateMachineBehaviourEx{

		#region Variables

		// Finite State Machine for City Scene
		public enum NPCStates {Initializing, SubwayInOut, Walking, WaitAtDestination, MovingWithGroup, StandingIdleInGroup}

		// Animator
		public Animator anim;							// a reference to the animator on the character
		private AnimatorStateInfo currentLayer1State;	// a reference to the current state of the animator, used for base layer
		private AnimatorStateInfo currentLayer2State;	// a reference to the current state of the animator, used for 2nd layer
		private float animSpeed = 1.0f;					// a public setting for overall animator animation speed
		
		// Layer 2
		static int idleStateHash = Animator.StringToHash("Base Layer.Motion.Idle");
		static int walkStateHash = Animator.StringToHash("Base Layer.Motion.Walk");
		static int runStateHash = Animator.StringToHash("Base Layer.Motion.Run");
		
		// Layer 1
		static int askQ1Trigger = Animator.StringToHash("AskQ1Trigger");
		static int askQ2Trigger = Animator.StringToHash("AskQ2Trigger");
		static int dismissTrigger = Animator.StringToHash("DismissTrigger");
		static int shrugTrigger = Animator.StringToHash("ShrugTrigger");
		static int twoHandGestureTrigger = Animator.StringToHash("TwoHandGestureTrigger");
		static int wavingTrigger = Animator.StringToHash("WavingTrigger");
		static int crazyHandTrigger = Animator.StringToHash("CrazyHandTrigger");
		static int frustratedTrigger = Animator.StringToHash("FrustratedTrigger");

		// Private variables
		private float pedestrianSpeed = 1.0f;	// default walk speed when in group
		private float targetSpeed = 0.0f;		// speed to adjust to
		private float stoppingDistance = 0.1f;	// how close to target can begin to stop
		private float personalWalkSpeed = 0.0f; // when walking alone
		private UnityEngine.AI.NavMeshAgent navComponent;		// cache of navigation mesh agent component
		private GameObject characterSpot;		// the circle underneath the character
		private float scale = CitySimulator.scale; // in the future, this should be 1 meter!!!
		private GameObject mDestination;
		private GameObject mSubwayRef;			// a reference used to get in and out of the subway stations
		private Color color;					// color of group
		private float prevDistanceToTarget = float.MaxValue;
		private Vector3 targetPos;				// where to go
		private bool panic;						// if in panic mode
		private bool waiting;					// if waiting
		private Vector3 COLLISION_OFFSET = new Vector3(0f, 0.5f, 0f);
		private Semaphore mSemRef;
		private GameObject mCrossRef;
		private Vector3 leftHandHoldPosition;
		private Vector3 rightHandHoldPosition;
		private bool useLeftHand = false;
		private bool useRightHand = false;
		private float leftHandAnimWeight = 0.0f;
		private float rightHandAnimWeight = 0.0f;
		private Vector3 velocity;
		private Vector3 angularVelocity;
		private float timeDelay = 0.0f;
		private const float DIRECTION_EPSILON = 0.5f;
		private const float SIDE_EPSILON = 0.1f;
		private const float AWAY_DIST_RED = 15f;
		private const float AWAY_DIST_GREEN = 6f;
		private const float CAR_PANIC_DIST = 0.8f; // 0.8

		// Public variables
		public int m_ei;						// unique identifier
		public GameObject assignedGA;			// what group agent character belongs
		public int assignedSlotNumber;
		public bool stopped = false;
		public bool goNow = false;

		//private LookAt lookAt;

		private LookTargetController ltc;
		private EyeAndHeadAnimator eha;

		// Private Cache
		private Characteristics characteristics;

		#endregion

		#region Subroutines

		private void Talk(){
			int randInt = UnityEngine.Random.Range(1,9);
			int triggerHash = 0;
			switch(randInt){
			    case 1:
				    triggerHash = askQ1Trigger;
				    timeDelay += 4.0f;
				    break;
			    case 2:
				    triggerHash = askQ2Trigger;
				    timeDelay += 4.0f;
				    break;
			    case 3:
				    triggerHash = dismissTrigger;
				    timeDelay += 4.0f;
				    break;
			    case 4:
				    triggerHash = shrugTrigger;
				    timeDelay += 2.0f;
				    break;
			    case 5:
				    triggerHash = twoHandGestureTrigger;
				    timeDelay += 4.0f;
				    break;
			    case 6:
				    triggerHash = wavingTrigger;
				    timeDelay += 4.0f;
				    break;
			    case 7:
				    triggerHash = crazyHandTrigger;
				    timeDelay += 5.0f;
				    break;
			    case 8:
				    triggerHash = frustratedTrigger;
				    timeDelay += 7.0f;
				    break;
			}
			anim.SetTrigger(triggerHash);
		}

		public bool PersonIsReadyToGo(){
			if(goNow) return true;
			return false;
		}

		public void MoveWithGroup(){
			currentState = NPCStates.MovingWithGroup;
		}

		private void AdjustMovementSpeed(){
			pedestrianSpeed = targetSpeed;
			if(anim == null) anim = GetComponent<Animator>();
			anim.SetFloat("Speed", pedestrianSpeed * scale);
		}

		private void DeterminePersonalWalkSpeed(){
			if(characteristics.age < 10 && !characteristics.obese) personalWalkSpeed = 1.6f; //Random.Range(1.3f, 1.6f); // fast - kids
			else if(characteristics.age < 45 && !characteristics.obese) personalWalkSpeed = 1.3f; //Random.Range(1.0f, 1.3f); // normal
			else personalWalkSpeed = 1.0f; //Random.Range(0.7f, 1.0f); // slow - old people, obese people
			personalWalkSpeed = Mathf.Round(personalWalkSpeed * 10.0f)/10.0f;
		}

		private float WalkToTarget(Vector3 dest){
			float distance = Vector3.Distance(transform.position,dest);
			targetSpeed = Mathf.Round(distance * 10.0f) / 10.0f;
			if(targetSpeed > personalWalkSpeed) targetSpeed = personalWalkSpeed; // cannot go faster than this
			if (targetSpeed < 0.5f) targetSpeed = 0.0f;
			return distance;
		}

		private float UpdateTargetSpeedByDistanceToDestination(Vector3 dest){
			float distance = Vector3.Distance(transform.position,dest);
			targetSpeed = Mathf.Round(distance * 10.0f) / 10.0f;
			if(targetSpeed > 5.7f) targetSpeed = 5.7f; // cannot run faster than this
			if(targetSpeed < 0.5f) targetSpeed = assignedGA.GetComponent<GroupAgent>().gSpeed;
			return distance;
		}

		private void TurnToCentroid(){ // turn to centroid of group members
			Vector3 centroid = assignedGA.GetComponent<GroupAgent>().getCentroid();
			Vector3 vector = centroid - transform.position;
			
			if(vector != Vector3.zero){
				float angle = Quaternion.Angle(transform.localRotation,Quaternion.LookRotation(vector,Vector3.up));
				if(angle > 0.1f) transform.localRotation = Quaternion.Slerp(transform.localRotation,Quaternion.LookRotation(vector,Vector3.up),1.0f * Time.deltaTime); // turn speed
			}
		}

		private void CreateNavMeshAgent(){ // keep on navmesh!
			navComponent = transform.gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
			CapsuleCollider cc = (CapsuleCollider) transform.GetComponent("CapsuleCollider");
			navComponent.radius = cc.radius; // personal space others cannot pass
			navComponent.height = cc.height; // determines passing under obstacles or not
			navComponent.speed = 5.7f; // maximum speed allowed for agent
			navComponent.autoBraking = true; // prevent overshooting of destination point
			navComponent.autoRepath = true; // acquire new path if existing path becomes invalid
			navComponent.stoppingDistance = stoppingDistance; // in case cannot stop exactly at destination
			cc.isTrigger = true; // for car collisions and unstoppable navigation // edited! 12/23/2014
			//Destroy (cc); // need to activate trigger with rigidbody, by not destroying them

			/*eha = transform.gameObject.AddComponent<EyeAndHeadAnimator>();
			eha.headWeight = 1.0f;
			eha.useMicroSaccades = true;
			eha.useMacroSaccades = true;
			eha.kDrawSightlinesInEditor = false;
			eha.kMinNextBlinkTime = 3.0f;
			eha.kMaxNextBlinkTime = 15.0f;
			eha.maxEyeUpAngle = 0.0f;
			eha.maxEyeDownAngle = 0.0f;

			ltc = transform.gameObject.AddComponent<LookTargetController>();
			ltc.lookAtPlayerRatio = 0.0f;
			ltc.stareBackFactor = 0.0f;
			ltc.noticePlayerDistance = 0.0f;
			ltc.minLookTime = 2.0f;
			ltc.maxLookTime = 5.0f;*/
		}

		IEnumerator WaitingLook(){
			Quaternion finalRot = Quaternion.LookRotation((mDestination.transform.position - transform.position).normalized, Vector3.up);
			Quaternion currRot = transform.rotation;
			
			const int steps = 30;
			float delta = 1f / steps;
			float a = 0f;
			const float delay = 0.05f;
			for(int i = 0; i < steps; i++){
				transform.rotation = Quaternion.Slerp( currRot, finalRot, a );
				a += delta;
				yield return new WaitForSeconds(delay);
			}
		}

		#endregion

		#region Initialize State

		public void Start(){
			//Destroy(transform.GetComponent<Rigidbody>()); // needed to activate triggers

			characteristics = GetComponent<Characteristics>();

			characterSpot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			characterSpot.transform.localScale = new Vector3(0.6f / scale,0.01f,0.6f / scale);
			Destroy(characterSpot.GetComponent<Collider>());
			characterSpot.transform.parent = GameObject.Find("2_PedestrianDiscs").transform;
			characterSpot.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
			color = characterSpot.GetComponent<Renderer>().material.color;
			if (!CitySimulator.renderGroupAgent) color.a = 0.0f;
			
			if(navComponent == null) CreateNavMeshAgent();

			int mask = 1 << UnityEngine.AI.NavMesh.GetAreaFromName("CityWalk");
			mask |= 1 << UnityEngine.AI.NavMesh.GetAreaFromName("Road");
			navComponent.areaMask = mask;
			
			#region Animation State Machine
			if(m_ei != -1){
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
			}
			#endregion

			DeterminePersonalWalkSpeed();
			//lookAt = GetComponent<LookAt>();
			currentState = NPCStates.Initializing;
		}

		public void Update() {
			characterSpot.transform.position = transform.position;
		}

		void Initializing_EnterState()
		{
			if (assignedGA != null) {
				currentState = NPCStates.MovingWithGroup;
			} else {
				currentState = NPCStates.Walking;
			}
		}

		void Initializing_FixedUpdate()
		{

		}
		#endregion

		#region Moving Alone (No Group Agent)
		
		void SubwayInOut_EnterState(){
			if(navComponent == null) CreateNavMeshAgent();
			navComponent.enabled = false;
			///transform.GetComponent<Rigidbody>().useGravity = false; // disable gravity!
		}
		
		void SubwayInOut_FixedUpdate(){
			Vector3 dir = (mSubwayRef.transform.position - transform.position).normalized;
			transform.Translate( dir * Time.deltaTime * personalWalkSpeed, Space.World );
			dir.y = 0;
			transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime*2f);
			anim.SetFloat("Speed", personalWalkSpeed * scale);
			const float epsilon = 0.07f;
			
			if(Utils.GetDist(gameObject, mSubwayRef) < epsilon){				
				if(mSubwayRef.tag != "Untagged"){
					if( mSubwayRef.tag == "WaypointsBajadaSubte"){ // the person got into the subway station, so just warp it to another location
						transform.position = CitySimulator.getSpawnPoint();
						currentState = NPCStates.Walking;
						return;
					}
				}
				mSubwayRef = mSubwayRef.GetComponent<ObjectRef>().Ref;
			}
		}

		void WaitAtDestination_FixedUpdate(){
			if (timeInCurrentState > 10.0f) {
				currentState = NPCStates.Walking;
				return;
			}
		}

		void Walking_EnterState()
		{
			characterSpot.GetComponent<Renderer>().material.color = color;
			if(navComponent == null) CreateNavMeshAgent();
			navComponent.enabled = true;
			///transform.GetComponent<Rigidbody>().useGravity = true; // enable gravity!

			mDestination = CitySimulator.getWayPointObject();
			targetPos = mDestination.transform.position;
			navComponent.SetDestination(targetPos);

			//GameObject assignedGA = GetComponent<Navigate>().assignedGA;
			if(assignedGA!=null){
				assignedGA.GetComponent<GroupAgent>().RemoveMember(this.gameObject);
				assignedGA = null;
			}
			assignedSlotNumber = 10; // makes sure get real slot number later
			mSubwayRef = null;
			panic = false;
			waiting = false;
		}

		void Walking_Update()
		{
			//lookAt.lookAtTargetPosition = navComponent.steeringTarget + transform.forward;
		}
		
		void Walking_FixedUpdate()
		{
			if(targetPos != null && !panic){ // move!

				if(!waiting) navComponent.Resume();
				float distance = WalkToTarget(targetPos); // only walk to destination in normal circumstances //updateTargetSpeedByDistanceToDestination(targetPos);
				AdjustMovementSpeed(); // adjust speed

				//if(PedestrianSimulator.useNavmeshAgentNavigation){ // use navmesh agent to get agent path to destination
				navComponent.speed = pedestrianSpeed; // change limit on max speed
				//}

				if(waiting){ // waiting at traffic light
					anim.SetFloat("Speed", 0.0f);
					if(mSemRef != null){ // check traffic light to start walking again
						Semaphore.EState redState = Semaphore.EState.Red;
						if(Mathf.Abs(Vector3.Dot(mSemRef.transform.up, mCrossRef.transform.forward) ) < SIDE_EPSILON) redState = Semaphore.EState.Green;
						if(mSemRef.GetState() == redState && CarsAreFarAway(AWAY_DIST_GREEN)) waiting = false;
					}

					if(CarsAreFarAway(AWAY_DIST_RED)) waiting = false; // we're still waiting so check if there is no cars near and start walking again
				}
				else{ // not waiting for traffic light yet
					if(targetSpeed == 0.0f || distance < stoppingDistance){	// stop only if nearby or speed is 0
						ObjectRef destRef = mDestination.GetComponent<ObjectRef>();
						if( destRef != null ){										// we reached a waypoint that will lead us into a subway station.
							mSubwayRef = destRef.Ref;
							currentState = NPCStates.SubwayInOut;
							return;
						}
						else if( mDestination.tag == "WaypointsVeredaSpawn" ){		// entered a doorway, so teleport to another place
							transform.position = CitySimulator.getSpawnPoint();
							mDestination = CitySimulator.getWayPointObject();
							targetPos = mDestination.transform.position;
							navComponent.SetDestination(targetPos);
							mSubwayRef = null;
							return;
						}
						else{						// wait!
							targetSpeed = 0.0f;		// stop
							AdjustMovementSpeed();
							navComponent.speed = pedestrianSpeed; // change limit on max speed
							//StartCoroutine("WaitingLook"); // turn body toward destination
							//StopCoroutine("WaitingLook"); // turn body toward destination
							currentState = NPCStates.WaitAtDestination;
							return;
						}
					}
				}

				if(!CarsAreFarAwayAndNotInFront()){ //we're almost being hit by a car so stop right here!
					navComponent.Stop();
					anim.SetFloat("Speed", 0.0f);
					panic = true;
				}

			}
			else if(panic && CarsAreFarAwayAndNotInFront() ){
				navComponent.Resume();
				anim.SetFloat("Speed", personalWalkSpeed * scale);
				panic = false;
			}
		}
		#endregion



		/*private bool CarsAreFarAway (float dist) {
			
			foreach(GameObject car in mCars)
				if( !car.GetComponent<CarAi>().mIsStop && Utils.GetDist(car, gameObject) < dist )
					return false;
			
			return true;
		}*/

		private bool CarsAreFarAway (float dist) {

			//navComponent

			// much faster!
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, dist, CitySimulator.m_NPC_Cars_Collision_LayerForOverLapSphere);
			int i = 0;
			while (i < hitColliders.Length) {
				//e.g. hitColliders[i].SendMessage("Stop");
				//if(hitColliders[i].GetComponentInParent<CarAi>().mIsStop) return false;
				if(!hitColliders[i].transform.parent.GetComponent<CarAi>().mIsStop) return false;
				i++;
			}

			// much slower!
			/*foreach(GameObject car in mCars)
				if( !car.GetComponent<CarAi>().mIsStop && Utils.GetDist(car, gameObject) < dist )
					return false;*/
			
			return true;
		}

		private bool CarsAreFarAwayAndNotInFront() {
			// much faster!
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, CAR_PANIC_DIST, CitySimulator.m_NPC_Cars_Collision_LayerForOverLapSphere);
			int i = 0;
			while (i < hitColliders.Length) {
				//e.g. hitColliders[i].SendMessage("Stop");
				GameObject car = hitColliders[i].transform.gameObject;
				bool isInFront = transform.InverseTransformPoint(car.transform.position).z > 0;
				if(!car.transform.parent.GetComponent<CarAi>().mIsStop && isInFront && IsInPanicDist(car, CAR_PANIC_DIST)){
					//lookAt.lookAtTargetPosition = car.transform.position + transform.forward;
					return false;
				}
				/*GameObject car = hitColliders[i].transform.parent.gameObject;
						bool isInFront = transform.InverseTransformPoint(car.transform.position).z > 0;
						if(!car.GetComponent<CarAi>().mIsStop && isInFront && IsInPanicDist(car, dist)) return false;*/
				i++;
			}
			
			// much slower!
			
			/*foreach(GameObject car in mCars)
					{			
						bool isInFront = transform.InverseTransformPoint(car.transform.position).z > 0;
						if( !car.GetComponent<CarAi>().mIsStop && isInFront && IsInPanicDist(car, dist) )
							return false;
					}*/
			
			//lookAt.lookAtTargetPosition = navComponent.steeringTarget + transform.forward;
			
			return true;
		}
		
		private bool IsInPanicDist (GameObject obj, float dist) {
			Vector3[] borders = new Vector3[5];
			MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
			Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
			foreach(MeshFilter filter in meshFilters){
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
				if( (localToObjPos - point).magnitude < dist ) return true;
			
			return false;
		}

		void OnTriggerStay(Collider other){	
			if (other.gameObject.layer == LayerMask.NameToLayer ("NPC_Cars")) { // colliding with a car	//_Collision
				Vector3 colPos = other.ClosestPointOnBounds (transform.position + COLLISION_OFFSET);
				Vector3 colVec = transform.position + COLLISION_OFFSET - colPos;
				Vector3 slideVec = colVec;
				if (colVec == Vector3.zero) {// in case that we're inside the collider so ClosestPointOnBounds returned the same input point
						slideVec = transform.position - other.transform.position;
						slideVec.y = 0;
						slideVec.Normalize ();
				}
				transform.Translate (slideVec * 0.1f, Space.World);
				return;
			}

			if(waiting || CarsAreFarAway(AWAY_DIST_RED)) return; // do not check if we're not moving or there are no near cars

			ObjectRef objRef  = other.gameObject.GetComponent<ObjectRef>();
			if(objRef == null) return; // just work with gameobjs that have a ObjectRef script.
			
			mSemRef = objRef.Ref.GetComponent<Semaphore>();
			mCrossRef = other.gameObject;
			
			Semaphore.EState redState = Semaphore.EState.Red;
			if(Mathf.Abs( Vector3.Dot(mSemRef.transform.up, mCrossRef.transform.forward) ) < SIDE_EPSILON )
				redState = Semaphore.EState.Green; // if the semaphore is from the perpendicular street, we cross when it is green, not red.
			
			float dotRes = Vector3.Dot(transform.forward, mCrossRef.transform.forward);
			if(mSemRef.GetState() != redState  && dotRes > 0 && 1f - dotRes < DIRECTION_EPSILON ){ // mCrossRef points to the cross street direction, so this checks if we're crossing
				waiting = true; //stop walking, start waiting
				anim.SetFloat("Speed", 0.0f); // make sure speed is fully stopped
				navComponent.Stop();
				return;
			}
			

		}





		#region Moving with Group Agent

		public void SetLeftHand(Vector3 pos, bool use){
			leftHandHoldPosition = pos;
			useLeftHand = use;
		}
		
		public void SetRightHand(Vector3 pos, bool use){
			rightHandHoldPosition = pos;
			useRightHand = use;
		}
		
		public Vector3 GetLeftHandPos(){
			return anim.GetBoneTransform(HumanBodyBones.LeftHand).position; // Left Hand (works best)
			//return anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal).position; // Left Hand
			//return anim.GetIKPosition(AvatarIKGoal.LeftHand);
		}
		
		public Vector3 GetRightHandPos(){
			return anim.GetBoneTransform(HumanBodyBones.RightHand).position; // Right Hand (works best)
			//return anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal).position; // Right Hand
			//return anim.GetIKPosition(AvatarIKGoal.RightHand);
		}

		public Vector3 GetLeftShoulder(){
			return anim.GetBoneTransform(HumanBodyBones.LeftUpperArm).position; // Left Hand
			//return anim.GetIKPosition(AvatarIKGoal.LeftHand);
		}
		
		public Vector3 GetRightShoulder(){
			return anim.GetBoneTransform(HumanBodyBones.RightUpperArm).position; // Right Hand
			//return anim.GetIKPosition(AvatarIKGoal.RightHand);
		}

		public Vector3 GetUpperBack(){
			return anim.GetBoneTransform(HumanBodyBones.Head).position; // Upper Back
			//return anim.GetIKPosition(AvatarIKGoal.RightHand);
		}

		public Vector3 GetLowerSpine(){
			return anim.GetBoneTransform(HumanBodyBones.Spine).position; // Lower Spine
			//return anim.GetIKPosition(AvatarIKGoal.RightHand);
		}

		void OnAnimatorIK(int layerIndex){
			if(currentState.Equals(NPCStates.MovingWithGroup) && timeInCurrentState > 1.0f){

				//if(currentState == GestureStates.HoldingHand){
					if(useLeftHand){
						//transform.rigidbody.isKinematic = true;
						anim.SetIKPosition(AvatarIKGoal.LeftHand,leftHandHoldPosition);
						leftHandAnimWeight = Mathf.Lerp(leftHandAnimWeight, 1.0f, Time.deltaTime); // Time.time

						anim.SetIKRotation(AvatarIKGoal.LeftHand,Quaternion.LookRotation(transform.forward));
					}
					else{
						leftHandAnimWeight = Mathf.Lerp(leftHandAnimWeight, 0.0f, Time.deltaTime);
					}
					anim.SetIKPositionWeight(AvatarIKGoal.LeftHand,leftHandAnimWeight);

					if(useRightHand){
						anim.SetIKPosition(AvatarIKGoal.RightHand,rightHandHoldPosition);
						rightHandAnimWeight = Mathf.Lerp(rightHandAnimWeight, 1.0f, Time.deltaTime);

						anim.SetIKRotation(AvatarIKGoal.RightHand,Quaternion.LookRotation(transform.forward));
					}
					else{
						rightHandAnimWeight = Mathf.Lerp(rightHandAnimWeight, 0.0f, Time.deltaTime);
					}
					anim.SetIKPositionWeight(AvatarIKGoal.RightHand,rightHandAnimWeight);
				//}
				/*else{ // use end of middle finger (no finger IK!)
					if(useLeftHandMiddleFingerTip){
						//transform.rigidbody.isKinematic = true;
						anim.SetIKPosition(AvatarIKGoal.LeftHand,leftHandHoldPosition);
						leftHandAnimWeight = Mathf.Lerp(leftHandAnimWeight, 1.0f, Time.deltaTime); // Time.time
					}
					else{
						leftHandAnimWeight = Mathf.Lerp(leftHandAnimWeight, 0.0f, Time.deltaTime);
					}
					anim.SetIKPositionWeight(AvatarIKGoal.LeftHand,leftHandAnimWeight);
					
					if(useRightHandMiddleFingerTip){
						anim.SetIKPosition(AvatarIKGoal.RightHand,rightHandHoldPosition);
						rightHandAnimWeight = Mathf.Lerp(rightHandAnimWeight, 1.0f, Time.deltaTime);
					}
					else{
						rightHandAnimWeight = Mathf.Lerp(rightHandAnimWeight, 0.0f, Time.deltaTime);
					}
					anim.SetIKPositionWeight(AvatarIKGoal.RightHand,rightHandAnimWeight);
				}*/

				//transform.rigidbody.isKinematic = false;
			}
			else{ // idle state
				leftHandAnimWeight = Mathf.Lerp(leftHandAnimWeight, 0.0f, Time.deltaTime);
				rightHandAnimWeight = Mathf.Lerp(rightHandAnimWeight, 0.0f, Time.deltaTime);
				anim.SetIKPositionWeight(AvatarIKGoal.LeftHand,leftHandAnimWeight);
				anim.SetIKPositionWeight(AvatarIKGoal.RightHand,rightHandAnimWeight);
			}
		}

		private void MovingRoutine(){
			navComponent.speed = pedestrianSpeed; // change limit on max speed
			navComponent.SetDestination(targetPos);
			navComponent.Resume(); // does it work? yes, solves groups getting stuck problem
		}

		void MovingWithGroup_EnterState()
		{
			if(characterSpot != null) characterSpot.GetComponent<Renderer>().material.color = assignedGA.GetComponent<GroupAgent>().groupColor;
			stopped = false;
			//////navComponent.enabled = true; // allow movement
			///rigidbody.isKinematic = true;
			if(navComponent == null) CreateNavMeshAgent();
			navComponent.enabled = true; // make sure
			
			//StartCoroutine("MovingRoutine");
			InvokeRepeating("MovingRoutine", 0, 0.5f); // 0.8f
		}

		void MovingWithGroup_Update()
		{
			//lookAt.lookAtTargetPosition = navComponent.steeringTarget + transform.forward;
		}
		
		void MovingWithGroup_FixedUpdate(){
			if(timeInCurrentState > 1.0f){ // delay 1 second before following group agent (prevents strange movement when group agent changes orientation)
				/*if(PedestrianSimulator.pedestriansLoaded && assignedGA != null){

				// change to stopped state if huddle completed!
				if(assignedGA.GetComponent<GroupAgent>().huddleFormationComplete){
					currentState = NPCStates.StandingIdleInGroup;
					return;
				}*/

				targetPos = assignedGA.GetComponent<GroupAgent>().getSlotPosition(assignedSlotNumber); // get target position

				// draw line to assigned slot
				if(CitySimulator.renderHiddenObjects) Debug.DrawLine(transform.position, targetPos, Color.green);
				
				/*
				// first rotate body!
				Vector3 direction = targetPos - transform.position;
				transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(direction,Vector3.up),1.5f * Time.deltaTime);
				*/
					
				// move!
				float distance = UpdateTargetSpeedByDistanceToDestination(targetPos);
				// leave group
				/*if(distance > 5.0f || timeInCurrentState > leaveTimeSec){
					currentState = NPCStates.Walking;
					return;
				}*/

				AdjustMovementSpeed(); // adjust speed
				
				/*if(PedestrianSimulator.useNavmeshAgentNavigation){ // use navmesh agent to get agent path to destination
					navComponent.speed = speedPed; // change limit on max speed
					navComponent.SetDestination(targetPos);
					//Vector3.Distance(navComponent.desiredVelocity,Vector3.zero);
				}
				else{ // control agent movement yourself
					/// Code for not using Navmesh contol (by not setting target)
					// first rotate body!
					Vector3 direction = targetPos - transform.position;
					transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(direction,Vector3.up),1.5f * Time.deltaTime);

					transform.Translate(0,0,Time.deltaTime*speedPed*0.5f); // move forward
				}*/

				//anim.SetFloat("Speed", speedPed);

				//prevSlotPosition = targetPos;
				
				if(!assignedGA.GetComponent<GroupAgent>().isMoving()){ // group agent stopped

					if(assignedGA.GetComponent<GroupAgent>().ifWaitingAtTrafficLight()){ // if waiting at traffic light, bypass if not waiting mode

						if(distance < stoppingDistance){ // stop only if nearby assigned slot
							currentState = NPCStates.StandingIdleInGroup;
						}
						else{
							
							if(Mathf.Abs(prevDistanceToTarget - distance) < 0.0001f){ // user position did not change, just stop
								currentState = NPCStates.StandingIdleInGroup;
							}
							
							/*if(distance < 10.0f){
								print ("less 10");
								// check if someone in the way, if so, also stop
								Ray ray = new Ray(transform.position,transform.forward);
								RaycastHit hit;

								if (Physics.Raycast (ray,out hit,distance)){ // ray cast to destination
									print ("ray collided");
									currentState = NPCStates.StandingIdleInGroup; // stop, something blocking way to assigned slot
								}
							}*/
						}
					}
				}
				
				prevDistanceToTarget = distance;
				//previousPedestrianPosition = transform.position;
				
				/*if(!assignedGA.GetComponent<GroupAgent>().isMoving() && distance < 0.5f){ // just stop
					currentState = NPCStates.StandingIdleInGroup;
				}*/
			}
		}
		
		void MovingWithGroup_ExitState(){
			//StopCoroutine("MovingRoutine");
			CancelInvoke("MovingRoutine");
			///rigidbody.isKinematic = false;
		}
		
		#endregion

		#region Idle state
		
		void StandingIdleInGroup_EnterState(){
			pedestrianSpeed = 0.0f;
			targetSpeed = 0.0f; // stop
			AdjustMovementSpeed();
			stopped = true;
			goNow = false;
			timeDelay = UnityEngine.Random.Range(0.0f, 5.0f); // change wait time for next talk
			///velocity = rigidbody.velocity;
			///angularVelocity = rigidbody.angularVelocity;
			///rigidbody.isKinematic = false; // make sure
			///rigidbody.velocity = Vector3.zero;
			///rigidbody.angularVelocity = Vector3.zero;
			///rigidbody.useGravity = false;
			///rigidbody.isKinematic = true;
		}
		
		void StandingIdleInGroup_FixedUpdate(){
			if(timeInCurrentState > timeDelay){ // delay 5 seconds

				// if group agent wants to go, start moving
				if(assignedGA.GetComponent<GroupAgent>().flagWantToGo){
					goNow = true;
					return;
				}
				else{
					timeDelay += UnityEngine.Random.Range(0.0f, 5.0f); // change wait time for next talk
					Talk();
				}
			}
			else if(timeInCurrentState > 1.0f){ // delay reporting of positions and turning to center of group
				assignedGA.GetComponent<GroupAgent>().notifyPedestrianPosition(assignedSlotNumber,transform.position);
				TurnToCentroid();
			}
		}
		
		void StandingIdleInGroup_ExitState(){
			assignedGA.GetComponent<GroupAgent>().resetAllPedestrianPosition();
			///rigidbody.isKinematic = false;
			///rigidbody.velocity = velocity;
			///rigidbody.angularVelocity = angularVelocity;
			///rigidbody.useGravity = true;
		}

		#endregion
	}
}