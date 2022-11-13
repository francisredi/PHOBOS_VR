// ##############################################################################
//
// ice_CreatureSurface.cs
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
using ICE.Utilities;

namespace ICE.Creatures.Objects
{
	
	[System.Serializable]
	public class SurfaceDataObject : System.Object
	{
		public SurfaceDataObject()
		{
			Audio = new AudioDataObject();
		}

		public bool Foldout = true;
		public bool Enabled = true;
		public float Interval = 1;
		public string Name = ""; 

		//public List<AudioClip> Sounds = new List<AudioClip>(); 
		public List<Texture> Textures = new List<Texture>(); 

		public bool UseBehaviourModeKey = false;
		public string BehaviourModeKey = "";

		public EffectContainer Effect;
		public StatusContainer Influences;
		public AudioDataObject Audio;
	}
	
	[System.Serializable]
	public class SurfaceObject : System.Object
	{
		public bool Enabled = false;
		public float Interval = 1;

		private float m_IntervalTimer = 0.0f;
		private string m_TextureName = "";


		private SurfaceDataObject m_ActiveSurface = null;
		public SurfaceDataObject ActiveSurface{
			get{ return m_ActiveSurface; }
		}
		
		public List<SurfaceDataObject> Surfaces = new List<SurfaceDataObject>();

		private GameObject m_Owner = null;

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;

			Audio.Init( m_Owner );
		}

		private AudioObject m_Audio = null;
		public AudioObject Audio
		{
			get{
				if( m_Audio == null )
					m_Audio = new AudioObject( m_Owner );
				
				return m_Audio;
			}
		}

		private float m_ScanIntervalTimer = 0;
		public float ScanInterval = 1;

		/// <summary>
		/// Update surface handling.
		/// </summary>
		public void Update( CreatureObject _creature )
		{
			if( Enabled == false || _creature == null )
				return;

			Vector3 _velocity = _creature.Move.MoveVelocity;
			if( _creature.Behaviour.BehaviourMode != null && _creature.Behaviour.BehaviourMode.Rule != null )
				_velocity = _creature.Behaviour.BehaviourMode.Rule.Move.Velocity.Velocity;
			
			// Update Ground Texture Name - only required while creature is moving or texture name is empty
			if( _velocity.z > 0 || m_TextureName == "" )
			{
				m_ScanIntervalTimer += Time.deltaTime;
				if( m_ScanIntervalTimer >= ScanInterval )
				{
					m_ScanIntervalTimer = 0;
					m_TextureName = _creature.Move.UpdateGroundTextureName();
				}
			}
				
			HandleSurface( _velocity );
		}

		/// <summary>
		/// Handles the surface.
		/// </summary>
		/// <param name="_velocity">_velocity.</param>
		public void HandleSurface( Vector3 _velocity )
		{
			if( _velocity.z == 0 )
			{
				Audio.Stop();
				return;
			}
				
			if( m_IntervalTimer > 0)
				m_IntervalTimer -= Time.deltaTime;
			
			
			if( m_IntervalTimer <= 0 )
			{
				m_IntervalTimer = Interval;// * (_velocity.z/100);
				
				SurfaceDataObject _new_surface = null;
				
				foreach( SurfaceDataObject _surface in Surfaces)
				{
					foreach( Texture _texture in _surface.Textures )
					{
						if( _texture != null && _texture.name == m_TextureName )
						{
							_new_surface = _surface;
							break;
						}
					}
				}
				
				if( _new_surface != null )
				{
					m_IntervalTimer = _new_surface.Interval - MathTools.Normalize( _velocity.z, 0, 25 );
					
					if( m_ActiveSurface != null && m_ActiveSurface != _new_surface )
						m_ActiveSurface.Effect.StopEffect();
					
					if( m_ActiveSurface != _new_surface )
					{
						m_ActiveSurface = _new_surface;
						m_ActiveSurface.Effect.StartEffect( m_Owner );
					}

					Audio.Play( m_ActiveSurface.Audio );
						
				}
				else
				{
					if( m_ActiveSurface != null )
						m_ActiveSurface.Effect.StopEffect();
					
					m_ActiveSurface = null;

					Audio.Stop();
				}
			}
		}


	}
}