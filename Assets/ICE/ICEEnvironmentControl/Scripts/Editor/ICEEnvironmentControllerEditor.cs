// ##############################################################################
//
// ICEEnvironmentControllerEditor.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using ICE;
using ICE.Styles;
using ICE.Layouts;

namespace ICE.Environment
{
	[CustomEditor(typeof(ICEEnvironmentController))]
	public class ICEEnvironmentControllerEditor : Editor {

		public ICEEnvironmentController m_environment_master;

		public virtual void OnEnable()
		{
			m_environment_master = (ICEEnvironmentController)target;
		}

		public override void OnInspectorGUI()
		{
			ICEEditorLayout.Label( "Display", true );
			EditorGUI.indentLevel++;
				m_environment_master.UITextDate = (Text)EditorGUILayout.ObjectField( "Date", m_environment_master.UITextDate, typeof(Text), true );
				m_environment_master.UITextTime = (Text)EditorGUILayout.ObjectField( "Time", m_environment_master.UITextTime, typeof(Text), true );
				EditorGUI.indentLevel++;
					m_environment_master.UseShortTime = ICEEditorLayout.Toggle( "Use Short Time", "", m_environment_master.UseShortTime, "" );
				EditorGUI.indentLevel--;
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();

			ICEEditorLayout.Label( "Day-Night Cycle", true );
			EditorGUI.indentLevel++;
				m_environment_master.Sun = (Light)EditorGUILayout.ObjectField( "Sun", m_environment_master.Sun, typeof(Light), true );
				EditorGUI.indentLevel++;
					m_environment_master.Azimut = (int)ICEEditorLayout.DefaultSlider( "Azimut", "", m_environment_master.Azimut, 1, 0, 360, 270, ""); 
					m_environment_master.Radius = (int)ICEEditorLayout.MaxDefaultSlider( "Distance", "", m_environment_master.Radius, 1, 0, ref m_environment_master.RadiusMax, 60, ""); 
					m_environment_master.Zenit = (int)ICEEditorLayout.DefaultSlider( "Zenit Angle", "", m_environment_master.Zenit, 1, 0, 90, 60, ""); 
				EditorGUI.indentLevel--;

				EditorGUILayout.Separator();

			ICEEditorLayout.BeginHorizontal();
				EditorGUI.BeginDisabledGroup( m_environment_master.UseSystemTime == true );
					m_environment_master.DayLengthInMinutes = (int)ICEEditorLayout.MaxDefaultSlider( "Length Of Day (minutes)", "", m_environment_master.DayLengthInMinutes, 1, 1, ref m_environment_master.DayLengthInMinutesMax, 60, ""); 
				EditorGUI.EndDisabledGroup();
				m_environment_master.UseSystemTime = ICEEditorLayout.ButtonCheck( "SYS", "Use System Time", m_environment_master.UseSystemTime, ICEEditorStyle.CMDButtonDouble );  
			ICEEditorLayout.EndHorizontal();

				m_environment_master.StartTimeInHours = ICEEditorLayout.DefaultSlider( "Start Time (hour)", "", m_environment_master.StartTimeInHours, 0.25f, 0, 24, 6, ""); 
				//m_environment_master.SunriseHour = ICEEditorLayout.DefaultSlider( "Sunrise (hour)", "", m_environment_master.SunriseHour, 0.25f, 1, 12, 6, ""); 
				//m_environment_master.SunsetHour = ICEEditorLayout.DefaultSlider( "Sunset (hour)", "", m_environment_master.SunsetHour, 0.25f, 12, 24, 18, ""); 

			EditorGUI.indentLevel--;
				

			//ICEEditorLayout.DefaultSlider( "Hour", "", m_environment_master.Hour, 0.25f, 1, 24, 0, ""); 
			//ICEEditorLayout.DefaultSlider( "Seconds", "", m_environment_master.CurrentSecondsOfDay, 1, 1, 24*3600, 0, "");



		}
	}
}