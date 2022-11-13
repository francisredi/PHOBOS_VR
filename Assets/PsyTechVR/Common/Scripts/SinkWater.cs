using UnityEngine;
using System.Collections;

public class SinkWater : MonoBehaviour {

	private Rewired.Player mInput;
	private bool enter;

	public bool isRunning = false;
	public string Msg = "Press 'A' to turn on/off tap water";

	private ParticleSystem sinkWater;
	public GameObject sinkWaterObject;

	void Awake(){
		mInput = Rewired.ReInput.players.GetPlayer(0);
		sinkWater = sinkWaterObject.GetComponent<ParticleSystem>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

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
		SceneGUI.Instance.drawText (300, 300, 600, 60, ref  Msg, Color.white);
	}

    public void OnPointerClick()
    {
        if (!isRunning)
        {
            sinkWater.Play();
        }
        else
        {
            sinkWater.Stop();
        }
        isRunning = !isRunning;
    }
}
