// ##############################################################################
//
// ice_CreatureWizard.cs
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
using UnityEditor;
using ICE;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Styles;
using ICE.Layouts;
using ICE.Shared;
using ICE.Utilities;
using ICE.Utilities.EnumTypes;
using ICE.Creatures.EditorInfos;
using ICE.Creatures.Attributes;
using ICE.Creatures.Extensions;
using ICE.Environment;

using ICE.Creatures.EditorHandler;




namespace ICE.Creatures.Windows
{
	public class ice_CreatureWizard : EditorWindow {

		public static Texture2D m_ICECCLogo = (Texture2D)Resources.Load("ICECC_LOGO", typeof(Texture2D));
		public static Texture2D m_ICELogo = (Texture2D)Resources.Load("ICE_LOGO", typeof(Texture2D));

		private static Vector2 m_DialogSize = new Vector2(520, 700);
		private static string m_Version = "Version " + Info.Version;
		private static string m_Copyright = "© " + System.DateTime.Now.Year + " Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.";

		private ICECreatureRegister m_CreatureRegister = null;
		private ICEEnvironmentController m_Environment = null;
		private ICECreatureControl m_CreatureControl = null;
		private ICECreaturePlayer m_CreaturePlayer = null;
		private GameObject m_Ground = null;
		private float m_CreatureSizeMultiplier = 1;
		//private int m_GroundLayer = -1;

		private bool m_ShowGroundHandling = false;
		private bool m_ShowPlayerHandling = false;
		private bool m_ShowCreatureHandling = false;
		//private bool m_ShowEnvironmentHandling = false;
		/// <summary>
		/// 
		/// </summary>
		public static void Create()
		{

			ice_CreatureWizard _msg_box = (ice_CreatureWizard)EditorWindow.GetWindow(typeof(ice_CreatureWizard), true);

			_msg_box.titleContent = new GUIContent( "ICE Creature Control Wizard v0.8 (beta)", "");

			_msg_box.minSize = new Vector2(m_DialogSize.x, m_DialogSize.y);
			_msg_box.maxSize = new Vector2(m_DialogSize.x + 1, m_DialogSize.y + 1);
			_msg_box.position = new Rect(
				(Screen.currentResolution.width / 2) - (m_DialogSize.x / 2),
				(Screen.currentResolution.height / 2) - (m_DialogSize.y / 2),
				m_DialogSize.x,
				m_DialogSize.y);
			_msg_box.Show();



		}

		private bool CheckRegister()
		{
			
			ICECreatureRegister[] _registers = GameObject.FindObjectsOfType<ICECreatureRegister>();
			if( _registers.Length == 1 ) 
			{
				m_CreatureRegister = _registers[0];
				return true;
			}
			else 
			{
				m_CreatureRegister = null;
				return false;
			}
				
		}

		private bool CheckEnvironment()
		{

			ICEEnvironmentController[] _environments = GameObject.FindObjectsOfType<ICEEnvironmentController>();
			if( _environments.Length == 1 ) 
			{
				m_Environment = _environments[0];
				return true;
			}
			else 
			{
				m_Environment = null;
				return false;
			}

		}

		private bool CheckGround()
		{
			if( m_CreatureRegister == null )
				return false;

			if( m_CreatureRegister.GroundCheck == GroundCheckType.SAMPLEHEIGHT && Terrain.activeTerrain != null  )
				return true;
			else if( m_CreatureRegister.GroundCheck == GroundCheckType.RAYCAST && m_CreatureRegister.GroundLayers.Count > 0 )
			{
				//if( Terrain.activeTerrain != null && GraphicTools.IsInLayerMask( Terrain.activeTerrain.gameObject, m_CreatureRegister.GroundLayerMask ) )

				return true;
			}
			else
			{
				return false;
			}
		}

		private bool CheckPlayer()
		{
			ICECreaturePlayer[] _players = GameObject.FindObjectsOfType<ICECreaturePlayer>();
			if( _players.Length >= 1 )
			{
				m_CreaturePlayer = _players[0];

				GameObject _cam = GameObject.Find( "Main Camera" );
				if( _cam )
					_cam.SetActive( false );
				
				return true;
			}
			else
			{
				m_CreaturePlayer = null;
				return false;
			}
		}

		private bool CheckCreature()
		{
			if( m_CreatureControl == null )
				return false;


			return true;
		}

		private void SpawnRandomLocations( GameObject _ground, int _count = 10 )
		{
			if( m_CreatureRegister == null )
				return;

			for(int i= 0; i< _count ; i++ )
			{
				GameObject _obj = GameObject.CreatePrimitive( PrimitiveType.Cylinder );


				_obj.name = "LocationTarget";
				int _radius = Random.Range( 25, 60 );
				_obj.transform.localScale = new Vector3( _radius, 0.1f, _radius );
				Vector3 _pos = new Vector3( Random.Range( -400,400 ), 0, Random.Range( -400,400 ) );

				CapsuleCollider _capsule_collider = _obj.GetComponent<CapsuleCollider>();
				if( _capsule_collider != null )
					_capsule_collider.enabled = false;

				_obj.transform.position = _pos;
				_obj.AddComponent<ICECreatureLocation>();
				Renderer _renderer = _obj.GetComponent<Renderer>();
				if( _renderer != null )
				{
					_renderer.sharedMaterial = new Material( Shader.Find("Standard") );
					_renderer.sharedMaterial.color = Color.red;
				}
		
				m_CreatureRegister.AddReference( _obj );

			}

			m_CreatureRegister.ReorganizeHierarchy();
		}

		private void SpawnRandomWater( GameObject _ground, int _count = 10 )
		{
			if( m_CreatureRegister == null )
				return;

			GameObject _ponds = GameObject.Find( "Ponds" );
			if( _ponds == null )
			{
				_ponds = new GameObject();
				_ponds.name = "Ponds";
			}

			for(int i= 0; i< _count ; i++ )
			{
				GameObject _obj = GameObject.CreatePrimitive( PrimitiveType.Cube );

				if( _ponds != null )
					_obj.transform.parent = _ponds.transform;
				
				_obj.name = "Pond";		
				int _radius = Random.Range( 20, 60 );
				_obj.transform.eulerAngles = new Vector3( 0, Random.Range(0,360 ), 0 );
				_obj.transform.localScale = new Vector3( _radius, 0.1f, _radius );
				Vector3 _pos = new Vector3( Random.Range( -400,400 ), 0, Random.Range( -400,400 ) );
				_obj.layer = LayerMask.NameToLayer( "Water" );

				_obj.transform.position = _pos;
				//_obj.AddComponent<ICECreatureLocation>();
				Renderer _renderer = _obj.GetComponent<Renderer>();
				if( _renderer != null )
				{
					_renderer.sharedMaterial = new Material( Shader.Find("Standard") );
					_renderer.sharedMaterial.color = Color.blue;
				}

				//m_CreatureRegister.AddReference( _obj );

			}

			//m_CreatureRegister.ReorganizeHierarchy();
		}

		private void SpawnRandomWaypoints( GameObject _ground, int _count = 10 )
		{
			if( m_CreatureRegister == null )
				return;

			for(int i= 0; i< _count ; i++ )
			{
				GameObject _obj = GameObject.CreatePrimitive( PrimitiveType.Cylinder );

				_obj.name = "WayPointTarget" + i;
				float _radius = 0.25f;
				_obj.transform.localScale = new Vector3( _radius, 5, _radius );
				Vector3 _pos = new Vector3( Random.Range( -400,400 ), 0, Random.Range( -400,400 ) );

				_pos.y = _obj.transform.localScale.y/2;
				_obj.transform.position = _pos;
				_obj.AddComponent<ICECreatureWaypoint>();
				Renderer _renderer = _obj.GetComponent<Renderer>();
				if( _renderer != null )
				{
					_renderer.sharedMaterial = new Material( Shader.Find("Standard") );
					_renderer.sharedMaterial.color = Color.blue;
				}

				m_CreatureRegister.AddReference( _obj );

			}

			m_CreatureRegister.ReorganizeHierarchy();
		}

		private void SpawnRandomItems( GameObject _ground, int _count = 250 )
		{
			if( m_CreatureRegister == null )
				return;

			for(int i= 0; i< _count ; i++ )
			{
				int _selection = Random.Range(0,5 );

				PrimitiveType _type = PrimitiveType.Cube;
				if( _selection == 0 )
					_type = PrimitiveType.Sphere;
				else if( _selection == 1 )
					_type = PrimitiveType.Cylinder;
				else if( _selection == 2 )
					_type = PrimitiveType.Capsule;
				else
					_type = PrimitiveType.Cube;
				
				GameObject _obj = GameObject.CreatePrimitive( _type );

				_obj.name += "Item";
				_obj.transform.eulerAngles = new Vector3( 0, Random.Range(0,360 ), 0 );
				_obj.transform.localScale = new Vector3( Random.Range( 0.1f,1.5f ), Random.Range( 0.1f,1.5f ), Random.Range( 0.1f,1.5f) );
				Vector3 _pos = new Vector3( Random.Range( -400,400 ), 0, Random.Range( -400,400 ) );

				_pos.y = _obj.transform.localScale.y/2;
				_obj.transform.position = _pos;
				_obj.AddComponent<Rigidbody>();
				//_obj.AddComponent<LODGroup>();
				_obj.AddComponent<ICECreatureItem>();
				_obj.AddComponent<ICECreatureInventoryExtension>();
				Renderer _renderer = _obj.GetComponent<Renderer>();
				if( _renderer != null )
				{
					_renderer.sharedMaterial = new Material( Shader.Find("Standard") );
					_renderer.sharedMaterial.color = UnityEngine.Random.ColorHSV();
				}
		
				m_CreatureRegister.AddReference( _obj );
			}

			m_CreatureRegister.ReorganizeHierarchy();
		}

		private void SpawnRandomObstacles( GameObject _ground, int _count = 250 )
		{
			if( m_CreatureRegister == null )
				return;

			GameObject _obstacles = GameObject.Find( "Obstacles" );
			if( _obstacles == null )
			{
				_obstacles = new GameObject();
				_obstacles.name = "Obstacles";
			}
			
			for(int i= 0; i< _count ; i++ )
			{
				GameObject _obj = GameObject.CreatePrimitive( PrimitiveType.Cube );

				if( _obstacles != null )
					_obj.transform.parent = _obstacles.transform;

				_obj.name = "Obstacle";
				_obj.layer = LayerMask.NameToLayer( "Obstacle" );
				_obj.transform.eulerAngles = new Vector3( 0, Random.Range(0,360 ), 0 );
				_obj.transform.localScale = new Vector3( Random.Range( 1,10 ), Random.Range( 1,10 ), Random.Range( 1,10 ) );
				Vector3 _pos = new Vector3( Random.Range( -400,400 ), 0, Random.Range( -400,400 ) );

				_pos.y = _obj.transform.localScale.y/2;
				_obj.transform.position = _pos;
				//_obj.AddComponent<ICECreatureLocation>();
				Renderer _renderer = _obj.GetComponent<Renderer>();
				if( _renderer != null )
				{
					_renderer.sharedMaterial = new Material( Shader.Find("Standard") );
					_renderer.sharedMaterial.color = UnityEngine.Random.ColorHSV();


				}
		
				//m_CreatureRegister.AddReference( _obj );
			}

			//m_CreatureRegister.ReorganizeHierarchy();
		}

		private void DrawRegister()
		{
			// CREATURE REGISTER BEGIN
			GUI.backgroundColor = (CheckRegister()?Color.green:Color.yellow);
			EditorGUI.BeginDisabledGroup ( CheckRegister() == true );

			if( ICEEditorLayout.Button( "ADD CREATURE REGISTER", "" , ICEEditorStyle.ButtonExtraLarge ))
			{
				GameObject _register = new GameObject();
				_register.name = "CreatureRegister";
				_register.transform.position = Vector3.zero;
				_register.AddComponent<ICECreatureRegister>();
				m_CreatureRegister = _register.GetComponent<ICECreatureRegister>();

				Selection.activeGameObject = m_CreatureRegister.gameObject;
			}
			EditorGUI.EndDisabledGroup();
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

			if( m_CreatureRegister != null )
			{
				m_CreatureRegister.UseHierarchyManagement = true;
				m_CreatureRegister.ForceHierarchyGroups( false );

				m_CreatureRegister.UseDebug = true;
			}
			// CREATURE REGISTER END
		}

		private void DrawEnvironment()
		{
			// ENVIRONMENT BEGIN
			GUI.backgroundColor = (CheckEnvironment()?Color.green:Color.yellow);
			EditorGUI.BeginDisabledGroup ( CheckEnvironment() == true );

			if( ICEEditorLayout.Button( "ADD ENVIRONMENT CONTROLLER", "" , ICEEditorStyle.ButtonExtraLarge ))
			{
				GameObject _environment = new GameObject();
				_environment.name = "Environment";
				_environment.transform.position = Vector3.zero;
				_environment.AddComponent<ICEEnvironmentController>();
				m_Environment = _environment.GetComponent<ICEEnvironmentController>();

				Selection.activeGameObject = m_Environment.gameObject;
			}
			EditorGUI.EndDisabledGroup();
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

			if( m_Environment != null )
			{
				Light[] _lights = GameObject.FindObjectsOfType<Light>();
				foreach( Light _light in _lights )
				{
					if( _light != null && _light.type == LightType.Directional )
					{
						m_Environment.Sun = _light;
						break;
					}
				}

				if( m_Environment.Sun != null )
				{
					m_Environment.Sun.transform.parent = m_Environment.transform;
					m_Environment.Sun.transform.position = Vector3.zero;
				}
			}
			// ENVIRONMENT END
		}

		private void DrawScene()
		{
			// SCENE BEGIN
			GUI.backgroundColor = (CheckGround()?Color.green:Color.yellow);
			EditorGUI.BeginDisabledGroup ( CheckRegister() == false );
			m_ShowGroundHandling = ICEEditorLayout.ButtonCheck( "DEMO SCENE", "" ,m_ShowGroundHandling, ICEEditorStyle.ButtonExtraLarge );
			EditorGUI.EndDisabledGroup();
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

			if( m_ShowGroundHandling )
			{
				m_ShowCreatureHandling = false;
				m_ShowPlayerHandling = false;
			}

			if( m_ShowGroundHandling && CheckRegister() == true )
			{
				if( LayerMask.NameToLayer( "Terrain" ) != -1 && LayerMask.NameToLayer( "Obstacle" ) != -1 )
					GUI.backgroundColor = Color.green;
				else
					GUI.backgroundColor = Color.yellow;
				
				EditorGUI.BeginDisabledGroup( GUI.backgroundColor == Color.green );
					if( ICEEditorLayout.Button( "Create Terrain and Obstacle Layer", "" , ICEEditorStyle.ButtonExtraLarge ))
					{
						if( ICEEditorTools.AddLayer( "Terrain" ) && ICECreatureRegister.Instance.GroundLayers.Count == 0 )
							ICECreatureRegister.Instance.GroundLayers.Add( "Terrain" );
						if( ICEEditorTools.AddLayer( "Obstacle" ) && ICECreatureRegister.Instance.ObstacleLayers.Count == 0 )
							ICECreatureRegister.Instance.ObstacleLayers.Add( "Obstacle" );
					}
				EditorGUI.EndDisabledGroup();
				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

				if( m_Ground == null )
				{
					GameObject[] _grounds = SystemTools.FindGameObjectsByLayer( LayerMask.NameToLayer( "Terrain" ) );

					if( _grounds != null && _grounds.Length > 0 )
						m_Ground = _grounds[0];
				}

				if( m_Ground != null )
					GUI.backgroundColor = Color.green;
				else
					GUI.backgroundColor = Color.yellow;
				
				EditorGUI.BeginDisabledGroup( GUI.backgroundColor == Color.green );
					ICEEditorLayout.BeginHorizontal();
					if( ICEEditorLayout.Button( "ADD DEMO PLANE", "" , ICEEditorStyle.ButtonExtraLarge ))
					{
						m_Ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
						m_Ground.transform.localScale = new Vector3( 100, 1, 100);
					}
					if( ICEEditorLayout.Button( "ADD DEMO TERRAIN", "" , ICEEditorStyle.ButtonExtraLarge ))
					{
						TerrainData terrainData = new TerrainData(); 
						terrainData.size = new Vector3( 1000, 100, 1000 ); 
						m_Ground = Terrain.CreateTerrainGameObject( terrainData );
						m_Ground.transform.position = new Vector3( -500, 0, -500);
					}
					ICEEditorLayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

				int _size = 70;
				ICEEditorLayout.BeginHorizontal();
				if(  ICEEditorLayout.Button( "LOCATIONS", "" , ICEEditorStyle.ButtonFlex, GUILayout.MinWidth( _size ) ))
					SpawnRandomLocations( m_Ground, 25 );
				if( ICEEditorLayout.Button( "WAYPOINTS", "" , ICEEditorStyle.ButtonFlex, GUILayout.MinWidth( _size ) ))
					SpawnRandomWaypoints( m_Ground, 10 );				
				if( ICEEditorLayout.Button( "ITEMS", "" , ICEEditorStyle.ButtonFlex, GUILayout.MinWidth( _size ) ))
					SpawnRandomItems( m_Ground, 100 );
				if( ICEEditorLayout.Button( "OBSTACLES", "" , ICEEditorStyle.ButtonFlex, GUILayout.MinWidth( _size ) ))
					SpawnRandomObstacles( m_Ground, 100 );
				if( ICEEditorLayout.Button( "WATER", "" , ICEEditorStyle.ButtonFlex, GUILayout.MinWidth( _size ) ))
					SpawnRandomWater( m_Ground, 10 );
				ICEEditorLayout.EndHorizontal();

				EditorSharedTools.DrawGroundCheck( ref m_CreatureRegister.GroundCheck, m_CreatureRegister.GroundLayers );
				EditorSharedTools.DrawObstacleCheck( ref m_CreatureRegister.ObstacleCheck, m_CreatureRegister.ObstacleLayers );

				if( m_Ground != null && m_CreatureRegister.GroundLayers.Count > 0 )
					m_Ground.layer = LayerMask.NameToLayer( m_CreatureRegister.GroundLayers[0] );


			}
			// SCENE END
		}

		private void DrawPlayer()
		{
			// PLAYER BEGIN
			GUI.backgroundColor = (CheckPlayer()?Color.green:Color.yellow);
			EditorGUI.BeginDisabledGroup ( CheckGround() == false );
			m_ShowPlayerHandling = ICEEditorLayout.ButtonCheck( "PLAYER", "" ,m_ShowPlayerHandling, ICEEditorStyle.ButtonExtraLarge );
			EditorGUI.EndDisabledGroup();

			if( m_ShowPlayerHandling )
			{
				m_ShowCreatureHandling = false;
				m_ShowGroundHandling = false;
			}

			if( m_ShowPlayerHandling && CheckRegister() == true && CheckGround() == true )
			{
				GameObject _object = null;
				if( m_CreaturePlayer != null )
					_object = m_CreaturePlayer.gameObject;

				_object = (GameObject)EditorGUILayout.ObjectField( "Player Object", _object, typeof(GameObject), true );

				if( _object != null )
				{
					GameObject _player = _object;

					if( ICEEditorTools.IsPrefab( _player ) )
					{
						_player = GameObject.Instantiate( _object );
						_player.name = _object.name;
						_player.transform.position = new Vector3( 0,0,0 );
					}

					if( _player.GetComponent<ICECreaturePlayer>() == null )
						_player.AddComponent<ICECreaturePlayer>();

					m_CreatureRegister.AddReference( _player );
					m_CreatureRegister.ReorganizeHierarchy();

					m_CreaturePlayer = _player.GetComponent<ICECreaturePlayer>();

					Selection.activeGameObject = m_CreaturePlayer.gameObject;
				}
				/*
				if( ICEEditorLayout.Button( "ADD PLAYER", "" , ICEEditorStyle.ButtonExtraLarge ))
				{
					GameObject _player = new GameObject();
					_player.name = "FPSPlayer";
					_player.transform.position = Vector3.zero;
					_player.AddComponent<ICECreaturePlayer>();
					m_CreaturePlayer = _player.GetComponent<ICECreaturePlayer>();
				}*/

				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

				if( m_CreaturePlayer != null )
				{

				}
			}
			// PLAYER END
		}

		private void DrawCreature()
		{
			// CREATURES BEGIN

			GUI.backgroundColor = (CheckCreature()?Color.green:Color.yellow);
			EditorGUI.BeginDisabledGroup ( CheckGround() == false );
			m_ShowCreatureHandling = ICEEditorLayout.ButtonCheck( "CREATURE", "" ,m_ShowCreatureHandling, ICEEditorStyle.ButtonExtraLarge );
			EditorGUI.EndDisabledGroup();
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

			if( m_ShowCreatureHandling )
			{
				m_ShowGroundHandling = false;
				m_ShowPlayerHandling = false;
			}

			if( m_ShowCreatureHandling && CheckRegister() == true && CheckGround() == true )
			{
				if( m_CreatureRegister != null )
				{
					GameObject _object = null;
					if( m_CreatureControl != null )
						_object = m_CreatureControl.gameObject;

					GameObject _new_object = (GameObject)EditorGUILayout.ObjectField( "Creature Object", _object, typeof(GameObject), true );

					if( _new_object != null && _new_object != _object )
						Selection.activeGameObject = _new_object;

					_object = _new_object;
					
					if( _object != null )
					{
						GameObject _creature = _object;

						if( ICEEditorTools.IsPrefab( _creature ) )
						{
							_creature = GameObject.Instantiate( _object );
							_creature.name = _object.name;
						}

						if( _creature.GetComponent<ICECreatureControl>() == null )
							_creature.AddComponent<ICECreatureControl>();

						m_CreatureRegister.AddReference( _creature );

						m_CreatureControl = _creature.GetComponent<ICECreatureControl>();
						if( m_CreatureControl != null )
						{
							m_CreatureControl.Display.ShowDebug = true;

							m_CreatureSizeMultiplier = ICEEditorLayout.DefaultSlider( "Scale","", m_CreatureSizeMultiplier, 0.25f ,-0.5f,100, 1 );

							m_CreatureControl.gameObject.transform.localScale = new Vector3(m_CreatureSizeMultiplier, m_CreatureSizeMultiplier, m_CreatureSizeMultiplier);

							ICEEditorLayout.BeginHorizontal();
							m_CreatureControl.Creature.Characteristics.TrophicLevel = (CreatureTrophicLevelType)ICEEditorLayout.EnumPopup( "Trophic Level","", m_CreatureControl.Creature.Characteristics.TrophicLevel ); 
							EditorGUI.BeginDisabledGroup( m_CreatureControl.Creature.Characteristics.TrophicLevel == CreatureTrophicLevelType.UNDEFINED );
								m_CreatureControl.Creature.Characteristics.UseAutoDetectInteractors = ICEEditorLayout.ButtonCheck( "INTERACTORS", "Detects and prepares automatically potential interactors", m_CreatureControl.Creature.Characteristics.UseAutoDetectInteractors , ICEEditorStyle.ButtonLarge );
							EditorGUI.EndDisabledGroup();
							ICEEditorLayout.EndHorizontal();
								m_CreatureControl.Creature.Characteristics.MotionControl = MotionControlType.INTERNAL;
								//m_CreatureControl.Creature.Characteristics.MotionControl = (MotionControlType)ICEEditorLayout.EnumPopup("Motion Control","", m_CreatureControl.Creature.Characteristics.MotionControl );
								m_CreatureControl.Creature.Characteristics.GroundOrientation = (GroundOrientationType)ICEEditorLayout.EnumPopup("Body Orientation", "Vertical direction relative to the ground", m_CreatureControl.Creature.Characteristics.GroundOrientation );

							EditorGUILayout.Separator();

							ICEEditorLayout.Label( "Desired Speed", false );
							EditorGUI.indentLevel++;
							m_CreatureControl.Creature.Characteristics.DefaultRunningSpeed = ICEEditorLayout.DefaultSlider( "Running","", m_CreatureControl.Creature.Characteristics.DefaultRunningSpeed, 0.25f ,0,25, 4 );
							m_CreatureControl.Creature.Characteristics.DefaultWalkingSpeed = ICEEditorLayout.DefaultSlider( "Walking","", m_CreatureControl.Creature.Characteristics.DefaultWalkingSpeed, 0.25f ,0,25, 1.5f );
							m_CreatureControl.Creature.Characteristics.DefaultTurningSpeed = ICEEditorLayout.DefaultSlider( "Turning","", m_CreatureControl.Creature.Characteristics.DefaultTurningSpeed, 0.25f ,0,25, 2 );
							EditorGUI.indentLevel--;
							EditorGUILayout.Separator();

							if( ICEEditorTools.HasAnimations( m_CreatureControl.gameObject ) )
							{
								ICEEditorLayout.Label( "Desired Animations", false );
								EditorGUI.indentLevel++;

								ICEEditorLayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup( m_CreatureControl.Creature.Characteristics.IgnoreAnimationIdle == true );
								m_CreatureControl.Creature.Characteristics.AnimationIdle = EditorWizard.WizardAnimationPopup( "Idle", m_CreatureControl, m_CreatureControl.Creature.Characteristics.AnimationIdle );
								EditorGUI.EndDisabledGroup();
								m_CreatureControl.Creature.Characteristics.IgnoreAnimationIdle = ICEEditorLayout.ButtonCheck( "IGNORE", "",m_CreatureControl.Creature.Characteristics.IgnoreAnimationIdle, ICEEditorStyle.ButtonMiddle );
								ICEEditorLayout.EndHorizontal();

								ICEEditorLayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup( m_CreatureControl.Creature.Characteristics.IgnoreAnimationWalk == true );
								m_CreatureControl.Creature.Characteristics.AnimationWalk = EditorWizard.WizardAnimationPopup( "Walk", m_CreatureControl, m_CreatureControl.Creature.Characteristics.AnimationWalk );
								EditorGUI.EndDisabledGroup();
								m_CreatureControl.Creature.Characteristics.IgnoreAnimationWalk = ICEEditorLayout.ButtonCheck( "IGNORE", "",m_CreatureControl.Creature.Characteristics.IgnoreAnimationWalk, ICEEditorStyle.ButtonMiddle );
								ICEEditorLayout.EndHorizontal();

								ICEEditorLayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup( m_CreatureControl.Creature.Characteristics.IgnoreAnimationRun == true );
								m_CreatureControl.Creature.Characteristics.AnimationRun = EditorWizard.WizardAnimationPopup( "Run", m_CreatureControl, m_CreatureControl.Creature.Characteristics.AnimationRun );
								EditorGUI.EndDisabledGroup();
								m_CreatureControl.Creature.Characteristics.IgnoreAnimationRun = ICEEditorLayout.ButtonCheck( "IGNORE", "",m_CreatureControl.Creature.Characteristics.IgnoreAnimationRun, ICEEditorStyle.ButtonMiddle );
								ICEEditorLayout.EndHorizontal();

								ICEEditorLayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup( m_CreatureControl.Creature.Characteristics.IgnoreAnimationAttack == true );
								m_CreatureControl.Creature.Characteristics.AnimationAttack = EditorWizard.WizardAnimationPopup( "Attack", m_CreatureControl, m_CreatureControl.Creature.Characteristics.AnimationAttack );
								EditorGUI.EndDisabledGroup();
								m_CreatureControl.Creature.Characteristics.IgnoreAnimationAttack = ICEEditorLayout.ButtonCheck( "IGNORE", "",m_CreatureControl.Creature.Characteristics.IgnoreAnimationAttack, ICEEditorStyle.ButtonMiddle );
								ICEEditorLayout.EndHorizontal();

								ICEEditorLayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup( m_CreatureControl.Creature.Characteristics.IgnoreAnimationImpact == true );
								m_CreatureControl.Creature.Characteristics.AnimationImpact = EditorWizard.WizardAnimationPopup( "Impact", m_CreatureControl, m_CreatureControl.Creature.Characteristics.AnimationImpact );
								EditorGUI.EndDisabledGroup();
								m_CreatureControl.Creature.Characteristics.IgnoreAnimationImpact = ICEEditorLayout.ButtonCheck( "IGNORE", "",m_CreatureControl.Creature.Characteristics.IgnoreAnimationImpact, ICEEditorStyle.ButtonMiddle );
								ICEEditorLayout.EndHorizontal();

								ICEEditorLayout.BeginHorizontal();
								EditorGUI.BeginDisabledGroup( m_CreatureControl.Creature.Characteristics.IgnoreAnimationDead == true );
								m_CreatureControl.Creature.Characteristics.AnimationDead = EditorWizard.WizardAnimationPopup( "Die", m_CreatureControl, m_CreatureControl.Creature.Characteristics.AnimationDead );
								EditorGUI.EndDisabledGroup();
								m_CreatureControl.Creature.Characteristics.IgnoreAnimationDead = ICEEditorLayout.ButtonCheck( "IGNORE", "",m_CreatureControl.Creature.Characteristics.IgnoreAnimationDead, ICEEditorStyle.ButtonMiddle );
								ICEEditorLayout.EndHorizontal();

								EditorGUI.indentLevel--;
								EditorGUILayout.Separator();
							}
							else
							{
								ICEEditorLayout.BeginHorizontal();
								//GUILayout.FlexibleSpace();
								EditorGUILayout.HelpBox( "No Mecanim or Legacy animations available. Please check your creature!", MessageType.Info );
								//EditorGUILayout.LabelField( "- No Mecanim or Legacy animations available -" );
								//GUILayout.FlexibleSpace();
								ICEEditorLayout.EndHorizontal();
								EditorGUILayout.Separator();
							}

							// GENERATE BEGIN
							EditorGUI.BeginDisabledGroup( Application.isPlaying == true );
							ICEEditorLayout.BeginHorizontal();
							if( GUILayout.Button( "GENERATE", ICEEditorStyle.ButtonExtraLarge ))
							{
								EditorWizard.WizardGenerate( m_CreatureControl );
								Selection.activeGameObject = m_CreatureControl.gameObject;
							}
							ICEEditorLayout.EndHorizontal();
							EditorGUI.EndDisabledGroup();
							// GENERATE END

						}



						//ReferenceGroupObject _group = m_CreatureRegister.GetGroup( _creature );
					}
				}

			}


			// CREATURES END
		}

		/// <summary>
		/// 
		/// </summary>
		void OnGUI()
		{
			ICEEditorLayout.DefaultBackgroundColor = GUI.backgroundColor;

			if( m_ICECCLogo != null )
				GUI.DrawTexture(new Rect(10, 10, m_ICECCLogo.width, m_ICECCLogo.height), m_ICECCLogo);

			GUILayout.BeginArea(new Rect(20, 140, Screen.width - 40, Screen.height - 40));

				DrawRegister();
				DrawScene();
				DrawPlayer();
				DrawCreature();
				DrawEnvironment();

			GUILayout.EndArea();


			GUILayout.BeginArea(new Rect(20, m_DialogSize.y - 20, Screen.width - 40, Screen.height - 40));
				GUI.backgroundColor = Color.clear;
				GUILayout.Label(m_Version + " - " + m_Copyright  + "\n\n", ICEEditorStyle.SmallTextStyle );
			GUILayout.EndArea();
		}
	}
}
