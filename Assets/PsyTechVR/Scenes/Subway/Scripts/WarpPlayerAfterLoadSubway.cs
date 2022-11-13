using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Subte {

public class WarpPlayerAfterLoadSubway : MonoBehaviour {
		
	public GameObject S1Spawn;
	public GameObject S2Spawn;
	public GameObject S3Spawn;
		
	void Start () {
			
		GameObject comesFrom = GameObject.FindGameObjectWithTag("LevelWarp");
		if( comesFrom == null )
		{
			this.enabled = false;
			return;
		}
			
		// only one of these will exist at once.. to lazy to code it right.
		List<GameObject> warpRefs = new List<GameObject>();
		warpRefs.Add(S1Spawn);
		warpRefs.Add(S2Spawn);
		warpRefs.Add(S3Spawn);
			
		LevelWarp from = comesFrom.GetComponent<LevelWarp>();
		if( from.Target == "S1" )
		{
			transform.position = S1Spawn.transform.position;
			transform.forward = S1Spawn.transform.forward;
			warpRefs.Remove(S1Spawn);
		}
		else if( from.Target == "S2" )
		{	
			transform.position = S2Spawn.transform.position;	
			transform.forward = S2Spawn.transform.forward;
			warpRefs.Remove(S2Spawn);
		}
		else if( from.Target == "S3" )
		{	
			transform.position = S3Spawn.transform.position;	
			transform.forward = S3Spawn.transform.forward;
			warpRefs.Remove(S3Spawn);
		}
			
		GameObject.Destroy( comesFrom );
		
		// enabling remaining exits
		foreach(GameObject warpRef in warpRefs)
		{			
			warpRef.GetComponent<LevelWarp>().enabled = true;
			warpRef.GetComponent<EnableExit>().enabled = false;
		}
			
		this.enabled = false;
	}

}
}
