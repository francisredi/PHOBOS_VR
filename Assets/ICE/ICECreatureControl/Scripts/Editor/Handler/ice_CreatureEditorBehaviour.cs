// ##############################################################################
//
// ice_CreatureEditorBehaviour.cs
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
using ICE.Creatures.Extensions;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Styles;
using ICE.Layouts;
using ICE.Creatures.EditorInfos;



namespace ICE.Creatures.EditorHandler
{
	
	public static class EditorBehaviour
	{	
		private static string m_key = "";
		private static string m_old_key = "";
		private static string m_new_key = "";

		/// <summary>
		/// Handles the behaviour settings.
		/// </summary>
		public static void Print( ICECreatureControl _control ){
			
			if( ! _control.Display.ShowBehaviour )
				return;
			
			ICEEditorStyle.SplitterByIndent( 0 );

			ICEEditorLayout.BeginHorizontal();
				_control.Display.FoldoutBehaviours = ICEEditorLayout.Foldout(_control.Display.FoldoutBehaviours, "Behaviours" );
				if (GUILayout.Button( new GUIContent( "SAVE","Saves behaviours to file" ), ICEEditorStyle.ButtonMiddle ))
					CreatureIO.SaveBehaviourToFile( _control.Creature.Behaviour, _control.gameObject.name );				
				if (GUILayout.Button( new GUIContent( "LOAD","Loads behavious form file" ), ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Behaviour = CreatureIO.LoadBehaviourFromFile( _control.Creature.Behaviour );				
				if (GUILayout.Button( new GUIContent( "RESET","Removes all behaviours" ), ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Behaviour.Reset();
			ICEEditorLayout.EndHorizontalInfo( Info.BEHAVIOUR );
			
			if( _control.Display.FoldoutBehaviours == false ) 
				return;

			EditorGUI.indentLevel++;				
				for( int i = 0 ; i < _control.Creature.Behaviour.BehaviourModes.Count; i++ )
					DrawBehaviourMode( _control, _control.Creature.Behaviour.BehaviourModes[i], i );								
			EditorGUI.indentLevel--;

			ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel + 1 );

			EditorGUILayout.Separator();				
			ICEEditorLayout.Label( "New Behaviour Mode", true );				
			EditorGUI.indentLevel++;				
				ICEEditorLayout.BeginHorizontal();									
					m_key = EditorGUILayout.TextField( "Behaviour Mode Key", m_key );
					
					if (GUILayout.Button( new GUIContent( "ADD", "Adds new behaviour mode"), ICEEditorStyle.ButtonMiddle ))
					{
						_control.Creature.Behaviour.AddBehaviourMode( m_key );							
						m_key = "";
					}
				ICEEditorLayout.EndHorizontal();				
			EditorGUI.indentLevel--;				
			EditorGUILayout.Separator();

		}

		private static string _rename_key = "";
		private static BehaviourModeObject _rename_mode = null;
		public static void RenameBehaviourMode( ICECreatureControl _control, BehaviourModeObject _mode )
		{
			if( _rename_mode == null || _rename_mode != _mode )
				return;

			ICEEditorLayout.Label( "Rename Behaviour Mode", true );
			EditorGUI.indentLevel++;
			ICEEditorLayout.BeginHorizontal();
				_rename_key = EditorGUILayout.TextField( "Mode Key",_control.Creature.Behaviour.GetFixedBehaviourModeKey(  _rename_key ) );

				if (GUILayout.Button("RENAME", ICEEditorStyle.ButtonMiddle ))
				{
					if( _control.Creature.Behaviour.BehaviourModeExists( _rename_key ) == false )
					{
						_rename_mode.Key = _rename_key;
						_rename_key = "";
						_rename_mode = null;
					}
				}

				if (GUILayout.Button("CANCEL", ICEEditorStyle.ButtonMiddle ))
				{
					_rename_key = "";
					_rename_mode = null;
				}

			ICEEditorLayout.EndHorizontal( Info.BEHAVIOUR_MODE_RENAME );
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
		}
		
		/// <summary>
		/// Draws a behaviour mode.
		/// </summary>
		/// <param name="_mode">_mode.</param>
		/// <param name="_index">_index.</param>
		public static void DrawBehaviourMode( ICECreatureControl _control, BehaviourModeObject _mode, int _index )
		{
			if ( _mode == null ) 
				return;
			
			ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel );
			ICEEditorLayout.BeginHorizontal();
				_mode.Foldout = ICEEditorLayout.Foldout( _mode.Foldout, _mode.Key + " (" + _mode.Rules.Count + (_mode.Rules.Count == 1?" Rule)":" Rules)") );
				GUILayout.FlexibleSpace();

				EditorGUI.BeginDisabledGroup( _rename_mode == _mode );
					if (GUILayout.Button("RENAME", ICEEditorStyle.ButtonMiddle ))
					{
						_rename_key = _mode.Key;
						_rename_mode = _mode;
					}
				EditorGUI.EndDisabledGroup();

				if (GUILayout.Button("COPY", ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Behaviour.CopyBehaviourMode( _mode );
				
				if (GUILayout.Button("REMOVE", ICEEditorStyle.ButtonMiddle ))
				{
					_control.Creature.Behaviour.BehaviourModes.RemoveAt( _index );
					--_index;
				}

			ICEEditorLayout.EndHorizontal(  ref _mode.ShowInfoText, ref _mode.InfoText, Info.BEHAVIOUR_MODE );
			RenameBehaviourMode( _control, _mode );
			EditorGUILayout.Separator();
			if ( _mode.Foldout ) 
				DrawBehaviourModeContent( _control, _mode );


		}

		public static void DrawBehaviourModeContent( ICECreatureControl _control, BehaviourModeObject _mode )
		{
			if( _mode == null )
				return;
			/*
			if( _mode.Rules.Count == 0 )
				_mode.Active = false;
			
		
				ICEEditorLayout.BeginHorizontal();
				_behaviour.Active = EditorGUILayout.Toggle( "Active", _behaviour.Active );
				*/

			_mode.Favoured.Enabled = ICEEditorLayout.ToggleLeft("Favoured", "Blocks other targets and behaviours according to the conditions.", _mode.Favoured.Enabled, true, Info.BEHAVIOUR_MODE_FAVOURED );

			if( _mode.Favoured.Enabled )
			{
				EditorGUI.indentLevel++;

				_mode.Favoured.FavouredPriority = ICEEditorLayout.Slider( "Priority", "Blocks lower prioritized behaviours and breaks for higher ones.", _mode.Favoured.FavouredPriority, 1, 0, 100,  Info.BEHAVIOUR_MODE_FAVOURED_PRIORITY );

				ICEEditorLayout.RandomMinMaxGroupExt( "Desired Runtime", "", ref _mode.Favoured.MinRuntime, ref _mode.Favoured.MaxRuntime, 0, ref _mode.Favoured.MaximumRuntime,0,0, 30, 0.05f, Info.BEHAVIOUR_MODE_FAVOURED_PRIORITY );

				_mode.Favoured.FavouredUntilNextMovePositionReached = ICEEditorLayout.Toggle("Next Move Position", "Blocks other targets and behaviours until the next move position was reached.", _mode.Favoured.FavouredUntilNextMovePositionReached, Info.BEHAVIOUR_MODE_FAVOURED_MOVE_POSITION_REACHED );
				_mode.Favoured.FavouredUntilTargetMovePositionReached = ICEEditorLayout.Toggle("Target Move Position", "Blocks other targets and behaviours until the target move position was reached.", _mode.Favoured.FavouredUntilTargetMovePositionReached, Info.BEHAVIOUR_MODE_FAVOURED_TARGET_MOVE_POSITION_REACHED );
				_mode.Favoured.FavouredTarget = ICEEditorLayout.Toggle( "Specific Target", "Blocks other targets and behaviours while waiting for a specific target", _mode.Favoured.FavouredTarget, Info.BEHAVIOUR_MODE_FAVOURED_TARGET );

				if( _mode.Favoured.FavouredTarget )
				{
					EditorGUI.indentLevel++;
					_mode.Favoured.FavouredTargetName =  EditorSharedTools.TargetPopup( "Wait for Target", "", _mode.Favoured.FavouredTargetName, Info.BEHAVIOUR_MODE_FAVOURED_TARGET_POPUP );
					EditorGUI.BeginDisabledGroup( _mode.Favoured.FavouredTargetName.Trim() == "" );
					_mode.Favoured.FavouredTargetRange = ICEEditorLayout.DefaultSlider( "Range", "Blocks other targets and behaviours until the specified target is inside the range", _mode.Favoured.FavouredTargetRange, 0.25f, 0, 100, 10, Info.BEHAVIOUR_MODE_FAVOURED_TARGET_RANGE );
						EditorGUI.EndDisabledGroup();
					EditorGUI.indentLevel--;
				}

				if( _mode.HasDetourRules )
					_mode.Favoured.FavouredUntilDetourPositionReached = ICEEditorLayout.Toggle("Detour Position", "Blocks other targets and behaviours until a detour position was reached.", _mode.Favoured.FavouredUntilDetourPositionReached, Info.BEHAVIOUR_MODE_FAVOURED_DETOUR );

				
				EditorGUI.indentLevel--;

	
				EditorGUILayout.Separator();
			}
			
			if( _mode.Rules.Count > 1 )
			{
				ICEEditorLayout.Label("Rules", true, Info.BEHAVIOUR_MODE_RULE );
				EditorGUI.indentLevel++;
					ICEEditorLayout.BeginHorizontal();
						_mode.RulesOrderType = (SequenceOrderType)ICEEditorLayout.EnumPopup("Order Type","", _mode.RulesOrderType );
						EditorGUI.BeginDisabledGroup( _mode.RulesOrderType != SequenceOrderType.CYCLE );
							_mode.RulesOrderInverse = ICEEditorLayout.ButtonCheck( "INVERSE", "", _mode.RulesOrderInverse, ICEEditorStyle.ButtonMiddle ); 
						EditorGUI.EndDisabledGroup();
					ICEEditorLayout.EndHorizontal( Info.BEHAVIOUR_MODE_RULES_ORDER );
				EditorGUI.indentLevel--;
			}
			
			for (int i = 0; i < _mode.Rules.Count; ++i)
				DrawBehaviourModeRule( _control, i, _mode.Rules, _mode.Key  );

			ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel + 1 );			
			ICEEditorLayout.BeginHorizontal();
			EditorGUILayout.LabelField( "Add Behaviour Rule", ICEEditorStyle.LabelBold );
			if (GUILayout.Button("ADD", ICEEditorStyle.ButtonMiddle ) )
				_mode.Rules.Add( new BehaviourModeRuleObject() );
			ICEEditorLayout.EndHorizontal();
			EditorGUILayout.Separator();
		}
		
		
		/// <summary>
		/// Draws the behaviour mode rule.
		/// </summary>
		/// <param name="_index">_index.</param>
		/// <param name="_list">_list.</param>
		public static void DrawBehaviourModeRule( ICECreatureControl _control, int _index, List<BehaviourModeRuleObject> _list, string _key )
		{
			
			BehaviourModeRuleObject _rule = _list[_index];
			
			if( _rule == null )
				return;
			
			bool _foldout = true;
			if( _list.Count > 1 )
			{
				EditorGUI.indentLevel++;
				
				ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel );

				EditorGUI.BeginDisabledGroup( _rule.Enabled == false );
				//ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel );
				ICEEditorLayout.BeginHorizontal();
				_rule.Foldout = EditorGUILayout.Foldout( _rule.Foldout, _key + " Rule #"+(_index+1), ICEEditorStyle.Foldout );
				GUILayout.FlexibleSpace();
				EditorGUI.EndDisabledGroup();
				
				if( ICEEditorLayout.ButtonDown() )
				{
					BehaviourModeRuleObject _obj = _list[_index]; 
					_list.RemoveAt( _index );
					
					if( _index - 1 < 0 )
						_list.Add( _obj );
					else
						_list.Insert( _index - 1, _obj );
					
					return;
				}	


				if( ICEEditorLayout.ButtonUp() )
				{
					BehaviourModeRuleObject _obj = _list[_index]; 
					_list.RemoveAt( _index );
					
					if( _index + 1 > _list.Count )
						_list.Insert( 0, _obj );
					else
						_list.Insert( _index +1, _obj );
					
					return;
				}	

				if (GUILayout.Button("COPY", ICEEditorStyle.CMDButtonDouble ))
					_list.Add ( new BehaviourModeRuleObject( _rule ) );
						
				_rule.Enabled = ICEEditorLayout.ButtonCheck( "ENABLED", "Enables and disables the given rule", _rule.Enabled , ICEEditorStyle.ButtonMiddle );
				if (GUILayout.Button("X", ICEEditorStyle.CMDButton ))
				{
					_list.RemoveAt(_index);
					--_index;
				}
				
				ICEEditorLayout.EndHorizontal();
				
				_foldout = _rule.Foldout;
			}
			



			if( _foldout )
			{
				EditorGUI.BeginDisabledGroup( _rule.Enabled == false );
				EditorGUILayout.Separator();
				
				if( _list.Count > 1 || _rule.Links.Enabled )
					_rule = DrawBehaviourCustomLength( _rule );
				
				_rule.Animation = DrawBehaviourAnimation( _control, _rule.Animation );
				
				Vector3 _velocity = Vector3.zero;				
				if( _control.GetComponentInChildren<Renderer>() != null )
					_velocity = _control.GetComponentInChildren<Renderer>().bounds.size;				
				_velocity *= _rule.Animation.GetAnimationLength();
				
				_rule.Move = DrawBehaviourMovements( _control, _rule.Move, _velocity );
				_rule.Influences = EditorSharedTools.DrawSharedInfluences( _rule.Influences, Info.BEHAVIOUR_INFLUENCES, _control.Creature.Status.UseAdvanced );
				_rule.Inventory = EditorSharedTools.DrawSharedInventory( _control, _rule.Inventory, Info.BEHAVIOUR_INVENTORY );
				_rule.Audio = EditorSharedTools.DrawSharedAudio( _rule.Audio, Info.BEHAVIOUR_AUDIO );
				_rule.Effect = EditorSharedTools.DrawSharedEffect( _control, _rule.Effect, Info.BEHAVIOUR_EFFECT );				
				_rule.Links = DrawBehaviourModeRuleLinks( _control, _rule.Links, _list );					

				EditorGUILayout.Separator();
				EditorGUI.EndDisabledGroup();
			}
			
			if( _list.Count > 1 )
				EditorGUI.indentLevel--;
			
		}

		
		private static BehaviourModeRuleObject DrawBehaviourCustomLength( BehaviourModeRuleObject _rule )
		{
			//_rule.UseCustomLength = EditorGUILayout.ToggleLeft("Length", _rule.UseCustomLength, EditorStyles.boldLabel );

			_rule.UseCustomLength = ICEEditorLayout.ToggleLeft( "Length", "", _rule.UseCustomLength, true, Info.BEHAVIOUR_LENGTH );
			if( _rule.UseCustomLength )
			{
				EditorGUI.indentLevel++;

					ICEEditorLayout.BeginHorizontal();
						ICEEditorLayout.MinMaxGroupSimple( "Min/Max Length (secs.)", "Enter the desired Play-Length or press 'RND' to set randomized values.", ref _rule.LengthMin, ref _rule.LengthMax, 0, ref _rule.LengthMaximum, 0.25f, 35 );
						if( ICEEditorLayout.Button( "RND", "", ICEEditorStyle.CMDButtonDouble ) )
						{
							_rule.LengthMax = Random.Range( _rule.LengthMin, _rule.LengthMaximum );
							_rule.LengthMin = Random.Range( 0, _rule.LengthMax );
						}

						if( ICEEditorLayout.Button( "RESET", "", ICEEditorStyle.CMDButtonDouble ) )
						{
							_rule.LengthMin = 0;
							_rule.LengthMax = 0;
						}
					ICEEditorLayout.EndHorizontal( Info.BEHAVIOUR_LENGTH );

				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();
			}

			return _rule;
		}

		
		private static LinkContainer DrawBehaviourModeRuleLinks( ICECreatureControl _control, LinkContainer _link, List<BehaviourModeRuleObject> _list )
		{
			_link.Enabled = ICEEditorLayout.ToggleLeft( "Link","", _link.Enabled, true, Info.BEHAVIOUR_LINK );
			
			if( _link.Enabled )
			{
				EditorGUI.indentLevel++;
				
				if( _list.Count > 1 )
					_link.Link = (LinkType)ICEEditorLayout.EnumPopup( "Link Type","", _link.Link, Info.BEHAVIOUR_LINK_SELECT );
				else
					_link.Link = LinkType.MODE;
				
				if( _link.Link == LinkType.MODE )
				{
					EditorGUI.indentLevel++;
					_link.BehaviourModeKey =  BehaviourSelect( _control, "Next Behaviour Mode", "Next desired Behaviour Mode", _link.BehaviourModeKey, "", Info.BEHAVIOUR_LINK_MODE );
					//BehaviourEditor( _control, _link.BehaviourModeKey );
					EditorGUI.indentLevel--;
					
					EditorGUILayout.Separator();
				}
				else 
				{
					EditorGUI.indentLevel++;
					_link.RuleIndex = (int)ICEEditorLayout.Slider( "Next Rule", "Next desired Behaviour Rule" , _link.RuleIndex + 1, 1, 1 , _list.Count, Info.BEHAVIOUR_LINK_RULE )-1;
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}
				
				
				EditorGUI.indentLevel--;
			}
			
			return _link;
		}

		/// <summary>
		/// Draws the behaviour movements.
		/// </summary>
		/// <returns>The behaviour movements.</returns>
		/// <param name="_control">_control.</param>
		/// <param name="_move">_move.</param>
		/// <param name="default_velocity">Default_velocity.</param>
		private static MoveContainer DrawBehaviourMovements( ICECreatureControl _control, MoveContainer _move, Vector3 default_velocity )
		{
			_move.Enabled = ICEEditorLayout.ToggleLeft("Movement","", _move.Enabled, true,  Info.BEHAVIOUR_MOVEMENTS );			
			if( ! _move.Enabled )
				return _move;

			EditorGUI.indentLevel++;
	
				// VELOCITY BEGIN
				//_move.Velocity.Type = (VelocityType)ICEEditorLayout.EnumPopup( "Velocity","", _move.Velocity.Type, Info.BEHAVIOUR_MOVE_VELOCITY );
				ICEEditorLayout.Label( "Velocity", false );
				EditorGUI.indentLevel++;

					
					ICEEditorLayout.BeginHorizontal();

						//_move.Velocity.Velocity.z = ICEEditorLayout.MaxDefaultSlider( "Forwards (z)", "z-Velocity", _move.Velocity.Velocity.z, 0.01f, (_move.Velocity.UseNegativeVelocity?-_move.Velocity.VelocityMax.z:0), ref _move.Velocity.VelocityMax.z, 0, "" );

						_move.Velocity.Velocity.z = ICEEditorLayout.DefaultSlider( "Forwards (z)", "z-Velocity", _move.Velocity.Velocity.z, Init.MOVE_VELOCITY_FORWARDS_STEP, (_move.Velocity.UseNegativeVelocity?-Init.MOVE_VELOCITY_FORWARDS_MAX:Init.MOVE_VELOCITY_FORWARDS_MIN), Init.MOVE_VELOCITY_FORWARDS_MAX, Init.MOVE_VELOCITY_FORWARDS_DEFAULT );
						_move.Velocity.UseNegativeVelocity = ICEEditorLayout.ButtonCheck( "NEG", "Allows negative velocity settings", _move.Velocity.UseNegativeVelocity, ICEEditorStyle.CMDButtonDouble );
						_move.Velocity.UseTargetVelocity = ICEEditorLayout.ButtonCheck( "AUTO", "Adapts the velocity automatically to target", _move.Velocity.UseTargetVelocity, ICEEditorStyle.CMDButtonDouble );
					
						bool _adv = false;
						if( _move.Velocity.Type == VelocityType.ADVANCED )
							_adv = true;

						_adv = ICEEditorLayout.ButtonCheck( "ADV", "Use advanced velocity settings", _adv, ICEEditorStyle.CMDButtonDouble );

						if( _adv )
							_move.Velocity.Type = VelocityType.ADVANCED;
						else
							_move.Velocity.Type = VelocityType.DEFAULT;				   
				   	ICEEditorLayout.EndHorizontal( Info.BEHAVIOUR_MOVE_VELOCITY );

					if( _move.Velocity.Type == VelocityType.ADVANCED )
					{
						EditorGUI.indentLevel++;
							ICEEditorLayout.MinMaxGroup( "Variance (neg/pos)", "", ref _move.Velocity.VelocityMinVariance, ref _move.Velocity.VelocityMaxVariance, -1, 1, 0.01f, Info.BEHAVIOUR_MOVE_VELOCITY_VARIANCE );
							_move.Velocity.Inertia = ICEEditorLayout.DefaultSlider( "Mass Inertia", "", _move.Velocity.Inertia, 0.01f, 0, 1, 0, Info.BEHAVIOUR_MOVE_VELOCITY_INERTIA ); 
						EditorGUI.indentLevel--;
						_move.Velocity.Velocity.x = ICEEditorLayout.DefaultSlider( "Sidewards \t(x)", "x-Velocity", _move.Velocity.Velocity.x, 0.1f, -25, 25, 0, Info.BEHAVIOUR_MOVE_VELOCITY_SIDEWARDS);
						/*if( _move.Velocity.UseAutoDrift )
						{
							EditorGUI.indentLevel++;
								_move.Velocity.DriftMultiplier = ICEEditorLayout.DefaultSlider( "Drift Multiplier" , "",_move.Velocity.DriftMultiplier, 0.01f, 0, 1, 0, Info.BEHAVIOUR_MOVE_VELOCITY_DRIFT );
							EditorGUI.indentLevel--;
						}*/
						
					//_move.Velocity.Velocity.y = ICEEditorLayout.DefaultSlider( "Vertical \t(y)", "y-Velocity", _move.Velocity.Velocity.y, 0.5f, -25, 25, 0,Info.BEHAVIOUR_MOVE_VELOCITY_VERTICAL );
						
					}
					_move.Velocity.AngularVelocity = ICEEditorLayout.DefaultSlider( "Angular (y)", "", _move.Velocity.AngularVelocity, 0.25f, 0, 50, (_move.Velocity.Velocity.z / 2), Info.BEHAVIOUR_MOVE_ANGULAR_VELOCITY );  

					if( _move.Velocity.AngularVelocityAuto )
						_move.Velocity.AngularVelocity =  ( ( 25 / _move.Velocity.Velocity.z / 4  ) );
				EditorGUI.indentLevel--;
				// VELOCITY END

				// VIEWING DIRECTION BEGIN
				EditorGUILayout.Separator();					
				_move.ViewingDirection = (ViewingDirectionType)ICEEditorLayout.EnumPopup( "Viewing Direction", "", _move.ViewingDirection, Info.BEHAVIOUR_MOVE_VIEWING_DIRECTION );

				if( _move.ViewingDirection == ViewingDirectionType.POSITION )
					_move = DrawBehaviourModeRuleMoveViewingDirection( _control, _move );
				// VIEWING DIRECTION END

				// BEHAVIOUR MOVE BEGIN
				string _move_help = Info.BEHAVIOUR_MOVE_DEFAULT;
				if( _move.Type == MoveType.RANDOM )
					_move_help = Info.BEHAVIOUR_MOVE_RANDOM;
				else if( _move.Type == MoveType.CUSTOM )
					_move_help = Info.BEHAVIOUR_MOVE_CUSTOM;
				else if( _move.Type == MoveType.ESCAPE )
					_move_help = Info.BEHAVIOUR_MOVE_ESCAPE;
				else if( _move.Type == MoveType.AVOID )
					_move_help = Info.BEHAVIOUR_MOVE_AVOID;
				else if( _move.Type == MoveType.ORBIT )
					_move_help = Info.BEHAVIOUR_MOVE_ORBIT;
				else if( _move.Type == MoveType.DETOUR )
					_move_help = Info.BEHAVIOUR_MOVE_DETOUR;
					
				MoveType _move_type = (MoveType)ICEEditorLayout.EnumPopup( "Move", "", _move.Type, _move_help );					
				if( _move_type != _move.Type )
				{
					_move.MoveSegmentLength = _control.Creature.Move.DefaultMove.MoveSegmentLength;
					_move.MoveSegmentVariance = _control.Creature.Move.DefaultMove.MoveSegmentVariance;
					_move.MoveStopDistance = _control.Creature.Move.DefaultMove.MoveStopDistance;
					_move.MoveDeviationVariance = _control.Creature.Move.DefaultMove.MoveDeviationVariance;
					_move.MoveIgnoreLevelDifference = _control.Creature.Move.DefaultMove.MoveIgnoreLevelDifference;
				}

				_move.Type = _move_type;				

				EditorGUI.indentLevel++;					
					if( _move.Type == MoveType.RANDOM )
						_move = DrawBehaviourModeRuleMoveRandom( _control, _move );
					else if( _move.Type == MoveType.CUSTOM )
						_move = DrawBehaviourModeRuleMoveCustom( _control, _move );
					else if( _move.Type == MoveType.ESCAPE )
						_move = DrawBehaviourModeRuleMoveEscape( _control, _move );
					else if( _move.Type == MoveType.AVOID )
						_move = DrawBehaviourModeRuleMoveAvoid( _control, _move );
					else if( _move.Type == MoveType.ORBIT )
						_move = DrawBehaviourModeRuleMoveOrbit( _control, _move );
					else if( _move.Type == MoveType.DETOUR )
						_move = DrawBehaviourModeRuleMoveDetour( _control, _move );					
				EditorGUI.indentLevel--;
				// BEHAVIOUR MOVE END
			
			EditorGUI.indentLevel--;

			
			return _move;
		}


		private static MoveContainer DrawBehaviourModeRuleMoveCustom( ICECreatureControl _control, MoveContainer _move )
		{
			EditorGUILayout.Separator();
			EditorSharedTools.DrawMove( 
				ref _move.MoveSegmentLength, 
				ref _move.MaxMoveSegmentLength, 
				ref _move.MoveStopDistance, 
				ref _move.MoveSegmentVariance, 
				ref _move.MoveDeviationLength,
				ref _move.MoveDeviationLengthMax,
				ref _move.MoveDeviationVariance, 
				ref _move.MoveIgnoreLevelDifference );

			return _move;
		}

		private static MoveContainer DrawBehaviourModeRuleMoveAvoid( ICECreatureControl _control, MoveContainer _move )
		{
			EditorGUILayout.Separator();
			
			_move.Avoid.AvoidDistance = ICEEditorLayout.DefaultSlider( "Max. Avoid Distance", "", _move.Avoid.AvoidDistance, Init.MOVE_AVOID_DISTANCE_STEP, Init.MOVE_AVOID_DISTANCE_MIN, Init.MOVE_AVOID_DISTANCE_MAX, Init.MOVE_AVOID_DISTANCE_DEFAULT, Info.BEHAVIOUR_MOVE_AVOID_DISTANCE );  

			EditorGUILayout.Separator();
			EditorSharedTools.DrawMove( 
				ref _move.MoveSegmentLength, 
				ref _move.MaxMoveSegmentLength, 
				ref _move.MoveStopDistance, 
				ref _move.MoveSegmentVariance, 
				ref _move.MoveDeviationLength,
				ref _move.MoveDeviationLengthMax,
				ref _move.MoveDeviationVariance, 
				ref _move.MoveIgnoreLevelDifference );

			return _move;
		}


		private static MoveContainer DrawBehaviourModeRuleMoveEscape( ICECreatureControl _control, MoveContainer _move )
		{
			_move.Escape.EscapeDistance = ICEEditorLayout.DefaultSlider( "Escape Distance", "", _move.Escape.EscapeDistance, Init.MOVE_ESCAPE_DISTANCE_STEP, Init.MOVE_ESCAPE_DISTANCE_MIN, Init.MOVE_ESCAPE_DISTANCE_MAX, Init.MOVE_ESCAPE_DISTANCE_DEFAULT, Info.BEHAVIOUR_MOVE_ESCAPE_DISTANCE );  
			_move.Escape.RandomEscapeAngle = ICEEditorLayout.DefaultSlider( "Random Escape Angle", "", _move.Escape.RandomEscapeAngle, Init.MOVE_ESCAPE_RANDOM_ANGLE_STEP, Init.MOVE_ESCAPE_RANDOM_ANGLE_MIN, Init.MOVE_ESCAPE_RANDOM_ANGLE_MAX, Init.MOVE_ESCAPE_RANDOM_ANGLE_DEFAULT, Info.BEHAVIOUR_MOVE_ESCAPE_ANGLE );  
			EditorGUILayout.Separator();
			EditorSharedTools.DrawMove( 
				ref _move.MoveSegmentLength, 
				ref _move.MaxMoveSegmentLength, 
				ref _move.MoveStopDistance, 
				ref _move.MoveSegmentVariance, 
				ref _move.MoveDeviationLength,
				ref _move.MoveDeviationLengthMax,
				ref _move.MoveDeviationVariance, 
				ref _move.MoveIgnoreLevelDifference );
			return _move;
		}
		
		private static MoveContainer DrawBehaviourModeRuleMoveOrbit( ICECreatureControl _control, MoveContainer _move )
		{
			EditorGUILayout.Separator();
			
			_move.Orbit.Radius = ICEEditorLayout.DefaultSlider( "Orbit Radius", "", _move.Orbit.Radius, Init.MOVE_ORBIT_RADIUS_STEP, _move.MoveStopDistance, Init.MOVE_ORBIT_RADIUS_MAX, Init.MOVE_ORBIT_RADIUS_DEFAULT );  
			
			EditorGUI.indentLevel++;
			
			_move.Orbit.RadiusShift = ICEEditorLayout.DefaultSlider( "Radius Shift", "", _move.Orbit.RadiusShift, Init.MOVE_ORBIT_SHIFT_STEP, - _move.Orbit.Radius , _move.Orbit.Radius , Init.MOVE_ORBIT_SHIFT_DEFAULT );  
			
			if( _move.Orbit.RadiusShift < 0 )
				_move.Orbit.MinDistance = ICEEditorLayout.DefaultSlider( "Min Distance", "", _move.Orbit.MinDistance, Init.MOVE_ORBIT_SHIFT_STEP, _move.MoveStopDistance, _move.Orbit.Radius, _move.MoveStopDistance );  
			else if( _move.Orbit.RadiusShift > 0 )
				_move.Orbit.MaxDistance = ICEEditorLayout.DefaultSlider( "Max Distance", "", _move.Orbit.MaxDistance, Init.MOVE_ORBIT_SHIFT_STEP, _move.Orbit.Radius, Init.MOVE_ORBIT_RADIUS_MAX, _move.Orbit.Radius + _move.MoveStopDistance );  
			
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
			EditorSharedTools.DrawMove( 
				ref _move.MoveSegmentLength, 
				ref _move.MaxMoveSegmentLength, 
				ref _move.MoveStopDistance, 
				ref _move.MoveSegmentVariance, 
				ref _move.MoveDeviationLength,
				ref _move.MoveDeviationLengthMax,
				ref _move.MoveDeviationVariance, 
				ref _move.MoveIgnoreLevelDifference );

			return _move;
		}

		
		private static MoveContainer DrawBehaviourModeRuleMoveRandom( ICECreatureControl _control, MoveContainer _move )
		{
			EditorGUILayout.Separator();
			EditorSharedTools.DrawMove( 
				ref _move.MoveSegmentLength, 
				ref _move.MaxMoveSegmentLength, 
				ref _move.MoveStopDistance, 
				ref _move.MoveSegmentVariance, 
				ref _move.MoveDeviationLength,
				ref _move.MoveDeviationLengthMax,
				ref _move.MoveDeviationVariance, 
				ref _move.MoveIgnoreLevelDifference );
			
			return _move;
		}

		private static bool _add_viewing_position = false;
		private static MoveContainer DrawBehaviourModeRuleMoveViewingDirection( ICECreatureControl _control, MoveContainer _move )
		{
			EditorGUI.indentLevel++;
			ICEEditorLayout.BeginHorizontal();
			
			if( _add_viewing_position )
			{
				Transform _tmp_transform = (Transform)EditorGUILayout.ObjectField("Position", null, typeof(Transform), true);
				
				if( _tmp_transform != null )
				{
					_move.ViewingDirectionPosition = _tmp_transform.position;
					_add_viewing_position = false;
				}
				
			}
			else
				_move.ViewingDirectionPosition = EditorGUILayout.Vector3Field( "Position", _move.ViewingDirectionPosition );
			
			ICEEditorLayout.ButtonShowObject(  _move.ViewingDirectionPosition ); 


			if( _add_viewing_position )
				GUI.backgroundColor = Color.yellow;
			
			if (GUILayout.Button("ADD", ICEEditorStyle.ButtonMiddle ) )
				_add_viewing_position = ! _add_viewing_position;
			
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			
			
			ICEEditorLayout.EndHorizontal();
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
			
			
			return _move;
		}

		private static bool _add_position = false;
		private static MoveContainer DrawBehaviourModeRuleMoveDetour( ICECreatureControl _control, MoveContainer _move )
		{
			ICEEditorLayout.BeginHorizontal();
	
				if( _add_position )
				{
					Transform _tmp_transform = (Transform)EditorGUILayout.ObjectField("Detour Position", null, typeof(Transform), true);
					
					if( _tmp_transform != null )
					{
						_move.Detour.Position = _tmp_transform.position;
						_add_position = false;
					}
					
				}
				else
					_move.Detour.Position = EditorGUILayout.Vector3Field( "Detour Position", _move.Detour.Position );

				ICEEditorLayout.ButtonShowObject( _move.Detour.Position );
				
				if( _add_position )
					GUI.backgroundColor = Color.yellow;

				if (GUILayout.Button("ADD", ICEEditorStyle.ButtonMiddle ) )
					_add_position = ! _add_position;

				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;


			ICEEditorLayout.EndHorizontal();

			EditorSharedTools.DrawMove( 
				ref _move.MoveSegmentLength, 
				ref _move.MaxMoveSegmentLength, 
				ref _move.MoveStopDistance, 
				ref _move.MoveSegmentVariance, 
				ref _move.MoveDeviationLength,
				ref _move.MoveDeviationLengthMax,
				ref _move.MoveDeviationVariance, 
				ref _move.MoveIgnoreLevelDifference );
		
			EditorGUILayout.Separator();

			
			return _move;
		}

		public static void BehaviourEditor( ICECreatureControl _control, string _key )
		{
			if( BehaviourSelectEditor[BehaviourSelectIndex] == false &&  _key.Trim() == ""  )
				return;

			//if( _key.Trim() == "" || _key.Trim() != m_edit_key )
			//	return;

			BehaviourModeObject _mode = _control.Creature.Behaviour.GetBehaviourModeByKey ( _key );
			
			if( _mode == null )
				return;
			
			
			ICEEditorLayout.BeginHorizontal();
			EditorGUILayout.BeginVertical( "box" );
			
			_mode.Foldout = true;
			int _indent = EditorGUI.indentLevel;
			
			EditorGUI.indentLevel = 1;
			
			EditorGUILayout.Separator();
			
			ICEEditorLayout.BeginHorizontal();
			EditorGUILayout.LabelField( _mode.Key + " (" + _mode.Rules.Count + (_mode.Rules.Count == 1?" Rule)":" Rules)"), EditorStyles.boldLabel );
			GUILayout.FlexibleSpace();

			if( _mode.Rules.Count == 1 )
			{
				if (GUILayout.Button("COPY", ICEEditorStyle.ButtonMiddle ))
					_mode.Rules.Add( new BehaviourModeRuleObject( _mode.Rules[0] ) );	
			}

			if (GUILayout.Button("ADD RULE", ICEEditorStyle.ButtonMiddle ))
				_mode.Rules.Add( new BehaviourModeRuleObject() );					
			
			ICEEditorLayout.EndHorizontal( Info.BEHAVIOUR_MODE );
			EditorGUILayout.Separator();
			
			EditorGUI.indentLevel++;
			
			if ( _mode.Foldout ) 
				DrawBehaviourModeContent( _control, _mode );
			
			EditorGUI.indentLevel--;
			
			EditorGUI.indentLevel = _indent;
			
			EditorGUILayout.EndVertical();
			ICEEditorLayout.EndHorizontal();
			
		}

		public static int BehaviourSelectIndex = 0;
		public static bool[] BehaviourSelectEditor = new bool[1000];
		public static string BehaviourSelect( ICECreatureControl _control, string _title, string _hint, string _key, string _default_key = "", string _help = ""  )
		{
			ICEEditorLayout.BeginHorizontal();
			_key = BasicBehaviourSelect( _control, _title, _hint, _key, _default_key , _help );
			ICEEditorLayout.EndHorizontal( _help );

			if( BehaviourSelectEditor[BehaviourSelectIndex] )
				BehaviourEditor( _control, _key );

			return _key;
		}

		public static string BasicBehaviourSelect( ICECreatureControl _control, string _title, string _hint, string _key, string _default_key = "", string _help = ""  )
		{
			BehaviourSelectIndex++;
			
			if( _control.Creature.Behaviour.BehaviourModes.Count > 0 && _key != "NEW" )
			{
				_key = BehaviourPopup( _control,_title, _hint, _key );
				
				if (GUILayout.Button( new GUIContent("NEW", "Creates a new Behaviour Mode"), ICEEditorStyle.ButtonMiddle ) )
				{	
					m_old_key = _key;
					_key = "NEW";
					m_new_key = "";
				}
				
				if( _key.Trim() != "" && _key != "NEW" )
				{
					if( BehaviourSelectEditor[BehaviourSelectIndex] )
						GUI.backgroundColor = Color.yellow;
					
					if (GUILayout.Button( new GUIContent("EDIT", "Creates a new Behaviour Mode"), ICEEditorStyle.ButtonMiddle ) )
						BehaviourSelectEditor[BehaviourSelectIndex] = ! BehaviourSelectEditor[BehaviourSelectIndex];
					
					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
				}
				
				if( _key.Trim() == "" && _default_key.Trim() != "" )
				{
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button( "AUTO", ICEEditorStyle.ButtonMiddle ))
						_key = EditorWizard.WizardBehaviour( _control, _default_key );
					
					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
				}
			}
			else
			{
				m_new_key = EditorGUILayout.TextField( new GUIContent( _title, "Name of a new Behaviour") , m_new_key );
				
				if (GUILayout.Button( new GUIContent("ADD", "Creates a new Behaviour Mode"), ICEEditorStyle.ButtonMiddle ) )
				{	
					if( m_new_key != "" )
					{
						_key = _control.Creature.Behaviour.AddBehaviourMode( m_new_key );
						m_new_key = "";
					}
				}
				
				if( m_old_key.Trim() != "" || _control.Creature.Behaviour.BehaviourModes.Count > 0 )
				{
					if (GUILayout.Button( "BACK", ICEEditorStyle.ButtonMiddle ))
					{
						_key = m_old_key;
						m_new_key = "";
					}
				}
				
				if( _default_key.Trim() != "" )
				{
					GUI.backgroundColor = Color.green;
					if (GUILayout.Button( "AUTO", ICEEditorStyle.ButtonMiddle ))
						_key = EditorWizard.WizardBehaviour( _control, _default_key );
		
					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
				}
			}

			return _key;
		}
		
		
		public static string BehaviourPopup( ICECreatureControl _creature_control, string _title, string _hint, string _key )
		{
			string _new_key = "";
			if( _creature_control.Creature.Behaviour.BehaviourModes.Count == 0 )
			{
				EditorGUILayout.LabelField( _title );
				return _new_key;
			}
			else
			{
				GUIContent[] _options = new GUIContent[ _creature_control.Creature.Behaviour.BehaviourModes.Count + 1];
				int _options_index = 0;
				
				_options[0] = new GUIContent( " ");
				for( int i = 0 ; i < _creature_control.Creature.Behaviour.BehaviourModes.Count ; i++ )
				{
					BehaviourModeObject _mode = _creature_control.Creature.Behaviour.BehaviourModes[i];
					
					int _index = i + 1;
					
					_options[ _index ] = new GUIContent( _mode.Key );
					
					if( _mode.Key == _key )
					{
						_options_index = _index;
					}
				}


				_options_index = EditorGUILayout.Popup( new GUIContent( _title, _hint), _options_index , _options  );
				
				_new_key = _options[ _options_index ].text;
			}
			
			return _new_key;
		}

		public static string BehaviourPopup( ICECreatureControl _creature_control, string _key )
		{
			string _new_key = "";
			if( _creature_control.Creature.Behaviour.BehaviourModes.Count == 0 )
			{
				EditorGUILayout.LabelField( "" );
				return _new_key;
			}
			else
			{
				GUIContent[] _options = new GUIContent[ _creature_control.Creature.Behaviour.BehaviourModes.Count + 1];
				int _options_index = 0;
				
				_options[0] = new GUIContent( " ");
				for( int i = 0 ; i < _creature_control.Creature.Behaviour.BehaviourModes.Count ; i++ )
				{
					BehaviourModeObject _mode = _creature_control.Creature.Behaviour.BehaviourModes[i];
					
					int _index = i + 1;
					
					_options[ _index ] = new GUIContent( _mode.Key );
					
					if( _mode.Key == _key )
					{
						_options_index = _index;
					}
				}
				
				
				_options_index = EditorGUILayout.Popup( _options_index , _options );
				
				_new_key = _options[ _options_index ].text;
			}
			
			return _new_key;
		}

		
		
		private static AnimationClipDataContainer DrawBehaviourAnimationAnimationClipData( ICECreatureControl _control, AnimationClipDataContainer _clip )
		{
			Animation m_animation = _control.GetComponentInChildren<Animation>();

			if( m_animation != null && m_animation.enabled == true )
			{
				Info.Help ( Info.BEHAVIOUR_ANIMATION_CLIP );
				
				_clip.Clip = (AnimationClip)EditorGUILayout.ObjectField( "Animation Clip", _clip.Clip, typeof(AnimationClip), false );
				
				if( _clip.Clip != null )
				{
					ICEEditorLayout.Label( "Length", "Animation length in seconds. ", _clip.Clip.length.ToString() + " secs." );
					ICEEditorLayout.Label( "Frame Rate", "This is the frame rate that was used in the animation program you used to create the animation or model.", _clip.Clip.frameRate.ToString() + " secs." );
					
					_clip.Clip.legacy = ICEEditorLayout.Toggle( "Legacy", "Set to true to use it here with the Legacy Animation component",_clip.Clip.legacy );
					_clip.Clip.wrapMode = (WrapMode)ICEEditorLayout.EnumPopup( "WarpMode", "Determines how time is treated outside of the keyframed range of an AnimationClip or AnimationCurve." , _clip.Clip.wrapMode );
					
					bool _toggle = false;
					_clip.TransitionDuration = ICEEditorLayout.AutoSlider( "Transition Duration", "", _clip.TransitionDuration, 0.01f, 0, 10, ref _toggle, 0.5f  );
					
					if( _toggle )
						_clip.TransitionDuration = _clip.Clip.length / 3;
					
					_toggle = false;
				}
			}
			else
			{
				EditorGUILayout.HelpBox( "Check your Animation Component", MessageType.Warning ); 
			}
			
			return _clip;
		}

		private static int AnimationPopup( Animation _animation, int _selected, string _title = "" )
		{
			ICEEditorLayout.BeginHorizontal();
				_selected = AnimationPopupBase( _animation, _selected, _title );
			ICEEditorLayout.EndHorizontal();

			return _selected;
		}

		public static int AnimationPopupBase( Animation _animation, int _selected, string _title = "" )
		{
			if( _animation == null )
				return 0;



				string[] _animation_names = new string[ _animation.GetClipCount() ];
				int[] _animation_index = new int[ _animation.GetClipCount() ];
				
				int i = 0;	
				foreach (AnimationState _animation_state in _animation )
				{
					_animation_index[i] = i;
					_animation_names[i] = _animation_state.name;
					
					i++;
				}
				if( _title == "" )
					_title = "Animation";

				_selected = (int)EditorGUILayout.IntPopup( _title , _selected, _animation_names,_animation_index);
			//new GUIContent( _title , "Animation name and length in seconds" )

				_selected = (int)ICEEditorLayout.PlusMinusGroup( _selected, 1, ICEEditorStyle.CMDButtonDouble );

				if( _selected < 0 )
					_selected = 0;
				else if( _selected >= _animation.GetClipCount() - 1 )
					_selected = _animation.GetClipCount() - 1;

			return _selected;
		}

		private static int AnimPopup( ICECreatureControl _control, string _title, int _selected )
		{
			Animator _animator = _control.GetComponentInChildren<Animator>();
			Animation _animation = _control.GetComponentInChildren<Animation>();
			
			if( _animator != null && _animator.enabled == true && _animator.runtimeAnimatorController != null && _animator.avatar != null )
				_selected = AnimatorPopup( _animator, _selected, _title);
			else if( _animation != null && _animation.enabled == true )
				_selected = AnimationPopup( _animation, _selected, _title);

			return _selected;
		}

		private static AnimationDataContainer DrawBehaviourAnimationAnimationData( ICECreatureControl _control, AnimationDataContainer _animation_data )
		{
			Animation m_animation = _control.GetComponentInChildren<Animation>();

			if( m_animation != null && m_animation.enabled == true )
			{
				Info.Help ( Info.BEHAVIOUR_ANIMATION_ANIMATION );
				
				if( EditorApplication.isPlaying )
				{
					EditorGUILayout.LabelField("Name", _animation_data.Name );
				}
				else
				{
					_animation_data.Index = AnimationPopup( m_animation, _animation_data.Index, "Animation (" + _animation_data.Length.ToString() + " secs.)" );

					AnimationState _state = GetAnimationStateByIndex( _control, _animation_data.Index );
					
					if( _state != null )
					{				
						if( _state.clip != null )
							_state.clip.legacy = true;

						if( _animation_data.Name != _state.name )
						{
							_animation_data.Name = _state.name;
							_animation_data.Length = _state.length;
							_animation_data.Speed =_state.speed;
							_animation_data.TransitionDuration = 0.5f;
							_animation_data.wrapMode = _state.wrapMode;

							_animation_data.Length = _state.length;
							_animation_data.DefaultSpeed = _state.speed;
							_animation_data.DefaultWrapMode = _state.wrapMode;
						}
					}
				}
		
				EditorGUI.indentLevel++;

				_animation_data.wrapMode = (WrapMode)ICEEditorLayout.EnumPopup( "WrapMode (" + _animation_data.DefaultWrapMode + ")", "Determines how time is treated outside of the keyframed range of an AnimationClip or AnimationCurve.", _animation_data.wrapMode );
				_animation_data.Speed = ICEEditorLayout.AutoSlider( "Speed (" + _animation_data.DefaultSpeed + ")", "The playback speed of the animation. 1 is normal playback speed. A negative playback speed will play the animation backwards. Adapt this value to your movement settings.", _animation_data.Speed, 0.01f, -10, 10, ref _animation_data.AutoSpeed, 1 );

				bool _toggle = false;
				_animation_data.TransitionDuration = ICEEditorLayout.AutoSlider( "Transition Duration", "", _animation_data.TransitionDuration, 0.01f, 0, 10, ref _toggle, 0.5f  );
				
				if( _toggle )
					_animation_data.TransitionDuration = _animation_data.Length / 3;
				
				_toggle = false;
				
				EditorGUI.indentLevel--;
				
			}
			else
			{
				EditorGUILayout.HelpBox( "Check your Animation Component", MessageType.Warning ); 
			}
			
			return _animation_data;
		}

		private static int AnimatorPopup( Animator _animator, int _selected, string _title = "" )
		{
			ICEEditorLayout.BeginHorizontal();
				_selected = AnimatorPopupBase( _animator, _selected, _title );
			ICEEditorLayout.EndHorizontal();
			
			return _selected;
		}

		public static int AnimatorPopupBase( Animator _animator, int _selected, string _title = "" )
		{
			AnimationClip[] _clips = AnimationUtility.GetAnimationClips( _animator.gameObject );

			string[] _animation_names = new string[ _clips.Length ];//_animator.runtimeAnimatorController.animationClips.Length ];
			int[] _animation_index = new int[  _clips.Length ];//_animator.runtimeAnimatorController.animationClips.Length ];
			
			int i = 0;							
			foreach( AnimationClip _clip in _clips )// _animator.runtimeAnimatorController.animationClips )
			{
				_animation_index[i] = i;
				_animation_names[i] = _clip.name;
				
				i++;
			}
			
			if( _title == "" )
				_title = "Animation";
			
			_selected = (int)EditorGUILayout.IntPopup( _title , _selected, _animation_names,_animation_index);
			
			if (GUILayout.Button("<", ICEEditorStyle.CMDButtonDouble ))
			{
				_selected--;
				if( _selected < 0 ){ 
					_selected = _animator.runtimeAnimatorController.animationClips.Length-1;
				}
			}
			if (GUILayout.Button(">", ICEEditorStyle.CMDButtonDouble))
			{
				_selected++;
				if( _selected >= _animator.runtimeAnimatorController.animationClips.Length ){ 
					_selected = 0;
				}
			}

			return _selected;
		}

		private static List<AnimatorParameterObject> DrawBehaviourAnimationAnimatorParameterData( ICECreatureControl _control, List<AnimatorParameterObject> _parameter_list )
		{
			Animator _animator = _control.GetComponent<Animator>();

			for( int _i = 0 ; _i < _parameter_list.Count; _i++ )
			{
				AnimatorParameterObject _parameter = _parameter_list[_i];
				var indent = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 0;
				
				
				// PARAMETER LINE BEGIN
				ICEEditorLayout.BeginHorizontal();

				EditorGUILayout.LabelField("",GUILayout.Width( 15 * indent ) );

				EditorGUI.BeginDisabledGroup( _parameter.Enabled == false );
				AnimatorControllerParameter _data = ICEEditorLayout.AnimatorParametersPopupData( _animator, _parameter.Name, GUILayout.MinWidth( 65 ), GUILayout.MaxWidth( 200 ) );

					if( _data != null )
					{
						_parameter.Name = _data.name;
						_parameter.Type = _data.type;

						if( _parameter.Type == AnimatorControllerParameterType.Bool )
						{
							if( _parameter.UseDynamicValue )
							_parameter.BooleanValueType = (DynamicBooleanValueType)EditorGUILayout.EnumPopup( _parameter.BooleanValueType, GUILayout.MinWidth( 65 ) );
							else
							_parameter.BooleanValue = ICEEditorLayout.ButtonCheck( (_parameter.BooleanValue?"TRUE":"FALSE" ),"", _parameter.BooleanValue, ICEEditorStyle.ButtonFlex, GUILayout.MinWidth( 65 ) );
						}
						else if( _parameter.Type == AnimatorControllerParameterType.Int )
						{
							if( _parameter.UseDynamicValue )
								_parameter.IntegerValueType = (DynamicIntegerValueType)EditorGUILayout.EnumPopup( _parameter.IntegerValueType );
							else
								_parameter.IntegerValue = EditorGUILayout.IntField( _parameter.IntegerValue, GUILayout.MinWidth( 65 ) );
						}
						else if( _parameter.Type == AnimatorControllerParameterType.Float )
						{
							if( _parameter.UseDynamicValue )
								_parameter.FloatValueType = (DynamicFloatValueType)EditorGUILayout.EnumPopup( _parameter.FloatValueType );
							else
								_parameter.FloatValue = EditorGUILayout.FloatField( _parameter.FloatValue, GUILayout.MinWidth( 65 ) );
						}
						else if( _parameter.Type == AnimatorControllerParameterType.Trigger )
						{
						}

						_parameter.UseDynamicValue = ICEEditorLayout.ButtonCheck( "DYN", "Use dynamic value", _parameter.UseDynamicValue, ICEEditorStyle.CMDButtonDouble );
					}
				EditorGUI.EndDisabledGroup();

				_parameter.Enabled = ICEEditorLayout.ButtonCheck( "ENABLED", "Use dynamic value", _parameter.Enabled, ICEEditorStyle.ButtonMiddle );

				if( ICEEditorLayout.Button( "x", "", ICEEditorStyle.CMDButton ) ) 
				{
					_parameter_list.RemoveAt(_i);
					return _parameter_list;
				}

				ICEEditorLayout.EndHorizontal(); // Info.GetTargetSelectionExpressionTypeHint( _condition.ExpressionType )
				// PARAMETER LINE END


				EditorGUI.indentLevel = indent;
				

	
			}

			// ADD CONDITION LINE BEGIN
			ICEEditorLayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			
			EditorGUILayout.LabelField( new GUIContent( "Add Parameter ", "" ), EditorStyles.wordWrappedMiniLabel );
			
			
			//GUILayout.FlexibleSpace();
			if (GUILayout.Button( new GUIContent( "ADD", "Add parameter" ) , ICEEditorStyle.CMDButtonDouble ) )
				_parameter_list.Add( new AnimatorParameterObject() );
			
			if (GUILayout.Button( new GUIContent( "DEL", "Removes all parameter" ) , ICEEditorStyle.CMDButtonDouble ) )
			{
				_parameter_list.Clear();
				return _parameter_list;
			}
			
			ICEEditorLayout.EndHorizontal();

			return _parameter_list;
		}

		private static AnimatorDataContainer DrawBehaviourAnimationAnimatorData( ICECreatureControl _control, AnimatorDataContainer _animator_data )
		{
			Animator m_animator = _control.GetComponent<Animator>();

			if( m_animator != null && m_animator.enabled == true && m_animator.runtimeAnimatorController != null && m_animator.avatar != null )
			{
				if( ! EditorApplication.isPlaying )
				{
					string _help_control_type = Info.BEHAVIOUR_ANIMATION_ANIMATOR_CONTROL_TYPE_DIRECT;

					if( _animator_data.Type == AnimatorControlType.ADVANCED )
						_help_control_type = Info.BEHAVIOUR_ANIMATION_ANIMATOR_CONTROL_TYPE_ADVANCED;

					_animator_data.Type = (AnimatorControlType)ICEEditorLayout.EnumPopup( "Control Type", "", _animator_data.Type, _help_control_type );
					
					if( _animator_data.Type == AnimatorControlType.DIRECT )
					{
						_animator_data.Index = AnimatorPopup( m_animator, _animator_data.Index );

						if( m_animator.runtimeAnimatorController.animationClips.Length == 0 )
						{
							Info.Warning( Info.BEHAVIOUR_ANIMATION_ANIMATOR_ERROR_NO_CLIPS );
						}
						else
						{
							AnimationClip _animation_clip = m_animator.runtimeAnimatorController.animationClips[_animator_data.Index];

							if( _animation_clip != null )
							{				
								if( _animator_data.Name != _animation_clip.name )
									_animator_data.Init();

								_animation_clip.wrapMode = (WrapMode)ICEEditorLayout.EnumPopup( "WarpMode", "Determines how time is treated outside of the keyframed range of an AnimationClip or AnimationCurve.", _animation_clip.wrapMode );
								_animation_clip.legacy = false;

								_animator_data.Name = _animation_clip.name;
								_animator_data.Length = _animation_clip.length;
								_animator_data.DefaultWrapMode = _animation_clip.wrapMode;

								_animator_data.Speed = ICEEditorLayout.AutoSlider("Speed", "", _animator_data.Speed, 0.01f, -5, 5, ref _animator_data.AutoSpeed, 1 );

								bool _toggle = false;
								_animator_data.TransitionDuration = ICEEditorLayout.AutoSlider( "Transition Duration", "", _animator_data.TransitionDuration, 0.01f, 0, 10, ref _toggle, 0.5f  );
								
								if( _toggle )
									_animator_data.TransitionDuration = _animator_data.Length / 3;						
								_toggle = false;
							}
						}
						

						
					}
					else if( _animator_data.Type == AnimatorControlType.ADVANCED )
					{
						_animator_data.ApplyRootMotion = ICEEditorLayout.Toggle( "Apply Root Motion", "", _animator_data.ApplyRootMotion );

						_animator_data.Parameters = DrawBehaviourAnimationAnimatorParameterData( _control, _animator_data.Parameters );

					}
					
				}
				else
				{
					
					if( _animator_data.Type == AnimatorControlType.DIRECT )
					{
						ICEEditorLayout.Label( "Name", "Animation name.", _animator_data.Name );
						ICEEditorLayout.Label( "Length", "Animation length in seconds.", _animator_data.Length.ToString() + " secs." );
						ICEEditorLayout.Label( "WrapMode", "Determines how time is treated outside of the keyframed range of an AnimationClip.", _animator_data.DefaultWrapMode.ToString() );

						_animator_data.Speed = ICEEditorLayout.AutoSlider("Speed", "", _animator_data.Speed, 0.01f, -5, 5, ref _animator_data.AutoSpeed, 1 );


						bool _toggle = false;
						_animator_data.TransitionDuration = ICEEditorLayout.AutoSlider( "Transition Duration", "", _animator_data.TransitionDuration, 0.01f, 0, 10, ref _toggle, 0.5f  );

						if( _toggle )
							_animator_data.TransitionDuration = _animator_data.Length / 3;						
						_toggle = false;
					}
					else if( _animator_data.Type == AnimatorControlType.ADVANCED )
					{
						foreach( AnimatorParameterObject _parameter in _animator_data.Parameters )
						{
							switch( _parameter.Type )
							{
							case AnimatorControllerParameterType.Bool:
								EditorGUILayout.LabelField( _parameter.Name, "(BOOLEAN) " +  _parameter.BooleanValue.ToString() );
								break;
							case AnimatorControllerParameterType.Int:
								EditorGUILayout.LabelField( _parameter.Name, "(INTEGER) " +  _parameter.IntegerValue.ToString() );
								break;
							case AnimatorControllerParameterType.Float:
								EditorGUILayout.LabelField( _parameter.Name, "(FLOAT) " + ( _parameter.UseDynamicValue?_control.GetDynamicFloatValue( _parameter.FloatValueType ):_parameter.FloatValue ) );
								break;
							case AnimatorControllerParameterType.Trigger:
								EditorGUILayout.LabelField( _parameter.Name, "(TRIGGER)" );
								break;
							}
						}
					}
				}
			}
			else 
			{
				if( m_animator != null )
				{
					if( m_animator.enabled == false )
					{
						EditorGUILayout.HelpBox( "Sorry, your Animator Component is disabled!", MessageType.Warning ); 
						
						ICEEditorLayout.BeginHorizontal();
						EditorGUILayout.LabelField( "Enable Animator Component", EditorStyles.boldLabel);
						if (GUILayout.Button( new GUIContent("ENABLE", "Enable Animator Component"), ICEEditorStyle.ButtonMiddle ) )
							m_animator.enabled = true;
						ICEEditorLayout.EndHorizontal();
					}
					else if( m_animator.runtimeAnimatorController == null )
					{
						EditorGUILayout.HelpBox( "Sorry, there is no Runtime Animator Controller!", MessageType.Warning ); 
						
						ICEEditorLayout.BeginHorizontal();
						EditorGUILayout.LabelField( "Enable Animator Component", EditorStyles.boldLabel);
						if (GUILayout.Button( new GUIContent("ENABLE", "Enable Animator Component"), ICEEditorStyle.ButtonMiddle ) )
							m_animator.enabled = true;
						ICEEditorLayout.EndHorizontal();
					}
					else if( m_animator.avatar == null )
					{
						EditorGUILayout.HelpBox( "Sorry, there is no Avatar asigned to your Animator Component!", MessageType.Warning ); 
						
						ICEEditorLayout.BeginHorizontal();
						EditorGUILayout.LabelField( "Enable Animator Component", EditorStyles.boldLabel);
						if (GUILayout.Button( new GUIContent("ENABLE", "Enable Animator Component"), ICEEditorStyle.ButtonMiddle ) )
							m_animator.enabled = true;
						ICEEditorLayout.EndHorizontal();
					}
					
				}
				else
				{
					
					EditorGUILayout.HelpBox( "Sorry, there is no Animator Component!", MessageType.Warning ); 
					
					ICEEditorLayout.BeginHorizontal();
					EditorGUILayout.LabelField( "Add Animator Component", EditorStyles.boldLabel);
					if (GUILayout.Button( new GUIContent("ADD", "Add Animator Component"), ICEEditorStyle.ButtonMiddle ) )
						m_animator = _control.gameObject.AddComponent<Animator>();
					ICEEditorLayout.EndHorizontal();
				}
				
			}
			return _animator_data;
		}

		public static AnimationContainer DrawBehaviourAnimation( ICECreatureControl _control, AnimationContainer _anim )
		{

			//--------------------------------------------------
			// ANIMATION
			//--------------------------------------------------
			bool _animation = (_anim.InterfaceType != AnimationInterfaceType.NONE?true:false);
			
			ICEEditorLayout.BeginHorizontal();
			_animation = ICEEditorLayout.ToggleLeft("Animation","", _animation, true );
			GUILayout.FlexibleSpace();
			if( _animation )
				_anim.AllowInterfaceSelector = ICEEditorLayout.ButtonCheck( "SELECTOR", "", _anim.AllowInterfaceSelector, ICEEditorStyle.ButtonMiddle );
			ICEEditorLayout.EndHorizontal( Info.BEHAVIOUR_ANIMATION );
			
			EditorGUI.indentLevel++;
			
				if( _animation )
				{
					if( ( _control.GetComponentInChildren<Animator>() != null && _control.GetComponentInChildren<Animation>() != null ) || _anim.AllowInterfaceSelector )
					{
						if( _anim.InterfaceType == AnimationInterfaceType.NONE && _control.GetComponentInChildren<Animator>() != null && _control.GetComponentInChildren<Animator>().runtimeAnimatorController != null )
							_anim.InterfaceType = AnimationInterfaceType.MECANIM;
						else if( _anim.InterfaceType == AnimationInterfaceType.NONE )
							_anim.InterfaceType = AnimationInterfaceType.LEGACY;

						string _help = Info.BEHAVIOUR_ANIMATION_NONE;
						if( _anim.InterfaceType == AnimationInterfaceType.MECANIM )
							_help = Info.BEHAVIOUR_ANIMATION_ANIMATOR;
						else if( _anim.InterfaceType == AnimationInterfaceType.LEGACY )
							_help = Info.BEHAVIOUR_ANIMATION_ANIMATION;
						else if( _anim.InterfaceType == AnimationInterfaceType.CLIP )
							_help = Info.BEHAVIOUR_ANIMATION_CLIP;
						else if( _anim.InterfaceType == AnimationInterfaceType.CUSTOM )
							_help = Info.BEHAVIOUR_ANIMATION_CUSTOM;

						_anim.InterfaceType = (AnimationInterfaceType)ICEEditorLayout.EnumPopup( "Interface","", _anim.InterfaceType , _help );
					}
					else if( _control.GetComponentInChildren<Animator>() )
						_anim.InterfaceType = AnimationInterfaceType.MECANIM;
					else 
						_anim.InterfaceType = AnimationInterfaceType.LEGACY;
				}
				else
					_anim.InterfaceType = AnimationInterfaceType.NONE; 

							
				if( _anim.InterfaceType != AnimationInterfaceType.NONE )
				{
					if( _anim.InterfaceType == AnimationInterfaceType.MECANIM )
						_anim.Animator = DrawBehaviourAnimationAnimatorData( _control, _anim.Animator );
					else if( _anim.InterfaceType == AnimationInterfaceType.LEGACY )
						_anim.Animation = DrawBehaviourAnimationAnimationData( _control,_anim.Animation );
					else if( _anim.InterfaceType == AnimationInterfaceType.CLIP )
						_anim.Clip = DrawBehaviourAnimationAnimationClipData( _control, _anim.Clip );

					EditorGUILayout.Separator();
				}
				else
					Info.Help ( Info.BEHAVIOUR_ANIMATION_NONE );
			
			EditorGUI.indentLevel--;
			return _anim;
		}

		/// <summary>
		/// Get AnimationName by Index 
		/// </summary>
		/// <returns>The index to name.</returns>
		/// <param name="_control">_control.</param>
		/// <param name="index">Index.</param>
		private static string GetAnimationNameByIndex( ICECreatureControl _control, int _index)
		{
			AnimationState state = GetAnimationStateByIndex( _control, _index );
			if (state == null)
				return "";
			
			return state.name;
		}

		/// <summary>
		/// Get AnimationState by Index
		/// </summary>
		/// <returns>The state by index.</returns>
		/// <param name="_control">_control.</param>
		/// <param name="index">Index.</param>
		public static AnimationState GetAnimationStateByIndex( ICECreatureControl _control, int index)
		{
			Animation _anim = _control.GetComponentInChildren<Animation>();
			
			if( _anim == null )
				return null;
			
			int i = 0;
			foreach (AnimationState state in _anim )
			{
				if (i == index)
					return state;
				i++;
			}
			return null;
		}

	}
}