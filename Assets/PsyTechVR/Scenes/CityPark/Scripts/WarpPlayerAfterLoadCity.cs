using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// this class sucks, could inherit from subte's one.
public class WarpPlayerAfterLoadCity : MonoBehaviour {

    //public HotelElevator Elevator;
    public GameObject AMVRTowerIndoorElevator;
	public GameObject S1Spawn;
	public GameObject S2Spawn;
	public GameObject S3Spawn;
	public GameObject HallSpawn;
	public GameObject ExtElevatorSpawn;
    private HotelElevator he;


    void Start () {
	
		GameObject comesFrom = GameObject.FindGameObjectWithTag("LevelWarp");
		if( comesFrom == null )
		{
			this.enabled = false;
			return;
		}
		
		// the user could spawn in S3, but that station it is out of the user zone, 
		// so that's why the commented code.
		
		List<GameObject> warpRefs = new List<GameObject>();
		warpRefs.Add(S1Spawn);
		warpRefs.Add(S2Spawn);
		//warpRefs.Add(S3Spawn);
		
		LevelWarp from = comesFrom.GetComponent<LevelWarp>();
		
		if( from.Target == "Elevator" )
		{
            he = AMVRTowerIndoorElevator.GetComponent<HotelElevator>();
            he.InstantGoToFloor(HotelElevator.ROOM_FLOOR);
			he.enabled = false;
			transform.position = he.transform.position + (new Vector3(0.0f,0.0f,0.2f)); // 0.4
            //transform.rotation = Quaternion.identity;
            transform.rotation = Quaternion.LookRotation(he.transform.up,Vector3.up);
			StartCoroutine(DelayedEnableElevator());
		}
		else if (from.Target == "ExtElevator" )
		{
			transform.position = ExtElevatorSpawn.transform.position;	
			transform.forward = ExtElevatorSpawn.transform.right;
		}
		else if (from.Target == "Hall" )
		{
			transform.position = HallSpawn.transform.position;	
			transform.forward = HallSpawn.transform.right;
		}
		else if( from.Target == "S1" )
		{
			transform.position = S1Spawn.transform.position;	
			transform.forward = S1Spawn.transform.right;
			warpRefs.Remove(S1Spawn);
		}
		else if( from.Target == "S2" || from.Target == "S3" )
		{
			transform.position = S2Spawn.transform.position;	
			transform.forward = S2Spawn.transform.right;
			warpRefs.Remove(S2Spawn);
		}
		/*else if( from.Target == "S3")
		{
			transform.position = S3Spawn.transform.position;	
			transform.forward = S3Spawn.transform.right;
			warpRefs.Remove(S3Spawn);
		}*/
		
		GameObject.Destroy(comesFrom);
		
		// enabling remaining exits
		foreach(GameObject warpRef in warpRefs)
		{			
			warpRef.GetComponent<LevelWarp>().enabled = true;
			warpRef.GetComponent<EnableExit>().enabled = false;
		}
		
		this.enabled = false;
	}
		
	IEnumerator DelayedEnableElevator()
	{
		yield return new WaitForSeconds(2f);
		
		he.enabled = true;
	}
}
