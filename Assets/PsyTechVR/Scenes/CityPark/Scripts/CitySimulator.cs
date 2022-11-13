using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Wayfinding;
using MonsterLove.StateMachine;
using System;
using System.Linq;

namespace CrowdSim
{
	public class CitySimulator: StateBehaviour{

		public enum SimulatorState{
			Bootup,
			Run,
			Exit
		}
		
		#region Simulation Settings
		public int numPedestrians;		// default is 100 total
		public string[] SpawnTags;
		public bool groups = true;		// Individuals vs Groups
		public static bool sightBeamsOn = false;	// view sight beams
		public static bool renderGroupAgent = false; // see group agent or not, pedestrian disks or not, framerate or not
		public static bool cornerTurn = false;		// turn on turning
		public bool fillGroup = false;		// make all groups 5 members
		public static bool usingGroupGMAbasedCA = false;  // use GMA-based collision avoidance
		public static bool useNavmeshAgentNavigation = true; // if true, use navmesh agent path finding and avoidance (to slot of group agent)
		public static bool offsetEnabled = false; // allow offset placement from current node in path (random nearby location at each waypoint)
		public static float avatarScale = 0.65f;			// handle scaling of 3D avatar models
		public static float scale = 1.5f; // scale divisor
		
		#endregion
		
		#region Change Mode Settings
		public static bool breadCrumbs = false;		// show trail of pedestrians
		public static bool renderHiddenObjects = false;	// show hidden objects or not (e.g. group agent, discs)
		#endregion
		
		#region Simulation Objects

		// unique characters
		public GameObject[] Boys;
		public GameObject[] Girls;
		public GameObject[] Men;
		public GameObject[] Women;
		public GameObject[] OldMen;
		public GameObject[] OldWomen;
        private List<GameObject> peopleList; // all the above in one list

        // other
        public GameObject groupAgent;
		//public GameObject fpController;
		//public GameObject riftController;
		#endregion

		private Rewired.Player input;
		
		// frames per second vars
		private float updateInterval = 0.5f;
		//private float accum = 0.0f; // FPS accumulated over the interval
		//private int frames = 0;     // Frames drawn over the interval
		private float timeleft;     // Left time for current interval
		
		// other variables
		public GUIStyle hUDStyle;
		public int numActivePedestrians;
		public static int numActiveGroupAgents;
		private GameObject focusPedestrian = null; // camera focus
		private static List<GameObject> mSpawnPoints;
		//private static List<GameObject> rendezvousPoints;
		
		public Graph globalPathPlanningGraph = new Graph(); // waypoint graph for group agents
		public GameObject[] globalPathPlanningWaypoints; // used to get initial locations of group agents
		public ArrayList destinationList = new ArrayList(); // possible destinations for group agents
		public GameObject[] pedestrians;
		public GameObject[] groupAgents; // should bomb nearby pedestrians to find nearby group agents
		public static bool pedestriansLoaded = false;
		
		public static GameObject groupAgentObject;

		// bus lines FIFO
		public Queue<GameObject> busLine3;
		
		private int numGroup5 = 0;
		private int numGroup4 = 0;
		private int numGroup3 = 0;
		private int numGroup2 = 0;

		public static LayerMask m_WallLayerForRayCast;
		public static LayerMask m_FloorLayerForRayCast;
		public static LayerMask m_VehicleLayerForRayCast;
		public static LayerMask m_NPCLayerForRayCast;
		public static LayerMask m_NPC_Cars_Collision_LayerForOverLapSphere;
		public static LayerMask raycastLayers;

		private static List<GameObject> mAllWaypoints;
		private GameObject[] mWarpWaypoints;

		private Camera[] cameras;
		private bool carsEnabled = true;
		private bool npcEnabled = true;

		void OnEnable()
		{
			numActivePedestrians = 0;
		}

		void OnDisable() // called when destroyed
		{
			StopAllCoroutines();
		}

		public GameObject[] GetGroupAgents()
		{
			return groupAgents;
		}

		public ArrayList GetDestinationList()
		{
			return destinationList;
		}

		public Graph GetGlobalPathPlanningGraph()
		{
			return globalPathPlanningGraph;
		}

        public static T[] CreateCombinedArrayFrom<T>(T[] first, T[] second, T[] third, T[] fourth, T[] fifth, T[] sixth)
        {
            T[] result = new T[first.Length + second.Length + third.Length + fourth.Length + fifth.Length + sixth.Length];
            int index = 0;
            Array.Copy(first, 0, result, index, first.Length);
            index += first.Length;
            Array.Copy(second, 0, result, index, second.Length);
            index += second.Length;
            Array.Copy(third, 0, result, index, third.Length);
            index += third.Length;
            Array.Copy(fourth, 0, result, index, fourth.Length);
            index += fourth.Length;
            Array.Copy(fifth, 0, result, index, fifth.Length);
            index += fifth.Length;
            Array.Copy(sixth, 0, result, index, sixth.Length);
            return result;
        }

        void Awake()
		{
			input = Rewired.ReInput.players.GetPlayer(0);
			Initialize<SimulatorState>();
		}
		
		// Use this for initialization
		void Start () {

			m_WallLayerForRayCast = 1 << LayerMask.NameToLayer("Collidable");
			m_FloorLayerForRayCast = 1 << LayerMask.NameToLayer("Floor");
			m_VehicleLayerForRayCast = 1 << LayerMask.NameToLayer("NPC_Cars");
			m_NPCLayerForRayCast = 1 << LayerMask.NameToLayer("NPC_People");
			m_NPC_Cars_Collision_LayerForOverLapSphere = 1 << LayerMask.NameToLayer("NPC_Cars_Collision");

			raycastLayers = m_NPCLayerForRayCast | m_WallLayerForRayCast.value;

			mSpawnPoints = new List<GameObject>( GameObject.FindGameObjectsWithTag(SpawnTags[0]) );
			for(int i = 1 ; i < SpawnTags.Length; i++)
				mSpawnPoints.AddRange(GameObject.FindGameObjectsWithTag(SpawnTags[i]));

			// old
			mAllWaypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("WaypointsVereda")); // TODO: Create a static class to hold tag strings
			mWarpWaypoints = GameObject.FindGameObjectsWithTag("WaypointsVeredaSpawn");
			mAllWaypoints.AddRange( mWarpWaypoints );
			
			timeleft = updateInterval;

			cameras = Camera.allCameras; // only player cameras allowed! no other cameras should be in scene

            ChangeState(SimulatorState.Bootup);
        }

		float timeDelay = 0.0f;
		private int remainder;
		
		void Bootup_Enter()
		{
			print ("Loading waypoint graph");
			CreateWaypointGraph();   // create waypoint graph for global navigation by group agents
			CreateDestinationList(); // based on graph, choose possible destination points for group agents

			CreateBusLines();
            //StartCoroutine("SpawnGroupAgents");

            GameObject[] people = CreateCombinedArrayFrom(Boys, Girls, Men, Women, OldMen, OldWomen);
            peopleList = people.ToList<GameObject>();

            // determine group agent instance appearance
            groupAgentObject = Instantiate(groupAgent,new Vector3(22.0f,0.1f,10.0f),Quaternion.identity) as GameObject;
			
			if(!renderGroupAgent){
				Renderer[] renderers = groupAgentObject.GetComponentsInChildren<Renderer>();
				
				foreach(Renderer r in renderers){
					r.enabled = false;
				}
				
				//renderers = GameObject.FindGameObjectWithTag("Player").FindInChildren("GroupAgent").GetComponentsInChildren<Renderer>();
				foreach(Renderer r in renderers){
					r.enabled = false;
				}
			}

            // determine num active group agents
            //int totalPed = numPedestrians;
            numPedestrians = peopleList.Count;
            int half = numPedestrians / 2;
			remainder = numPedestrians - half;

			numActiveGroupAgents = 0;
			numGroup5 = Mathf.RoundToInt((remainder)*0.01f);
			numGroup4 = Mathf.RoundToInt((remainder)*0.02f);
			numGroup3 = Mathf.RoundToInt((remainder)*0.06f);
			numGroup2 = Mathf.RoundToInt((remainder)*0.28f);

			print("Loading group agents with people");
			SpawnGroupsOfPeople();

			//currentState = SimulatorState.Run;

			print("Loading single pedestrians");
			timeDelay = 0.0f; // 0 seconds
		}

		public void SpawnGroupsOfPeople()
		{
			int numWaypoints = globalPathPlanningWaypoints.Length;
			
			for(int i = 0; i < remainder; i++){

				// spawn according to realistic group size distribution
				int numifs = NumMembers(); // Random.Range(1,6); // 1 to 5 passengers
				if(numifs == 1) return; // all 1-person groups are not part of group!
				//if(CrowdSimulation.groups) numifs = Random.Range(0,3)+Random.Range(1,4); // 1 to 5 passengers
				//else numifs = 1; // 1 per car
				
				// center, originally facing positive Z
				Vector3 pos = new Vector3(0.0f,0.1f,0.0f); // random position above floor
				Quaternion rotQ = Quaternion.identity;
				string closest = "";
				
				int randomWaypointIndex = UnityEngine.Random.Range(0,numWaypoints);
				GameObject go = (GameObject) globalPathPlanningWaypoints.GetValue(randomWaypointIndex);
				pos = go.transform.position; // go.transform.position
				closest = go.name;
				
				GameObject obj;
				obj = getNewGroupAgentObject(pos,rotQ);
				obj.transform.tag = "GA";
				obj.AddComponent<GroupAgent>(); // add WaypointAgent component!
				obj.GetComponent<GroupAgent>().FindStartNode(closest);
				obj.transform.parent = GameObject.Find("3_GroupAgents").transform;
				
				if(numifs == 5){
					InitializePedestrianFollowTarget(obj,5);
					//numActivePedestrians++;
					i++;
					if(i < numPedestrians) numifs--;
				}
				if(numifs == 4){
					InitializePedestrianFollowTarget(obj,4);
					//numActivePedestrians++;
					i++;
					if(i < numPedestrians) numifs--;
				}
				if(numifs == 3){
					InitializePedestrianFollowTarget(obj,3);
					//numActivePedestrians++;
					i++;
					if(i < numPedestrians) numifs--;
				}
				if(numifs == 2){
					InitializePedestrianFollowTarget(obj,2);
					//numActivePedestrians++;
					i++;
					if(i < numPedestrians) numifs--;
				}
				if(numifs == 1){
					InitializePedestrianFollowTarget(obj,1);
					//numActivePedestrians++;
				}
			}
		}

		void Bootup_Update(){

			if (getTimeEnteredState() > timeDelay) { // delay

				timeDelay += 0.25f; // delay 1/4 second for next spawn

				//for (int i = 0; i < numPedestrians; i++) {
				int index = UnityEngine.Random.Range(0, mSpawnPoints.Count);
				Vector3 startPos = mSpawnPoints [index].transform.position;
				GameObject newPerson = CreatePedestrian (startPos); // create new pedestrian

				ObjectRef oref = mSpawnPoints [index].GetComponent<ObjectRef> ();
				if (oref != null) {
					// change initial state to getting out of subway
					newPerson.SendMessage ("SubwayInOut", oref.Ref, SendMessageOptions.DontRequireReceiver);
					//print ("Come out of subway");
				}

				//newPerson.GetComponent<Navigate>().SetStartPoint(mSpawnPoints[index]);
				//newPerson.SendMessage("SetStartPoint", mSpawnPoints[index],SendMessageOptions.DontRequireReceiver );
				//numActivePedestrians++;
				//}

				if(numActivePedestrians > numPedestrians-1){
					//currentState = SimulatorState.Run;
					ChangeState(SimulatorState.Run);
				}

			}

		}

		void Bootup_Exit()
		{
			///////SpawnGroupAgents();
		}
		
		void Run_Enter()
		{
			if(numPedestrians > 0) pedestrians = GameObject.FindGameObjectsWithTag("Pedestrian"); // save all pedestrians in array for later reference
			GameObject.Destroy(GameObject.Find(GroupAgent.IGNORE_WAYPOINT_AGENT_NAME));
			groupAgents = GameObject.FindGameObjectsWithTag("GA"); // save all group agents in array for later reference

			print("Total Group Agents: " + numActiveGroupAgents);
			print("Total Pedestrians: " + numActivePedestrians);
			pedestriansLoaded = true;
		}
		
		void Run_Update() // update simulation
		{
			// controls
			/*if(Input.GetKeyUp(KeyCode.B)){ // toggle bread crumbs view
				if(breadCrumbs == false){
					breadCrumbs = true;
					print ("Breadcrumbs Turned On");
				}
				else{
					breadCrumbs = false;
					print ("Breadcrumbs Turned Off");
				}
			}*/
			/*if(Input.GetKeyUp(KeyCode.R)){ // toggle render hidden objects view
				if(renderHiddenObjects == false){
					renderHiddenObjects = true;
					print ("Hidden Objects Turned On");
					for(int i = 0; i < groupAgents.Length; i++){
						groupAgents[i].GetComponent<GroupAgent>().ChangeVisibility(1.0f); // make appear
					}
				}
				else{
					renderHiddenObjects = false;
					print ("Hidden Objects Turned Off");
					for(int i = 0; i < groupAgents.Length; i++){
						groupAgents[i].GetComponent<GroupAgent>().ChangeVisibility(0.0f); // make transparent
					}
				}
			}*/

			if( input.GetButtonDown("DPadLeft"))
			{
				carsEnabled = !carsEnabled;
				if(carsEnabled)
				{
					Physics.IgnoreLayerCollision(15,12, false);
					Physics.IgnoreLayerCollision(10,12, false);
				}
				else
				{
					Physics.IgnoreLayerCollision(15,12, true);
					Physics.IgnoreLayerCollision(10,12, true);
				}
				updateMaskstate();
			}
			if( input.GetButtonDown("DPadRight"))
			{
				npcEnabled = !npcEnabled;
				if(npcEnabled)
				{
					Physics.IgnoreLayerCollision(11,12, false);
				}
				else
				{
					Physics.IgnoreLayerCollision(11,12, true);
				}
				updateMaskstate();
			}
			
			// display hidden stuff or not
			if(renderHiddenObjects){
				globalPathPlanningGraph.debugDraw(); // draw the paths in the scene view of the editor while playing
			}
		}

		void updateMaskstate()
		{
			int renderMask = 0;
			if( !carsEnabled )
			{
				renderMask |= 1<<10;
			}
			if( !npcEnabled )
			{
				renderMask |= 1<<11;
			}
			foreach( Camera cam in cameras)
			{
				cam.cullingMask = ~renderMask;
			}
		}
		
		public static GameObject getNewGroupAgentObject(Vector3 pos, Quaternion rotQ){
			numActiveGroupAgents++;
			return Instantiate(groupAgentObject,pos,rotQ) as GameObject;
		}
		
		private void InitializeFollowTargetForPedestrian(GameObject obj, int slotTarget, GameObject person){
			Vector3 pos = obj.GetComponent<GroupAgent>().getSlotPosition(slotTarget);
			person.transform.position = pos;
			
			
			//person.transform.GetComponent<Navigate>().SetTarget(obj.GetComponent<GroupAgent>().getSlotTransform(slotTarget));
			//obj.GetComponent<GroupAgent>().setSlotNotAvailable(slotTarget);
			//person.GetComponent<Navigate>().assignedGA = obj;
			//person.GetComponent<Navigate>().assignedSlotNumber = slotTarget;
			
			obj.GetComponent<GroupAgent>().AddMember(person,slotTarget);
		}
		
		private void InitializePedestrianFollowTarget(GameObject obj, int slotTarget){
			Vector3 pos = obj.GetComponent<GroupAgent>().getSlotPosition(slotTarget);
			GameObject person = CreatePedestrian(pos); // create new pedestrian

			if (person == null){
				numActivePedestrians--;
				return;
			}

			//person.GetComponent<Animator> ().applyRootMotion = true;

			//person.transform.GetComponent<Navigate>().SetTarget(obj.GetComponent<GroupAgent>().getSlotTransform(slotTarget));
			//obj.GetComponent<GroupAgent>().setSlotNotAvailable(slotTarget);
			//person.GetComponent<Navigate>().assignedGA = obj;
			//person.GetComponent<Navigate>().assignedSlotNumber = slotTarget;
			
			obj.GetComponent<GroupAgent>().AddMember(person,slotTarget);
		}
		
		private int NumMembers(){
			if(fillGroup) return 5;
			if(numGroup5 > 0){ numGroup5--; return 5;}
			if(numGroup4 > 0){ numGroup4--; return 4;}
			if(numGroup3 > 0){ numGroup3--; return 3;}
			if(numGroup2 > 0){ numGroup2--; return 2;}
			return 1;
		}

		/*public static Vector3 getRendezvousPoint()
		{
			int index = Random.Range(0, rendezvousPoints.Count);
			return rendezvousPoints[index].transform.position;
		}*/

		public static Vector3 getSpawnPoint()
		{
			int index = UnityEngine.Random.Range(0, mSpawnPoints.Count);
			return mSpawnPoints[index].transform.position;
		}

		public static GameObject getWayPointObject()
		{
			int index = UnityEngine.Random.Range(0, mAllWaypoints.Count);
			return mAllWaypoints[index];
		}

		/*public void SpawnPeople()
		{
			int index = 0;
			Vector3 startPos;
			for(int i = 0; i < numPedestrians; i++) {
				index = Random.Range(0, mSpawnPoints.Count);
				startPos = mSpawnPoints[index].transform.position;
				GameObject newPerson = CreatePedestrian(startPos); // create new pedestrian

				ObjectRef oref = mSpawnPoints[index].GetComponent<ObjectRef>();
				if(oref != null)
				{
					// change initial state to getting out of subway
					newPerson.SendMessage("SubwayInOut", oref.Ref, SendMessageOptions.DontRequireReceiver );
					//print ("Come out of subway");
				}

				//newPerson.GetComponent<Navigate>().SetStartPoint(mSpawnPoints[index]);
				//newPerson.SendMessage("SetStartPoint", mSpawnPoints[index],SendMessageOptions.DontRequireReceiver );
				numActivePedestrians++;
			}
		}*/

		/*IEnumerator SpawnGroupAgents()
		{
			int count = groupAgent.CountPooled();
			for(int i = 0; i < count; i++)
			{
				SpawnGroupAgent();
				yield return new WaitForSeconds(1);
			}
			StopCoroutine("SpawnGroupAgents");
			yield return 0;
		}*/

		/*public void SpawnGroupAgent()
		{
			// center, originally facing positive Z
			Vector3 pos = new Vector3(0.0f,0.1f,0.0f); // random position above floor
			Quaternion rotQ = Quaternion.identity;
			string closest = "";
			
			bool doit = true;
			
			int ind = Random.Range(0,destinationList.Count); // 0 to count - 1
			string dest = (string) destinationList[ind]; // decide destination from list
			GameObject go = GameObject.Find(dest);
			pos = go.transform.position; // go.transform.position
			closest = go.name;

			GameObject obj;
			obj = groupAgent.Spawn(pos, rotQ);
			obj.transform.tag = "GA";
			obj.AddComponent<GroupAgent>(); // add GroupAgent component!
			obj.GetComponent<GroupAgent>().FindStartNode(closest);
			obj.transform.parent = GameObject.Find("3_GroupAgents").transform;

			if(!renderGroupAgent){
				Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
				
				foreach(Renderer r in renderers){
					r.enabled = false;
				}
				
				//renderers = GameObject.FindGameObjectWithTag("Player").FindInChildren("GroupAgent").GetComponentsInChildren<Renderer>();
				foreach(Renderer r in renderers){
					r.enabled = false;
				}
			}
		}*/
		
		/*public void SpawnGroupAgents()
		{
			int numWaypoints = globalPathPlanningWaypoints.Length;
			int numGroupAgents = (int) Mathf.Floor(numPedestrians/2.0f);

			for(int i = 0; i < numGroupAgents; i++){
				
				// center, originally facing positive Z
				Vector3 pos = new Vector3(0.0f,0.1f,0.0f); // random position above floor
				Quaternion rotQ = Quaternion.identity;
				string closest = "";
				
				bool doit = true;

				int ind = Random.Range(0,destinationList.Count); // 0 to count - 1
				string dest = (string) destinationList[ind]; // decide destination from list
				GameObject go = GameObject.Find(dest);
				pos = go.transform.position; // go.transform.position
				closest = go.name;
				
				// hacked
				//pos = new Vector3(-154.2886f,0.3f,-125.095f);
				//closest = "e5";
				
				GameObject obj;
				obj = getNewGroupAgentObject(pos,rotQ);
				obj.transform.tag = "GA";
				obj.AddComponent<GroupAgent>(); // add GroupAgent component!
				obj.GetComponent<GroupAgent>().FindStartNode(closest);
				obj.transform.parent = GameObject.Find("3_GroupAgents").transform;
			}
		}*/

		private int boysOnce = 0;
		private int girlsOnce = 0;
		private int menOnce = 0;
		private int womenOnce = 0;
		private int oldMenOnce = 0;
		private int oldWomenOnce = 0;
		private UnityEngine.AI.NavMeshHit closestHit;
		
		private GameObject CreatePedestrian(Vector3 pos){
			
			// GameObject clone; use only if need
			if(UnityEngine.AI.NavMesh.SamplePosition(pos, out closestHit, 1, 1 )){ // make sure on navigation mesh!
				pos = closestHit.position;
			}
			
			// originally facing positive Z
			Quaternion rotQ = Quaternion.identity;
			GameObject newPerson = null;
			
			NavigateCity navC = null;

            // get a unique person
            int index = UnityEngine.Random.Range(0, peopleList.Count);
            GameObject person = peopleList.ElementAt(index);
            peopleList.RemoveAt(index);

            newPerson = GameObject.Instantiate(person, pos, rotQ) as GameObject;

            /*if (menOnce < Men.Length) {
				newPerson = GameObject.Instantiate(Men[menOnce], pos, rotQ) as GameObject;
				menOnce++;
			}
            else if (womenOnce < Women.Length) {
				newPerson = GameObject.Instantiate(Women[womenOnce], pos, rotQ) as GameObject;
				womenOnce++;
			}
            else if (oldMenOnce < OldMen.Length) {
				newPerson = GameObject.Instantiate(OldMen[oldMenOnce], pos, rotQ) as GameObject;
				oldMenOnce++;
			}
            else if (oldWomenOnce < OldWomen.Length) {
				newPerson = GameObject.Instantiate(OldWomen[oldWomenOnce], pos, rotQ) as GameObject;
				oldWomenOnce++;
			}
            else if (boysOnce < Boys.Length) {
				newPerson = GameObject.Instantiate(Boys[boysOnce], pos, rotQ) as GameObject;
				boysOnce++;
			}
            else if (girlsOnce < Girls.Length) {
				newPerson = GameObject.Instantiate(Girls[girlsOnce], pos, rotQ) as GameObject;
				girlsOnce++;
            }
            else{
                int personType = Random.Range(0, 6);
                switch (personType)
                {
                    case 0:
                        newPerson = GameObject.Instantiate(Men[Random.Range(0, Men.Length)], pos, rotQ) as GameObject;
                        break;
                    case 1:
                        newPerson = GameObject.Instantiate(Women[Random.Range(0, Women.Length)], pos, rotQ) as GameObject;
                        break;
                    case 2:
                        newPerson = GameObject.Instantiate(OldMen[Random.Range(0, OldMen.Length)], pos, rotQ) as GameObject;
                        break;
                    case 3:
                        newPerson = GameObject.Instantiate(OldWomen[Random.Range(0, OldWomen.Length)], pos, rotQ) as GameObject;
                        break;
                    case 4:
                        newPerson = GameObject.Instantiate(Boys[Random.Range(0, Boys.Length)], pos, rotQ) as GameObject;
                        break;
                    case 5:
                        newPerson = GameObject.Instantiate(Girls[Random.Range(0, Girls.Length)], pos, rotQ) as GameObject;
                        break;
                }
            }*/

            navC = newPerson.AddComponent<NavigateCity>();
            //newPerson.AddComponent<LookAt>();
            navC.m_ei = numActivePedestrians;

            newPerson.transform.parent = GameObject.Find("1_Pedestrians").transform;
			newPerson.transform.localScale = newPerson.transform.localScale*avatarScale;

            numActivePedestrians++; // record this person

            return newPerson;
		}

		private void CreateBusLines(){
			busLine3 = new Queue<GameObject>();
			for (int i = 1; i < 9; i++) {
				busLine3.Enqueue (GameObject.Find ("BusStop3-" + i));
			}
		}

		private void AddPedestrianToBusLine(int busStopNum, GameObject pedestrian){
			foreach(GameObject node in busLine3){
				node.GetComponent<BusStopNode>();
			}
		}
		
		#region Waypoint Graph Routines
		
		private void CreateBiPath(string name1,string name2, int a, int b, float narrowness)
		{
			GameObject w1 = GameObject.Find(name1 + "-" + a);
			GameObject w2 = GameObject.Find(name2 + "-" + b);
			
			if(w1 && w2) // if both objects exist
			{
				// create edges between the waypoints in both directions
				globalPathPlanningGraph.AddEdge(w1,w2,narrowness);
				globalPathPlanningGraph.AddEdge(w2,w1,narrowness);
			}
		}
		
		private void CreateWaypointGraph()
		{
			globalPathPlanningWaypoints = GameObject.FindGameObjectsWithTag("waypoint");
			
			// add all the waypoints to the graph
			foreach(GameObject go in globalPathPlanningWaypoints){
				globalPathPlanningGraph.AddNode(go,true,true);
			}


			// extra
			//CreateBiPath("Block6","Block6",5,11,1.0f);

			for(int i = 1; i < 50; i++){ // 1 to 50
				CreateBiPath("Block2","Block2",i,i+1,1.0f);
				CreateBiPath("Block3","Block3",i,i+1,1.0f);
				CreateBiPath("Block5","Block5",i,i+1,1.0f);
				CreateBiPath("Block6","Block6",i,i+1,1.0f);
				CreateBiPath("Block7","Block7",i,i+1,1.0f);
				CreateBiPath("Block8","Block8",i,i+1,1.0f);
				CreateBiPath("Block10","Block10",i,i+1,1.0f);
				CreateBiPath("Block11","Block11",i,i+1,1.0f);
			}
			CreateBiPath("Block2","Block2",50,1,1.0f);
			CreateBiPath("Block3","Block3",50,1,1.0f);
			CreateBiPath("Block5","Block5",50,1,1.0f);
			CreateBiPath("Block6","Block6",50,1,1.0f);
			CreateBiPath("Block7","Block7",50,1,1.0f);
			CreateBiPath("Block8","Block8",50,1,1.0f);
			CreateBiPath("Block10","Block10",50,1,1.0f);
			CreateBiPath("Block11","Block11",50,1,1.0f);

			for(int i = 1; i < 11; i++){ // 1 to 11
				CreateBiPath("Block1","Block1",i,i+1,1.0f);
				CreateBiPath("Block4","Block4",i,i+1,1.0f);
				CreateBiPath("Block9","Block9",i,i+1,1.0f);
				CreateBiPath("Block12","Block12",i,i+1,1.0f);
			}

			// connect the blocks at crosswalks
			CreateBiPath("Block1","Block8",1,25,1.0f);
			CreateBiPath("Block1","Block2",5,5,1.0f);
			CreateBiPath("Block1","Block2",11,11,1.0f);
			CreateBiPath("Block2","Block7",1,15,1.0f);
			CreateBiPath("Block2","Block7",41,25,1.0f);
			CreateBiPath("Block2","Block3",36,5,1.0f);
			CreateBiPath("Block2","Block3",30,11,1.0f);
			CreateBiPath("Block3","Block6",1,15,1.0f);
			CreateBiPath("Block3","Block6",41,25,1.0f);
			CreateBiPath("Block3","Block4",36,5,1.0f);
			CreateBiPath("Block3","Block4",30,11,1.0f);
			CreateBiPath("Block4","Block5",1,15,1.0f);
			CreateBiPath("Block5","Block6",11,30,1.0f);
			CreateBiPath("Block5","Block6",5,36,1.0f);
			CreateBiPath("Block5","Block12",1,1,1.0f);
			CreateBiPath("Block6","Block11",41,25,1.0f);
			CreateBiPath("Block6","Block11",1,15,1.0f);
			CreateBiPath("Block6","Block7",5,36,1.0f);
			CreateBiPath("Block6","Block7",11,30,1.0f);
			CreateBiPath("Block7","Block10",41,25,1.0f);
			CreateBiPath("Block10","Block7",15,1,1.0f);
			CreateBiPath("Block7","Block8",5,36,1.0f);
			CreateBiPath("Block7","Block8",11,30,1.0f);
			CreateBiPath("Block8","Block9",41,1,1.0f);
			CreateBiPath("Block9","Block10",5,11,1.0f);
			CreateBiPath("Block9","Block10",11,5,1.0f);
			CreateBiPath("Block10","Block11",30,11,1.0f);
			CreateBiPath("Block10","Block11",36,5,1.0f);
			CreateBiPath("Block11","Block12",30,5,1.0f);
			CreateBiPath("Block11","Block12",36,11,1.0f);

			// park
			CreateBiPath("Block7","Block7",46,51,1.0f);
			for(int i = 51; i < 65; i++){ // 51 to 65
				CreateBiPath("Block7","Block7",i,i+1,1.0f);
			}
			CreateBiPath("Block7","Block7",65,54,1.0f);
			CreateBiPath("Block7","Block7",66,63,1.0f);
			for(int i = 66; i < 69; i++){ // 66 to 69
				CreateBiPath("Block7","Block7",i,i+1,1.0f);
			}
			CreateBiPath("Block7","Block7",69,8,1.0f);
			for(int i = 70; i < 73; i++){ // 70 to 73
				CreateBiPath("Block7","Block7",i,i+1,1.0f);
			}
			CreateBiPath("Block7","Block7",73,33,1.0f);
			CreateBiPath("Block7","Block7",57,70,1.0f);
			CreateBiPath("Block7","Block7",60,74,1.0f);
			for(int i = 74; i < 76; i++){ // 74 to 76
				CreateBiPath("Block7","Block7",i,i+1,1.0f);
			}
			CreateBiPath("Block7","Block7",76,20,1.0f);
			CreateBiPath("Block6","Block13",44,1,1.0f);
			for(int i = 1; i < 7; i++){ // 1 to 7
				CreateBiPath("Block13","Block13",i,i+1,1.0f);
			}
		}
		
		void CreateDestinationList(){
			destinationList.Add("Block6-48");
			destinationList.Add("Block6-8");////
			destinationList.Add("Block6-17");
			destinationList.Add("Block6-23");
			destinationList.Add("Block6-34");
			destinationList.Add("Block6-43");
			destinationList.Add("Block11-23");
			destinationList.Add("Block12-3");
			destinationList.Add("Block11-8");
			destinationList.Add("Block10-23");
			destinationList.Add("Block10-16");
			destinationList.Add("Block9-6");
			destinationList.Add("Block8-44");
			destinationList.Add("Block8-23");
			destinationList.Add("Block1-8");
			destinationList.Add("Block2-10");
			destinationList.Add("Block7-9");
			destinationList.Add("Block7-49");
			destinationList.Add("Block7-43");
			destinationList.Add("Block7-34");
			destinationList.Add("Block7-28");
			destinationList.Add("Block7-21");
			destinationList.Add("Block7-16");
			destinationList.Add("Block2-46");
			destinationList.Add("Block2-35");
			destinationList.Add("Block3-48");
			destinationList.Add("Block3-42");
			destinationList.Add("Block4-8");
			destinationList.Add("Block3-31");
			destinationList.Add("Block5-19");
			destinationList.Add("Block5-9");
			destinationList.Add("Block5-47");
			destinationList.Add("Block11-37");
			destinationList.Add("Block13-7");
		}
		
		#endregion

		
		/*void OnGUI () {
			if (renderGroupAgent) {
				GUI.BeginGroup (new Rect (0, 10, 300, 25));
				GUI.Box (new Rect (10, 10, 290, 15), "");
				GUI.Label (new Rect (10, 10, 290, 15), " # Pedestrians: " + numActivePedestrians.ToString ("00") + ", FPS: " + (accum / frames).ToString ("00.0000") + ", Time: " + Time.fixedTime.ToString ("f2"), hUDStyle);
				GUI.EndGroup ();
			}
		}
		
		void LateUpdate() {
		    // fps calculation
		    timeleft -= Time.deltaTime;
		    accum += Time.timeScale/Time.deltaTime;
		    ++frames;
		    if( timeleft <= 0.0f ){
		        timeleft = updateInterval;
		        accum = 0.0f;
		        frames = 0;
		    }
		}*/
	}
}