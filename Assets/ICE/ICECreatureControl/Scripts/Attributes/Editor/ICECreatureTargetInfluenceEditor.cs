// ##############################################################################
//
// ICECreatureInfluenceAttributeEditor.cs
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
	[CustomEditor(typeof(ICECreatureInfluenceAttribute))]
	public class ICECreatureInfluenceAttributeEditor : Editor 
	{
		private ICECreatureInfluenceAttribute m_attribute;
		
		public virtual void OnEnable()
		{
			m_attribute = (ICECreatureInfluenceAttribute)target;
			
		}
		
		public override void OnInspectorGUI()
		{
			GUI.changed = false;
			Info.HelpButtonIndex = 0;

			EditorGUILayout.Separator();

			ICEEditorLayout.Label( "Default Target Settings", true );
			EditorSharedTools.DrawTargetMoveSettings( null, m_attribute.Target );
			
			EditorGUILayout.Separator();
			if (GUI.changed)
				EditorUtility.SetDirty( m_attribute );
			
		}
	}
}
