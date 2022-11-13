using UnityEngine;
using System.Collections;

public class BillboardPlay : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		MovieTexture movie = GetComponent<Renderer>().material.mainTexture as MovieTexture;
		movie.Play ();
		movie.loop = true;
	
	}
}
