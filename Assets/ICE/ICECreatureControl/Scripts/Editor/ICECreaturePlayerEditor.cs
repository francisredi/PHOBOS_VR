// ##############################################################################
//
// ICECreatureItemEditor.cs
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
	[CustomEditor(typeof(ICECreaturePlayer))]
	public class ICECreaturePlayerEditor : Editor 
	{
		private ICECreaturePlayer m_creature_item;


		public virtual void OnEnable()
		{
			m_creature_item = (ICECreaturePlayer)target;

		}
		
		public override void OnInspectorGUI()
		{
			GUI.changed = false;
			Info.HelpButtonIndex = 0;

			EditorGUILayout.Separator();
			m_creature_item.gameObject.name = ICEEditorLayout.Text( "Name", "", m_creature_item.gameObject.name, Info.ITEM_NAME );

			EditorSharedTools.DrawOdourObject( "Odour", "", m_creature_item.Odour, Info.STATUS_ODOUR );
			EditorSharedTools.DrawInventoryObject( "Use Inventory", "", m_creature_item.Inventory, m_creature_item.gameObject, "" );

			EditorGUILayout.Separator();
			ICEEditorLayout.AttributeCreature( m_creature_item.gameObject );
			EditorGUILayout.Separator();
			if (GUI.changed)
				EditorUtility.SetDirty( m_creature_item );
			
		}
	}
}
