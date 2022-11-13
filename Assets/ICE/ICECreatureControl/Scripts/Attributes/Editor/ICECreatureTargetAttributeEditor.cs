// ##############################################################################
//
// ICECreatureTargetAttributeEditor.cs
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

namespace ICE.Creatures.Attributes
{
	[CustomEditor(typeof(ICECreatureTargetAttribute))]
	public class ICECreatureTargetAttributeEditor : Editor 
	{
		private ICECreatureTargetAttribute m_attribute;
		
		public virtual void OnEnable()
		{
			m_attribute = (ICECreatureTargetAttribute)target;
			
		}
		
		public override void OnInspectorGUI()
		{
			GUI.changed = false;
			Info.HelpButtonIndex = 0;

			EditorGUILayout.Separator();

			TargetType _type = TargetType.UNDEFINED;
			if( m_attribute.GetComponentInChildren<ICECreaturePlayer>() != null )
				_type = TargetType.PLAYER;
			else if( m_attribute.GetComponentInChildren<ICECreatureItem>() != null )
				_type = TargetType.ITEM;
			else if( m_attribute.GetComponentInChildren<ICECreatureLocation>() != null )
				_type = TargetType.WAYPOINT;
			else if( m_attribute.GetComponentInChildren<ICECreatureWaypoint>() != null )
				_type = TargetType.WAYPOINT;
			else if( m_attribute.GetComponentInChildren<ICECreatureMarker>() != null )
				_type = TargetType.WAYPOINT;
			else if( m_attribute.GetComponentInChildren<ICECreatureControl>() != null )
				_type = TargetType.CREATURE;

			ICEEditorLayout.Label( "Default Target Settings", true );
			EditorGUI.indentLevel++;
				EditorSharedTools.DrawTargetSelectors( null, m_attribute.Target.Selectors, _type, 0, 250 );
				EditorSharedTools.DrawTargetMoveSettings( null, m_attribute.Target );
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
			if (GUI.changed)
				EditorUtility.SetDirty( m_attribute );
			
		}
	}
}
