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
	[CustomEditor(typeof(ICECreatureItem))]
	public class ICECreatureItemEditor : Editor 
	{
		private ICECreatureItem m_creature_item;


		public virtual void OnEnable()
		{
			m_creature_item = (ICECreatureItem)target;

		}
		
		public override void OnInspectorGUI()
		{
			GUI.changed = false;
			Info.HelpButtonIndex = 0;

			EditorGUILayout.Separator();
			m_creature_item.gameObject.name = ICEEditorLayout.Text( "Name", "", m_creature_item.gameObject.name, Info.ITEM_NAME );

			m_creature_item.UseLimitedLifespan = ICEEditorLayout.Toggle( "Use Limited Lifespan", "", m_creature_item.UseLimitedLifespan , "" );

			if( m_creature_item.UseLimitedLifespan )
			{
				EditorGUI.indentLevel++;
				ICEEditorLayout.BeginHorizontal();
					ICEEditorLayout.MinMaxGroupSimple( "Lifespan", "", ref m_creature_item.LifespanMin, ref m_creature_item.LifespanMax, 0, ref m_creature_item.MaxLifespan, 0.25f, 40, "" );

					if( ICEEditorLayout.Button( "RND", "", ICEEditorStyle.CMDButtonDouble ) )
					{
						m_creature_item.LifespanMax = Random.Range( m_creature_item.LifespanMin, m_creature_item.MaxLifespan );
						m_creature_item.LifespanMin = Random.Range( 0, m_creature_item.LifespanMax );
					}

					if( ICEEditorLayout.Button( "D", "", ICEEditorStyle.CMDButtonDouble ) )
					{
						m_creature_item.LifespanMin = 0;
						m_creature_item.LifespanMax = 0;
					}

				ICEEditorLayout.EndHorizontal();
				m_creature_item.DetachChildren = ICEEditorLayout.Toggle( "Detach Children", "", m_creature_item.DetachChildren , "" );
				EditorGUI.indentLevel--;
			}

			EditorSharedTools.DrawInventoryObject( "Use Inventory", "", m_creature_item.Inventory, m_creature_item.gameObject, "" );

			EditorGUILayout.Separator();
			ICEEditorLayout.AttributeCreature( m_creature_item.gameObject );
			EditorGUILayout.Separator();
			if (GUI.changed)
				EditorUtility.SetDirty( m_creature_item );
			
		}
	}
}
