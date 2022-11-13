using UnityEngine;
using System.Collections;

namespace ICE.Utilities.EnumTypes
{
	public enum GroundCheckType
	{
		NONE,
		RAYCAST,
		SAMPLEHEIGHT
	}

	public enum TemperatureScaleType
	{
		CELSIUS,
		FAHRENHEIT
	}
	
	public enum WeatherType
	{
		UNDEFINED = 0,
		FOGGY,
		RAIN,
		HEAVY_RAIN,
		WINDY,
		STORMY,
		CLEAR,
		PARTLY_CLOUDY,
		MOSTLY_CLOUDY,
		CLOUDY
	}
}
