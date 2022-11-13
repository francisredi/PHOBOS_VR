// ##############################################################################
//
// ICECreatureInventoryExtensionEditor.cs
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

namespace ICE.Creatures.Extensions
{
	[CustomEditor(typeof(ICECreatureInventoryExtension))]
	public class ICECreatureInventoryExtensionEditor : Editor 
	{
		private ICECreatureInventoryExtension m_extension;
		
		public virtual void OnEnable()
		{
			m_extension = (ICECreatureInventoryExtension)target;
			
		}
		
		public override void OnInspectorGUI()
		{
			GUI.changed = false;
			Info.HelpButtonIndex = 0;

			foreach( InventoryItem _item in m_extension.Items )
				DrawInventoryItem( _item );

			ICEEditorLayout.BeginHorizontal();
			ICEEditorLayout.Label( "Add Inventory Item", false );
			if( ICEEditorLayout.Button( "ADD", "", ICEEditorStyle.ButtonFlex ) )
				m_extension.Items.Add( new InventoryItem() );
			ICEEditorLayout.EndHorizontal();

			EditorGUILayout.Separator();
			if (GUI.changed)
				EditorUtility.SetDirty( m_extension );
		}

		private static void DrawInventoryItem( InventoryItem _item )
		{
			// BEGIN OBJECT
			ICEEditorLayout.BeginHorizontal();
				_item.ReferenceItem = (ICECreatureItem)EditorGUILayout.ObjectField( "Reference Object", _item.ReferenceItem, typeof(ICECreatureItem), true );			
			ICEEditorLayout.EndHorizontal( Info.REGISTER_REFERENCE_OBJECT );
			// END OBJECT


		}
	}
}
