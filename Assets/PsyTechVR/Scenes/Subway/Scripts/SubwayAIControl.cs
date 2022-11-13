using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SubwayAIControl : MonoBehaviour {
	
	private List<NavigateSubway> mPersons;
	private Dictionary<string, StationCollision> mStations;
	
	void Awake() {
		mPersons = new List<NavigateSubway>();
		mStations = new Dictionary<string, StationCollision>();
	}
	
	public void Register( NavigateSubway person )
	{
		mPersons.Add( person );
	}

	public void Register( StationCollision stationCollision)
	{
		mStations[stationCollision.Station] = stationCollision;
	}

	public List<NavigateSubway> getPersonsOnSameSideInStation( string side, string station ){ // Francis added
		// filter out the people is not on the same side and station
		List<NavigateSubway> aux = new List<NavigateSubway>( mPersons );
		aux.RemoveAll( delegate(NavigateSubway obj) {
			return obj.GetStation() != station || obj.GetSide().ToString() != side;
		});
		return aux;
	}
	
	
	public void ReachedStation( SubwayCoach subway, string station )
	{	
		// filter out the people is not on the same side and station that the subway
		// and the ones that are too far away from it, or waiting.
		List<NavigateSubway> aux = new List<NavigateSubway>( mPersons );
		aux.RemoveAll( delegate(NavigateSubway obj) {
			return obj.GetStation() != station || obj.GetSide().ToString() != subway.Side ||
					!obj.CanGetIntoSubway(subway.wagons);
		});
		
		foreach( NavigateSubway person in aux )
		{
			person.GetIntoSubwayCar( subway.wagons );
			subway.passengers.Add( person );
		}
		
		mStations[station].DisableCollisions( subway.Side );
	}
	
	public void LeftStation( SubwayCoach subway, string station )
	{
		mStations[station].EnableCollisions( subway.Side );
	}
}
