using UnityEngine;
using System.Collections;

public class PushPullObject : MonoBehaviour {

	private Rewired.Player mInput;
	private bool enter;
	private bool open;
	public string Msg = "Press 'A' to pull/push an object";
	private Vector3 closedPos;
	private Vector3 openPos;
	private bool sleep;
	float smooth = 2.0f;
	public GameObject closetdoorpivot;
	private SingleDoor sdRef;
	public GameObject personAffected;
	private Quaternion originalRotation;
	private Vector3 originalPosition;
    public GameObject rayBlocker;

	void Awake()
	{
		mInput = Rewired.ReInput.players.GetPlayer(0);
		sleep = false;
        rayBlocker.SetActive(false);
    }

	public bool isOpen(){
		return open;
	}

	// Use this for initialization
	void Start () {
		closedPos = transform.position;
		openPos = new Vector3 (transform.position.x, transform.position.y, -1.144f);
		sdRef = closetdoorpivot.GetComponent<SingleDoor>();
	}

	// Update is called once per frame
	void Update () {
		if(!sleep){ // on the floor
			if(sdRef.isOpen()){ // door is open
				if(open){
                    //pull stairs
                    rayBlocker.SetActive(true);
                    transform.position = Vector3.Lerp(transform.position, openPos, Time.deltaTime * smooth);
					Msg = "Press 'Y' to get into or out of bed";
				}else{
                    //push stairs
                    transform.position = Vector3.Lerp(transform.position, closedPos, Time.deltaTime * smooth);
                    if (Vector3.Distance(transform.position,closedPos) < 0.02f)
                    {
                        rayBlocker.SetActive(false);
                    }
				}

                if (enter/*Vector3.Distance(transform.position, openPos) < 0.1f*/)
                {
                    if (mInput.GetButtonDown("Y"))
                    { // go to bed
                        sleep = true;
                        Fade.FadeOut();
                        //personAffected.GetComponent<CharacterController>().radius = 0.15f;
                        //personAffected.GetComponent<OVRPlayerController>().GravityModifier = 0;
                        personAffected.GetComponent<OVRPlayerController>().enabled = false;
                        personAffected.GetComponent<CharacterController>().enabled = false;
                       
                        //personAffected.GetComponent<UserControl>().enabled = false;
                        //personAffected.GetComponent<AlternateMovement>().enabled = false;
                        originalPosition = personAffected.transform.position;
                        originalRotation = personAffected.transform.rotation;
                        
                        personAffected.transform.rotation = Quaternion.LookRotation(Vector3.up, -Vector3.right);
                        personAffected.transform.position = new Vector3(-1.2f, 1.9352f, -2.169f);

                        //personAffected.GetComponent<Animator>().enabled = false;
                        SceneGUI.Instance.removeRenderCallback(onDisplay);
                        Fade.FadeIn();
                    }
                }
            }
		}
		else if (sleep) {
			if(mInput.GetButtonDown("Y")){ // get out of bed
				Fade.FadeOut();
				personAffected.transform.position = originalPosition;
				personAffected.transform.rotation = originalRotation;
                //personAffected.GetComponent<Animator>().enabled = true;
                //personAffected.GetComponent<AlternateMovement>().enabled = true;
                //personAffected.GetComponent<UserControl>().enabled = true;
                
                //personAffected.GetComponent<CharacterController>().radius = 0.3f;
                personAffected.GetComponent<CharacterController>().enabled = true;
                //personAffected.GetComponent<OVRPlayerController>().GravityModifier = 1;
                personAffected.GetComponent<OVRPlayerController>().enabled = true;
                sleep = false;
				Msg = "Press 'A' to pull/push an object";
				Fade.FadeIn();
			}
		}
	
	}

	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "Player" && sdRef.isOpen ()) {
			enter = true;
			SceneGUI.Instance.addRenderCallback( onDisplay );
		}
	}

	//Deactivate the Main function when player is going away from door
	void OnTriggerExit (Collider other){
		if (other.gameObject.tag == "Player" && sdRef.isOpen ()) {
			enter = false;
			SceneGUI.Instance.removeRenderCallback( onDisplay );
		}
	}

	void onDisplay(bool isStereo)
	{
		SceneGUI.Instance.drawText (300, 300, 600, 60, ref  Msg, Color.white);
	}

    public void OnPointerClick()
    {
        if (!sleep)
        {
            open = !open;
            if (GetComponent<AudioSource>())
                GetComponent<AudioSource>().Play();
        }
    }
}
