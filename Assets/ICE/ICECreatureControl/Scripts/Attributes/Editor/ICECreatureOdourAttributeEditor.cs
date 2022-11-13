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
	[CustomEditor(typeof(ICECreatureOdourAttribute))]
	public class ICECreatureOdourAttributeEditor : Editor {

		private ICECreatureOdourAttribute m_attribute;

		public virtual void OnEnable()
		{
			m_attribute = (ICECreatureOdourAttribute)target;

		}

		public override void OnInspectorGUI()
		{
			GUI.changed = false;
			Info.HelpButtonIndex = 0;

			EditorGUILayout.Separator();

			m_attribute.Odour = (OdourType)ICEEditorLayout.EnumPopup("Odour","", m_attribute.Odour, Info.STATUS_ODOUR );			
			if( m_attribute.Odour != OdourType.NONE )
			{					
				EditorGUI.indentLevel++;
					m_attribute.OdourIntensity = ICEEditorLayout.MaxDefaultSlider( "Intensity", "", m_attribute.OdourIntensity , 1, 0, ref m_attribute.OdourIntensityMax, 0, Info.STATUS_ODOUR_INTENSITY );
					m_attribute.OdourRange = ICEEditorLayout.MaxDefaultSlider( "Range", "", m_attribute.OdourRange , 1, 0, ref m_attribute.OdourRangeMax, 0, Info.STATUS_ODOUR_RANGE );
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();
			}

			EditorGUILayout.Separator();
			if (GUI.changed)
				EditorUtility.SetDirty( m_attribute );

		}
	}
}
