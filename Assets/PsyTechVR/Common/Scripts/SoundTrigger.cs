using UnityEngine;
using System.Collections;

public class SoundTrigger : MonoBehaviour {

    public new AudioSource audio;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (!audio.isPlaying)
        {
            audio.Play();
        }
    }
}
