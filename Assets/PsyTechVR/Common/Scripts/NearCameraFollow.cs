using UnityEngine;
using System.Collections;

public class NearCameraFollow : MonoBehaviour {
	
	public static GameObject target;  // Assign this to the character
	float maxDist = 20.0f; // distance before triggering follow
	float minDist = 10.0f; // distance before untriggering follow
	float speed = 6.0f;  // Should be set to the same speed as your character

	//private Transform tran;
	private Vector3 trans;
	private float acc = 0.0f; // Amount of movement on the camera. this starts at 0 and builds until it reaches speed. 

	// Use this for initialization
	void Start () {
		//tran = new GameObject().transform;
		trans = new Vector3();
	}
	
	// Update is called once per frame
	void Update () {
		/*// update character speed
	    if(target.GetComponent(Move) == null){
	        speed = target.GetComponent(MoveCharacter).speed;
	    }
	    else speed = target.GetComponent(Move).speed;*/
	}
	
	void LateUpdate() // Had to be late update becuase it was studdering otherwise.  
	{
	
	    float myDist = 0.0f; // This variable is a (changing) threshhold to determine whether or not our camera should be moving or not. 
	
	    // when the camera is not moving (acc==0), it is asigned to maxDist.
	    // Once the camera starts moving, we set myDist to minDist. This means the camera needs to get even CLOSER to the player before it stops
	    // moving than it took to trigger it in the first place. this behavior does 2 things: 
	    // When the camera is stopped, it lets the character walk around a bit before the cam starts to move.
	    // When the camera is moving, it prevents studdering as the character walks in-and-out of threshold. 
	
	    if (acc == 0.0f) // If the camera is not moving
	    {
	        myDist = maxDist; // Set our threshhold to Max
	    }
	    else // else the camera IS moving
	    {
	        myDist = minDist; // set threshold to min
	    }
	
	    // shallow copy
		trans.x = target.transform.position.x;
	    //tran.position.x = target.transform.position.x;
		trans.y = 5.5f;
	    //tran.position.y = 5.5f;
		trans.z = target.transform.position.z;
	    //tran.position.z = target.transform.position.z;
		//dummy.transform.rotation = target.transform.rotation;
	    //tran.rotation = target.transform.rotation;
	    //transform.LookAt(dummy.transform);
		transform.LookAt(trans); // Aim at the character, not at its feet!
	
	    float idist = Vector3.Distance(transform.position, target.transform.position); // Get distance between the cam and the player
	
	    if(idist > 20.0f){ // If camera is too far
	        acc = idist - 20.0f;
	    }
	    else if (idist > myDist) // If distance beyond our current threshold 
	    {
	        acc = Mathf.Min(acc + 2.0f, speed); // accelerate by a fixed amount (+2) until we're at max speed (speed)
	    }
	    else  // Else the distance is inside our threshold
	    {
	        acc = Mathf.Max(acc - 2.0f, 0.0f);  // deacclerate by (-2) until we're at stopped (0)
	    }
	    
	    if (acc > 0.0f) // If we're moving
	    {
	        transform.Translate(0.0f,0.0f,acc * Time.deltaTime); // Old way without collision
	    }
	}
}
