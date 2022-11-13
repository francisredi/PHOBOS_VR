// ##############################################################################
//
// ice_CreatureEditorDisplay.cs
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
	public static class EditorDisplay
	{
		public static void Print( ICECreatureControl _control )
		{
			Header( _control );
			Options( _control.Display );
			Footer( _control.Display );
		}

		private static void Header( ICECreatureControl _control  )
		{
			ICEEditorLayout.BeginHorizontal();
			
			string _global_info = " (local)";
			
			if( _control.Display.UseGlobalAll )
				_global_info = " (all global)";
			else if( _control.Display.UseGlobal )
				_global_info = " (global)";
			
			_control.Display.DisplayOptions = (DisplayOptionType)ICEEditorLayout.EnumPopup("Display Options" + _global_info,"", _control.Display.DisplayOptions );
			
			string _use_global_all_title = "ALL GLOBAL";
			if( _control.Display.UseGlobalAll )
				_use_global_all_title = "LOCAL";
			
			if( _control.Display.UseGlobalAll )
				GUI.backgroundColor = Color.yellow;
			
			if (GUILayout.Button( _use_global_all_title, ICEEditorStyle.ButtonLarge ))
			{
				if( ! _control.Display.UseGlobalAll )
					_control.Display.SetLocalToGlobal();
				else
					_control.Display.SetGlobalToLocal();

				_control.Display.UseGlobalAll = ! _control.Display.UseGlobalAll;
			}
			
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			
			ICEEditorLayout.EndHorizontal( Info.DISPLAY_OPTIONS );	



			HandleQuickChange( _control.Display );	

			/*
			EditorGUI.indentLevel++;
				if( ! _display.UseGlobalAll )
					_display.UseGlobal = EditorGUILayout.ToggleLeft( "Use Global Display Options", _display.UseGlobal );
			EditorGUI.indentLevel--;
			*/

			_control.Display.ShowMissionsHome = true;
			_control.Display.ShowMissionsEscort = true;
			_control.Display.ShowMissionsPatrol = true;

			_control.Display.ShowBehaviourMove = true;
			_control.Display.ShowBehaviourAudio = true;
			_control.Display.ShowBehaviourInluences = true;
			_control.Display.ShowBehaviourEffect = true;
			_control.Display.ShowBehaviourLink = true;
		}

		private static void Footer( DisplayData _display )
		{
			EditorGUI.indentLevel++;
				_display.ShowHelp = ICEEditorLayout.ToggleLeft("Show Help","Displays all help informations", _display.ShowHelp, false );
				_display.ShowDebug = ICEEditorLayout.ToggleLeft("Show Debug","Displays debug informations and gizmos",  _display.ShowDebug, false );
				_display.ShowInfo = ICEEditorLayout.ToggleLeft("Show Info","Displays creature informations",  _display.ShowInfo, false );
			EditorGUI.indentLevel--;

		}

		private static void Options( DisplayData _display )
		{
			
			if( _display.DisplayOptions == DisplayOptionType.START )
			{
				//_display.ShowWizard = true;
				_display.ShowEssentials = true;
				_display.ShowStatus = false;
				_display.ShowMissions = false;
				_display.ShowBehaviour = false;
				_display.ShowInteractionSettings = false;
				_display.ShowEnvironmentSettings = false;
			}
			else if( _display.DisplayOptions == DisplayOptionType.BASIC )
			{
				//_display.ShowWizard = false;
				_display.ShowEssentials = true;
				_display.ShowStatus = true;
				_display.ShowMissions = true;
				_display.ShowBehaviour = false;
				_display.ShowInteractionSettings = false;
				_display.ShowEnvironmentSettings = false;
			}
			else if( _display.DisplayOptions == DisplayOptionType.FULL )
			{
				//_display.ShowWizard = false;
				_display.ShowEssentials = true;
				_display.ShowStatus = true;
				_display.ShowMissions = true;
				_display.ShowBehaviour = true;
				_display.ShowInteractionSettings = true;
				_display.ShowEnvironmentSettings = true;
			}
		}

		private static void HandleQuickChange( DisplayData _display )
		{
			if( _display.DisplayOptions == DisplayOptionType.MENU )
			{
				EditorGUILayout.Separator();

				
				ICEEditorLayout.BeginHorizontal();

				/*
				if (GUILayout.Button( "WIZARD", ICEEditorStyle.ButtonExtraLarge ))
				{
					//_display.ShowWizard = true;
					_display.ShowEssentials = false;
					_display.ShowStatus = false;
					_display.ShowMissions = false;
					_display.ShowBehaviour = false;
					_display.ShowInteractionSettings = false;
					_display.ShowEnvironmentSettings = false;
				}*/
				
				ICEEditorLayout.EndHorizontal();
				
				ICEEditorLayout.BeginHorizontal();
				
				GUILayout.FlexibleSpace();
				if (GUILayout.Button( "ESSENTIALS", GUILayout.MinWidth(80),  GUILayout.MaxWidth(180) ))
				{
					//_display.ShowWizard = false;
					_display.ShowEssentials = true;
					_display.ShowStatus = false;
					_display.ShowMissions = false;
					_display.ShowBehaviour = false;
					_display.ShowInteractionSettings = false;
					_display.ShowEnvironmentSettings = false;
				}
				
				if (GUILayout.Button( "STATUS", GUILayout.MinWidth(80),  GUILayout.MaxWidth(180)  ))
				{
					//_display.ShowWizard = false;
					_display.ShowEssentials = false;
					_display.ShowStatus = true;
					_display.ShowMissions = false;
					_display.ShowBehaviour = false;
					_display.ShowInteractionSettings = false;
					_display.ShowEnvironmentSettings = false;
				}

				if (GUILayout.Button( "MISSIONS", GUILayout.MinWidth(80),  GUILayout.MaxWidth(180)  ))
				{
					//_display.ShowWizard = false;
					_display.ShowEssentials = false;
					_display.ShowStatus = false;
					_display.ShowMissions = true;
					_display.ShowBehaviour = false;
					_display.ShowInteractionSettings = false;
					_display.ShowEnvironmentSettings = false;
				}

				GUILayout.FlexibleSpace();
				ICEEditorLayout.EndHorizontal();
				
				ICEEditorLayout.BeginHorizontal();
				
					GUILayout.FlexibleSpace();

					if (GUILayout.Button( "INTERACTION", GUILayout.MinWidth(80),  GUILayout.MaxWidth(180)  ))
					{
						//_display.ShowWizard = false;
						_display.ShowEssentials = false;
						_display.ShowStatus = false;
						_display.ShowMissions = false;
						_display.ShowBehaviour = false;
						_display.ShowInteractionSettings = true;
						_display.ShowEnvironmentSettings = false;
					}

					if (GUILayout.Button( "ENVIRONMENT", GUILayout.MinWidth(80),  GUILayout.MaxWidth(180)  ))
					{
						//_display.ShowWizard = false;
						_display.ShowEssentials = false;
						_display.ShowStatus = false;
						_display.ShowMissions = false;
						_display.ShowBehaviour = false;
						_display.ShowInteractionSettings = false;
						_display.ShowEnvironmentSettings = true;
					}

					if (GUILayout.Button( "BEHAVIOURS", GUILayout.MinWidth(80),  GUILayout.MaxWidth(180)  ))
					{
						//_display.ShowWizard = false;
						_display.ShowEssentials = false;
						_display.ShowStatus = false;
						_display.ShowMissions = false;
						_display.ShowBehaviour = true;
						_display.ShowInteractionSettings = false;
						_display.ShowEnvironmentSettings = false;
					}

					GUILayout.FlexibleSpace();
				ICEEditorLayout.EndHorizontal();

				
				EditorGUILayout.Separator();
				
			}
			
		}
		

	}
}
