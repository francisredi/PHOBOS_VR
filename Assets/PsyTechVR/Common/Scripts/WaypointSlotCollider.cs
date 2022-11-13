using UnityEngine;
using System.Collections;

namespace CrowdSim
{
	public class WaypointSlotCollider : MonoBehaviour {
		
		int slotNumber;

		// Use this for initialization
		void Start () {
			slotNumber = int.Parse(transform.gameObject.name);
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		
		void OnTriggerEnter(Collider other){
			
			if(other.isTrigger) return;
			
			if(other.gameObject.tag == "Pedestrian"){
				if(other.gameObject.GetComponent<NavigateCity>().assignedGA == transform.parent.gameObject
					/*&& other.gameObject.GetComponent<Navigate>().assignedSlotNumber == slotNumber*/){
					//other.gameObject.GetComponent<Navigate>().stopped = true;
					/////other.gameObject.GetComponent<Navigate>().SetTarget(other.gameObject.transform);
				}
			}
		}
		
		void OnTriggerExit(Collider other){
			
			if(other.isTrigger) return;
			
			if(other.gameObject.tag == "Pedestrian"){
				if(other.gameObject.GetComponent<NavigateCity>().assignedGA == transform.parent.gameObject
					/*&& other.gameObject.GetComponent<Navigate>().assignedSlotNumber == slotNumber*/){
					//other.gameObject.GetComponent<Navigate>().stopped = false;
				}
			}
		}
	}
}
