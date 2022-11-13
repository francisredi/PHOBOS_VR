// ##############################################################################
//
// ICECreatureLocationEditor.cs
// Version 1.1.15
//
// © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.icecreaturecontrol.com
// mailto:support@icecreaturecontrol.com
// 
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// ##############################################################################

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;

using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Creatures.EditorInfos;
using ICE.Creatures.EditorHandler;
using ICE.Styles;
using ICE.Layouts;

namespace ICE.Creatures
{
	[CustomEditor(typeof(ICECreatureLocation))]
	public class ICECreatureLocationEditor : Editor 
	{
		private ICECreatureLocation m_creature_location;

		private AttributeType m_Attribute;

		public virtual void OnEnable()
		{
			m_creature_location = (ICECreatureLocation)target;

		}
		
		public override void OnInspectorGUI()
		{
			GUI.changed = false;
			Info.HelpButtonIndex = 0;

			EditorGUILayout.Separator();
			m_creature_location.gameObject.name = ICEEditorLayout.Text( "Name", "", m_creature_location.gameObject.name, Info.LOCATION_NAME );
			ICEEditorLayout.AttributeCreature( m_creature_location.gameObject );

			EditorGUILayout.Separator();
			if (GUI.changed)
				EditorUtility.SetDirty( m_creature_location );
			
		}
	}
}
