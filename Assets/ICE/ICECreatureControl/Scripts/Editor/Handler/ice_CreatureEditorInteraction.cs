// ##############################################################################
//
// ice_CreatureEditorInteraction.cs
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
using ICE.Shared;
using ICE.Utilities;
using ICE.Creatures.EditorInfos;

namespace ICE.Creatures.EditorHandler
{
	public static class EditorInteraction
	{	
		private static ICECreatureRegister m_creature_register = null;

		public static void Print( ICECreatureControl _control )
		{
			if( m_creature_register == null )
				m_creature_register = ICECreatureRegister.Instance;

			if( m_creature_register == null )
				return;
			
			if( ! _control.Display.ShowInteractionSettings )
				return;
			
			ICEEditorStyle.SplitterByIndent( 0 );			
			ICEEditorLayout.BeginHorizontal();
				_control.Display.FoldoutInteraction = ICEEditorLayout.Foldout( _control.Display.FoldoutInteraction, "Interaction" );			
				if (GUILayout.Button( new GUIContent( "SAVE", "Saves the complete interaction settings to file" ), ICEEditorStyle.ButtonMiddle ))
					CreatureIO.SaveInteractionToFile( _control.Creature.Interaction, _control.gameObject.name );			
				if (GUILayout.Button( new GUIContent( "LOAD", "Loads existing interaction settings form file" ), ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Interaction = CreatureIO.LoadInteractionFromFile( _control.Creature.Interaction );			
				if (GUILayout.Button( new GUIContent( "RESET", "Removes all interaction settings" ), ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Interaction.Reset();			
			ICEEditorLayout.EndHorizontal( Info.INTERACTION );
			
			if ( ! _control.Display.FoldoutInteraction ) 
				return;
				
			EditorGUILayout.Separator();
			EditorGUI.indentLevel++;				
				for (int _interactor_index = 0; _interactor_index < _control.Creature.Interaction.Interactors.Count; ++_interactor_index )
					DrawInteractor( _control, _control.Creature.Interaction, _interactor_index );					
			EditorGUI.indentLevel--;
			
			ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel + 1 );			
			ICEEditorLayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Add Interactor", ICEEditorStyle.LabelBold );				
				if (GUILayout.Button( new GUIContent( "LOAD", "Load existing interactor settings from file" ), ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Interaction.Interactors.Add( CreatureIO.LoadInteractorFromFile( new InteractorObject() ) );				
				if (GUILayout.Button( new GUIContent( "ADD", "Create a new interactor record" ), ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Interaction.Interactors.Add( new InteractorObject() );	
			ICEEditorLayout.EndHorizontal();
						
			EditorGUILayout.Separator();

		}


		private static void DrawInteractor( ICECreatureControl _control, InteractionObject _interaction_object, int _index )
		{
			ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel );
			
			InteractorObject _interactor = _interaction_object.Interactors[_index];

			// INTERACTOR TITLE BEGIN
			string _title = "Interactor";
			if( _interactor.TargetGameObject != null )
				_title += " '" + _interactor.TargetTitle + "'";
			else if( _interactor.Enabled )
				_title += " [INVALID]";

			if( _interactor.Rules.Count == 0  )
				_title += " (1 act)";
			else if( _interactor.Rules.Count > 1  )
				_title += " (" + (_interactor.Rules.Count + 1) + " acts)";
			// INTERACTOR TITLE END

			// HEADER BEGIN
			ICEEditorLayout.BeginHorizontal();				
				_interactor.InteractorFoldout = EditorGUILayout.Foldout(_interactor.InteractorFoldout, _title , ICEEditorStyle.Foldout);
				
				EditorGUI.BeginDisabledGroup ( _interactor.Enabled == false);
				
				if (GUILayout.Button(new GUIContent( "SAVE", "Saves selected interactor to file" ), ICEEditorStyle.CMDButtonDouble ))
					CreatureIO.SaveInteractorToFile( _interactor, _interactor.TargetName );
				
				if (GUILayout.Button( new GUIContent( "LOAD", "Replaces selected interactor settings" ), ICEEditorStyle.CMDButtonDouble ))
				{
					_control.Creature.Interaction.Interactors.Insert(_index, CreatureIO.LoadInteractorFromFile( new InteractorObject() ) );
					_interaction_object.Interactors.Remove( _interactor );
					return;
				}

				if (GUILayout.Button( new GUIContent( "COPY", "Creates a copy of the selected interactor" ), ICEEditorStyle.CMDButtonDouble ))
					_control.Creature.Interaction.Interactors.Add( new InteractorObject( _interactor ) );	
				
				if (GUILayout.Button( new GUIContent( "DEL", "Removes selected interactor" ), ICEEditorStyle.CMDButtonDouble ))
				{
					_interaction_object.Interactors.RemoveAt(_index);
					--_index;
					return;
				}

				//_interactor.UseMultipleRules = ICEEditorLayout.ButtonCheck( "COMPLEX", "", _interactor.UseMultipleRules, ICEEditorStyle.ButtonMiddle );


				EditorGUI.EndDisabledGroup();
				_interactor.Enabled = ICEEditorLayout.ButtonCheck( "ENABLE", "", _interactor.Enabled, ICEEditorStyle.ButtonMiddle );

				ICEEditorLayout.DrawPriorityInfo( _interactor.AveragePriority, "Average Priority" );

			ICEEditorLayout.EndHorizontal(  ref _interactor.ShowInteractorInfoText, ref _interactor.InteractorInfoText, Info.INTERACTION_INTERACTOR );
			// HEADER END
			
			if( ! _interactor.InteractorFoldout )
				return;

			EditorGUI.BeginDisabledGroup ( _interactor.Enabled == false);


				ICEEditorLayout.BeginHorizontal();	
					_interactor.Foldout = ICEEditorLayout.Foldout( _interactor.Foldout, " Act #1 - " + (_interactor.BehaviourModeKey.Trim() != ""?_interactor.BehaviourModeKey:"UNDEFINED" ) );
					ICEEditorLayout.DrawPriorityInfo( _interactor.SelectionPriority );
				ICEEditorLayout.EndHorizontal(  ref _interactor.ShowInfoText, ref _interactor.InfoText, Info.INTERACTION_INTERACTOR_TARGET );
				if( _interactor.Foldout )
				{
					if( Application.isPlaying )
						EditorSharedTools.DrawTargetObjectBlind( _interactor, "", "" );
					else
						EditorSharedTools.DrawTargetObject( _control, _interactor, "", "" );
					EditorSharedTools.DrawTargetContent( _control, _interactor );
					_interactor.BehaviourModeKey = EditorBehaviour.BehaviourSelect( _control, "Behaviour", "Behaviour while sensing this interactor", _interactor.BehaviourModeKey, "SENSE" ); 


				}

				//ICEEditorLayout.Label( "Additional Rules for meeting '" + _interactor.TargetTitle + "' creatures.", true );	
				
				if( _interactor.Rules.Count == 0 )
					Info.Note( Info.INTERACTION_INTERACTOR_NO_RULES );
				else
				{
					//EditorGUILayout.Separator();
					for( int _behaviour_index = 0 ; _behaviour_index < _interactor.Rules.Count ; _behaviour_index++ )
						DrawInteractorRule( _control, _interactor, _behaviour_index );
				}
				
				
				ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel + 1 );					
				ICEEditorLayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Add Interaction Rule for '" + _interactor.TargetTitle + "'", EditorStyles.boldLabel );				
				if (GUILayout.Button("ADD", ICEEditorStyle.ButtonMiddle ))
					_interactor.Rules.Add( new InteractorRuleObject( "" ) );
				ICEEditorLayout.EndHorizontal();				
				
				EditorGUILayout.Separator();
			EditorGUI.EndDisabledGroup();

		}
		/*
		private static InteractorRuleObject DrawInteractorRuleOffset( ICECreatureControl _control,InteractorObject _interactor, InteractorRuleObject _rule )
		{
			TargetObject _target = _rule as TargetObject;//new TargetObject(  TargetType.INTERACTOR );
			_target.TargetGameObject = _interactor.TargetGameObject;// m_creature_register.GetTargetByName( _interactor.TargetName );
			_target.TargetTag = _interactor.TargetTag;
			_target.TargetName = _interactor.TargetName;

			_rule.OverrideTargetMovePosition = EditorSharedTools.DrawTargetObjectBlind( _target, _rule.OverrideTargetMovePosition );
			if( _rule.OverrideTargetMovePosition )
				EditorSharedTools.DrawTargetMoveSettings( _control.gameObject, _target);
		
			return _rule;
		}	*/
		
		private static void DrawInteractorRule( ICECreatureControl _control, InteractorObject _interactor, int _index )
		{
	
			InteractorRuleObject _rule = _interactor.Rules[_index];
			InteractorRuleObject _prev_rule = null;
			InteractorRuleObject _next_rule = null;
			
			//float _rule_max_distance = Init.DEFAULT_MAX_DISTANCE;
			//float _rule_min_distance = 0;
			
			
			//if( _interactor.Selectors.SelectionRange >= _rule.Selectors.SelectionRange + Init.SELECTION_RANGE_STEP )
			//	_rule_max_distance = _interactor.Selectors.SelectionRange - Init.SELECTION_RANGE_STEP;
			
			int _prev_index = _index - 1;
			int _next_index = _index + 1;
			
			
			if( _prev_index >= 0 )
			{
				_prev_rule = _interactor.Rules[_prev_index];
				//_rule_max_distance = _prev_rule.Selectors.SelectionRange - Init.SELECTION_RANGE_STEP;
			}
			
			if( _next_index < _interactor.Rules.Count )
			{
				_next_rule = _interactor.Rules[_next_index];
				//_rule_min_distance = _next_rule.Selectors.SelectionRange + Init.SELECTION_RANGE_STEP;
			}
			

			ICEEditorLayout.BeginHorizontal();	
				_rule.Foldout = ICEEditorLayout.Foldout( _rule.Foldout, " Act #" + ( _index + 2 )+ " - " + (_rule.BehaviourModeKey.Trim() != ""?_rule.BehaviourModeKey:"UNDEFINED" ) );
				
				if( _interactor.Rules.Count > 1 )
				{
					EditorGUI.BeginDisabledGroup( _index <= 0 );					
						if( ICEEditorLayout.ButtonUp() )
						{
							InteractorRuleObject _obj = _interactor.Rules[_index]; 
							_interactor.Rules.RemoveAt( _index );
							float _obj_selection_range = _obj.Selectors.SelectionRange;
							
							if( _index - 1 < 0 )
								_interactor.Rules.Add( _obj );
							else
								_interactor.Rules.Insert( _index - 1, _obj );
							
							if( _prev_rule != null )
							{	
								_obj.Selectors.SelectionRange = _prev_rule.Selectors.SelectionRange;
								_prev_rule.Selectors.SelectionRange = _obj_selection_range;
							}	
							return;
						}					
					EditorGUI.EndDisabledGroup();
					
					EditorGUI.BeginDisabledGroup( _index >= _interactor.Rules.Count - 1 );					
						if( ICEEditorLayout.ButtonDown() )
						{
							InteractorRuleObject _obj = _interactor.Rules[_index]; 
							_interactor.Rules.RemoveAt( _index );
							float _obj_selection_range = _obj.Selectors.SelectionRange;
							
							if( _index + 1 > _interactor.Rules.Count )
								_interactor.Rules.Insert( 0, _obj );
							else
								_interactor.Rules.Insert( _index +1, _obj );
							
							if( _next_rule  != null )
							{	
								_obj.Selectors.SelectionRange = _next_rule.Selectors.SelectionRange;
								_next_rule.Selectors.SelectionRange = _obj_selection_range;
							}	
							return;
						}	
					EditorGUI.EndDisabledGroup();
				}

				if( ICEEditorLayout.Button( "DEL", "Removes selected rule", ICEEditorStyle.CMDButtonDouble ) )
				{
					_interactor.Rules.RemoveAt( _index );
					--_index;
					return;
				}

				_rule.Enabled = ICEEditorLayout.ButtonCheck( "ENABLED", "Enables and disables the selected rule" , _rule.Enabled , ICEEditorStyle.ButtonMiddle );
				
				ICEEditorLayout.DrawPriorityInfo( _rule.SelectionPriority );


			ICEEditorLayout.EndHorizontal(  ref _rule.ShowInfoText, ref _rule.InfoText, Info.INTERACTION_INTERACTOR_RULE );

			if( _rule.Foldout )
			{
				EditorGUI.BeginDisabledGroup ( _rule.Enabled == false);

					_rule.OverrideTargetGameObject( _interactor.TargetGameObject );

					_rule.Selectors.CanUseDefaultPriority = true;

					if( _rule.Selectors.UseDefaultPriority )
						_rule.Selectors.Priority = _interactor.Selectors.Priority;

					EditorSharedTools.DrawTargetObjectBlind( _rule as TargetObject, "", "" );
					EditorSharedTools.DrawTargetContent( _control, _rule as TargetObject );

			
					// BEHAVIOUR
					string _auto_key = _interactor.TargetName + "_action_" + _index;
					_rule.BehaviourModeKey = EditorBehaviour.BehaviourSelect( _control, "Behaviour", "Action behaviour for this interaction rule", _rule.BehaviourModeKey, _auto_key ); 
					EditorGUILayout.Separator();					

				EditorGUI.EndDisabledGroup ();
			}
		}

	}


}
