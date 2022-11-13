using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Require a character controller to be attached to the same game object
[RequireComponent(typeof(CharacterMotor))]
[AddComponentMenu("Character/FPS Input Controller")]

public class FPSInputController : MonoBehaviour
{
    private CharacterMotor motor;
	private Rewired.Player input;

    public GameObject centerEye;

    // Use this for initialization
    void Awake()
    {
        motor = GetComponent<CharacterMotor>();
		input = Rewired.ReInput.players.GetPlayer(0);
    }

	void DisableFPSController(bool value)
	{
		enabled = value;
	}

    // Update is called once per frame
    void Update()
    {
        // Get the input vector from keyboard or analog stick
        Vector3 directionVector = new Vector3(input.GetAxis("Joy X"), 0, input.GetAxis("Joy Y"));

        if (directionVector != Vector3.zero)
        {
            // Get the length of the directon vector and then normalize it
            // Dividing by the length is cheaper than normalizing when we already have the length anyway
            float directionLength = directionVector.magnitude;
            directionVector = directionVector / directionLength;

            // Make sure the length is no bigger than 1
            directionLength = Mathf.Min(1.0f, directionLength);

            // Make the input vector more sensitive towards the extremes and less sensitive in the middle
            // This makes it easier to control slow speeds when using analog sticks
            directionLength = directionLength * directionLength;

            // Multiply the normalized direction vector by the modified length
            directionVector = directionVector * directionLength;
        }

        Quaternion combinedRotation = transform.rotation;
        combinedRotation *= Quaternion.Euler(0, centerEye.transform.localEulerAngles.y, 0);

        // Apply the direction to the CharacterMotor
        //motor.inputMoveDirection = transform.rotation * directionVector;
        motor.inputMoveDirection = combinedRotation * directionVector;
        motor.inputJump = input.GetButton("Jump");
    }

    void OnCollisionEnter(Collision collision)
    {
        float mass = collision.rigidbody.mass;
        transform.parent.GetComponent<Rigidbody>().AddForceAtPosition(Vector3.Scale(collision.relativeVelocity, new Vector3(mass, mass, mass)), collision.rigidbody.position);

    }

    // this script pushes all rigidbodies that the character touches
    float pushPower = 2.0f;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody; // no rigidbody
        if (body == null || body.isKinematic) { return; } // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3) { return; } // Calculate push direction from move direction, // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z); // If you know how fast your character is trying to move then you can also multiply the push velocity by that.
                                                                                    // Apply the push
        body.velocity = pushDir * pushPower;
    }

}