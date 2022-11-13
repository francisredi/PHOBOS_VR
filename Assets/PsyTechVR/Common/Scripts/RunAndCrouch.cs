using UnityEngine;
using System.Collections;

public class RunAndCrouch : MonoBehaviour {

	public float walkSpeed = 7; // regular speed
	public float crchSpeed = 3; // crouching speed
	public float runSpeed = 20; // run speed
	
	private CharacterMotor chMotor;
	private Transform tr;
	private float dist; // distance to ground
	private Rewired.Player input;
	private float vScaleHeight;
	private float speed;
	private float vScale;

	// Use this for initialization
	void Start () {
		chMotor = GetComponent<CharacterMotor>();
		input = Rewired.ReInput.players.GetPlayer(0);
		tr = transform;
		vScaleHeight = tr.localScale.y;
		CharacterController ch = GetComponent<CharacterController>();
		dist = ch.height/2; // calculate distance to ground
	}
	
	// Update is called once per frame
	void Update(){
		
		vScale = vScaleHeight; // originalHeight
		speed = walkSpeed;
		
		if (chMotor.grounded && input.GetButton("Run")){
			speed = runSpeed;
		}
		if (input.GetButton("Crouch")){ // crouch
			vScale = vScaleHeight/2.0f;
			speed = crchSpeed; // slow down when crouching
		}
		chMotor.movement.maxForwardSpeed = speed; // set max speed
		float ultScale = tr.localScale.y; // crouch/stand up smoothly
		float y = Mathf.Lerp(tr.localScale.y, vScale, 5*Time.deltaTime);
		//tr.localScale.y = y;
		tr.localScale = new Vector3 (tr.localScale.x,y,tr.localScale.z);
		//tr.position.y += dist * (y-ultScale); // fix vertical position
		y = tr.position.y + (dist * (y-ultScale)); // fix vertical position
		tr.position = new Vector3 (tr.position.x, y, tr.position.z);
	}
}
