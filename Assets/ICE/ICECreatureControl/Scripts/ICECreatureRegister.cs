// ##############################################################################
//
// ICECreatureRegister.cs
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
using System.Linq;
using ICE;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Utilities.EnumTypes;
using ICE.Utilities;

namespace ICE.Creatures{

	/*
	public static class CreatureRegister
	{
		public static ICECreatureRegister Register{
			get{ return ICECreatureRegister.Instance; }
		}

		public static EnvironmentInfoContainer EnvironmentInfos{
			get{ return GlobalEnvironment.Infos; }
		}
	}*/

	public class ICECreatureRegister : MonoBehaviour 
	{
		public bool UseDebug = false;
		public bool UseDrawSelected = false;
		public Color ColorReferences = Color.red;
		public Color ColorClones = Color.blue;
		public Color ColorSpawnPoints = Color.cyan;
		public bool ShowReferenceGizmos = true;
		public bool ShowCloneGizmos = true;
		public bool ShowSpawnPointGizmos = true;
		public bool ShowReferenceGizmosText = true;
		public bool ShowCloneGizmosText = true;
		public bool ShowSpawnPointGizmosText = true;
		public bool UseDontDestroyOnLoad = false;
		public GroundCheckType GroundCheck = GroundCheckType.NONE;
		public ObstacleCheckType ObstacleCheck = ObstacleCheckType.NONE;


		private LayerMask m_GroundLayerMask = -1;
		public LayerMask GroundLayerMask{
			get{
				m_GroundLayerMask = GraphicTools.GetLayerMask( GroundLayers, m_GroundLayerMask );

				return m_GroundLayerMask;
			}
		}

		private LayerMask m_ObstacleLayerMask = -1;
		public LayerMask ObstacleLayerMask{
			get{
				m_ObstacleLayerMask = GraphicTools.GetLayerMask( ObstacleLayers, m_ObstacleLayerMask );

				return m_ObstacleLayerMask;
			}
		}

		[SerializeField]
		private List<string> m_GroundLayers = new List<string>();
		public List<string> GroundLayers{
			get{ return m_GroundLayers; }
		}

		[SerializeField]
		private List<string> m_ObstacleLayers = new List<string>();
		public List<string> ObstacleLayers{
			get{ return m_ObstacleLayers; }
		}

		public RandomSeedType RandomSeed = RandomSeedType.DEFAULT;
		public int CustomRandomSeed = 23;


		// Network Handling
		public NetworkAdapterType NetworkAdapter = NetworkAdapterType.NONE;
		public bool NetworkReady = false;

		/*
		//public bool UseEnvironmentManagenent = false;
		public EnvironmentInfoContainer EnvironmentInfos{
			get{ return GlobalEnvironment.Infos; }
		}*/

		public bool UsePlayerTagAsGroup = false;

		//Here is a private reference only this class can access
		private static ICECreatureRegister m_Instance = null;
		public static ICECreatureRegister Instance
		{
			get
			{
				//If m_Register hasn't been set yet, we grab it from the scene!
				//This will only happen the first time this reference is used.
				if( m_Instance == null )
					m_Instance = GameObject.FindObjectOfType<ICECreatureRegister>();

				if( m_Instance == null )
				{
					GameObject _register = GameObject.Find( "CreatureRegister" );

					if( _register != null )
					{
						_register.SetActive( true );

						m_Instance = GameObject.FindObjectOfType<ICECreatureRegister>();
					}

				}
			
				/*
				// If there is no valid register in the scene we have to create one!
				if( m_Register == null )
				{
					GameObject _register_object = new GameObject();
					_register_object.name = "ICECreatureRegister.Instance";
					_register_object.transform.position = Vector3.zero;
					m_Register = _register_object.AddComponent<ICECreatureRegister>();
					m_Register.Scan();

				}*/

				return m_Instance;
			}
		}

		void Awake () 
		{
			transform.parent = null;
			transform.gameObject.isStatic = true;

			if( UseSceneManagement )
			{
				if( UseDontDestroyOnLoad ) 
					DontDestroyOnLoad(transform.gameObject);
				
				if( RandomSeed == RandomSeedType.CUSTOM )
					Random.seed = CustomRandomSeed;
				else if( RandomSeed == RandomSeedType.TIME )
					Random.seed = (int)System.DateTime.Now.Second;
			}
		}

		void Start () 
		{
			//if( NetworkAdapter == NetworkAdapterType.NONE )
			InitialSpawn();
		}


		void Update () 
		{
			if( NetworkAdapter != NetworkAdapterType.NONE && ! NetworkReady )
				return;

			foreach( ReferenceGroupObject _group in ReferenceGroupObjects )
			{
				if( _group.PoolManagementEnabled )
					_group.Update();
			}
		}

		public delegate void OnSpawnObjectEvent( out GameObject _object, GameObject _reference, Vector3 _position, Quaternion _rotation );
		public event OnSpawnObjectEvent OnSpawnObject;

		public List<ReferenceGroupObject> ReferenceGroupObjects = new List<ReferenceGroupObject>();
		public bool UseReferenceCategories = false;
		public bool[] CategoriesFoldout = new bool[8];

		/// <summary>
		/// Register the specified _object.
		/// </summary>
		/// <param name="_object">_object.</param>
		public ReferenceGroupObject Register( GameObject _object )
		{
			if( _object == null )
				return null;

			ReferenceGroupObject _group = GetGroup( _object );
			if( _group != null )
				_group.Register( _object );

			return _group;
		}
		
		/// <summary>
		/// Deregister the specified _object.
		/// </summary>
		/// <param name="_object">_object.</param>
		public bool Deregister( GameObject _object )
		{
			if( _object == null )
				return false;

			ReferenceGroupObject _group = GetGroup( _object );			
			if( _group != null )
				return _group.Deregister( _object );
			else 
				return false;
		}

		public bool Cleanup( GameObject _object )
		{
			if( _object == null )
				return false;
			
			ReferenceGroupObject _group = GetGroup( _object );			
			if( _group != null )
				return _group.Cleanup( _object );
			else 
				return false;
		}

		private static string CleanName( string _name )
		{
			if( _name == null || _name == "" )
				return "";
			
			return _name.Replace("(Clone)", ""); 
		}

		public ReferenceGroupObject GetGroup( GameObject _object )
		{
			foreach( ReferenceGroupObject _group in ReferenceGroupObjects )
			{
				if( _group.Compare( _object ) )
					return _group;
			}

			return null;
		}

		public ReferenceGroupObject GetGroupByName( string _name )
		{
			if( _name == "" )
				return null;
			
			foreach( ReferenceGroupObject _group in ReferenceGroupObjects ){
				if( _group.CompareByName( _name ) )
					return _group;
			}
			
			return null;
		}

		public ReferenceGroupObject GetGroupByTag( string _tag )
		{
			if( _tag == "" )
				return null;
			
			foreach( ReferenceGroupObject _group in ReferenceGroupObjects ){
				if( _group.CompareByTag( _tag ) )
					return _group;
			}
			
			return null;
		}

		//################################################################################
		// RANDOM RUNTIME TARGETS
		//################################################################################

		/// <summary>
		/// Gets the random name of the target by.
		/// </summary>
		/// <description>GetRandomTargetByTag will be called by a ReferenceGroupObject inside GetSpawnPosition()</description>
		/// <returns>The random target by name.</returns>
		/// <param name="_name">Name.</param>
		public GameObject GetRandomTargetByName( string _name )
		{
			GameObject _target_game_object = null;

			ReferenceGroupObject _group = GetGroupByName( _name );
			if( _group != null )
				_target_game_object = _group.GetRandomObject();

			// BACKUP SEARCH
			if( _target_game_object == null && _name != "" )
			{
				_target_game_object = GraphicTools.GetRandomObjectByName( _name );
				if( _target_game_object == null )
					_target_game_object = GraphicTools.GetRandomObjectByName( _name + "(Clone)" );
			}

			return _target_game_object;
		}

		/// <summary>
		/// Gets the random target by tag.
		/// </summary>
		/// <description>GetRandomTargetByTag will be called by a ReferenceGroupObject inside GetSpawnPosition()</description>
		/// <returns>The random target by tag.</returns>
		/// <param name="_tag">Tag.</param>
		public GameObject GetRandomTargetByTag( string _tag )
		{
			GameObject _target_game_object = null;

			ReferenceGroupObject _group = GetGroupByTag( _tag );
			if( _group != null )
				_target_game_object = _group.GetRandomObject();


			// BACKUP SEARCH
			if( _target_game_object == null && _tag != "" )
				_target_game_object = GraphicTools.GetRandomObjectByTag( _tag );

			return _target_game_object;
		}

		//################################################################################
		// NEAREST RUNTIME TARGETS
		//################################################################################

		/// <summary>
		/// Finds the nearest target by name.
		/// </summary>
		/// <description>FindNearestTargetByName will be called directly from a target to update the TargetGameObject</description>
		/// <returns>The nearest target by name.</returns>
		/// <param name="_name">Name.</param>
		/// <param name="_position">Position.</param>
		/// <param name="_distance">Distance.</param>
		/// <param name="_sender">Sender.</param>
		public GameObject FindNearestTargetByName( GameObject _sender, string _name, float _distance )
		{
			if( _sender == null )
				return null;
			
			GameObject _target_game_object = null;
			ReferenceGroupObject _group = GetGroupByName( _name );
			if( _group != null )
				_target_game_object = _group.FindNearestObject( _sender , _distance );

			return _target_game_object;
		}

		/// <summary>
		/// Finds the nearest target by tag.
		/// </summary>
		/// <description>FindNearestTargetByTag will be called directly from a target to update the TargetGameObject</description>
		/// <returns>The nearest target by tag.</returns>
		/// <param name="_tag">Tag.</param>
		/// <param name="_position">Position.</param>
		/// <param name="_distance">Distance.</param>
		/// <param name="_sender">Sender.</param>
		public GameObject FindNearestTargetByTag( GameObject _sender, string _tag, float _distance )
		{
			if( _sender == null )
				return null;
			
			GameObject _target_game_object = null;
			ReferenceGroupObject _group = GetGroupByTag( _tag );
			if( _group != null )
				_target_game_object = _group.FindNearestObject( _sender, _distance );

			return _target_game_object;
		}

		//################################################################################
		// REFERENCE GAME OBJECTS
		//################################################################################

		/// <summary>
		/// Gets all the reference item names.
		/// </summary>
		/// <description>ReferenceItemNames will be used only to fill the item selection popup</description>
		/// <value>The reference item names.</value>
		public List<string> ReferenceItemNames
		{
			get{
				List<string> _keys = new List<string>();

				foreach( ReferenceGroupObject _group in ReferenceGroupObjects )
				{
					if( _group != null && _group.Reference != null && _group.Reference.GetComponent<ICECreatureItem>() != null )
						_keys.Add( _group.Name );
				}

				return _keys;
			}
		}

		/// <summary>
		/// Gets all reference GameObject names.
		/// </summary>
		/// <description>ReferenceGameObjectNames will be used only to fill the target selection popup</description>
		/// <value>The reference GameObject names.</value>
		public List<string> ReferenceGameObjectNames
		{
			get{
				List<string> _keys = new List<string>();
				List<string> _player_keys = new List<string>();
				List<string> _creature_keys = new List<string>();
				List<string> _item_keys = new List<string>();
				List<string> _location_keys = new List<string>();
				List<string> _waypoint_keys = new List<string>();
				List<string> _other_keys = new List<string>();

				foreach( ReferenceGroupObject _group in ReferenceGroupObjects )
				{
					if( _group.Reference == null )
						continue;

					if( _group.Reference.GetComponent<ICECreatureControl>() != null )
						_creature_keys.Add( _group.Name );
					else if( _group.Reference.GetComponent<ICECreaturePlayer>() != null )
						_player_keys.Add( _group.Name );
					else if( _group.Reference.GetComponent<ICECreatureItem>() != null )
						_item_keys.Add( _group.Name );
					else if( _group.Reference.GetComponent<ICECreatureLocation>() != null )
						_location_keys.Add( _group.Name );
					else if( _group.Reference.GetComponent<ICECreatureWaypoint>() != null )
						_waypoint_keys.Add( _group.Name );
					else 
						_other_keys.Add( _group.Name );
				}

				_keys.Add( " " );

				if( _player_keys.Count > 0 )
				{
					foreach( string _key in _player_keys )
						_keys.Add( _key );
				}

				if( _creature_keys.Count > 0 )
				{
					if( _keys.Count > 0 ) 
						_keys.Add( " " );

					foreach( string _key in _creature_keys )
						_keys.Add( _key );
				}

				if( _item_keys.Count > 0 )
				{
					if( _keys.Count > 0 ) 
						_keys.Add( " " );

					foreach( string _key in _item_keys )
						_keys.Add( _key );
				}

				if( _location_keys.Count > 0 )
				{
					if( _keys.Count > 0 ) 
						_keys.Add( " " );

					foreach( string _key in _location_keys )
						_keys.Add( _key );
				}

				if( _waypoint_keys.Count > 0 )
				{
					if( _keys.Count > 0 ) 
						_keys.Add( " " );

					foreach( string _key in _waypoint_keys )
						_keys.Add( _key );
				}

				if( _other_keys.Count > 0 )
				{
					if( _keys.Count > 0 ) 
						_keys.Add( " " );

					foreach( string _key in _other_keys )
						_keys.Add( _key );
				}

				return _keys;

			}

		}

		/// <summary>
		/// Gets all listed reference GameObjects.
		/// </summary>
		/// <value>All reference targets.</value>
		public List<GameObject> ReferenceGameObjects{
			get{
				List<GameObject> _targets = new List<GameObject>();

				foreach( ReferenceGroupObject _group in ReferenceGroupObjects )
					_targets.Add( _group.Reference );
				
				return _targets;
			}

		}

		/// <summary>
		/// Gets the reference game object by tag.
		/// </summary>
		/// <returns>The reference game object by tag.</returns>
		/// <param name="_tag">Tag.</param>
		public GameObject GetReferenceGameObjectByTag( string _tag )
		{
			if( _tag.Trim() == "" )
				return null;

			GameObject _object = null;

			foreach( GameObject _reference_object in ReferenceGameObjects )
			{
				if( _reference_object != null && _reference_object.tag == _tag )
				{
					_object = _reference_object;
					break;
				}
			}

			// BACKUP SEARCH
			if( _object == null )
			{
				_object = GameObject.FindWithTag( _tag );

				// add as reference while its not listed
				AddReference( _object );
			}

			return _object;
		}

		/// <summary>
		/// Gets the reference game object by name.
		/// </summary>
		/// <returns>The reference game object by name.</returns>
		/// <param name="_name">Name.</param>
		public GameObject GetReferenceGameObjectByName( string _name )
		{
			if( _name.Trim() == "" )
				return null;
			
			GameObject _object = null;

			foreach( GameObject _reference_object in ReferenceGameObjects )
			{
				if( _reference_object != null && _reference_object.name == _name )
				{
					_object = _reference_object;
					break;
				}
			}

			if( _object == null )
			{
				_object = GameObject.Find( _name );

				// add as reference while its not listed
				AddReference( _object );
			}

			return _object;
		}


		/// <summary>
		/// Gets a list with all active GameObjects of the named group.
		/// </summary>
		/// <returns>All active GameObjects of the named group</returns>
		/// <param name="_name">Name.</param>
		public List<GameObject> GetActiveGroupItemsByName( string _name )
		{
			List<GameObject> _objects = new List<GameObject>();
			
			ReferenceGroupObject _group = GetGroupByName( _name );
			if( _group != null )
			{
				foreach( GameObject _object in _group.Items )
					_objects.Add( _object );

				if( _objects.Count == 0 && _group.Reference != null && _group.Reference.activeInHierarchy )
					_objects.Add( _group.Reference ); 
			}

			return _objects;
		}

		/// <summary>
		/// Gets a list with all active GameObjects of the tagged group.
		/// </summary>
		/// <returns>All active GameObjects of the tagged group</returns>
		/// <param name="_name">Name.</param>
		public List<GameObject> GetActiveGroupItemsByTag( string _tag )
		{
			List<GameObject> _objects = new List<GameObject>();
			
			ReferenceGroupObject _group = GetGroupByTag( _tag );
			if( _group != null )
			{
				foreach( GameObject _object in _group.Items )
					_objects.Add( _object );

				if( _objects.Count == 0 && _group.Reference != null && _group.Reference.activeInHierarchy )
					_objects.Add( _group.Reference ); 
			}
			
			return _objects;
		}


		//################################################################################
		// SPAWN HANDLER
		//################################################################################

		public void InitialSpawn()
		{
			List<ReferenceGroupObject> _groups = ReferenceGroupObjects.OrderByDescending( _item => _item.InitialSpawnPriority ).ToList();

			for( int _i = 0 ; _i < _groups.Count; _i++ )
			{
				ReferenceGroupObject _group = _groups[_i];
				if( _group != null && _group.PoolManagementEnabled && _group.UseInitialSpawn )
					_group.SpawnAll();
			}
		}

		public GameObject SpawnObject( GameObject _reference, Vector3 _position, Quaternion _rotation )
		{
			if( _reference == null )
				return null;
			
			GameObject _object = null;
			if( OnSpawnObject != null )
				OnSpawnObject( out _object, _reference, _position, _rotation );
			else
				_object = (GameObject)Object.Instantiate( _reference, _position, _rotation );

			return _object;
		}

		//################################################################################
		// HIERARCHY MANAGEMENT
		//################################################################################

		public bool UseHierarchyManagement = true;
		public bool UsePoolManagement = true;
		public bool UseSceneManagement = true;


		public bool IgnoreRootGroup = false;
		public Transform RootGroup = null;
		public Transform GetRootGroup()
		{
			if( UseHierarchyManagement && ! IgnoreRootGroup )
			{
				if( RootGroup == null )
					RootGroup = transform;

				return RootGroup;
			}
			else
				return transform.root;
		}

		public bool IgnoreCreatureGroup = false;
		public Transform CreatureGroup = null;
		public bool IgnoreItemGroup = false;
		public Transform ItemGroup  = null;
		public bool IgnorePlayerGroup = false;
		public Transform PlayerGroup  = null;
		public bool IgnoreLocationGroup = false;
		public Transform LocationGroup  = null;
		public bool IgnoreWaypointGroup = false;
		public Transform WaypointGroup  = null;
		public bool IgnoreMarkerGroup = false;
		public Transform MarkerGroup  = null;
		public bool IgnoreOtherGroup = false;
		public Transform OtherGroup  = null;

		public Transform GetHierarchyGroupTransformByType( HierarchyGroupType _type )
		{
			switch( _type )
			{
			case HierarchyGroupType.Creatures:
				return CreatureGroup;
			case HierarchyGroupType.Items:
				return ItemGroup;
			case HierarchyGroupType.Players:
				return PlayerGroup;
			case HierarchyGroupType.Locations:
				return LocationGroup;
			case HierarchyGroupType.Waypoints:
				return WaypointGroup;
			case HierarchyGroupType.Markers:
				return MarkerGroup;
			default:
				return OtherGroup;
			}
		}

		public bool GetHierarchyGroupIgnoreByType( HierarchyGroupType _type )
		{
			switch( _type )
			{
				case HierarchyGroupType.Creatures:
					return IgnoreCreatureGroup;
				case HierarchyGroupType.Items:
					return IgnoreItemGroup;
				case HierarchyGroupType.Players:
					return IgnorePlayerGroup;
				case HierarchyGroupType.Locations:
					return IgnoreLocationGroup;
				case HierarchyGroupType.Waypoints:
					return IgnoreWaypointGroup;
				case HierarchyGroupType.Markers:
					return IgnoreMarkerGroup;
				default:
					return IgnoreOtherGroup;
			}

		}

		public Transform GetHierarchyGroup( HierarchyGroupType _type )
		{
			Transform _group = GetHierarchyGroupTransformByType( _type );
			bool _ignore = GetHierarchyGroupIgnoreByType( _type );
			string _name = _type.ToString();

			if( UseHierarchyManagement )
			{
				if( ! _ignore )
				{
					if( _group == null )
					{
						GameObject _obj = GameObject.Find( _name );
						if( _obj == null )
						{
							_group = new GameObject().transform;
							_group.parent = GetRootGroup();
							_group.name = _name;
							_group.position = Vector3.zero;
						}
						else
							_group = _obj.transform;
					}
					else if( IgnoreRootGroup && _group.parent != transform.root.parent )
						_group.parent = transform.root.parent;
					else if( ! IgnoreRootGroup && _group.parent != GetRootGroup() )
						_group.parent = GetRootGroup();

					return _group;
				}
				else
				{
					return GetRootGroup();
				}
			}
			else
				return transform.root;
		}

		public void ForceHierarchyGroups( bool _ignore )
		{
			if( RootGroup == null )
			{
				RootGroup = GetRootGroup();
				IgnoreRootGroup = _ignore;
			}

			if( PlayerGroup == null )
			{
				PlayerGroup = GetHierarchyGroup( HierarchyGroupType.Players );
				IgnorePlayerGroup = _ignore;
			}

			if( CreatureGroup == null )
			{
				CreatureGroup = GetHierarchyGroup( HierarchyGroupType.Creatures );
				IgnoreCreatureGroup = _ignore;
			}

			if( ItemGroup == null )
			{
				ItemGroup = GetHierarchyGroup( HierarchyGroupType.Items );
				IgnoreItemGroup = _ignore;
			}

			if( LocationGroup == null )
			{
				LocationGroup = GetHierarchyGroup( HierarchyGroupType.Locations );
				IgnoreLocationGroup = _ignore;
			}

			if( WaypointGroup == null )
			{
				WaypointGroup = GetHierarchyGroup( HierarchyGroupType.Waypoints );
				IgnoreWaypointGroup = _ignore;
			}

			if( MarkerGroup == null )
			{
				MarkerGroup = GetHierarchyGroup( HierarchyGroupType.Markers );
				IgnoreMarkerGroup = _ignore;
			}

			if( OtherGroup == null )
			{
				OtherGroup = GetHierarchyGroup( HierarchyGroupType.Other );
				IgnoreOtherGroup = _ignore;
			}
		}

		public void ReorganizeHierarchy()
		{
			if( ! IgnoreCreatureGroup )
			{
				CreatureGroup = GetHierarchyGroup( HierarchyGroupType.Creatures );
				ICECreatureControl[] _creatures = FindObjectsOfType<ICECreatureControl>();
				foreach( ICECreatureControl _creature in _creatures )
					_creature.transform.parent = CreatureGroup;
			}

			if( ! IgnorePlayerGroup )
			{
				PlayerGroup = GetHierarchyGroup( HierarchyGroupType.Players );
				ICECreaturePlayer[] _players = FindObjectsOfType<ICECreaturePlayer>();
				foreach( ICECreaturePlayer _player in _players )
					_player.transform.parent = PlayerGroup;
			}

			if( ! IgnoreItemGroup )
			{
				ItemGroup = GetHierarchyGroup( HierarchyGroupType.Items );
				ICECreatureItem[] _items = FindObjectsOfType<ICECreatureItem>();
				foreach( ICECreatureItem _item in _items )
					_item.transform.parent = ItemGroup;
			}

			if( ! IgnoreLocationGroup )
			{
				LocationGroup = GetHierarchyGroup( HierarchyGroupType.Locations );
				ICECreatureLocation[] _locations = FindObjectsOfType<ICECreatureLocation>();
				foreach( ICECreatureLocation _location in _locations )
					_location.transform.parent = LocationGroup;
			}

			if( ! IgnoreWaypointGroup )
			{
				WaypointGroup = GetHierarchyGroup( HierarchyGroupType.Waypoints );
				ICECreatureWaypoint[] _waypoints = FindObjectsOfType<ICECreatureWaypoint>();
				foreach( ICECreatureWaypoint _waypoint in _waypoints )
					_waypoint.transform.parent = WaypointGroup;
			}

			if( ! IgnoreMarkerGroup )
			{
				MarkerGroup = GetHierarchyGroup( HierarchyGroupType.Markers );
				ICECreatureMarker[] _markers = FindObjectsOfType<ICECreatureMarker>();
				foreach( ICECreatureMarker _marker in _markers )
					_marker.transform.parent = MarkerGroup;
			}

			if( ! IgnoreOtherGroup )
			{
				OtherGroup = GetHierarchyGroup( HierarchyGroupType.Other );

			}
		}


		//################################################################################
		// REFERENCE MANAGEMENT
		//################################################################################

		public void UpdateAllReferences()
		{
			UpdateReferencePlayer();
			UpdateReferenceLocations();
			UpdateReferenceWaypoints();
			UpdateReferenceItems();
			UpdateReferenceCreature();
			UpdateReferenceMarker();
		}

		public void UpdateReferencePlayer()
		{
			ICECreaturePlayer[] _players = FindObjectsOfType<ICECreaturePlayer>();			
			foreach( ICECreaturePlayer _player in _players )
			{
				if( _player != null )
					AddReference( _player.gameObject );
			}
		}

		public void UpdateReferenceLocations()
		{
			ICECreatureLocation[] _locations = FindObjectsOfType<ICECreatureLocation>();			
			foreach( ICECreatureLocation _location in _locations )
			{
				if( _location != null )
					AddReference( _location.gameObject );
			}
		}

		public void UpdateReferenceWaypoints()
		{
			ICECreatureWaypoint[] _waypoints = FindObjectsOfType<ICECreatureWaypoint>();			
			foreach( ICECreatureWaypoint _waypoint in _waypoints )
			{
				if( _waypoint != null )
					AddReference( _waypoint.gameObject );
			}
		}

		public void UpdateReferenceMarker()
		{
			ICECreatureMarker[] _markers = FindObjectsOfType<ICECreatureMarker>();			
			foreach( ICECreatureMarker _marker in _markers )
			{
				if( _marker != null )
					AddReference( _marker.gameObject );
			}
		}
		
		public void UpdateReferenceItems()
		{
			ICECreatureItem[] _items = FindObjectsOfType<ICECreatureItem>();			
			foreach( ICECreatureItem _item in _items )
			{
				if( _item != null )
					AddReference( _item.gameObject );
			}
		}

		public void UpdateReferenceCreature()
		{
			ICECreatureControl[] _creatures = FindObjectsOfType<ICECreatureControl>();
			foreach( ICECreatureControl _creature in _creatures )
			{
				if( _creature != null )
					AddReference( _creature.gameObject );
			}
		}
			
		public bool AddReference( GameObject _object )
		{
			if( _object == null )
				return false;

			if( IsListedAsReference( _object ) == RegisterReferenceType.NONE )
			{
				ReferenceGroupObjects.Add( new ReferenceGroupObject( _object ) );	
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// Determines whether the specified object is already registered.
		/// </summary>
		/// <returns><c>RegisterReferenceType</c> if this instance is registered the specified _object; otherwise, <c>RegisterReferenceType.NONE</c>.</returns>
		/// <param name="_object">_object.</param>
		public RegisterReferenceType IsListedAsReference( GameObject _object )
		{
			if( _object == null )
				return RegisterReferenceType.ERROR;

			return IsListedAsReferenceByName( _object.name );
		}


		/// <summary>
		/// Determines whether this instance is registered the specified _object_name.
		/// </summary>
		/// <returns><c>RegisterReferenceType</c> if this instance is registered the specified _object_name; otherwise, <c>RegisterReferenceType.NONE</c>.</returns>
		/// <param name="_object_name">_object_name.</param>
		public RegisterReferenceType IsListedAsReferenceByName( string _name )
		{
			if( _name.Length == 0 )
				return RegisterReferenceType.ERROR;

			_name = CleanName( _name );

			RegisterReferenceType _registered = RegisterReferenceType.NONE;

			foreach( ReferenceGroupObject _group in ReferenceGroupObjects )
			{
				if( _group.Reference == null || _group.Reference.name != _name )
					continue;

				if( _group.Reference.GetComponent<ICECreatureControl>() != null )
					_registered = RegisterReferenceType.CREATURE;
				else if( _group.Reference.GetComponent<ICECreaturePlayer>() != null )
					_registered = RegisterReferenceType.PLAYER;
				else if( _group.Reference.GetComponent<ICECreatureItem>() != null )
					_registered = RegisterReferenceType.ITEM;
				else if( _group.Reference.GetComponent<ICECreatureLocation>() != null )
					_registered = RegisterReferenceType.LOCATION;
				else if( _group.Reference.GetComponent<ICECreatureWaypoint>() != null )
					_registered = RegisterReferenceType.WAYPOINT;
				else if( _group.Reference.GetComponent<ICECreatureMarker>() != null )
					_registered = RegisterReferenceType.MARKER;
				else
					_registered = RegisterReferenceType.UNDEFINED;
			}

			return _registered;
		}



		/*
		public GameObject GetReferenceCreatureByName( string _object_name )
		{
			GameObject _creature = null;
			
			foreach( CreatureReferenceObject _item in ReferenceCreatures )
			{
				if( _item.Reference != null )
				{
					if( _object_name.Length == 0 || _item.Reference.name == _object_name )
						_creature = _item.Reference;
				}
				
			}
			
			return _creature;
		}


		public List<GameObject> GetCreaturesByKey( string _key )
		{
			CreatureRegisterGroupObject _group = ICECreatureRegister.Instance.GetCreatureGroup( _key );
			
			if( _group != null && _group.Records.Count > 0 )
				return _group.Records;
			else
			{
				List<GameObject> _creatures = new List<GameObject>();

				foreach( CreatureReferenceObject _item in ReferenceCreatures )
				{
					if( _item.Reference != null )
					{
						if( _key.Length == 0 || _item.Reference.name == _key )
							_creatures.Add( _item.Reference );
					}
				}

				return _creatures;
			}
		}*/

		public int TotalCreatureCount{
			get{ return 0;}
		}

		public int GroupCount{
			get{ return 0;}
		}

		/*
		public void UpdateWeather( WeatherType _weather )
		{
			EnvironmentInfos.Weather = _weather;
		}

		public void UpdateTemperature( float _temperature )
		{
			EnvironmentInfos.Temperature = _temperature;
		}

		public void UpdateTime( int _hour, int _minutes, int _seconds )
		{
			EnvironmentInfos.TimeHour = _hour;
			EnvironmentInfos.TimeMinutes = _minutes;
			EnvironmentInfos.TimeSeconds = _seconds;
		}

		public void UpdateDate( int _day, int _month, int _year )
		{
			EnvironmentInfos.DateDay = _day;
			EnvironmentInfos.DateMonth = _month;
			EnvironmentInfos.DateYear = _year;
		}*/


	}

}
