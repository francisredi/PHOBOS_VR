using UnityEngine;
using System.Collections;

public class ObjectRef : MonoBehaviour {
	
	public GameObject Ref;
	
	
	// TODO: REMOVE ME!
	void OnDrawGizmos() 
	{
		if(GetComponent<Collider>() == null || Ref == null)
			return;
		
		Gizmos.color = new Color(0.1f, 1f, 0.1f, 0.5f);
		Gizmos.DrawLine(gameObject.transform.position, Ref.transform.position);
	}
}
