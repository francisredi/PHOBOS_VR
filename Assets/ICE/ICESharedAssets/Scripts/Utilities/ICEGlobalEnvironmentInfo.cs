// ##############################################################################
//
// ICEGlobalEnvironmentInfo.cs
// Version 1.1.15
//
// © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.ice-technologies.com
// mailto:support@ice-technologies.com
// 
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// ##############################################################################

using UnityEngine;
using System.Collections;
using ICE.Utilities.EnumTypes;
using ICE.Utilities;

public class ICEGlobalEnvironmentInfo : MonoBehaviour {

	//Here is a private reference only this class can access
	private static ICEGlobalEnvironmentInfo m_Instance = null;
	public static ICEGlobalEnvironmentInfo Instance
	{
		get
		{
			//If m_Register hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if( m_Instance == null )
				m_Instance = GameObject.FindObjectOfType<ICEGlobalEnvironmentInfo>();

			if( m_Instance == null )
			{
				GameObject _register = GameObject.Find( "CreatureRegister" );

				if( _register != null )
				{
					_register.SetActive( true );

					m_Instance = GameObject.FindObjectOfType<ICEGlobalEnvironmentInfo>();
				}

			}

			return m_Instance;
		}
	}

	public TemperatureScaleType TemperatureScale;
	public float Temperature;
	public float MinTemperature;
	public float MaxTemperature;

	public int DateDay;
	public int DateMonth;
	public int DateYear;

	public int TimeHour;
	public int TimeMinutes;
	public int TimeSeconds;

	public WeatherType Weather;

	public float WindSpeed;
	public float WindDirection;

	public void UpdateTemperatureScale( TemperatureScaleType _scale )
	{
		if( _scale == TemperatureScaleType.CELSIUS && TemperatureScale == TemperatureScaleType.FAHRENHEIT )
		{
			TemperatureScale = _scale;
			Temperature = GraphicTools.FahrenheitToCelsius( Temperature );
			MinTemperature = GraphicTools.FahrenheitToCelsius( MinTemperature );
			MaxTemperature = GraphicTools.FahrenheitToCelsius( MaxTemperature );

		}
		else if( _scale == TemperatureScaleType.FAHRENHEIT && TemperatureScale == TemperatureScaleType.CELSIUS )
		{
			TemperatureScale = _scale;
			Temperature = GraphicTools.CelsiusToFahrenheit( Temperature );
			MinTemperature = GraphicTools.CelsiusToFahrenheit( MinTemperature );
			MaxTemperature = GraphicTools.CelsiusToFahrenheit( MaxTemperature );
		}
	}
}
