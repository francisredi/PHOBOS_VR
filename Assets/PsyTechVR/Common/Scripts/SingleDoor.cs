using UnityEngine;
using System.Collections;

public class SingleDoor : MonoBehaviour {

	//Make an empty game object and call it "Door"
	//Rename your 3D door model to "Body"
	//Parent a "Body" object to "Door"
	//Make sure thet a "Door" object is in left down corner of "Body" object. The place where a Door Hinge need be
	//Add a box collider to "Door" object and make it much bigger then the "Body" model, mark it trigger
	//Assign this script to a "Door" game object that have box collider with trigger enabled
	//Make sure the main character is tagged "player"

	public string Msg = "Press 'A' to open/close the door";
	
	// Smoothly open a door
	float smooth = 2.0f;
	public float DoorOpenAngle = 90.0f;
	private bool open;
	private bool enter;
	
	private Vector3 defaultRot;
	private Vector3 openRot;
	private Rewired.Player mInput;

	void Awake()
	{
		mInput = Rewired.ReInput.players.GetPlayer(0);
	}

	// Use this for initialization
	void Start () {
		defaultRot = transform.eulerAngles;
		openRot = new Vector3 (defaultRot.x, defaultRot.y + DoorOpenAngle, defaultRot.z);
	}

	public bool isOpen(){
		return open;
	}
	
	// Update is called once per frame
	void Update () {
		if(open){
			//Open door
			transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
		}else{
			//Close door
			transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaultRot, Time.deltaTime * smooth);
		}
		
		/*if(mInput.GetButtonDown("A") && enter){
			open = !open;
			if(GetComponent<AudioSource>())
				GetComponent<AudioSource>().Play();
		}*/
	}

	//Activate the Main function when player is near the door
	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "Player") {
			enter = true;
			SceneGUI.Instance.addRenderCallback( onDisplay );
		}
	}

	//Deactivate the Main function when player is going away from door
	void OnTriggerExit (Collider other){
		if (other.gameObject.tag == "Player") {
			enter = false;
			SceneGUI.Instance.removeRenderCallback( onDisplay );
		}
	}

	void onDisplay(bool isStereo)
	{
		SceneGUI.Instance.drawText(300, 300, 600, 60,ref  Msg, Color.white);
	}

    public void OnPointerClick()
    {
        open = !open;
        if (GetComponent<AudioSource>())
            GetComponent<AudioSource>().Play();
    }
}