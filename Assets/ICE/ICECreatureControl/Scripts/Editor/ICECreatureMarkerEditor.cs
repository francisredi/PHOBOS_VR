//
// ICECreatureMarkerEditor.cs
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
	[CustomEditor(typeof(ICECreatureMarker))]
	public class ICECreatureMarkerEditor : Editor {
		
		private ICECreatureMarker m_marker;

		public virtual void OnEnable()
		{
			m_marker = (ICECreatureMarker)target;

		}

		public override void OnInspectorGUI()
		{
			GUI.changed = false;
			Info.HelpButtonIndex = 0;

			EditorGUILayout.Separator();
			m_marker.UseLimitedLifespan = ICEEditorLayout.Toggle( "Use Limited Lifespan", "", m_marker.UseLimitedLifespan , Info.LIFESPAN );
			EditorGUI.indentLevel++;
				ICEEditorLayout.RandomMinMaxGroupExt( "Lifespan (min/max)", "", ref m_marker.LifespanMin , ref m_marker.LifespanMax, 0, ref m_marker.MaxLifespan, 20, 40, 30, 0.25f, Info.LIFESPAN_INTERVAL );
				m_marker.DetachChildren = ICEEditorLayout.Toggle( "Detach Children", "", m_marker.DetachChildren , Info.LIFESPAN_DETACH );
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();

			EditorSharedTools.DrawOdourObject( "Odour", "", m_marker.Odour, Info.STATUS_ODOUR);

			EditorGUILayout.Separator();
			if (GUI.changed)
				EditorUtility.SetDirty( m_marker );

		}
	}
}
