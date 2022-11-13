using UnityEngine;
using System.Collections;

public class SubwayWaypoint : MonoBehaviour {
	
	public string Station = "";
	public SubwayWaypoint[] connectedWaypoints;
	
	public bool IsStation { get { return Station != ""; } }
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnDrawGizmos() 
	{
		if(connectedWaypoints == null ||connectedWaypoints.Length == 0)
			return;
		
		Gizmos.color = Color.cyan;
		foreach(SubwayWaypoint wp in connectedWaypoints)
			Gizmos.DrawLine(transform.position, wp.transform.position);
	}
}
