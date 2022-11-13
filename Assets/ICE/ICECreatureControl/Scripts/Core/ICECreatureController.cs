// ##############################################################################
//
// ICECreatureController.cs
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
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Utilities;

namespace ICE.Creatures
{


	/// <summary>
	/// ICECreatureController Core Component 
	/// </summary>
	public abstract class ICECreatureController : MonoBehaviour {

		public DisplayData Display = new DisplayData();

		private bool m_CoUpdateIsRunning = false;

		[SerializeField]
		private CreatureObject m_Creature = new CreatureObject();
		public CreatureObject Creature{
			set{ m_Creature = value; }
			get{ return m_Creature; }
		}

		void Awake () {
			
			if( Creature.DontDestroyOnLoad )
				DontDestroyOnLoad(this);

			// wakes up the creature ...
			Creature.Init( gameObject );
		}

		void Start () {

			// initial start of the update coroutine (if required)
			if( Creature.UseCoroutine && m_CoUpdateIsRunning == false )
				StartCoroutine( CoUpdate() );
		}

		void OnEnable() {

			// if script or gameobject were disabled, we have to start the coroutine again ... 
			if( Creature.UseCoroutine && m_CoUpdateIsRunning == false )
				StartCoroutine( CoUpdate() );
		}

		void OnDisable() {

			// deactivating the gameobject will stopping the coroutine, so we capture the ondisable 
			// and stops the coroutine controlled ... btw. if only the script was disabled, the coroutine would be 
			// still running, but we don't need the coroutine if the rest of the script isn't running and so we 
			// capture all cases
			if( Creature.UseCoroutine )
				StopCoroutine( CoUpdate() );

			m_CoUpdateIsRunning = false;
		}

		void OnDestroy() {
			// informs the creature register that the creature leaves 
			Creature.Bye();
		}

		//********************************************************************************
		// Coroutine Update
		//********************************************************************************
		IEnumerator CoUpdate()
		{
			while( Creature.UseCoroutine )
			{
				// coroutine is alive ... 
				m_CoUpdateIsRunning = true;

				// sense is using its internal timer ... so we don't need an other yield here ...
				Sense();

				// react is using its internal timer ... so we don't need an other yield here ...
				React();

				yield return null;
			}		
			m_CoUpdateIsRunning = false;
			yield break;
		}

		//********************************************************************************
		// Update
		//********************************************************************************
		void Update () {

			UpdateBegin();
			Creature.UpdateBegin();

			if( Creature.UseCoroutine == false )
			{
				Sense();
				React();
			}

			Move();

			Creature.UpdateComplete();
			UpdateComplete();
		}

		//********************************************************************************
		// Fixed Update
		//********************************************************************************
		void FixedUpdate()
		{
			FixedUpdateBegin();
			Creature.FixedUpdate();
			FixedUpdateComplete();

			m_CoUpdateIsRunning = false;
		}

		void OnCollisionEnter(Collision _collision) 
		{
			if( _collision == null )
				return;
			
			string _name = "";
			if( _collision.contacts.Length > 0 )
				_name = _collision.contacts[0].thisCollider.name;

			Creature.HandleCollider( _collision.collider, _name );
		}

		void OnCollisionStay(Collision _collision) 
		{
			if( _collision == null )
				return;

			string _name = "";
			if( _collision.contacts.Length > 0 )
				_name = _collision.contacts[0].thisCollider.name;

			Creature.HandleCollider( _collision.collider, _name );
		}

		void OnTriggerEnter( Collider _collider ) 
		{
			if( _collider == null )
				return;

			if( Creature.Status.UseShelter && _collider.gameObject.tag == Creature.Status.ShelterTag )
				Creature.Status.IsSheltered = true;
			if( Creature.Status.UseIndoor && _collider.gameObject.tag == Creature.Status.IndoorTag )
				Creature.Status.IsIndoor = true;

			Creature.HandleCollider( _collider );
		}
		
		void OnTriggerStay( Collider _collider ) 
		{
			if( _collider == null )
				return;

			Creature.HandleCollider( _collider );
		}

		void OnTriggerExit( Collider _collider ) 
		{
			if( _collider == null )
				return;
			
			if( Creature.Status.UseShelter && _collider.gameObject.tag == Creature.Status.ShelterTag )
				Creature.Status.IsSheltered = false;
			if( Creature.Status.UseIndoor && _collider.gameObject.tag == Creature.Status.IndoorTag )
				Creature.Status.IsIndoor = false;
		}

		void OnControllerColliderHit( ControllerColliderHit hit ) 
		{
			if( hit == null )
				return;

			Creature.HandleCollider( hit.collider );
			/*
			Rigidbody body = hit.collider.attachedRigidbody;
			if (body == null || body.isKinematic)
				return;
			
			if (hit.moveDirection.y < -0.3F)
				return;
			
			Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
			body.velocity = pushDir * 100;*/
		}

		void OnBecameVisible()
		{
			Creature.HandleVisibility( true );
		}

		void OnBecameInvisible()
		{
			Creature.HandleVisibility( false );
		}

		public abstract void UpdateBegin();
		public abstract void SenseComplete();
		public abstract void ReactComplete();
		public abstract void MoveComplete();
		public abstract void UpdateComplete();
		
		public abstract void FixedUpdateBegin();
		public abstract void FixedUpdateComplete();


		public float StatusDamageInPercent{
			get{ return Creature.Status.DamageInPercent; }
		}
		
		public float StatusStressInPercent{
			get{ return Creature.Status.StressInPercent; }
		}
		
		public float StatusDebilityInPercent{
			get{ return Creature.Status.DebilityInPercent; }
		}
		
		public float StatusThirstInPercent{
			get{ return Creature.Status.ThirstInPercent; }
		}
		
		public float StatusHungerInPercent{
			get{ return Creature.Status.HungerInPercent; }
		}

		public float StatusAddDamage{
			set{ Creature.Status.AddDamage( value ); }
		}

		public float StatusAddStress{
			set{ Creature.Status.AddStress( value ); }
		}
				
		public float StatusAddDebility{
			set{ Creature.Status.AddDebility( value ); }
		}

		public float StatusAddHunger{
			set{ Creature.Status.AddHunger( value ); }
		}

		public float StatusAddThirst{
			set{ Creature.Status.AddThirst( value ); }
		}


		// character
		public float StatusAddAggressivity{
			set{ Creature.Status.AddAggressivity( value ); }
		}

		public float StatusAddAnxiety{
			set{ Creature.Status.AddAnxiety( value ); }
		}

		public float StatusAddExperience{
			set{ Creature.Status.AddExperience( value ); }
		}

		public float StatusAddNosiness{
			set{ Creature.Status.AddNosiness( value ); }
		}

		public float StatusAggressivityInPercent{
			get{ return Creature.Status.AggressivityInPercent; }
		}

		public float StatusAnxietyInPercent{
			get{ return Creature.Status.AnxietyInPercent; }
		}

		public float StatusExperienceInPercent{
			get{ return Creature.Status.ExperienceInPercent; }
		}

		public float StatusNosinessInPercent{
			get{ return Creature.Status.NosinessInPercent; }
		}

		public float StatusAge{
			set{ Creature.Status.SetAge( value ); }
			get{ return Creature.Status.Age; }
		}

		public float StatusLifespanInPercent{
			get{ return Creature.Status.LifespanInPercent; }
		}

		// Vital
		public float StatusFitnessInPercent{
			get{ return Creature.Status.FitnessInPercent; }
		}

		public float StatusArmorInPercent{
			get{ return Creature.Status.ArmorInPercent; }
		}

		public float StatusHealthInPercent{
			get{ return Creature.Status.HealthInPercent; }
		}

		public float StatusStaminaInPercent{
			get{ return Creature.Status.StaminaInPercent; }
		}

		public float StatusPowerInPercent{
			get{ return Creature.Status.PowerInPercent; }
		}

		public GameObject ActiveTargetGameObject{
			get{ 

				if( Creature.ActiveTarget != null ) 
					return Creature.ActiveTarget.TargetGameObject; 
				else
					return null;
			}
		}

		public float MoveDirection{
			get{ return Creature.Move.MoveDirection; }
		}


		public float MoveForwardVelocity{
			get{ return Creature.Move.MoveVelocity.z; }
		}

		public float MoveAngularVelocity{
			get{ return Creature.Move.RealAngularVelocity; }// BehaviourAngularVelocity * GraphicTools.AngleDirectionExt( transform.forward, Vector3.up, Creature.Move.MovePositionDelta ); }
		}

		public float MovePositionDeltaForward{
			get{ return Creature.Move.MovePositionDelta.z; }
		}
		
		public float MoveRotationDeltaVertical{
			get{ return Creature.Move.MoveRotationDelta.y; }
		}

		public float BehaviourForwardVelocity{
			get{ 
				if( Creature.Behaviour.BehaviourMode != null && Creature.Behaviour.BehaviourMode.Rule != null )
					return Creature.Behaviour.BehaviourMode.Rule.Move.Velocity.Velocity.z;
				else
					return 0;
			}
		}
		
		public float BehaviourAngularVelocity{
			get{ 
				if( Creature.Behaviour.BehaviourMode != null && Creature.Behaviour.BehaviourMode.Rule != null )
					return Creature.Behaviour.BehaviourMode.Rule.Move.Velocity.AngularVelocity;
				else
					return 0;
			}
		}

		public bool GetDynamicBooleanValue( DynamicBooleanValueType _type )
		{
			switch( _type )
			{
			case DynamicBooleanValueType.CreatureIsGrounded:
				return Creature.Move.IsGrounded;
			case DynamicBooleanValueType.CreatureDeadlocked:
				return Creature.Move.Deadlocked;
			case DynamicBooleanValueType.CreatureMovePositionReached:
				return Creature.Move.MovePositionReached;
			case DynamicBooleanValueType.CreatureTargetMovePositionReached:
				return Creature.Move.TargetMovePositionReached;
			case DynamicBooleanValueType.CreatureMovePositionUpdateRequired:
				return Creature.Move.MovePositionUpdateRequired;

			}
			
			return false;
		}

		public int GetDynamicIntegerValue( DynamicIntegerValueType _type )
		{/*
			switch( _type )
			{

			case DynamicIntegerValueType.CreatureForwardSpeed:
				return MoveForwardVelocity;
			case DynamicIntegerValueType.CreatureAngularSpeed:
				return MoveAngularVelocity;
			case DynamicIntegerValueType.CreatureDirection:
				return MoveDirection;
			}*/
			
			return 0;
		}

		public float GetDynamicFloatValue( DynamicFloatValueType _type )
		{
			switch( _type )
			{
				case DynamicFloatValueType.CreatureForwardSpeed:
					return MoveForwardVelocity;
				case DynamicFloatValueType.CreatureAngularSpeed:
					return MoveAngularVelocity;
				case DynamicFloatValueType.CreatureDirection:
					return MoveDirection;

			case DynamicFloatValueType.CreatureMovePositionDistance:
				return Creature.Move.MovePositionDistance;
			}

			return 0;
		}

		public void TriggerBehaviourRuleAudio()
		{
			if( Creature.Behaviour.ActiveBehaviourModeRule != null )
				Creature.Behaviour.BehaviourAudio.Play( Creature.Behaviour.ActiveBehaviourModeRule.Audio );
		}




		//********************************************************************************
		// Sense
		//********************************************************************************
		public virtual void Sense()
		{
			// that's a short delay to slowing the creatures sense, because it looks more natural if the 
			// creature can't sence everything in milliseconds. Btw. if you want to use a large crowd/herd 
			// you can use the sence and reaction time also to optimize the performance
			if( Creature.IsSenseTime( transform ) )
			{
				//Creature.Essentials.Target.GetBestTargetGameObject( transform.gameObject );
				//Creature.Missions.Outpost.Target.GetBestTargetGameObject( transform.gameObject );
				//Creature.Missions.Outpost.Target.GetBestTargetGameObject( transform.gameObject );
				//Creature.Interaction.Sense();			
				SenseComplete();
			}
		}

		//********************************************************************************
		// React
		//********************************************************************************
		public virtual void React()
		{
			// that's a short delay to slowing the creatures reaction time, because it looks more natural if the 
			// creature don't react within milliseconds. Btw. if you want to use a large crowd/herd 
			// you can use the sence and reaction time also to optimize the performance
			if( Creature.IsReactionTime( transform ) )
			{
				Creature.AvailableTargets.Clear();

				// PREDECISIONS HOME
				Creature.AddAvailableTarget( Creature.Essentials.PrepareTarget( Creature ) );

				// PREDECISIONS MISSION OUTPOST
				Creature.AddAvailableTarget( Creature.Missions.Outpost.PrepareTarget( transform.gameObject, Creature ) );

				// PREDECISIONS MISSION ESCORT
				Creature.AddAvailableTarget( Creature.Missions.Escort.PrepareTarget( transform.gameObject, Creature ) );
			
				// PREDECISIONS MISSION PATROL
				Creature.AddAvailableTarget( Creature.Missions.Patrol.PrepareTarget( transform.gameObject, Creature ) );

				// PREDECISIONS INTERACTORS
				foreach( InteractorObject _interactor in Creature.Interaction.Interactors )
				{
					_interactor.PrepareTargets( transform.gameObject );
					foreach( TargetObject _target in _interactor.PreparedTargets )
						Creature.AddAvailableTarget( _target );
				}

				Creature.SelectBestTarget();

				ReactComplete();
			}
		}

		//********************************************************************************
		// Move
		//********************************************************************************
		public virtual void Move()
		{
			// move should be the only update which is required in each frame ...
			Creature.UpdateMove();
			MoveComplete();
		}

/*
		public virtual void OnAnimatorMove()
		{
			Debug.Log( "ROOT MOTION" );

		}*/

	}
}
