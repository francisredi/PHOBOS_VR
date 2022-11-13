using UnityEngine;
using System.Collections;

public class IntersectionTriggers : MonoBehaviour {
	
	private int id; // Going 1) North, 2) South, 3) West, 4) East

	// Use this for initialization
	void Start () {
		if(name == "GoingEastTrigger") id = 4;
		else if(name == "GoingWestTrigger") id = 3;
		else if(name == "GoingSouthTrigger") id = 2;
		else id = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other){
		
		// Original version
		/*if(!CrowdSimulation.cornerTurn) return;
		GameObject car = other.GetComponent<Navigate>().assignedCar;
		
		if(car != null && car.GetComponent<WaypointAgent>().destination == 0){ // only do logic if no destination assigned
			// check if car and pedestrian are going in similar directions
			//Vector3 forward = transform.TransformDirection(Vector3.forward); // transform direction from local space to world space
			//Vector3 toOther = other.transform.position - transform.position;
			if(Vector3.Dot(transform.forward.normalized,car.transform.forward.normalized) < 0.9) return; // if not in same direction, do nothing
			
			switch(id)
			{
			case 1: // going north
				//if(car.transform.position.z > other.transform.position.z){ // car must be farther north than pedestrian
					switch (Random.Range(1, 4))
		            {
		                case 1: // go west
							car.GetComponent<WaypointAgent>().findClosestWaypoint(3,"CR3/1","CR33/1","CRC/44","CR333/0","CR3333/0","CR3/12","CR33/14","CRC/30","CR333/17","CR3333/17");
		                    break;
		                case 2: // go east
							car.GetComponent<WaypointAgent>().findClosestWaypoint(4,"CR4/1","CR44/1","CRC/44","CR444/0","CR4444/0","CR4/12","CR44/14","CRC/16","CR444/17","CR4444/17");
		                    break;
		                case 3: // go north
		                    car.GetComponent<WaypointAgent>().destination = 1;
		                    break;
		            }
				//}
				break;
			case 2: // going south
				//if(car.transform.position.z < other.transform.position.z){ // car must be farther south than pedestrian
					switch (Random.Range(1, 4))
		            {
		                case 1: // go west
							car.GetComponent<WaypointAgent>().findClosestWaypoint(3,"CR2/1","CR22/1","CRC/1","CR222/0","CR2222/0","CR2/12","CR22/14","CRC/30","CR222/17","CR2222/17");
		                    break;
		                case 2: // go east
							car.GetComponent<WaypointAgent>().findClosestWaypoint(4,"CR1/1","CR11/1","CRC/1","CR111/0","CR1111/0","CR1/12","CR11/14","CRC/16","CR111/17","CR1111/17");
		                    break;
		                case 3: // go south
		                    car.GetComponent<WaypointAgent>().destination = 2;
		                    break;
		            }
				//}
				break;
			case 3: // going west
				//if(car.transform.position.x < other.transform.position.x){ // car must be farther west than pedestrian
					switch (Random.Range(1, 4))
		            {
		                case 1: // go north
							car.GetComponent<WaypointAgent>().findClosestWaypoint(1,"CR1/12","CR11/14","CRC/16","CR111/17","CR1111/17","CR1/1","CR11/1","CRC/1","CR111/0","CR1111/0");
		                    break;
		                case 2: // go south
							car.GetComponent<WaypointAgent>().findClosestWaypoint(2,"CR4/12","CR44/14","CRC/16","CR444/17","CR4444/17","CR4/1","CR44/1","CRC/44","CR444/0","CR4444/0");
		                    break;
		                case 3: // go west
		                    car.GetComponent<WaypointAgent>().destination = 3;
		                    break;
		            }
				//}
				break;
			case 4: // going east
				//if(car.transform.position.x > other.transform.position.x){ // car must be farther east than pedestrian
					switch (Random.Range(1, 4))
		            {
		                case 1: // go north
							car.GetComponent<WaypointAgent>().findClosestWaypoint(1,"CR2/12","CR22/14","CRC/30","CR222/17","CR2222/17","CR2/1","CR22/1","CRC/1","CR222/0","CR2222/0");
		                    break;
		                case 2: // go south
							car.GetComponent<WaypointAgent>().findClosestWaypoint(2,"CR3/12","CR33/14","CRC/30","CR333/17","CR3333/17","CR3/1","CR33/1","CRC/44","CR333/0","CR3333/0");
		                    break;
		                case 3: // go east
		                    car.GetComponent<WaypointAgent>().destination = 4;
		                    break;
		            }
				//}
				break;
			}
		}*/
	}
}
