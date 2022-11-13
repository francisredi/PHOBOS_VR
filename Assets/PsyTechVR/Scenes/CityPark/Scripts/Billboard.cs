using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Camera.current == null)
			return;
			
		transform.up = (transform.position - Camera.current.transform.position).normalized; // just because our quad has its forward pointing upwards.
	}
}
