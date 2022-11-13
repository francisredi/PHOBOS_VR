using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	public LayerMask m_LayerForRayCast;

	// Use this for initialization
	void Start () {
		//m_LayerForRayCast.value = Mathf.Pow(2,LayerMask.NameToLayer("City"));
		//m_LayerForRayCast = LayerMask.NameToLayer("City");
		m_LayerForRayCast.value = 1 << LayerMask.NameToLayer("City");
	}
	
	// Update is called once per frame
	void Update () {

		Ray ray = new Ray(transform.position, transform.right);
		
		RaycastHit hit;
		
		if (Physics.Raycast (ray,out hit,20.0f, m_LayerForRayCast.value)){ // ray cast to destination
			print(hit.distance);
			//Debug.DrawRay(transform.position,ray);
			Debug.DrawLine(transform.position,hit.point);
		}
	
	}
}
