// ##############################################################################
//
// ice_CreatureEditorRegister.cs
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
using ICE.Creatures.EditorInfos;



namespace ICE.Creatures.EditorHandler
{
	
	public static class EditorRegister
	{	
		private static ICECreatureRegister m_creature_register;

		public static void Print( ICECreatureControl _control )
		{
		
			ICEEditorLayout.BeginHorizontal();
			EditorGUILayout.LabelField( _control.gameObject.name + "", ICEEditorStyle.LabelBold );
			if (GUILayout.Button("SAVE", ICEEditorStyle.ButtonMiddle ))
				CreatureIO.SaveCreatureToFile( _control.Creature, _control.gameObject.name );
			if (GUILayout.Button("LOAD", ICEEditorStyle.ButtonMiddle ))
				_control.Creature = CreatureIO.LoadCreatureFromFile( _control.Creature );	
			if (GUILayout.Button("RESET", ICEEditorStyle.ButtonMiddle ))
				_control.Creature = new CreatureObject();
			ICEEditorLayout.EndHorizontal( Info.CREATURE_PRESETS );



			EditorGUILayout.Separator();


			m_creature_register = ICECreatureRegister.Instance;
			
			if( m_creature_register == null )
			{
				Info.Warning( Info.REGISTER_MISSING );
		
				ICEEditorLayout.BeginHorizontal();
				ICEEditorLayout.Label( "Add Creature Register", true);
				if (GUILayout.Button("ADD REGISTER", ICEEditorStyle.ButtonLarge ) )
				{
					GameObject _object = new GameObject();
					m_creature_register = _object.AddComponent<ICECreatureRegister>();
					_object.name = "CreatureRegister";

					if( m_creature_register != null )
						m_creature_register.UpdateReferenceCreature();
				}
				ICEEditorLayout.EndHorizontal( Info.REGISTER );
				
			}
			else if( ! m_creature_register.isActiveAndEnabled )
			{
				Info.Warning( Info.REGISTER_DISABLED );
				
				ICEEditorLayout.BeginHorizontal();
				ICEEditorLayout.Label( "Activate Creature Register", true);
				if (GUILayout.Button("ACTIVATE", ICEEditorStyle.ButtonLarge ) )
				{
					m_creature_register.gameObject.SetActive( true);
				}
				ICEEditorLayout.EndHorizontal(  Info.REGISTER  );
			}
			else
			{
				ICEEditorLayout.BeginHorizontal();
				
					GameObject _registered_object = DrawRegisterPopup( "Quick Selection" );
					
					if( _registered_object != null )
					{
						ICEEditorLayout.ButtonSelectObject( _registered_object );				
					}
					else
					{
						if (GUILayout.Button("SCAN", ICEEditorStyle.ButtonMiddle ) )
							m_creature_register.UpdateReferenceCreature();
					}
					
					if (GUILayout.Button("REGISTER", ICEEditorStyle.ButtonMiddle ) )
						Selection.activeGameObject = m_creature_register.gameObject;
				
				ICEEditorLayout.EndHorizontal( Info.REGISTER );
			}
		}

	

		private static int _register_popup_index = 0;
		private static GameObject DrawRegisterPopup( string _title )
		{
			if( m_creature_register == null )
				m_creature_register = GameObject.FindObjectOfType<ICECreatureRegister>();
			
			if( m_creature_register == null )
			{
				EditorGUILayout.LabelField( _title );
				return null;
			}
			else if( m_creature_register.ReferenceGroupObjects.Count == 0 )
			{
				EditorGUILayout.LabelField( _title );
				return null;
			}
			else
			{
				List<ReferenceGroupObject> _group = m_creature_register.ReferenceGroupObjects;
				
				string[] _names = new string[_group.Count];
				
				if( _register_popup_index > _group.Count )
					_register_popup_index = 0;
				
				for(int i=0;i < _group.Count ;i++)
				{
					_names[i] = _group[i].Name;
				}
				
				_register_popup_index = EditorGUILayout.Popup( _title, _register_popup_index, _names );
				
				return _group[ _register_popup_index ].Reference;
				
			}			
		}
	}
}