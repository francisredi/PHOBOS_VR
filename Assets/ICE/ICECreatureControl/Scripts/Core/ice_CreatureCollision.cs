// ##############################################################################
//
// ice_CreatureCollision.cs
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
using ICE.Creatures;
using ICE.Creatures.EnumTypes;

namespace ICE.Creatures.Objects
{

	[System.Serializable]
	public class CollisionDataObject : System.Object
	{
		public bool Foldout = true;
		public bool Enabled = true;

		public bool UseTag = true;
		public bool UseLayer = true;
		public bool UseBodyPart = true;

		public string Name = "";
		public bool ForceInteraction = false;
		public string Tag = "";
		public int Layer = 0;
		public string BodyPart = "";
		public StatusContainer Influences;
		public bool UseBehaviourModeKey = false;
		public string BehaviourModeKey = "";

	}

	[System.Serializable]
	public class CollisionObject : System.Object
	{
		public CollisionObject()
		{

		}

		public bool Foldout = true;
		public bool Enabled = false;

		private TargetObject m_Target = null;
		public virtual TargetObject Target 
		{
			get{ 

				if( m_Target == null )
				   m_Target = new TargetObject( TargetType.UNDEFINED );

				return m_Target; 
			
			}
		}

		public List<CollisionDataObject> Collisions = new List<CollisionDataObject>();
		
		private GameObject m_Owner = null;
		
		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;
		}
		/*
		public CollisionDataObject CheckExternal( GameObject _object )
		{
			if( _object == null )
				return null;

			CollisionDataObject _current_collision = null;
			
			foreach( CollisionDataObject _collision in Collisions )
			{
				if( _collision.Enabled && _collision.Type == ImpactType.EXTERN )
					_current_collision = _collision;
			}
			
			if( _current_collision != null && _current_collision.ForceInteraction )
			{
				Target.TargetGameObject = _object;
				Target.TargetOffset.z = 2;
				Target.TargetStopDistance = 2;
				Target.TargetRandomRange = 0;
				Target.BehaviourModeKey = _current_collision.BehaviourModeKey;
			}
			
			return _current_collision;
		}*/

		public List<CollisionDataObject> CheckCollider( Collider _collider, string _contact_name )
		{
			if( _collider == null || _collider.transform.IsChildOf( m_Owner.transform ) )
				return new List<CollisionDataObject>();

			string _tag = _collider.tag;
			int _layer = _collider.gameObject.layer;
	
			List<CollisionDataObject> _collisions = new List<CollisionDataObject>();

			foreach( CollisionDataObject _impact in Collisions )
			{
				if( _impact.Enabled )
				{
					bool _ready = true;

					if( _impact.UseTag && _impact.Tag != _tag )
						_ready = false;

					if( _impact.UseLayer && _impact.Layer != _layer )
						_ready = false;

					if( _impact.UseBodyPart && _impact.BodyPart != _contact_name )
						_ready = false;
					
					if( _ready )
						_collisions.Add( _impact );
					 
				}
			}

			/*
			if( _current_impact != null && _current_impact.ForceInteraction )
			{
				Target.OverrideTargetGameObject( _collider.gameObject );
				Target.Move.Offset.z = 2;
				Target.Move.StopDistance = 2;
				Target.Move.RandomRange = 0;
				Target.BehaviourModeKey = _current_impact.BehaviourModeKey;
			}*/

			return _collisions;
		}

		public bool HasTarget()
		{
			if( m_Target != null && m_Target.TargetGameObject != null ) 
				return true;
			else
				return false;
		}


	}
}