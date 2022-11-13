using UnityEngine;
using System.Collections;

public class BusAi : CarAi {
	
	float stopTime;
	bool atStop = false;
	// Use this for initialization
	override public void Start () {
		base.Start();
	}
	
	override public void Update ()
	{
		int mask = target.mTypeMask;
			
		if( (mask & (int)StreetWaypoint.eWayPointType.BusStop) > 0 )
		{
			if( !atStop )
			{
				atStop = true;
				stopTime = Time.time;
			}
			if( Time.time - stopTime < 10.0f )
			{
				updateWheels();
				return;
			}
			atStop = false;
		}
		base.Update();
	}
	override protected void pickNewTarget()
	{
		if(target != null)
		{
			target.removeCar(this);
		}
		StreetWaypoint wp = target.getNextWaipointWithMask( (int)StreetWaypoint.eWayPointType.BusStop );
		if( wp == null)
		{
			wp = target.getNextWaipointWithMask( (int)StreetWaypoint.eWayPointType.Bus );
		}
		target = wp;
		if(target != null)
		{
			target.addCar(this);
		}
	}
}
