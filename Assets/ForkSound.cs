using UnityEngine;
using System.Collections;

public class ForkSound : MonoBehaviour {

	public AudioSource audio;
	public AudioClip forkSoundClip;


	void OnCollissionEnter (Collision col) {

		if (col.gameObject.name == "Fork" && col.gameObject.tag == "Prop") {
							
				//GetComponent<AudioSource> () = GetComponent<AudioSource> ();
				//GetComponent<AudioSource> ().Play ();
			AudioSource audio = GetComponent<AudioSource>();
			audio.Play ();

			}
		}

	}