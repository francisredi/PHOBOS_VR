using UnityEngine;
using System.Collections;

public class Movie : MonoBehaviour {

	public MovieTexture movie;
	public bool loop;

		void Start () {
			((MovieTexture)GetComponent<Renderer> ().material.mainTexture).Play ();
		movie.loop = true;
		}

}