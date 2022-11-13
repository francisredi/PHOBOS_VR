using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour {
	
	public GameObject[] PersonPrefabs;
	public int Count;
	public float SpeedRnd = 0.25f;
	public string[] SpawnTags;
	
	private List<GameObject> mSpawnPoints;
	
	private static string WALK_ANIM = "Walk";
	
	void Awake()
	{

	}
	
	void Start () {
		
		mSpawnPoints = new List<GameObject>( GameObject.FindGameObjectsWithTag(SpawnTags[0]) );
		for(int i = 1 ; i < SpawnTags.Length; i++)
			mSpawnPoints.AddRange( GameObject.FindGameObjectsWithTag(SpawnTags[i]) ); 
		
		for(int i = 0; i < Count; i++)
		{
			int index = Random.Range(0, mSpawnPoints.Count);
			Vector3 dest = mSpawnPoints[index].transform.position;
			
			GameObject model = PersonPrefabs[ Random.Range(0, PersonPrefabs.Length) ];
			
			GameObject newPerson = GameObject.Instantiate(model, dest, Quaternion.identity) as GameObject;
			float randomizer = 1f + Random.Range(-SpeedRnd, SpeedRnd);
			newPerson.GetComponent<UnityEngine.AI.NavMeshAgent>().speed *= randomizer;
			newPerson.SendMessage("SetStartPoint", mSpawnPoints[index],SendMessageOptions.DontRequireReceiver );
			
			newPerson.GetComponent<Animation>()[WALK_ANIM].speed *= randomizer;
		}
	}
}
