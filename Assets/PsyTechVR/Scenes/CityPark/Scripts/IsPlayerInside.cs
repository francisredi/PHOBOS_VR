using UnityEngine;
using System.Collections;

public class IsPlayerInside : MonoBehaviour {
	
	public bool PlayerIsInside;
	
	void Awake () {
		PlayerIsInside = false;
	}
	
	void OnTriggerEnter (Collider other )
	{	
		GameObject character = other.gameObject;
		if(character.tag != "Player")
			return;
		
		PlayerIsInside = true;
	}
	
	void OnTriggerExit (Collider other )
	{
		GameObject character = other.gameObject;
		if(character.tag != "Player")
			return;
		
		PlayerIsInside = false;
	}
}
