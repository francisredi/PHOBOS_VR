using UnityEngine;
using System.Collections;

public class BusStopNode : MonoBehaviour {

	public ArrayList pedestrians;
	public float lastAddTime = 0.0f;

	// Use this for initialization
	void Start () {
		pedestrians = new ArrayList();
	}

	public bool AddPedestrian(GameObject pedestrian){
		if((pedestrians.Count > 3) || (lastAddTime != 0.0f && Time.fixedTime-lastAddTime > 5.0f)) return false; // do not add if after 5 seconds passed since last update
		pedestrians.Add (pedestrian);
		lastAddTime = Time.fixedTime;
		pedestrian.GetComponent<UnityEngine.AI.NavMeshAgent> ().SetDestination(transform.position);
		return true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
