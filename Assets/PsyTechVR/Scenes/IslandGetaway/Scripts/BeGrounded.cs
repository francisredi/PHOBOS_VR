using UnityEngine;
using System.Collections;

public class BeGrounded : MonoBehaviour {

	public GameObject leftToe;
	public GameObject rightToe;
	public GameObject pelvis;

	Vector3 originalPos;
	Quaternion originalRot;
	Animator animator;
	float prevPlayBackTime = 1.0f;

	static int yogaState = Animator.StringToHash("Base.Yoga");
	static int gangnamState = Animator.StringToHash("Base.Gangnam Style");

	// Use this for initialization
	void Start () {
		originalPos = transform.position;
		originalRot = transform.rotation;
		animator = transform.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update()
	{
		AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0); // animation layer
		float playbackTime = currentState.normalizedTime % 1;

		if(prevPlayBackTime > playbackTime){ // reset
			//print(playbackTime);
			transform.rotation = originalRot;
			transform.position = originalPos;

			if(currentState.fullPathHash == yogaState){
				//print("Yoga"); // play yoga music
			}
			else if(currentState.fullPathHash == gangnamState){
				//print("Gangnam Style"); // play yoga music
			}
		}

		prevPlayBackTime = playbackTime;

		/*//ray starts at player position and points down
		Ray ray = new Ray(transform.position, Vector3.down);
		
		//will store info of successful ray cast
		RaycastHit hitInfo;

		//print ("hello");
		
		//terrain should have mesh collider and be on custom terrain 
		//layer so we don't hit other objects with our raycast
		//LayerMask layer = 1 << LayerMask.NameToLayer("Terrain");

		Vector3 pos = transform.position;

		pos.y += 5.0f;

		transform.position = pos;
		
		//cast ray
		if(Physics.Raycast(ray, out hitInfo, 5.0f))
		{
			print ("hit");
			//get where on the y axis our raycast hit the ground
			float y = hitInfo.point.y;
			
			//copy current position into temporary container
			pos = transform.position;
			
			//change y to where on the y axis our raycast hit the ground
			pos.y = y;
			
			//override our position with the new adjusted position.
			transform.position = pos;
		}*/
	}
}
