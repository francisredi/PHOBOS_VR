// ##############################################################################
//
// ice_CreatureInteraction.cs
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
	public class InteractorRuleObject : TargetObject
	{
		public InteractorRuleObject() : base() {}
		public InteractorRuleObject( InteractorRuleObject _rule ) : base( _rule as TargetObject )
		{
			Enabled =_rule.Enabled;
			BlockRuleUpdateUntilMovePositionReached =_rule.BlockRuleUpdateUntilMovePositionReached;
			OverrideTargetMovePosition = _rule.OverrideTargetMovePosition;
			OverrideInfluences = _rule.OverrideInfluences;
		}
		public InteractorRuleObject( string _key ) : base()
		{
			Enabled = true;
			BehaviourModeKey = _key;
		}

		//public bool Foldout = true;
		public bool Enabled = true;
		public bool OverrideTargetMovePosition = false;
		public bool OverrideInfluences = false;

		public bool BlockRuleUpdateUntilMovePositionReached = false;

		public bool PrepareTarget( TargetObject _target )
		{
			if( _target == null || _target.TargetGameObject == null )
				return false;

			OverrideTargetGameObject( _target.TargetGameObject );

			if( OverrideInfluences == false )
				Influences = _target.Influences;

			if( Selectors.UseDefaultPriority )
				Selectors.Priority = _target.Selectors.Priority;

			if( OverrideTargetMovePosition == false )
				Move.Copy( _target.Move );

			return true;
		}

		/*
		public string BehaviourModeKey;	

		public Vector3 Offset;
		public float StopDistance;
		public float RandomRange;
		public bool IgnoreLevelDifference;
		public bool UpdateOffsetOnActivateTarget;
		public bool UpdateOffsetOnMovePositionReached;
		public bool UpdateOffsetOnRandomizedTimer;
		public float UpdateOffsetUpdateTimeMax = 0;
		public float UpdateOffsetUpdateTimeMin = 0;

		public float SmoothingMultiplier;*/
/*
		[SerializeField]
		private TargetSelectorsObject m_Selectors = new TargetSelectorsObject( TargetType.INTERACTOR );
		public TargetSelectorsObject Selectors{
			set{ m_Selectors = value;}
			get{ return m_Selectors;}
		}
*/


/*
		public Vector3 GetOffsetPosition( Transform _transform )
		{
			if( _transform == null )
				return Vector3.zero;

			Vector3 _local_offset = TargetOffset;
			
			_local_offset.x = _local_offset.x/_transform.lossyScale.x;
			_local_offset.y = _local_offset.y/_transform.lossyScale.y;
			_local_offset.z = _local_offset.z/_transform.lossyScale.z;
			
			return _transform.TransformPoint( _local_offset );
		}*/
	}

	//--------------------------------------------------
	// InteractorObject
	//--------------------------------------------------
	[System.Serializable]
	public class InteractorObject : TargetObject
	{
		public InteractorObject() : base( TargetType.INTERACTOR ){}
		public InteractorObject( InteractorObject _interactor ) : base( _interactor as TargetDataObject )
		{
			//BehaviourModeKey = _interactor.BehaviourModeKey;

			Enabled = _interactor.Enabled;
			InteractorFoldout = _interactor.InteractorFoldout;
			//Key = _interactor.Key;
			/*
			DefaultForceMovePositionReached = _interactor.DefaultForceMovePositionReached;
			DefaultIgnoreLevelDifference = _interactor.DefaultIgnoreLevelDifference;
			DefaultOffset = _interactor.DefaultOffset;
			DefaultRandomRange = _interactor.DefaultRandomRange;
			DefaultSmoothingMultiplier = _interactor.DefaultSmoothingMultiplier;
			DefaultStopDistance = _interactor.DefaultStopDistance;
			DefaultStopDistanceZoneRestricted = _interactor.DefaultStopDistanceZoneRestricted;
			UpdateOffsetOnActivateTarget = _interactor.UpdateOffsetOnActivateTarget;
			UpdateOffsetOnMovePositionReached = _interactor.UpdateOffsetOnMovePositionReached;
			UpdateOffsetOnRandomizedTimer = _interactor.UpdateOffsetOnRandomizedTimer;
			UpdateOffsetUpdateTimeMax = _interactor.UpdateOffsetUpdateTimeMax;
			UpdateOffsetUpdateTimeMin  = _interactor.UpdateOffsetUpdateTimeMin;*/

			//Target.Copy( _interactor.Target );
			//Selectors = new TargetSelectorsObject( _interactor.Selectors );

			Rules.Clear();
			foreach( InteractorRuleObject _rule in _interactor.Rules )
				Rules.Add( new InteractorRuleObject( _rule ) );
		}

		public bool InteractorFoldout = true;
		public bool Enabled = false;
		public bool ShowInteractorInfoText = false;
		public string InteractorInfoText = "";

		public int AveragePriority{
			get{
				int _priority = SelectionPriority;
	
				foreach( InteractorRuleObject _rule in m_Rules )
					_priority += _rule.SelectionPriority;

				_priority = _priority / (m_Rules.Count + 1);

				return _priority;
			}
		}



		[SerializeField]
		private List<InteractorRuleObject> m_Rules = new List<InteractorRuleObject>();
		public List<InteractorRuleObject> Rules
		{
			set{ m_Rules = value; }
			get{ return m_Rules; }
		}
	
		private InteractorRuleObject m_PreviousRule = null;
		public InteractorRuleObject PreviousRule{
			get{ return m_PreviousRule; }
		}

		private InteractorRuleObject m_ActiveRule = null;
		public InteractorRuleObject ActiveRule{
			get{ return m_ActiveRule; }
		}

		/*
		public InteractorRuleObject GetRuleByDistanceAndSituation( float _distance )
		{
		}*/

		public InteractorRuleObject GetRuleByIndexOffset( int _offset )
		{
			int _index = m_Rules.IndexOf( m_ActiveRule );
			int _new_index = _index + _offset;

			if( _new_index < 0 )
				return null;
			else if( _new_index >= m_Rules.Count )
				return null;
			else
				return m_Rules[_new_index];
		}

		/// <summary>
		/// Returns the nearest rule by distance.
		/// </summary>
		/// <returns>The rule by distance.</returns>
		/// <param name="_distance">_distance.</param>
		public InteractorRuleObject GetRuleByDistance( float _distance )
		{
			InteractorRuleObject _selected = null;

			foreach( InteractorRuleObject _rule in m_Rules )
			{
				if( _distance <= _rule.Selectors.SelectionRange && _rule.Enabled )
					_selected = _rule;
			}

			return _selected;
		}

		public List<InteractorRuleObject> GetRulesByDistance( float _distance )
		{
			List<InteractorRuleObject> _rules = new List<InteractorRuleObject>();
			
			foreach( InteractorRuleObject _rule in m_Rules )
			{
				_rule.Selectors.ResetStatus();
				float _range = _rule.Selectors.SelectionRange;

				if( _range == 0 )
					_range = Mathf.Infinity;

				if( _rule.Enabled && ( _rule.Selectors.UseAdvanced || _distance <= _range ) )
					_rules.Add( _rule );
			}
			
			return _rules;
		}

		public bool IsValidAndEnabled
		{
			get{
				if( Enabled && IsValid )
					return true;
				else
					return false;
			}
		}

		public void FixedUpdates()
		{
			FixedUpdate();

			foreach( InteractorRuleObject _rule in Rules )
				_rule.FixedUpdate();
		}



		private List<TargetObject> m_PreparedTargets = null;
		public List<TargetObject> PreparedTargets{
			get{
				if( m_PreparedTargets == null )
					m_PreparedTargets = new List<TargetObject>();

				return m_PreparedTargets; 
			}
		}

		public int PreparedTargetsCount{
			get{ 
				if( m_PreparedTargets != null )
					return m_PreparedTargets.Count;
				else
					return 0;
			}
		}
		
		public void PrepareTargets( GameObject _owner )
		{
			if( Enabled == false || GetBestTargetGameObject( _owner, Selectors.SelectionRange ) == null )
				return;

			PreparedTargets.Clear();

			List<InteractorRuleObject> _rules = GetRulesByDistance( TargetDistanceTo( _owner.transform.position ) );			
			foreach( InteractorRuleObject _rule in _rules )
			{
				if( _rule.PrepareTarget( this as TargetObject ) )
					PreparedTargets.Add( _rule as TargetObject );
			}
			
			if( Enabled && IsValid )
				PreparedTargets.Add( this as TargetObject );
		}

		public bool AutoCreate( GameObject _object , StatusObject _status )
		{
			if( _object == null || _status == null )
				return false;

			Enabled = true;
			AccessType = TargetAccessType.NAME;
			OverrideTargetGameObject( _object );

			if( Creature() != null )
			{
				return CreateAutoCreatureRules( _status, Creature().Creature.Status );
			}
			else if( Player() != null )
			{
				return CreateAutoPlayerRules( _status );
			}				
			else
				return false;
		}

		public bool CreateAutoCreatureRules( StatusObject _own_status, StatusObject _target_status )
		{
			if( Rules.Count > 0 || _own_status == null || _target_status == null  )
				return false;
			
			Selectors.Priority = (int)Random.Range( 50, 70 );
			Selectors.SelectionRange = (int)Random.Range( 30, 50 );

			BehaviourModeKey = "SENSE";

			// TARGET IS PREDATOR
			if( _target_status.TrophicLevel == CreatureTrophicLevelType.CARNIVORE || _target_status.TrophicLevel == CreatureTrophicLevelType.OMNIVORES || 
				_own_status.TrophicLevel == CreatureTrophicLevelType.CARNIVORE || _own_status.TrophicLevel == CreatureTrophicLevelType.OMNIVORES )
			{
				Rules.Add( CreateAutoRule( "HUNT", Selectors.SelectionRange - 5 ) );
				Rules.Add( CreateAutoRule( "ATTACK", Move.StopDistance ) );
			}
			else
			{
				Rules.Add( CreateAutoRule( "ESCAPE", Selectors.SelectionRange - 5 ) );
				Rules.Add( CreateAutoRule( "DEFEND", Move.StopDistance ) );
			}

			return true;
		}

		public bool CreateAutoPlayerRules( StatusObject _own_status )
		{
			if( Rules.Count > 0 || _own_status == null  )
				return false;

			Selectors.Priority = (int)Random.Range( 50, 70 );
			Selectors.SelectionRange = (int)Random.Range( 30, 50 );

			BehaviourModeKey = "SENSE";

			// TARGET IS PREDATOR
			if( _own_status.TrophicLevel == CreatureTrophicLevelType.CARNIVORE || _own_status.TrophicLevel == CreatureTrophicLevelType.OMNIVORES )
			{
				Rules.Add( CreateAutoRule( "HUNT", Selectors.SelectionRange - 5 ) );
				Rules.Add( CreateAutoRule( "ATTACK", Move.StopDistance ) );
			}
			else
			{
				Rules.Add( CreateAutoRule( "ESCAPE", Selectors.SelectionRange - 5 ) );
				Rules.Add( CreateAutoRule( "DEFEND", Move.StopDistance ) );
			}

			return true;
		}

		public InteractorRuleObject CreateAutoRule( string _key, float _range  )
		{
			InteractorRuleObject _rule = new InteractorRuleObject();
			_rule.Enabled = true;
			_rule.AccessType = AccessType;
			_rule.OverrideTargetGameObject( TargetGameObject );
			_rule.Selectors.UseDefaultPriority = true;
			_rule.Selectors.Priority = Selectors.Priority;
			_rule.Selectors.SelectionRange = _range;
			_rule.BehaviourModeKey = _key;

			return _rule;
		}



		/*
		[SerializeField]
		private TargetObject m_Target = null;
		public TargetObject Target{
			get{ 
				if( m_Target == null )
					m_Target = new TargetObject( TargetType.INTERACTOR );
				
				if( m_Target != null )
				{
					m_Target.TargetName = TargetName;
					m_Target.TargetTag = TargetTag;
					m_Target.AccessType = AccessType; 
				}
				
				return m_Target; 
			}
		}

		public InteractorRuleObject GetBestInteractorRule( Vector3 _position )
		{
			InteractorRuleObject _new_rule = null;

			// if the active rule have advanced settings 
			if( m_ActiveRule != null && m_ActiveRule.OverrideTargetMovePosition )
			{
				if( m_ActiveRule.BlockRuleUpdateUntilMovePositionReached )
				{
					// if the target move position was reached, the creture needs a new rule
					if( Target.TargetMovePositionReached( _position ) )
					{
						// if the active rule have a zero range, we select the next higher rule
						if( m_ActiveRule.Selectors.SelectionRange == 0 )
							_new_rule = GetRuleByIndexOffset( 1 );

						// if the active rule have a zero range, we select the next higher rule
						else
							_new_rule = GetRuleByDistance( Target.TargetDistanceTo( _position ) );
					}
				}
				else
					_new_rule = GetRuleByDistance( Target.TargetDistanceTo( _position ) );
			}

			// handle default - get rule by distance 
			else
			{
				_new_rule = GetRuleByDistance( Target.TargetDistanceTo( _position ) );
			}

			if( m_ActiveRule != _new_rule )
			{
				m_PreviousRule = m_ActiveRule;
				m_ActiveRule = _new_rule;
			}

			return m_ActiveRule;
		}

			
		private bool PrepareTarget( GameObject _owner )
		{
			if( Enabled == false )
				return false;

			InteractorRuleObject _rule = GetBestInteractorRule( _owner.transform.position );
			
			if( _rule == null )
			{
				//Target.Copy( this as TargetDataObject );
				Target.BehaviourModeKey = BehaviourModeKey;	
				Target.Selectors.Copy( Selectors ); // incl. default priority
			}
			else
			{
				Target.BehaviourModeKey = _rule.BehaviourModeKey;
				Target.Selectors.Copy( _rule.Selectors );
				Target.Selectors.Priority = ( _rule.Selectors.UseDefaultPriority?Selectors.Priority:_rule.Selectors.Priority );
			}
			
			if( _rule == null || _rule.OverrideTargetMovePosition == false )
			{
				Target.UpdateOffset( TargetOffset );
				Target.TargetStopDistance = TargetStopDistance;
				Target.TargetStopDistanceZoneRestricted = TargetStopDistanceZoneRestricted;
				Target.TargetRandomRange = TargetRandomRange;
				Target.TargetSmoothingMultiplier = TargetSmoothingMultiplier;
				Target.UseUpdateOffsetOnActivateTarget= UseUpdateOffsetOnActivateTarget;
				Target.UseUpdateOffsetOnMovePositionReached = UseUpdateOffsetOnMovePositionReached;
				Target.UseUpdateOffsetOnRandomizedTimer = UseUpdateOffsetOnRandomizedTimer;	
				Target.OffsetUpdateTimeMax = OffsetUpdateTimeMax;
				Target.OffsetUpdateTimeMin = OffsetUpdateTimeMin;
				Target.TargetIgnoreLevelDifference = TargetIgnoreLevelDifference;
			}
			else
			{
				Target.UpdateOffset( _rule.TargetOffset );
				Target.TargetStopDistance = _rule.TargetStopDistance;
				Target.TargetRandomRange = _rule.TargetRandomRange;
				Target.TargetSmoothingMultiplier = _rule.TargetSmoothingMultiplier;
				Target.UseUpdateOffsetOnActivateTarget= _rule.UseUpdateOffsetOnActivateTarget;
				Target.UseUpdateOffsetOnMovePositionReached = _rule.UseUpdateOffsetOnMovePositionReached;		
				Target.UseUpdateOffsetOnRandomizedTimer = _rule.UseUpdateOffsetOnRandomizedTimer;	
				Target.OffsetUpdateTimeMax = _rule.OffsetUpdateTimeMax;
				Target.OffsetUpdateTimeMin = _rule.OffsetUpdateTimeMin;
				Target.TargetIgnoreLevelDifference = _rule.TargetIgnoreLevelDifference;
			}
			
			return true;		
			
		}
		*/
	}

	[System.Serializable]
	public class InteractionObject
	{
		private GameObject m_Owner = null;
		private ICECreatureRegister m_CreatureRegister = null;


		[SerializeField]
		private List<InteractorObject> m_Interactors = new List<InteractorObject>();
		public List<InteractorObject> Interactors{
			set{ m_Interactors = value; }
			get{ return m_Interactors; }
		}

		public void Reset()
		{
			m_Interactors.Clear();
		}
	

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;

			if( m_CreatureRegister == null )
				m_CreatureRegister = GameObject.FindObjectOfType<ICECreatureRegister>();
		}

		public List<InteractorObject> GetValidInteractors()
		{
			List<InteractorObject> _interactors = new List<InteractorObject> ();
			foreach( InteractorObject _interactor in Interactors )
			{
				if( _interactor.IsValidAndEnabled )
					_interactors.Add( _interactor );
			}

			return _interactors;
		}

		public bool TargetsReady()
		{
			if( GetValidInteractors().Count > 0 )
				return true;
			else
				return false;
		}

		public bool CheckExternalTarget( GameObject _target )
		{
			if( _target == null )
				return false;

			return false;
		}

		public bool UseAutoInteractorDetection = false;
		public void AutoInteractorDetection()
		{
		}

		public InteractorObject AddInteractor( GameObject _object, StatusObject _status )
		{
			if( _object == null || _status == null )
				return null;
			
			InteractorObject _interactor = GetInteractor( _object );

			if( _interactor == null )
			{
				_interactor = new InteractorObject();
				if( _interactor.AutoCreate( _object, _status ) )
					Interactors.Add( _interactor );
			}

			return _interactor;
		}

		public InteractorObject GetInteractor( GameObject _object )
		{
			if( _object == null )
				return null;

			InteractorObject _interactor_obj = null;
			
			foreach( InteractorObject _interactor in Interactors )
			{
				if( _interactor.TargetGameObject == null ) 
					continue;					
				else if( ( _interactor.AccessType == TargetAccessType.NAME && _interactor.TargetName == _object.name ) ||
					( _interactor.AccessType == TargetAccessType.TAG && _interactor.TargetTag == _object.tag ) ||
					( _interactor.AccessType == TargetAccessType.OBJECT && _interactor.TargetGameObject.GetInstanceID() == _object.GetInstanceID() ) )
				{
					_interactor_obj = _interactor;
					break;
				}
			}

			return _interactor_obj;
		}

		public void Sense()
		{
			foreach( InteractorObject _interactor in Interactors )
			{
				if( _interactor.IsValidAndEnabled )
					_interactor.PrepareTargets( m_Owner );
			}
		}

		public void FixedUpdate()
		{
			foreach( InteractorObject _interactor in Interactors )
			{
				if( _interactor.IsValidAndEnabled )
					_interactor.FixedUpdates();
			}
				
		}

	}

}

