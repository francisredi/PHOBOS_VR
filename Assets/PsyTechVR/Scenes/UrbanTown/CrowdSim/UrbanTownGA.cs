using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Wayfinding;

namespace CrowdSim
{

    public class UrbanTownGA : StateMachineBehaviourEx
    {

        public enum GAStates { Disabled, WaitForLoadFinish, Player, Moving, Huddle, Waiting }
        public static string IGNORE_WAYPOINT_AGENT_NAME = "GroupAgent(Clone)";

        // Gesture States
        public enum GestureStates { None, HoldingHand, HandOnShoulder, HandOnUpperBack, HandOnLowerBack, HandOnHip }

        private float radius; // radius of group agent disc

        // used for linear interpolation
        private Transform slot1node;
        private Transform slot2node;
        private Transform slot3node;
        private Transform slot4node;
        private Transform slot5node;

        private ArrayList membersList = new ArrayList(); // all group members

        private Vector3 pedestrianPosition1 = new Vector3();
        private Vector3 pedestrianPosition2 = new Vector3();
        private Vector3 pedestrianPosition3 = new Vector3();
        private Vector3 pedestrianPosition4 = new Vector3();
        private Vector3 pedestrianPosition5 = new Vector3();

        public Color groupColor;
        private Vector3 centroid;
        //private GameObject gaSpot;
        private Transform nearestGroupAgent;

        #region Default Group Speed based on Age
        private int oldestAge; // middle-age default
        public float gSpeed;
        private float originalSpeed;
        private float scale;

        private GameObject offsetWaypoint;
        private int formationState = 0; // abreast
        private float gmaFrequency = 1.0f; // the frequency with which to re-scane for new nearest target in seconds
        private float distanceToBoundary = 10.0f; // abreast formation by default
        private float comingDistance = 20.0f;

        private bool destinationReached = false;
        private bool huddleFormationComplete = false;
        public bool flagWantToGo = false;
        //public bool allReadyToMoveNow = false; // do not move group until all pedestrians stop gesture interaction

        //Transform backLeftTransform;
        //Transform backRightTransform;

        private bool slot1, slot2, slot3, slot4, slot5;
        private int numMembers;

        private ArrayList breadcrumbs = new ArrayList();

        public GameObject currentNode;
        public GameObject destinationNode;
        public GameObject destinationOffsetNode = null; // 12/3/2014
        public Graph graph; // graph reference
        private List<Node> pathList = new List<Node>();
        private int currentWP;
        //private BoxRegionTrigger boxRegion;
        private Vector3 startNodePosition;
        public GameObject boundaryPointHelper;
        private GameObject boundaryPoint;

        private float targetSpeed;
        private float turnSpeed;
        private string dest;

        private GameObject[] groupAgentRef; // from PedestrianSimulator class
        private ArrayList destinationList; // from PedestrianSimulator class

        private GestureStates S1Gesture;
        private GestureStates S2Gesture;
        private GestureStates S3Gesture;
        private GestureStates S4Gesture;
        private GestureStates S5Gesture;

        void OnEnable() // before Start(), set defaults
        {
            membersList.Clear();
            breadcrumbs.Clear();
            pathList.Clear();
            numMembers = 0;
            resetAllPedestrianPosition();
            nearestGroupAgent = null;
            oldestAge = 30;
            gSpeed = 1.0f;
            slot1 = slot2 = slot3 = slot4 = slot5 = true;
            destinationReached = false;
            huddleFormationComplete = false;
            flagWantToGo = false;
            currentNode = null;
            destinationNode = null;
            currentWP = 0;
            targetSpeed = 1.0f;
            turnSpeed = 1.5f;
            dest = "";

            currentState = GAStates.WaitForLoadFinish;
        }

        private void UpdateOldestAgeInGroup()
        {
            int tempOldAge = 1; // default baby age
            bool obeseMember = false;
            for (int i = 0; i < membersList.Count; i++)
            {
                GameObject go = (GameObject)membersList[i];
                if (go.GetComponent<Characteristics>().age > tempOldAge)
                {
                    tempOldAge = go.GetComponent<Characteristics>().age;
                }
            }
            if (tempOldAge == 1) tempOldAge = 30; // set to middle age
            oldestAge = tempOldAge;
            if (oldestAge < 10 && !obeseMember)
            {
                gSpeed = Random.Range(1.4f, 1.6f); // fast - kids, 1.6
            }
            else if (oldestAge < 45 && !obeseMember)
            {
                gSpeed = Random.Range(1.2f, 1.5f); // normal, 1.3

            }
            else
            {
                gSpeed = Random.Range(1.1f, 1.3f); // slow - old people, obese people, 1.0
            }
            gSpeed = (Mathf.Round(gSpeed * 10.0f) / 10.0f) - ((membersList.Count - 1) * 0.1f);
            originalSpeed = gSpeed;
            targetSpeed = gSpeed;
        }
        #endregion

        #region Membership and Slot Assignment

        private int ClosestSlotNumber(GameObject member)
        {
            float smallest = 10000.0f;
            int index = 0;
            float dist1 = Vector3.Distance(member.transform.position, slot1node.transform.position);
            float dist2 = Vector3.Distance(member.transform.position, slot2node.transform.position);
            float dist3 = Vector3.Distance(member.transform.position, slot3node.transform.position);
            float dist4 = Vector3.Distance(member.transform.position, slot4node.transform.position);
            float dist5 = Vector3.Distance(member.transform.position, slot5node.transform.position);

            if (slotIsAvailable(1) && dist1 < smallest) { smallest = dist1; index = 1; }
            if (slotIsAvailable(2) && dist2 < smallest) { smallest = dist2; index = 2; }
            if (slotIsAvailable(3) && dist3 < smallest) { smallest = dist3; index = 3; }
            if (slotIsAvailable(4) && dist4 < smallest) { smallest = dist4; index = 4; }
            if (slotIsAvailable(5) && dist5 < smallest) { smallest = dist5; index = 5; }

            return index;
        }

        private void ReassignSlots()
        {

            slot1 = slot2 = slot3 = slot4 = slot5 = true;

            for (int i = 0; i < membersList.Count; i++)
            {
                GameObject member = (GameObject)membersList[i];

                int slotNum = ClosestSlotNumber(member);
                setSlotAvailable(slotNum, false);
                member.GetComponent<NavigateUrbanTown>().assignedSlotNumber = slotNum;
            }

        }

        private int getAvailableSlot()
        {
            if (slot1) return 1;
            if (slot2) return 2;
            if (slot3) return 3;
            if (slot4) return 4;
            if (slot5) return 5;
            return 0; // no slot available
        }

        private bool slotIsAvailable(int slotNumber)
        {
            if (slotNumber == 1) return slot1;
            if (slotNumber == 2) return slot2;
            if (slotNumber == 3) return slot3;
            if (slotNumber == 4) return slot4;
            if (slotNumber == 5) return slot5;
            return false;
        }

        public void setSlotAvailable(int slotNumber, bool available)
        {
            if (slotNumber == 1)
            {
                slot1 = available;
            }
            if (slotNumber == 2)
            {
                slot2 = available;
            }
            if (slotNumber == 3)
            {
                slot3 = available;
            }
            if (slotNumber == 4)
            {
                slot4 = available;
            }
            if (slotNumber == 5)
            {
                slot5 = available;
            }
        }

        public Vector3 getSlotPosition(int slotNumber)
        {
            return gameObject.transform.Find(slotNumber + "").position;
        }

        public Transform getSlotTransform(int slotNumber)
        {
            return gameObject.transform.Find(slotNumber + "");
        }

        public bool AddMember(GameObject pedestrianObject, int slotTarget)
        {
            if (!membersList.Contains(pedestrianObject))
            {
                int slotNum;
                if (slotTarget > 0 && slotTarget < 6) slotNum = slotTarget;
                else slotNum = getAvailableSlot(); // assign slot position

                if (slotNum > 0)
                {
                    setSlotAvailable(slotNum, false);
                    numMembers++;
                    pedestrianObject.GetComponent<NavigateUrbanTown>().assignedGA = this.gameObject;
                    pedestrianObject.GetComponent<NavigateUrbanTown>().assignedSlotNumber = slotNum;
                    pedestrianObject.GetComponent<NavigateUrbanTown>().currentState = CrowdSim.NavigateUrbanTown.NPCStates.MovingWithGroup;

                    membersList.Add(pedestrianObject);
                    UpdateOldestAgeInGroup();
                    return true; // success
                }
            }
            return false;
        }

        public void RemoveMember(GameObject pedestrianObject)
        {
            membersList.Remove(pedestrianObject);
            UpdateOldestAgeInGroup();
            //pedestrianObject.GetComponent<Navigate> ().currentState = CrowdSim.Navigate.NPCStates.Walking; // will go to random destination
        }

        IEnumerator Wait(float duration)
        {
            for (float timer = 0; timer < duration; timer += Time.deltaTime)
                yield return 0;
        }

        /*IEnumerator ModifyGroupMembers(){
	
			while(true){
				for(float timer = 0; timer < 1.0f; timer += Time.deltaTime)
					yield return 0;
				if(currentState.Equals(GAStates.Moving)||currentState.Equals(GAStates.Huddle)||currentState.Equals(GAStates.Waiting)) {
					float waitTime = Random.Range(1.0f,15.0f); // 1 to 30 seconds
					yield return StartCoroutine(Wait(waitTime));
					CheckNearbyAgents();
				}
			}
		}

		private void CheckNearbyAgents()
		{
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.2f, PedestrianSimulator.m_NPCLayerForRayCast);
			int i = 0;
			while (i < hitColliders.Length) {
				//e.g. hitColliders[i].SendMessage("Stop");
				GameObject pedestrianObject = hitColliders[i].transform.gameObject;
				if(pedestrianObject.GetComponent<Navigate>().assignedGA == null
				//&& pedestrianObject.transform.position.y > 204.0f
		 		){ // must not be below road level
					int slot = getAvailableSlot();
					if(slot == 0) return; // stop adding members since no slot is available
					AddMember(pedestrianObject, slot);
				}
				i++;
			}
		}*/
        #endregion

        /*public void ChangeVisibility(float transparency){
			Color newColor = gaSpot.renderer.material.color;
			newColor.a = transparency;
			gaSpot.renderer.material.color = newColor;
			offsetWaypoint.renderer.material.color = newColor;
			groupColor = newColor;
			slot1node.renderer.material.color = newColor;
			slot2node.renderer.material.color = newColor;
			slot3node.renderer.material.color = newColor;
			slot4node.renderer.material.color = newColor;
			slot5node.renderer.material.color = newColor;
		}*/

        void RemoveBreadCrumb()
        {
            Destroy((GameObject)breadcrumbs[0]);
            breadcrumbs.RemoveAt(0);
        }

        private Transform tr;

        // Use this for initialization
        void Start()
        {
            //tr = new GameObject().transform; // dummy transform

            if (string.Equals(transform.gameObject.name, IGNORE_WAYPOINT_AGENT_NAME))
            {
                currentState = GAStates.Disabled;
                return;
            }

            originalSpeed = 1.0f;
            scale = PedestrianSimulator.scale;
            radius = 1.2f;

            float render = 0.0f;
            if (PedestrianSimulator.renderGroupAgent) render = 1.0f;
            groupColor = new Color(Random.value, Random.value, Random.value, render);
            //groupColor = new Color(0.0f,0.0f,0.0f,render);

            // GAspot
            boundaryPointHelper = gameObject.FindInChildren("BoundaryPtHelper");
            /*gaSpot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			gaSpot.transform.localScale = new Vector3(3.0f / scale,0.005f,3.0f / scale); // RVO formation radius
			gaSpot.renderer.material.shader = Shader.Find("Transparent/Diffuse");
			gaSpot.renderer.material.color = groupColor;
			Destroy(gaSpot.collider);
			///////////gaSpot.transform.parent = GameObject.Find("4_GroupAgentDiscs").transform;
			gaSpot.transform.parent = boundaryPointHelper.transform;
			gaSpot.transform.localPosition = new Vector3(0.0f,-0.9f,0.0f);*/

            // boundary point
            boundaryPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            boundaryPoint.transform.localScale = new Vector3(0.1f / scale, 0.1f / scale, 0.1f / scale);
            boundaryPoint.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
            boundaryPoint.GetComponent<Renderer>().material.color = groupColor;
            Destroy(boundaryPoint.GetComponent<Collider>());
            boundaryPoint.transform.parent = GameObject.Find("5_GroupAgentBoundaryPts").transform;

            //offsetWaypoint = gameObject.transform.FindChild("GATarget").gameObject;
            offsetWaypoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(offsetWaypoint.GetComponent<Collider>());
            offsetWaypoint.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
            offsetWaypoint.GetComponent<Renderer>().material.color = groupColor;
            offsetWaypoint.transform.localScale = new Vector3(0.5f / scale, 0.5f / scale, 0.5f / scale);
            offsetWaypoint.transform.parent = GameObject.Find("4_GroupAgentWaypointTargets").transform;

            GameObject sim = GameObject.Find("CrowdSimulator");
            graph = sim.GetComponent<PedestrianSimulator>().GetGlobalPathPlanningGraph();
            destinationList = sim.GetComponent<PedestrianSimulator>().GetDestinationList();

            groupAgentRef = sim.GetComponent<PedestrianSimulator>().GetGroupAgents(); // added 12/2/2014

            // cache transforms and BoxRegionTrigger component
            slot1node = getSlotTransform(1);
            slot2node = getSlotTransform(2);
            slot3node = getSlotTransform(3);
            slot4node = getSlotTransform(4);
            slot5node = getSlotTransform(5);

            //boxRegion = getBoxRegion();
            //boxRegion.GetComponentInChildren<TextMesh>().color = groupColor;

            //remove mesh renderer
            /*foreach(Transform child in transform){
				GameObject.Destroy(child.renderer);
			}*/

            slot1node.GetComponent<Renderer>().material.color = groupColor;
            slot2node.GetComponent<Renderer>().material.color = groupColor;
            slot3node.GetComponent<Renderer>().material.color = groupColor;
            slot4node.GetComponent<Renderer>().material.color = groupColor;
            slot5node.GetComponent<Renderer>().material.color = groupColor;

            tr = (new GameObject()).transform;
            tr.parent = GameObject.Find("8_TempTransforms").transform;

            setGesture(1, GestureStates.None);
            setGesture(2, GestureStates.None);
            setGesture(3, GestureStates.None);
            setGesture(4, GestureStates.None);
            setGesture(5, GestureStates.None);

            //if(PedestrianSimulator.renderGroupAgent) ChangeVisibility(0.0f);
        }

        void WaitForLoadFinish_FixedUpdate()
        {
            currentState = GAStates.Moving;
            if (PedestrianSimulator.usingGroupGMAbasedCA)
            {
                InvokeRepeating("UpdateGMATimeStep", 0, gmaFrequency); // never stop!
            }

            rayR = new Ray(boundaryPointHelper.transform.position, boundaryPointHelper.transform.right);
            rayL = new Ray(boundaryPointHelper.transform.position, -boundaryPointHelper.transform.right);
            rayRF = new Ray(boundaryPointHelper.transform.position, boundaryPointHelper.transform.forward + boundaryPointHelper.transform.right);
            rayLF = new Ray(boundaryPointHelper.transform.position, boundaryPointHelper.transform.forward - boundaryPointHelper.transform.right);

            rayS5 = new Ray(slot5node.position, boundaryPointHelper.transform.forward);
            rayS4 = new Ray(slot4node.position, boundaryPointHelper.transform.forward);
            rayS3 = new Ray(slot3node.position, boundaryPointHelper.transform.forward);
            rayS2 = new Ray(slot2node.position, boundaryPointHelper.transform.forward);
            rayS1 = new Ray(slot1node.position, boundaryPointHelper.transform.forward);

            //////////StartCoroutine("ModifyGroupMembers");
            //InvokeRepeating("AdjustWalkingFormation", 0, 0.1f);
        }

        #region Moving Group Agent and Adjusting Speed

        public void FindStartNode(string start)
        {
            if (currentNode != null) return;
            currentNode = GameObject.Find(start);
        }

        public void FindDestinationNode(string destination)
        {
            destinationNode = GameObject.Find(destination);
        }

        private void AdjustSpeed()
        {
            if (gSpeed < targetSpeed) gSpeed += 0.1f;
            else if (gSpeed > targetSpeed) gSpeed -= 0.1f;
        }

        void Moving_EnterState()
        {
            if (dest != "") FindStartNode(dest);
            do
            {
                int ind = Random.Range(0, destinationList.Count); // 0 to count - 1
                dest = (string)destinationList[ind]; // decide destination from list
                FindDestinationNode(dest);
                destinationOffsetNode = GameObject.Find(dest); // 12/3/2014
            } while (currentNode.name.Equals(destinationNode.name, System.StringComparison.Ordinal)); // do not allow origin to be destination
                                                                                                      //print (currentNode.name + " " + destinationNode.name);
            graph.AStar(currentNode, destinationNode, pathList);
            currentWP = 0;
            destinationReached = false;

            startNodePosition = graph.getPathPoint(currentWP, pathList).transform.position;
            offsetWaypoint.transform.position = startNodePosition;

            caState = Constants.INITIAL_POSITIONING;

            //InvokeRepeating("LessOftenRoutine", 0, 0.02f);
        }

        /*void Moving_Update()
		{
			if(PedestrianSimulator.renderHiddenObjects){
				
				if(nearestGroupAgent != null){ // if there is a nearby agent
					
					switch(caState){
					case Constants.HEAD_ON_COLLISION_AVOIDANCE:
						Debug.DrawLine(transform.position,nearestGroupAgent.position,Color.red);
						//gaSpot.renderer.material.color = Color.red;
						break;
					case Constants.REAR_END_COLLISION_AVOIDANCE:
						Debug.DrawLine(transform.position,nearestGroupAgent.position,Color.black);
						//gaSpot.renderer.material.color = Color.black;
						break;
					case Constants.SIDE_COLLISION_AVOIDANCE:
						Debug.DrawLine(transform.position,nearestGroupAgent.position,Color.grey);
						//gaSpot.renderer.material.color = Color.grey;
						break;
					default:
						Debug.DrawLine(transform.position,nearestGroupAgent.position,Color.blue);
						break;
					}
				}
				else{
					//caState = Constants.NORMAL;
					//targetSpeed = originalSpeed;
					//gaSpot.renderer.material.color = groupColor; // default
				}
				
				Debug.DrawRay(transform.position,transform.forward*1,Color.cyan);
			}
		}*/

        GameObject nextNode = null;
        GameObject nextNextNode = null;
        GameObject prevOffsetWaypoint;

        void UpdateOffSetWaypointTransform()
        {

            if (currentWP == graph.getPathLength(pathList))
            { // if reached end, there is no next node
                return;
            }

            if ((currentWP + 1) == graph.getPathLength(pathList))
            {
                nextNextNode = null;
            }
            else
            {
                nextNextNode = graph.getPathPoint(currentWP + 1, pathList);
            }

            // calculate direction group agent must go
            prevOffsetWaypoint = offsetWaypoint;
            nextNode = graph.getPathPoint(currentWP, pathList);
            Vector3 dir = nextNode.transform.position - currentNode.transform.position;
            offsetWaypoint.transform.position = nextNode.transform.position;
            if (PedestrianSimulator.offsetEnabled)
            {
                offsetWaypoint.transform.rotation = Quaternion.LookRotation(dir, Vector3.up); // orient the offset transform's forward direction
                                                                                              // now calculate an offset on x-axis that is within the width of the passage, random stopping closer or farther than waypoint on local z-axis
                offsetWaypoint.transform.Translate(Random.Range(-5.0f, 5.0f), 0, Random.Range(-3.0f, 3.0f)); // translate relative to local x-axis, also z-axis
            }
        }

        public bool ignore = false;
        public bool waitAtTrafficLight = false;
        Status stat1, stat2;
        Vector3 direction;
        bool prepared = false;

        float startTime = 0.0f;

        void Moving_FixedUpdate()
        {
            // if at end of path
            if (currentWP == graph.getPathLength(pathList))
            {
                currentNode = GameObject.Find(dest);
                offsetWaypoint.transform.position = currentNode.transform.position;

                currentState = GAStates.Huddle;
                caState = Constants.INITIAL_POSITIONING;
                return;
            }

            // node closest to at moment
            currentNode = graph.getPathPoint(currentWP, pathList);

            // if close enough to current waypoint start moving toward the next
            float turnSpeed = 1.5f;

            if (!ignore && Vector3.Distance(offsetWaypoint.transform.position, transform.position) < 1.5f)
            {
                currentWP++;
                nextNode = null; // reset
                UpdateOffSetWaypointTransform(); // update offset transform for diverse paths for group agents
            }

            // if not at end of path, keep on moving
            if (currentWP < graph.getPathLength(pathList))
            {

                if (!ignore)
                {
                    direction = offsetWaypoint.transform.position - boundaryPointHelper.transform.position;
                    direction.y = 0.0f;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), turnSpeed * Time.deltaTime);
                }


                // must change group agent height position here
                if (Physics.Raycast(boundaryPointHelper.transform.position, Vector3.down, out hit, 5.0f, PedestrianSimulator.m_FloorLayerForRayCast.value))
                { // ray cast downward
                    transform.position = new Vector3(transform.position.x, hit.point.y + 0.3f, transform.position.z);
                }

                if (PedestrianSimulator.usingGroupGMAbasedCA)
                { // GMA-based collision avoidance
                    NavigateByGMA();
                }
                else
                { // AMVR case!

                    if (nextNextNode == null)
                    { // path ends, so keep going regardless
                        targetSpeed = originalSpeed; // go back to normal speed if not already
                    }
                    else
                    { // path continues
                        stat1 = nextNode.GetComponent<Status>();
                        stat2 = nextNextNode.GetComponent<Status>();
                        if (stat1 != null && stat2 != null)
                        { // prepare to pass crosswalk
                          //targetSpeed = originalSpeed;
                            Call(GAStates.Waiting); // slow????
                            prepared = true;
                            startTime = Time.time;
                            return;
                        }
                        else if (stat1 != null && stat2 == null)
                        { // crossing crosswalk
                            if (prepared && ((Time.time - startTime) > 5.0f))
                            { // enter once, only if prepared for crosswalk
                                prepared = false;
                                //print ("crosswalk");
                                //if((Time.time - stat1.time_started) > 5.0f){
                                //print (Time.time - stat1.time_started);
                                //print ("Moving fast");
                                targetSpeed = originalSpeed * 3.0f; // run
                            }
                        }
                        else
                        { // path is not through crosswalk, so continue
                            targetSpeed = originalSpeed; // go back to normal speed if not already
                        }
                    }

                    transform.Translate(0, 0, Time.deltaTime * gSpeed / scale); // move straight in current direction
                }

                // change formation shape
                AdjustSpeed();
                AdjustWalkingFormation();
            }

        }

        void Moving_ExitState()
        {
            destinationNode = null;
            destinationReached = true;
            CancelInvoke("LessOftenRoutine");
            caState = Constants.NORMAL;
        }

        public bool isMoving()
        {
            return currentState.Equals(GAStates.Moving);
        }

        public bool ifWaitingAtTrafficLight()
        {
            if (currentState.Equals(GAStates.Waiting))
            {
                return waitAtTrafficLight;
            }
            else return true; // bypass
        }

        #endregion

        void Waiting_EnterState()
        {
            flagWantToGo = false;
            waitAtTrafficLight = false;
        }

        void Waiting_FixedUpdate()
        {
            if (stat1.WaitAtCrossWalk())
            {

                if (Vector3.Distance(prevOffsetWaypoint.transform.position, transform.position) < 0.5f)
                { // if near, stop
                    waitAtTrafficLight = true;
                    // just look across the street
                    direction = nextNextNode.transform.position - boundaryPointHelper.transform.position;
                    direction.y = 0.0f;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), turnSpeed * Time.deltaTime);
                }
                else
                {
                    // override rotation
                    direction = prevOffsetWaypoint.transform.position - boundaryPointHelper.transform.position;
                    direction.y = 0.0f;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), turnSpeed * Time.deltaTime);

                    transform.Translate(0, 0, Time.deltaTime * gSpeed / scale); // move straight in current direction
                }
            }
            else
            { // allowed to pass street, so continue
                flagWantToGo = true;

                if (waitAtTrafficLight && CheckIfReadyToGo())
                {
                    for (int i = 0; i < membersList.Count; i++)
                    {
                        GameObject member = (GameObject)membersList[i];
                        member.GetComponent<NavigateUrbanTown>().MoveWithGroup();
                    }
                    waitAtTrafficLight = false;
                    Return();
                    //return;
                }
                else if (!waitAtTrafficLight)
                {
                    transform.Translate(0, 0, Time.deltaTime * 2.0f * gSpeed / scale); // move straight in current direction
                    Return();
                    //return;
                }
            }

            // change formation shape
            AdjustSpeed();
            AdjustWalkingFormation();
        }

        #region Collision Prediction and Avoidance

        private float getNonAbsoluteAngle(Vector3 v1, Vector3 v2)
        {
            float angle = Vector3.Angle(v1, v2); // absolute value
            Vector3 cross = Vector3.Cross(v1, v2); // cross product
            if (cross.y < 0) angle = -angle;
            return angle;
        }

        // information process space
        Ray seeObject;
        Vector3 directionToTarget;
        private bool CanSeeObject(Transform seer, Transform target, float seeRange, float sightAngle, out float distance)
        {
            directionToTarget = target.position - seer.position;
            float angle = Vector3.Angle(directionToTarget, seer.forward); // angle
            distance = Vector3.Distance(seer.position, target.position);
            if (distance > seeRange || angle > sightAngle) // use simple visible range parameter
            {
                return false;
            }

            // check if static objects in-between
            if (seeObject.Equals(null)) seeObject = new Ray(seer.position, directionToTarget);
            else
            {
                seeObject.direction = directionToTarget;
                seeObject.origin = seer.position;
            }

            seeObject.origin = seeObject.origin + Vector3.up; // move up a little
            if (Physics.Raycast(seeObject, out hit, distance, PedestrianSimulator.m_WallLayerForRayCast.value))
            { // ray cast to other group
                return false;
            }

            return true;
        }

        private void ComputeNearestGroupAgent(float maxCollisionAvoidanceDistance)
        {
            float nearestDistance = Mathf.Infinity;
            nearestGroupAgent = null; // reset

            // remembering nearest one found - REDO using bomb technique since group agents may be disabled (array won't work)
            for (int i = 0; i < groupAgentRef.Length; i++)
            {

                GameObject go = groupAgentRef[i];

                if (go == transform.gameObject) continue;

                //float distanceSqr = (go.transform.position - transform.position).sqrMagnitude;
                float distance = Mathf.Infinity;



                //if(CanSeeObject(this.transform,go.transform,maxCollisionAvoidanceDistance,80.0f,out distance)) // can see other group agent, 90, 135
                if (CanSeeObject(this.boundaryPointHelper.transform, go.GetComponent<UrbanTownGA>().boundaryPointHelper.transform, maxCollisionAvoidanceDistance, 80.0f, out distance)) // can see other group agent, 90, 135
                {

                    if (distance < nearestDistance)
                    { // candidate

                        //go.GetComponent<WaypointAgent>().positions.Count > 0

                        //float dis = Vector3.Distance(previousPosition,go.GetComponent<WaypointAgent>().previousPosition);

                        //if(dis > distance){ // if distances between previous positions greater, likely to collide
                        //print (dis.ToString() + ">" + distance.ToString());
                        nearestGroupAgent = groupAgentRef[i].transform;
                        nearestDistance = distance;
                        //}
                    }
                }

            }
        }

        public Queue<Vector3> positions = new Queue<Vector3>();
        private float gazeMovementAngle1 = 0.0f;
        private float observedGazeMovementAngle1 = 0.0f;
        private int caState = 0;
        private int collisionType = -1;
        private int GMASteps = 5; // 5

        private void UpdateGMATimeStep()
        { // do once per second

            ComputeNearestGroupAgent(Constants.MAX_CONSIDERATION_DISTANCE); // find nearest group agent

            positions.Enqueue(transform.position); // store position in position FIFO queue
            if (positions.Count == GMASteps + 1) positions.Dequeue(); // remove oldest

            if (nearestGroupAgent != null)
            {

                UrbanTownGA nearestGroupAgentWaypointAgent = nearestGroupAgent.gameObject.GetComponent<UrbanTownGA>();

                if (positions.Count == GMASteps && nearestGroupAgentWaypointAgent.positions.Count == GMASteps)
                { // check GMA for specified time steps

                    Vector3[] observingGAPos = positions.ToArray();
                    Vector3[] observedGAPos = nearestGroupAgentWaypointAgent.positions.ToArray();

                    Vector3 initObs1 = observingGAPos[0];
                    Vector3 initObs2 = observedGAPos[0];
                    Vector3 gazeDirection1 = initObs2 - initObs1;
                    gazeMovementAngle1 = getNonAbsoluteAngle(transform.forward, gazeDirection1); // from heading to gaze direction in degrees
                    observedGazeMovementAngle1 = getNonAbsoluteAngle(nearestGroupAgent.forward, initObs1 - initObs2);

                    float newDistance = Vector3.Magnitude(gazeDirection1);

                    for (int i = 1; i < GMASteps; i++)
                    {
                        Vector3 obs1 = observingGAPos[i];
                        Vector3 obs2 = observedGAPos[i];

                        float prevDistance = newDistance; // save previous
                        gazeDirection1 = obs2 - obs1;
                        newDistance = Vector3.Magnitude(gazeDirection1);
                        float threshold = Mathf.Asin(2.0f * radius / newDistance) * 180 / Mathf.PI;
                        float gazeMovementAngle2 = getNonAbsoluteAngle(transform.forward, gazeDirection1);

                        if (((newDistance / prevDistance) < 1.0f) && (Mathf.Abs(Mathf.Abs(gazeMovementAngle2) - Mathf.Abs(gazeMovementAngle1)) < threshold))
                        { // consistent GMA AND decreasing distance?
                            if (i == GMASteps - 1)
                            {
                                return; // there is at least a side collision
                            }
                            continue;
                        }
                        else break;
                    }

                }
                nearestGroupAgent = null; // do not do collision avoidance since GMA not consistent or distance not decreasing
                return;
            }

        }

        private int CollisionType()
        {
            int collisionType;

            //float hisMovingDirection = pedestrians[id].GetComponent<Navigate>().m_mDirection;

            float resultAngle = Vector3.Angle(transform.forward, nearestGroupAgent.forward); // already absolute value

            //float resultAngle = Mathf.Abs(m_mDirection-hisMovingDirection);
            float distance = Vector3.Distance(transform.position, nearestGroupAgent.position);

            if (distance * Mathf.Sin(Mathf.Abs(observedGazeMovementAngle1 * Mathf.PI / 180)) < Constants.INTIMATE_DISTANCE * 2)
            {
                if ((resultAngle < Constants.TAU_FOR_BEING_COLLINEAR) || (resultAngle > 360 - Constants.TAU_FOR_BEING_COLLINEAR))
                {
                    collisionType = Constants.TAIL; // rear-end collision
                }
                else if ((resultAngle > (180 - Constants.TAU_FOR_BEING_COLLINEAR)) && (resultAngle < (180 + Constants.TAU_FOR_BEING_COLLINEAR)))
                {
                    collisionType = Constants.HEAD; // head-on collision
                }
                else
                {
                    collisionType = Constants.SIDE;
                }
            }
            else
            {
                collisionType = Constants.SIDE;
            }
            return collisionType;
        }

        private int collisionDirection = 0;
        private Vector3 caTarget = Vector3.zero;



        private void SteerForTargetPosition()
        {
            // determine group agent direction
            float turnSpeed = 1.5f;
            sideStepping = false; // reset

            if (caState == Constants.HEAD_ON_COLLISION_AVOIDANCE)
            { // shift formation left or right, not turn!
                offsetWaypoint.transform.position = caTarget;

                Vector3 targetDirection = caTarget - transform.position;
                float angle = Vector3.Angle(transform.forward, targetDirection);

                angle = 1.0f; // 50.0f

                if (angle < 10.0f)
                { // side step only, already absolute angle
                    sideStepping = true;
                    if (collisionDirection == Constants.LEFT_COLLISION)
                    { // go right
                        transform.Translate(Time.deltaTime * 0.75f, 0.0f, Time.deltaTime * gSpeed);
                    }
                    else if (collisionDirection == Constants.RIGHT_COLLISION)
                    { // go left
                        transform.Translate(Time.deltaTime * -0.75f, 0.0f, Time.deltaTime * gSpeed);
                    }
                    else
                    { // just stop
                    }
                }
                else
                { // moving direction has same value as target direction (rotate)

                    /*sideStepping = true;
					if(collisionDirection == Constants.LEFT_COLLISION){ // go right
						transform.Translate(Time.deltaTime * 0.75f,0.0f,Time.deltaTime * gSpeed);
					}
					else if(collisionDirection == Constants.RIGHT_COLLISION){ // go left
						transform.Translate(Time.deltaTime * -0.75f,0.0f,Time.deltaTime * gSpeed);
					}
					else{ // just stop
					}*/


                    turnSpeed = 1.0f;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection, Vector3.up), turnSpeed * Time.deltaTime);
                    transform.Translate(0, 0, Time.deltaTime * gSpeed); // move straight in current direction
                }
            }
            else if (caState == Constants.REAR_END_COLLISION_AVOIDANCE/* || caState == Constants.TAG_ALONG*/)
            {

                offsetWaypoint.transform.position = caTarget;

                Vector3 targetDirection = caTarget - transform.position;
                float angle = Vector3.Angle(transform.forward, targetDirection);

                if (angle < 10.0f)
                { // side step, already absolute angle
                    sideStepping = true;

                    if (collisionDirection == Constants.RIGHT_COLLISION)
                    { // go left
                        transform.Translate(Time.deltaTime * -0.5f, 0.0f, Time.deltaTime * gSpeed);
                    }
                    else
                    { // go right
                        transform.Translate(Time.deltaTime * 0.5f, 0.0f, Time.deltaTime * gSpeed);
                    }
                }
                else
                { // moving direction has same value as target direction (rotate)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection, Vector3.up), turnSpeed * Time.deltaTime);
                    transform.Translate(0, 0, Time.deltaTime * gSpeed); // move straight in current direction
                }
            }
            else
            { // steer to actual target

                offsetWaypoint.transform.position = destinationOffsetNode.transform.position;
                Vector3 realDirection = destinationOffsetNode.transform.position - transform.position;
                float angle = getNonAbsoluteAngle(transform.forward, realDirection);

                if (caState != Constants.INITIAL_POSITIONING && Mathf.Abs(angle) < 10.0f)
                { // side step
                    sideStepping = true;

                    if (angle == 0)
                    { // go straight
                        transform.Translate(0.0f, 0.0f, Time.deltaTime * gSpeed);
                    }
                    else if (angle < 0)
                    { // go left
                        transform.Translate(Time.deltaTime * -0.1f, 0.0f, Time.deltaTime * gSpeed);
                    }
                    else
                    { // go right
                        transform.Translate(Time.deltaTime * 0.1f, 0.0f, Time.deltaTime * gSpeed);
                    }
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(realDirection, Vector3.up), turnSpeed * Time.deltaTime);
                    transform.Translate(0, 0, Time.deltaTime * gSpeed); // move straight in current direction

                    if (caState == Constants.INITIAL_POSITIONING)
                    {
                        caState = Constants.NORMAL;
                    }
                }
            }
        }

        private void UpdateHeadOnCollisionTargetPosition()
        {

            if (collisionDirection == Constants.LEFT_COLLISION)
            { // go right
              // -60, -85
              /*Vector3 v = (Quaternion.AngleAxis(-60, transform.up) * oldNearestGroupAgent.forward).normalized; // rotate -60 to get vector
              caTarget = oldNearestGroupAgent.position + v*5.0f;
              caTarget.y = oldNearestGroupAgent.position.y;*/

                caTarget.x = oldNearestGroupAgent.position.x - (Constants.INTIMATE_DISTANCE * Mathf.Cos((oldNearestGroupAgent.rotation.eulerAngles.y - 60.0f) * Mathf.PI / 180.0f));
                caTarget.z = oldNearestGroupAgent.position.z + (Constants.INTIMATE_DISTANCE * Mathf.Sin((oldNearestGroupAgent.rotation.eulerAngles.y + 60.0f) * Mathf.PI / 180.0f));
                caTarget.y = oldNearestGroupAgent.position.y;
            }
            else
            { // go left - works!
              /*Vector3 v = (Quaternion.AngleAxis(60, transform.up) * oldNearestGroupAgent.forward).normalized; // rotate 60 to get vector
              caTarget = oldNearestGroupAgent.position + v*5.0f;
              caTarget.y = oldNearestGroupAgent.position.y;*/

                caTarget.x = oldNearestGroupAgent.position.x + (Constants.INTIMATE_DISTANCE * Mathf.Cos((oldNearestGroupAgent.rotation.eulerAngles.y + 60.0f) * Mathf.PI / 180.0f));
                caTarget.z = oldNearestGroupAgent.position.z + (Constants.INTIMATE_DISTANCE * Mathf.Sin((oldNearestGroupAgent.rotation.eulerAngles.y + 60.0f) * Mathf.PI / 180.0f));
                caTarget.y = oldNearestGroupAgent.position.y;
            }
        }

        private void HandleHeadCollision()
        {

            caState = Constants.HEAD_ON_COLLISION_AVOIDANCE;
            Debug.DrawLine(transform.position, nearestGroupAgent.position, Color.red);
            //gaSpot.renderer.material.color = Color.red;

            if (gazeMovementAngle1 < 0.0f && observedGazeMovementAngle1 < 0.0f)
            { // go right
                collisionDirection = Constants.LEFT_COLLISION;
            }
            else if (gazeMovementAngle1 > 0.0f && observedGazeMovementAngle1 > 0.0f)
            { // go left
                collisionDirection = Constants.RIGHT_COLLISION;
            }
            else
            {
                collisionDirection = Constants.RIGHT_COLLISION; // always go left
            }
            oldNearestGroupAgent = nearestGroupAgent;
            UpdateHeadOnCollisionTargetPosition();
        }

        Transform oldNearestGroupAgent;

        private void UpdateRearEndCollisionTargetPosition()
        {
            if (collisionDirection == Constants.LEFT_COLLISION)
            {
                caTarget.x = oldNearestGroupAgent.position.x + (Constants.INTIMATE_DISTANCE * Mathf.Cos((oldNearestGroupAgent.rotation.eulerAngles.y + 30.0f) * Mathf.PI / 180.0f));
                caTarget.z = oldNearestGroupAgent.position.z + (Constants.INTIMATE_DISTANCE * Mathf.Sin((oldNearestGroupAgent.rotation.eulerAngles.y + 30.0f) * Mathf.PI / 180.0f));
                caTarget.y = oldNearestGroupAgent.position.y;
            }
            else
            {
                caTarget.x = oldNearestGroupAgent.position.x - (Constants.INTIMATE_DISTANCE * Mathf.Cos((oldNearestGroupAgent.rotation.eulerAngles.y + 30.0f) * Mathf.PI / 180.0f));
                caTarget.z = oldNearestGroupAgent.position.z + (Constants.INTIMATE_DISTANCE * Mathf.Sin((oldNearestGroupAgent.rotation.eulerAngles.y + 30.0f) * Mathf.PI / 180.0f));
                caTarget.y = oldNearestGroupAgent.position.y;
            }
        }

        private void HandleRearEndCollision()
        {

            if (gSpeed > nearestGroupAgent.GetComponent<UrbanTownGA>().gSpeed)
            { // overtake if this group agent is faster than the observed
                caState = Constants.REAR_END_COLLISION_AVOIDANCE;
                Debug.DrawLine(transform.position, nearestGroupAgent.position, Color.gray);
                //gaSpot.renderer.material.color = Color.gray;
                targetSpeed = originalSpeed * 1.5f;

                if (gazeMovementAngle1 < 0.0f)
                { // go right
                    collisionDirection = Constants.LEFT_COLLISION;
                }
                else
                { // go left
                    collisionDirection = Constants.RIGHT_COLLISION;
                }
                oldNearestGroupAgent = nearestGroupAgent;
                UpdateRearEndCollisionTargetPosition();
            }
            /*else{ // just tag along from behind
				caState = Constants.TAG_ALONG;
				Debug.DrawLine(transform.position,nearestGroupAgent.position,Color.white);
				//gaSpot.renderer.material.color = Color.white;

				caTarget = nearestGroupAgent.position;
			}*/
        }

        // This function assumes to not handle collision with standing still group
        private void HandleSideCollision()
        {
            float distance = Vector3.Distance(nearestGroupAgent.position, transform.position);

            if (Mathf.Abs(gazeMovementAngle1) > Mathf.Abs(observedGazeMovementAngle1))
            { // pass the other by speeding up
              // speed up depending on distance
                if (distance > 8.0f) targetSpeed = originalSpeed * 1.2f;
                else if (distance > 4.0f) targetSpeed = originalSpeed * 1.4f;
                else targetSpeed = originalSpeed * 1.6f;

                Debug.DrawLine(transform.position, nearestGroupAgent.position, Color.green);
                //gaSpot.renderer.material.color = Color.green;
            }
            else
            { // slow down depending on distance
                if (distance > 8.0f) targetSpeed = originalSpeed * 0.8f;
                else if (distance > 4.0f) targetSpeed = originalSpeed * 0.6f;
                else targetSpeed = originalSpeed * 0.4f;

                Debug.DrawLine(transform.position, nearestGroupAgent.position, Color.magenta);
                //gaSpot.renderer.material.color = Color.magenta;
            }
            caState = Constants.SIDE_COLLISION_AVOIDANCE;
        }

        private void SetupCollisionCases()
        {

            //WaypointAgent other = nearestGroupAgent.GetComponent<WaypointAgent>();

            /*if(!other.isMoving()){ // if other is not moving
				caState = Constants.NORMAL;
				return;
			}*/


            if (caState == Constants.NORMAL) collisionType = CollisionType(); // update collision type
                                                                              //print ("Collision Avoiding: "+ collisionType);

            switch (collisionType)
            {
                case Constants.HEAD:
                    HandleHeadCollision();
                    break;
                case Constants.TAIL:
                    HandleRearEndCollision();
                    break;
                case Constants.SIDE:
                    HandleSideCollision();
                    break;
            }
        }

        private float overtakeStartTime = 0.0f;
        private float headCAStartTime = 0.0f;
        private bool sideStepping = false;

        private void NavigateByGMA()
        {

            // steer for target initial positioning
            if (caState == Constants.INITIAL_POSITIONING)
            {
                caState = Constants.NORMAL;
                transform.rotation = Quaternion.LookRotation(destinationOffsetNode.transform.position - transform.position, Vector3.up);
                ////////transform.rotation = Quaternion.LookRotation(offsetWaypoint.transform.position - transform.position,Vector3.up);
                ReassignSlots();
            }

            // STEP ONE - Calculate GMA to earliest collidee on its own interval coroutine

            // STEP TWO - Setup for collision avoidance based on cases if there is anything to avoid
            if (nearestGroupAgent != null)
            { // there is a collidee, so set up for collision avoidance
                SetupCollisionCases();
            }
            else if (caState == Constants.PASSING_GROUP)
            { // overtaking mode
              //gaSpot.renderer.material.color = Color.black;

                if (timeInCurrentState - overtakeStartTime > 3.0f)
                { // after 3 seconds, back to normal mode
                    caState = Constants.NORMAL;
                    overtakeStartTime = 0.0f; // reset for next time
                }
            }
            else
            { // no collision

                if (caState == Constants.REAR_END_COLLISION_AVOIDANCE)
                {
                    Debug.DrawLine(transform.position, oldNearestGroupAgent.position, Color.gray);
                    UpdateRearEndCollisionTargetPosition();
                }

                if (caState != Constants.REAR_END_COLLISION_AVOIDANCE && caState != Constants.HEAD_ON_COLLISION_AVOIDANCE)
                {
                    caState = Constants.NORMAL;
                    targetSpeed = originalSpeed;
                    //gaSpot.renderer.material.color = groupColor; // default
                }
            }

            // STEP THREE - Steer for target position
            SteerForTargetPosition();

            // STEP FOUR - Short range collision prediction (not based on GMA but distance), might be different group agent

            // STEP FIVE - Short range collision avoidance by urgent stop or speed reduction

            // end of collision avoidance

            // update waypoint position

            // update target for collision avoidance if group agent under REAR/HEAD on collision avoidance

            // invalidate rear end collision avoidance?
            if (caState == Constants.REAR_END_COLLISION_AVOIDANCE)
            {
                float distanceCA = Vector3.Distance(transform.position, caTarget);
                float distanceDest = Vector3.Distance(transform.position, destinationOffsetNode.transform.position);
                float ang = Vector3.Angle(transform.forward, (caTarget - transform.position));
                if (distanceCA < 0.1f)
                { // reached CA target
                    caState = Constants.PASSING_GROUP;
                    overtakeStartTime = timeInCurrentState;
                }
                if (distanceDest < distanceCA || ang > 30.0f)
                { // closer to destination now? or angle not proper
                    caState = Constants.NORMAL;
                }
            }

            // invalidate if arrived at CA Target (go now to destination)
            if (caState == Constants.HEAD_ON_COLLISION_AVOIDANCE)
            {
                //if(nearestGroupAgent != null){
                //////UpdateHeadOnCollisionTargetPosition();
                //}
                float distance = Vector3.Distance(transform.position, caTarget);
                float ang = Vector3.Angle(transform.forward, (caTarget - transform.position));
                if (distance < Constants.INTIMATE_DISTANCE || distance > Constants.MAX_CONSIDERATION_DISTANCE * 0.5f || ang > 30.0f)
                {
                    caState = Constants.NORMAL;
                }
            }
        }
        #endregion

        #region Holding Hands

        private bool HoldHands(GameObject m1, GameObject m2)
        {

            Vector3 holdPosition = (m1.transform.position + m2.transform.position) / 2.0f; // average position

            if (m1.GetComponent<CapsuleCollider>().height < 1.51f || m2.GetComponent<CapsuleCollider>().height < 1.51f) holdPosition.y = m1.transform.position.y + 0.55f / scale; // both are short, so lower
            else holdPosition.y = m1.transform.position.y + 0.7f / scale; // other cases (short person must raise hand higher)

            Vector3 ray = m2.transform.position - m1.transform.position;
            float angle = Vector3.Angle(ray, m1.transform.forward);
            bool oppositeGender = true;
            if (m1.GetComponent<Characteristics>().male == m2.GetComponent<Characteristics>().male) oppositeGender = false;

            if (oppositeGender && Vector3.Distance(m1.transform.position, m2.transform.position) < 0.7f &&
               angle < 100.0f && angle > 80.0f)
            { // if they are near enough and side-by-side, hold hands
              //Vector3 d1 = (holdPosition-m2.transform.position)*0.05f;
              //Vector3 d2 = (holdPosition-m1.transform.position)*0.05f;
              // get location to hold hands
                m1.GetComponent<NavigateUrbanTown>().SetRightHand(holdPosition, true);
                m2.GetComponent<NavigateUrbanTown>().SetLeftHand(holdPosition, true);
                return true; // success

                /*
				m1.GetComponent<Navigate>().anim.SetIKPositionWeight(AvatarIKGoal.RightHand,1.0f);
				m2.GetComponent<Navigate>().anim.SetIKPositionWeight(AvatarIKGoal.LeftHand,1.0f);

				m1.GetComponent<Navigate>().anim.SetIKPosition(AvatarIKGoal.RightHand,holdPosition);
				//m1.GetComponent<Navigate>().anim.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);

				m2.GetComponent<Navigate>().anim.SetIKPosition(AvatarIKGoal.LeftHand,holdPosition);*/
                //m1.GetComponent<Navigate>().anim.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);
            }
            return false; // cannot hold hands
        }

        private GestureStates getGesture(int num)
        {
            switch (num)
            {
                case 1:
                    return S1Gesture;
                case 2:
                    return S2Gesture;
                case 3:
                    return S3Gesture;
                case 4:
                    return S4Gesture;
                case 5:
                    return S5Gesture;
            }
            return GestureStates.None; // dummy
        }

        private void setGesture(int num, GestureStates state)
        {
            switch (num)
            {
                case 1:
                    S1Gesture = state;
                    break;
                case 2:
                    S2Gesture = state;
                    break;
                case 3:
                    S3Gesture = state;
                    break;
                case 4:
                    S4Gesture = state;
                    break;
                case 5:
                    S5Gesture = state;
                    break;
            }
        }

        private void SetUpGestures(int leftSide, int rightside)
        {
            //int allowed = 2; // only allow none or holding hands

            /*if(m1.GetComponent<Characteristics>().male != m2.GetComponent<Characteristics>().male && Mathf.Abs(m1.GetComponent<Characteristics>().age - m2.GetComponent<Characteristics>().age) < 11){
					allowed = 6; // other intimate gestures acceptable if within age range and opposite gender
				}*/

            /*int rand = Random.Range(0,allowed);
			switch(rand){
				case 0: // no gestures
					if(leftSide == 1){ S1Gesture = GestureStates.None; S2Gesture = GestureStates.None; }
					else if(leftSide == 2){ S2Gesture = GestureStates.None; S4Gesture = GestureStates.None; }
					else if(leftSide == 3){ S3Gesture = GestureStates.None; S1Gesture = GestureStates.None; }
					else if(leftSide == 5){ S5Gesture = GestureStates.None; S3Gesture = GestureStates.None; }
					break;
				case 1: // hold hands
					if(leftSide == 1){ S1Gesture = GestureStates.HoldingHand; S2Gesture = GestureStates.HoldingHand; }
					else if(leftSide == 2){ S2Gesture = GestureStates.HoldingHand; S4Gesture = GestureStates.HoldingHand; }
					else if(leftSide == 3){ S3Gesture = GestureStates.HoldingHand; S1Gesture = GestureStates.HoldingHand; }
					else if(leftSide == 5){ S5Gesture = GestureStates.HoldingHand; S3Gesture = GestureStates.HoldingHand; }
					break;
				case 2:*/ // on shoulder
                          /*if(leftSide == 1){ S1Gesture = GestureStates.HandOnShoulder; S2Gesture = GestureStates.None; }
                          else if(leftSide == 2){ S2Gesture = GestureStates.HandOnShoulder; S4Gesture = GestureStates.None; }
                          else if(leftSide == 3){ S3Gesture = GestureStates.None; S1Gesture = GestureStates.HandOnShoulder; }
                          else if(leftSide == 5){ S5Gesture = GestureStates.None; S3Gesture = GestureStates.HandOnShoulder; }*/
                          //break;
                          /*	m1.GetComponent<NavigateUrbanTown>().currentGesture = NavigateUrbanTown.GestureStates.HandOnShoulder;
                              break;
                          case 3:*/ // upper back
                                    //m2.GetComponent<NavigateUrbanTown>().currentGesture = NavigateUrbanTown.GestureStates.HandOnShoulder;
                                    /*if(leftSide == 1){ S1Gesture = GestureStates.HandOnUpperBack; S2Gesture = GestureStates.None; }
                                    else if(leftSide == 2){ S2Gesture = GestureStates.HandOnUpperBack; S4Gesture = GestureStates.None; }
                                    else if(leftSide == 3){ S3Gesture = GestureStates.None; S1Gesture = GestureStates.HandOnUpperBack; }
                                    else if(leftSide == 5){ S5Gesture = GestureStates.None; S3Gesture = GestureStates.HandOnUpperBack; }*/
                                    /*break;
                                case 4:
                                    m1.GetComponent<NavigateUrbanTown>().currentGesture = NavigateUrbanTown.GestureStates.HandOnUpperBack;*/
            if (leftSide == 1) { S1Gesture = GestureStates.HandOnLowerBack; S2Gesture = GestureStates.None; }
            else if (leftSide == 2) { S2Gesture = GestureStates.HandOnLowerBack; S4Gesture = GestureStates.None; }
            else if (leftSide == 3) { S3Gesture = GestureStates.None; S1Gesture = GestureStates.HandOnLowerBack; }
            else if (leftSide == 5) { S5Gesture = GestureStates.None; S3Gesture = GestureStates.HandOnLowerBack; }
            /*
					break;
				case 5:
					m2.GetComponent<NavigateUrbanTown>().currentGesture = NavigateUrbanTown.GestureStates.HandOnUpperBack;
					break;*/
            //}
        }

        private void ApplyIntimateGesture(int leftSide, int rightside)
        {

            // get the two members at positions for left and right sides and their gesture states
            GameObject m1 = null, m2 = null;
            GestureStates leftSideGestureState = GestureStates.None, rightSideGestureState = GestureStates.None;

            for (int i = 0; i < membersList.Count; i++)
            {
                GameObject go = (GameObject)membersList[i];

                if (go.GetComponent<NavigateUrbanTown>().assignedSlotNumber == leftSide)
                {
                    m1 = go;
                    leftSideGestureState = getGesture(leftSide);
                }
                else if (go.GetComponent<NavigateUrbanTown>().assignedSlotNumber == rightside)
                {
                    m2 = go;
                    rightSideGestureState = getGesture(rightside);
                }
            }

            // reset
            m1.GetComponent<NavigateUrbanTown>().SetRightHand(Vector3.zero, false);
            m1.GetComponent<NavigateUrbanTown>().SetLeftHand(Vector3.zero, false);
            m2.GetComponent<NavigateUrbanTown>().SetRightHand(Vector3.zero, false);
            m2.GetComponent<NavigateUrbanTown>().SetLeftHand(Vector3.zero, false);

            // decide what to do based on gesture states
            if ((leftSideGestureState == GestureStates.None) && (rightSideGestureState == GestureStates.None))
            {
                SetUpGestures(leftSide, rightside);
            }

            leftSideGestureState = getGesture(leftSide);
            rightSideGestureState = getGesture(rightside);

            // if holding hands, continue to do so
            if ((leftSideGestureState == GestureStates.HoldingHand) && (rightSideGestureState == GestureStates.HoldingHand))
            {
                if (!HoldHands(m1, m2))
                { // if not successful
                    setGesture(leftSide, GestureStates.None);
                    setGesture(rightside, GestureStates.None);
                }
            }
            // shoulder
            else if (leftSideGestureState == GestureStates.HandOnShoulder)
            {
                if (Vector3.Distance(m1.transform.position, m2.transform.position) < 0.7f &&
                   Vector3.Angle(m2.transform.position - m1.transform.position, m1.transform.forward) < 80.0f)
                { // if they are near enough and other in front
                    Vector3 pos = m2.GetComponent<NavigateUrbanTown>().GetLeftShoulder();
                    //pos.x = pos.x + 0.05f;
                    m1.GetComponent<NavigateUrbanTown>().SetRightHand(pos, true);
                }
            }
            else if (rightSideGestureState == GestureStates.HandOnShoulder)
            {
                if (Vector3.Distance(m1.transform.position, m2.transform.position) < 0.7f &&
                   Vector3.Angle(m1.transform.position - m2.transform.position, m2.transform.forward) < 80.0f)
                { // if they are near enough and other in front
                    Vector3 pos = m1.GetComponent<NavigateUrbanTown>().GetRightShoulder();
                    //pos.x = pos.x - 0.05f;
                    m2.GetComponent<NavigateUrbanTown>().SetLeftHand(pos, true);
                }
            }
            // upper back
            else if (leftSideGestureState == GestureStates.HandOnUpperBack)
            {
                if (Vector3.Distance(m1.transform.position, m2.transform.position) < 0.7f &&
                   Vector3.Angle(m2.transform.position - m1.transform.position, m1.transform.forward) < 80.0f)
                { // if they are near enough and other in front
                    Vector3 pos = m2.GetComponent<NavigateUrbanTown>().GetUpperBack();
                    pos.x = pos.x - 0.05f;
                    m1.GetComponent<NavigateUrbanTown>().SetRightHand(pos, true);
                }
            }
            else if (rightSideGestureState == GestureStates.HandOnUpperBack)
            {
                if (Vector3.Distance(m1.transform.position, m2.transform.position) < 0.7f &&
                   Vector3.Angle(m1.transform.position - m2.transform.position, m2.transform.forward) < 80.0f)
                { // if they are near enough and other in front
                    Vector3 pos = m1.GetComponent<NavigateUrbanTown>().GetUpperBack();
                    pos.x = pos.x - 0.05f;
                    m2.GetComponent<NavigateUrbanTown>().SetLeftHand(pos, true);
                }
            }
            // lower back
            else if (leftSideGestureState == GestureStates.HandOnLowerBack)
            {
                if (Vector3.Distance(m1.transform.position, m2.transform.position) < 0.7f &&
                   Vector3.Angle(m2.transform.position - m1.transform.position, m1.transform.forward) < 80.0f)
                { // if they are near enough and other in front
                    Vector3 pos = m2.GetComponent<NavigateUrbanTown>().GetLowerSpine();
                    pos.z = pos.z - 0.05f;
                    m1.GetComponent<NavigateUrbanTown>().SetRightHand(pos, true);
                }
            }
            else if (rightSideGestureState == GestureStates.HandOnLowerBack)
            {
                if (Vector3.Distance(m1.transform.position, m2.transform.position) < 0.7f &&
                   Vector3.Angle(m1.transform.position - m2.transform.position, m2.transform.forward) < 80.0f)
                { // if they are near enough and other in front
                    Vector3 pos = m1.GetComponent<NavigateUrbanTown>().GetLowerSpine();
                    pos.z = pos.z - 0.05f;
                    m2.GetComponent<NavigateUrbanTown>().SetLeftHand(pos, true);
                }
            }

            //
        }

        private void StopIntimateGesture(int leftSide, int rightSide)
        {

            GameObject m1 = null, m2 = null;

            for (int i = 0; i < membersList.Count; i++)
            {
                GameObject go = (GameObject)membersList[i];

                if (go.GetComponent<NavigateUrbanTown>().assignedSlotNumber == leftSide)
                {
                    m1 = go;
                    setGesture(leftSide, GestureStates.None);

                    m1.GetComponent<NavigateUrbanTown>().SetRightHand(Vector3.zero, false);
                    m1.GetComponent<NavigateUrbanTown>().SetLeftHand(Vector3.zero, false);
                }
                else if (go.GetComponent<NavigateUrbanTown>().assignedSlotNumber == rightSide)
                {
                    m2 = go;
                    setGesture(rightSide, GestureStates.None);

                    m2.GetComponent<NavigateUrbanTown>().SetRightHand(Vector3.zero, false);
                    m2.GetComponent<NavigateUrbanTown>().SetLeftHand(Vector3.zero, false);
                }
            }
        }

        #endregion

        private RaycastHit hit;
        Ray rayR;
        Ray rayL;
        Ray rayRF;
        Ray rayLF;
        Ray rayS5;
        Ray rayS4;
        Ray rayS3;
        Ray rayS2;
        Ray rayS1;

        private int RelativeDirection(GameObject pedestrian)
        {
            int collisionType;

            float resultAngle = Vector3.Angle(transform.forward, pedestrian.transform.forward); // already absolute value

            if ((resultAngle < Constants.TAU_FOR_BEING_COLLINEAR) || (resultAngle > 360 - Constants.TAU_FOR_BEING_COLLINEAR))
            {
                collisionType = Constants.TAIL; // rear-end collision
            }
            else if ((resultAngle > (180 - Constants.TAU_FOR_BEING_COLLINEAR)) && (resultAngle < (180 + Constants.TAU_FOR_BEING_COLLINEAR)))
            {
                collisionType = Constants.HEAD; // head-on collision
            }
            else
            {
                collisionType = Constants.SIDE;
            }

            return collisionType;
        }

        float ForwardCast(ref float tempDistance)
        {
            //if (Physics.Raycast (rayS5,out hit,10.0f, PedestrianSimulator.m_WallLayerForRayCast.value)){ // ray cast to destination
            if (!slotIsAvailable(5) && Physics.Raycast(rayS5, out hit, 10.0f, PedestrianSimulator.raycastLayers.value))
            { // ray cast to destination
                if (hit.collider.gameObject.tag == "Pedestrian" && RelativeDirection(hit.collider.gameObject) == Constants.TAIL)
                {
                    // ignore pedestrian since coming up from behind and pedestrian moving forward
                }
                else
                {
                    if (hit.distance < tempDistance) tempDistance = hit.distance;
                    Debug.DrawLine(slot5node.position, hit.point, groupColor);
                    lockS5S3 = false;
                }
            }
            //if (Physics.Raycast (rayS4,out hit,10.0f, PedestrianSimulator.m_WallLayerForRayCast.value)){ // ray cast to destination
            if (!slotIsAvailable(4) && Physics.Raycast(rayS4, out hit, 10.0f, PedestrianSimulator.raycastLayers.value))
            { // ray cast to destination
                if (hit.collider.gameObject.tag == "Pedestrian" && RelativeDirection(hit.collider.gameObject) == Constants.TAIL)
                {
                    // ignore pedestrian since coming up from behind and pedestrian moving forward
                }
                else
                {
                    if (hit.distance < tempDistance) tempDistance = hit.distance;
                    Debug.DrawLine(slot4node.position, hit.point, groupColor);
                    lockS4S2 = false;
                }
            }
            //if (Physics.Raycast (rayS3,out hit,10.0f, PedestrianSimulator.m_WallLayerForRayCast.value)){ // ray cast to destination
            if (!slotIsAvailable(3) && Physics.Raycast(rayS3, out hit, 10.0f, PedestrianSimulator.raycastLayers.value))
            { // ray cast to destination
                if (hit.collider.gameObject.tag == "Pedestrian" && RelativeDirection(hit.collider.gameObject) == Constants.TAIL)
                {
                    // ignore pedestrian since coming up from behind and pedestrian moving forward
                }
                else
                {
                    if (hit.distance < tempDistance) tempDistance = hit.distance;
                    Debug.DrawLine(slot3node.position, hit.point, groupColor);
                    lockS3S1 = false;
                }
            }
            //if (Physics.Raycast (rayS2,out hit,10.0f, PedestrianSimulator.m_WallLayerForRayCast.value)){ // ray cast to destination
            if (!slotIsAvailable(2) && Physics.Raycast(rayS2, out hit, 10.0f, PedestrianSimulator.raycastLayers.value))
            { // ray cast to destination
                if (hit.collider.gameObject.tag == "Pedestrian" && RelativeDirection(hit.collider.gameObject) == Constants.TAIL)
                {
                    // ignore pedestrian since coming up from behind and pedestrian moving forward
                }
                else
                {
                    if (hit.distance < tempDistance) tempDistance = hit.distance;
                    Debug.DrawLine(slot2node.position, hit.point, groupColor);
                    lockS2S1 = false;
                }
            }
            //if (Physics.Raycast (rayS1,out hit,10.0f, PedestrianSimulator.m_WallLayerForRayCast.value)){ // ray cast to destination
            if (!slotIsAvailable(1) && Physics.Raycast(rayS1, out hit, 10.0f, PedestrianSimulator.raycastLayers.value))
            { // ray cast to destination
                if (hit.collider.gameObject.tag == "Pedestrian" && RelativeDirection(hit.collider.gameObject) == Constants.TAIL)
                {
                    // ignore pedestrian since coming up from behind and pedestrian moving forward
                }
                else
                {
                    if (hit.distance < tempDistance) tempDistance = hit.distance;
                    Debug.DrawLine(slot1node.position, hit.point, groupColor);
                }
            }

            return tempDistance;
        }

        float BoundaryCast(ref float tempDistance)
        {

            //if(!slotIsAvailable(5) && !slotIsAvailable(3)){

            if (Physics.Raycast(rayL, out hit, 5.0f, PedestrianSimulator.m_WallLayerForRayCast.value)
                /*|| Physics.Raycast (rayL,out hit,5.0f, PedestrianSimulator.m_VehicleLayerForRayCast.value)*/)
            { // ray cast to destination
                if (hit.distance < tempDistance) tempDistance = hit.distance;
                Debug.DrawLine(boundaryPointHelper.transform.position, hit.point, groupColor);
                lockS3S1 = false;
                lockS5S3 = false;
            }

            //}

            if (Physics.Raycast(rayLF, out hit, 5.0f, PedestrianSimulator.m_WallLayerForRayCast.value)
                /* || Physics.Raycast (rayLF,out hit,5.0f, PedestrianSimulator.m_VehicleLayerForRayCast.value)*/)
            { // ray cast to destination
                if (hit.distance < tempDistance) tempDistance = hit.distance;
                Debug.DrawLine(boundaryPointHelper.transform.position, hit.point, groupColor);
                lockS3S1 = false;
                lockS5S3 = false;
            }

            //if (!slotIsAvailable (2) && !slotIsAvailable (4)) {
            if (Physics.Raycast(rayR, out hit, 5.0f, PedestrianSimulator.m_WallLayerForRayCast.value)
                /*|| Physics.Raycast (rayR,out hit,5.0f, PedestrianSimulator.m_VehicleLayerForRayCast.value)*/)
            { // ray cast to destination
                if (hit.distance < tempDistance) tempDistance = hit.distance;
                Debug.DrawLine(boundaryPointHelper.transform.position, hit.point, groupColor);
                lockS2S1 = false;
                lockS4S2 = false;
            }

            //}

            if (Physics.Raycast(rayRF, out hit, 5.0f, PedestrianSimulator.m_WallLayerForRayCast.value)
                /*|| Physics.Raycast (rayRF,out hit,5.0f, PedestrianSimulator.m_VehicleLayerForRayCast.value)*/)
            { // ray cast to destination
                if (hit.distance < tempDistance) tempDistance = hit.distance;
                Debug.DrawLine(boundaryPointHelper.transform.position, hit.point, groupColor);
                lockS2S1 = false;
                lockS4S2 = false;
            }

            return tempDistance;
        }

        void AvoidUnwalkableAreas()
        {

            lockS3S1 = lockS2S1 = lockS5S3 = lockS4S2 = false; // default

            // update rays
            rayR.origin = boundaryPointHelper.transform.position;
            rayR.direction = boundaryPointHelper.transform.right;
            rayL.origin = boundaryPointHelper.transform.position;
            rayL.direction = -boundaryPointHelper.transform.right;
            rayRF.origin = boundaryPointHelper.transform.position;
            rayRF.direction = boundaryPointHelper.transform.forward + boundaryPointHelper.transform.right;
            rayLF.origin = boundaryPointHelper.transform.position;
            rayLF.direction = boundaryPointHelper.transform.forward - boundaryPointHelper.transform.right;

            rayS1.direction = rayS2.direction = rayS3.direction = rayS4.direction = rayS5.direction = boundaryPointHelper.transform.forward;
            rayS1.origin = slot1node.position;
            rayS2.origin = slot2node.position;
            rayS3.origin = slot3node.position;
            rayS4.origin = slot4node.position;
            rayS5.origin = slot5node.position;

            float tempDistance = 10.0f;

            // test
            if (!slotIsAvailable(3) && !slotIsAvailable(1))
            {
                lockS3S1 = true;
                ApplyIntimateGesture(3, 1);
            }
            else
            {
                StopIntimateGesture(3, 1);
            }
            if (!slotIsAvailable(5) && !slotIsAvailable(3))
            {
                lockS5S3 = true;
                ApplyIntimateGesture(5, 3);
            }
            else
            {
                StopIntimateGesture(5, 3);
            }
            if (!slotIsAvailable(4) && !slotIsAvailable(2))
            {
                lockS4S2 = true;
                ApplyIntimateGesture(2, 4);
            }
            else
            {
                StopIntimateGesture(2, 4);
            }
            if (!slotIsAvailable(2) && !slotIsAvailable(1))
            {
                lockS2S1 = true;
                ApplyIntimateGesture(1, 2);
            }
            else
            {
                StopIntimateGesture(1, 2);
            }

            comingDistance = ForwardCast(ref tempDistance);
            distanceToBoundary = BoundaryCast(ref tempDistance);

            /*if(!lockS3S1) StopIntimateGesture(3,1);
			if(!lockS5S3) StopIntimateGesture(5,3);
			if(!lockS4S2) StopIntimateGesture(2,4);
			if(!lockS2S1) StopIntimateGesture(1,2);*/

            // check if rays hit vehicles, depending where


            /*Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10.0f);
			int i = 0;
			while (i < hitColliders.Length) {
				hitColliders[i].SendMessage("AddDamage");
				i++;
			}*/
        }

        #region Huddle Formation

        private float startMovingSec = 60.0f;

        void Huddle_EnterState()
        {
            flagWantToGo = false;
            startMovingSec = UnityEngine.Random.Range(5.0f, 10.0f); // change wait time for next destination
            s = 0.0f;

            StopIntimateGesture(3, 1);
            StopIntimateGesture(5, 3);
            StopIntimateGesture(2, 4);
            StopIntimateGesture(1, 2);

            //gaSpot.renderer.material.color = groupColor;
            caState = Constants.INITIAL_POSITIONING;
        }

        private bool CheckIfReadyToGo()
        {
            for (int i = 0; i < membersList.Count; i++)
            {
                GameObject member = (GameObject)membersList[i];
                if (!member.GetComponent<NavigateUrbanTown>().PersonIsReadyToGo()) return false; // at least one not ready
            }
            return true;
        }

        void Huddle_FixedUpdate()
        {
            Huddle_Template();
            if (s < 1.0f) { s += 0.001f; }
            //s += 0.001f;

            if (s >= 0.1)
            {
                huddleFormationComplete = true;
            }
            else
            {
                InterpolateFormation();
                //updateCentroid();
            }

            if (timeInCurrentState > startMovingSec)
            {  // if time in state more than required wait time
                flagWantToGo = true;
            }

            if (flagWantToGo && CheckIfReadyToGo())
            {
                destinationReached = false;

                int ind = Random.Range(0, destinationList.Count); // 0 to count - 1
                dest = (string)destinationList[ind]; // decide destination from list

                /*int j = Random.Range(0,destinationList.Length);*/
                //destinationOffsetNode = destinationList[j];
                currentState = GAStates.Moving; // change to moving mode
            }

        }

        void Huddle_ExitState()
        {
            huddleFormationComplete = false;
            for (int i = 0; i < membersList.Count; i++)
            {
                GameObject member = (GameObject)membersList[i];
                member.GetComponent<NavigateUrbanTown>().MoveWithGroup();
            }
        }

        #endregion

        /*public BoxRegionTrigger getBoxRegion(){
			return gameObject.FindInChildren("BoxRegionTrigger").GetComponent<BoxRegionTrigger>();
		}*/

        #region Formation Changes

        public void updateCentroid()
        {
            centroid = new Vector3(0.0f, 0.0f, 0.0f);
            int num = 0;
            if (pedestrianPosition1 != Vector3.zero) { centroid = centroid + pedestrianPosition1; num++; }
            if (pedestrianPosition2 != Vector3.zero) { centroid = centroid + pedestrianPosition2; num++; }
            if (pedestrianPosition3 != Vector3.zero) { centroid = centroid + pedestrianPosition3; num++; }
            if (pedestrianPosition4 != Vector3.zero) { centroid = centroid + pedestrianPosition4; num++; }
            if (pedestrianPosition5 != Vector3.zero) { centroid = centroid + pedestrianPosition5; num++; }

            //if(num == 0) centroid = transform.position;
            if (num == 0) centroid = boundaryPointHelper.transform.position;
            else centroid = new Vector3(centroid.x / num, centroid.y / num, centroid.z / num);
        }

        public Vector3 getCentroid()
        {
            updateCentroid();
            return centroid;
        }

        public void resetAllPedestrianPosition()
        {
            pedestrianPosition1 = Vector3.zero;
            pedestrianPosition2 = Vector3.zero;
            pedestrianPosition3 = Vector3.zero;
            pedestrianPosition4 = Vector3.zero;
            pedestrianPosition5 = Vector3.zero;
        }

        public void notifyPedestrianPosition(int assignedSlotNumber, Vector3 position)
        {
            if (assignedSlotNumber == 1) pedestrianPosition1 = position;
            else if (assignedSlotNumber == 2) pedestrianPosition2 = position;
            else if (assignedSlotNumber == 3) pedestrianPosition3 = position;
            else if (assignedSlotNumber == 4) pedestrianPosition4 = position;
            else if (assignedSlotNumber == 5) pedestrianPosition5 = position;
        }

        private Vector3 slot1DesiredPosition;
        private Vector3 slot2DesiredPosition;
        private Vector3 slot3DesiredPosition;
        private Vector3 slot4DesiredPosition;
        private Vector3 slot5DesiredPosition;
        private float s = 0.0f;
        private Vector3 referencePoint;

        private void Huddle_Template()
        {
            slot1DesiredPosition = referencePoint + -0.4f / scale * transform.forward + 0.0f / scale * transform.right; // z, x-axis
            slot2DesiredPosition = referencePoint + -0.1f / scale * transform.forward + 0.5f / scale * transform.right; // z, x-axis
            slot3DesiredPosition = referencePoint + -0.1f / scale * transform.forward + -0.5f / scale * transform.right; // z, x-axis
            slot4DesiredPosition = referencePoint + 0.4f / scale * transform.forward + 0.3f / scale * transform.right; // z, x-axis
            slot5DesiredPosition = referencePoint + 0.4f / scale * transform.forward + -0.3f / scale * transform.right; // z, x-axis
        }

        private void River_Template()
        {
            slot1DesiredPosition = referencePoint + -1.2f / scale * transform.forward + 0.0f / scale * transform.right; // z, x-axis
            slot2DesiredPosition = referencePoint + -0.6f / scale * transform.forward + 0.0f / scale * transform.right; // z, x-axis
            slot3DesiredPosition = referencePoint + 0.0f / scale * transform.forward + 0.0f / scale * transform.right; // z, x-axis
            slot4DesiredPosition = referencePoint + 0.6f / scale * transform.forward + 0.0f / scale * transform.right; // z, x-axis
            slot5DesiredPosition = referencePoint + 1.2f / scale * transform.forward + 0.0f / scale * transform.right; // z, x-axis
        }

        private void U_Template()
        {
            slot1DesiredPosition = referencePoint + -0.2f / scale * transform.forward + 0.0f / scale * transform.right; // z, x-axis
            slot2DesiredPosition = referencePoint + 0.0f / scale * transform.forward + 0.5f / scale * transform.right; // z, x-axis
            slot3DesiredPosition = referencePoint + 0.0f / scale * transform.forward + -0.5f / scale * transform.right; // z, x-axis
            slot4DesiredPosition = referencePoint + 0.3f / scale * transform.forward + 0.9f / scale * transform.right; // z, x-axis
            slot5DesiredPosition = referencePoint + 0.3f / scale * transform.forward + -0.9f / scale * transform.right; // z, x-axis
        }

        private void Abreast_Template()
        {
            slot1DesiredPosition = referencePoint + 0.0f / scale * transform.forward + 0.0f / scale * transform.right; // z, x-axis
            slot2DesiredPosition = referencePoint + 0.0f / scale * transform.forward + 0.6f / scale * transform.right; // z, x-axis
            slot3DesiredPosition = referencePoint + 0.0f / scale * transform.forward + -0.6f / scale * transform.right; // z, x-axis
            slot4DesiredPosition = referencePoint + 0.0f / scale * transform.forward + 1.2f / scale * transform.right; // z, x-axis
            slot5DesiredPosition = referencePoint + 0.0f / scale * transform.forward + -1.2f / scale * transform.right; // z, x-axis
        }

        public void InterpolateFormation()
        {
            slot1node.transform.position = ((1.0f - s) * slot1node.transform.position) + (s * slot1DesiredPosition);
            slot2node.transform.position = ((1.0f - s) * slot2node.transform.position) + (s * slot2DesiredPosition);
            slot3node.transform.position = ((1.0f - s) * slot3node.transform.position) + (s * slot3DesiredPosition);
            slot4node.transform.position = ((1.0f - s) * slot4node.transform.position) + (s * slot4DesiredPosition);
            slot5node.transform.position = ((1.0f - s) * slot5node.transform.position) + (s * slot5DesiredPosition);
        }

        private bool lockS3S1, lockS2S1, lockS5S3, lockS4S2;

        // inner couple
        private void S3S1()
        {
            tr.position = slot1DesiredPosition;
            //tr.position = (slot3DesiredPosition + slot1DesiredPosition)/2.0f;
            //tr.position = (slot3node.transform.position + slot1node.transform.position)/2.0f;
            tr.rotation = boundaryPointHelper.transform.rotation;

            tr.Translate(-0.6f / scale, 0.0f, 0.0f);
            //slot3node.transform.position = tr.position;
            slot3DesiredPosition = tr.position;
            //tr.Translate(0.6f / scale,0.0f,0.0f);
            //slot1node.transform.position = tr.position;
            //slot1DesiredPosition = tr.position;
        }

        // inner couple
        private void S2S1()
        {
            tr.position = slot1DesiredPosition;
            //tr.position = (slot2DesiredPosition + slot1DesiredPosition)/2.0f;
            //tr.position = (slot2node.transform.position + slot1node.transform.position)/2.0f;
            tr.rotation = boundaryPointHelper.transform.rotation;

            tr.Translate(0.6f / scale, 0.0f, 0.0f);
            //slot2node.transform.position = tr.position;
            slot2DesiredPosition = tr.position;
            //tr.Translate(-0.6f / scale,0.0f,0.0f);
            //slot1node.transform.position = tr.position;
            //slot1DesiredPosition = tr.position;
        }

        private void S5S3()
        {
            tr.position = (slot5DesiredPosition + slot3DesiredPosition) / 2.0f;
            //tr.position = (slot5node.transform.position + slot3node.transform.position)/2.0f;
            tr.rotation = boundaryPointHelper.transform.rotation;

            tr.Translate(-0.3f / scale, 0.0f, 0.0f);
            //slot5node.transform.position = tr.position;
            slot5DesiredPosition = tr.position;
            tr.Translate(0.6f / scale, 0.0f, 0.0f);
            //slot3node.transform.position = tr.position;
            slot3DesiredPosition = tr.position;
        }

        private void S4S2()
        {
            tr.position = (slot4DesiredPosition + slot2DesiredPosition) / 2.0f;
            //tr.position = (slot4node.transform.position + slot2node.transform.position)/2.0f;
            tr.rotation = boundaryPointHelper.transform.rotation;

            tr.Translate(0.3f / scale, 0.0f, 0.0f);
            //slot4node.transform.position = tr.position;
            slot4DesiredPosition = tr.position;
            tr.Translate(-0.6f / scale, 0.0f, 0.0f);
            //slot2node.transform.position = tr.position;
            slot2DesiredPosition = tr.position;
        }

        //public void AdjustWalkingFormation(){ // called in FSM for moving, env space availability
        private void AdjustWalkingFormation()
        {

            // reassign to closest slots
            if (!sideStepping) ReassignSlots();
            AvoidUnwalkableAreas(); ///////slow

            if (s < 1.0f) { s += 0.0001f; } //0.0001f

            if (distanceToBoundary < 1.0f || comingDistance < 5.0f)
            { // contract to river
                if (formationState != 2) s = 0.0f;
                formationState = 2;
            }
            else if (distanceToBoundary < 2.0f || comingDistance < 10.0f)
            { // contract to wide U
                if (formationState != 1) s = 0.0f;
                formationState = 1;
            }
            else
            { // expand to abreast
                if (formationState != 0) s = 0.0f;
                formationState = 0;
            }

            // update reference point
            referencePoint = transform.position;
            //if(formationState != 2) referencePoint = ((1.0f-s) * transform.position) + (s * transform.position);
            //else referencePoint = ((1.0f-s) * transform.position) + (s * transform.position);

            switch (formationState)
            {
                case 0:
                    Abreast_Template();
                    break;
                case 1:
                    U_Template();
                    break;
                case 2:
                    River_Template();
                    break;
            }

            // Determine relationship between two slots

            // inner
            if (lockS3S1) S3S1();
            if (lockS2S1) S2S1();

            // outer
            if (lockS5S3) S5S3();
            if (lockS4S2) S4S2();

            InterpolateFormation();
        }

        #endregion

    }
}
