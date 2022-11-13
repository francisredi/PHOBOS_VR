using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pasillo{

    // this class sucks, could inherit from subte's one.
    public class WarpPlayerAfterLoadPassage : MonoBehaviour {
	
	    public GameObject RoomSpawn;
	
	    void Start () {
	
		    GameObject comesFrom = GameObject.FindGameObjectWithTag("LevelWarp");
		    if( comesFrom == null )
		    {
			    this.enabled = false;
			    return;
		    }
		

		    LevelWarp from = comesFrom.GetComponent<LevelWarp>();
		
		    if( from.Target == "Room" )
		    {
			    transform.position = RoomSpawn.transform.position;	
			    transform.forward = RoomSpawn.transform.right;
		    }
		
		    GameObject.Destroy(comesFrom);
			
		    this.enabled = false;
	    }
    }
}
