// ##############################################################################
//
// ice_CreatureRegisterReferences.cs
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
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Utilities;

namespace ICE.Creatures.Objects
{
	public enum BroadcastMessageType
	{
		NONE,
		COMMAND
	}

	[System.Serializable]
	public class BroadcastMessageDataObject
	{
		public BroadcastMessageDataObject(){}
		public BroadcastMessageDataObject( BroadcastMessageDataObject _object ){
			Copy( _object );
		}

		public BroadcastMessageType Type;
		[XmlIgnore]
		public GameObject TargetGameObject;
		public string BahaviourKey;
		public string Command;

		public void Copy( BroadcastMessageDataObject _object )
		{
			Type = _object.Type;
			Command = _object.Command;
			TargetGameObject = _object.TargetGameObject;
			BahaviourKey = _object.BahaviourKey;
		}

	}

	[System.Serializable]
	public class BroadcastMessageObject : BroadcastMessageDataObject
	{
		public BroadcastMessageObject(){}
		public BroadcastMessageObject( BroadcastMessageObject _object ){

			Foldout = _object.Foldout;
			Enabled = _object.Enabled;
			Copy( _object as BroadcastMessageDataObject );
		}

		public bool Foldout = false;
		public bool Enabled = false;
	}

	[System.Serializable]
	public struct ReferenceStatusContainer
	{
		public bool HasCreatureController;
		public bool HasCreatureAdapter;
		public bool HasHome;
		public bool HasMissionOutpost;
		public bool HasMissionEscort;
		public bool HasMissionPatrol;
		public bool isPrefab;
		public bool isActiveAndEnabled;
		public bool isActiveInHierarchy;
	}

	[System.Serializable]
	public abstract class ReferenceObject
	{
		public ReferenceObject(){}
		public ReferenceObject( GameObject _object ){
			Reference = _object;
		}

		public ICECreatureRegister RegisterInstance{
			get{ return ICECreatureRegister.Instance; }
		}

		public ICECreatureController CreatureController{
			get{ return ( Reference != null )?Reference.GetComponent<ICECreatureController>():null; }
		}

		public ICECreatureItem CreatureItem{
			get{ return ( Reference != null )?Reference.GetComponent<ICECreatureItem>():null; }
		}

		public ICECreaturePlayer CreaturePlayer{
			get{ return ( Reference != null )?Reference.GetComponent<ICECreaturePlayer>():null; }
		}

		public ICECreatureLocation CreatureLocation{
			get{ return ( Reference != null )?Reference.GetComponent<ICECreatureLocation>():null; }
		}

		public ICECreatureWaypoint CreatureWaypoint{
			get{ return ( Reference != null )?Reference.GetComponent<ICECreatureWaypoint>():null; }
		}

		public ICECreatureMarker CreatureMarker{
			get{ return ( Reference != null )?Reference.GetComponent<ICECreatureMarker>():null; }
		}

		public ReferenceStatusContainer Status;
		public bool Foldout = true;
		public bool Enabled = true;
		public bool GroupByTag = false;

		[SerializeField]
		public GameObject Reference = null;

		public string Key{
			get{ 
				if( Reference != null )
				{
					if( GroupByTag )
						return Reference.tag;
					else
						return Reference.name;
				}
				else
					return "";
			}
		}

		public string Name{
			get{ 
				if( Reference != null )
					return Reference.name;
				else
					return "";
			}
		}

		public string Tag{
			get{ 
				if( Reference != null )
					return Reference.tag;
				else
					return "";
			}
		}

		public int ID{
			get{ 
				if( Reference != null )
					return Reference.GetInstanceID();
				else
					return 0;
			}
		}

		public bool Compare( GameObject _object )
		{
			if( _object == null || Reference == null )
				return false;

			if( ( GroupByTag && Reference.CompareTag( _object.tag ) ) || Reference.name == CleanName( _object.name ) )
				return true;
			else
				return false;
		}

		public bool CompareByName( string _name )
		{
			if( _name == "" || Reference == null )
				return false;

			if( Reference.name == CleanName( _name ) )
				return true;
			else
				return false;
		}

		public bool CompareByTag( string _tag )
		{
			if( _tag == "" || Reference == null )
				return false;
			
			if( Reference.CompareTag( _tag ) )
				return true;
			else
				return false;
		}

		private static string CleanName( string _name )
		{
			if( _name == null || _name == "" )
				return "";
			
			return _name.Replace("(Clone)", ""); 
		}
	}

	[System.Serializable]
	public class SpawnPointObject
	{
		public SpawnPointObject(){}
		public SpawnPointObject( TargetObject _target )
		{
			AccessType = _target.AccessType;
			SpawnPointGameObject = _target.TargetGameObject;
			SpawnPointName = _target.TargetName;
			SpawnPointTag = _target.TargetTag;

			if( _target.Move.RandomRange > 0 )
			{
				MinSpawningRange = _target.Move.StopDistance;
				MaxSpawningRange = _target.Move.RandomRange;
			}
		}
		public SpawnPointObject( GameObject _object )
		{
			AccessType = TargetAccessType.OBJECT;
			SpawnPointGameObject = _object;
			SpawnPointName = _object.name;
			SpawnPointTag = _object.tag;

			MinSpawningRange = 0;
			MaxSpawningRange = SpawningRangeMax;
		}

		public TargetAccessType AccessType = TargetAccessType.OBJECT;
		public bool IsPrefab = false;
		public bool IsValid{
			get{
				if( SpawnPointGameObject != null )
					return true;
				else
					return false;
			}
		}

		public GameObject SpawnPointGameObject = null;
		public string SpawnPointName = "";
		public string SpawnPointTag = "Untagged";

		public float MinSpawningRange = 0;
		public float MaxSpawningRange = 25;
		public float SpawningRangeMax = 250;
	}

	[System.Serializable]
	public class ReferenceGroupObject : ReferenceObject
	{
		public ReferenceGroupObject(){}
		public ReferenceGroupObject( GameObject _object ) : base( _object ){}

		public bool CanSpawn{
			get{
				if( PoolManagementEnabled && (
					CreatureController != null || 
					CreatureItem != null ||
					CreatureLocation != null ||
					CreatureWaypoint != null ||
					CreatureMarker != null ) )
					return true;
				else
					return false;
			}
		}

		public HierarchyGroupType GroupType{
			get{
				if( CreatureController != null )
					return HierarchyGroupType.Creatures;
				else if( CreatureItem != null )
					return HierarchyGroupType.Items;
				else if( CreatureLocation != null )
					return HierarchyGroupType.Locations;
				else if( CreatureWaypoint != null )
					return HierarchyGroupType.Waypoints;
				else if( CreatureMarker != null )
					return HierarchyGroupType.Markers;
				else if( CreaturePlayer != null )
					return HierarchyGroupType.Players;
				else
					return HierarchyGroupType.Other;
			}
		}

		private List<GameObject> m_Items = null;
		public List<GameObject> Items{
			get{ 
				if( m_Items == null )
					m_Items = new List<GameObject>();

				return m_Items; 			
			}
		}

		private List<GameObject> m_SuspendedItems = null;
		public List<GameObject> SuspendedItems{
			get{ 
				if( m_SuspendedItems == null )
					m_SuspendedItems = new List<GameObject>();
				
				return m_SuspendedItems; 			
			}
		}

		[SerializeField]
		private List<SpawnPointObject> m_SpawnPoints = new List<SpawnPointObject>();
		public List<SpawnPointObject> SpawnPoints{
			get{			
				if( m_SpawnPoints == null )
					m_SpawnPoints = new List<SpawnPointObject>();

				return m_SpawnPoints;
			}
			set{m_SpawnPoints = value;}
		}

		public bool Register( GameObject _object )
		{
			bool _added = false;
			if( ! IsRegistered( _object ) )
			{
				Items.Add( _object );
				_added = true;

				if( m_GroupObject == null )
					Reorganize();

				if( UseHierarchyGroupObject && m_GroupObject != null && _object.transform.parent != m_GroupObject.transform )
					_object.transform.parent = m_GroupObject.transform;

				if( _object.GetComponent<ICECreatureControl>() != null )
					OnGroupMessage += _object.GetComponent<ICECreatureControl>().Creature.ReceiveGroupMessage;
			}
			return _added;
		}
		
		public bool Deregister( GameObject _object )
		{
			if( _object == null )
				return false;

			if( _object.GetComponent<ICECreatureControl>() != null )
				OnGroupMessage -= _object.GetComponent<ICECreatureControl>().Creature.ReceiveGroupMessage;

			SuspendedItems.Remove( _object );
			return Items.Remove( _object );
		}

		public bool Cleanup( GameObject _object )
		{
			if( _object == null )
				return false;
			
			if( PoolManagementEnabled && UseSoftRespawn )
			{
				_object.SetActive( false );
				SuspendedItems.Add( _object );
			}
			else
			{
				DestroyItem( _object );
			}

			return true;
		}

		// Event Handler
		public delegate void OnGroupMessageEvent( ReferenceGroupObject _group, GameObject _sender, BroadcastMessageDataObject _msg  );
		public event OnGroupMessageEvent OnGroupMessage;

		public void Message( GameObject _sender, BroadcastMessageDataObject _msg )
		{
			if( _sender == null )
				return;

			if( OnGroupMessage != null )
				OnGroupMessage( this, _sender, _msg );

		}

		public void DestroyItem( GameObject _object )
		{
			if( _object == null )
				return;
			
			SuspendedItems.Remove( _object );
			Items.Remove( _object );
			GameObject.Destroy( _object );
		}
		
		public void DestroyItems()
		{
			while( Items.Count > 0 )
				DestroyItem( Items[0] );
		}

		public bool IsRegistered( GameObject _object )
		{
			int _id = _object.GetInstanceID();
			foreach( GameObject _item in Items )
			{
				if( _item.GetInstanceID() == _id )
					return true;
			}
			
			return false;
		}

		public bool IsSuspended( GameObject _object )
		{
			int _id = _object.GetInstanceID();
			foreach( GameObject _item in SuspendedItems )
			{
				if( _item.GetInstanceID() == _id )
					return true;
			}
			
			return false;
		}

		public bool PoolManagementEnabled = false;
		public bool UseSoftRespawn = true;	

		public int InitialSpawnPriority = 0;
		public int MaxObjects = 25;
		public bool UseInitialSpawn = false;		
		public float MinSpawnInterval = 10;
		public float MaxSpawnInterval = 60;
		public float RespawnIntervalMax = 360;

		private bool m_InitialSpawnComplete = false;
		public bool InitialSpawnComplete{
			get{ return m_InitialSpawnComplete; }
		}
		
		public bool UseRandomSize = false;
		public float RandomSizeMin = 0;
		public float RandomSizeMax = 0;
		
		public bool UseHierarchyGroupObject = false;	
		private GameObject m_GroupObject = null;
		private GameObject m_GroupRootObject = null;
		public GameObject CustomHierarchyGroupObject = null;
		
		private float m_SpawnTimer = 0;
		private float m_SpawnInterval = 0;
		public float RespawnInterval{
			get{return m_SpawnInterval; }
		}
		
		public int Count{
			get{ return Items.Count; }
		}

		public GameObject GetRandomObject()
		{
			if( Items.Count > 0 )
				return Items[ Random.Range( 0, Items.Count ) ];
			else
				return null;
		}

		/// <summary>
		/// Finds the nearest creature.
		/// </summary>
		/// <returns>The nearest creature.</returns>
		/// <param name="_origin">_origin.</param>
		/// <param name="_dist">_dist.</param>
		public GameObject FindNearestObject( GameObject _sender, float _range )
		{
			if( _sender == null )
				return null;
			
			GameObject _best_object = null;
			float _best_distance = _range;
			
			if( _best_distance == 0 )
				_best_distance = Mathf.Infinity;
			
			for( int i = 0; i < Items.Count; i++ )
			{
				GameObject _object = Items[i];
				
				if( _object != null && _object.activeInHierarchy && _object.transform.IsChildOf( _sender.transform ) == false )
				{
					float _distance = Vector3.Distance( _sender.transform.position, _object.transform.position );					
					if( _distance < _best_distance )
					{
						_best_distance = _distance;					
						_best_object = _object;
					}
				}
			}
			
			return _best_object;
		}

		/// <summary>
		/// Finds the nearest creature.
		/// </summary>
		/// <returns>The nearest creature.</returns>
		/// <param name="_origin">_origin.</param>
		/// <param name="_dist">_dist.</param>
		public List<GameObject> FindObjectsInRange( Vector3 _origin, float _range  )
		{
			List<GameObject> _objects = new List<GameObject>();

			for( int i = 0; i < Items.Count; i++ )
			{
				GameObject _object = Items[i];
				
				if( _object != null && _object.activeInHierarchy && _object.transform.position != _origin )
				{
					float _object_distance = Vector3.Distance( _origin, _object.transform.position );					
					if( _object_distance < _range )
						_objects.Add(_object );
				}
			}
			
			return _objects;
		}
		/*
		public GameObject GetRandomSpawnPoint() 
		{
			GameObject _object = null;

			if( SpawnPoints.Count > 0 )
			{
				SpawnPointObject _point = SpawnPoints[ Random.Range( 0, SpawnPoints.Count ) ];

				if( _point != null )
				{
					_object = _point.SpawnPointGameObject;

					if( _point.AccessType == TargetAccessType.NAME )
						_object = ICECreatureRegister.Instance.GetRandomTargetByName( _point.SpawnPointName );
					else if( _point.AccessType == TargetAccessType.TAG )
						_object = ICECreatureRegister.Instance.GetRandomTargetByTag( _point.SpawnPointTag );

				}				
			}

			return _object;
		}*/
		
		public Vector3 GetSpawnPosition() 
		{
			Vector3 _position = Vector3.zero;

			if( SpawnPoints.Count > 0 )
			{
				SpawnPointObject _point = SpawnPoints[ Random.Range( 0, SpawnPoints.Count ) ];

				if( _point != null )
				{
					GameObject _object = _point.SpawnPointGameObject;

					if( _point.AccessType == TargetAccessType.NAME )
						_object = ICECreatureRegister.Instance.GetRandomTargetByName( _point.SpawnPointName );
					else if( _point.AccessType == TargetAccessType.TAG )
						_object = ICECreatureRegister.Instance.GetRandomTargetByTag( _point.SpawnPointTag );

					if( _object != null )
					{ 
						if( _point.MaxSpawningRange > 0 )
							_position = GraphicTools.GetRandomCirclePosition( _object.transform.position, _point.MinSpawningRange, _point.MaxSpawningRange );
						else
							_position = _object.transform.position;
					}
				}				
			}
			else if( Reference != null )
			{
				_position = GraphicTools.GetRandomPosition( Reference.transform.position, 25 );
			}
			
			_position.y = GraphicTools.GetGroundLevel( _position, ICECreatureRegister.Instance.GroundCheck , ICECreatureRegister.Instance.GroundLayerMask );

			return _position;
		}

		public bool SoftRespawn() 
		{
			if( ! UseSoftRespawn || SuspendedItems.Count == 0 )
				return false;
			
			GameObject _creature = SuspendedItems[0];
			SuspendedItems.Remove(_creature);

			if( _creature != null )
			{
				_creature.transform.position = GetSpawnPosition();
				Quaternion _rotation = Random.rotation;
				_rotation.z = 0;
				_rotation.x = 0;
				
				_creature.transform.rotation = _rotation;
				
				if( _creature.GetComponent<ICECreatureControl>() != null )
					_creature.GetComponent<ICECreatureControl>().Creature.Status.Reset();
				
				_creature.SetActive( true );
			}

			return true;
			
		}
		
		public void SpawnAll()
		{
			bool _allow_spawning = true;
			while( _allow_spawning )
				_allow_spawning = Spawn();

			//Debug.Log( Name );
		}
		
		public bool Spawn()
		{
			if( Count < MaxObjects && Reorganize() )
			{
				
				Vector3 _position = GetSpawnPosition();		
				Quaternion _rotation = Random.rotation;
				
				_rotation.z = 0;
				_rotation.x = 0;
				
				GameObject _object = ICECreatureRegister.Instance.SpawnObject( Reference, _position, _rotation );
				
				if( _object == null )
					return false;
				
				if( UseRandomSize )
					_object.transform.localScale = _object.transform.localScale + ( _object.transform.localScale * (float)Random.Range( RandomSizeMin, RandomSizeMax ) );
						
				_object.name = Reference.name;
				
				if( UseHierarchyGroupObject && _object.transform.parent != m_GroupObject.transform )
					_object.transform.parent = m_GroupObject.transform;
				
				if( _object.GetComponent<ICECreatureControl>() != null )
				{
					_object.GetComponent<ICECreatureControl>().Creature.Status.Reset();
				}
				
				_object.SetActive( true );
				
				ICECreatureRegister.Instance.Register( _object );
				
				return true;		
			}
			else
				return false;
		}
		
		private bool Reorganize()
		{
			bool _done = false;
			
			if( UseHierarchyGroupObject )
			{
				Transform _parent = null;
				if( CustomHierarchyGroupObject != null ) 
				{
					if( m_GroupRootObject == null )
					{
						if( CustomHierarchyGroupObject.activeInHierarchy )
							m_GroupRootObject = CustomHierarchyGroupObject;
						else
							m_GroupRootObject = GameObject.Find( CustomHierarchyGroupObject.name );
					}
					
					if( m_GroupRootObject != null && m_GroupRootObject.activeInHierarchy )
					{
						_parent = m_GroupRootObject.transform;
						_done = true;
					}
					else
						_done = false;
				}
				else if( RegisterInstance != null )
				{
					_parent = RegisterInstance.GetHierarchyGroup( GroupType );
					_done = true;
				}
				
				if( _done )
				{
					// if no group object we create it 
					if( m_GroupObject == null )
					{
						m_GroupObject = new GameObject();						
						m_GroupObject.name = Name + "(Group)";
					}
					
					if( m_GroupObject != null )
					{
						m_GroupObject.transform.parent = _parent;
						m_GroupObject.transform.position = Vector3.zero;
					}
				}
			}
			else
				_done = true;
			
			return _done;
		}
		
		public void Update()
		{
			if( ! CanSpawn )
				return;
			
			if( Reorganize() )
			{
				if( UseInitialSpawn && InitialSpawnComplete == false )
				{
					m_InitialSpawnComplete = true;
					SpawnAll();
				}
				else
				{
					if( m_SpawnInterval == 0 )
						m_SpawnInterval = Random.Range( MinSpawnInterval, MaxSpawnInterval );

					m_SpawnTimer += Time.deltaTime;				
					if( m_SpawnTimer >= m_SpawnInterval )
					{
						m_SpawnTimer = 0;
						m_SpawnInterval = Random.Range( MinSpawnInterval, MaxSpawnInterval );
						
						if( SoftRespawn() == false )
							Spawn();
					}
				}
			}
		}
	}

}
