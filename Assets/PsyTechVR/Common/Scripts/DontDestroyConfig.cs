using UnityEngine;
using System.Collections;

public class DontDestroyConfig : MonoBehaviour {

	public static DontDestroyConfig i;
	
	/*void Awake () {
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag(gameObject.tag);
		*//*if(gos.Length > 1) {
			Destroy(gameObject);
		}
		else*//* if(gos.Length == 1){ // if only one, don't delete
			DontDestroyOnLoad (gameObject);
		}
	}*/

	void Awake(){
		if (i == null) {
			i = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy(this);
		}
	}

}
