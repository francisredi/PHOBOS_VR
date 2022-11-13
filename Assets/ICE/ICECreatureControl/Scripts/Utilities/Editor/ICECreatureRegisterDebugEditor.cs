// ##############################################################################
//
// ICECreatureRegisterDebugEditor.cs
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
using ICE.Styles;
using ICE.Layouts;
using ICE.Creatures;
using ICE.Creatures.EditorHandler;
using ICE.Creatures.EditorInfos;
using ICE.Creatures.Utilities;

namespace ICE.Creatures.Utilities
{
	[CustomEditor(typeof(ICECreatureRegisterDebug))]
	public class ICECreatureRegisterDebugEditor : Editor 
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox( "Use the debug options of the CreatureRegister to adapt the settings and please note, that this component is part of " +
				"the ICECreatureRegister and will be used automatically by the register, so please don't assign it manual to other GameObjects. ",MessageType.None );
		}
	}
}