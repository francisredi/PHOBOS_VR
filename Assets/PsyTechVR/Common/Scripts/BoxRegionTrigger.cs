using UnityEngine;
using System.Collections;

namespace CrowdSim
{
	public class BoxRegionTrigger : MonoBehaviour {
		
		private ArrayList pedestrians = new ArrayList();
		private GameObject cacheGA;

		// Use this for initialization
		void Start () {
			cacheGA = transform.parent.gameObject;
		}
		
		// Update is called once per frame
		void Update () {
			if(this.GetComponentInChildren<TextMesh>() != null) this.GetComponentInChildren<TextMesh>().text = pedestrians.Count.ToString();
		}
		
		public int numPedestriansInRegion(){
			return 0;
			//if(this.GetComponentInChildren<TextMesh>() != null) this.GetComponentInChildren<TextMesh>().text = pedestrians.Count.ToString();
			//return pedestrians.Count;
		}
		
		void OnTriggerEnter(Collider other){
			
			if(other.isTrigger) return;
			
			// check if collider is from pedestrian
			if(other.gameObject.tag != "Pedestrian") return;
			
			// check if own group
			if(cacheGA != other.GetComponent<NavigateCity>().assignedGA){
				if(!pedestrians.Contains(other.gameObject)){
					pedestrians.Add(other.gameObject);
				}
			}
		}
		
		void OnTriggerExit(Collider other){
			
			if(other.isTrigger) return;
			
			// check if collider is from pedestrian
			if(other.gameObject.tag != "Pedestrian") return;
			
			if(pedestrians.Contains(other.gameObject)){
				pedestrians.Remove(other.gameObject);
			}
		}
	}
}
