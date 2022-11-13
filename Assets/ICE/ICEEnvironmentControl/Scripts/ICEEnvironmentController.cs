// ##############################################################################
//
// ICEEnvironmentController.cs
// Version 1.0
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
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using ICE.Environment;
using ICE.Utilities.EnumTypes;
using ICE.Utilities;



namespace ICE.Environment
{
	public class ICEEnvironmentController : ICEGlobalEnvironmentInfo {

		//Here is a private reference only this class can access
		private static ICEEnvironmentController m_Environment;
		
		//This is the public reference that other classes will use
		public static ICEEnvironmentController Environment
		{
			get
			{
				//If m_Environment hasn't been set yet, we grab it from the scene!
				//This will only happen the first time this reference is used.
				if(m_Environment == null)
				{
					m_Environment = GameObject.FindObjectOfType<ICEEnvironmentController>();
				}
				/*
				// If there is no valid register in the scene we have to create one!
				if( m_Register == null )
				{
					GameObject _register_object = new GameObject();
					_register_object.name = "ICECreatureRegister.Instance";
					_register_object.transform.position = Vector3.zero;
					m_Register = _register_object.AddComponent<ICECreatureRegister>();
					m_Register.Scan();

				}*/
				
				return m_Environment;
			}
		}

		/*
		public TemperatureScaleType TemperatureScale = TemperatureScaleType.CELSIUS;
		public float Temperature = 25;
		public float MinTemperature = -25;
		public float MaxTemperature = 50;*/

		public Text UITextDate = null;
		public Text UITextTime = null;

		public bool UseShortTime = true;

		public float Azimut = 270;
		public float Radius2Max = 1000;
		public float RadiusMax = 1000;

		[SerializeField]
		private float m_Radius = 100;
		public float Radius
		{
			get{ return m_Radius; }
			set{
				m_Radius = value;
				CalcZenitLevel();
			}
		}

		[SerializeField]
		private float m_Zenit = 45;
		public float Zenit
		{
			get{ return m_Zenit; }
			set{
				m_Zenit = value;
				CalcZenitLevel();
			}
		}

		private float m_CalcZenitLevel = 0;
		private void CalcZenitLevel()
		{
			m_CalcZenitLevel = m_Radius * Mathf.Sin( m_Zenit* Mathf.PI/180 ) / Mathf.Sin( 90* Mathf.PI/180 );
		}

		[SerializeField]
		private float m_StartTimeInSecondes = 6*3600;
		public float StartTimeInHours{
			set{ m_StartTimeInSecondes = value*3600; }
			get{ return m_StartTimeInSecondes/3600; }
		}

		[SerializeField]
		private float m_SunriseNormalized = 0.25f;
		public float SunriseHour{
			set{ m_SunriseNormalized = MathTools.Normalize( value, 1, 24 ); }
			get{ return MathTools.Denormalize( m_SunriseNormalized, 1, 24 ); }
		}

		[SerializeField]
		private float m_SunsetNormalized = 0.75f;
		public float SunsetHour{
			set{ m_SunsetNormalized = MathTools.Normalize( value, 1, 24 ); }
			get{ return MathTools.Denormalize( m_SunsetNormalized, 1, 24 ); }
		}

		[SerializeField]
		private float m_DayLengthInSeconds = 1440;
		public float DayLengthInMinutesMax = 10;
		public float DayLengthInMinutes{
			set{ m_DayLengthInSeconds = 86400/(value*60); }
			get{ return (86400/m_DayLengthInSeconds)/60; }
		}

		public bool UseSystemTime = false;
		public float CurrentSecondsOfDay = 0;
		private float m_CurrentSecondsOfDayNormalized{
			get{ return MathTools.Normalize( CurrentSecondsOfDay, 0, 86400 ); }
		}

		public int StartYear = DateTime.Today.Year;
		public int StartMonth = DateTime.Today.Month;
		public int StartDay = DateTime.Today.Day;

		public float Year{
			get{ return CurrentSecondsOfDay/3600; }
		}

		public float Month{
			get{ return (CurrentSecondsOfDay/60)%60; }
		}

		public float Day{
			get{ return (CurrentSecondsOfDay/60)%60; }
		}

		public int DayTotal = 0;

		public float Hour{
			get{ return CurrentSecondsOfDay/3600; }
		}

		public float Minutes{
			get{ return (CurrentSecondsOfDay/60)%60; }
		}

		public float Seconds{
			get{ return (CurrentSecondsOfDay/60)%60; }
		}

		public string TimeToString{
			get{ return string.Format("{0:00}:{1:00}:{2:00}",Hour,Minutes,Seconds); }
		}

		public string ShortTimeToString{
			get{ return string.Format("{0:00}:{1:00}",Hour,Minutes); }
		}



		public Light Sun;

		//public GameObject _test = null;
		// Use this for initialization
		void Start () {
		
			if( UseSystemTime )
				CurrentSecondsOfDay = (float)System.DateTime.Now.TimeOfDay.TotalSeconds;
			else
				CurrentSecondsOfDay = m_StartTimeInSecondes;


			DayTotal = 0;
			m_SunInitialIntensity = Sun.intensity;

			//StartCoroutine( Do() );

			//_test = GameObject.CreatePrimitive( PrimitiveType.Sphere );
		}
		
		// Update is called once per frame
		void Update () {
		
			if( UITextTime != null )
			{
				if( UseShortTime )
					UITextTime.text = ShortTimeToString;
				else
					UITextTime.text = TimeToString;
			}

			UpdateSunPosition();

			//UPDATE GLOABL ENVIRONMENT INFOS
			TimeHour = (int)Hour;
			TimeMinutes = (int)Minutes;
			TimeSeconds = (int)Seconds;

			DateDay = (int)Day;
			DateMonth = (int)Month;
			DateYear = (int)Year;
		}

		void FixedUpdate()
		{
			if( UseSystemTime )
				CurrentSecondsOfDay = (float)System.DateTime.Now.TimeOfDay.TotalSeconds;
			else
				CurrentSecondsOfDay += Time.fixedDeltaTime * m_DayLengthInSeconds;

			if( CurrentSecondsOfDay > 86400 )
			{
				CurrentSecondsOfDay = 0;
				DayTotal++;
			}
		}

		Vector3 _orbit = Vector3.zero;
		float _angle = 0;
		private float m_SunInitialIntensity;	

		private void UpdateSunPosition()
		{
			if( Sun == null )
				return;

			float _angle_2 = (( m_CurrentSecondsOfDayNormalized * 360f ) - Azimut )* (-1);
			float _angle = m_Zenit - 180;

			var qX = Quaternion.AngleAxis( _angle, Vector3.right);
			var qY = Quaternion.AngleAxis(_angle_2, Vector3.up);
			var q = qX * qY;
			//Rotates about the local axis
			Sun.transform.localRotation = q;

			GameObject _orbit = GameObject.Find("Sphere");

			if( _orbit != null )
				m_PathPositions.Add( _orbit.transform.position );


			float intensityMultiplier = Sun.intensity;			
			if( m_CurrentSecondsOfDayNormalized >= m_SunriseNormalized && m_CurrentSecondsOfDayNormalized < m_SunsetNormalized && intensityMultiplier < m_SunInitialIntensity )
				intensityMultiplier = Mathf.MoveTowards( intensityMultiplier,m_SunInitialIntensity, 0.1f * Time.deltaTime );			
			else if( m_CurrentSecondsOfDayNormalized >= m_SunsetNormalized && intensityMultiplier > 0 )
				intensityMultiplier = Mathf.MoveTowards( intensityMultiplier,0, 0.1f * Time.deltaTime );			
			else if( m_CurrentSecondsOfDayNormalized <= m_SunriseNormalized || m_CurrentSecondsOfDayNormalized >= m_SunsetNormalized ) 
				intensityMultiplier = 0;			
			Sun.intensity = m_SunInitialIntensity * intensityMultiplier;
		}

		private void UpdateSunPosition2()
		{
			if( Sun == null )
				return;


			_angle = ( m_CurrentSecondsOfDayNormalized * 360f ) - Azimut;


			_orbit = GraphicTools.GetAnglePosition( Sun.transform.position, _angle, Radius );

			_angle = ( m_SunriseNormalized * 360f ) - ( m_CurrentSecondsOfDayNormalized * 360f );

			_orbit.y = m_CalcZenitLevel * Mathf.Sin( _angle * Mathf.PI/180 );

			//_test.transform.position = _orbit;

			Sun.transform.LookAt( _orbit );
			m_PathPositions.Add( _orbit );


		}

		List<Vector3> m_PathPositions = new List<Vector3>();
		private void OnDrawGizmos()
		{
			if( m_PathPositions.Count > 1000 )
				m_PathPositions.RemoveAt(0);
	
			Vector3 _prior_pos = Vector3.zero;
			foreach( Vector3 _pos in m_PathPositions)
			{
				if( _prior_pos != Vector3.zero  )
					Gizmos.DrawLine( _prior_pos, _pos );
				
				_prior_pos = _pos;
			}
			
		}
	}
}
