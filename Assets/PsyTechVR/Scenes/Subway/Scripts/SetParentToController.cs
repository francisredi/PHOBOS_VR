using UnityEngine;
using System.Collections;

public class SetParentToController : MonoBehaviour {

	public GameObject[] AvailableWagons;
	
	void Start () {
	
	}

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        const float MIN_VERTICAL_Y = 0.9f; // our normal needs to be at least as this vertical to be considered 'ground'
        if (hit.normal.y < MIN_VERTICAL_Y)
            return;

        foreach (GameObject wagon in AvailableWagons)
            if (wagon == hit.transform.parent.gameObject)
            {
                if( transform.parent != wagon.transform.parent.transform) // do not change parent! bug fix
                	transform.parent = wagon.transform.parent.transform; // do not change parent! bug fix
                return;
            }

        if(transform.parent != null) transform.parent = null;
    }

}
