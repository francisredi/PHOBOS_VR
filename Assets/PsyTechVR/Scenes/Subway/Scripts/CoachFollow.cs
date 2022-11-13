using UnityEngine;
using System.Collections;

public class CoachFollow : MonoBehaviour {
	
	public SubwayWaypoint target;
	public GameObject nose;

	
	// to make sure that the update occurs in right order.
	public void CustomUpdate () {
		Vector3 targetPos = target.transform.position;
		//Debug.Log ( "my " + targetPos + " local:" + transform.position);
		float len = - nose.transform.InverseTransformPoint( targetPos).z;
		//bool test = nose.transform.InverseTransformPoint( targetPos).z > 0;
		//Debug.Log( distance );
		len *= 0.9f;
		transform.Translate( transform.forward * len, Space.World );
		transform.LookAt( targetPos );

			
		
		//transform.position = targetPos;
	}
}
