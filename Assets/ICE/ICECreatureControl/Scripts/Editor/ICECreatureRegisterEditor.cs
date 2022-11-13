// ##############################################################################
//
// ice_CreatureRegisterEditor.cs
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
using ICE;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Styles;
using ICE.Layouts;
using ICE.Creatures.EditorHandler;
using ICE.Creatures.EditorInfos;

namespace ICE.Creatures
{

	[CustomEditor(typeof(ICECreatureRegister))]
	public class ICECreatureRegisterEditor : Editor {

		private ICECreatureRegister m_creature_register;

		public bool m_foldout_register = true;
		public bool m_foldout_options = true;
		public bool m_foldout_environment = true;
	
		public virtual void OnEnable()
		{
			//m_creature_register = (ICECreatureRegister)target;
		}


		public override void OnInspectorGUI()
		{
			m_creature_register = (ICECreatureRegister)target;

			GUI.changed = false;
			Info.HelpButtonIndex = 0;

			EditorGUI.indentLevel++;
				EditorRegisterOptions.Print( m_creature_register );					
				EditorRegisterGroups.Print( m_creature_register );
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();

			if ( GUI.changed )
				EditorUtility.SetDirty( m_creature_register );

		}


	}

}
