using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StreetWaypoint : MonoBehaviour {
	
	public enum eWayPointType 
	{ 
		Cars = 1,
		Bus = 2,
		BusStop = 4
	}
	
	public int mTypeMask { get;set;}
	public bool isBusPath = false;
	public bool isCarPath = true;
	public bool isBusStop = false;
	public StreetWaypoint[] connectedWaypoints;
	public Semaphore semaphoreControl = null;
	
	private List<CarAi> mCarsOnWp = new List<CarAi>();
	// Use this for initialization
	void Start () {
		if( isBusPath) mTypeMask |= (int)eWayPointType.Bus;
		if( isCarPath) mTypeMask |= (int)eWayPointType.Cars;
		if( isBusStop) mTypeMask |= (int)eWayPointType.BusStop;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDrawGizmos() 
	{
		if(connectedWaypoints == null ||connectedWaypoints.Length == 0)
			return;
		
		Gizmos.color = Color.cyan;
		foreach(StreetWaypoint wp in connectedWaypoints)
			Gizmos.DrawLine(transform.position, wp.transform.position);
	}
	
	public StreetWaypoint getNextWaipointWithMask(int mask)
	{
		List<StreetWaypoint> randomList = new List<StreetWaypoint>();
		
		for( int i = 0; i < connectedWaypoints.Length; i++)
		{
			if( (connectedWaypoints[i].mTypeMask & mask) > 0 )
			{
				randomList.Add(connectedWaypoints[i]);
			}
		}
		if( randomList.Count == 0) return null;
		int carsInwp = 1000000;
		StreetWaypoint freeWp = null;
		foreach( StreetWaypoint wp in randomList )
		{
			if( wp.mCarsOnWp.Count < carsInwp )
			{
				carsInwp = wp.mCarsOnWp.Count;
				freeWp = wp; 
			}
		}
		if(freeWp != null) return freeWp;
		int random = Random.Range(0, randomList.Count );
		return randomList[random];
	}
	
	public bool isSemaphoreOk()
	{
		if(semaphoreControl == null) return true;
		
		return semaphoreControl.GetState() == Semaphore.EState.Green;
	}
	
	public void addCar(CarAi car)
	{
		mCarsOnWp.Add(car);
	}
	
	public void removeCar(CarAi car)
	{
		mCarsOnWp.Remove(car);
	}
}
