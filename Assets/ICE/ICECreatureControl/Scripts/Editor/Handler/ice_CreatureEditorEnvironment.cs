// ##############################################################################
//
// ice_CreatureEditorEnvironment.cs
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
	
	public static class EditorEnvironment
	{	
		public static void Print( ICECreatureControl _control )
		{
			if( ! _control.Display.ShowEnvironmentSettings )
				return;
			
			string _surfaces = _control.Creature.Environment.SurfaceHandler.Surfaces.Count.ToString();
			string _impacts = _control.Creature.Environment.CollisionHandler.Collisions.Count.ToString();
			
			ICEEditorStyle.SplitterByIndent( 0 );
			_control.Display.FoldoutEnvironment = ICEEditorLayout.Foldout( _control.Display.FoldoutEnvironment, "Environment (" + _surfaces + "/" + _impacts + ")" , Info.ENVIROMENT );
			
			if( ! _control.Display.FoldoutEnvironment ) 
				return;

			EditorGUILayout.Separator();
				DrawEnvironmentSurfaceSettings( _control );				
				DrawEnvironmentCollisionSettings( _control );	
			EditorGUILayout.Separator();
				
		}

		
		private static void DrawEnvironmentSurfaceSettings( ICECreatureControl _control )
		{
			ICEEditorLayout.BeginHorizontal();
			_control.Creature.Environment.SurfaceHandler.Enabled = ICEEditorLayout.ToggleLeft( "Surfaces","", _control.Creature.Environment.SurfaceHandler.Enabled, true );
			if (GUILayout.Button(new GUIContent("ADD", "add a new surface rule"), ICEEditorStyle.ButtonMiddle))
			{
				_control.Creature.Environment.SurfaceHandler.Surfaces.Add( new SurfaceDataObject() );	
				_control.Creature.Environment.SurfaceHandler.Enabled = true;
			}
			if (GUILayout.Button(new GUIContent("RESET", "removes all surface rules"), ICEEditorStyle.ButtonMiddle))
			{
				_control.Creature.Environment.SurfaceHandler.Surfaces.Clear();	

				if( _control.Creature.Environment.SurfaceHandler.Surfaces.Count == 0 )
					_control.Creature.Environment.SurfaceHandler.Enabled = false;
			}
			ICEEditorLayout.EndHorizontal( Info.ENVIROMENT_SURFACE );

			if( _control.Creature.Environment.SurfaceHandler.Enabled == true && _control.Creature.Environment.SurfaceHandler.Surfaces.Count == 0 )
				_control.Creature.Environment.SurfaceHandler.Surfaces.Add( new SurfaceDataObject() );

			EditorGUI.BeginDisabledGroup( _control.Creature.Environment.SurfaceHandler.Enabled == false );

			EditorGUI.indentLevel++;

				_control.Creature.Environment.SurfaceHandler.ScanInterval = ICEEditorLayout.DefaultSlider( "Scan Interval", "Defines the interval for the ground check", _control.Creature.Environment.SurfaceHandler.ScanInterval, 0.25f, 0, 10, 1, Info.ENVIROMENT_SURFACE_SCAN_INTERVAL );
			
				for (int i = 0; i < _control.Creature.Environment.SurfaceHandler.Surfaces.Count; ++i)
				{
					// HEADER BEGIN
					SurfaceDataObject _surface = _control.Creature.Environment.SurfaceHandler.Surfaces[i];
					
					if(_surface.Name == "")
						_surface.Name = "Surface Rule #"+(i+1);
					
					ICEEditorLayout.BeginHorizontal();
						_surface.Foldout = ICEEditorLayout.Foldout( _surface.Foldout, _surface.Name );	
						_surface.Enabled = ICEEditorLayout.ButtonCheck( "ACTIVE", "activates/deactivates the rule", _surface.Enabled , ICEEditorStyle.ButtonMiddle );
				
						if (GUILayout.Button( new GUIContent( "REMOVE", "removes the selected surface rule"), ICEEditorStyle.ButtonMiddle ))
						{
							_control.Creature.Environment.SurfaceHandler.Surfaces.RemoveAt(i);
							--i;

							if( _control.Creature.Environment.SurfaceHandler.Surfaces.Count == 0 )
								_control.Creature.Environment.SurfaceHandler.Enabled = false;
						}
					ICEEditorLayout.EndHorizontal( Info.ENVIROMENT_SURFACE_RULE );
					// HEADER END

					// CONTENT
					
					if( _surface.Foldout ) 
					{
						EditorGUI.BeginDisabledGroup( _surface.Enabled == false );							
							ICEEditorLayout.BeginHorizontal();
								_surface.Name = ICEEditorLayout.Text("Name", "", _surface.Name );	
								if( GUILayout.Button( new GUIContent( "CLR", ""), ICEEditorStyle.CMDButtonDouble ))
									_surface.Name = "";
							ICEEditorLayout.EndHorizontal( Info.ENVIROMENT_SURFACE_RULE_NAME );											
							_surface.Interval = ICEEditorLayout.DefaultSlider( "Interval", "", _surface.Interval, 0.005f, 0, 30, 1, Info.ENVIROMENT_SURFACE_RULE_INTERVAL );
							
							//ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel + 1 );
							
							DrawEnvironmentTextures( _surface );
							
							ICEEditorLayout.Label( "Procedures" , true, Info.ENVIROMENT_SURFACE_RULE_PROCEDURES );
							EditorGUI.indentLevel++;
							EditorGUI.BeginDisabledGroup( _surface.Textures.Count == 0 );

								_surface.UseBehaviourModeKey = ICEEditorLayout.ToggleLeft("Behaviour", "", _surface.UseBehaviourModeKey, true, Info.ENVIROMENT_SURFACE_BEHAVIOUR  );
								if( _surface.UseBehaviourModeKey )
								{
									EditorGUI.indentLevel++;
										_surface.BehaviourModeKey = EditorBehaviour.BehaviourSelect( _control, "Behaviour","Reaction to this impact", _surface.BehaviourModeKey, "SURFACE_" + _surface.Name.ToUpper() );
									EditorGUI.indentLevel--;			
								}
								_surface.Influences = EditorSharedTools.DrawSharedInfluences( _surface.Influences, Info.ENVIROMENT_SURFACE_INFLUENCES, _control.Creature.Status.UseAdvanced );
								_surface.Audio = EditorSharedTools.DrawSharedAudio( _surface.Audio, Info.ENVIROMENT_SURFACE_AUDIO );
								_surface.Effect = EditorSharedTools.DrawSharedEffect( _control, _surface.Effect, Info.ENVIROMENT_SURFACE_EFFECT );								
							EditorGUI.EndDisabledGroup();
							EditorGUI.indentLevel--;
							EditorGUILayout.Separator();							
						EditorGUI.EndDisabledGroup();
						ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel + 1 );
					}
				}
				
			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();

		}
		
		private static void DrawEnvironmentCollisionSettings( ICECreatureControl _control )
		{

			// IMPACT HEADER BEGIN
			ICEEditorLayout.BeginHorizontal();
			_control.Creature.Environment.CollisionHandler.Enabled = ICEEditorLayout.ToggleLeft( "Collisions", "", _control.Creature.Environment.CollisionHandler.Enabled, true );
			if (GUILayout.Button( new GUIContent("ADD", "add a new impact rule"), ICEEditorStyle.ButtonMiddle ))
			{
				_control.Creature.Environment.CollisionHandler.Collisions.Add( new CollisionDataObject() ); 
				_control.Creature.Environment.CollisionHandler.Enabled = true;
			}
			if (GUILayout.Button(new GUIContent("RESET", "removes all impact rules"), ICEEditorStyle.ButtonMiddle))
			{
				_control.Creature.Environment.CollisionHandler.Collisions.Clear();	

				if( _control.Creature.Environment.CollisionHandler.Collisions.Count == 0 )
					_control.Creature.Environment.CollisionHandler.Enabled = false;
			}
			ICEEditorLayout.EndHorizontal( Info.ENVIROMENT_COLLISION );
			// IMPACT HEADER END

			if( _control.Creature.Environment.CollisionHandler.Enabled == true && _control.Creature.Environment.CollisionHandler.Collisions.Count == 0 )
				_control.Creature.Environment.CollisionHandler.Collisions.Add( new CollisionDataObject() ); 

			// IMPACT CONTENT BEGIN
			EditorGUI.BeginDisabledGroup( _control.Creature.Environment.CollisionHandler.Enabled == false );
			EditorGUI.indentLevel++;
				for( int i = 0; i < _control.Creature.Environment.CollisionHandler.Collisions.Count ; i++ )
				{
					CollisionDataObject _collision = _control.Creature.Environment.CollisionHandler.Collisions[i];
					
					if( _collision != null )
					{
			
						if( _collision.Name.Trim() == "" )
							_collision.Name = "Collision Rule #"+(i+1);
								
						// IMPACT RULE HEADER BEGIN
						ICEEditorLayout.BeginHorizontal();						
							_collision.Foldout = ICEEditorLayout.Foldout( _collision.Foldout, _collision.Name );
							_collision.Enabled = ICEEditorLayout.ButtonCheck( "ACTIVE", "activates/deactivates the selected collision rule", _collision.Enabled , ICEEditorStyle.ButtonMiddle );

							if( GUILayout.Button( new GUIContent( "REMOVE", "removes the selected collision rule"), ICEEditorStyle.ButtonMiddle ))
							{
								_control.Creature.Environment.CollisionHandler.Collisions.Remove( _collision );

								if( _control.Creature.Environment.CollisionHandler.Collisions.Count == 0 )
									_control.Creature.Environment.CollisionHandler.Enabled = false;
								return;
							}
						ICEEditorLayout.EndHorizontal(  Info.ENVIROMENT_COLLISION_RULE  );
						// IMPACT RULE HEADER END

						// IMPACT RULE CONTENT BEGIN
						if( _collision.Foldout ) 
						{
							EditorGUI.BeginDisabledGroup( _collision.Enabled == false );		
								ICEEditorLayout.BeginHorizontal();
									_collision.Name = ICEEditorLayout.Text("Name", "", _collision.Name );	
									if( GUILayout.Button( new GUIContent( "CLR", ""), ICEEditorStyle.CMDButtonDouble ))
										_collision.Name = "";
								ICEEditorLayout.EndHorizontal( Info.ENVIROMENT_COLLISION_RULE_NAME );

								EditorGUILayout.Separator();
							ICEEditorLayout.BeginHorizontal();
								ICEEditorLayout.Label( "Conditions" , true );

								_collision.UseTag = ICEEditorLayout.ButtonCheck( "TAG", "", _collision.UseTag, ICEEditorStyle.ButtonMiddle );
								_collision.UseLayer = ICEEditorLayout.ButtonCheck( "LAYER", "", _collision.UseLayer, ICEEditorStyle.ButtonMiddle );
								_collision.UseBodyPart = ICEEditorLayout.ButtonCheck( "COLLIDER", "", _collision.UseBodyPart, ICEEditorStyle.ButtonMiddle );
							ICEEditorLayout.EndHorizontal( Info.ENVIROMENT_COLLISION_RULE_CONDITIONS );
								EditorGUI.indentLevel++;

									if( _collision.UseLayer )
										_collision.Layer = ICEEditorLayout.Layer( "Layer","Desired collision layer", _collision.Layer, Info.ENVIROMENT_COLLISION_RULE_LAYER );
					
									if( _collision.UseTag )
										_collision.Tag = ICEEditorLayout.Tag( "Tag","Desired collision tag", _collision.Tag, Info.ENVIROMENT_COLLISION_RULE_TAG );
								
									if( _collision.UseBodyPart )
										_collision.BodyPart = ICEEditorLayout.ColliderPopup( _control.gameObject, "Body Part","Desired body part", _collision.BodyPart, Info.ENVIROMENT_COLLISION_RULE_BODYPART );
						
								EditorGUI.indentLevel--;	
									
								EditorGUILayout.Separator();
								ICEEditorLayout.Label( "Procedures" , true, Info.ENVIROMENT_COLLISION_RULE_PROCEDURES );
								EditorGUI.indentLevel++;
									_collision.UseBehaviourModeKey = ICEEditorLayout.ToggleLeft("Behaviour", "", _collision.UseBehaviourModeKey, true, Info.ENVIROMENT_COLLISION_BEHAVIOUR  );								
									if( _collision.UseBehaviourModeKey )
									{
										EditorGUI.indentLevel++;
											_collision.BehaviourModeKey = EditorBehaviour.BehaviourSelect( _control, "Behaviour","Reaction to this impact", _collision.BehaviourModeKey, "COLLISION_" + _collision.Name.ToUpper() );
										EditorGUI.indentLevel--;			
									}

									_collision.Influences = EditorSharedTools.DrawSharedInfluences( _collision.Influences, Info.ENVIROMENT_COLLISION_INFLUENCES, _control.Creature.Status.UseAdvanced );


									//_impact.ForceInteraction = EditorGUILayout.Toggle("Force Interaction", _impact.ForceInteraction );
									EditorGUI.indentLevel--;
								EditorGUILayout.Separator();
							
														
							EditorGUI.EndDisabledGroup();
							ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel + 1 );
						}
						// IMPACT RULE CONTENT END
					}
				}
				
				EditorGUILayout.Separator();
			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();
		}
		
		
		private static void DrawEnvironmentTextures( SurfaceDataObject _environment )
		{
			if( _environment == null )
				return;

			ICEEditorLayout.BeginHorizontal();
				ICEEditorLayout.Label( "Trigger Textures", true );
				if (GUILayout.Button( new GUIContent( "ADD", "Add a texture"), ICEEditorStyle.ButtonMiddle ))
					_environment.Textures.Add( new Texture() );		
			ICEEditorLayout.EndHorizontal( Info.ENVIROMENT_SURFACE_RULE_TEXTURES );

			if( _environment.Textures != null && _environment.Textures.Count > 0 )
			{
				int _width = 90;
				int _tolerance_space = 50 + (EditorGUI.indentLevel * 15) + _width;
				int _inspector_width = Screen.width - _tolerance_space;
				int _textures_width = 0;		
				int _max_count = 0;
				int _counter = 0;

				if( _inspector_width < 120 )
					_max_count = 3;

				for (int i = 0; i < _environment.Textures.Count; i++)
				{	
					if(_counter == 0)
					{
						ICEEditorLayout.BeginHorizontal();
						GUILayout.Space( EditorGUI.indentLevel * 15 );
					}

					int _indent = EditorGUI.indentLevel;
					
					EditorGUI.indentLevel = 0;
					GUILayout.BeginVertical("box", GUILayout.MinWidth(_width), GUILayout.MaxWidth(_width), GUILayout.MinHeight(90));
						_environment.Textures[i] = (Texture)EditorGUILayout.ObjectField(_environment.Textures[i], typeof(Texture), false, GUILayout.Height(75) );

					if( GUILayout.Button( "DELETE" ) )
					{
						_environment.Textures.RemoveAt(i);
						--i;
					}

					GUILayout.EndVertical();
					EditorGUI.indentLevel = _indent;					

					_counter++;					
					_textures_width = _counter * _width;
					if( _textures_width > _inspector_width || _counter == _max_count || i == _environment.Textures.Count - 1 )
					{
						ICEEditorLayout.EndHorizontal();
						EditorGUILayout.Separator();
						_counter = 0;
					}
				}
			}

			if(_environment.Textures.Count == 0)
				EditorGUILayout.HelpBox("No textures assigned. Press ADD to assign a texture!", MessageType.Info);
			
			EditorGUILayout.Separator();
			
		}
		

		

	}
}
