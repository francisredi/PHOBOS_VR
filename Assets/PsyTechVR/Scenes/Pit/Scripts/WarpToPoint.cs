using UnityEngine;
using System.Collections;

public class WarpToPoint : MonoBehaviour {
    public Transform warpTransform;
    public GameObject warpWhat;
    public string buttonControl = "B";
    private Rewired.Player mInput;

    // Use this for initialization
    void Start () {
        mInput = Rewired.ReInput.players.GetPlayer(0);
    }
	
	// Update is called once per frame
	void Update () {
        if (mInput.GetButton(buttonControl))
        {
            warpWhat.transform.position = warpTransform.position;
        }
	}
}
