// ##############################################################################
//
// ICECreaturePlayMakerAdapterEditor.cs
// Version 1.1
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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using ICE;
using ICE.Styles;
using ICE.Layouts;
using ICE.Creatures;
using ICE.Creatures.EditorHandler;
using ICE.Creatures.EnumTypes;

namespace ICE.Creatures.Adapter
{
	[CustomEditor(typeof(ICECreaturePlayMakerAdapter))]
	public class ICECreaturePlayMakerAdapterEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			ICECreaturePlayMakerAdapter _adapter = (ICECreaturePlayMakerAdapter)target;
			//ICECreatureRegister.Register.Network = NetworkType.PUN;
		}
	}
}
