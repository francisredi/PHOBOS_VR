// ##############################################################################
//
// ice_CreatureEditorEssentials.cs
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
using ICE.Creatures.EditorInfos;



namespace ICE.Creatures.EditorHandler
{
	
	public static class EditorEssentials
	{	
		public static void Print( ICECreatureControl _creature_control )
		{
			if( ! _creature_control.Display.ShowEssentials )
				return;

			ICEEditorStyle.SplitterByIndent( 0 );
			_creature_control.Display.FoldoutEssentials = ICEEditorLayout.Foldout( _creature_control.Display.FoldoutEssentials , "Essentials", Info.ESSENTIALS );
			
			if( ! _creature_control.Display.FoldoutEssentials ) 
				return;

			HandleEssentialSettings( _creature_control );
			HandleSystemSettings( _creature_control );

			
		
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


				if( _control.GetComponent<UnityEngine.AI.NavMeshAgent>() != null && _control.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled )
					_control.Creature.Move.MotionControl = MotionControlType.NAVMESHAGENT;

				string _motion_control_help = Info.MOTION_CONTROL;
				if( _control.Creature.Move.MotionControl == MotionControlType.NAVMESHAGENT )
					_motion_control_help += "\n\n" + Info.MOTION_CONTROL_NAVMESHAGENT;
				else if( _control.Creature.Move.MotionControl == MotionControlType.RIGIDBODY )
					_motion_control_help += "\n\n" + Info.MOTION_CONTROL_RIGIDBODY;
				else if( _control.Creature.Move.MotionControl == MotionControlType.CHARACTERCONTROLLER )
					_motion_control_help += "\n\n" + Info.MOTION_CONTROL_CHARACTER_CONTROLLER;
				else if( _control.Creature.Move.MotionControl == MotionControlType.CUSTOM )
					_motion_control_help += "\n\n" + Info.MOTION_CONTROL_CUSTOM;
			
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

						if( _control.GetComponent<UnityEngine.AI.NavMeshAgent>() != null )
							_control.Creature.Move.ObstacleCheck = ObstacleCheckType.NONE;
				
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
								_control.transform.gameObject.AddComponent<Rigidbody>();
							ICEEditorLayout.EndHorizontal( Info.MOTION_CONTROL_RIGIDBODY_MISSING );
						}
						else
						{
							_rigidbody.useGravity = ICEEditorLayout.Toggle( "Gravity", "", _rigidbody.useGravity );
							_rigidbody.isKinematic = ICEEditorLayout.Toggle( "Kinematic", "", _rigidbody.isKinematic );
							_rigidbody.constraints = RigidbodyConstraints.None;
							
							if( _rigidbody.useGravity == false )
								_rigidbody.constraints = RigidbodyConstraints.FreezePositionY;

							_rigidbody.freezeRotation = true;
						}
						EditorGUILayout.Separator();
					}
					else if( _control.Creature.Move.MotionControl == MotionControlType.CHARACTERCONTROLLER )
					{
						CharacterController _character_controller = _control.GetComponent<CharacterController>();

						if( _character_controller == null )
						{
							ICEEditorLayout.BeginHorizontal();
							ICEEditorLayout.Label( "CharacterController required", false );
							if (GUILayout.Button("ADD CONTROLLER", ICEEditorStyle.ButtonLarge ))
								_control.transform.gameObject.AddComponent<CharacterController>();
							ICEEditorLayout.EndHorizontal( Info.MOTION_CONTROL_CHARACTER_CONTROLLER_MISSING );
							EditorGUILayout.Separator();
						}
						else
						{
							//_character_controller.detectCollisions = ICEEditorLayout.Toggle( "Detect Collisions", "", _character_controller.detectCollisions );
						}
						
					}
					else if( _control.Creature.Move.MotionControl == MotionControlType.CUSTOM )
					{
				/*
						ICEEditorLayout.BeginHorizontal();
							GUILayout.FlexibleSpace();						
							EditorGUILayout.LabelField( new GUIContent( "external pathfinding active", "" ), EditorStyles.wordWrappedMiniLabel );
							GUILayout.FlexibleSpace();
						ICEEditorLayout.EndHorizontal();
						*/
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

				// GROUND CHECK BEGIN
				EditorSharedTools.DrawGroundCheck( ref _control.Creature.Move.GroundCheck, _control.Creature.Move.GroundLayers, Info.ESSENTIALS_SYSTEM_GROUND_CHECK );


				if( _control.Creature.Move.GroundCheck != GroundCheckType.NONE )
				{
					EditorGUI.indentLevel++;
						_control.Creature.Move.GroundCheckBaseOffset = ICEEditorLayout.DefaultSlider( "Base Offset", "", _control.Creature.Move.GroundCheckBaseOffset, 0.01f, -1, 1,0, Info.ESSENTIALS_SYSTEM_BASE_OFFSET ); 
						if( _control.Creature.Move.GroundCheck == GroundCheckType.RAYCAST )
						{
							_control.Creature.Move.VerticalRaycastOffset = ICEEditorLayout.MaxDefaultSlider( "Vertical Raycast Offset", "", _control.Creature.Move.VerticalRaycastOffset, 0.25f, 0, ref _control.Creature.Move.DownRaycastOffsetMax,0.5f, Info.ESSENTIALS_SYSTEM_RAYCAST_VERTICAL_OFFSET ); 

							_control.Creature.Move.AvoidWater = ICEEditorLayout.Toggle("Avoid Water", "", _control.Creature.Move.AvoidWater,  Info.ESSENTIALS_AVOID_WATER );

							_control.Creature.Move.UseSlopeLimits = ICEEditorLayout.Toggle("Use Slope Limits", "", _control.Creature.Move.UseSlopeLimits,  Info.ESSENTIALS_SLOPE_LIMITS );
							if( _control.Creature.Move.UseSlopeLimits )
							{
								EditorGUI.indentLevel++;
									_control.Creature.Move.MaxSlopeAngle = ICEEditorLayout.DefaultSlider("Max. Slope Angle", "Maximum slope angle for walkable surfaces", _control.Creature.Move.MaxSlopeAngle, 1, 0, 90, 45, Info.ESSENTIALS_MAX_SLOPE_ANGLE );
									_control.Creature.Move.MaxWalkableSlopeAngle = ICEEditorLayout.DefaultSlider("Max. Walkable Slope Angle", "Maximum slope angle for walkable surfaces", _control.Creature.Move.MaxWalkableSlopeAngle, 1, 0, 45, 35, Info.ESSENTIALS_MAX_WALKABLE_SLOPE_ANGLE );
								EditorGUI.indentLevel--;		
							}	

		
						}

					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}
				// GROUND CHECK END

				// BODY ORIENTATION BEGIN
				_control.Creature.Move.DefaultBody = EditorSharedTools.DrawBody( _control, _control.Creature.Move.DefaultBody );
				// BODY ORIENTATION END



			// OBSTACLE AVOIDANCE BEGIN
			EditorGUI.BeginDisabledGroup( _control.Creature.Move.MotionControl == MotionControlType.NAVMESHAGENT );
				ICEEditorLayout.BeginHorizontal();
					_control.Creature.Move.ObstacleCheck = (ObstacleCheckType)ICEEditorLayout.EnumPopup("Obstacle Avoidance", "", _control.Creature.Move.ObstacleCheck );
					if( _control.Creature.Move.ObstacleCheck == ObstacleCheckType.BASIC )
					{
						if (GUILayout.Button("Add Layer", ICEEditorStyle.ButtonMiddle ))
							_control.Creature.Move.ObstacleLayers.Add( (LayerMask.NameToLayer("Obstacle") != -1?"Obstacle":"Default") );						
					}				
				ICEEditorLayout.EndHorizontal( Info.ESSENTIALS_SYSTEM_AVOIDANCE );
				if( _control.Creature.Move.ObstacleCheck == ObstacleCheckType.BASIC )
				{
					EditorGUI.indentLevel++;
						EditorSharedTools.DrawLayers( _control.Creature.Move.ObstacleLayers );
					EditorGUI.indentLevel--;

					EditorGUI.indentLevel++;
						_control.Creature.Move.ObstacleAvoidanceScanningRange = (int)ICEEditorLayout.MaxDefaultSlider( "Scanning Range (units)", "", _control.Creature.Move.ObstacleAvoidanceScanningRange, 1, 0, ref _control.Creature.Move.ObstacleAvoidanceCheckDistanceMax,15, Info.ESSENTIALS_SYSTEM_AVOIDANCE_RANGE );
						_control.Creature.Move.ObstacleAvoidanceScanningAngle = (int)ICEEditorLayout.DefaultSlider( "Scanning Angle (degrees)", "", _control.Creature.Move.ObstacleAvoidanceScanningAngle, 2f, 0, 36,10, Info.ESSENTIALS_SYSTEM_AVOIDANCE_ANGLE );
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}			
			EditorGUI.EndDisabledGroup();
			// OBSTACLE AVOIDANCE END


				// GRAVITY BEGIN
				bool _allow_internal_gravity = true;
				if( _control.Creature.Move.MotionControl == MotionControlType.RIGIDBODY && _control.GetComponent<Rigidbody>() != null && _control.GetComponent<Rigidbody>().useGravity )
				{
					_allow_internal_gravity = false;
					_control.Creature.Move.UseInternalGravity = false;
				}

				EditorGUI.BeginDisabledGroup( _allow_internal_gravity == false );
					_control.Creature.Move.UseInternalGravity = ICEEditorLayout.Toggle("Handle Gravity", "Use internal gravity", _control.Creature.Move.UseInternalGravity,  Info.ESSENTIALS_SYSTEM_GRAVITY );
					if( _control.Creature.Move.UseInternalGravity )
					{
						EditorGUI.indentLevel++;
							_control.Creature.Move.Gravity = ICEEditorLayout.AutoSlider( "Gravity", "Gravity value (default 9.8)", _control.Creature.Move.Gravity, 0.01f, 0, 100, ref _control.Creature.Move.UseWorldGravity, 9.81f );
							EditorGUILayout.Separator();
						EditorGUI.indentLevel--;
					}
				EditorGUI.EndDisabledGroup();
				// GRAVITY END

				// DEADLOCK BEGIN
				_control.Creature.Move.UseDeadlockHandling = ICEEditorLayout.Toggle("Handle Deadlocks", "Use deadlock handling", _control.Creature.Move.UseDeadlockHandling, Info.DEADLOCK );
				if( _control.Creature.Move.UseDeadlockHandling )
				{
					EditorGUI.indentLevel++;
						_control.Creature.Move.DeadlockMinMoveDistance = ICEEditorLayout.DefaultSlider( "Move Distance Check", "Expected distance the creature should have covered until the defined interval", _control.Creature.Move.DeadlockMinMoveDistance, 0.01f, 0, 5, 0.2f, Info.DEADLOCK_MOVE_DISTANCE );
						EditorGUI.indentLevel++;
							_control.Creature.Move.DeadlockMoveInterval = ICEEditorLayout.DefaultSlider( "Test Interval (sec.)", "Interval until the next test", _control.Creature.Move.DeadlockMoveInterval, 0.25f, 0, 30, 2, Info.DEADLOCK_MOVE_INTERVAL );
							_control.Creature.Move.DeadlockMoveMaxCriticalPositions = (int)ICEEditorLayout.DefaultSlider( "Max. Critical Positions", "Tolerates the defined number of critical positions before deadlocked will flagged as true.", _control.Creature.Move.DeadlockMoveMaxCriticalPositions, 1, 0, 100, 10, Info.DEADLOCK_MOVE_CRITICAL_POSITION );
						EditorGUI.indentLevel--;

						_control.Creature.Move.DeadlockLoopRange = ICEEditorLayout.DefaultSlider( "Loop Range Check", "Expected distance the creature should have covered until the defined interval", _control.Creature.Move.DeadlockLoopRange, 0.01f, 0, 25, _control.Creature.Move.MoveStopDistance, Info.DEADLOCK_LOOP_RANGE );
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
						EditorGUILayout.Separator();
					EditorGUI.indentLevel--;
				}
				// DEADLOCK END

				// FOV BEGIN
				_control.Creature.Move.FieldOfView = ICEEditorLayout.DefaultSlider( "Field Of View", "Field Of View", _control.Creature.Move.FieldOfView * 2, 0.05f, 0, 360, 160, Info.FOV ) / 2;
				if( _control.Creature.Move.FieldOfView > 0 )
				{
					EditorGUI.indentLevel++;
						_control.Creature.Move.VisualRange = ICEEditorLayout.MaxDefaultSlider( "Visual Range", "Max. Sighting Distance", _control.Creature.Move.VisualRange, 0.25f, 0, ref _control.Creature.Move.MaxVisualRange, 100, Info.FOV_VISUAL_RANGE );
					EditorGUI.indentLevel--;		
				}
				// FOV END


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

			//EditorGUI.indentLevel--;

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
			ICEEditorLayout.Label( "Basic Behaviours", true, Info.ESSENTIALS_BEHAVIOURS );
			EditorGUI.indentLevel++;
	
				_control.Creature.Essentials.BehaviourModeIdle = EditorBehaviour.BehaviourSelect( _control, "Idle", "Idle behaviour after reaching the current target move position.", _control.Creature.Essentials.BehaviourModeIdle, "IDLE" );
				_control.Creature.Essentials.BehaviourModeWalk = EditorBehaviour.BehaviourSelect( _control, "Walk", "Randomized leisure activities around the home area", _control.Creature.Essentials.BehaviourModeWalk, "WALK" );
				_control.Creature.Essentials.BehaviourModeRun = EditorBehaviour.BehaviourSelect( _control, "Run", "Move behaviour if your creature is on a journey", _control.Creature.Essentials.BehaviourModeRun, "RUN" );
				_control.Creature.Essentials.BehaviourModeJump = EditorBehaviour.BehaviourSelect( _control, "Jump", "Move behaviour if your creature is not grounded", _control.Creature.Essentials.BehaviourModeJump, "JUMP" );
				_control.Creature.Essentials.BehaviourModeDead = EditorBehaviour.BehaviourSelect( _control, "Dead", "Static behaviour if your creature is dead", _control.Creature.Essentials.BehaviourModeDead, "DEAD" );
				_control.Creature.Essentials.BehaviourModeSpawn = EditorBehaviour.BehaviourSelect( _control, "Spawn", "Idle behaviour during the respawn process", _control.Creature.Essentials.BehaviourModeSpawn, "SPAWN" );
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();

		}
	}
}
