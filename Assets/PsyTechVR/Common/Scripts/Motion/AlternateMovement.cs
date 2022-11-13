using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)

public class AlternateMovement : MonoBehaviour {

	public float joysensitivityX = 0.05F;
	public float joysensitivityY = 0.05F;

	private Rewired.Player input;

    void Awake() {
        input = Rewired.ReInput.players.GetPlayer(0);
	}
	
	void Update ()
	{
		float Xon = Mathf.Abs (input.GetAxis ("Joy X"));
		float Yon = Mathf.Abs (input.GetAxis ("Joy Y"));
		/*float rotationX = 0.0f;
		
		if (axes == RotationAxes.MouseXAndY)
		{
			if (Xon>.05 || Yon>.05){
				rotationX = transform.localEulerAngles.y + input.GetAxis("Joy X") * joysensitivityX;
				rotationY += input.GetAxis("Joy Y") * joysensitivityY;
			}
			else{
				rotationX = transform.localEulerAngles.y + input.GetAxis("Mouse X") * mousesensitivityX;
				rotationY += input.GetAxis("Mouse Y") * mousesensitivityY;
			}
			
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{*/
			if (Xon>.05){ // side stepping
				//transform.Rotate(0, input.GetAxis("Joy X") * joysensitivityX, 0);
				transform.Translate(input.GetAxis("Joy X") * joysensitivityX, 0, 0);
			}

			if (Yon>.05){ // move straight back or forward
				transform.Translate(0, 0, input.GetAxis("Joy Y") * joysensitivityY);
			}
		/*}
		else
		{
			if (Yon>.05){
				rotationY += input.GetAxis("Joy Y") * joysensitivityY;
			}
			else rotationY += input.GetAxis("Mouse Y") * mousesensitivityY;
			
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}*/
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}
}
