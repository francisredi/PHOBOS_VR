using UnityEngine;
using System.Collections;

public class PushPull : MonoBehaviour {
	
	private Rewired.Player mInput;
	//private bool enter;
	private bool open;
	public string Msg = "Press 'A' to pull/push an object";
	private Vector3 closedPos;
	private Vector3 openPos;
	public float forwardZ = -1.144f;
	float smooth = 2.0f;
	
	void Awake()
	{
		mInput = Rewired.ReInput.players.GetPlayer(0);
	}
	
	public bool isOpen(){
		return open;
	}
	
	// Use this for initialization
	void Start () {
		closedPos = transform.position;
		openPos = new Vector3 (transform.position.x, transform.position.y, forwardZ);
	}
	
	// Update is called once per frame
	void Update () {

		if(open){
			//pull door
			transform.position = Vector3.Lerp(transform.position, openPos, Time.deltaTime * smooth);
		}else{
			//push door
			transform.position = Vector3.Lerp(transform.position, closedPos, Time.deltaTime * smooth);
		}
		
	}
	
	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "Player") {
			//enter = true;
			SceneGUI.Instance.addRenderCallback( onDisplay );
		}
	}
	
	//Deactivate the Main function when player is going away from door
	void OnTriggerExit (Collider other){
		if (other.gameObject.tag == "Player") {
			//enter = false;
			SceneGUI.Instance.removeRenderCallback( onDisplay );
		}
	}
	
	void onDisplay(bool isStereo)
	{
		SceneGUI.Instance.drawText (300, 300, 600, 60, ref  Msg, Color.white);
	}

    public void OnPointerClick()
    {
        open = !open;
        if (GetComponent<AudioSource>())
            GetComponent<AudioSource>().Play();
    }
}
