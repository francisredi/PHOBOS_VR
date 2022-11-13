using UnityEngine;
using System.Collections;

public class ActivateCollider : MonoBehaviour {

	void Activate()
	{
		GetComponent<Collider>().enabled = true;
	}
	
	void Deactivate()
	{
		GetComponent<Collider>().enabled = false;
	}
}
