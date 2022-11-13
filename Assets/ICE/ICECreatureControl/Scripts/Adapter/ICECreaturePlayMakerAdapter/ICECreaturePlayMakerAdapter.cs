// ##############################################################################
//
// ICECreaturePlayMakerAdapter.cs
// Version 1.0
//
// © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.ice-technologies.com
// mailto:support@ice-technologies.com
// 
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// ##############################################################################

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures.Objects;
using HutongGames.PlayMaker;

namespace ICE.Creatures.Adapter
{
	[RequireComponent (typeof (PlayMakerFSM))]
	[RequireComponent (typeof (ICECreatureControl))]
	public class ICECreaturePlayMakerAdapter : MonoBehaviour 
	{
		protected PlayMakerFSM m_FSM = null;
		protected PlayMakerFSM FSM{
			get{
				if( m_FSM == null )
					m_FSM = GetComponent<PlayMakerFSM>();
				
				return m_FSM;
			}
		}
		
		protected ICECreatureControl m_Controller = null;
		protected ICECreatureControl Controller{
			get{
				if( m_Controller == null )
					m_Controller = GetComponent<ICECreatureControl>();
				
				return m_Controller;
			}
		}

		public GameObject m_ActiveTargetGameObject;
		public GameObject ActiveTargetGameObject{
			get{ return m_ActiveTargetGameObject; }
			set{ m_ActiveTargetGameObject = value; }
		}

		// CREATURE STATUS INFLUENCES
		public void AddDamage( float _damage ){
			Controller.Creature.Status.AddDamage( _damage );
		}

		public void AddStress( float _stress ){
			Controller.Creature.Status.AddStress( _stress );
		}

		public void AddDebility( float _debility ){
			Controller.Creature.Status.AddDebility( _debility );
		}

		public void AddHunger( float _hunger ){
			Controller.Creature.Status.AddHunger( _hunger );
		}

		public void AddThirst( float _thirst ){
			Controller.Creature.Status.AddThirst( _thirst );
		}


		void Update()
		{
			FsmGameObject _fsm_active_target_gameobject = FSM.FsmVariables.GetFsmGameObject( "ActiveTargetGameObject" );			
			if( _fsm_active_target_gameobject != null )
				_fsm_active_target_gameobject.Value = Controller.Creature.ActiveTarget.TargetGameObject; 

			FsmGameObject _fsm_previous_target_gameobject = FSM.FsmVariables.GetFsmGameObject( "PreviousTargetGameObject" );			
			if( _fsm_previous_target_gameobject != null && Controller.Creature.PreviousTarget != null )
				_fsm_previous_target_gameobject.Value = Controller.Creature.PreviousTarget.TargetGameObject; 

			FsmString _fsm_active_behaviour_mode_key = FSM.FsmVariables.GetFsmString( "ActiveBehaviourModeKey" );			
			if( _fsm_active_behaviour_mode_key != null )
				_fsm_active_behaviour_mode_key.Value = Controller.Creature.Behaviour.BehaviourModeKey; 

			FsmVector3 _fsm_current_move_position = FSM.FsmVariables.GetFsmVector3( "CurrentMovePosition" );			
			if( _fsm_current_move_position != null )
				_fsm_current_move_position.Value = Controller.Creature.Move.MovePosition; 

			FsmVector3 _fsm_current_target_move_position = FSM.FsmVariables.GetFsmVector3( "CurrentTargetMovePosition" );			
			if( _fsm_current_target_move_position != null )
				_fsm_current_target_move_position.Value = Controller.Creature.ActiveTarget.TargetMovePosition;

			// CREATURE STATUS VALUES
			FsmFloat _fsm_status_age = FSM.FsmVariables.GetFsmFloat( "StatusAge" );			
			if( _fsm_status_age != null )
				_fsm_status_age.Value = Controller.Creature.Status.Age;

			FsmFloat _fsm_status_fitness = FSM.FsmVariables.GetFsmFloat( "StatusFitness" );			
			if( _fsm_status_fitness != null )
				_fsm_status_fitness.Value = Controller.Creature.Status.FitnessInPercent;

			FsmFloat _fsm_status_health = FSM.FsmVariables.GetFsmFloat( "StatusHealth" );			
			if( _fsm_status_health != null )
				_fsm_status_health.Value = Controller.Creature.Status.HealthInPercent;

			FsmFloat _fsm_status_debility = FSM.FsmVariables.GetFsmFloat( "StatusDebility" );			
			if( _fsm_status_debility != null )
				_fsm_status_debility.Value = Controller.Creature.Status.DebilityInPercent;

			FsmFloat _fsm_status_stress = FSM.FsmVariables.GetFsmFloat( "StatusStress" );			
			if( _fsm_status_stress != null )
				_fsm_status_stress.Value = Controller.Creature.Status.StressInPercent;

			FsmFloat _fsm_status_damage = FSM.FsmVariables.GetFsmFloat( "StatusDamage" );			
			if( _fsm_status_damage != null )
				_fsm_status_damage.Value = Controller.Creature.Status.DamageInPercent;

			FsmFloat _fsm_status_hunger = FSM.FsmVariables.GetFsmFloat( "StatusHunger" );			
			if( _fsm_status_hunger != null )
				_fsm_status_hunger.Value = Controller.Creature.Status.HungerInPercent;

			FsmFloat _fsm_status_thirst = FSM.FsmVariables.GetFsmFloat( "StatusThirst" );			
			if( _fsm_status_thirst != null )
				_fsm_status_thirst.Value = Controller.Creature.Status.ThirstInPercent;


		}
	}
}


