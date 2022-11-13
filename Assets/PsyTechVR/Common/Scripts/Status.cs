using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour {

	private bool waitHere;
	public bool straightTraffic;
	public GameObject trafficLight;
	private Semaphore lightStatus;

	private Semaphore.EState prevStatus;
	public float time_started;

	// Use this for initialization
	void Start () {
		//lightStatus = trafficLight.GetComponent<Semaphore>();
		//prevStatus = lightStatus.GetState();
		time_started = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

		if(lightStatus == null){
			lightStatus = trafficLight.GetComponent<Semaphore>();
			return;
		}

		if(lightStatus.GetState() != prevStatus) time_started = Time.time;

		if(!straightTraffic){
			if (lightStatus.GetState () == Semaphore.EState.Green){ // if green, proceed status
				waitHere = false;
			}
			else { // if red or yellow, must stop group agent
				waitHere = true;
			}
		}
		else{ // if not straight direction, the opposite is true
			if (lightStatus.GetState () == Semaphore.EState.Green){ // if green, stop group agent
				waitHere = true;
			}
			else { // if red or yellow, proceed
				waitHere = false;
				time_started = Time.time;
			}
		}

		prevStatus = lightStatus.GetState();
	}

	public bool WaitAtCrossWalk(){
		return waitHere;
	}
}
