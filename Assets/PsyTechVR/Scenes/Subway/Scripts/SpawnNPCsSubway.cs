using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnNPCsSubway : MonoBehaviour {

	public GameObject[] PersonPrefabs;
	public float SpeedRnd = 0.25f;
	public string[] SpawnTags;
	private int mask;
	
	private List<GameObject> mSpawnPoints;

	private void CreateNavMeshAgent(GameObject newPerson){ // keep on navmesh!
		UnityEngine.AI.NavMeshAgent navComponent = newPerson.AddComponent<UnityEngine.AI.NavMeshAgent>();
		CapsuleCollider cc = (CapsuleCollider) newPerson.GetComponent("CapsuleCollider");
		cc.radius = 0.2f; // 0.3 --> 0.2
		navComponent.radius = cc.radius; // personal space others cannot pass
		navComponent.height = cc.height; // determines passing under obstacles or not
		navComponent.speed = 1.0f; // 5.7 maximum speed allowed for agent 0.8
		navComponent.acceleration = 8.0f;
		navComponent.angularSpeed = 120.0f;
		navComponent.autoTraverseOffMeshLink = true;
		navComponent.autoBraking = true; // prevent overshooting of destination point
		navComponent.autoRepath = true; // acquire new path if existing path becomes invalid
		navComponent.stoppingDistance = 0.0f; //0.8 in case cannot stop exactly at destination
		navComponent.baseOffset = 0.0f;
		navComponent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
		navComponent.avoidancePriority = 50;
		navComponent.areaMask = mask;
		cc.isTrigger = false; // don't change! 2/3/2015
		//cc.material = (PhysicMaterial) Resources.Load("SubwayCarFloor");
	}
	
	//private static string WALK_ANIM = "Walk";
	
	void Awake()
	{
		
	}
	
	void Start () {
		mask = 1 << UnityEngine.AI.NavMesh.GetAreaFromName("PlataformaSubte");

		// first get outer exits
		mSpawnPoints = new List<GameObject>( GameObject.FindGameObjectsWithTag(SpawnTags[0]) );

        int size = PersonPrefabs.Length;
		
		for(int i = 0; i < size; i++){ // 2/3
			int index = Random.Range(0, mSpawnPoints.Count);
			Vector3 dest = mSpawnPoints[index].transform.position;
			
			GameObject model = PersonPrefabs[i]; // Random.Range(0, PersonPrefabs.Length)

            GameObject newPerson = GameObject.Instantiate(model, dest, Quaternion.identity) as GameObject;
			CreateNavMeshAgent(newPerson);
			NavigateSubway navSub = newPerson.AddComponent<NavigateSubway>();
			navSub.AvailableStations = new string[]{"S1","S2","S3"};

			// random speed
			float randomizer = 1f + Random.Range(-SpeedRnd, SpeedRnd);
			newPerson.GetComponent<UnityEngine.AI.NavMeshAgent>().speed *= randomizer;

			newPerson.SendMessage("SetStartPoint", mSpawnPoints[index],SendMessageOptions.DontRequireReceiver );
			
			//newPerson.animation[WALK_ANIM].speed *= randomizer;
		}

		// then add inner exits
		for(int i = 1 ; i < SpawnTags.Length; i++)
			mSpawnPoints.AddRange( GameObject.FindGameObjectsWithTag(SpawnTags[i]) );

		for(int i = 0; i < size; i++) // i = size*2/3
		{
			int index = Random.Range(0, mSpawnPoints.Count);
			Vector3 dest = mSpawnPoints[index].transform.position;
			
			GameObject model = PersonPrefabs[i];
			
			GameObject newPerson = GameObject.Instantiate(model, dest, Quaternion.identity) as GameObject;
			CreateNavMeshAgent(newPerson);
			NavigateSubway navSub = newPerson.AddComponent<NavigateSubway>();
			navSub.AvailableStations = new string[]{"S1","S2","S3"};
			
			// random speed
			float randomizer = 1f + Random.Range(-SpeedRnd, SpeedRnd);
			newPerson.GetComponent<UnityEngine.AI.NavMeshAgent>().speed *= randomizer;
			
			newPerson.SendMessage("SetStartPoint", mSpawnPoints[index],SendMessageOptions.DontRequireReceiver );
			
			//newPerson.animation[WALK_ANIM].speed *= randomizer;
		}
	}
}