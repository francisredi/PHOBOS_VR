using UnityEngine;
using System.Collections;

public class EnableExit : MonoBehaviour {
	
	private GameObject Ref;

    void Awake()
    {
        Ref = GameObject.FindGameObjectWithTag("Player");
    }
		
	void Update () {
		
		if( Utils.GetDist(gameObject, Ref) > GetComponent<LevelWarp>().Dist * 1.5f)
		{
			GetComponent<LevelWarp>().enabled = true;
			this.enabled = false;
		}
		
	}
}
