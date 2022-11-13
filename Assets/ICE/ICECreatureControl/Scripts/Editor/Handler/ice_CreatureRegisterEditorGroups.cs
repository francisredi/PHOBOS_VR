// ##############################################################################
//
// ice_CreatureRegisterEditorCreatures.cs
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
using ICE.Creatures.EditorInfos;
using ICE.Creatures.Attributes;

namespace ICE.Creatures.EditorHandler
{
	public static class EditorRegisterGroups
	{
		private static bool m_foldout = true;
		public static void Print( ICECreatureRegister _register )
		{
			// HEADER BEGIN
			ICEEditorLayout.BeginHorizontal();
				if( _register.UseReferenceCategories )
					ICEEditorLayout.Label( "Reference Objects", true );
				else
					m_foldout =  ICEEditorLayout.Foldout( m_foldout, "Reference Objects" );
				_register.UseReferenceCategories = ICEEditorLayout.ButtonCheck( "GROUPS", "Shows Reference by Category Groups", _register.UseReferenceCategories, ICEEditorStyle.ButtonMiddle );
				if( GUILayout.Button("UPDATE", ICEEditorStyle.ButtonMiddle ) )
					_register.UpdateAllReferences();
			ICEEditorLayout.EndHorizontal( Info.REGISTER_REFERENCE_OBJECTS );
			// HEADER END

			if( ! m_foldout && ! _register.UseReferenceCategories )
				return;

			EditorGUI.indentLevel++;

			if( _register.UseReferenceCategories )
			{
				List<ReferenceGroupObject> _players = new List<ReferenceGroupObject>();
				List<ReferenceGroupObject> _creatures = new List<ReferenceGroupObject>();
				List<ReferenceGroupObject> _items = new List<ReferenceGroupObject>();
				List<ReferenceGroupObject> _locations = new List<ReferenceGroupObject>();
				List<ReferenceGroupObject> _waypoints = new List<ReferenceGroupObject>();
				List<ReferenceGroupObject> _marker = new List<ReferenceGroupObject>();
				List<ReferenceGroupObject> _undefined = new List<ReferenceGroupObject>();

				foreach( ReferenceGroupObject _group in _register.ReferenceGroupObjects )
				{
					if( _group.Reference == null )
						continue;

					if( _group.Reference.GetComponent<ICECreatureControl>() != null )
						_creatures.Add( _group );
					else if( _group.Reference.GetComponent<ICECreaturePlayer>() != null )
						_players.Add( _group );
					else if( _group.Reference.GetComponent<ICECreatureItem>() != null )
						_items.Add( _group );
					else if( _group.Reference.GetComponent<ICECreatureLocation>() != null )
						_locations.Add( _group );
					else if( _group.Reference.GetComponent<ICECreatureWaypoint>() != null )
						_waypoints.Add( _group );
					else if( _group.Reference.GetComponent<ICECreatureMarker>() != null )
						_marker.Add( _group );
					else 
						_undefined.Add( _group );
				}

				DrawReferenceGroupListExt( _register, "Players", _players, ref _register.CategoriesFoldout[0] );
				DrawReferenceGroupListExt( _register, "Creatures", _creatures, ref _register.CategoriesFoldout[1] );
				DrawReferenceGroupListExt( _register, "Items", _items, ref _register.CategoriesFoldout[2] );
				DrawReferenceGroupListExt( _register, "Locations", _locations, ref _register.CategoriesFoldout[3] );
				DrawReferenceGroupListExt( _register, "Waypoints", _waypoints, ref _register.CategoriesFoldout[4] );
				DrawReferenceGroupListExt( _register, "Markers", _marker, ref _register.CategoriesFoldout[5] );
				DrawReferenceGroupListExt( _register, "Other Objects", _undefined, ref _register.CategoriesFoldout[6] );
			}
			else
			{
				DrawReferenceGroupList( _register, _register.ReferenceGroupObjects );
			}
				EditorGUILayout.Separator();
				ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel );
				
				ICEEditorLayout.BeginHorizontal();
					GameObject _new = (GameObject)EditorGUILayout.ObjectField( "Add Reference Object", null, typeof(GameObject), true );					
					if( _new != null )
						_register.AddReference( _new );				
				ICEEditorLayout.EndHorizontal( Info.REGISTER_REFERENCE_OBJECTS_ADD );
		
		
			EditorGUI.indentLevel--;		
			EditorGUILayout.Separator();
			ICEEditorStyle.SplitterByIndent(0);
		}

		private static void DrawReferenceGroupListExt( ICECreatureRegister _register, string _list_title, List<ReferenceGroupObject> _list, ref bool _foldout )
		{
			EditorGUI.BeginDisabledGroup( _list.Count == 0 );

			_list_title += " (" + _list.Count + ")";
			_foldout =  ICEEditorLayout.Foldout( _foldout, _list_title, "" );
			EditorGUI.indentLevel++;
				if( _foldout )
				{
					for( int _index = 0 ; _index < _list.Count ; _index++ )
					{
						ReferenceGroupObject _obj = _list[_index];
						
						UpdateStatus( _obj );
						
						if( _obj != null && _obj.Reference != null )
						{
							string _amount = "";
							if( _obj.PoolManagementEnabled )
								_amount = " [" + _obj.Items.Count + " of " + _obj.MaxObjects + "]";
							else
								_amount = " [" + _obj.Items.Count + "]";
							
							
							string _title = _obj.Name + " " + ( ICEEditorTools.IsPrefab( _obj.Reference )?"(PREFAB)":"(SCENE)") + _amount;
							ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel );
							ICEEditorLayout.BeginHorizontal();
								_obj.Foldout = EditorGUILayout.Foldout( _obj.Foldout, _title , ICEEditorStyle.Foldout );
								GUILayout.FlexibleSpace();					
								
								DrawReferenceTypeButton( _obj );
								/*
								if( ICEEditorLayout.ButtonUp() )
								{
									ReferenceGroupObject _tmp_obj = _list[_index]; 
									_list.RemoveAt( _index );
									
									if( _index - 1 < 0 )
										_list.Add( _tmp_obj );
									else
										_list.Insert( _index - 1, _tmp_obj );
									
									return;
								}	
								
								
								if( ICEEditorLayout.ButtonDown() )
								{
									ReferenceGroupObject _tmp_obj = _list[_index]; 
									_list.RemoveAt( _index );
									
									if( _index + 1 > _list.Count )
										_list.Insert( 0, _tmp_obj );
									else
										_list.Insert( _index +1, _tmp_obj );
									
									return;
								}	*/

								if( _obj.GroupType == HierarchyGroupType.Players )
									_obj.PoolManagementEnabled = false;
						
								_obj.GroupByTag = ICEEditorLayout.ButtonCheck( "TAG", "Group objects by tag", _obj.GroupByTag, ICEEditorStyle.ButtonMiddle );

								EditorGUI.BeginDisabledGroup( _register.UsePoolManagement == false || _obj.GroupType == HierarchyGroupType.Players  );
									_obj.PoolManagementEnabled = ICEEditorLayout.ButtonCheck( "POOL", "Activates Pool Management", _obj.PoolManagementEnabled, ICEEditorStyle.ButtonMiddle );
								EditorGUI.EndDisabledGroup();

								_obj.Enabled = ICEEditorLayout.ButtonCheck( "ENABLED", "Enables/Disables the group", _obj.Enabled, ICEEditorStyle.ButtonMiddle );

																	
								if (GUILayout.Button("x", ICEEditorStyle.CMDButton ))
								{
									ICECreatureRegister.Instance.ReferenceGroupObjects.Remove( _obj );
									return;
								}
							
							ICEEditorLayout.EndHorizontal(  Info.REGISTER_REFERENCE_OBJECT_GROUP );
							
					
						DrawReferenceGroup( _register, _obj );
							
						}
						else
						{
							_list.RemoveAt(_index);
							--_index;
						}
					}

				if( _list.Count > 0 )
					EditorGUILayout.Separator();

			}
			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();
		}

		private static void DrawReferenceGroupList( ICECreatureRegister _register, List<ReferenceGroupObject> _list )
		{
			for( int _index = 0 ; _index < _list.Count ; _index++ )
			{
				ReferenceGroupObject _obj = _list[_index];
				
				UpdateStatus( _obj );
				
				if( _obj != null && _obj.Reference != null )
				{
					string _amount = "";
					if( _obj.PoolManagementEnabled )
						_amount = " [" + _obj.Items.Count + " of " + _obj.MaxObjects + "]";
					else
						_amount = " [" + _obj.Items.Count + "]";
					
					
					string _title = _obj.Name + " " + ( ICEEditorTools.IsPrefab( _obj.Reference )?"(PREFAB)":"(SCENE)") + _amount;
					ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel );
					ICEEditorLayout.BeginHorizontal();
						_obj.Foldout = EditorGUILayout.Foldout( _obj.Foldout, _title , ICEEditorStyle.Foldout );
						GUILayout.FlexibleSpace();					
						
						DrawReferenceTypeButton( _obj );
						
						if( ICEEditorLayout.ButtonUp() )
						{
							ReferenceGroupObject _tmp_obj = _list[_index]; 
							_list.RemoveAt( _index );
							
							if( _index - 1 < 0 )
								_list.Add( _tmp_obj );
							else
								_list.Insert( _index - 1, _tmp_obj );
							
							return;
						}	
						
						
						if( ICEEditorLayout.ButtonDown() )
						{
							ReferenceGroupObject _tmp_obj = _list[_index]; 
							_list.RemoveAt( _index );
							
							if( _index + 1 > _list.Count )
								_list.Insert( 0, _tmp_obj );
							else
								_list.Insert( _index +1, _tmp_obj );
							
							return;
						}	
						if( _obj.GroupType == HierarchyGroupType.Players )
							_obj.PoolManagementEnabled = false;
					
						_obj.GroupByTag = ICEEditorLayout.ButtonCheck( "TAG", "Group objects by tag", _obj.GroupByTag, ICEEditorStyle.ButtonMiddle );
						EditorGUI.BeginDisabledGroup( _register.UsePoolManagement == false || _obj.GroupType == HierarchyGroupType.Players );
							_obj.PoolManagementEnabled = ICEEditorLayout.ButtonCheck( "POOL", "Activates Pool Management", _obj.PoolManagementEnabled, ICEEditorStyle.ButtonMiddle );
						EditorGUI.EndDisabledGroup();
						_obj.Enabled = ICEEditorLayout.ButtonCheck( "ENABLED", "Enables/Disables the group", _obj.Enabled, ICEEditorStyle.ButtonMiddle );
						if (GUILayout.Button("x", ICEEditorStyle.CMDButton ))
						{
							_list.Remove( _obj );
							_obj = null;
							--_index;
							//return;
						}
					
					ICEEditorLayout.EndHorizontal( Info.REGISTER_REFERENCE_OBJECT_GROUP );
					
					

					DrawReferenceGroup( _register, _obj );
					
				}
				else
				{
					_list.RemoveAt(_index);
					return;
				}
			}
		}

		private static void DrawReferenceTypeButton( ReferenceGroupObject _obj )
		{
			GUI.backgroundColor = Color.cyan;
			if( _obj.Reference.GetComponent<ICECreatureControl>() != null )
			{
				if( GUILayout.Button(new GUIContent( "CC", "Remove Creature Script" ), ICEEditorStyle.CMDButtonDouble ))
					GameObject.DestroyImmediate( _obj.Reference.GetComponent <ICECreatureControl>(), true ); 							
			}
			else if( _obj.Reference.GetComponent<ICECreaturePlayer>() != null )
			{
				if( GUILayout.Button(new GUIContent( "CP", "Remove Player Script" ), ICEEditorStyle.CMDButtonDouble ))
					GameObject.DestroyImmediate( _obj.Reference.GetComponent <ICECreaturePlayer>(), true ); 							
			}
			else if( _obj.Reference.GetComponent<ICECreatureItem>() != null )
			{
				if( GUILayout.Button(new GUIContent( "CI", "Remove Item Script" ), ICEEditorStyle.CMDButtonDouble ))
					GameObject.DestroyImmediate( _obj.Reference.GetComponent <ICECreatureItem>(), true ); 							
			}
			else if( _obj.Reference.GetComponent<ICECreatureLocation>() != null )
			{
				if( GUILayout.Button(new GUIContent( "CL", "Remove Location Script" ), ICEEditorStyle.CMDButtonDouble ))
					GameObject.DestroyImmediate( _obj.Reference.GetComponent <ICECreatureLocation>(), true ); 							
			}
			else if( _obj.Reference.GetComponent<ICECreatureWaypoint>() != null )
			{
				if( GUILayout.Button(new GUIContent( "CW", "Remove Waypoint Script" ), ICEEditorStyle.CMDButtonDouble ))
					GameObject.DestroyImmediate( _obj.Reference.GetComponent <ICECreatureWaypoint>(), true); 							
			}
			else if( _obj.Reference.GetComponent<ICECreatureMarker>() != null )
			{
				if( GUILayout.Button(new GUIContent( "CM", "Remove Marker Script" ), ICEEditorStyle.CMDButtonDouble ))
					GameObject.DestroyImmediate( _obj.Reference.GetComponent <ICECreatureMarker>(), true ); 							
			}
			else if( _obj.Reference.GetComponent<ICECreatureControl>() == null &&
				_obj.Reference.GetComponent<ICECreaturePlayer>() == null &&
				_obj.Reference.GetComponent<ICECreatureItem>() == null &&
				_obj.Reference.GetComponent<ICECreatureWaypoint>() == null &&
				_obj.Reference.GetComponent<ICECreatureMarker>() == null &&
				_obj.Reference.GetComponent<ICECreatureLocation>() == null )
			{
				GUI.backgroundColor = Color.yellow;
				if( GUILayout.Button(new GUIContent( "CC", "Add Creature Script" ), ICEEditorStyle.CMDButtonDouble ))
					_obj.Reference.AddComponent<ICECreatureControl>();
				if( GUILayout.Button(new GUIContent( "CP", "Add Player Script" ), ICEEditorStyle.CMDButtonDouble ))
					_obj.Reference.AddComponent<ICECreaturePlayer>();
				if( GUILayout.Button(new GUIContent( "CL", "Add Location Script" ), ICEEditorStyle.CMDButtonDouble ))
					_obj.Reference.AddComponent<ICECreatureLocation>();
				if( GUILayout.Button(new GUIContent( "CW", "Add Waypoint Script" ), ICEEditorStyle.CMDButtonDouble ))
					_obj.Reference.AddComponent<ICECreatureWaypoint>();
				if( GUILayout.Button(new GUIContent( "CM", "Add Marker Script" ), ICEEditorStyle.CMDButtonDouble ))
					_obj.Reference.AddComponent<ICECreatureMarker>();
				if( GUILayout.Button(new GUIContent( "CI", "Add Item Script" ), ICEEditorStyle.CMDButtonDouble ))
					_obj.Reference.AddComponent<ICECreatureItem>();
			}

			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

			GUILayout.Space( 10 );
		}
		
		private static void DrawReferenceGroup( ICECreatureRegister _register, ReferenceGroupObject _obj )
		{
			if( _obj == null || _obj.Foldout == false )
				return;
			
			EditorGUI.indentLevel++;

				// BEGIN OBJECT
				ICEEditorLayout.BeginHorizontal();
					_obj.Reference = (GameObject)EditorGUILayout.ObjectField( "Reference Object", _obj.Reference, typeof(GameObject), true );			
					EditorGUI.BeginDisabledGroup( _obj.Reference == null );	
						if( _obj.Reference != null )
							ICEEditorLayout.ButtonShowObject( _obj.Reference.transform.position );
						else
							ICEEditorLayout.ButtonShowObject( Vector3.zero );
						ICEEditorLayout.ButtonSelectObject( _obj.Reference );
						_obj.UseHierarchyGroupObject = ICEEditorLayout.ButtonCheck( "GROUP", "Assorts instances to the defined Hierarchy Group", _obj.UseHierarchyGroupObject, ICEEditorStyle.ButtonMiddle ); 
					EditorGUI.EndDisabledGroup();
				ICEEditorLayout.EndHorizontal( Info.REGISTER_REFERENCE_OBJECT );
				// END OBJECT

				//_obj.UseHierarchyGroupObject = ICEEditorLayout.Toggle( "Use Hierarchy Group", "", _obj.UseHierarchyGroupObject, Info.REGISTER_REFERENCE_OBJECT_POOL_GROUP_USE );
				if( _obj.UseHierarchyGroupObject )
				{
					EditorGUI.indentLevel++;
					ICEEditorLayout.BeginHorizontal();
					_obj.CustomHierarchyGroupObject = (GameObject)EditorGUILayout.ObjectField( "Custom Hierarchy Group", _obj.CustomHierarchyGroupObject, typeof(GameObject), true );
					ICEEditorLayout.EndHorizontal( Info.REGISTER_REFERENCE_OBJECT_POOL_GROUP_CUSTOM );
					EditorGUI.indentLevel--;
				}

				// BEGIN POOL MANAGEMENT
				if( _obj.PoolManagementEnabled == true && _register.UsePoolManagement == true )
				{
					ICEEditorLayout.BeginHorizontal();
						_obj.MaxObjects = (int)ICEEditorLayout.Slider( "Max. Spawn Objects (" + _obj.Count + ")","", _obj.MaxObjects, 1, 0, 250 );
						_obj.UseInitialSpawn = ICEEditorLayout.ButtonCheck( "INITIAL", "Spawns all instances on start according to the given priority", _obj.UseInitialSpawn , ICEEditorStyle.ButtonMiddle );				
					ICEEditorLayout.EndHorizontal( Info.REGISTER_REFERENCE_OBJECT_POOL_SPAWN_MAX  );

					if( _obj.UseInitialSpawn )
					{
						EditorGUI.indentLevel++;
						_obj.InitialSpawnPriority = (int)ICEEditorLayout.DefaultSlider( "Initial Spawn Priority","", _obj.InitialSpawnPriority, 1, 0, 100,0, Info.REGISTER_REFERENCE_OBJECT_POOL_SPAWN_PRIORITY );
						EditorGUI.indentLevel--;
					}

					ICEEditorLayout.RandomMinMaxGroupExt( "Spawn Interval (min/max)", "", ref _obj.MinSpawnInterval, ref _obj.MaxSpawnInterval,0, ref _obj.RespawnIntervalMax,0,0, 30,0.25f, Info.REGISTER_REFERENCE_OBJECT_POOL_SPAWN_INTERVAL ); 

					EditorGUILayout.Separator();
					_obj.UseRandomSize = ICEEditorLayout.Toggle( "Random Size", "", _obj.UseRandomSize, Info.REGISTER_REFERENCE_OBJECT_POOL_RANDOM_SIZE );
					EditorGUI.BeginDisabledGroup( _obj.UseRandomSize == false );
						EditorGUI.indentLevel++;
							ICEEditorLayout.RandomMinMaxGroup( "Size Variance (min/max)", "", ref _obj.RandomSizeMin, ref _obj.RandomSizeMax,-1,1,0,0, 0.025f, Info.REGISTER_REFERENCE_OBJECT_POOL_RANDOM_SIZE_VARIANCE ); 
						EditorGUI.indentLevel--;
					EditorGUI.EndDisabledGroup();
					
					EditorGUILayout.Separator();
					_obj.UseSoftRespawn = ICEEditorLayout.Toggle( "Soft Respawn", "Reuse of suspended objects without new instantiations", _obj.UseSoftRespawn, Info.REGISTER_REFERENCE_OBJECT_POOL_SOFTRESPAWN );

					ICEEditorLayout.Label( "Spawn Points", false );
					EditorGUI.indentLevel++;

						if( _obj.SpawnPoints.Count == 0 )
						{
							if( _obj.CreatureController != null )
								_obj.SpawnPoints.Add( new SpawnPointObject( _obj.CreatureController.Creature.Essentials.Target ) );						
							else if( _obj.Reference != null )
							{
								ICECreatureTargetAttribute _target = _obj.Reference.GetComponent<ICECreatureTargetAttribute>();
								if( _target != null )
									_obj.SpawnPoints.Add( new SpawnPointObject( _target.Target ) );
								else
									_obj.SpawnPoints.Add( new SpawnPointObject( _obj.Reference ) );
							}
						}

						foreach( SpawnPointObject _point in _obj.SpawnPoints )
						{
							ICEEditorLayout.BeginHorizontal();
								EditorSharedTools.BasicSpawnPointObject( _point, "SpawnPoint", "" );

						//EditorGUI.BeginDisabledGroup( (_obj.CreatureController.Creature.Essentials.Target.TargetGameObject == _point.SpawnPointGameObject?true:false) );
								if( ICEEditorLayout.Button( "DEL", "", ICEEditorStyle.CMDButtonDouble ) )
								{
									_obj.SpawnPoints.Remove( _point );
									return;
								}
						//EditorGUI.EndDisabledGroup();
							ICEEditorLayout.EndHorizontal( Info.REGISTER_REFERENCE_OBJECT_SPAWN_POINT  );

							EditorGUI.indentLevel++;
								ICEEditorLayout.MinMaxGroupSimple( "Random Range", "Random positioning range around the spawn point", ref _point.MinSpawningRange, ref _point.MaxSpawningRange, 0, ref _point.SpawningRangeMax, 0.25f, 40, Info.REGISTER_REFERENCE_OBJECT_SPAWN_POINT_RANGE );
							EditorGUI.indentLevel--;
						}

						ICEEditorLayout.BeginHorizontal();
							ICEEditorLayout.Label( "Add Spawn Point", false );

							if( ICEEditorLayout.Button( "ADD", "", ICEEditorStyle.CMDButtonDouble ) )
								_obj.SpawnPoints.Add( new SpawnPointObject() );
						ICEEditorLayout.EndHorizontal( Info.REGISTER_REFERENCE_OBJECT_POOL_SPAWN_MAX  );
					EditorGUI.indentLevel--;
				}
				// END POOL MANAGEMENT



			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();

		}

		private static void DrawFlags( ReferenceGroupObject _object )
		{

			string[] _flags = new string[10];

			// CC controlled
			if( _object.Status.HasCreatureController )
				_flags[0] = "icons/cc_1";
			else if( _object.Status.HasCreatureAdapter )
				_flags[0] = "icons/failed";
			else
				_flags[0] = "icons/failed";

			if( _object.Status.HasHome )
				_flags[1] = "icons/home_ready";
			else
				_flags[1] = "icons/home_failed";

			if( _object.Status.HasMissionOutpost )
				_flags[2] = "icons/cc_1";
			else
				_flags[2] = "icons/failed";

			if( _object.Status.HasMissionEscort ) 
				_flags[3] = "icons/cc_1";
			else
				_flags[3] = "icons/failed";

			if( _object.Status.HasMissionPatrol ) 
				_flags[4] = "icons/cc_1";
			else
				_flags[4] = "icons/failed";

			if( _object.Status.isActiveAndEnabled ) 
				_flags[5] = "icons/cc_1";
			else
				_flags[5] = "icons/failed";

			if( _object.Status.isActiveInHierarchy ) 
				_flags[6] = "icons/cc_1";
			else
				_flags[6] = "icons/failed";

			if( _object.Status.isPrefab ) 
				_flags[7] = "icons/cc_1";
			else
				_flags[7] = "icons/failed";

			//EditorGUILayout.Separator();
			ICEEditorLayout.DrawLabelIconBar( "Status", _flags, 16, 16, 0,0,5);
		}

		/// <summary>
		/// Updates the creature status.
		/// </summary>
		/// <param name="_object">_object.</param>
		private static void UpdateStatus( ReferenceGroupObject _object )
		{
			if( _object == null )
				return;
			
			_object.Status.HasCreatureController = false;
			_object.Status.HasCreatureAdapter = false;
			_object.Status.HasHome = false;
			_object.Status.HasMissionOutpost = false;
			_object.Status.HasMissionEscort = false;
			_object.Status.HasMissionPatrol = false;
			_object.Status.isActiveAndEnabled = false;
			_object.Status.isActiveInHierarchy = false;
			_object.Status.isPrefab = false;
			
			if( _object.Reference != null )
			{
				if( _object.CreatureController != null )
				{
					_object.Status.HasCreatureController = true;
					
					if( _object.CreatureController.isActiveAndEnabled )
						_object.Status.isActiveAndEnabled = true;
					
					if( _object.CreatureController.Creature.Essentials.TargetReady() )
						_object.Status.HasHome = true;
					
					if( _object.CreatureController.Creature.Missions.Outpost.TargetReady() )
						_object.Status.HasMissionOutpost = true;
					
					if( _object.CreatureController.Creature.Missions.Escort.TargetReady() )
						_object.Status.HasMissionEscort = true;
					
					if( _object.CreatureController.Creature.Missions.Patrol.TargetReady() )
						_object.Status.HasMissionPatrol = true;
					
				}
				
				if( _object.Reference.activeInHierarchy )
					_object.Status.isActiveInHierarchy = true;
				else if( ICEEditorTools.IsPrefab( _object.Reference ) ) // Is a prefab
					_object.Status.isPrefab = true;
				
			}
		}


	}
}
