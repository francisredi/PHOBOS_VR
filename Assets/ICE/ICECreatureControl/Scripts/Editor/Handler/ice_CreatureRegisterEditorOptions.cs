// ##############################################################################
//
// ice_CreatureRegisterEditorOptions.cs
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
using ICE.Creatures.EditorInfos;
using ICE.Creatures.Utilities;
using ICE.Styles;
using ICE.Layouts;
using ICE.Utilities.EnumTypes;
using ICE.Utilities;


namespace ICE.Creatures.EditorHandler
{
	public static class EditorRegisterOptions
	{
		private static bool m_foldout = true;
		public static void Print( ICECreatureRegister _register )
		{
			EditorGUILayout.Separator();

			m_foldout =  ICEEditorLayout.Foldout( m_foldout, "Options", Info.REGISTER_OPTIONS );
			if( ! m_foldout )
				return;

			EditorGUILayout.Separator();
			EditorGUI.indentLevel++;

				// HIERACHY MANAGEMENT BEGIN
				_register.UseHierarchyManagement = ICEEditorLayout.ToggleLeft( "Use Hierarchy Management","",_register.UseHierarchyManagement, true, Info.REGISTER_OPTIONS_GROUPS  );
				if( _register.UseHierarchyManagement )
				{
					EditorGUI.indentLevel++;
						ICEEditorLayout.BeginHorizontal();
							EditorGUI.BeginDisabledGroup( _register.IgnoreRootGroup == true );
								_register.RootGroup = (Transform)EditorGUILayout.ObjectField( new GUIContent( "Root", ""), _register.RootGroup, typeof(Transform), true);
							EditorGUI.EndDisabledGroup();				
							_register.IgnoreRootGroup = ICEEditorLayout.ButtonCheck ( "IGNORE", "Ignores this group", _register.IgnoreRootGroup, ICEEditorStyle.ButtonMiddle );
						ICEEditorLayout.EndHorizontal( Info.REGISTER_OPTIONS_GROUPS_ROOT );
						EditorGUI.indentLevel++;
							ICEEditorLayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup( _register.IgnorePlayerGroup == true );
									_register.PlayerGroup = (Transform)EditorGUILayout.ObjectField( new GUIContent( "Player", ""), _register.PlayerGroup, typeof(Transform), true);
								EditorGUI.EndDisabledGroup();				
								_register.IgnorePlayerGroup = ICEEditorLayout.ButtonCheck ( "IGNORE", "Ignores this group", _register.IgnorePlayerGroup, ICEEditorStyle.ButtonMiddle );
							ICEEditorLayout.EndHorizontal( Info.REGISTER_OPTIONS_GROUPS_PLAYER );

							ICEEditorLayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup( _register.IgnoreCreatureGroup == true );
									_register.CreatureGroup = (Transform)EditorGUILayout.ObjectField( new GUIContent( "Creatures", ""), _register.CreatureGroup, typeof(Transform), true);
								EditorGUI.EndDisabledGroup();				
								_register.IgnoreCreatureGroup = ICEEditorLayout.ButtonCheck ( "IGNORE", "Ignores this group", _register.IgnoreCreatureGroup, ICEEditorStyle.ButtonMiddle );
							ICEEditorLayout.EndHorizontal( Info.REGISTER_OPTIONS_GROUPS_CREATURES );

							ICEEditorLayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup( _register.IgnoreItemGroup == true );
									_register.ItemGroup = (Transform)EditorGUILayout.ObjectField( new GUIContent( "Items", ""), _register.ItemGroup, typeof(Transform), true);
								EditorGUI.EndDisabledGroup();				
								_register.IgnoreItemGroup = ICEEditorLayout.ButtonCheck ( "IGNORE", "Ignores this group", _register.IgnoreItemGroup, ICEEditorStyle.ButtonMiddle );
							ICEEditorLayout.EndHorizontal( Info.REGISTER_OPTIONS_GROUPS_ITEMS );

							ICEEditorLayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup( _register.IgnoreLocationGroup == true );
									_register.LocationGroup = (Transform)EditorGUILayout.ObjectField( new GUIContent( "Locations", ""), _register.LocationGroup, typeof(Transform), true);
								EditorGUI.EndDisabledGroup();
								_register.IgnoreLocationGroup = ICEEditorLayout.ButtonCheck ( "IGNORE", "Ignores this group", _register.IgnoreLocationGroup, ICEEditorStyle.ButtonMiddle );
							ICEEditorLayout.EndHorizontal( Info.REGISTER_OPTIONS_GROUPS_LOCATIONS );

							ICEEditorLayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup( _register.IgnoreWaypointGroup == true );
									_register.WaypointGroup = (Transform)EditorGUILayout.ObjectField( new GUIContent( "Waypoints", ""), _register.WaypointGroup, typeof(Transform), true);
								EditorGUI.EndDisabledGroup();
								_register.IgnoreWaypointGroup = ICEEditorLayout.ButtonCheck ( "IGNORE", "Ignores this group", _register.IgnoreWaypointGroup, ICEEditorStyle.ButtonMiddle );
							ICEEditorLayout.EndHorizontal( Info.REGISTER_OPTIONS_GROUPS_WAYPOINTS );

							ICEEditorLayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup( _register.IgnoreMarkerGroup == true );
									_register.MarkerGroup = (Transform)EditorGUILayout.ObjectField( new GUIContent( "Marker", ""), _register.MarkerGroup, typeof(Transform), true);
								EditorGUI.EndDisabledGroup();
								_register.IgnoreMarkerGroup = ICEEditorLayout.ButtonCheck ( "IGNORE", "Ignores this group", _register.IgnoreMarkerGroup, ICEEditorStyle.ButtonMiddle );
							ICEEditorLayout.EndHorizontal( Info.REGISTER_OPTIONS_GROUPS_MARKERS );

							ICEEditorLayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup( _register.IgnoreOtherGroup == true );
									_register.OtherGroup = (Transform)EditorGUILayout.ObjectField( new GUIContent( "Other", ""), _register.OtherGroup, typeof(Transform), true);
								EditorGUI.EndDisabledGroup();
								_register.IgnoreOtherGroup = ICEEditorLayout.ButtonCheck ( "IGNORE", "Ignores this group", _register.IgnoreOtherGroup, ICEEditorStyle.ButtonMiddle );
							ICEEditorLayout.EndHorizontal( Info.REGISTER_OPTIONS_GROUPS_OTHER );
						EditorGUI.indentLevel--;
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();

					_register.ForceHierarchyGroups( true );
					
					ICEEditorLayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						if( ICEEditorLayout.Button( "Reorganize Hierarchy", "Sorts all creatures, items and locations according to the given groups.", ICEEditorStyle.ButtonExtraLarge ) )
							_register.ReorganizeHierarchy();
						GUILayout.FlexibleSpace();
					ICEEditorLayout.EndHorizontal();
					EditorGUILayout.Separator();
				}
				// HIERACHY MANAGEMENT END

				// POPULATION MANAGEMENT BEGIN
				_register.UsePoolManagement = ICEEditorLayout.ToggleLeft( "Use Pool Management","",_register.UsePoolManagement, true, Info.REGISTER_OPTIONS_POOL_MANAGEMENT  );
				if( _register.UsePoolManagement )
				{
					EditorGUI.indentLevel++;

						// GROUND CHECK BEGIN
						ICEEditorLayout.BeginHorizontal();
							_register.GroundCheck = (GroundCheckType)ICEEditorLayout.EnumPopup( "Spawn Ground Check","", _register.GroundCheck );	
							if( _register.GroundCheck == GroundCheckType.RAYCAST )
							{
								if (GUILayout.Button("Add Layer", ICEEditorStyle.ButtonMiddle ))
									_register.GroundLayers.Add( "Default" );

							}				
						ICEEditorLayout.EndHorizontal( Info.REGISTER_OPTIONS_POOL_MANAGEMENT_GROUND_CHECK );

						if( _register.GroundCheck != GroundCheckType.NONE )
						{
							EditorGUI.indentLevel++;
							if( _register.GroundCheck == GroundCheckType.RAYCAST )
								EditorSharedTools.DrawLayers( _register.GroundLayers );

							EditorGUI.indentLevel--;
							EditorGUILayout.Separator();
						}
						// GROUND CHECK END

					// OBSTACLE CHECK BEGIN
					ICEEditorLayout.BeginHorizontal();
						_register.ObstacleCheck = (ObstacleCheckType)ICEEditorLayout.EnumPopup( "Spawn Obstacle Check","", _register.ObstacleCheck );	
						if( _register.ObstacleCheck == ObstacleCheckType.BASIC )
						{
							if (GUILayout.Button("Add Layer", ICEEditorStyle.ButtonMiddle ))
								_register.ObstacleLayers.Add( "Default" );

						}				
					ICEEditorLayout.EndHorizontal( Info.REGISTER_OPTIONS_POOL_MANAGEMENT_OBSTACLE_CHECK );

					if( _register.ObstacleCheck != ObstacleCheckType.NONE )
					{
						EditorGUI.indentLevel++;
						if( _register.ObstacleCheck == ObstacleCheckType.BASIC )
							EditorSharedTools.DrawLayers( _register.ObstacleLayers );

						EditorGUI.indentLevel--;
						EditorGUILayout.Separator();
					}
					// OBSTACLE CHECK END

					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}
				// POPULATION MANAGEMENT END

				// SCENE MANAGEMENT BEGIN
				_register.UseSceneManagement = ICEEditorLayout.ToggleLeft( "Use Scene Management","",_register.UseSceneManagement, true, Info.REGISTER_OPTIONS_SCENE_MANAGEMENT  );
				if( _register.UseSceneManagement )
				{
					EditorGUI.indentLevel++;
						_register.UseDontDestroyOnLoad = ICEEditorLayout.Toggle( "Don't Destroy On Load", "", _register.UseDontDestroyOnLoad, Info.REGISTER_OPTIONS_DONTDESTROYONLOAD );
						_register.RandomSeed = (RandomSeedType)ICEEditorLayout.EnumPopup( "Random Seed","Sets the seed for the random number generator.", _register.RandomSeed, Info.REGISTER_OPTIONS_RANDOMSEED );
						if( _register.RandomSeed == RandomSeedType.CUSTOM )
						{
							EditorGUI.indentLevel++;
							_register.CustomRandomSeed = ICEEditorLayout.IntField( "Seed Value","Custom RandomSeed Integer Value", _register.CustomRandomSeed, Info.REGISTER_OPTIONS_RANDOMSEED_CUSTOM  );
							EditorGUI.indentLevel--;
							EditorGUILayout.Separator();
						}

					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}
				// SCENE MANAGEMENT END

			/*
				// ENVIRONMENT MANAGEMENT BEGIN
				_register.UseEnvironmentManagenent = ICEEditorLayout.ToggleLeft( "Use Environment Management", "", _register.UseEnvironmentManagenent, true, Info.REGISTER_OPTION_ENVIRONMENT );
				if( _register.UseEnvironmentManagenent )
				{
					EditorGUI.indentLevel++;				
					
						_register.EnvironmentInfos.UpdateTemperatureScale( (TemperatureScaleType)ICEEditorLayout.EnumPopup( "Temperature Scale","", _register.EnvironmentInfos.TemperatureScale ) );
						
						EditorGUI.indentLevel++;
						_register.EnvironmentInfos.MinTemperature = EditorGUILayout.FloatField( "Min. Temperature", _register.EnvironmentInfos.MinTemperature );
						_register.EnvironmentInfos.MaxTemperature = EditorGUILayout.FloatField( "Max. Temperature", _register.EnvironmentInfos.MaxTemperature );
						EditorGUI.indentLevel--;
						
						_register.EnvironmentInfos.Temperature = ICEEditorLayout.Slider( "Temperature","", _register.EnvironmentInfos.Temperature, 1,_register.EnvironmentInfos.MinTemperature,_register.EnvironmentInfos.MaxTemperature );
											
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}
				// ENVIRONMENT MANAGEMENT END
			*/
				


			//_register.NetworkAdapter = (NetworkAdapterType)ICEEditorLayout.EnumPopup( "Network Type", "Specifies the used network.", _register.NetworkAdapter, Info.REGISTER_OPTIONS_NETWORK );

			_register.UseDebug = ICEEditorLayout.ToggleLeft( "Use Debug", "", _register.UseDebug, true, Info.REGISTER_OPTIONS_DEBUG );
			if( _register.UseDebug )
			{
				if( _register.gameObject.GetComponent<ICECreatureRegisterDebug>() == null )
					_register.gameObject.AddComponent<ICECreatureRegisterDebug>();


				EditorGUI.indentLevel++;
					_register.UseDrawSelected = ICEEditorLayout.Toggle( "Draw Selected Only", "", _register.UseDrawSelected, Info.REGISTER_OPTIONS_DEBUG_GIZMOS_MODE );

					// BEGIN GIZMOS 
					ICEEditorLayout.BeginHorizontal();
						EditorGUI.BeginDisabledGroup( _register.ShowReferenceGizmos == false );
							_register.ColorReferences = EditorGUILayout.ColorField( new GUIContent( "References", ""),_register.ColorReferences);
							_register.ShowReferenceGizmosText = ICEEditorLayout.ButtonCheck( "TEXT", "Enables/Disables text labels",_register.ShowReferenceGizmosText, ICEEditorStyle.ButtonMiddle ); 
						EditorGUI.EndDisabledGroup();
						_register.ShowReferenceGizmos = ICEEditorLayout.ButtonCheck( "ENABLED", "Enables/Disables Reference Gizmos",_register.ShowReferenceGizmos, ICEEditorStyle.ButtonMiddle ); 
					ICEEditorLayout.EndHorizontal( Info.REGISTER_OPTIONS_DEBUG_REFERENCES );
					ICEEditorLayout.BeginHorizontal();
						EditorGUI.BeginDisabledGroup( _register.ShowCloneGizmos == false );
							_register.ColorClones = EditorGUILayout.ColorField( new GUIContent( "Clones", ""),_register.ColorClones);
							_register.ShowCloneGizmosText = ICEEditorLayout.ButtonCheck( "TEXT", "Enables/Disables text labels",_register.ShowCloneGizmosText, ICEEditorStyle.ButtonMiddle ); 
						EditorGUI.EndDisabledGroup();
						_register.ShowCloneGizmos = ICEEditorLayout.ButtonCheck( "ENABLED", "Enables/Disables Clones Gizmos",_register.ShowCloneGizmos, ICEEditorStyle.ButtonMiddle ); 
					ICEEditorLayout.EndHorizontal( Info.REGISTER_OPTIONS_DEBUG_CLONES );
					ICEEditorLayout.BeginHorizontal();
						EditorGUI.BeginDisabledGroup( _register.ShowSpawnPointGizmos == false );
							_register.ColorSpawnPoints = EditorGUILayout.ColorField( new GUIContent( "SpawnPoints", ""),_register.ColorSpawnPoints);
							_register.ShowSpawnPointGizmosText = ICEEditorLayout.ButtonCheck( "TEXT", "Enables/Disables text labels",_register.ShowSpawnPointGizmosText, ICEEditorStyle.ButtonMiddle ); 
						EditorGUI.EndDisabledGroup();
						_register.ShowSpawnPointGizmos = ICEEditorLayout.ButtonCheck( "ENABLED", "Enables/Disables SpawnPoint Gizmos",_register.ShowSpawnPointGizmos, ICEEditorStyle.ButtonMiddle ); 
					ICEEditorLayout.EndHorizontal( Info.REGISTER_OPTIONS_DEBUG_SPAWNPOINTS );
					// END GIZMOS 
				EditorGUI.indentLevel--;
			}


			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
			ICEEditorStyle.SplitterByIndent(0);
		}
	}
}
