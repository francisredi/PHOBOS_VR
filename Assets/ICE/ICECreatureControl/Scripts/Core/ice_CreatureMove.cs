// ##############################################################################
//
// ice_CreatureMove.cs
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

using ICE.Creatures.EnumTypes;
using ICE.Utilities;
using ICE.Utilities.EnumTypes;

namespace ICE.Creatures.Objects
{


	[System.Serializable]
	public struct MoveDetourContainer
	{
		public Vector3 Position;
	}

	[System.Serializable]
	public struct MoveEscapeContainer
	{
		public float EscapeDistance;
		public float RandomEscapeAngle;
	}

	[System.Serializable]
	public struct MoveAvoidContainer
	{
		public float AvoidDistance;
	}

	[System.Serializable]
	public struct MoveOrbitContainer
	{
		public float Radius;
		public float RadiusShift;
		public float MaxDistance;
		public float MinDistance;
	}

	[System.Serializable]
	public struct MoveVelocityContainer
	{
		public VelocityType Type;
		public Vector3 Velocity;
		public Vector3 VelocityMax;
		public bool UseNegativeVelocity;
		public float VelocityMinVariance;
		public float VelocityMaxVariance;
		public bool UseTargetVelocity;
		public float AngularVelocity;
		public bool AngularVelocityAuto;
		public float Inertia;

		public bool UseAutoDrift;
		public float DriftMultiplier;

		public float VelocityMultiplier;
		public float VelocityMultiplierUpdateTimer;
		public float VelocityMultiplierUpdateInterval;

		public void Copy( MoveVelocityContainer _velocity )
		{
			Type = _velocity.Type;

			Velocity = _velocity.Velocity;
			VelocityMax = _velocity.VelocityMax;
			UseNegativeVelocity = _velocity.UseNegativeVelocity;
			VelocityMinVariance = _velocity.VelocityMinVariance;
			VelocityMaxVariance = _velocity.VelocityMaxVariance;
			UseTargetVelocity = _velocity.UseTargetVelocity;
			AngularVelocity = _velocity.AngularVelocity;
			AngularVelocityAuto = _velocity.AngularVelocityAuto;
			Inertia = _velocity.Inertia;

			UseAutoDrift = _velocity.UseAutoDrift;
			DriftMultiplier = _velocity.DriftMultiplier;

			VelocityMultiplier = _velocity.VelocityMultiplier;
			VelocityMultiplierUpdateTimer = _velocity.VelocityMultiplierUpdateTimer;
			VelocityMultiplierUpdateInterval = _velocity.VelocityMultiplierUpdateInterval;
		}

		public float UpdateVelocityMultiplier()
		{
			VelocityMultiplier = Random.Range( VelocityMinVariance, VelocityMaxVariance );
			
			return VelocityMultiplier;
		}
		
		public float GetVelocityMultiplier()
		{
			//VelocityMultiplier = Random.Range( VelocityMinVariance, VelocityMaxVariance );
			
			return VelocityMultiplier;
		}

		private float _drift_time ;
		private float _drift_timer;
		public Vector3 GetVelocity( ICECreatureControl _control, Vector3 _current_velocity )
		{
			Vector3 _velocity = _current_velocity;
			float _target_velocity = _control.Creature.Move.CurrentTarget.TargetVelocity;
			float _forwards = 0;

			// FORWARD BEGIN
			if( UseTargetVelocity && _target_velocity > 0 ) 
				_forwards = _target_velocity + ( _target_velocity * GetVelocityMultiplier() ) + Velocity.z; 
			else
				_forwards = Velocity.z + ( Velocity.z * GetVelocityMultiplier() ); 

			if( Type == VelocityType.ADVANCED && Inertia > 0 && _velocity.z < _forwards )
				_velocity.z += ( _forwards - _velocity.z ) * Inertia;
			else
				_velocity.z = _forwards;
			
			//_velocity.z *= _control.Creature.Status.SpeedMultiplier;
			// FORWARD END
			/*
		
			// DRIFT BEGIN
			if( UseAutoDrift )
			{
				if( _drift_time == 0 || _drift_timer > _drift_time )
					_drift_time = Random.Range ( 0.5f, 3 );
				else
					_drift_timer += Time.deltaTime;

				if( _drift_time != 0 && _drift_timer > _drift_time )
				{
					_velocity.x += ( 0.1f * Random.Range ( - DriftMultiplier, DriftMultiplier ) );
					_drift_timer = 0;
				}
			}
			// DRIFT END	*/

			return _velocity;
		}

		public float GetTurnSpeed( ICECreatureControl _control )
		{
			float _speed = 0;

			if( AngularVelocityAuto && Velocity.z > 0 )
				_speed = ( ( 25 / Velocity.z / 4  ) );
			else
				_speed = AngularVelocity;

			return _speed;
		}
	}

	[System.Serializable]
	public struct BodyContainer
	{
		public BodyContainer( GroundOrientationType _type ){
			Type = _type;
			UseAdvanced = false;

			UseLeaningTurn = false;
			MaxLeanAngle = 30;
			LeanAngleMultiplier = 0.5f;
						
			Width = 0;
			Length = 0;
			Height = 0;
			WidthOffset = 0;
			DepthOffset = 0;

			DefaultWidth = 1;
			DefaultLength = 1;
			DefaultHeight = 1;
		}

		public BodyContainer( BodyContainer _body ){
			Type = _body.Type;
			UseAdvanced = _body.UseAdvanced;

			UseLeaningTurn = _body.UseLeaningTurn;
			MaxLeanAngle = _body.MaxLeanAngle;
			LeanAngleMultiplier = _body.LeanAngleMultiplier;

			Width = _body.Width;
			Length = _body.Length;
			Height = _body.Height;
			WidthOffset = _body.WidthOffset;
			DepthOffset = _body.DepthOffset;

			DefaultWidth = _body.DefaultWidth;
			DefaultLength = _body.DefaultLength;
			DefaultHeight = _body.DefaultHeight;
		}

		public void Copy( BodyContainer _body )
		{
			Type = _body.Type;
			UseAdvanced = _body.UseAdvanced;

			UseLeaningTurn = _body.UseLeaningTurn;
			MaxLeanAngle = _body.MaxLeanAngle;
			LeanAngleMultiplier = _body.LeanAngleMultiplier;

			Width = _body.Width;
			Length = _body.Length;
			Height = _body.Height;
			WidthOffset = _body.WidthOffset;
			DepthOffset = _body.DepthOffset;

			DefaultWidth = _body.DefaultWidth;
			DefaultLength = _body.DefaultLength;
			DefaultHeight = _body.DefaultHeight;
		}

		public GroundOrientationType Type;
		public bool UseAdvanced;

		public bool UseLeaningTurn;
		public float MaxLeanAngle;
		public float LeanAngleMultiplier;

		public float Width;
		public float Length;
		public float Height;
		public float WidthOffset;
		public float DepthOffset;

		public float DefaultWidth;
		public float DefaultLength;
		public float DefaultHeight;

		public void GetDefaultSize( GameObject _owner )
		{
			Transform[] _transforms = _owner.GetComponentsInChildren<Transform>();
			Vector3 _width_l = Vector3.zero;
			Vector3 _width_r = Vector3.zero;
			Vector3 _depth_f = Vector3.zero;
			Vector3 _depth_b = Vector3.zero;
			Vector3 _height_t = Vector3.zero;
			Vector3 _height_b = Vector3.zero;
	
			_width_l.y = Mathf.Infinity;
			_width_r.y = Mathf.Infinity;
			_depth_f.y = Mathf.Infinity;
			_depth_b.y = Mathf.Infinity;
		
			foreach( Transform _transform in _transforms )
			{

				Vector3 _position = _transform.TransformPoint( _transform.localPosition );

				_position.x = _position.x/_owner.transform.lossyScale.x;
				_position.y = _position.y/_owner.transform.lossyScale.y;
				_position.z = _position.z/_owner.transform.lossyScale.z;

				_position = _owner.transform.InverseTransformPoint( _position );

				if( _position.x <= _width_l.x && _position.y <= _width_l.y )
				{
					_width_l.x = _position.x;
					_width_l.y = _position.y;
				}

				if( _position.x >= _width_r.x && _position.y <= _width_r.y )
				{
					_width_r.x = _position.x;
					_width_r.y = _position.y;
				}
				if( _position.z <= _depth_b.z && _position.y <= _depth_b.y )
				{
					_depth_b.z = _position.z;
					_depth_b.y = _position.y;
				}

				if( _position.z >= _depth_f.z && _position.y <= _depth_f.y )
				{
					_depth_f.z = _position.z;
					_depth_f.y = _position.y;
				}

				if( _position.y >= _height_t.y )
				{
					_height_t.z = _position.z;
					_height_t.y = _position.y;
				}

				if( _position.y <= _height_b.y )
				{
					_height_b.z = _position.z;
					_height_b.y = _position.y;
				}
			}

			DefaultWidth = (_width_l.x * -1 ) + _width_r.x;
			DefaultLength = (_depth_b.z * -1 ) + _depth_f.z;
			DefaultHeight = (_height_b.z * -1 ) + _height_t.z;

		}
	}


	[System.Serializable]
	public struct MoveContainer
	{
		public bool Enabled;

		public MoveType Type;

		public MoveDetourContainer Detour;
		public MoveOrbitContainer Orbit;
		public MoveEscapeContainer Escape;
		public MoveAvoidContainer Avoid;
		public MoveVelocityContainer Velocity;

		public ViewingDirectionType ViewingDirection;
		public Vector3 ViewingDirectionPosition;

		// DEFAULT
		public float MoveStopDistance;
		public float MoveSegmentLength;
		public float MaxMoveSegmentLength;
		public float MoveSegmentVariance;	
		public float MoveDeviationLength;
		public float MoveDeviationLengthMax;
		public float MoveDeviationVariance;		
		public bool MoveIgnoreLevelDifference;

		public MoveCompleteType Link;
		public string NextBehaviourModeKey;

		public void Copy( MoveContainer _move )
		{
			this.Type = _move.Type;

			this.Detour = _move.Detour;
			this.Orbit = _move.Orbit;
			this.Escape = _move.Escape;
			this.Avoid = _move.Avoid;
			this.Velocity.Copy( _move.Velocity );

			this.ViewingDirection = _move.ViewingDirection;
			this.ViewingDirectionPosition = _move.ViewingDirectionPosition;

			this.MoveStopDistance = _move.MoveStopDistance;
			this.MoveSegmentLength = _move.MoveSegmentLength;
			this.MaxMoveSegmentLength = _move.MaxMoveSegmentLength;
			this.MoveSegmentVariance = _move.MoveSegmentVariance;
			this.MoveDeviationLength = _move.MoveDeviationLength;
			this.MoveDeviationLengthMax = _move.MoveDeviationLengthMax;
			this.MoveDeviationVariance = _move.MoveDeviationVariance;
			this.MoveIgnoreLevelDifference = _move.MoveIgnoreLevelDifference;

			this.Link = _move.Link;
			this.NextBehaviourModeKey = _move.NextBehaviourModeKey;
		}

		public float GetMoveDeviationVariance()
		{
			float _lateral_variance = MoveDeviationVariance;
			float _lateral_legth = MoveDeviationLength;
			
			if(  _lateral_legth > 0 && _lateral_variance > 0 )
				_lateral_variance = _lateral_legth * Random.Range( - _lateral_variance, _lateral_variance );
			
			return _lateral_variance;
		}

		public float GetMoveSegmentLength()
		{
			float _directional_variance = MoveSegmentVariance;
			float _segment_legth = MoveSegmentLength;
			
			if( _segment_legth > 0 && _directional_variance > 0 )
				_segment_legth += _segment_legth * Random.Range( - _directional_variance, _directional_variance );

			return _segment_legth;
		}

		public float MoveSegmentLengthMax{
			get{ 
				float _directional_variance = MoveSegmentVariance;
				float _segment_legth = MoveSegmentLength;

				if( _directional_variance > 0 )
					_segment_legth += _segment_legth * _directional_variance;
				return _segment_legth;
			}
		}

		public float MoveSegmentLengthMin{
			get{ 
				float _directional_variance = MoveSegmentVariance;
				float _segment_legth = MoveSegmentLength;
				
				if( _directional_variance > 0 )
					_segment_legth += _segment_legth *  - _directional_variance;
				return _segment_legth;
			}
		}

		public float GetMaxMoveSegmentLength()
		{
			float _directional_variance = MoveSegmentVariance;
			float _segment_legth = MoveSegmentLength;
			
			if( _segment_legth > 0 && _directional_variance > 0 )
				_segment_legth *= Random.Range( - _directional_variance, _directional_variance );
			
			return _segment_legth;
		}



		public MoveContainer( MoveType _type )
		{
			this.Enabled = false;
			this.Type = _type;
			
			this.ViewingDirection = ViewingDirectionType.DEFAULT;
			this.ViewingDirectionPosition = Vector3.zero;
			
			this.Detour = new MoveDetourContainer();
			this.Orbit = new MoveOrbitContainer();
			this.Escape = new MoveEscapeContainer();
			this.Avoid = new MoveAvoidContainer();
			this.Velocity = new MoveVelocityContainer();
			
			this.MoveStopDistance = 0;
			this.MoveSegmentLength = 0;
			this.MaxMoveSegmentLength = 100;
			this.MoveSegmentVariance = 0;
			this.MoveDeviationLength = 0;
			this.MoveDeviationLengthMax = 100;
			this.MoveDeviationVariance = 0;
			this.MoveIgnoreLevelDifference = true;
			
			this.Link = MoveCompleteType.DEFAULT;
			this.NextBehaviourModeKey = "";
		}
	}

	/// <summary>
	/// Move object. Handles all creature motions.
	/// </summary>
	[System.Serializable]
	public class MoveObject : System.Object
	{
		private GameObject m_Owner = null;

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;

			m_NavMeshAgent = m_Owner.GetComponent<UnityEngine.AI.NavMeshAgent>();

						
		}

		private ICECreatureControl m_Controller = null;
		private ICECreatureControl Controller
		{
			get{
				if( m_Controller == null )
					m_Controller = m_Owner.GetComponent<ICECreatureControl>();

				return m_Controller;
			}
		}


		// Event Handler
		public delegate void OnTargetMovePositionReachedEvent( GameObject _sender, TargetObject _target );
		public event OnTargetMovePositionReachedEvent OnTargetMovePositionReached;

		public delegate void OnMoveCompleteEvent( GameObject _sender, TargetObject _target );
		public event OnMoveCompleteEvent OnMoveComplete;

		public delegate void OnMoveUpdatePositionEvent( GameObject _sender, Vector3 _origin_position, ref Vector3 _new_position );
		public event OnMoveUpdatePositionEvent OnUpdateMovePosition;

		public BodyContainer DefaultBody = new BodyContainer( GroundOrientationType.DEFAULT );
		public BodyContainer CurrentBody;
		public MoveContainer DefaultMove = new MoveContainer( MoveType.DEFAULT );
		public MoveContainer CurrentMove;

		//private TargetObject m_PreviousTarget = null;
		private TargetObject m_CurrentTarget = null;
		public TargetObject CurrentTarget{
			get{ return m_CurrentTarget; }
		}

		//private BehaviourModeRuleObject m_PreviousBehaviourModeRule = null;
		private BehaviourModeRuleObject m_CurrentBehaviourModeRule = null;
		public BehaviourModeRuleObject CurrentBehaviourModeRule{
			get{ return m_CurrentBehaviourModeRule; }
		}

		private Vector3 m_LastMovePosition = Vector3.zero;
		public Vector3 LastMovePosition{
			get{ return m_LastMovePosition; }
		}

		private Quaternion m_LastMoveRotation = Quaternion.identity;
		public Quaternion LastMoveRotation{
			get{ return m_LastMoveRotation; }
		}

		private Vector3 m_NextMovePosition = Vector3.zero;
		public Vector3 NextMovePosition{
			get{ return m_NextMovePosition; }
		}
		
		private Quaternion m_NextMoveRotation = Quaternion.identity;
		public Quaternion NextMoveRotation{
			get{ return m_NextMoveRotation; }
		}

		private Quaternion m_MoveRotation = Quaternion.identity;
		public Quaternion MoveRotation{
			get{ 
				if( m_MoveRotation == Quaternion.identity && m_Owner != null )
				{
					if( m_Owner.GetComponent<ICECreatureControl>() != null && m_Owner.GetComponent<ICECreatureControl>().Creature.ActiveTarget != null )
					{
						Vector3 _heading = ( m_Owner.GetComponent<ICECreatureControl>().Creature.ActiveTarget.TargetMovePosition - m_Owner.transform.position ).normalized;
						m_MoveRotation = Quaternion.LookRotation( _heading );
					}
					else
						m_MoveRotation = m_Owner.transform.rotation;
				}
				
				return m_MoveRotation; 
				
			}
		}

		private Vector3 m_MovePosition = Vector3.zero;
		public Vector3 MovePosition{
			get{ 
				if( m_MovePosition == Vector3.zero && m_Owner != null )
				{
					if( m_Owner.GetComponent<ICECreatureControl>() != null && m_Owner.GetComponent<ICECreatureControl>().Creature.ActiveTarget != null )
						m_MovePosition = m_Owner.GetComponent<ICECreatureControl>().Creature.ActiveTarget.TargetMovePosition;
					else
						m_MovePosition = m_Owner.transform.position;
				}

				return m_MovePosition; 
			
			}
		}

		private Vector3 m_MoveVelocity = Vector3.zero;
		public Vector3 MoveVelocity{
			get{ return m_MoveVelocity; }
		}

		private float m_MoveAngularVelocity = 0.0f;
		public float MoveAngularVelocity{
			get{ return m_MoveAngularVelocity; }
		}

		public float MoveAvoidDistance = 5;
		public float MoveStopDistance = 2;


		private float m_MoveMaxDistance = 10.0f; 
		public float MoveMaxDistance {
			get{ return m_MoveMaxDistance; }
		}

		public bool UseDeadlockHandling = false;

		public bool AvoidWater = true;
		public bool UseSlopeLimits = true;
		public float MaxWalkableSlopeAngle = 35;
		public float MaxSlopeAngle = 45;

		public float MinAvoidAngle = 0.35f;

		public bool UseFreePosition = true;
		public bool UseInternalGravity = true;
		public bool UseWorldGravity = true;
		[SerializeField]
		private float m_Gravity = 9.8f;
		public float Gravity{
			set{
				if( ! UseWorldGravity )
					m_Gravity = value;
			}
			get{
				if( UseWorldGravity )
					return Physics.gravity.y * -1;
				else				
					return m_Gravity;
			}
		}

		public MotionControlType MotionControl = MotionControlType.INTERNAL;

		private Rigidbody m_Rigidbody = null;
		public Rigidbody RigidbodyComponent{
			get{
				if( m_Rigidbody == null )
					m_Rigidbody = m_Owner.GetComponent<Rigidbody>();
				
				return m_Rigidbody;
			}
		}

		public bool RigidbodyReady
		{
			get{
				if( MotionControl == MotionControlType.RIGIDBODY && RigidbodyComponent != null && RigidbodyComponent )
					return true;
				else
					return false;
			}
		}

		public bool NavMeshForced = false;
		private UnityEngine.AI.NavMeshAgent m_NavMeshAgent = null;
		public UnityEngine.AI.NavMeshAgent NavMeshAgentComponent
		{
			get{

				if( m_NavMeshAgent == null )
					m_NavMeshAgent = m_Owner.GetComponent<UnityEngine.AI.NavMeshAgent>();

				return m_NavMeshAgent;
			}
		}

		public int m_NavMeshPathPendingCounter;

		public bool NavMeshAgentReady
		{
			get{
				if( MotionControl == MotionControlType.NAVMESHAGENT && NavMeshAgentComponent != null )
					return true;
				else
					return false;
			}

		}

		public bool NavMeshAgentHasPath
		{
			get{
				if( NavMeshAgentReady == true && m_NavMeshAgent.hasPath == true )
					return true;
				else
					return false;
			}
			
		}

		public bool NavMeshAgentIsOnNavMesh
		{
			get{
				if( NavMeshAgentReady == true && m_NavMeshAgent.isOnNavMesh == true )
					return true;
				else
					return false;
			}
			
		}

		private CharacterController m_CharacterController = null;
		private CharacterController CharacterControllerComponent
		{
			get{
				
				if( m_CharacterController == null )
					m_CharacterController = m_Owner.GetComponent<CharacterController>();
				
				return m_Owner.GetComponent<CharacterController>();
			}
		}

		public bool CharacterControllerReady
		{
			get{
				if( MotionControl == MotionControlType.CHARACTERCONTROLLER && CharacterControllerComponent != null && CharacterControllerComponent )
					return true;
				else
					return false;
			}
		}

		public GroundCheckType GroundCheck = GroundCheckType.NONE;
		public float GroundCheckBaseOffset = 0;
		public float VerticalRaycastOffset = 0.5f;

		private float m_VerticalRaycastOffset = 0;
		private float m_HorizontalRaycastOffset = 0;
		private float RaycastOffset{
			get{ return VerticalRaycastOffset + m_VerticalRaycastOffset; }
		}

		public float DownRaycastOffsetMax = 50;
		public ObstacleCheckType ObstacleCheck = ObstacleCheckType.NONE;

		public float FieldOfView = 80;
		public float VisualRange = 0;
		public float MaxVisualRange = 100;
		public float EyeHeight = 1.6f;

		[SerializeField]
		private List<string> m_GroundLayers = new List<string>();
		public List<string> GroundLayers{
			get{ return m_GroundLayers; }
		}

		public void SetGroundLayers( List<string> _layers )
		{
			if( _layers == null )
				return;

			m_GroundLayers.Clear();
			foreach( string _layer in _layers )
				m_GroundLayers.Add( _layer );
		}

		private LayerMask m_GroundLayerMask = -1;
		private LayerMask GroundLayerMask{
			get{
				m_GroundLayerMask = GraphicTools.GetLayerMask( GroundLayers, m_GroundLayerMask, AvoidWater );
				return m_GroundLayerMask;
			}
		}

		[SerializeField]
		private List<string> m_ObstacleLayers = new List<string>();
		public List<string> ObstacleLayers{
			get{ return m_ObstacleLayers; }
		}

		public void SetObstacleLayers( List<string> _layers )
		{
			if( _layers == null )
				return;

			m_ObstacleLayers.Clear();
			foreach( string _layer in _layers )
				m_ObstacleLayers.Add( _layer );
		}

		private LayerMask m_ObstacleLayerMask = -1;
		private LayerMask ObstacleLayerMask{
			get{
				m_ObstacleLayerMask = GraphicTools.GetLayerMask( ObstacleLayers, m_ObstacleLayerMask, AvoidWater );
				return m_ObstacleLayerMask;
			}
		}

		private bool m_HasOrbit = false;
		public bool HasOrbit{
			get{return m_HasOrbit;}
		}

		private bool m_OrbitComplete = false;
		public bool OrbitComplete{
			get{return m_OrbitComplete;}
		}

		private bool m_HasDetour = false;
		public bool HasDetour{
			get{return m_HasDetour;}
		}

		private bool m_DetourComplete = false;
		public bool DetourComplete{
			get{return m_DetourComplete;}
		}

		bool m_HasEscapePosition = false;
		public bool HasEscape{
			get{return m_HasEscapePosition;}
		}

		bool m_HasAvoidPosition = false;
		public bool HasAvoid{
			get{return m_HasAvoidPosition;}
		}

		/// <summary>
		/// Stops the move.
		/// </summary>
		public void StopMove()
		{
			if( NavMeshAgentReady && m_NavMeshAgent.hasPath )
				m_NavMeshAgent.ResetPath();
		}

		/// <summary>
		/// Updates the move.
		/// </summary>
		/// <param name="behaviour">Behaviour.</param>
		/// <param name="target">Target.</param>
		public void UpdateMove( TargetObject _target, BehaviourModeRuleObject _rule )
		{
			if( m_Owner == null || _rule == null || _target == null || _target.IsValid == false )
			{
				//m_PreviousTarget = m_CurrentTarget;
				m_CurrentTarget = null;

				//m_PreviousBehaviourModeRule = m_CurrentBehaviourModeRule;
				m_CurrentBehaviourModeRule = null;

				return;
			}

			if( m_CurrentTarget != _target )
			{
				//m_PreviousTarget = m_CurrentTarget;
				m_CurrentTarget = _target;
			}

			if( m_CurrentBehaviourModeRule != _rule )
			{
				//m_PreviousBehaviourModeRule = m_CurrentBehaviourModeRule;
				m_CurrentBehaviourModeRule = _rule;

				m_CurrentBehaviourModeRule.Move.Velocity.UpdateVelocityMultiplier();
				m_MovePosition = Vector3.zero;
			}

			CurrentBody.Copy( m_CurrentBehaviourModeRule.Body );
			if( m_CurrentBehaviourModeRule.Body.Type == GroundOrientationType.DEFAULT )
			{
				CurrentBody.Type = DefaultBody.Type;
				CurrentBody.UseAdvanced = DefaultBody.UseAdvanced;
				CurrentBody.Length = DefaultBody.Length;
				CurrentBody.Width = DefaultBody.Width;
				CurrentBody.Height = DefaultBody.Height;
				CurrentBody.DepthOffset = DefaultBody.DepthOffset;
				CurrentBody.WidthOffset = DefaultBody.WidthOffset;

				CurrentBody.UseLeaningTurn = DefaultBody.UseLeaningTurn;
				CurrentBody.MaxLeanAngle = DefaultBody.MaxLeanAngle;
				CurrentBody.LeanAngleMultiplier = DefaultBody.LeanAngleMultiplier;
			}

			CurrentMove.Copy( m_CurrentBehaviourModeRule.Move );
			if( m_CurrentBehaviourModeRule.Move.Type == MoveType.DEFAULT )
			{
				CurrentMove.MoveSegmentLength = DefaultMove.MoveSegmentLength;
				CurrentMove.MoveSegmentVariance = DefaultMove.MoveSegmentVariance;
				CurrentMove.MoveDeviationLength = DefaultMove.MoveDeviationLength;
				CurrentMove.MoveDeviationVariance = DefaultMove.MoveDeviationVariance;
				CurrentMove.MoveStopDistance = DefaultMove.MoveStopDistance;
				CurrentMove.MoveIgnoreLevelDifference = DefaultMove.MoveIgnoreLevelDifference;
			}
				
			// VELOCITY BEGIN
			if( m_CurrentBehaviourModeRule.MoveRequired )
			{
				m_MoveVelocity = CurrentMove.Velocity.GetVelocity( Controller, m_MoveVelocity );
				m_MoveAngularVelocity = CurrentMove.Velocity.GetTurnSpeed( Controller );
			}
			else
			{
				m_MoveVelocity = Vector3.zero;
				m_MoveAngularVelocity = 0;
			}

			// VELOCITY END

			if( CurrentMove.Type == MoveType.DETOUR )
				m_HasDetour = true;
			else
			{
				m_HasDetour = false;
				m_DetourComplete = false;
			}

			if( CurrentMove.Type == MoveType.ORBIT )
				m_HasOrbit = true;
			else
			{
				m_HasOrbit = false;
				m_OrbitComplete = false;
			}

			if( CurrentMove.Type == MoveType.ESCAPE )
			{
			}
			else
			{
				m_HasEscapePosition = false;
			}

			m_CurrentTarget.TargetMovePositionUpdateLevel( GetGroundLevel( m_CurrentTarget.TargetMovePosition ) );

			// HANDLE TARGET MOVE POSITION
			if( m_CurrentTarget.TargetMovePositionReached( m_Owner.transform.position ) )
			{
				if( OnTargetMovePositionReached != null )
					OnTargetMovePositionReached( m_Owner, m_CurrentTarget );
			}

			m_MovePosition = GetMovePosition();
			m_MoveRotation = GetMoveRotation();

			// NAVMESHAGENT MOVEMENT 
			if( NavMeshAgentReady )
			{
				m_NavMeshAgent.speed = m_MoveVelocity.z;
				m_NavMeshAgent.angularSpeed = m_MoveAngularVelocity * 100;
				m_NavMeshAgent.stoppingDistance = Mathf.Min( CurrentMove.MoveStopDistance, CurrentTarget.Move.StopDistance ) - 0.5f;

				if( m_MovePosition != m_LastMovePosition || ! m_NavMeshAgent.hasPath )
				{
					m_NavMeshAgent.SetDestination( m_MovePosition );
					m_LastMovePosition = m_MovePosition;
				}

				if( m_MoveVelocity == Vector3.zero )
				{
					m_NavMeshAgent.Stop(); 

					if( m_NavMeshAgent.hasPath )
						m_NavMeshAgent.ResetPath();
				}
				else if( m_NavMeshAgent.hasPath ) 
					m_NavMeshAgent.Resume();

				// CREATURE ROTATION BEGIN	
				if( CurrentMove.ViewingDirection != ViewingDirectionType.DEFAULT )
					m_Owner.transform.rotation = HandleStepRotation();
				// CREATURE ROTATION END
			}

			// CHARACTER CONTROLLER MOVEMENT 
			else if( CharacterControllerReady  )
			{
				// CREATURE ROTATION BEGIN
				m_Owner.transform.rotation = HandleStepRotation();
				// CREATURE ROTATION END

			
				Vector3 _forward = m_Owner.transform.TransformDirection( Vector3.forward) ;
				
				CharacterControllerComponent.SimpleMove( _forward * m_MoveVelocity.z );
			
					/*
				_heading = _heading.normalized;

		
				if ( CharacterControllerComponent.isGrounded ) 
				{

					_heading = m_Owner.transform.TransformDirection( _heading );
					_heading *= m_MoveVelocity.z;					
				}

				_heading.y -= m_Gravity * Time.deltaTime * 10;
				CharacterControllerComponent.Move( _heading * Time.deltaTime );*/

			}

			// RIGIDBODY MOVEMENT 
			else if( RigidbodyReady  )
			{
				if( HandleGroundLevel() )
				{
					m_LastMovePosition = m_MovePosition;

					// CREATURE ROTATION BEGIN
					m_Owner.transform.rotation = HandleStepRotation();
					// CREATURE ROTATION END
					
					// CREATURE MOVE BEGIN
					Vector3 _step_position = HandleStepPosition();
					if( OnUpdateMovePosition != null )
						OnUpdateMovePosition( m_Owner, m_Owner.transform.position, ref _step_position );
					if( ! _rule.UseRootMotion )
						m_Rigidbody.MovePosition( _step_position );
					// CREATURE MOVE END

			
					//m_Rigidbody.AddForce( m_Owner.transform.forward * m_MoveVelocity.z * Time.deltaTime );
				}
			}

			// CUSTOM MOVEMENT (ASTARPATHFINDING) 
			else if( MotionControl == MotionControlType.CUSTOM )
			{
				if( OnUpdateMovePosition != null )
					OnUpdateMovePosition( m_Owner, m_Owner.transform.position, ref m_MovePosition );
			}

			// DEFAULT MOVEMENT WITHOUT EXTERNAL COMPONENTS
			else if( HandleGroundLevel() )
			{
				m_LastMovePosition = m_MovePosition;
				m_LastMoveRotation = m_MoveRotation;

				m_NextMoveRotation = HandleStepRotation();
				m_NextMovePosition = HandleStepPosition();

				UpdateMoveData();

				if( OnUpdateMovePosition != null )
					OnUpdateMovePosition( m_Owner, m_Owner.transform.position, ref m_NextMovePosition );

				// CREATURE ROTATION BEGIN
				if( ! _rule.UseRootMotion )
					m_Owner.transform.rotation = m_NextMoveRotation;
				// CREATURE ROTATION END

				// CREATURE MOVE BEGIN
				if( ! _rule.UseRootMotion )
					m_Owner.transform.position = m_NextMovePosition;
				// CREATURE MOVE END
			}

			CheckDeadlock();

			//m_HasCollision = false;
			/*
			if( _last_transform_position == Vector3.zero )
				_last_transform_position = m_Owner.transform.position;

			m_MovePositionDelta = m_Owner.transform.InverseTransformPoint( _last_transform_position ) / Time.deltaTime;
			m_MoveHeading = m_Owner.transform.TransformDirection( m_MovePositionDelta );
			Debug.Log ( m_Owner.transform.position + " - " + m_NextMovePosition + " --- " + ( m_Owner.transform.position - m_NextMovePosition ) );

			//Vector3 _heading = ( m_Owner.transform.position - _last_transform_position ).normalized;
			//Vector3 _axis = Vector3.Cross( m_Owner.transform.forward, m_MoveHeading );
			m_MoveDirection = Vector3.Angle( m_Owner.transform.forward, m_MoveHeading ) - 180;// 180.0f * (_axis.y < 0 ? -1 : 1);

			Debug.Log ( "m_MoveDirection : " + m_MoveDirection / Time.deltaTime );

			_last_transform_position = m_Owner.transform.position;*/

			if( OnMoveComplete != null )
				OnMoveComplete( m_Owner, m_CurrentTarget );
		}

		private void UpdateMoveData()
		{
			if( _last_transform_position == Vector3.zero )
				_last_transform_position = m_Owner.transform.position;

			m_MovePositionDelta = m_Owner.transform.InverseTransformPoint( _last_transform_position ) / Time.deltaTime;

			
			m_MoveRotationDelta = ( m_Owner.transform.rotation.eulerAngles - _last_transform_rotation.eulerAngles ) * Mathf.Rad2Deg;


			m_MoveHeading = m_Owner.transform.InverseTransformPoint( MovePosition );
			m_MoveDirection = ( Mathf.Atan2( m_MoveHeading.x, m_MoveHeading.z) * Mathf.Rad2Deg );
			if( m_MoveDirection > -0.5f && m_MoveDirection < 0.5f )
				m_MoveDirection = 0;

			//Debug.Log ( m_Owner.transform.position + " - " + m_MovePositionDelta.y + " : " + m_MoveRotationDelta + " --- " + m_MoveHeading + " --- " + m_MoveDirection );

			_last_transform_position = m_Owner.transform.position;
			_last_transform_rotation = m_Owner.transform.rotation;
		}
/*
		public float RealForwardVelocity{
			get{
			}
		}*/

		public float RealAngularVelocity{
			get{ return MathTools.Normalize( m_MoveDirection , - m_MoveAngularVelocity, m_MoveAngularVelocity ); }
		}

		//********************************************************************************
		// DEADLOCK HANDLING
		//********************************************************************************

		private bool m_Deadlocked = false;
		public bool Deadlocked{
			get{ return m_Deadlocked;}
		}

		private List<Vector3> m_DeadlocksCriticalPositions = new List<Vector3>();
		public int DeadlocksCriticalPositions{
			get{ return m_DeadlocksCriticalPositions.Count;}
		}

		private List<Vector3> m_DeadlocksCriticalLoops = new List<Vector3>();
		public int DeadlocksCriticalLoops{
			get{ return m_DeadlocksCriticalLoops.Count;}
		}

		private float m_DeadlockMoveTimer = 0;
		public float DeadlockMoveTimer{
			get{ return m_DeadlockMoveTimer;}
		}

		private float m_DeadlockLoopTimer = 0;
		public float DeadlockLoopTimer{
			get{ return m_DeadlockLoopTimer;}
		}

		
		private int m_DeadlocksCount = 0;
		public int DeadlocksCount{
			get{ return m_DeadlocksCount;}
		}

		private int m_DeadlockLoopsCount = 0;
		public int DeadlockLoopsCount{
			get{ return m_DeadlockLoopsCount;}
		}

		private float m_DeadlocksDistance= 0;
		public float DeadlocksDistance{
			get{ return m_DeadlocksDistance;}
		}

		public DeadlockActionType DeadlockAction = DeadlockActionType.DIE;
		public string DeadlockBehaviour = "";

		public float DeadlockMinMoveDistance = 0.25f;
		public float DeadlockMoveInterval = 2;
		public int DeadlockMoveMaxCriticalPositions = 10;

		public float DeadlockLoopRange = 2f;
		public float DeadlockLoopInterval = 5;
		public int DeadlockLoopMaxCriticalPositions = 10;

		private Vector3 m_DeadlockPosition = Vector3.zero;

		public void ResetDeadlock()
		{
			m_DeadlockMoveTimer = 0;
			m_DeadlockLoopTimer = 0;
			m_Deadlocked = false;
			m_DeadlockPosition = m_Owner.transform.position;
		}

		private bool CheckDeadlock()
		{
			if( UseDeadlockHandling == false )
				return false;

			if( m_MoveVelocity.z == 0 )
			{
				m_DeadlockMoveTimer = 0;
				m_DeadlockLoopTimer = 0;
				return false;
			}

			m_DeadlockMoveTimer += Time.deltaTime;
			m_DeadlockLoopTimer += Time.deltaTime;

			if( m_DeadlockPosition == Vector3.zero )
				m_DeadlockPosition = m_Owner.transform.position;

			if( m_DeadlockMoveTimer >= DeadlockMoveInterval )
			{
				m_DeadlocksDistance = Vector3.Distance( m_Owner.transform.position, m_DeadlockPosition );

				// CHECK DEADLOCK
				if( m_DeadlocksDistance <= DeadlockMinMoveDistance )
				{
					if( m_Deadlocked == false )
						m_DeadlocksCount++;

					m_DeadlocksCriticalPositions.Add( m_Owner.transform.position );

					if( m_DeadlocksCriticalPositions.Count > DeadlockMoveMaxCriticalPositions )
						m_Deadlocked = true;
				}
				else if( m_DeadlocksCriticalPositions.Count > 0 )
					m_DeadlocksCriticalPositions.RemoveAt(0);
				else 
				{
					m_DeadlockPosition = m_Owner.transform.position;
					m_DeadlockMoveTimer = 0;
				}

			
			}

			// CHECK INFINITY LOOP
			if( m_DeadlockLoopTimer >= DeadlockLoopInterval )
			{
				if( m_DeadlocksDistance <= DeadlockLoopRange )
				{
					if( m_Deadlocked == false )
						m_DeadlockLoopsCount++;
					
					m_DeadlocksCriticalLoops.Add( m_Owner.transform.position );
					
					if( m_DeadlocksCriticalLoops.Count > DeadlockLoopMaxCriticalPositions )
						m_Deadlocked = true;
				}
				else if( m_DeadlocksCriticalLoops.Count > 0 )
					m_DeadlocksCriticalLoops.RemoveAt(0);
				else
					m_DeadlockLoopTimer = 0;

			}

			if( m_DeadlockMoveTimer == 0 && m_DeadlocksCriticalPositions.Count == 0 && m_DeadlockLoopTimer == 0 && m_DeadlocksCriticalLoops.Count == 0 )
				m_Deadlocked = false;
				
			return m_Deadlocked;
		
		}



		//********************************************************************************
		// MOVE POSITIONS
		//********************************************************************************


		private Vector3 m_MoveHeading = Vector3.zero;
		public Vector3 MoveHeading{
			get{ return m_MoveHeading; }
		}

		private Vector3 m_MovePositionDelta = Vector3.zero;
		public Vector3 MovePositionDelta{
			get{ return m_MovePositionDelta; }
		}
		
		private Vector3 m_MoveRotationDelta = Vector3.zero;
		public Vector3 MoveRotationDelta{
			get{ return m_MoveRotationDelta; }
		}

		private float m_MoveDirection = 0;
		public float MoveDirection{
			get{ return m_MoveDirection; }
		}


		private Vector3 m_FinalVelocity = Vector3.zero;
		private float m_FinalAngularVelocity = 0;

		/// <summary>
		/// Handles the step rotation.
		/// </summary>
		/// <returns>The step rotation.</returns>
		private Quaternion HandleStepRotation()
		{
			Quaternion _step_rotation = m_Owner.transform.rotation;

			if( m_CurrentTarget.TargetMovePositionDistanceTo( m_Owner.transform.position ) < m_CurrentTarget.Move.StopDistance )
				m_FinalAngularVelocity = 0;
			else if( m_CurrentTarget.TargetMovePositionDistanceTo( m_Owner.transform.position ) < m_MoveVelocity.z && m_MoveAngularVelocity < m_MoveVelocity.z )
				m_FinalAngularVelocity = m_MoveAngularVelocity + m_FinalAngularVelocity + (m_MoveVelocity.z/100);
			else
				m_FinalAngularVelocity = m_MoveAngularVelocity;
			
			_step_rotation = Quaternion.Slerp ( m_Owner.transform.rotation, m_MoveRotation, m_FinalAngularVelocity * Time.deltaTime );


			return _step_rotation;
		}

		Vector3 _last_transform_position = Vector3.zero;
		Quaternion _last_transform_rotation = Quaternion.identity;

		/// <summary>
		/// Handles the step position.
		/// </summary>
		/// <returns>The step position.</returns>
		/// <description>Step position defines the move during a frame update</description>
		private Vector3 HandleStepPosition()
		{
			Vector3 _step_position = m_Owner.transform.position;

			if(  m_CurrentTarget.Move.StopDistanceZoneRestricted && m_CurrentTarget.TargetMovePositionReached( m_Owner.transform.position ) )
			{
				Vector3 _heading = m_Owner.transform.position - m_CurrentTarget.TargetMovePosition;
				_step_position = GraphicTools.GetAnglePosition( m_CurrentTarget.TargetMovePosition, GraphicTools.GetOffsetAngle( _heading ), m_CurrentTarget.Move.StopDistance + 0.5f );
			}
			else
			{
				m_FinalVelocity = m_MoveVelocity;

				if( m_CurrentTarget.TargetMovePositionDistanceTo( m_Owner.transform.position ) < ( m_CurrentTarget.Move.StopDistance / 2) )
					m_FinalVelocity = Vector3.zero;
				
				_step_position += m_Owner.transform.TransformDirection( Vector3.forward )* m_FinalVelocity.z * Time.deltaTime;
				_step_position += m_Owner.transform.TransformDirection( Vector3.right )* m_FinalVelocity.x * Time.deltaTime;
				_step_position += m_Owner.transform.TransformDirection( Vector3.up )* m_FinalVelocity.y * Time.deltaTime;	
			}

			//Debug.Log( m_CurrentTarget.TargetMovePositionDistanceTo ( m_Owner.transform.position ) + " - " + m_MoveVelocity.z + " - " + m_MoveAngularVelocity );

			//m_MovePositionDelta = m_Owner.transform.InverseTransformPoint( _step_position ) / Time.deltaTime;

			///m_MovePositionDelta = m_Owner.transform.position - _step_position;
			//m_MovePositionDelta = m_Owner.transform.InverseTransformPoint( _step_position ) / Time.deltaTime;
			//Debug.Log ( m_MovePositionDelta );

			return _step_position;
		}


		//********************************************************************************
		// OBSTACLE AVOIDANCE HANDLING
		//********************************************************************************



		//private Vector3 m_AvoidHeading = Vector3.zero;
		public int ObstacleAvoidanceCheckCycles = 10;

		private List<Vector3> m_Path = new List<Vector3> (); 
		public List<Vector3> Path
		{
			get{ return m_Path; }
		}


/*
		private Vector3 CreatePath( Vector3 _position )
		{
			m_Path.Clear();
			Transform _transform = m_Owner.transform;
			float _distance = ObstacleAvoidanceCheckDistance;
			float _segment = 10;

			LayerMask _mask = GroundLayerMask;
			RaycastHit _hit;
			
			Vector3 _pos = Tools.GetDirectionPosition( _transform, 0, _distance );
			
			_pos.y += DownRaycastOffset;
			
			for( int i = ObstacleAvoidanceCheckDegrees ; i <= 360 ; i += ObstacleAvoidanceCheckDegrees )
			{
				_pos = Tools.GetDirectionPosition( _transform, i, _distance );
				_pos.y += DownRaycastOffset;
				if( Physics.Raycast( _pos, Vector3.down, out _hit, 5000, _mask ))
				{
					Debug.DrawRay( _pos, Vector3.down * 5000 , Color.yellow);
					
					float _angle = Vector3.Angle( _hit.normal, Vector3.up );
					if( _angle <= MaxSlopeAngle )
					{
						Debug.DrawRay( _pos, Vector3.down * 5000 , Color.green);
						m_Path.Add( _pos );
						break;
					}
				}
			}
		}*/
		
		public int ObstacleAvoidanceCheckAngle = 90;
		public int ObstacleAvoidanceScanningAngle = 10;
		public int ObstacleAvoidanceScanningRange = 15;
		public float ObstacleAvoidanceCheckDistanceMax = 50;

		private Vector3 HandleObstacleAvoidance( Vector3 _position )
		{
			float _vertical_offset = 0;

			Transform _transform = m_Owner.transform;
			
			Vector3 _avoid_position = _position;
			Vector3 _transform_pos = _transform.position;
			Vector3 _move_pos = _position;
			float _distance = ObstacleAvoidanceScanningRange;
			float _slope_distance = 5;

			_transform_pos.y = _transform_pos.y + _vertical_offset;
			_move_pos.y = _transform_pos.y;
			
			if( UseSlopeLimits || AvoidWater )
			{
				LayerMask _mask = GroundLayerMask;
				RaycastHit _hit;

				Vector3 _pos = GraphicTools.GetDirectionPosition( _transform, 0, _slope_distance );

				_pos.y += RaycastOffset;		
				if( Physics.Raycast( _pos, Vector3.down, out _hit, Mathf.Infinity, _mask ))
				{
					//Debug.DrawRay( _pos, Vector3.down * 5000 , Color.red);

					Vector3 _dir = ( _hit.point - _transform.position ).normalized;
					float _path_angle = Vector3.Angle( _dir, Vector3.down ) - 90;
					float _surface_angle = Vector3.Angle( _hit.normal, Vector3.up );

					if( ( MaxSlopeAngle > 0 && Mathf.Abs( _surface_angle ) > MaxSlopeAngle ) ||
					   ( MaxWalkableSlopeAngle > 0 && Mathf.Abs( _path_angle ) > MaxWalkableSlopeAngle ) || 
					   ( AvoidWater && _hit.transform.gameObject.layer == LayerMask.NameToLayer( "Water" ) ) )
					{
						for( int i = ObstacleAvoidanceScanningAngle ; i <= 360 ; i += ObstacleAvoidanceScanningAngle )
						{
							_pos = GraphicTools.GetDirectionPosition( _transform, i, _slope_distance );
							_pos.y += RaycastOffset;
							if( Physics.Raycast( _pos, Vector3.down, out _hit, Mathf.Infinity, _mask ))
							{
								Debug.DrawRay( _pos, Vector3.down * 5000 , Color.yellow);

								_dir = ( _hit.point - _transform.position ).normalized;
								_path_angle = Vector3.Angle( _dir, Vector3.down ) - 90;
								_surface_angle = Vector3.Angle( _hit.normal, Vector3.up );

								bool _walkable = true;

								if( MaxSlopeAngle > 0 && Mathf.Abs( _surface_angle ) > MaxSlopeAngle )
									_walkable = false;

								if( MaxWalkableSlopeAngle > 0 && _walkable && Mathf.Abs( _path_angle ) > MaxWalkableSlopeAngle )
									_walkable = false;

								if( AvoidWater && _hit.transform.gameObject.layer == LayerMask.NameToLayer( "Water" ) )
									_walkable = false;
				
								if( _walkable )
								{
									//Debug.DrawRay( _pos, Vector3.down * 5000 , Color.green);
									_avoid_position = _hit.point;
									break;
								}
							}
							else
								m_VerticalRaycastOffset += 0.25f;
						}
					}
				}
				else
					m_VerticalRaycastOffset += 0.25f;
			}
				
			if( ObstacleCheck == ObstacleCheckType.BASIC )
			{
				LayerMask _mask = ObstacleLayerMask;				
				RaycastHit _hit;

				//Debug.DrawRay( _transform_pos, _transform.forward * 5 , Color.yellow);

				//check for forward raycast				
				if( Physics.Raycast( _transform_pos, _transform.forward, out _hit, _distance, _mask ))
				{
					if( _hit.transform != _transform )				
					{
						//Debug.DrawRay( _transform_pos, _transform.forward * _distance , Color.yellow);
		
						for( int i = ObstacleAvoidanceScanningAngle ; i <= 360 ; i += ObstacleAvoidanceScanningAngle )
						{
							Vector3 _pos = GraphicTools.GetDirectionPosition( _transform, i, _distance ); 
							
							if( ! Physics.Linecast( _transform_pos, _pos, _mask ) )
							{
								_avoid_position = _pos;
								Debug.DrawLine( _transform_pos, _pos, Color.green);
								break;
							}
							else
								Debug.DrawLine( _transform_pos, _pos, Color.red);
						}

						/*
						// target right hand
						else if( Tools.AngleDirection( _transform.forward, Vector3.up, _desired_heading ) > 0 || _avoid_position == _position )
						{
							_counter = 0;
							for( int i = 360 ; i >= 360-ObstacleAvoidanceCheckAngle ; i -= ObstacleAvoidanceCheckDegrees )
							{
								_counter++;
								Vector3 _pos = Tools.GetDirectionPosition( _transform, i, _distance + _counter ); 
								
								if( ! Physics.Linecast( _transform_pos, _pos, _mask ) )
								{
									_avoid_position = _pos;
									Debug.DrawLine( _transform_pos, _pos, Color.green);
									break;
								}
								else
									Debug.DrawLine( _transform_pos, _pos, Color.red);
							}
						}*/
					}
				}
			}
			
			return _avoid_position;
		}
		/*
		public Vector3 GetFreeGroundPosition( Vector3 _position )
		{
			LayerMask _mask = GroundLayerMask;
			RaycastHit _hit;

			Vector3 _new_pos = _position;
			Vector3 _pos = _position;	

			_pos.y += RaycastOffset;
			if( Physics.Raycast( _pos, Vector3.down, out _hit, 5000, _mask ) == false )
			{
				float _distance = 1f;
				for( int i = 0 ; i <= 3600 ; i += 36 )
				{
					_new_pos = GraphicTools.GetAnglePosition( _position, i, _distance );
					_pos = _new_pos;
					_pos.y += RaycastOffset;					
					if( Physics.Raycast( _pos, Vector3.down, out _hit, 5000, _mask ))
					{
						break;
					}
					else
						m_VerticalRaycastOffset += 0.25f;
					
					_distance += 0.5f;
				}
			}	
			else
				m_VerticalRaycastOffset += 0.25f;
			
			return _new_pos;
		}
	

		private bool MoveDirectionSideIsFree( Transform _transform, Vector3 _transform_pos, Vector3 _desired_heading, LayerMask _mask )
		{
			bool _side_is_free = true;
			float _side_scan_distance = 5;

			RaycastHit _hit;

			// target left hand
			if( GraphicTools.AngleDirection( _transform.forward, Vector3.up, _desired_heading ) < 0 )
			{
				//check right side raycast
				if(Physics.Raycast(_transform_pos, -_transform.right, out _hit, _side_scan_distance, _mask)){
					if(_hit.transform != _transform)
					{
						//Debug.DrawRay(_transform_pos, _transform.right * -_side_scan_distance, Color.red);
						_side_is_free = false;
					}
				}
			}
			// target right hand
			else if( GraphicTools.AngleDirection( _transform.forward, Vector3.up, _desired_heading ) > 0 )
			{
				//check right side raycast
				if(Physics.Raycast(_transform_pos, _transform.right, out _hit, _side_scan_distance, _mask)){
					if(_hit.transform != _transform)
					{
						//Debug.DrawRay(_transform_pos, _transform.right * _side_scan_distance, Color.red);
						_side_is_free = false;
					}
				}
			}

			return _side_is_free;
		}
	*/
		/*
		public float ObstacleAvoidanceHeadingCorrection = 25;
		public float ObstacleAvoidanceTimeIn = 0.1f;
		public float ObstacleAvoidanceTimeOut = 0.25f;

		private float m_ObstacleAvoidanceTimer = 0;
	
		private Vector3 HandleBasicObstacleAvoidance( Vector3 _position )
		{
			float _vertical_offset = 1;
			float _side_offset = 2;
			Transform _transform = m_Owner.transform;

			Vector3 _transform_pos = _transform.position;
			Vector3 _move_pos = _position;

			_transform_pos.y = _transform_pos.y + _vertical_offset;
			_move_pos.y = _transform_pos.y;
	
			//directional vector to our target
			Vector3 _desired_heading = ( _move_pos - _transform_pos ).normalized;
			Vector3 _avoid_heading = _desired_heading;

			if( m_AvoidHeading == Vector3.zero )
				m_AvoidHeading = _desired_heading;

			m_ObstacleAvoidanceTimer += Time.deltaTime;

			if( ObstacleCheck == ObstacleCheckType.BASIC )
			{

				LayerMask _mask = ObstacleLayerMask;

				RaycastHit _hit;

				//check for forward raycast

				if( Physics.Raycast( _transform_pos, _transform.forward, out _hit, 20, _mask )){
					if(_hit.transform != _transform)				
					{
						//Debug.DrawRay( _transform_pos, _transform.forward *20, Color.yellow);
						Vector3 _normal = _hit.normal;
						_normal.y = 0;
						_avoid_heading += _normal * ObstacleAvoidanceHeadingCorrection;
					}
				}
				
				//build the positions for left/right ray origin point
				Vector3 _left_ray = _transform_pos + (_transform.right *-_side_offset);
				Vector3 _right_ray = _transform_pos + (_transform.right *_side_offset);

				//check left raycast
				if(Physics.Raycast(_left_ray, _transform.forward, out _hit, 20, _mask)){
					if(_hit.transform != _transform)
					{
						//Debug.DrawRay(_left_ray, _transform.forward *20, Color.red);
						Vector3 _normal = _hit.normal;
						_normal.y = 0;
						_avoid_heading += _normal * ObstacleAvoidanceHeadingCorrection;
					}
				}
				
				//check right raycast
				if(Physics.Raycast(_right_ray, _transform.forward, out _hit, 20, _mask)){
					if(_hit.transform != _transform)
					{
						//Debug.DrawRay(_right_ray, _transform.forward *20, Color.red);
						Vector3 _normal = _hit.normal;
						_normal.y = 0;
						_avoid_heading += _normal * ObstacleAvoidanceHeadingCorrection;
					}
				}

				/*
				_is_free = MoveDirectionSideIsFree( _transform, _transform_pos, _desired_heading, _mask );

				if( _avoid_heading != _desired_heading )
					m_AvoidHeading = _avoid_heading;
				else if( m_ObstacleAvoidanceTimer > 1f  )
				{
					m_AvoidHeading = Vector3.zero;
					m_ObstacleAvoidanceTimer = 0;
				}
*/
		/*
				Vector3 _velocity = Vector3.zero;
				if( m_AvoidHeading != _avoid_heading )
					m_AvoidHeading = Vector3.SmoothDamp( m_AvoidHeading, _avoid_heading, ref _velocity, ObstacleAvoidanceTimeIn);
				else
					m_AvoidHeading = Vector3.SmoothDamp( m_AvoidHeading, _desired_heading, ref _velocity, ObstacleAvoidanceTimeOut);
			}



		//	if( _avoid_heading != Vector3.zero )
		//		_desired_heading = _avoid_heading;

			return m_AvoidHeading;
		}
*/
/*
		private Vector3 HandleFreePosition( Vector3 _position )
		{
			if( UseFreePosition == false )
				return _position;

			Vector3 _creature_position = m_Owner.transform.position;
			Vector3 _free_position = _position;
			float _distance = Vector3.Distance( _creature_position, _free_position );

			Collider _creature_collider = m_Owner.transform.root.GetComponentInChildren<Collider>();

			// consider object size because the cast may not intersect the own collider  
			float _creature_radius = 2;
			if( _creature_collider != null && _creature_collider.bounds.extents != Vector3.zero )
				_creature_radius = _creature_collider.bounds.extents.magnitude + 0.25f;

			_creature_radius = _creature_radius/(_distance/100)/100;
			_creature_position.y += _creature_radius;			
			_free_position.y += _creature_radius;
			
			_creature_position = Vector3.Lerp( _creature_position, _free_position, _creature_radius );

			RaycastHit _hit;
			if( Physics.Linecast( _creature_position, _free_position, out _hit ) ) 
			{
				if( _hit.transform.IsChildOf( m_Owner.transform ) )
					Debug.Log ( "Warning: Free Position Linecast of '" + m_Owner.transform.name + "' hits own collider!" );
				else
				{
					_free_position = Vector3.Lerp( _creature_position, _hit.point, 0.8f );
				}
			} 

			return _free_position;
		}
*/
		//private bool m_HasCollision = false;
		public void HandleCollision( Collision _collision )
		{
			if( _collision.gameObject.GetComponent<Terrain>() == null || GraphicTools.IsInLayerMask( _collision.gameObject, GroundLayerMask ) == false )
			{
				//m_HasCollision = true;
			}
		}

		//********************************************************************************
		// GROUND HANDLING
		//********************************************************************************

		private bool m_IsGrounded = true;
		public bool IsGrounded{
			get{ return m_IsGrounded;}
		}
		
		private float m_GroundeLevel = 0;
		private float m_FallSpeed = 9.8f;
		private float m_Altitude = 0;




		/// <summary>
		/// Handles the ground level.
		/// </summary>
		/// <returns><c>true</c>, if ground level was handled, <c>false</c> otherwise.</returns>
		private bool HandleGroundLevel()
		{
			if( GroundCheck == GroundCheckType.NONE || ! UseInternalGravity )
				return true;
			
			Vector3 _position = m_Owner.transform.position;
			
			if( GroundCheck == GroundCheckType.RAYCAST )
			{
				LayerMask _mask = GroundLayerMask;
				RaycastHit _hit;
				Vector3 _pos = _position + ( m_Owner.transform.TransformDirection( Vector3.back ) * m_HorizontalRaycastOffset );
				_pos.y += RaycastOffset;
				if (Physics.Raycast( _pos, Vector3.down, out _hit, Mathf.Infinity, _mask ) )
				{
					if( m_Owner != _hit.collider.gameObject )
						m_GroundeLevel = ( m_GroundeLevel + _hit.point.y ) /2;
					else
						m_HorizontalRaycastOffset += 0.1f;
					
					//Debug.DrawRay( _pos,Vector3.down * 10000,Color.cyan); 
				}
				else
					m_VerticalRaycastOffset += 0.25f;
			}
			else 
				m_GroundeLevel = Terrain.activeTerrain.SampleHeight( _position );
			
			_position.y = m_GroundeLevel;
			
			m_Altitude = m_Owner.transform.position.y + GroundCheckBaseOffset - _position.y;
			
			if( m_Altitude < 0.5f )
			{
				_position.y += GroundCheckBaseOffset * (-1);
				m_Owner.transform.position = _position;
				m_IsGrounded = true;
				
				m_FallSpeed = 0;
				
			}
			else
			{
				m_FallSpeed += Gravity * Time.deltaTime; // not correct but looks good  -9.81 m/s^2 * time
				
				m_Owner.transform.position += m_Owner.transform.TransformDirection( Vector3.down ) * m_FallSpeed * Time.deltaTime;
				m_IsGrounded = false;
			}
			
			return m_IsGrounded;
		}

		public string UpdateGroundTextureName()
		{
			return GetGroundTextureName( m_Owner.transform.position );
		}

		private string m_CurrentGroundTextureName;
		private Terrain m_SurfaceTerrain;
		private TerrainData m_SurfaceTerrainData;

		/// <summary>
		/// Gets the ground level.
		/// </summary>
		/// <returns>The ground level.</returns>
		/// <param name="position">Position.</param>
		private string GetGroundTextureName( Vector3 position )
		{
			if( GroundCheck != GroundCheckType.RAYCAST )
				return "";

			if( m_SurfaceTerrain == null )
			{
				m_SurfaceTerrain = Terrain.activeTerrain;

				if( m_SurfaceTerrain != null )
					m_SurfaceTerrainData = m_SurfaceTerrain.terrainData;
			}

			LayerMask _mask = GroundLayerMask;
			RaycastHit _hit;

			Vector3 _pos = m_Owner.transform.position;
			_pos.y += 0.1f;
			if( Physics.Raycast( _pos, Vector3.down, out _hit, Mathf.Infinity, _mask ) )
			{
				if( _hit.transform.GetComponent<Terrain>() != null  )
				{
					int _texture_id = GraphicTools.GetMainTerrainTexture( m_Owner.transform.position, _hit.transform.GetComponent<Terrain>());
					
					if( m_SurfaceTerrainData.splatPrototypes.Length > 0 )
						m_CurrentGroundTextureName = m_SurfaceTerrainData.splatPrototypes[_texture_id].texture.name;
				}
				else if( _hit.collider.gameObject.GetComponent<MeshRenderer>() != null )
				{
					MeshRenderer _renderer = _hit.collider.gameObject.GetComponent<MeshRenderer>();
					
					if( _renderer.material != null && _renderer.material.mainTexture != null  )
						m_CurrentGroundTextureName = _renderer.material.mainTexture.name;
				}

			}

			return m_CurrentGroundTextureName;
		}
		
		/// <summary>
		/// Gets the ground level.
		/// </summary>
		/// <returns>The ground level.</returns>
		/// <param name="position">Position.</param>
		private float GetGroundLevel( Vector3 position )
		{
			if( GroundCheck == GroundCheckType.NONE )
				return position.y;

			if( GroundCheck == GroundCheckType.RAYCAST )
			{
				LayerMask _mask = GroundLayerMask;
				RaycastHit hit;
				Vector3 _pos = position;
				_pos.y += RaycastOffset;
				if (Physics.Raycast( _pos, Vector3.down, out hit, Mathf.Infinity, _mask ) )
					position.y = hit.point.y;
				else
					m_VerticalRaycastOffset += 0.25f;
			}
			else 
				position.y = Terrain.activeTerrain.SampleHeight( position );
			
			return position.y;
		}




		private Vector3 m_FrontLeftPosition = Vector3.zero;
		public Vector3 FrontLeftPosition{
			get{return m_FrontLeftPosition;}
		}

		private Vector3 m_FrontRightPosition = Vector3.zero;
		public Vector3 FrontRightPosition{
			get{return m_FrontRightPosition;}
		}

		private Vector3 m_BackLeftPosition = Vector3.zero;
		public Vector3 BackLeftPosition{
			get{return m_BackLeftPosition;}
		}

		private Vector3 m_BackRightPosition = Vector3.zero;
		public Vector3 BackRightPosition{
			get{return m_BackRightPosition;}
		}

		private Vector3 m_FrontLeftPositionGround = Vector3.zero;
		public Vector3 FrontLeftPositionGround{
			get{return m_FrontLeftPositionGround;}
		}
		
		private Vector3 m_FrontRightPositionGround = Vector3.zero;
		public Vector3 FrontRightPositionGround{
			get{return m_FrontRightPositionGround;}
		}
		
		private Vector3 m_BackLeftPositionGround = Vector3.zero;
		public Vector3 BackLeftPositionGround{
			get{return m_BackLeftPositionGround;}
		}
		
		private Vector3 m_BackRightPositionGround = Vector3.zero;
		public Vector3 BackRightPositionGround{
			get{return m_BackRightPositionGround;}
		}

		/// <summary>
		/// Handles the ground orientation.
		/// </summary>
		public Quaternion HandleGroundOrientation( Quaternion _rotation )
		{
			if( CurrentBody.Type == GroundOrientationType.DEFAULT )
				return _rotation;

			float _roll_angle = 0;

			if( CurrentBody.UseLeaningTurn )
			{
				Vector3 _heading = m_MovePosition - m_Owner.transform.position;
				float _angle_direction = GraphicTools.AngleDirection( m_Owner.transform.forward, m_Owner.transform.up, _heading ) * (-10);

				_roll_angle = m_MoveVelocity.z * _angle_direction/180 * 100;

				_roll_angle = _roll_angle * CurrentBody.LeanAngleMultiplier * 2;

				// limits
				if( _roll_angle > CurrentBody.MaxLeanAngle )
					_roll_angle = CurrentBody.MaxLeanAngle;

				if( _roll_angle < - CurrentBody.MaxLeanAngle )
					_roll_angle = - CurrentBody.MaxLeanAngle;

				// prepare euler angle 
				if( _roll_angle < 0 )
					_roll_angle = 360 + _roll_angle;

				//Debug.Log ( _roll_angle );
			}

			if( CurrentBody.Type == GroundOrientationType.BIPED )
			{
				_rotation = Quaternion.Euler( 0, _rotation.eulerAngles.y, _roll_angle );
			}
			else if( CurrentBody.Type == GroundOrientationType.AVIAN )
			{
				_rotation = Quaternion.Euler( 0, _rotation.eulerAngles.y, _roll_angle );
			}
			else
			{

				Vector3 ray = Vector3.down;//m_Owner.transform.TransformDirection(Vector3.down); //Vector3.down * 100;//
				Vector3 pos = m_Owner.transform.position;
				Vector3 _normal = Vector3.zero;

				if( CurrentBody.Width == 0 )
					CurrentBody.Width = ( m_Owner.GetComponentInChildren<Renderer>().bounds.size.x / m_Owner.transform.lossyScale.x );
				
				if( CurrentBody.Length == 0 )
					CurrentBody.Length = ( m_Owner.GetComponentInChildren<Renderer>().bounds.size.z / m_Owner.transform.lossyScale.z );
				
				if( CurrentBody.Height == 0 )
					CurrentBody.Height = ( m_Owner.GetComponentInChildren<Renderer>().bounds.size.y / m_Owner.transform.lossyScale.y );
				
				
				m_FrontLeftPosition = m_Owner.transform.TransformPoint(new Vector3( - (CurrentBody.Width/2)+CurrentBody.WidthOffset, 0, (CurrentBody.Length/2)+CurrentBody.DepthOffset ));
				m_FrontRightPosition = m_Owner.transform.TransformPoint(new Vector3( (CurrentBody.Width/2)+CurrentBody.WidthOffset, 0, (CurrentBody.Length/2)+CurrentBody.DepthOffset ));
				m_BackLeftPosition = m_Owner.transform.TransformPoint(new Vector3( - (CurrentBody.Width/2)+CurrentBody.WidthOffset, 0, - (CurrentBody.Length/2)+CurrentBody.DepthOffset ));
				m_BackRightPosition = m_Owner.transform.TransformPoint(new Vector3( (CurrentBody.Width/2)+CurrentBody.WidthOffset, 0, - (CurrentBody.Length/2)+CurrentBody.DepthOffset ));
			
				if( GroundCheck == GroundCheckType.RAYCAST )
				{
					if( CurrentBody.UseAdvanced == false )
					{
						LayerMask _mask = GroundLayerMask;

						RaycastHit hit;
						if( Physics.Raycast( pos, ray ,out hit, Mathf.Infinity, _mask ) )
						{
							if( hit.collider.gameObject.GetComponent<Terrain>() ||  hit.collider.gameObject.GetComponent<MeshFilter>() )
								_normal = hit.normal;
						}
					}
					else
					{
						RaycastHit _hit_back_left;
						RaycastHit _hit_back_right;
						RaycastHit _hit_front_left;
						RaycastHit _hit_front_right;

						LayerMask _mask = GroundLayerMask;						
						if( Physics.Raycast( m_FrontLeftPosition + Vector3.up, ray, out _hit_front_left, Mathf.Infinity, _mask ) )
							m_FrontLeftPositionGround = _hit_front_left.point;
						if( Physics.Raycast( m_FrontRightPosition + Vector3.up, ray, out _hit_front_right, Mathf.Infinity, _mask ) )
							m_FrontRightPositionGround = _hit_front_right.point;
						if( Physics.Raycast( m_BackLeftPosition + Vector3.up, ray, out _hit_back_left, Mathf.Infinity, _mask ) )
							m_BackLeftPositionGround = _hit_back_left.point;
						if( Physics.Raycast( m_BackRightPosition + Vector3.up, ray, out _hit_back_right, Mathf.Infinity, _mask ) )
							m_BackRightPositionGround = _hit_back_right.point;
						
						_normal = (Vector3.Cross( m_BackRightPositionGround - Vector3.up, m_BackLeftPositionGround - Vector3.up) +
						           Vector3.Cross( m_BackLeftPositionGround - Vector3.up, m_FrontLeftPositionGround - Vector3.up) +
						           Vector3.Cross( m_FrontLeftPositionGround - Vector3.up, m_FrontRightPositionGround - Vector3.up) +
						           Vector3.Cross( m_FrontRightPositionGround - Vector3.up, m_BackRightPositionGround - Vector3.up)
						           ).normalized;
					}
				}
				else
				{
					if( CurrentBody.UseAdvanced == false )
					{
						pos.x =  pos.x - Terrain.activeTerrain.transform.position.x;
						pos.z =  pos.z - Terrain.activeTerrain.transform.position.z;
			
						TerrainData _terrain_data = Terrain.activeTerrain.terrainData;
						_normal = _terrain_data.GetInterpolatedNormal( pos.x/_terrain_data.size.x, pos.z/_terrain_data.size.z );
					}
					else
					{
						m_BackRightPositionGround = m_BackRightPosition;
						m_BackRightPositionGround.y = Terrain.activeTerrain.SampleHeight( m_BackRightPositionGround );

						m_BackLeftPositionGround = m_BackLeftPosition;
						m_BackLeftPositionGround.y = Terrain.activeTerrain.SampleHeight( m_BackLeftPositionGround );

						m_FrontLeftPositionGround = m_FrontLeftPosition;
						m_FrontLeftPositionGround.y = Terrain.activeTerrain.SampleHeight( m_FrontLeftPositionGround );

						m_FrontRightPositionGround = m_FrontRightPosition;
						m_FrontRightPositionGround.y = Terrain.activeTerrain.SampleHeight( m_FrontRightPositionGround );


						_normal = (Vector3.Cross( m_BackRightPositionGround - Vector3.up, m_BackLeftPositionGround - Vector3.up) +
						           Vector3.Cross( m_BackLeftPositionGround - Vector3.up, m_FrontLeftPositionGround - Vector3.up) +
						           Vector3.Cross( m_FrontLeftPositionGround - Vector3.up, m_FrontRightPositionGround - Vector3.up) +
						           Vector3.Cross( m_FrontRightPositionGround - Vector3.up, m_BackRightPositionGround - Vector3.up)
						           ).normalized;
					}
				}	

				//var pitch_angle = Vector3.Angle(Vector3.right, _normal)-90;
				//var sss_angle = Vector3.Angle(Vector3.forward, _normal)-90;

				_rotation = Quaternion.FromToRotation( Vector3.up , _normal ) * _rotation;
	
				if( CurrentBody.Type == GroundOrientationType.QUADRUPED )
					_rotation = Quaternion.Euler( _rotation.eulerAngles.x, _rotation.eulerAngles.y, _roll_angle );

			}

			return _rotation;
		}

		//********************************************************************************
		// MOVE POSITION HANDLING
		//********************************************************************************
		
		private Quaternion GetMoveRotation()
		{
			Vector3 _move_pos = m_MovePosition;
			Vector3 _creature_pos = m_Owner.transform.position;
			
			_move_pos.y = 0;
			_creature_pos.y = 0;
			
			Vector3 _heading = ( _move_pos - _creature_pos ).normalized;
			
			if( CurrentMove.ViewingDirection == ViewingDirectionType.CENTER )
				_heading = m_CurrentTarget.TargetPosition - _creature_pos;
			else if( CurrentMove.ViewingDirection == ViewingDirectionType.OFFSET )
				_heading = m_CurrentTarget.TargetOffsetPosition - _creature_pos;
			else if( CurrentMove.ViewingDirection == ViewingDirectionType.MOVE )
				_heading = m_CurrentTarget.TargetMovePosition - _creature_pos;
			else if( CurrentMove.ViewingDirection == ViewingDirectionType.POSITION )
				_heading = CurrentMove.ViewingDirectionPosition - _creature_pos;

			Quaternion _rotation = m_Owner.transform.rotation;

			if( CurrentMove.ViewingDirection != ViewingDirectionType.NONE )
			{
				if( _heading != Vector3.zero )
					_rotation = Quaternion.LookRotation ( _heading );

				_rotation = Quaternion.Euler( 0, _rotation.eulerAngles.y, 0 );
			}

			return HandleGroundOrientation( _rotation );
		}
	
		private Vector3 GetMovePosition()
		{
			Vector3 _position = m_MovePosition;

			if( MovePositionUpdateRequired )
			{
				switch( CurrentMove.Type )
				{
					case MoveType.AVOID: // AVOID MOVE
						_position = GetAvoidPosition();	
						break;
					case MoveType.ESCAPE: // ESCAPE MOVE
						_position = GetEscapePosition();	
						break;
					case MoveType.ORBIT: // ORBIT MOVE
						_position = GetOrbitPosition();
						break;
					case MoveType.DETOUR: // DETOUR MOVE
						_position = GetDetourPosition();
						break;
					case MoveType.RANDOM: // RANDOM MOVE
						_position = GetRandomPosition();
						break;
					default: // DEFAULT AND CUSTOM MOVE
						_position = m_CurrentTarget.TargetMovePosition;
						break;
				}

				_position = ModulateMovePosition( m_Owner.transform.position, _position );
				
			}

			_position = HandleObstacleAvoidance( _position );


			/*
			 if( NavMeshAgentReady )
			 {
			 	NavMeshHit _hit;

				NavMesh.SamplePosition( _position, out _hit, CurrentMove.MoveSegmentLengthMax, 1);
				_position = _hit.position;
			}*/

			return _position;
		}



		/// <summary>
		/// Gets the modulated move position.
		/// </summary>
		/// <returns>The modulated move position.</returns>
		/// <param name="_owner_pos">_owner_pos.</param>
		/// <param name="_target_pos">_target_pos.</param>
		private Vector3 ModulateMovePosition ( Vector3 _owner_position, Vector3 _desired_move_position ) {

			Vector3 _position = _desired_move_position;
			
			if( CurrentMove.MoveSegmentLength > 0 )
			{
				float _target_distance = Vector3.Distance(_owner_position, _desired_move_position);
				float _segment_legth = CurrentMove.GetMoveSegmentLength();
				
				if( _target_distance > 0 && _segment_legth < _target_distance )
				{
					float _f = _segment_legth/_target_distance;
					
					if( _f == 0 )
						_f = 0.1f;
					
					_position = Vector3.Lerp( _owner_position, _desired_move_position, _f);
					
					if( CurrentMove.MoveDeviationLength > 0 && CurrentMove.MoveDeviationVariance > 0 )
					{
						Vector3 _forward = _position - _owner_position;
						Vector3 _right = Vector3.Cross( Vector3.up, _forward ).normalized;
						_position = _position + ( _right * CurrentMove.GetMoveDeviationVariance() );
					}
				}
			}
			
			//_position = HandleFreePosition( _position );
			_position.y = GetGroundLevel( _position );
			
			return _position;

		}

		
		//--------------------------------------------------

		public bool MovePositionUpdateRequired{
			get{
				if( MovePositionReached || TargetMovePositionReached || m_MovePosition == Vector3.zero || m_Deadlocked )
				return true;
			else
				return false;
			}
		}

		//--------------------------------------------------

		public bool MovePositionReached{
			get{
				if( CurrentMove.MoveSegmentLength == 0 || CurrentMove.MoveStopDistance == 0 || MovePositionDistance <= CurrentMove.MoveStopDistance )
					return true;
				else
					return false;
			}
		}

		//--------------------------------------------------
		
		public bool TargetMovePositionReached{
			get{
				// HANDLE TARGET MOVE POSITION
				if( m_CurrentTarget != null && m_CurrentTarget.IsValid && m_CurrentTarget.TargetMovePositionReached( m_Owner.transform.position ) )
					return true;
				else
					return false;
			}
		}
		
		//--------------------------------------------------

		public float MovePositionDistance{
			get{
				if( m_Owner == null )
					return 0;
				
				Vector3 pos_1 = MovePosition;
				Vector3 pos_2 = m_Owner.transform.position;
				
				if( CurrentMove.MoveIgnoreLevelDifference )
				{
					pos_1.y = 0;
					pos_2.y = 0;
				}
				
				return Vector3.Distance(pos_1, pos_2);
			}
		}

		//--------------------------------------------------

		private Vector3 GetDetourPosition()
		{
			if( DetourPositionReached() )
				m_DetourComplete = true;
			
			if( m_DetourComplete )
				return m_CurrentTarget.TargetMovePosition;
			else
				return CurrentMove.Detour.Position;
		}

		//--------------------------------------------------
		public bool DetourPositionReached()
		{
			if( CurrentMove.MoveStopDistance == 0 || DetourPositionDistance() <=  CurrentMove.MoveStopDistance  )
				return true;
			else
				return false;
		}
		
		//--------------------------------------------------
		public float DetourPositionDistance()
		{
			if( m_Owner == null || CurrentMove.Detour.Position == Vector3.zero )
				return 0;
			
			Vector3 pos_1 = CurrentMove.Detour.Position;
			Vector3 pos_2 = m_Owner.transform.position;
			
			if( CurrentMove.MoveIgnoreLevelDifference )
			{
				pos_1.y = 0;
				pos_2.y = 0;
			}
			
			return Vector3.Distance(pos_1, pos_2);
		}

		//--------------------------------------------------


		/// <summary>
		/// Gets a randomized position.
		/// </summary>
		/// <returns>The random position.</returns>
		private Vector3 GetRandomPosition()
		{
			return GraphicTools.GetRandomPosition( m_Owner.transform.position, CurrentMove.GetMoveSegmentLength() ); 
		}

		//private float m_CurrentAvoidAngle = 0;
		private float m_CreatureRelatedDirectionAngle = 0;
		private Vector3 m_AvoidMovePosition = Vector3.zero;
		public Vector3 AvoidMovePosition {
			get{ return m_AvoidMovePosition;}
		}


		private Vector3 GetAvoidPosition()
		{
			Transform _creature = m_Owner.transform;
			Transform _target = m_CurrentTarget.TargetGameObject.transform;

			m_TargetRelatedDirectionAngle = GraphicTools.GetDirectionAngle( _target, _creature.position );
			m_CreatureRelatedDirectionAngle = GraphicTools.GetDirectionAngle( _creature , _target.position );

			m_HasAvoidPosition = true;
			
			float _avoid_range = CurrentMove.Avoid.AvoidDistance;
			if( ( m_CreatureRelatedDirectionAngle >= 0 && m_TargetRelatedDirectionAngle >= 0 ) || 
			   ( m_CreatureRelatedDirectionAngle >= 0 && m_TargetRelatedDirectionAngle <= 0 ) )// AVOID LEFT 
				_avoid_range *= 1;
			else if( ( m_CreatureRelatedDirectionAngle <= 0 && m_TargetRelatedDirectionAngle <= 0 ) || 
			        ( m_CreatureRelatedDirectionAngle <= 0 && m_TargetRelatedDirectionAngle >= 0 ) ) // AVOID RIGHT
				_avoid_range *= -1;
			
			Vector3 _right = Vector3.Cross( _target.up, _target.forward );
			m_AvoidMovePosition = _target.position + ( _right * _avoid_range );

			//Debug.DrawLine(_target.position, m_AvoidMovePosition, Color.green);

			return m_AvoidMovePosition;

		}

		private float m_TargetRelatedDirectionAngle = 0;
		private Vector3 m_EscapeMovePosition = Vector3.zero;
		public Vector3 EscapeMovePosition {
			get{ return m_EscapeMovePosition;}
		}
		
		private float m_EscapeAngle = 0;
		public float EscapeAngle {
			get{ return m_EscapeAngle;}
		}



		/// <summary>
		/// Gets the escape position.
		/// </summary>
		/// <returns>The escape position.</returns>
		private Vector3 GetEscapePosition()
		{
			Transform _creature = m_Owner.transform;
			Transform _target = m_CurrentTarget.TargetGameObject.transform;

			m_HasEscapePosition = true;

			m_TargetRelatedDirectionAngle = GraphicTools.GetDirectionAngle( _target, _creature.position );
			
			Vector3 _heading = _creature.position - _target.position;
			m_EscapeAngle = GraphicTools.GetOffsetAngle( _heading );			
			m_EscapeAngle += Random.Range( - CurrentMove.Escape.RandomEscapeAngle, CurrentMove.Escape.RandomEscapeAngle );
			m_EscapeMovePosition  = GraphicTools.GetAnglePosition( _target.position, m_EscapeAngle, m_CurrentTarget.Selectors.SelectionRange + CurrentMove.Escape.EscapeDistance );
			
			//Debug.DrawLine(_creature.position, m_EscapeMovePosition, Color.green);

			return m_EscapeMovePosition;
		}

		//--------------------------------------------------

		private float m_OrbitRadius;
		public float OrbitRadius{
			get{ return m_OrbitRadius;}
		}

		private float m_OrbitAngle;
		public float OrbitAngle{
			get{ return m_OrbitAngle;}
		}

		private float m_OrbitDegrees = 10;
		public float OrbitDegrees{
			get{ return m_OrbitDegrees;}
		}

		/// <summary>
		/// Gets the orbit position.
		/// </summary>
		/// <returns>The orbit position.</returns>
		private Vector3 GetOrbitPosition() 
		{ 
			Vector3 _center = m_CurrentTarget.TargetMovePositionRaw;
			float _radius = CurrentMove.Orbit.Radius;
			float _rate = CurrentMove.Orbit.RadiusShift;
			float _min = CurrentMove.Orbit.MinDistance;
			float _max = CurrentMove.Orbit.MaxDistance;

			if( m_OrbitRadius == 0 )
			{
				m_OrbitRadius = _radius;
				m_OrbitAngle = GraphicTools.GetOffsetAngle( m_Owner.transform.position - _center );
				m_OrbitDegrees = m_OrbitDegrees * (Random.Range(0,1) == 1?1:-1);
			}

			if( ( _rate > 0 && m_OrbitRadius < _max ) || ( _rate < 0 && m_OrbitRadius > _min ) )
			{
				m_OrbitRadius += _rate * Time.deltaTime;
				m_OrbitComplete = false;

				if( m_OrbitRadius < _min )
					m_OrbitRadius = _min;
				else if( _max > 0 && m_OrbitRadius > _max )
					m_OrbitRadius = _max;

			}
			else
				m_OrbitComplete = true;
		
			m_OrbitAngle += m_OrbitDegrees;
			
			if( m_OrbitAngle > 360 )
				m_OrbitAngle = m_OrbitAngle - 360;
			
			float _a = m_OrbitAngle * Mathf.PI / 180f;

			Vector3 new_position = _center + new Vector3(Mathf.Sin(_a) * m_OrbitRadius, 0, Mathf.Cos(_a) * m_OrbitRadius );
			//new_position.y = GetGroundLevel( new_position );

			return new_position;

		}
	}
}

