using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Wayfinding;
using MonsterLove.StateMachine;
using System;
using System.Linq;

namespace CrowdSim
{
	public class PedestrianSimulator: StateBehaviour{

		public enum SimulatorState{
			Bootup,
			Run,
			Exit
		}

        #region Simulation Settings
        private int numPedestrians;        // default is 100 total
        public string[] SpawnTags;
        public bool groups = true;      // Individuals vs Groups
        public static bool sightBeamsOn = false;    // view sight beams
        public static bool renderGroupAgent = false; // see group agent or not, pedestrian disks or not, framerate or not
        public static bool cornerTurn = false;      // turn on turning
        public bool fillGroup = false;      // make all groups 5 members
        public static bool usingGroupGMAbasedCA = false;  // use GMA-based collision avoidance
        public static bool useNavmeshAgentNavigation = true; // if true, use navmesh agent path finding and avoidance (to slot of group agent)
        public static bool offsetEnabled = false; // allow offset placement from current node in path (random nearby location at each waypoint)
        public static float avatarScale = 1.2f;            // handle scaling of 3D avatar models, 0.65
        public static float scale = 1.0f; // scale divisor, 1.5

        #endregion

        #region Change Mode Settings
        public static bool breadCrumbs = false;		// show trail of pedestrians
		public static bool renderHiddenObjects = false;  // show hiddent objects or not (e.g. group agent, discs)
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

        private int numGroup5 = 0;
        private int numGroup4 = 0;
        private int numGroup3 = 0;
        private int numGroup2 = 0;

		public static LayerMask m_WallLayerForRayCast;
		public static LayerMask m_FloorLayerForRayCast;
        public static LayerMask raycastLayers;

        private static List<GameObject> mAllWaypoints;

        private Camera[] cameras;
        private bool npcEnabled = true;

        void OnEnable(){
            numActivePedestrians = 0;
        }

        void OnDisable(){
            StopAllCoroutines();
        }

        public GameObject[] GetGroupAgents(){
            return groupAgents;
        }

        public ArrayList GetDestinationList(){
            return destinationList;
        }

        public Graph GetGlobalPathPlanningGraph(){
            return globalPathPlanningGraph;
        }

        public static T[] CreateCombinedArrayFrom<T>(T[] first, T[] second, T[] third, T[] fourth, T[] fifth, T[] sixth){
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

        void Awake(){
            input = Rewired.ReInput.players.GetPlayer(0);
            Initialize<SimulatorState>();
        }

        // Use this for initialization
        void Start(){
            m_WallLayerForRayCast = 1 << LayerMask.NameToLayer("Collidable");
            m_FloorLayerForRayCast = 1 << LayerMask.NameToLayer("Floor");

            raycastLayers = m_WallLayerForRayCast.value;

            mSpawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag(SpawnTags[0]));
            for (int i = 1; i < SpawnTags.Length; i++)
                mSpawnPoints.AddRange(GameObject.FindGameObjectsWithTag(SpawnTags[i]));

            // old
            mAllWaypoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("waypoint")); // TODO: Create a static class to hold tag strings
            //mWarpWaypoints = GameObject.FindGameObjectsWithTag("WaypointsVeredaSpawn");
            //mAllWaypoints.AddRange(mWarpWaypoints);

            timeleft = updateInterval;

            cameras = Camera.allCameras; // only player cameras allowed! no other cameras should be in scene

            ChangeState(SimulatorState.Bootup);
        }

        float timeDelay = 0.0f;
        private int remainder;

        void Bootup_Enter(){
			print ("Loading waypoint graph");
			CreateWaypointGraph();   // create waypoint graph for global navigation by group agents
			CreateDestinationList(); // based on graph, choose possible destination points for group agents

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
            numGroup5 = Mathf.RoundToInt((remainder) * 0.01f);
            numGroup4 = Mathf.RoundToInt((remainder) * 0.02f);
            numGroup3 = Mathf.RoundToInt((remainder) * 0.06f);
            numGroup2 = Mathf.RoundToInt((remainder) * 0.28f);

            print("Loading group agents with people");
            SpawnGroupsOfPeople();

            //currentState = SimulatorState.Run;

            print("Loading single pedestrians");
            timeDelay = 0.0f; // 0 seconds
        }

        public void SpawnGroupsOfPeople(){

            int numWaypoints = globalPathPlanningWaypoints.Length;

            for (int i = 0; i < remainder; i++){

                // spawn according to realistic group size distribution
                int numifs = NumMembers(); // 1 to 5 passengers
                if (numifs == 1) return; // all 1-person groups are not part of group!

                // center, originally facing positive Z
                Vector3 pos = new Vector3(0.0f,0.1f,0.0f); // random position above floor
                Quaternion rotQ = Quaternion.identity;
                string closest = "";

                int randomWaypointIndex = UnityEngine.Random.Range(0, numWaypoints);
                GameObject go = (GameObject) globalPathPlanningWaypoints.GetValue(randomWaypointIndex);
                pos = go.transform.position; // go.transform.position
                closest = go.name;

                GameObject obj;
                obj = getNewGroupAgentObject(pos,rotQ);
                obj.transform.tag = "GA";
                obj.AddComponent<UrbanTownGA>(); // add WaypointAgent component!
                obj.GetComponent<UrbanTownGA>().FindStartNode(closest);
                obj.transform.parent = GameObject.Find("3_GroupAgents").transform;

                if (numifs == 5){
                    InitializePedestrianFollowTarget(obj,5);
                    //numActivePedestrians++;
                    i++;
                    if (i < numPedestrians) numifs--;
                }
                if (numifs == 4){
                    InitializePedestrianFollowTarget(obj,4);
                    //numActivePedestrians++;
                    i++;
                    if (i < numPedestrians) numifs--;
                }
                if (numifs == 3){
                    InitializePedestrianFollowTarget(obj,3);
                    //numActivePedestrians++;
                    i++;
                    if (i < numPedestrians) numifs--;
                }
                if (numifs == 2){
                    InitializePedestrianFollowTarget(obj,2);
                    //numActivePedestrians++;
                    i++;
                    if (i < numPedestrians) numifs--;
                }
                if (numifs == 1){
                    InitializePedestrianFollowTarget(obj,1);
                    //numActivePedestrians++;
                }
            }
        }

        void Bootup_Update(){

            if (getTimeEnteredState() > timeDelay) { // delay

                timeDelay += 0.25f; // delay 1/4 second for next spawn

                int index = UnityEngine.Random.Range(0, mSpawnPoints.Count);
                Vector3 startPos = mSpawnPoints[index].transform.position;
                GameObject newPerson = CreatePedestrian(startPos); // create new pedestrian

                ObjectRef oref = mSpawnPoints[index].GetComponent<ObjectRef>();
                if (oref != null)
                {
                    // change initial state to getting out of subway
                    newPerson.SendMessage("SubwayInOut", oref.Ref, SendMessageOptions.DontRequireReceiver);
                }

                //numActivePedestrians++;

                if(numActivePedestrians > numPedestrians-1){
                    ChangeState(SimulatorState.Run);
                }

            }

        }

        void Bootup_Exit(){
            ///////SpawnGroupAgents();
        }

        void Run_Enter(){
            if(numPedestrians > 0) pedestrians = GameObject.FindGameObjectsWithTag("Pedestrian"); // save all pedestrians in array for later reference
            GameObject.Destroy(GameObject.Find(UrbanTownGA.IGNORE_WAYPOINT_AGENT_NAME));
            groupAgents = GameObject.FindGameObjectsWithTag("GA"); // save all group agents in array for later reference

            print("Total Group Agents: " + numActiveGroupAgents);
            print("Total Pedestrians: " + numActivePedestrians);
            pedestriansLoaded = true;
        }

        void Run_Update(){
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
						groupAgents[i].GetComponent<UrbanTownGA>().ChangeVisibility(1.0f); // make appear
					}
				}
				else{
					renderHiddenObjects = false;
					print ("Hidden Objects Turned Off");
					for(int i = 0; i < groupAgents.Length; i++){
						groupAgents[i].GetComponent<UrbanTownGA>().ChangeVisibility(0.0f); // make transparent
					}
				}
			}*/

            if (input.GetButtonDown("DPadRight")){
                npcEnabled = !npcEnabled;
                if (npcEnabled){
                    Physics.IgnoreLayerCollision(11, 12, false);
                }
                else{
                    Physics.IgnoreLayerCollision(11, 12, true);
                }
                updateMaskstate();
            }

            // display hidden stuff or not
            if (renderHiddenObjects){
                globalPathPlanningGraph.debugDraw(); // draw the paths in the scene view of the editor while playing
            }
        }

        void updateMaskstate(){
            int renderMask = 0;
            if (!npcEnabled){
                renderMask |= 1 << 11;
            }
            foreach (Camera cam in cameras){
                cam.cullingMask = ~renderMask;
            }
        }

        public static GameObject getNewGroupAgentObject(Vector3 pos, Quaternion rotQ){
            numActiveGroupAgents++;
            return Instantiate(groupAgentObject,pos,rotQ) as GameObject;
        }

        private void InitializeFollowTargetForPedestrian(GameObject obj, int slotTarget, GameObject person){
            Vector3 pos = obj.GetComponent<UrbanTownGA>().getSlotPosition(slotTarget);
            person.transform.position = pos;


            //person.transform.GetComponent<Navigate>().SetTarget(obj.GetComponent<UrbanTownGA>().getSlotTransform(slotTarget));
            //obj.GetComponent<UrbanTownGA>().setSlotNotAvailable(slotTarget);
            //person.GetComponent<Navigate>().assignedGA = obj;
            //person.GetComponent<Navigate>().assignedSlotNumber = slotTarget;

            obj.GetComponent<UrbanTownGA>().AddMember(person, slotTarget);
        }

        private void InitializePedestrianFollowTarget(GameObject obj, int slotTarget){
            Vector3 pos = obj.GetComponent<UrbanTownGA>().getSlotPosition(slotTarget);
            GameObject person = CreatePedestrian(pos); // create new pedestrian

            if (person == null){
                numActivePedestrians--;
                return;
            }

            //person.GetComponent<Animator> ().applyRootMotion = true;

            //person.transform.GetComponent<Navigate>().SetTarget(obj.GetComponent<UrbanTownGA>().getSlotTransform(slotTarget));
            //obj.GetComponent<UrbanTownGA>().setSlotNotAvailable(slotTarget);
            //person.GetComponent<Navigate>().assignedGA = obj;
            //person.GetComponent<Navigate>().assignedSlotNumber = slotTarget;

            obj.GetComponent<UrbanTownGA>().AddMember(person, slotTarget);
        }

        private int NumMembers(){
            if (fillGroup) return 5;
            if (numGroup5 > 0) { numGroup5--; return 5; }
            if (numGroup4 > 0) { numGroup4--; return 4; }
            if (numGroup3 > 0) { numGroup3--; return 3; }
            if (numGroup2 > 0) { numGroup2--; return 2; }
            return 1;
        }

        /*public static Vector3 getRendezvousPoint()
		{
			int index = Random.Range(0, rendezvousPoints.Count);
			return rendezvousPoints[index].transform.position;
		}*/

        public static Vector3 getSpawnPoint(){
            int index = UnityEngine.Random.Range(0, mSpawnPoints.Count);
            return mSpawnPoints[index].transform.position;
        }

        public static GameObject getWayPointObject(){
            int index = UnityEngine.Random.Range(0, mAllWaypoints.Count);
            return mAllWaypoints[index];
        }

        private int boysOnce = 0;
        private int girlsOnce = 0;
        private int menOnce = 0;
        private int womenOnce = 0;
        private int oldMenOnce = 0;
        private int oldWomenOnce = 0;
        private UnityEngine.AI.NavMeshHit closestHit;

        private GameObject CreatePedestrian(Vector3 pos){

            // GameObject clone; use only if need
            if (UnityEngine.AI.NavMesh.SamplePosition(pos, out closestHit, 1, 1)){ // make sure on navigation mesh!
                pos = closestHit.position;
            }

            // originally facing positive Z
            Quaternion rotQ = Quaternion.identity;
            GameObject newPerson = null;

            NavigateUrbanTown navC = null;

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
            else{ // choose randomly
                int personType = UnityEngine.Random.Range(0, 6);
                switch (personType){
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

            navC = newPerson.AddComponent<NavigateUrbanTown>();
            //newPerson.AddComponent<LookAt>();
            navC.m_ei = numActivePedestrians;

            newPerson.transform.parent = GameObject.Find("1_Pedestrians").transform;
            newPerson.transform.localScale = newPerson.transform.localScale*avatarScale;

            numActivePedestrians++; // record this person

            return newPerson;
        }

        #region Waypoint Graph Routines

        private void CreateBiPath(string a, string b, float narrowness){
			GameObject w1 = GameObject.Find(a);
			GameObject w2 = GameObject.Find(b);
			
			if(w1 && w2){
				// create edges between the waypoints in both directions
				globalPathPlanningGraph.AddEdge(w1,w2,narrowness);
				globalPathPlanningGraph.AddEdge(w2,w1,narrowness);
			}
		}
		
		private void CreateWaypointGraph(){
			globalPathPlanningWaypoints = GameObject.FindGameObjectsWithTag("waypoint");
			
			// add all the waypoints to the graph
			foreach(GameObject go in globalPathPlanningWaypoints){
				globalPathPlanningGraph.AddNode(go,true,true);
			}
				
			// create edges between the waypoints
			CreateBiPath("IslandToCentral","PlayerStart",1.0f);
			CreateBiPath("PlayerStart","CentralToIsland",1.0f);
			CreateBiPath("IslandToCentral","IslandCorner",0.0f);
			CreateBiPath("IslandCorner","IslandToSmallIsland",0.0f);
			CreateBiPath("SmallIslandToIsland","IslandToSmallIsland",1.0f);
			CreateBiPath("SmallIslandToIsland","SmallIslandCorner",0.75f);
			CreateBiPath("SmallIslandToCentral","SmallIslandCorner",0.75f);
			CreateBiPath("SmallIslandToCentral","SmallIslandToIsland",0.75f);
			CreateBiPath("SmallIslandCorner","SmallIslandPath1",0.75f);
			CreateBiPath("SmallIslandToIsland","SmallIslandPath1",0.75f);
			CreateBiPath("SmallIslandPath1","SmallIslandPath2",0.75f);
			CreateBiPath("SmallIslandPath2","SmallIslandPath3",0.75f);
			CreateBiPath("SmallIslandPath3","SmallIslandPath4",0.75f);
			CreateBiPath("SmallIslandPath4","SmallIslandPath5",0.75f);
			CreateBiPath("SmallIslandPath5","SmallIslandPath6",0.75f);
			CreateBiPath("SmallIslandPath6","SmallIslandCorner",0.75f);
			CreateBiPath("SmallIslandPath6","SmallIslandToCentral",0.75f);
			CreateBiPath("SmallIslandPath6","CentralPath2",1.0f);
			CreateBiPath("CentralPath1","CentralPath2",0.75f);
			CreateBiPath("CentralPath2","CentralPath3",0.75f);
			CreateBiPath("IslandPath1","IslandToSmallIsland",0.75f);
			CreateBiPath("IslandPath1","IslandPath2",0.75f);
			CreateBiPath("SmallIslandPath2","IslandPath2",1.0f);
			CreateBiPath("IslandPath3","IslandPath2",0.75f);
			CreateBiPath("IslandPath3","IslandPath1",0.75f);
			CreateBiPath("IslandPath3","IslandPath4",0.75f);
			CreateBiPath("IslandPath5","IslandPath4",0.75f);
			CreateBiPath("IslandPath5","IslandPath6",0.75f);
			CreateBiPath("IslandPath7","IslandPath6",0.75f);
			CreateBiPath("IslandPath7","IslandPath8",0.75f);
			CreateBiPath("IslandPath9","IslandPath8",0.75f);
			CreateBiPath("IslandPath9","IslandPath10",0.75f);
			CreateBiPath("IslandPath10","IslandPath11",0.75f);
			CreateBiPath("IslandToCentral","IslandPath11",0.75f);
			CreateBiPath("CentralPath0","CentralToIsland",0.75f);
			CreateBiPath("CentralPath00","CentralPath0",0.75f);
			CreateBiPath("CentralPath00","CentralPath01",0.75f);
			CreateBiPath("CentralPath02","CentralPath01",0.75f);
			CreateBiPath("CentralPath02","CentralPath03",0.75f);
			CreateBiPath("CentralPath1","CentralPath03",0.75f);
			CreateBiPath("CentralPath3","CentralPath4",0.75f);
			CreateBiPath("CentralPath4","CentralPath5",0.75f);
			CreateBiPath("CentralPath5","CentralPath6",0.75f);
			CreateBiPath("CentralPath6","CentralPath7",0.75f);
			CreateBiPath("CentralPath7","CentralPath8",0.75f);
			CreateBiPath("CentralPath8","CentralPath9",0.75f);
			CreateBiPath("CentralPath1","CentralPath9",0.75f);
			CreateBiPath("CentralPath3","CentralPath9",0.75f);
			CreateBiPath("CentralPath2","CentralPath9",0.75f);
			CreateBiPath("CentralPath00","CentralPath10",0.75f);
			CreateBiPath("CentralPath11","CentralPath10",0.75f);
			CreateBiPath("CentralPath11","CentralPath12",0.75f);
			CreateBiPath("CentralPath8","CentralPath12",0.75f);
			CreateBiPath("CentralPath5","CentralPath10",0.75f);
			CreateBiPath("CentralPath5","CentralPath13",0.75f);
			CreateBiPath("CentralPath14","CentralPath13",0.75f);
			CreateBiPath("CentralPath14","CentralPath15",0.75f);
			CreateBiPath("CentralPath16","CentralPath15",0.75f);
			CreateBiPath("CentralPath16","CentralToIsland",0.75f);
			CreateBiPath("CentralPath16","CentralPath0",0.75f);
			CreateBiPath("CentralPath12","CentralPath13",0.75f);
			CreateBiPath("CentralPath17","CentralPath15",0.75f);
			CreateBiPath("CentralPath17","CentralPath14",0.75f);
			CreateBiPath("CentralPath17","CentralPath18",0.75f);
			CreateBiPath("CentralPath18","CentralPath19",0.75f);
			CreateBiPath("CentralPath19","CentralPath20",0.75f);
			CreateBiPath("CentralPath20","CentralPath21",0.75f);
			CreateBiPath("CentralPath21","CentralPath22",0.75f);
			CreateBiPath("CentralPath22","CentralPath23",0.75f);
			CreateBiPath("CentralPath23","CentralPath24",0.75f);
			CreateBiPath("CentralPath24","CentralPath25",0.75f);
			CreateBiPath("CentralPath25","CentralPath26",0.75f);
			CreateBiPath("CentralPath26","CentralPath27",0.75f);
			CreateBiPath("CentralPath27","CentralPath28",0.75f);
			CreateBiPath("CentralPath28","CentralPath29",0.75f);
			CreateBiPath("CentralPath16","CentralPath29",0.75f);
			CreateBiPath("CentralToIsland","CentralPath29",0.75f);
			CreateBiPath("CentralPath0","CentralPath29",0.75f);
			CreateBiPath("CentralPath19","CentralPath28",0.75f);
			CreateBiPath("OuterPath1","OuterPath2",0.75f);
			CreateBiPath("OuterPath2","OuterPath3",0.75f);
			CreateBiPath("OuterPath3","OuterPath4",1.0f);
			CreateBiPath("OuterPath4","OuterPath5",1.0f);
			CreateBiPath("OuterPath5","OuterPath6",1.0f);
			CreateBiPath("OuterPath6","OuterPath7",1.0f);
			CreateBiPath("OuterPath7","OuterPath8",1.0f);
			CreateBiPath("OuterPath8","OuterPath9",1.0f);
			CreateBiPath("OuterPath9","OuterPath10",1.0f);
			CreateBiPath("OuterPath10","OuterPath11",0.75f);
			CreateBiPath("OuterPath11","OuterPath12",0.75f);
			CreateBiPath("OuterPath12","OuterPath13",0.75f);
			CreateBiPath("OuterPath13","OuterPath14",0.75f);
			CreateBiPath("OuterPath14","OuterPath15",0.75f);
			CreateBiPath("OuterPath15","OuterPath16",1.0f);
			CreateBiPath("OuterPath16","OuterPath17",1.0f);
			CreateBiPath("OuterPath17","OuterPath18",1.0f);
			CreateBiPath("OuterPath18","OuterPath19",1.0f);
			CreateBiPath("OuterPath19","OuterPath20",1.0f);
			CreateBiPath("OuterPath20","OuterPath21",1.0f);
			CreateBiPath("OuterPath21","OuterPath22",1.0f);
			CreateBiPath("OuterPath22","OuterPath23",1.0f);
			CreateBiPath("OuterPath23","OuterPath24",1.0f);
			CreateBiPath("OuterPath24","OuterPath25",1.0f);
			CreateBiPath("OuterPath25","OuterPath26",1.0f);
			CreateBiPath("OuterPath26","OuterPath27",1.0f);
			CreateBiPath("OuterPath27","OuterPath28",1.0f);
			CreateBiPath("OuterPath28","OuterPath29",1.0f);
			CreateBiPath("OuterPath29","OuterPath30",1.0f);
			CreateBiPath("OuterPath30","OuterPath31",1.0f);
			CreateBiPath("CentralPath4","OuterPath1",1.0f);
			CreateBiPath("CentralPath4","OuterPath2",1.0f);
			CreateBiPath("CentralPath3","OuterPath2",1.0f);
			CreateBiPath("CentralPath3","OuterPath3",1.0f);
			CreateBiPath("SmallIslandPath5","OuterPath5",1.0f);
			CreateBiPath("SmallIslandPath4","OuterPath7",1.0f);
			CreateBiPath("SmallIslandPath4","OuterPath8",1.0f);
			CreateBiPath("SmallIslandPath3","OuterPath11",1.0f);
			CreateBiPath("IslandPath4","OuterPath17",1.0f);
			CreateBiPath("IslandPath6","OuterPath24",1.0f);
			CreateBiPath("CentralPath26","OuterPath27",1.0f);
			CreateBiPath("CentralPath25","OuterPath28",1.0f);
			CreateBiPath("CentralPath25","OuterPath29",1.0f);
			CreateBiPath("CentralPath24","OuterPath30",1.0f);
			CreateBiPath("CentralPath24","OuterPath31",1.0f);
			CreateBiPath("CentralPath23","OuterPath31",1.0f);
			CreateBiPath("CentralPath27","IslandPath8",1.0f);
			CreateBiPath("OuterPath3","OuterPath32",0.0f);
			CreateBiPath("OuterPath32","OuterPath33",0.0f);
			CreateBiPath("OuterPath33","OuterPath34",0.0f);
			CreateBiPath("OuterPath34","OuterPath4",0.0f);
			CreateBiPath("OuterPath34","OuterPath35",0.0f);
			CreateBiPath("OuterPath33","OuterPath35",0.0f);
			CreateBiPath("OuterPath35","OuterPath36",0.0f);
			CreateBiPath("OuterPath36","OuterPath37",0.0f);
			CreateBiPath("OuterPath37","OuterPath6",0.0f);
			CreateBiPath("OuterPath37","OuterPath38",0.0f);
			CreateBiPath("OuterPath38","OuterPath7",0.0f);
			CreateBiPath("OuterPath8","OuterPath39",0.0f);
			CreateBiPath("OuterPath9","OuterPath40",0.0f);
			CreateBiPath("OuterPath40","OuterPath41",0.0f);
			CreateBiPath("OuterPath41","OuterPath42",0.0f);
			CreateBiPath("OuterPath42","OuterPath10",0.0f);
			CreateBiPath("OuterPath42","OuterPath9",0.0f);
			CreateBiPath("OuterPath41","OuterPath43",0.0f);
			CreateBiPath("OuterPath44","OuterPath45",0.0f);
			CreateBiPath("OuterPath10","OuterPath45",0.0f);
			CreateBiPath("OuterPath10","OuterPath44",0.0f);
			CreateBiPath("OuterPath44","OuterPath46",0.0f);
			CreateBiPath("OuterPath43","OuterPath46",0.0f);
			CreateBiPath("OuterPath46","OuterPath47",0.0f);
			CreateBiPath("OuterPath43","OuterPath47",0.0f);
			CreateBiPath("OuterPath47","OuterPath48",0.0f);
			CreateBiPath("OuterPath45","OuterPath48",0.0f);
			CreateBiPath("OuterPath45","OuterPath49",0.0f);
			CreateBiPath("OuterPath48","OuterPath49",0.0f);
			CreateBiPath("OuterPath49","OuterPath50",0.0f);
			CreateBiPath("OuterPath48","OuterPath50",0.0f);
			CreateBiPath("OuterPath39","OuterPath40",0.0f);
			CreateBiPath("OuterPath50","OuterPath51",0.0f);
			CreateBiPath("OuterPath51","OuterPath52",0.0f);
			CreateBiPath("OuterPath52","OuterPath53",0.0f);
			CreateBiPath("OuterPath53","OuterPath54",0.0f);
			CreateBiPath("OuterPath54","OuterPath55",0.0f);
			CreateBiPath("OuterPath55","OuterPath56",0.0f);
			CreateBiPath("OuterPath56","OuterPath57",0.0f);
			CreateBiPath("OuterPath57","OuterPath58",0.0f);
			CreateBiPath("OuterPath58","OuterPath11",0.0f);
			CreateBiPath("OuterPath57","OuterPath54",0.0f);
			CreateBiPath("OuterPath59","OuterPath12",0.0f);
			CreateBiPath("OuterPath59","OuterPath56",0.0f);
			CreateBiPath("OuterPath14","OuterPath60",0.0f);
			CreateBiPath("OuterPath60","OuterPath56",0.0f);
			CreateBiPath("OuterPath60","OuterPath59",0.0f);
			CreateBiPath("OuterPath60","OuterPath57",0.0f);
			CreateBiPath("OuterPath61","OuterPath60",0.0f);
			CreateBiPath("OuterPath61","OuterPath56",0.0f);
			CreateBiPath("OuterPath61","OuterPath59",0.0f);
			CreateBiPath("OuterPath61","OuterPath57",0.0f);
			CreateBiPath("OuterPath59","OuterPath57",0.0f);
			CreateBiPath("OuterPath61","OuterPath62",0.0f);
			CreateBiPath("OuterPath62","OuterPath63",0.0f);
			CreateBiPath("OuterPath63","OuterPath64",0.0f);
			CreateBiPath("OuterPath62","OuterPath64",0.0f);
			CreateBiPath("OuterPath64","OuterPath65",0.0f);
			CreateBiPath("OuterPath65","OuterPath15",0.0f);
			CreateBiPath("OuterPath66","OuterPath63",0.0f);
			CreateBiPath("OuterPath67","OuterPath66",0.0f);
			CreateBiPath("OuterPath67","OuterPath65",0.0f);
			CreateBiPath("OuterPath68","OuterPath16",0.0f);
			CreateBiPath("OuterPath68","OuterPath69",0.0f);
			CreateBiPath("OuterPath68","OuterPath66",0.0f);
			CreateBiPath("OuterPath69","OuterPath65",0.0f);
			CreateBiPath("OuterPath69","OuterPath70",0.0f);
			CreateBiPath("OuterPath65","OuterPath70",0.0f);
			CreateBiPath("OuterPath70","OuterPath71",0.0f);
			CreateBiPath("OuterPath16","OuterPath71",0.0f);
			CreateBiPath("OuterPath72","OuterPath71",0.0f);
			CreateBiPath("OuterPath72","OuterPath16",0.0f);
			CreateBiPath("OuterPath72","OuterPath17",0.0f);
			CreateBiPath("OuterPath17","OuterPath71",0.0f);
			CreateBiPath("OuterPath72","OuterPath73",0.0f);
			CreateBiPath("OuterPath17","OuterPath73",0.0f);
			CreateBiPath("OuterPath72","OuterPath74",0.0f);
			CreateBiPath("OuterPath74","OuterPath75",0.0f);
			CreateBiPath("OuterPath75","OuterPath70",0.0f);
			CreateBiPath("OuterPath75","OuterPath69",0.0f);
			CreateBiPath("OuterPath75","OuterPath67",0.0f);
			CreateBiPath("OuterPath75","OuterPath65",0.0f);
			CreateBiPath("OuterPath76","OuterPath74",0.0f);
			CreateBiPath("OuterPath76","OuterPath77",0.0f);
			CreateBiPath("OuterPath77","OuterPath73",0.0f);
			CreateBiPath("OuterPath77","OuterPath78",0.0f);
			CreateBiPath("OuterPath18","OuterPath78",0.0f);
			CreateBiPath("OuterPath78","OuterPath79",0.0f);
			CreateBiPath("OuterPath79","OuterPath80",0.0f);
			CreateBiPath("OuterPath80","OuterPath76",0.0f);
			CreateBiPath("OuterPath81","OuterPath80",0.0f);
			CreateBiPath("OuterPath81","OuterPath82",0.0f);
			CreateBiPath("OuterPath79","OuterPath82",0.0f);
			CreateBiPath("OuterPath83","OuterPath82",0.0f);
			CreateBiPath("OuterPath83","OuterPath79",0.0f);
			CreateBiPath("OuterPath83","OuterPath19",0.0f);
			CreateBiPath("OuterPath81","OuterPath84",0.0f);
			CreateBiPath("OuterPath81","OuterPath85",0.0f);
			CreateBiPath("OuterPath82","OuterPath84",0.0f);
			CreateBiPath("OuterPath85","OuterPath82",0.0f);
			CreateBiPath("OuterPath84","OuterPath85",0.0f);
			CreateBiPath("OuterPath86","OuterPath84",0.0f);
			CreateBiPath("OuterPath87","OuterPath85",0.0f);
			CreateBiPath("OuterPath87","OuterPath88",0.0f);
			CreateBiPath("OuterPath88","OuterPath20",0.0f);
			CreateBiPath("OuterPath88","OuterPath21",0.0f);
			CreateBiPath("OuterPath87","OuterPath89",0.0f);
			CreateBiPath("OuterPath90","OuterPath89",0.0f);
			CreateBiPath("OuterPath90","OuterPath91",0.0f);
			CreateBiPath("OuterPath91","OuterPath21",0.0f);
			CreateBiPath("OuterPath91","OuterPath20",0.0f);
			CreateBiPath("OuterPath91","OuterPath88",0.0f);
			CreateBiPath("OuterPath92","OuterPath91",0.0f);
			CreateBiPath("OuterPath92","OuterPath90",0.0f);
			CreateBiPath("OuterPath92","OuterPath93",0.0f);
			CreateBiPath("OuterPath93","OuterPath22",0.0f);
			CreateBiPath("OuterPath93","OuterPath94",0.0f);
			CreateBiPath("OuterPath94","OuterPath23",0.0f);
			CreateBiPath("OuterPath95","OuterPath94",0.0f);
			CreateBiPath("OuterPath96","OuterPath95",0.0f);
			CreateBiPath("OuterPath96","OuterPath25",0.0f);
			CreateBiPath("OuterPath96","OuterPath97",0.0f);
			CreateBiPath("OuterPath97","OuterPath98",0.0f);
			CreateBiPath("OuterPath98","OuterPath99",0.0f);
			CreateBiPath("OuterPath99","OuterPath26",0.0f);
			CreateBiPath("OuterPath99","OuterPath100",0.0f);
			CreateBiPath("OuterPath100","OuterPath101",0.0f);
			CreateBiPath("OuterPath101","OuterPath102",0.0f);
			CreateBiPath("OuterPath102","OuterPath27",0.0f);
			CreateBiPath("OuterPath102","OuterPath103",0.0f);
			CreateBiPath("OuterPath103","OuterPath104",0.0f);
			CreateBiPath("OuterPath104","OuterPath102",0.0f);
			CreateBiPath("OuterPath104","OuterPath101",0.0f);
			CreateBiPath("OuterPath101","OuterPath103",0.0f);
			CreateBiPath("OuterPath105","OuterPath28",0.0f);
			CreateBiPath("OuterPath105","OuterPath106",0.0f);
			CreateBiPath("OuterPath107","OuterPath106",0.0f);
			CreateBiPath("OuterPath107","OuterPath31",0.0f);
			CreateBiPath("OuterPath106","OuterPath107",0.0f);
			CreateBiPath("OuterPath108","OuterPath109",0.0f);
			CreateBiPath("OuterPath109","OuterPath104",0.0f);
			CreateBiPath("OuterPath108","OuterPath106",0.0f);
			CreateBiPath("OuterPath105","OuterPath103",0.0f);
			
			// CR1
			/*foreach(GameObject go in cr1s){
				cr1.AddNode(go,true,true);
			}
			GameObject o = GameObject.Find("CR1");
			for(int i = 1; i < 12; i++){ // 1 to 12
				CreateBiPath(ref cr1,"CR1",i,i+1);
			}
			*/
		}
		
		void CreateDestinationList(){
			destinationList.Add("SmallIslandCorner");
			destinationList.Add("OuterPath16");
			destinationList.Add("PlayerStart");
			destinationList.Add("CentralPath4");
			destinationList.Add("CentralPath29");
			destinationList.Add("CentralPath20");
			destinationList.Add("CentralPath14");
			destinationList.Add("CentralPath10");
			destinationList.Add("OuterPath31");
			destinationList.Add("OuterPath47");
			destinationList.Add("OuterPath80");
			destinationList.Add("OuterPath50");
			destinationList.Add("OuterPath25");
			destinationList.Add("OuterPath7");
		}
		
		#endregion

		
		/*void OnGUI () {
		    GUI.BeginGroup(new Rect(0,10,300,25));
		    GUI.Box(new Rect(10,10,290,15),"");
		    GUI.Label(new Rect(10,10,290,15)," # Pedestrians: " + numActivePedestrians.ToString("00") + ", FPS: " + (accum/frames).ToString("00.0000") + ", Time: " + Time.fixedTime.ToString("f2"),hUDStyle);
		    GUI.EndGroup();
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