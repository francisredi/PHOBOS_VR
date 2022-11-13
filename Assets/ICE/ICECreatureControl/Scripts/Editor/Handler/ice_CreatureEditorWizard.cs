// ##############################################################################
//
// ice_CreatureEditorWizard.cs
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
using ICE.Utilities.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Styles;
using ICE.Layouts;
using ICE.Shared;
using ICE.Creatures.EditorInfos;



namespace ICE.Creatures.EditorHandler
{
	
	public static class EditorWizard
	{	
		public static void Print( ICECreatureControl _control )
		{
			//if( ! _control.Display.ShowWizard )
			//	return;

			EditorGUILayout.Separator();
			//ICEEditorLayout.Label( "Wizard", true );
			EditorGUI.indentLevel++;
				_control.Creature.Status.TrophicLevel = (CreatureTrophicLevelType)ICEEditorLayout.EnumPopup( "Trophic Level","", _control.Creature.Status.TrophicLevel, Info.STATUS_FEEDTYPE  ); 
				EditorGUILayout.Separator();

				ICEEditorLayout.Label( "Motion and Pathfinding", false );
				EditorGUI.indentLevel++;
		
					_control.Creature.Characteristics.MotionControl = (MotionControlType)ICEEditorLayout.EnumPopup("Motion Control","", _control.Creature.Characteristics.MotionControl, Info.MOTION_CONTROL );
					_control.Creature.Characteristics.GroundOrientation = (GroundOrientationType)ICEEditorLayout.EnumPopup("Ground Orientation", "Vertical direction relative to the ground", _control.Creature.Characteristics.GroundOrientation, Info.ESSENTIALS_SYSTEM_GROUND_ORIENTATION );
					
				EditorGUILayout.Separator();

				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();

				ICEEditorLayout.Label( "Desired Speed", false );
				EditorGUI.indentLevel++;
					_control.Creature.Characteristics.DefaultRunningSpeed = ICEEditorLayout.DefaultSlider( "Running","", _control.Creature.Characteristics.DefaultRunningSpeed, 0.25f ,0,25, 7, Info.CHARACTERISTICS_SPEED_RUNNING );
					_control.Creature.Characteristics.DefaultWalkingSpeed = ICEEditorLayout.DefaultSlider( "Walking","", _control.Creature.Characteristics.DefaultWalkingSpeed, 0.25f ,0,25, 3, Info.CHARACTERISTICS_SPEED_WALKING );
					_control.Creature.Characteristics.DefaultTurningSpeed = ICEEditorLayout.DefaultSlider( "Turning","", _control.Creature.Characteristics.DefaultTurningSpeed, 0.25f ,0,25, 4, Info.CHARACTERISTICS_SPEED_TURNING );
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();

				if( ICEEditorTools.HasAnimations( _control.gameObject ) )
				{
					ICEEditorLayout.Label( "Desired Animations", false );
					EditorGUI.indentLevel++;

						ICEEditorLayout.BeginHorizontal();
							EditorGUI.BeginDisabledGroup( _control.Creature.Characteristics.IgnoreAnimationIdle == true );
								_control.Creature.Characteristics.AnimationIdle = WizardAnimationPopup( "Idle", _control, _control.Creature.Characteristics.AnimationIdle );
							EditorGUI.EndDisabledGroup();
							_control.Creature.Characteristics.IgnoreAnimationIdle = ICEEditorLayout.ButtonCheck( "IGNORE", "",_control.Creature.Characteristics.IgnoreAnimationIdle, ICEEditorStyle.ButtonMiddle );
						ICEEditorLayout.EndHorizontal();

						ICEEditorLayout.BeginHorizontal();
							EditorGUI.BeginDisabledGroup( _control.Creature.Characteristics.IgnoreAnimationWalk == true );
								_control.Creature.Characteristics.AnimationWalk = WizardAnimationPopup( "Walk", _control, _control.Creature.Characteristics.AnimationWalk );
							EditorGUI.EndDisabledGroup();
							_control.Creature.Characteristics.IgnoreAnimationWalk = ICEEditorLayout.ButtonCheck( "IGNORE", "",_control.Creature.Characteristics.IgnoreAnimationWalk, ICEEditorStyle.ButtonMiddle );
						ICEEditorLayout.EndHorizontal();

						ICEEditorLayout.BeginHorizontal();
							EditorGUI.BeginDisabledGroup( _control.Creature.Characteristics.IgnoreAnimationRun == true );
								_control.Creature.Characteristics.AnimationRun = WizardAnimationPopup( "Run", _control, _control.Creature.Characteristics.AnimationRun );
							EditorGUI.EndDisabledGroup();
							_control.Creature.Characteristics.IgnoreAnimationRun = ICEEditorLayout.ButtonCheck( "IGNORE", "",_control.Creature.Characteristics.IgnoreAnimationRun, ICEEditorStyle.ButtonMiddle );
						ICEEditorLayout.EndHorizontal();

						ICEEditorLayout.BeginHorizontal();
							EditorGUI.BeginDisabledGroup( _control.Creature.Characteristics.IgnoreAnimationJump == true );
								_control.Creature.Characteristics.AnimationJump = WizardAnimationPopup( "Jump", _control, _control.Creature.Characteristics.AnimationJump );
							EditorGUI.EndDisabledGroup();
							_control.Creature.Characteristics.IgnoreAnimationJump = ICEEditorLayout.ButtonCheck( "IGNORE", "",_control.Creature.Characteristics.IgnoreAnimationJump, ICEEditorStyle.ButtonMiddle );
						ICEEditorLayout.EndHorizontal();

						ICEEditorLayout.BeginHorizontal();
							EditorGUI.BeginDisabledGroup( _control.Creature.Characteristics.IgnoreAnimationJump == true );
								_control.Creature.Characteristics.AnimationCrouchIdle = WizardAnimationPopup( "CROUCH IDLE", _control, _control.Creature.Characteristics.AnimationCrouchIdle );
							EditorGUI.EndDisabledGroup();
							_control.Creature.Characteristics.IgnoreAnimationCrouchIdle = ICEEditorLayout.ButtonCheck( "IGNORE", "",_control.Creature.Characteristics.IgnoreAnimationCrouchIdle, ICEEditorStyle.ButtonMiddle );
						ICEEditorLayout.EndHorizontal();
						
						ICEEditorLayout.BeginHorizontal();
							EditorGUI.BeginDisabledGroup( _control.Creature.Characteristics.IgnoreAnimationJump == true );
								_control.Creature.Characteristics.AnimationCrouchMove = WizardAnimationPopup( "CROUCH MOVE", _control, _control.Creature.Characteristics.AnimationCrouchMove );
							EditorGUI.EndDisabledGroup();
							_control.Creature.Characteristics.IgnoreAnimationCrouchMove = ICEEditorLayout.ButtonCheck( "IGNORE", "",_control.Creature.Characteristics.IgnoreAnimationCrouchMove, ICEEditorStyle.ButtonMiddle );
						ICEEditorLayout.EndHorizontal();

						ICEEditorLayout.BeginHorizontal();
							EditorGUI.BeginDisabledGroup( _control.Creature.Characteristics.IgnoreAnimationJump == true );
								_control.Creature.Characteristics.AnimationCrawlIdle = WizardAnimationPopup( "CRAWL IDLE", _control, _control.Creature.Characteristics.AnimationCrawlIdle );
							EditorGUI.EndDisabledGroup();
							_control.Creature.Characteristics.IgnoreAnimationCrawlIdle = ICEEditorLayout.ButtonCheck( "IGNORE", "",_control.Creature.Characteristics.IgnoreAnimationCrawlIdle, ICEEditorStyle.ButtonMiddle );
						ICEEditorLayout.EndHorizontal();
				
						ICEEditorLayout.BeginHorizontal();
							EditorGUI.BeginDisabledGroup( _control.Creature.Characteristics.IgnoreAnimationJump == true );
								_control.Creature.Characteristics.AnimationCrawlMove = WizardAnimationPopup( "CRAWL MOVE", _control, _control.Creature.Characteristics.AnimationCrawlMove );
							EditorGUI.EndDisabledGroup();
							_control.Creature.Characteristics.IgnoreAnimationCrawlMove = ICEEditorLayout.ButtonCheck( "IGNORE", "",_control.Creature.Characteristics.IgnoreAnimationCrawlMove, ICEEditorStyle.ButtonMiddle );
						ICEEditorLayout.EndHorizontal();
				



						ICEEditorLayout.BeginHorizontal();
							EditorGUI.BeginDisabledGroup( _control.Creature.Characteristics.IgnoreAnimationDead == true );
								_control.Creature.Characteristics.AnimationDead = WizardAnimationPopup( "DEAD", _control, _control.Creature.Characteristics.AnimationDead );
							EditorGUI.EndDisabledGroup();
							_control.Creature.Characteristics.IgnoreAnimationDead = ICEEditorLayout.ButtonCheck( "IGNORE", "",_control.Creature.Characteristics.IgnoreAnimationDead, ICEEditorStyle.ButtonMiddle );
						ICEEditorLayout.EndHorizontal();

					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}
				else
				{
					ICEEditorLayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						EditorGUILayout.LabelField( "- No Mecanim or Legacy animations available -" );
						GUILayout.FlexibleSpace();
					ICEEditorLayout.EndHorizontal();
					EditorGUILayout.Separator();
				}


				//HandleEssentialSettings( _control );
				//HandleSystemSettings( _control );

			EditorGUI.indentLevel--;

			EditorGUI.BeginDisabledGroup( Application.isPlaying == true );
				ICEEditorLayout.BeginHorizontal();
				
				if( GUILayout.Button( "GENERATE", ICEEditorStyle.ButtonExtraLarge ))
					WizardGenerate( _control );

				ICEEditorLayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
			
		}

		public static void WizardGenerate( ICECreatureControl _control )
		{
			string _tasks = "BEGIN GENERATE CREATURE SETTINGS \n";

				_tasks += "- Reset Creature Object \n";
				_control.Creature.Reset();

				_tasks += "- Create new Home Location and define the Random Positioning Range \n";
				WizardRandomTarget( _control, _control.Creature.Essentials.Target );

				if( ICECreatureRegister.Instance != null )
				{
					_tasks += "- Set GroundCheck to " + ICECreatureRegister.Instance.GroundCheck.ToString() + " and copy ground layer \n";
					_control.Creature.Move.GroundCheck = ICECreatureRegister.Instance.GroundCheck;
					_control.Creature.Move.SetGroundLayers( ICECreatureRegister.Instance.GroundLayers );

					_tasks += "- Set ObstacleCheck to " + ICECreatureRegister.Instance.ObstacleCheck.ToString() + " and copy obstacle layer \n";
					_control.Creature.Move.ObstacleCheck = ICECreatureRegister.Instance.ObstacleCheck;
					_control.Creature.Move.SetObstacleLayers( ICECreatureRegister.Instance.ObstacleLayers );
				}

				_tasks += "- Set GroundOrientation to " + _control.Creature.Characteristics.GroundOrientation.ToString() + " \n";
				_control.Creature.Move.DefaultBody.Type = _control.Creature.Characteristics.GroundOrientation;

				_tasks += "- Set TrophicLevel to " + _control.Creature.Characteristics.TrophicLevel.ToString() + " \n";
				_control.Creature.Status.TrophicLevel = _control.Creature.Characteristics.TrophicLevel;
				_control.Creature.Status.IsCannibal = _control.Creature.Characteristics.IsCannibal;

				_tasks += "- Create behaviours \n";
				WizardBehaviour( _control );

				if( _control.Creature.Characteristics.UseAutoDetectInteractors )
				{
					_tasks += "- Detect and prepare potential interactors \n";
					_control.Creature.AutoDetectInteractors();
				}

				_tasks += "END GENERATE CREATURE SETTINGS";

			Debug.Log( _tasks );

		}


		public static void WizardRandomTarget( ICECreatureControl _control, TargetObject _target )
		{
			string _new_target_name = ( _target.Type.ToString() + "_" + _control.transform.name ).ToUpper();
			
			GameObject _new_target_object = GameObject.Find( _new_target_name );
			
			if( _new_target_object == null )
			{
				_new_target_object = new GameObject();
				_new_target_object.transform.position = _control.transform.position; 
				_new_target_object.name = _new_target_name;
				_new_target_object.AddComponent<ICECreatureLocation>();

				if( ICECreatureRegister.Instance != null )
					_new_target_object.transform.parent = ICECreatureRegister.Instance.GetHierarchyGroup( HierarchyGroupType.Locations );
			}

			_target.OverrideTargetGameObject(  _new_target_object );

			_target.Move.RandomRange = 100;//Random.Range( Init.WIZARD_RANDOM_RANGE_MIN, Init.WIZARD_RANDOM_RANGE_MAX );
			_target.Move.UseUpdateOffsetOnActivateTarget = true;
			_target.Move.UseUpdateOffsetOnMovePositionReached = true;
			_target.Move.StopDistance = 2;
			_target.Move.IgnoreLevelDifference = true;

			if( ICECreatureRegister.Instance != null )
				ICECreatureRegister.Instance.AddReference( _target.TargetGameObject );
		}

		public static void WizardBehaviour( ICECreatureControl _control )
		{
			string _key = "";
	
			// ESSENTIAL ANIMATION BASED BEHAVIOURS 
			_key = _control.Creature.Behaviour.AddBehaviourMode( "RUN" );
			CreateBehaviourRuleRun( _control, _control.Creature.Behaviour.GetBehaviourModeByKey( _key ) );
			_control.Creature.Essentials.BehaviourModeRun = _key;

			_key = _control.Creature.Behaviour.AddBehaviourMode( "WALK" );
			CreateBehaviourRuleWalk( _control, _control.Creature.Behaviour.GetBehaviourModeByKey( _key ) );
			_control.Creature.Essentials.BehaviourModeWalk = _key;

			_key = _control.Creature.Behaviour.AddBehaviourMode( "IDLE" );
			CreateBehaviourRuleIdle( _control, _control.Creature.Behaviour.GetBehaviourModeByKey( _key ) );
			_control.Creature.Essentials.BehaviourModeIdle = _key;

			_key = _control.Creature.Behaviour.AddBehaviourMode( "DEAD" );
			CreateBehaviourRuleDie( _control, _control.Creature.Behaviour.GetBehaviourModeByKey( _key ) );
			_control.Creature.Essentials.BehaviourModeDead = _key;

			_key = _control.Creature.Behaviour.AddBehaviourMode( "SPAWN" );
			CreateBehaviourRuleIdle( _control, _control.Creature.Behaviour.GetBehaviourModeByKey( _key ) );
			_control.Creature.Essentials.BehaviourModeSpawn = _key;

			_key = _control.Creature.Behaviour.AddBehaviourMode( "JUMP" );
			CreateBehaviourRuleJump( _control, _control.Creature.Behaviour.GetBehaviourModeByKey( _key ) );
			_control.Creature.Essentials.BehaviourModeJump = _key;

			// ADDITIONAL ANIMATION BASED BEHAVIOURS 

			_key = _control.Creature.Behaviour.AddBehaviourMode( "ATTACK" );
			CreateBehaviourRuleAttack( _control, _control.Creature.Behaviour.GetBehaviourModeByKey( _key ) );

			_key = _control.Creature.Behaviour.AddBehaviourMode( "DEFEND" );
			CreateBehaviourRuleImpact( _control, _control.Creature.Behaviour.GetBehaviourModeByKey( _key ) );
	
			// ADDITIONAL BEHAVIOURS

			_key = _control.Creature.Behaviour.AddBehaviourMode( "SENSE" );
			CreateBehaviourRuleIdle( _control, _control.Creature.Behaviour.GetBehaviourModeByKey( _key ) );

			_key = _control.Creature.Behaviour.AddBehaviourMode( "HUNT" );
			CreateBehaviourRuleRun( _control, _control.Creature.Behaviour.GetBehaviourModeByKey( _key ) );

			_key = _control.Creature.Behaviour.AddBehaviourMode( "ESCAPE" );
			CreateBehaviourRuleRun( _control, _control.Creature.Behaviour.GetBehaviourModeByKey( _key ) );
	
		}

		/// <summary>
		/// Creates automatically a run behaviour rule.
		/// </summary>
		/// <param name="_control">Control.</param>
		/// <param name="_key">Key.</param>
		public static void CreateBehaviourRuleRun( ICECreatureControl _control, BehaviourModeObject _behaviour )
		{
			if( _behaviour == null )
				return;

			_behaviour.NextRule();
			if( _behaviour.Rule != null )
			{	
				_behaviour.Rule.Move.Enabled = true;
				_behaviour.Rule.Move.Velocity.AngularVelocity = _control.Creature.Characteristics.DefaultTurningSpeed;
				if( ! _control.Creature.Characteristics.IgnoreAnimationRun )
				{
					_behaviour.Rule.Move.Velocity.Velocity.z = _control.Creature.Characteristics.DefaultRunningSpeed;
					_behaviour.Rule.Animation = _control.Creature.Characteristics.AnimationRun;
				}
				else if( ! _control.Creature.Characteristics.IgnoreAnimationWalk )
				{
					_behaviour.Rule.Move.Velocity.Velocity.z = _control.Creature.Characteristics.DefaultWalkingSpeed;
					_behaviour.Rule.Animation = _control.Creature.Characteristics.AnimationWalk;
				}
				else
					_behaviour.Rule.Move.Velocity.Velocity.z = _control.Creature.Characteristics.DefaultRunningSpeed;	
			}
		}

		/// <summary>
		/// Creates automatically an walk behaviour rule.
		/// </summary>
		/// <param name="_control">Control.</param>
		/// <param name="_key">Key.</param>
		public static void CreateBehaviourRuleWalk( ICECreatureControl _control, BehaviourModeObject _behaviour )
		{
			if( _behaviour == null )
				return;

			_behaviour.NextRule();
			if( _behaviour.Rule != null )
			{			
				_behaviour.Rule.Move.Enabled = true;
				_behaviour.Rule.Move.Velocity.AngularVelocity = _control.Creature.Characteristics.DefaultTurningSpeed;
				if( ! _control.Creature.Characteristics.IgnoreAnimationWalk )
				{
					_behaviour.Rule.Move.Velocity.Velocity.z = _control.Creature.Characteristics.DefaultWalkingSpeed;
					_behaviour.Rule.Animation = _control.Creature.Characteristics.AnimationWalk;
				}
				else if( ! _control.Creature.Characteristics.IgnoreAnimationRun )
				{
					_behaviour.Rule.Move.Velocity.Velocity.z = _control.Creature.Characteristics.DefaultRunningSpeed;
					_behaviour.Rule.Animation = _control.Creature.Characteristics.AnimationRun;
				}
				else
					_behaviour.Rule.Move.Velocity.Velocity.z = _control.Creature.Characteristics.DefaultWalkingSpeed;
			}
		}

		/// <summary>
		/// Creates automatically an attack behaviour rule.
		/// </summary>
		/// <param name="_control">Control.</param>
		/// <param name="_key">Key.</param>
		public static void CreateBehaviourRuleAttack( ICECreatureControl _control, BehaviourModeObject _behaviour )
		{
			if( _behaviour == null )
				return;

			_behaviour.NextRule();
			if( _behaviour.Rule != null )
			{			
				_behaviour.Rule.Move.Enabled = false;
				_behaviour.Rule.Move.Velocity.AngularVelocity = _control.Creature.Characteristics.DefaultTurningSpeed;
				if( ! _control.Creature.Characteristics.IgnoreAnimationAttack )
				{
					_behaviour.Rule.Move.Velocity.Velocity = Vector3.zero;
					_behaviour.Rule.Animation = _control.Creature.Characteristics.AnimationAttack;
				}
			}
		}

		/// <summary>
		/// Creates automatically an impact behaviour rule.
		/// </summary>
		/// <param name="_control">Control.</param>
		/// <param name="_key">Key.</param>
		public static void CreateBehaviourRuleImpact( ICECreatureControl _control, BehaviourModeObject _behaviour )
		{
			if( _behaviour == null )
				return;

			_behaviour.NextRule();
			if( _behaviour.Rule != null )
			{			
				_behaviour.Rule.Move.Enabled = false;
				_behaviour.Rule.Move.Velocity.AngularVelocity = _control.Creature.Characteristics.DefaultTurningSpeed;
				if( ! _control.Creature.Characteristics.IgnoreAnimationImpact )
				{
					_behaviour.Rule.Move.Velocity.Velocity = Vector3.zero;
					_behaviour.Rule.Animation = _control.Creature.Characteristics.AnimationImpact;
				}
			}
		}

		/// <summary>
		/// Creates automatically an idle behaviour rule.
		/// </summary>
		/// <param name="_control">Control.</param>
		/// <param name="_key">Key.</param>
		public static void CreateBehaviourRuleIdle( ICECreatureControl _control, BehaviourModeObject _behaviour )
		{
			if( _behaviour == null )
				return;
			
			_behaviour.NextRule();
			if( _behaviour.Rule != null )
			{			
				_behaviour.Rule.Move.Enabled = false;
				_behaviour.Rule.Move.Velocity.AngularVelocity = _control.Creature.Characteristics.DefaultTurningSpeed;
				if( ! _control.Creature.Characteristics.IgnoreAnimationIdle )
				{
					_behaviour.Rule.Move.Velocity.Velocity = Vector3.zero;
					_behaviour.Rule.Animation = _control.Creature.Characteristics.AnimationIdle;
				}
			}
		}

		/// <summary>
		/// Creates automatically a jump behaviour rule.
		/// </summary>
		/// <param name="_control">Control.</param>
		/// <param name="_key">Key.</param>
		public static void CreateBehaviourRuleJump( ICECreatureControl _control, BehaviourModeObject _behaviour )
		{
			if( _behaviour == null )
				return;

			_behaviour.NextRule();
			if( _behaviour.Rule != null )
			{			
				_behaviour.Rule.Move.Enabled = true;
				_behaviour.Rule.Move.Velocity.AngularVelocity = _control.Creature.Characteristics.DefaultTurningSpeed;
				if( ! _control.Creature.Characteristics.IgnoreAnimationJump )
				{
					_behaviour.Rule.Move.Velocity.Velocity.z = _control.Creature.Characteristics.DefaultRunningSpeed;
					_behaviour.Rule.Move.Velocity.Velocity.y = _control.Creature.Characteristics.DefaultRunningSpeed;
					_behaviour.Rule.Animation = _control.Creature.Characteristics.AnimationIdle;
				}
			}
		}

		/// <summary>
		/// Creates automatically a die behaviour rule.
		/// </summary>
		/// <param name="_control">Control.</param>
		/// <param name="_key">Key.</param>
		public static void CreateBehaviourRuleDie( ICECreatureControl _control, BehaviourModeObject _behaviour )
		{
			if( _behaviour == null )
				return;

			_behaviour.NextRule();
			if( _behaviour.Rule != null )
			{			
				_behaviour.Rule.Move.Enabled = false;
				_behaviour.Rule.Move.Velocity.AngularVelocity = 0;
				if( ! _control.Creature.Characteristics.IgnoreAnimationDead )
				{
					_behaviour.Rule.Move.Velocity.Velocity = Vector3.zero;
					_behaviour.Rule.Animation = _control.Creature.Characteristics.AnimationDead;
				}
				else if( ! _control.Creature.Characteristics.IgnoreAnimationIdle )
				{
					_behaviour.Rule.Move.Velocity.Velocity = Vector3.zero;
					_behaviour.Rule.Animation = _control.Creature.Characteristics.AnimationIdle;
				}
			}
		}

		public static string WizardBehaviour( ICECreatureControl _control, string _key )
		{
			_key = _control.Creature.Behaviour.AddBehaviourMode( _key );

			BehaviourModeObject _behaviour = _control.Creature.Behaviour.GetBehaviourModeByKey( _key );


			if( ! WizardAnimation( _control, _behaviour ) )
				return _key;



			return _key;
		}

		public static bool WizardAnimation( ICECreatureControl _control, BehaviourModeObject _behaviour )
		{
			if( _behaviour == null ) 
				return false;

			_behaviour.NextRule();
			 if( _behaviour.Rule == null )
			   return false;

			if( _behaviour.Key == "RUN" || _behaviour.Key == "TRAVEL" || _behaviour.Key == "JOURNEY" || _behaviour.Key == "HUNT" || _behaviour.Key == "ESCAPE" || _behaviour.Key == "FLEE" )
			{
				_behaviour.Rule.Move.Enabled = true;
				_behaviour.Rule.Move.Velocity.Velocity.z  = _control.Creature.Characteristics.DefaultRunningSpeed;
				_behaviour.Rule.Move.Velocity.AngularVelocity = _control.Creature.Characteristics.DefaultTurningSpeed;
			}
			else if( _behaviour.Key == "WALK" || _behaviour.Key == "LEISURE" || _behaviour.Key == "AVOID" )
			{
				_behaviour.Rule.Move.Enabled = true;
				_behaviour.Rule.Move.Velocity.Velocity.z  = _control.Creature.Characteristics.DefaultWalkingSpeed;
				_behaviour.Rule.Move.Velocity.AngularVelocity = _control.Creature.Characteristics.DefaultTurningSpeed;
			}
			else 
			{
				_behaviour.Rule.Move.Enabled = false;
				_behaviour.Rule.Move.Velocity.Velocity.z  = 0;
				_behaviour.Rule.Move.Velocity.AngularVelocity = 0;
			}

			if( _control.GetComponent<Animator>() != null && _control.GetComponent<Animator>().runtimeAnimatorController != null )
			{
				AnimationClip[] _clips = _control.GetComponent<Animator>().runtimeAnimatorController.animationClips;
				int _index = 0;
				foreach( AnimationClip _clip in _clips )
				{
					if( AnimationIsSuitable( _behaviour.Key, _clip.name ) )
					{
						_behaviour.Rule.Animation.InterfaceType = AnimationInterfaceType.MECANIM;
						_behaviour.Rule.Animation.Animator.Type = AnimatorControlType.DIRECT;
						_behaviour.Rule.Animation.Animator.Name = _clip.name;
						_behaviour.Rule.Animation.Animator.Index = _index;
						break;
					}

					_index++;
				}
			}
			else if( _control.GetComponentInChildren<Animation>() != null )
			{
				Animation _animation = _control.GetComponentInChildren<Animation>();
				int _index = 0;
				foreach (AnimationState _state in _animation )
				{
					if( AnimationIsSuitable( _behaviour.Key, _state.name ) )
					{
						_behaviour.Rule.Animation.InterfaceType = AnimationInterfaceType.LEGACY;
						_behaviour.Rule.Animation.Animation.Name = _state.name;
						_behaviour.Rule.Animation.Animation.Index = _index;
					}
					
					_index++;
				}
			}

			return true;
		}

		public static AnimationContainer WizardAnimationPopup( string _title, ICECreatureControl _control, AnimationContainer _anim )
		{
			if( _control.GetComponentInChildren<Animator>() )
				_anim.InterfaceType = AnimationInterfaceType.MECANIM;
			else if( _control.GetComponentInChildren<Animation>() )
				_anim.InterfaceType = AnimationInterfaceType.LEGACY;
			else
				_anim.InterfaceType = AnimationInterfaceType.NONE; 
			
			
			if( _anim.InterfaceType != AnimationInterfaceType.NONE )
			{
				if( _anim.InterfaceType == AnimationInterfaceType.MECANIM )
					_anim.Animator = WizardAnimationPopupMecanim( _title, _control, _anim.Animator );
				else if( _anim.InterfaceType == AnimationInterfaceType.LEGACY )
					_anim.Animation = WizardAnimationPopupLegacy( _title, _control,_anim.Animation );
			}
			return _anim;
		}

		private static AnimationDataContainer WizardAnimationPopupLegacy( string _title, ICECreatureControl _control, AnimationDataContainer _animation_data )
		{
			Animation m_animation = _control.GetComponentInChildren<Animation>();
			
			if( m_animation != null && m_animation.enabled == true )
			{
				if( EditorApplication.isPlaying )
				{
					EditorGUILayout.LabelField("Name", _animation_data.Name );
				}
				else
				{

					_animation_data.Index = EditorBehaviour.AnimationPopupBase( m_animation, _animation_data.Index, _title );
					AnimationState _state = EditorBehaviour.GetAnimationStateByIndex( _control, _animation_data.Index );					
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
			}
			else
			{
				EditorGUILayout.HelpBox( "Check your Animation Component", MessageType.Warning ); 
			}
			
			return _animation_data;
		}

		private static AnimatorDataContainer WizardAnimationPopupMecanim( string _title, ICECreatureControl _control, AnimatorDataContainer _animator_data )
		{
			Animator m_animator = _control.GetComponent<Animator>();			
			if( m_animator != null && m_animator.enabled == true && m_animator.runtimeAnimatorController != null && m_animator.avatar != null )
			{
				if( ! EditorApplication.isPlaying )
				{
					_animator_data.Type = AnimatorControlType.DIRECT;
					
					ICEEditorLayout.BeginHorizontal();
					_animator_data.Index = EditorBehaviour.AnimatorPopupBase( m_animator, _animator_data.Index, _title );
					ICEEditorLayout.EndHorizontal();
					
					if( m_animator.runtimeAnimatorController.animationClips.Length == 0 )
						Info.Warning( Info.BEHAVIOUR_ANIMATION_ANIMATOR_ERROR_NO_CLIPS );
					else
					{
						AnimationClip _animation_clip = m_animator.runtimeAnimatorController.animationClips[_animator_data.Index];						
						if( _animation_clip != null )
						{				
							if( _animator_data.Name != _animation_clip.name )
								_animator_data.Init();
							
							_animation_clip.wrapMode = WrapMode.Loop;
							_animation_clip.legacy = false;
							
							_animator_data.Name = _animation_clip.name;
							_animator_data.Length = _animation_clip.length;
							_animator_data.DefaultWrapMode = _animation_clip.wrapMode;							
							_animator_data.Speed = 1;
							_animator_data.TransitionDuration = 0.5f;
							
						}
					}
				}
				else
				{
					ICEEditorLayout.Label( "Name", "Animation name.", _animator_data.Name );
				}
			}
			else 
			{
			
				
			}
			return _animator_data;
		}

		private static bool AnimationIsSuitable( string _key, string _animation )
		{
			_key = _key.ToUpper();
			_animation = _animation.ToUpper();

			if( _key == "RUN" && ( _animation.IndexOf( "RUN" ) > -1 || _animation.IndexOf( "WALK" ) > -1 ) )
				return true;
			else if( _key == "WALK" && _animation.IndexOf( "WALK" ) > -1 )
				return true;
			else if( _key == "IDLE" && _animation.IndexOf( "IDLE" ) > -1 )
				return true;
			else if( _key == "DEAD" && ( _animation.IndexOf( "DEAD" ) > -1 || _animation.IndexOf( "DEATH" ) > -1 ) )
				return true;
			else
				return false;
		}

		
		/// <summary>
		/// Handles the system settings.
		/// </summary>
		/// <param name="_creature_control">_creature_control.</param>
		private static void HandleSystemSettings( ICECreatureControl _control )
		{
			EditorGUILayout.Separator();
			
			
			ICEEditorLayout.Label( "Motion and Pathfinding", true, Info.ESSENTIALS_SYSTEM );
			EditorGUI.indentLevel++;
			
			
			if( _control.GetComponent<UnityEngine.AI.NavMeshAgent>() != null )
				_control.Creature.Move.MotionControl = MotionControlType.NAVMESHAGENT;
			
			string _motion_control_help = Info.MOTION_CONTROL;
			if( _control.Creature.Move.MotionControl == MotionControlType.NAVMESHAGENT )
				_motion_control_help += "\n\n" + Info.MOTION_CONTROL_NAVMESHAGENT;
			else if( _control.Creature.Move.MotionControl == MotionControlType.RIGIDBODY )
				_motion_control_help += "\n\n" + Info.MOTION_CONTROL_RIGIDBODY;
			else if( _control.Creature.Move.MotionControl == MotionControlType.CHARACTERCONTROLLER )
				_motion_control_help += "\n\n" + Info.MOTION_CONTROL_CHARACTER_CONTROLLER;
			
			_control.Creature.Move.MotionControl = (MotionControlType)ICEEditorLayout.EnumPopup("Motion Control","", _control.Creature.Move.MotionControl, _motion_control_help );
			EditorGUI.indentLevel++;
			if( _control.Creature.Move.MotionControl == MotionControlType.NAVMESHAGENT )
			{
				if( _control.GetComponent<UnityEngine.AI.NavMeshAgent>() == null )
				{
					ICEEditorLayout.BeginHorizontal();
					ICEEditorLayout.Label( "NavMeshAgent required", false );
					if (GUILayout.Button("ADD AGENT", ICEEditorStyle.ButtonLarge ))
						_control.transform.gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
					ICEEditorLayout.EndHorizontal( Info.MOTION_CONTROL_NAVMESHAGENT_MISSING );
				}
				EditorGUILayout.Separator();
			}
			else if( _control.Creature.Move.MotionControl == MotionControlType.RIGIDBODY )
			{
				Rigidbody _rigidbody = _control.GetComponent<Rigidbody>();
				
				if( _rigidbody == null )
				{
					ICEEditorLayout.BeginHorizontal();
					ICEEditorLayout.Label( "Rigidbody required", false );
					if (GUILayout.Button("ADD BODY", ICEEditorStyle.ButtonLarge ))
						_control.transform.gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
					ICEEditorLayout.EndHorizontal( Info.MOTION_CONTROL_RIGIDBODY_MISSING );
				}
				else
				{
					_rigidbody.useGravity = ICEEditorLayout.Toggle( "Gravity", "", _rigidbody.useGravity );
					_rigidbody.isKinematic = ICEEditorLayout.Toggle( "Kinematic", "", _rigidbody.isKinematic );
					_rigidbody.freezeRotation = true;
				}
				EditorGUILayout.Separator();
			}
			else if( _control.Creature.Move.MotionControl == MotionControlType.CHARACTERCONTROLLER )
			{
				if( _control.GetComponent<CharacterController>() == null )
				{
					ICEEditorLayout.BeginHorizontal();
					ICEEditorLayout.Label( "CharacterController required", false );
					if (GUILayout.Button("ADD CONTROLLER", ICEEditorStyle.ButtonLarge ))
						_control.transform.gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
					ICEEditorLayout.EndHorizontal( Info.MOTION_CONTROL_CHARACTER_CONTROLLER_MISSING );
				}
				EditorGUILayout.Separator();
			}
			else 
			{
				if( _control.GetComponent<Rigidbody>() != null )
				{
					_control.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
				}
				
				_control.Creature.Move.MotionControl = MotionControlType.INTERNAL;
			}
			EditorGUI.indentLevel--;
			
			
			// OBSTACLE AVOIDANCE BEGIN
			ICEEditorLayout.BeginHorizontal();
			_control.Creature.Move.ObstacleCheck = (ObstacleCheckType)ICEEditorLayout.EnumPopup("Obstacle Avoidance", "", _control.Creature.Move.ObstacleCheck );
			
			if( _control.Creature.Move.ObstacleCheck == ObstacleCheckType.BASIC )
			{
				if (GUILayout.Button("Add Layer", ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Move.ObstacleLayers.Add( "Default" );
				
			}				
			ICEEditorLayout.EndHorizontal( Info.ESSENTIALS_SYSTEM_GROUND_CHECK );
			if( _control.Creature.Move.ObstacleCheck == ObstacleCheckType.BASIC )
			{
				EditorGUI.indentLevel++;
				DrawObstacleLayers( _control.Creature.Move.ObstacleLayers );
				EditorGUI.indentLevel--;
				
				EditorGUI.indentLevel++;
				_control.Creature.Move.ObstacleAvoidanceScanningAngle = (int)ICEEditorLayout.DefaultSlider( "Step Size (degrees)", "", _control.Creature.Move.ObstacleAvoidanceScanningAngle, 2f, 0, 36,10 );
				_control.Creature.Move.ObstacleAvoidanceScanningRange = (int)ICEEditorLayout.DefaultSlider( "Range (units)", "", _control.Creature.Move.ObstacleAvoidanceScanningRange, 1, 0, 50,15 );
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();
				
			}					
			
			
			
			// OBSTACLE AVOIDANCE END
			
			
			ICEEditorLayout.BeginHorizontal();
			_control.Creature.Move.GroundCheck = (GroundCheckType)ICEEditorLayout.EnumPopup("Ground Check", "Method to handle ground related checks and movements", _control.Creature.Move.GroundCheck );
			if( _control.Creature.Move.GroundCheck == GroundCheckType.RAYCAST )
			{
				if (GUILayout.Button("Add Layer", ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Move.GroundLayers.Add( (LayerMask.NameToLayer("Terrain") != -1?"Terrain":"Default") );

				if (GUILayout.Button("AUTO", ICEEditorStyle.ButtonMiddle ))
				{
					//SystemTools.
					_control.Creature.Move.GroundLayers.Add( (LayerMask.NameToLayer("Terrain") != -1?"Terrain":"Default") );
				}
				
			}				
			ICEEditorLayout.EndHorizontal( Info.ESSENTIALS_SYSTEM_GROUND_CHECK );
			
			if( _control.Creature.Move.GroundCheck == GroundCheckType.RAYCAST )
			{
				EditorGUI.indentLevel++;
				DrawGroundLayers( _control.Creature.Move.GroundLayers );
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.Separator();
			
			if( _control.Creature.Move.DefaultBody.Type == GroundOrientationType.DEFAULT  || _control.Creature.Move.DefaultBody.Type == GroundOrientationType.BIPED )
			{
				_control.Creature.Move.DefaultBody.UseAdvanced = false;
			}
			
			ICEEditorLayout.BeginHorizontal();
			_control.Creature.Move.DefaultBody.Type = (GroundOrientationType)ICEEditorLayout.EnumPopup("Ground Orientation", "Vertical direction relative to the ground", _control.Creature.Move.DefaultBody.Type );
			EditorGUI.BeginDisabledGroup( _control.Creature.Move.DefaultBody.Type == GroundOrientationType.DEFAULT  || _control.Creature.Move.DefaultBody.Type == GroundOrientationType.BIPED );
			_control.Creature.Move.DefaultBody.UseAdvanced = ICEEditorLayout.ButtonCheck( "ADV", "", _control.Creature.Move.DefaultBody.UseAdvanced, ICEEditorStyle.CMDButtonDouble );
			EditorGUI.EndDisabledGroup();
			ICEEditorLayout.EndHorizontal( Info.ESSENTIALS_SYSTEM_GROUND_ORIENTATION );
			
			EditorGUI.BeginDisabledGroup( _control.Creature.Move.DefaultBody.Type == GroundOrientationType.DEFAULT );
			EditorGUI.indentLevel++;
			
			if( _control.Creature.Move.DefaultBody.UseAdvanced )
			{
				_control.Creature.Move.DefaultBody.Width = ICEEditorLayout.DefaultSlider( "Width", "", _control.Creature.Move.DefaultBody.Width, 0.01f, 0, 45, (_control.GetComponentInChildren<Renderer>() != null?( _control.GetComponentInChildren<Renderer>().bounds.size.x / _control.transform.lossyScale.x ):1) );
				EditorGUI.indentLevel++;
				_control.Creature.Move.DefaultBody.WidthOffset = ICEEditorLayout.DefaultSlider( "x-Offset", "", _control.Creature.Move.DefaultBody.WidthOffset, 0.01f, -10, 10, 0 );
				EditorGUI.indentLevel--;
				_control.Creature.Move.DefaultBody.Length = ICEEditorLayout.DefaultSlider( "Depth", "", _control.Creature.Move.DefaultBody.Length, 0.01f, 0, 45, (_control.GetComponentInChildren<Renderer>() != null?( _control.GetComponentInChildren<Renderer>().bounds.size.z / _control.transform.lossyScale.z ):1) );
				EditorGUI.indentLevel++;
				_control.Creature.Move.DefaultBody.DepthOffset = ICEEditorLayout.DefaultSlider( "z-Offset", "", _control.Creature.Move.DefaultBody.DepthOffset, 0.01f, -10, 10, 0 );
				EditorGUI.indentLevel--;
				
			}
			
			_control.Creature.Move.GroundCheckBaseOffset = ICEEditorLayout.DefaultSlider( "Base Offset", "", _control.Creature.Move.GroundCheckBaseOffset, 0.01f, -1, 1,0, Info.ESSENTIALS_SYSTEM_BASE_OFFSET ); 
			_control.Creature.Move.AvoidWater = ICEEditorLayout.Toggle("Avoid Water", "", _control.Creature.Move.AvoidWater,  Info.ESSENTIALS_AVOID_WATER );
			_control.Creature.Move.UseSlopeLimits = ICEEditorLayout.Toggle("Use Slope Limits", "", _control.Creature.Move.UseSlopeLimits,  Info.ESSENTIALS_SLOPE_LIMITS );
			EditorGUI.indentLevel++;
			EditorGUI.BeginDisabledGroup( _control.Creature.Move.UseSlopeLimits == false );
			_control.Creature.Move.MaxSlopeAngle = ICEEditorLayout.DefaultSlider("Max. Slope Angle", "Maximum slope angle for walkable surfaces", _control.Creature.Move.MaxSlopeAngle, 1, 0, 90, 45, Info.ESSENTIALS_MAX_SLOPE_ANGLE );
			_control.Creature.Move.MaxWalkableSlopeAngle = ICEEditorLayout.DefaultSlider("Max. Walkable Slope Angle", "Maximum slope angle for walkable surfaces", _control.Creature.Move.MaxWalkableSlopeAngle, 1, 0, 45, 35, Info.ESSENTIALS_MAX_WALKABLE_SLOPE_ANGLE );
			EditorGUI.EndDisabledGroup();
			EditorGUI.indentLevel--;
			
			_control.Creature.Move.DefaultBody.UseLeaningTurn = ICEEditorLayout.Toggle("Use Leaning Turn", "Allows to lean into a turn", _control.Creature.Move.DefaultBody.UseLeaningTurn ,  Info.ESSENTIALS_SYSTEM_LEAN_ANGLE );
			
			if( _control.Creature.Move.DefaultBody.UseLeaningTurn )
				Info.Warning( Info.ESSENTIALS_SYSTEM_LEAN_ANGLE_WARNING );
			
			EditorGUI.indentLevel++;
			EditorGUI.BeginDisabledGroup( _control.Creature.Move.DefaultBody.UseLeaningTurn == false );
			_control.Creature.Move.DefaultBody.LeanAngleMultiplier = ICEEditorLayout.DefaultSlider( "Lean Angle Multiplier", "Lean angle multiplier", _control.Creature.Move.DefaultBody.LeanAngleMultiplier, 0.05f, 0, 1, 0.5f );
			_control.Creature.Move.DefaultBody.MaxLeanAngle = ICEEditorLayout.DefaultSlider( "Max. Lean Angle", "Maximum lean angle", _control.Creature.Move.DefaultBody.MaxLeanAngle, 0.25f, 0, 45, 35 );
			EditorGUI.EndDisabledGroup();
			EditorGUI.indentLevel--;
			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.Separator();
			
			bool _allow_internal_gravity = true;
			if( _control.Creature.Move.MotionControl == MotionControlType.RIGIDBODY && _control.GetComponent<Rigidbody>() != null && _control.GetComponent<Rigidbody>().useGravity )
			{
				_allow_internal_gravity = false;
				_control.Creature.Move.UseInternalGravity = false;
			}
			
			EditorGUI.BeginDisabledGroup( _allow_internal_gravity == false );
			_control.Creature.Move.UseInternalGravity = ICEEditorLayout.Toggle("Handle Gravity", "Use internal gravity", _control.Creature.Move.UseInternalGravity,  Info.ESSENTIALS_SYSTEM_GRAVITY );
			EditorGUI.indentLevel++;
			EditorGUI.BeginDisabledGroup( _control.Creature.Move.UseInternalGravity == false );
			_control.Creature.Move.Gravity = ICEEditorLayout.AutoSlider( "Gravity", "Gravity value (default 9.8)", _control.Creature.Move.Gravity, 0.01f, 0, 100, ref _control.Creature.Move.UseWorldGravity, 9.81f );
			EditorGUI.EndDisabledGroup();
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
			EditorGUI.EndDisabledGroup();
			
			
			EditorGUILayout.Separator();
			_control.Creature.Move.UseDeadlockHandling = ICEEditorLayout.Toggle("Handle Deadlocks", "Use deadlock handling", _control.Creature.Move.UseDeadlockHandling, Info.DEADLOCK );
			EditorGUI.indentLevel++;
			EditorGUI.BeginDisabledGroup( _control.Creature.Move.UseDeadlockHandling == false );
			
			_control.Creature.Move.DeadlockMinMoveDistance = ICEEditorLayout.DefaultSlider( "Test 1 - Move Distance", "Expected distance the creature should have covered until the defined interval", _control.Creature.Move.DeadlockMinMoveDistance, 0.01f, 0, 5, 0.2f, Info.DEADLOCK_MOVE_DISTANCE );
			EditorGUI.indentLevel++;
			_control.Creature.Move.DeadlockMoveInterval = ICEEditorLayout.DefaultSlider( "Test Interval (sec.)", "Interval until the next test", _control.Creature.Move.DeadlockMoveInterval, 0.25f, 0, 30, 2, Info.DEADLOCK_MOVE_INTERVAL );
			_control.Creature.Move.DeadlockMoveMaxCriticalPositions = (int)ICEEditorLayout.DefaultSlider( "Max. Critical Positions", "Tolerates the defined number of critical positions before deadlocked will flagged as true.", _control.Creature.Move.DeadlockMoveMaxCriticalPositions, 1, 0, 100, 10, Info.DEADLOCK_MOVE_CRITICAL_POSITION );
			EditorGUI.indentLevel--;
			
			_control.Creature.Move.DeadlockLoopRange = ICEEditorLayout.DefaultSlider( "Test 2 - Loop Range", "Expected distance the creature should have covered until the defined interval", _control.Creature.Move.DeadlockLoopRange, 0.01f, 0, 25, _control.Creature.Move.MoveStopDistance, Info.DEADLOCK_LOOP_RANGE );
			EditorGUI.indentLevel++;
			_control.Creature.Move.DeadlockLoopInterval = ICEEditorLayout.DefaultSlider( "Test Interval (sec.)", "Interval until the next test", _control.Creature.Move.DeadlockLoopInterval, 0.25f, 0, 30, 5, Info.DEADLOCK_LOOP_INTERVAL );
			_control.Creature.Move.DeadlockLoopMaxCriticalPositions = (int)ICEEditorLayout.DefaultSlider( "Max. Critical Positions", "Tolerates the defined number of critical positions before deadlocked will flagged as true.", _control.Creature.Move.DeadlockLoopMaxCriticalPositions, 1, 0, 100, 10, Info.DEADLOCK_LOOP_CRITICAL_POSITION );
			EditorGUI.indentLevel--;
			
			
			_control.Creature.Move.DeadlockAction = (DeadlockActionType)ICEEditorLayout.EnumPopup( "Deadlock Action", "", _control.Creature.Move.DeadlockAction, Info.DEADLOCK_ACTION );
			
			if( _control.Creature.Move.DeadlockAction == DeadlockActionType.BEHAVIOUR )
			{
				EditorGUI.indentLevel++;
				_control.Creature.Move.DeadlockBehaviour = EditorBehaviour.BehaviourSelect( _control, "Deadlock Behaviour","", _control.Creature.Move.DeadlockBehaviour, "DEADLOCK", Info.DEADLOCK_ACTION_BEHAVIOUR );
				EditorGUI.indentLevel--;
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
			
			EditorGUILayout.Separator();
			_control.Creature.Move.FieldOfView = ICEEditorLayout.DefaultSlider( "Field Of View", "Field Of View", _control.Creature.Move.FieldOfView * 2, 0.05f, 0, 360, 60, Info.FOV ) / 2;
			EditorGUI.BeginDisabledGroup( _control.Creature.Move.FieldOfView == 0 );
			EditorGUI.indentLevel++;
			_control.Creature.Move.VisualRange = ICEEditorLayout.DefaultSlider( "Visual Range", "Max. Sighting Distance", _control.Creature.Move.VisualRange, 0.25f, 0, 500, 100, Info.FOV_VISUAL_RANGE );
			EditorGUI.indentLevel--;		
			EditorGUI.EndDisabledGroup();
			
			
			
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField( "Default Move" );
			EditorGUI.indentLevel++;
			EditorSharedTools.DrawMove( 
				ref _control.Creature.Move.DefaultMove.MoveSegmentLength, 
				ref _control.Creature.Move.DefaultMove.MaxMoveSegmentLength, 
				ref _control.Creature.Move.DefaultMove.MoveStopDistance, 
				ref _control.Creature.Move.DefaultMove.MoveSegmentVariance, 
				ref _control.Creature.Move.DefaultMove.MoveDeviationLength,
				ref _control.Creature.Move.DefaultMove.MoveDeviationLengthMax,
				ref _control.Creature.Move.DefaultMove.MoveDeviationVariance, 
				ref _control.Creature.Move.DefaultMove.MoveIgnoreLevelDifference,
				Info.MOVE_DEFAULT );
			EditorGUI.indentLevel--;
			EditorGUI.indentLevel--;
			
			
			
			
			
			
			/*
			 *			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Additional Components", true, Info.EXTERNAL_COMPONENTS );
			EditorGUI.indentLevel++;
				if( _control.GetComponent<NavMeshAgent>() == null )
					_control.Creature.Move.MotionControl == MotionControlType.INTERNAL;			
				if( _control.Creature.Move.UseNavMesh && _control.GetComponent<NavMeshAgent>() == null )
					_control.gameObject.AddComponent<NavMeshAgent>();
						
				ICEEditorLayout.BeginHorizontal();
					if( _control.GetComponent<Rigidbody>() == null )
						_control.Creature.Move.UseRigidbody = false;				
						_control.Creature.Move.UseRigidbody = ICEEditorLayout.Toggle("Use Rigidbody","", _control.Creature.Move.UseRigidbody );		
					if( _control.Creature.Move.UseRigidbody && _control.GetComponent<Rigidbody>() == null )
						_control.gameObject.AddComponent<Rigidbody>();

					GUILayout.FlexibleSpace();

					if( _control.GetComponent<Rigidbody>() != null )
					{
						if (GUILayout.Button("FULL", ICEEditorStyle.ButtonMiddle ))
						{
							Rigidbody _rigidbody = _control.GetComponent<Rigidbody>();
							
							_rigidbody.useGravity = true;
							_rigidbody.isKinematic = false;					
							_rigidbody.angularDrag = 0;
							_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
						}

						if (GUILayout.Button("SEMI", ICEEditorStyle.ButtonMiddle ))
						{
							Rigidbody _rigidbody = _control.GetComponent<Rigidbody>();
							
							_rigidbody.useGravity = false;
							_rigidbody.isKinematic = true;					
							_rigidbody.angularDrag = 0.05f;
							_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
						}

						if (GUILayout.Button("OFF", ICEEditorStyle.ButtonMiddle ))
						{
							Rigidbody _rigidbody = _control.GetComponent<Rigidbody>();

							_rigidbody.useGravity = false;
							_rigidbody.isKinematic = true;					
							_rigidbody.angularDrag = 0;
							_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
						}
					}

				ICEEditorLayout.EndHorizontal( Info.EXTERNAL_COMPONENTS_RIGIDBODY );


				if( _control.GetComponent<CharacterController>() == null )
					_control.Creature.Move.UseCharacterController = false;
				_control.Creature.Move.UseCharacterController = EditorGUILayout.Toggle("Use Character Controller", _control.Creature.Move.UseCharacterController );
				if( _control.Creature.Move.UseCharacterController && _control.GetComponent<CharacterController>() == null )
					_control.gameObject.AddComponent<CharacterController>();
*/
			
			EditorGUI.indentLevel--;
			
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField( "Runtime Behaviour", ICEEditorStyle.LabelBold );
			EditorGUI.indentLevel++;
			_control.Creature.UseCoroutine = ICEEditorLayout.Toggle("Use Coroutine","", _control.Creature.UseCoroutine, Info.RUNTIME_COROUTINE );
			
			_control.Creature.DontDestroyOnLoad = ICEEditorLayout.Toggle("Dont Destroy On Load","", _control.Creature.DontDestroyOnLoad, Info.RUNTIME_DONTDESTROYONLOAD );
			
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
		}
		
		/// <summary>
		/// Handles the essential settings.
		/// </summary>
		/// <param name="_creature_control">_creature_control.</param>
		private static void HandleEssentialSettings( ICECreatureControl _control )
		{
			EditorSharedTools.DrawTarget( _control, _control.Creature.Essentials.Target, "Home", Info.ESSENTIALS_HOME );	

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Behaviours", true, Info.ESSENTIALS_BEHAVIOURS );
			EditorGUI.indentLevel++;
			
				_control.Creature.Essentials.BehaviourModeRun = EditorBehaviour.BehaviourSelect( _control, "Travel", "Move behaviour if your creature is on a journey", _control.Creature.Essentials.BehaviourModeRun, "TRAVEL" );
				_control.Creature.Essentials.BehaviourModeIdle = EditorBehaviour.BehaviourSelect( _control, "Rendezvous", "Idle behaviour after reaching the current target move position.", _control.Creature.Essentials.BehaviourModeIdle, "RENDEZVOUS" );
				
				EditorGUI.BeginDisabledGroup( _control.Creature.Essentials.Target.Move.RandomRange == 0 );
				_control.Creature.Essentials.BehaviourModeWalk = EditorBehaviour.BehaviourSelect( _control, "Leisure", "Randomized leisure activities around the home area", _control.Creature.Essentials.BehaviourModeWalk, "LEISURE" );
				EditorGUI.EndDisabledGroup();	
				
				_control.Creature.Essentials.BehaviourModeDead = EditorBehaviour.BehaviourSelect( _control, "Dead", "Static behaviour if your creature is dead", _control.Creature.Essentials.BehaviourModeDead, "DEAD" );
				_control.Creature.Essentials.BehaviourModeSpawn = EditorBehaviour.BehaviourSelect( _control, "Spawn", "Idle behaviour during the spawning process", _control.Creature.Essentials.BehaviourModeSpawn, "SPAWN" );
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
			
		}

		/// <summary>
		/// Draws the ground layers.
		/// </summary>
		/// <param name="_layers">_layers.</param>
		public static void DrawGroundLayers( List<string> _layers )
		{
			for( int i = 0 ; i < _layers.Count; i++ )
			{
				ICEEditorLayout.BeginHorizontal();
				_layers[i] = LayerMask.LayerToName( EditorGUILayout.LayerField( "Ground Layer", LayerMask.NameToLayer(_layers[i]) ));
				if (GUILayout.Button("X", ICEEditorStyle.CMDButton ))
				{
					_layers.RemoveAt(i);
					--i;
				}
				ICEEditorLayout.EndHorizontal();
			}
			
		}
		
		/// <summary>
		/// Draws the obstacle layers.
		/// </summary>
		/// <param name="_layers">_layers.</param>
		private static void DrawObstacleLayers( List<string> _layers )
		{
			for( int i = 0 ; i < _layers.Count; i++ )
			{
				ICEEditorLayout.BeginHorizontal();
				_layers[i] = LayerMask.LayerToName( EditorGUILayout.LayerField( "Obstacle Layer", LayerMask.NameToLayer(_layers[i]) ));
				if (GUILayout.Button("X", ICEEditorStyle.CMDButton ))
				{
					_layers.RemoveAt(i);
					--i;
				}
				ICEEditorLayout.EndHorizontal();
			}
			
		}
	}
}
