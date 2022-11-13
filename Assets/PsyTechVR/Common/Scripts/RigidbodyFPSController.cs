using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
 
public class RigidbodyFPSController : MonoBehaviour {
 
	public float speed = 4.0f;
	public float gravity = 9.8f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 1.0f;
	private bool grounded = false;
	public float sensitivityX = 15.0f;
	public Transform wayptAgent;

	void Awake () {
	    GetComponent<Rigidbody>().freezeRotation = true;
	    GetComponent<Rigidbody>().useGravity = true;
		wayptAgent = GameObject.Find("PlayersWaypointAgent").transform;
	}
 
	void FixedUpdate () {
	    if (grounded) {
			
	        // Calculate how fast we should be moving
	        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	        targetVelocity = transform.TransformDirection(targetVelocity);
	        targetVelocity *= speed;
 
	        // Apply a force that attempts to reach our target velocity
	        Vector3 velocity = GetComponent<Rigidbody>().velocity;
	        Vector3 velocityChange = (targetVelocity - velocity);
	        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
	        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
	        velocityChange.y = 0;
			
	        GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);
			//wayptAgent.Translate(targetVelocity*0.1f);
 
	        // Jump
	        if (canJump && Input.GetButton("Jump")) {
	            GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
	        }
	    }
 
	    // We apply gravity manually for more tuning control
	    //rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
 
	    grounded = false;
	}
 
	void OnCollisionStay () {
	    grounded = true;    
	}
 
	float CalculateJumpVerticalSpeed () {
	    // From the jump height and gravity we deduce the upwards speed 
	    // for the character to reach at the apex.
	    return Mathf.Sqrt(2 * jumpHeight * gravity);
	}
}
