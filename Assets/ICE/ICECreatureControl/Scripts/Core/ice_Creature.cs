// ##############################################################################
//
// ice_Creature.cs
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
using ICE.Creatures.Objects;
using ICE.Utilities;

namespace ICE.Creatures.Objects
{
	[System.Serializable]
	public class SelectorObject : System.Object
	{
		private GameObject m_Owner = null;
		private CreatureObject Creature = null;
		private MoveObject Move = null;
		private BehaviourObject Behaviour = null;
		private StatusObject Status = null;
		TargetObject m_ActiveTarget = null;
		TargetObject m_PreviousTarget = null;


		public SelectorObject( CreatureObject _creature )
		{
			m_Owner = _creature.Owner;
			Creature = _creature;
			Move = _creature.Move;
			Behaviour = _creature.Behaviour;
			Status = _creature.Status;
			m_ActiveTarget = _creature.ActiveTarget;
			m_PreviousTarget = _creature.PreviousTarget;
		}

		public TargetObject SelectBestTarget( List<TargetObject> _targets )
		{
			TargetObject _favourite_target = null;
			foreach( TargetObject _target in _targets )
			{
				if( _target == null || _target.IsValid == false || _target.TargetGameObject.activeInHierarchy == false )
					continue;

				_target.Selectors.IsValid = BasicCheckComplied( _target ); 
				_target.Selectors.TotalCheckIsValid = _target.Selectors.IsValid;

				if( _target.Selectors.UseAdvanced )
				{
					if( _target.Selectors.InitialOperatorType == ConditionalOperatorType.AND && _target.Selectors.IsValid == true )
						_target.Selectors.TotalCheckIsValid = AdvancedCheckComplied( _target, _target.Selectors.IsValid );
					else if( _target.Selectors.InitialOperatorType == ConditionalOperatorType.OR && _target.Selectors.IsValid == false )
						_target.Selectors.TotalCheckIsValid = AdvancedCheckComplied( _target, _target.Selectors.IsValid );
				}



				if( _target.Selectors.TotalCheckIsValid )
				{
					if( _favourite_target == null )
						_favourite_target = _target;
					else if( _target.Selectors.Priority > _favourite_target.Selectors.Priority )
						_favourite_target = _target;
					else if( _target.Selectors.Priority == _favourite_target.Selectors.Priority )
					{
						if( _target.TargetMovePositionDistanceTo( m_Owner.transform.position ) < _favourite_target.TargetMovePositionDistanceTo( m_Owner.transform.position ) || ( Random.Range(0,1 )==1 ) )
							_favourite_target = _target;
					}
				}
			}

			return _favourite_target;
		}

		public bool BasicCheckComplied( TargetObject _target )
		{
			return _target.TargetInSelectionRange( m_Owner.transform, Move.FieldOfView, Move.VisualRange, Move.EyeHeight );
		}

		public bool AdvancedCheckComplied( TargetObject _target, bool _final_result )
		{
			foreach( TargetSelectorObject _selector in _target.Selectors.Selectors )
			{
				_selector.ResetStatus();

				foreach( TargetSelectorConditionObject _condition in _selector.Conditions )
				{
					_condition.ResetStatus();
	
					if( _condition.Enabled == false )
						continue;

					if( TargetSelectorExpression.IsEnumValue( _condition.ExpressionType ) )
						_condition.IsValid = CompareEnumValue( _target, _condition );

					if( TargetSelectorExpression.IsNumericValue( _condition.ExpressionType ) )
						_condition.IsValid = CompareNumericValue( _target, _condition );

					else if( TargetSelectorExpression.IsObjectValue( _condition.ExpressionType ) )
						_condition.IsValid = CompareObjects( _target, _condition );

					else if( TargetSelectorExpression.IsStringValue( _condition.ExpressionType ) )
						_condition.IsValid = CompareStringValue( _target, _condition );

					else if( TargetSelectorExpression.IsKeyCodeValue( _condition.ExpressionType ) )
						_condition.IsValid = CompareKeyCodeValue( _target, _condition );

					else if( TargetSelectorExpression.IsBooleanValue( _condition.ExpressionType ) )
						_condition.IsValid = CompareBooleanValue( _target, _condition );
					/*
					// TIME
					else if( _condition.ExpressionType == TargetSelectorExpressionType.EnvironmentTimeHour )
						_condition.IsValid = CompareNumber( CreatureRegister.EnvironmentInfos.TimeHour, _condition.FloatValue, _condition.Operator );
					else if( _condition.ExpressionType == TargetSelectorExpressionType.EnvironmentTimeMinute )
						_condition.IsValid = CompareNumber( CreatureRegister.EnvironmentInfos.TimeMinutes, _condition.FloatValue, _condition.Operator );
					else if( _condition.ExpressionType == TargetSelectorExpressionType.EnvironmentTimeSecond )
						_condition.IsValid = CompareNumber( CreatureRegister.EnvironmentInfos.TimeSeconds, _condition.FloatValue, _condition.Operator );

					// DATE
					else if( _condition.ExpressionType == TargetSelectorExpressionType.EnvironmentDateYear )
						_condition.IsValid = CompareNumber( CreatureRegister.EnvironmentInfos.DateYear, _condition.FloatValue, _condition.Operator );
					else if( _condition.ExpressionType == TargetSelectorExpressionType.EnvironmentDateMonth )
						_condition.IsValid = CompareNumber( CreatureRegister.EnvironmentInfos.DateMonth, _condition.FloatValue, _condition.Operator );
					else if( _condition.ExpressionType == TargetSelectorExpressionType.EnvironmentDateDay )
						_condition.IsValid = CompareNumber( CreatureRegister.EnvironmentInfos.DateDay, _condition.FloatValue, _condition.Operator );


					// TEMPREATURE
					else if( _condition.ExpressionType == TargetSelectorExpressionType.EnvironmentTemperature )
						_condition.IsValid = CompareNumber( ICECreatureRegister.Instance.EnvironmentInfos.Temperature, _condition.FloatValue, _condition.Operator );
					*/

					// CREATURE BEHAVIOUR
					else if( _condition.ExpressionType == TargetSelectorExpressionType.OwnBehaviour )
					{
						if( Behaviour.BehaviourModeKey == _condition.StringValue )
							_condition.IsValid = true;

						if( _condition.Operator == LogicalOperatorType.NOT )
							_condition.IsValid = ! _condition.IsValid;

					}


					else if( _condition.ExpressionType == TargetSelectorExpressionType.OwnPosition )
					{
						switch( _condition.PositionType )
						{
						case TargetSelectorPositionType.TargetMovePosition:
							_condition.IsValid = _target.TargetMovePositionReached( m_Owner.transform.position );
							break;
						case TargetSelectorPositionType.TargetMaxRange:
							_condition.IsValid =_target.TargetInMaxRange( m_Owner.transform.position );
							break;
						case TargetSelectorPositionType.ActiveTargetMovePosition:
							_condition.IsValid = (m_ActiveTarget != null?m_ActiveTarget.TargetMovePositionReached( m_Owner.transform.position):false );
							break;
						case TargetSelectorPositionType.ActiveTargetMaxRange:
							_condition.IsValid = (m_ActiveTarget != null?m_ActiveTarget.TargetInMaxRange( m_Owner.transform.position ):false );
							break;
						case TargetSelectorPositionType.HomeTargetMovePosition:
							_condition.IsValid = (Creature.Essentials.Target != null?Creature.Essentials.Target.TargetMovePositionReached( m_Owner.transform.position):false );
							break;
						case TargetSelectorPositionType.HomeTargetMaxRange:
							_condition.IsValid = (Creature.Essentials.Target != null?Creature.Essentials.Target.TargetInMaxRange( m_Owner.transform.position ):false );
							break;
						case TargetSelectorPositionType.OutpostTargetMovePosition:
							_condition.IsValid = (Creature.Missions.Outpost.Target != null?Creature.Missions.Outpost.Target.TargetMovePositionReached( m_Owner.transform.position):false );
							break;
						case TargetSelectorPositionType.OutpostTargetMaxRange:
							_condition.IsValid = (Creature.Missions.Outpost.Target != null?Creature.Missions.Outpost.Target.TargetInMaxRange( m_Owner.transform.position ):false );
							break;
						case TargetSelectorPositionType.EscortTargetMovePosition:
							_condition.IsValid = (Creature.Missions.Escort.Target != null?Creature.Missions.Escort.Target.TargetMovePositionReached( m_Owner.transform.position):false );
							break;
						case TargetSelectorPositionType.EscortTargetMaxRange:
							_condition.IsValid = (Creature.Missions.Escort.Target != null?Creature.Missions.Escort.Target.TargetInMaxRange( m_Owner.transform.position ):false );
							break;
						case TargetSelectorPositionType.PatrolTargetMovePosition:
							_condition.IsValid = (Creature.Missions.Patrol.Target != null?Creature.Missions.Patrol.Target.TargetMovePositionReached( m_Owner.transform.position):false );
							break;
						case TargetSelectorPositionType.PatrolTargetMaxRange:
							_condition.IsValid = (Creature.Missions.Patrol.Target != null?Creature.Missions.Patrol.Target.TargetInMaxRange( m_Owner.transform.position ):false );
							break;
						}

						if( _condition.Operator == LogicalOperatorType.NOT )
							_condition.IsValid = ! _condition.IsValid;

					}
					else if( _condition.ExpressionType == TargetSelectorExpressionType.PreviousTargetType )
					{
						TargetObject _precursor = null;
						if( _target == m_ActiveTarget )
							_precursor = m_PreviousTarget;
						else
							_precursor = m_ActiveTarget;

						_condition.IsValid = _condition.ComparePrecursorTarget( _precursor );

					}

					if( _selector.Conditions.IndexOf( _condition ) == 0 )
						_selector.IsValid = _condition.IsValid;
					else if( _condition.ConditionType == ConditionalOperatorType.AND && _selector.IsValid == true )
						_selector.IsValid = _condition.IsValid;
					else if( _condition.ConditionType == ConditionalOperatorType.OR && _selector.IsValid == false )
						_selector.IsValid = _condition.IsValid;

				}

				if( _target.Selectors.Selectors.IndexOf( _selector ) == 0 )
					_final_result = _selector.IsValid;
				else if( _selector.InitialOperatorType == ConditionalOperatorType.AND && _final_result == true )
					_final_result = _selector.IsValid;
				else if( _selector.InitialOperatorType == ConditionalOperatorType.OR && _final_result == false )
					_final_result = _selector.IsValid;
			}

			return _final_result;
		}

		private GameObject GetGameObject( TargetObject _target, TargetSelectorExpressionType _type )
		{
			if( _target == null )
				return null;

			switch( _type )
			{
			// CREATURE 
			case TargetSelectorExpressionType.OwnGameObject:
				return ( m_Owner != null)?m_Owner.gameObject:null;

				// CREATURE ACTIVE TARGET
			case TargetSelectorExpressionType.ActiveTarget:
				return ( m_ActiveTarget != null)?m_ActiveTarget.TargetGameObject:null;

				// TARGET  
			case TargetSelectorExpressionType.TargetGameObject:
				return _target.TargetGameObject;

				// TARGET CREATURE 
			case TargetSelectorExpressionType.Creature:
				return (_target.Creature() != null)?_target.TargetGameObject:null;

				// TARGET CREATURE ACTIVE TARGET
			case TargetSelectorExpressionType.CreatureActiveTarget:
				return (_target.Creature() != null && _target.Creature().Creature.ActiveTarget != null )?_target.Creature().Creature.ActiveTarget.TargetGameObject:null;

				// TARGET ITEM 
			case TargetSelectorExpressionType.Item:
				return (_target.Item() != null)?_target.TargetGameObject:null;

				// TARGET WAYPOINT 
			case TargetSelectorExpressionType.Waypoint:
				return (_target.Waypoint() != null)?_target.TargetGameObject:null;

				// TARGET MARKER 
			case TargetSelectorExpressionType.Marker:
				return (_target.Marker() != null)?_target.TargetGameObject:null;

				// TARGET PLAYER 
			case TargetSelectorExpressionType.Player:
				return (_target.Player() != null)?_target.TargetGameObject:null;

				// TARGET LOCATION 
			case TargetSelectorExpressionType.Location:
				return (_target.Location() != null)?_target.TargetGameObject:null;
			}

			return null;
		}

		private string GetStringValue( TargetObject _target, TargetSelectorExpressionType _type )
		{
			switch( _type )
			{
			// CREATURE BEHAVIOUR
			case TargetSelectorExpressionType.OwnBehaviour:
				return Behaviour.BehaviourModeKey;

				// CREATURE COMMAND
			case TargetSelectorExpressionType.OwnCommand:
				return Creature.LastGroupCommand;

			// ACTIVE TARGET NAME
			case TargetSelectorExpressionType.ActiveTargetName:
				return ( m_ActiveTarget != null?m_ActiveTarget.TargetName:"" );

			case TargetSelectorExpressionType.ActiveTargetParentName:
				return ( m_ActiveTarget != null?m_ActiveTarget.TargetParentName:"" );

			case TargetSelectorExpressionType.TargetName:
				return ( _target != null?_target.TargetName:"" );

			case TargetSelectorExpressionType.TargetParentName:
				return ( _target != null ?_target.TargetParentName:"" );



				// CREATURE BEHAVIOUR
			case TargetSelectorExpressionType.CreatureBehaviour:
				return (_target.Creature() != null )?_target.Creature().Creature.Behaviour.BehaviourModeKey:"";

				// CREATURE BEHAVIOUR
			case TargetSelectorExpressionType.CreatureCommand:
				return (_target.Creature() != null )?_target.Creature().Creature.LastGroupCommand:"";

			}

			return "";
		}

		private bool GetBooleanValue( TargetObject _target, TargetSelectorExpressionType _type )
		{
			switch( _type )
			{
			// CREATURE BEHAVIOUR
			case TargetSelectorExpressionType.OwnIsDead:
				return Status.IsDead;

			case TargetSelectorExpressionType.OwnIsSheltered:
				return Status.IsSheltered;

			case TargetSelectorExpressionType.OwnIsIndoor:
				return Status.IsIndoor;

				// CREATURE BEHAVIOUR
			case TargetSelectorExpressionType.CreatureIsDead:
				return (_target.Creature() != null )?_target.Creature().Creature.Status.IsDead:false;

			case TargetSelectorExpressionType.CreatureIsSheltered:
				return (_target.Creature() != null )?_target.Creature().Creature.Status.IsSheltered:false;

			case TargetSelectorExpressionType.CreatureIsIndoor:
				return (_target.Creature() != null )?_target.Creature().Creature.Status.IsIndoor:false;

			case TargetSelectorExpressionType.ActiveTargetHasParent:
				return (m_ActiveTarget != null && m_ActiveTarget.TargetGameObject != null && m_ActiveTarget.TargetGameObject.transform.parent != null )?true:false;

			case TargetSelectorExpressionType.TargetHasParent:
				return (_target != null && _target.TargetGameObject != null && _target.TargetGameObject.transform.parent != null )?true:false;
			}

			return false;
		}

		private int GetEnumValue( TargetObject _target, TargetSelectorExpressionType _type )
		{
			switch( _type )
			{

			case TargetSelectorExpressionType.TargetReferenceType:
				return (int)((_target != null )?_target.TargetReferenceType:RegisterReferenceType.UNDEFINED);
			case TargetSelectorExpressionType.EnvironmentWeather:
				return (ICEGlobalEnvironmentInfo.Instance != null?(int)ICEGlobalEnvironmentInfo.Instance.Weather:0);
			case TargetSelectorExpressionType.OwnOdour:
				return (int)Status.Odour.Type;
			case TargetSelectorExpressionType.CreatureOdour:
				return (int)((_target.Creature() != null )?_target.Creature().Creature.Status.Odour.Type:OdourType.UNDEFINED);
			case TargetSelectorExpressionType.MarkerOdour:
				return (int)((_target.Marker() != null )?_target.Marker().Odour.Type:OdourType.UNDEFINED);
			default:
				return 0;
			}
		}

		private float GetFloatValue( TargetObject _target, TargetSelectorExpressionType _type )
		{
			switch( _type )
			{
			// CREATURE AGE
			case TargetSelectorExpressionType.OwnAge:
				return Status.Age;

				// CREATURE ODOUR INTENSITY
			case TargetSelectorExpressionType.OwnOdourIntensity:
				return Status.Odour.Intensity;
				// CREATURE ODOUR RANGE
			case TargetSelectorExpressionType.OwnOdourRange:
				return Status.Odour.Range;

				// CREATURE TEMPERATURE DEVIATION
			case TargetSelectorExpressionType.OwnEnvTemperatureDeviation:
				return Status.TemperatureDeviationInPercent;

				// CREATURE FITNESS
			case TargetSelectorExpressionType.OwnFitness:
				return Status.FitnessInPercent;
				// CREATURE HEALTH
			case TargetSelectorExpressionType.OwnHealth:
				return Status.HealthInPercent;
				// CREATURE POWER
			case TargetSelectorExpressionType.OwnPower:
				return Status.PowerInPercent;
				// CREATURE STAMINA
			case TargetSelectorExpressionType.OwnStamina:
				return Status.StaminaInPercent;

				// CREATURE DAMAGE
			case TargetSelectorExpressionType.OwnDamage:
				return Status.DamageInPercent;			
				// CREATURE DEBILITY
			case TargetSelectorExpressionType.OwnDebility:
				return Status.DebilityInPercent;
				// CREATURE STRESS
			case TargetSelectorExpressionType.OwnStress:
				return Status.StressInPercent;
				// CREATURE HUNGER
			case TargetSelectorExpressionType.OwnHunger:
				return Status.HungerInPercent;
				// CREATURE THIRST
			case TargetSelectorExpressionType.OwnThirst:
				return Status.ThirstInPercent;

				// CREATURE AGGRESSIVITY
			case TargetSelectorExpressionType.OwnAggressivity:
				return Status.AggressivityInPercent;
				// CREATURE EXPERIENCE
			case TargetSelectorExpressionType.OwnExperience:
				return Status.ExperienceInPercent;
				// CREATURE ANXIETY
			case TargetSelectorExpressionType.OwnAnxiety:
				return Status.AggressivityInPercent;
				// CREATURE NOSINESS
			case TargetSelectorExpressionType.OwnNosiness:
				return Status.NosinessInPercent;

				// CREATURE VISUAL
			case TargetSelectorExpressionType.OwnVisualSense:
				return Status.SenseVisualInPercent;
				// CREATURE AUDITORY
			case TargetSelectorExpressionType.OwnAuditorySense:
				return Status.SenseAuditoryInPercent;
				// CREATURE OLFACTORY
			case TargetSelectorExpressionType.OwnOlfactorySense:
				return Status.SenseOlfactoryInPercent;
				// CREATURE GUSTATORY
			case TargetSelectorExpressionType.OwnGustatorySense:
				return Status.SenseGustatoryInPercent;
				// CREATURE TOUCH
			case TargetSelectorExpressionType.OwnTactileSense:
				return Status.SenseTouchInPercent;

			case TargetSelectorExpressionType.OwnSlot0Amount:
				return Status.Inventory.SlotItemAmount( 0 );
			case TargetSelectorExpressionType.OwnSlot1Amount:
				return Status.Inventory.SlotItemAmount( 1 );
			case TargetSelectorExpressionType.OwnSlot2Amount:
				return Status.Inventory.SlotItemAmount( 2 );
			case TargetSelectorExpressionType.OwnSlot3Amount:
				return Status.Inventory.SlotItemAmount( 3 );
			case TargetSelectorExpressionType.OwnSlot4Amount:
				return Status.Inventory.SlotItemAmount( 4 );
			case TargetSelectorExpressionType.OwnSlot5Amount:
				return Status.Inventory.SlotItemAmount( 5 );
			case TargetSelectorExpressionType.OwnSlot6Amount:
				return Status.Inventory.SlotItemAmount( 6 );
			case TargetSelectorExpressionType.OwnSlot7Amount:
				return Status.Inventory.SlotItemAmount( 7 );
			case TargetSelectorExpressionType.OwnSlot8Amount:
				return Status.Inventory.SlotItemAmount( 8 );
			case TargetSelectorExpressionType.OwnSlot9Amount:
				return Status.Inventory.SlotItemAmount( 9 );

			case TargetSelectorExpressionType.OwnSlot0MaxAmount:
				return Status.Inventory.SlotItemMaxAmount( 0 );
			case TargetSelectorExpressionType.OwnSlot1MaxAmount:
				return Status.Inventory.SlotItemMaxAmount( 1 );
			case TargetSelectorExpressionType.OwnSlot2MaxAmount:
				return Status.Inventory.SlotItemMaxAmount( 2 );
			case TargetSelectorExpressionType.OwnSlot3MaxAmount:
				return Status.Inventory.SlotItemMaxAmount( 3 );
			case TargetSelectorExpressionType.OwnSlot4MaxAmount:
				return Status.Inventory.SlotItemMaxAmount( 4 );
			case TargetSelectorExpressionType.OwnSlot5MaxAmount:
				return Status.Inventory.SlotItemMaxAmount( 5 );
			case TargetSelectorExpressionType.OwnSlot6MaxAmount:
				return Status.Inventory.SlotItemMaxAmount( 6 );
			case TargetSelectorExpressionType.OwnSlot7MaxAmount:
				return Status.Inventory.SlotItemMaxAmount( 7 );
			case TargetSelectorExpressionType.OwnSlot8MaxAmount:
				return Status.Inventory.SlotItemMaxAmount( 8 );
			case TargetSelectorExpressionType.OwnSlot9MaxAmount:
				return Status.Inventory.SlotItemMaxAmount( 9 );

				// CREATURE ACTIVE TARGET TIME
			case TargetSelectorExpressionType.ActiveTargetTime:
				return ( m_ActiveTarget != null)?m_ActiveTarget.ActiveTime:0;
				// CREATURE ACTIVE TARGET TIME TOTAL
			case TargetSelectorExpressionType.ActiveTargetTimeTotal:
				return ( m_ActiveTarget != null)?m_ActiveTarget.ActiveTimeTotal:0;

			// TARGET 
			case TargetSelectorExpressionType.TargetSlot0Amount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemAmount( 0 ):0;
			case TargetSelectorExpressionType.TargetSlot1Amount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemAmount( 1 ):0;
			case TargetSelectorExpressionType.TargetSlot2Amount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemAmount( 2 ):0;
			case TargetSelectorExpressionType.TargetSlot3Amount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemAmount( 3 ):0;
			case TargetSelectorExpressionType.TargetSlot4Amount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemAmount( 4 ):0;
			case TargetSelectorExpressionType.TargetSlot5Amount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemAmount( 5 ):0;
			case TargetSelectorExpressionType.TargetSlot6Amount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemAmount( 6 ):0;
			case TargetSelectorExpressionType.TargetSlot7Amount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemAmount( 7 ):0;
			case TargetSelectorExpressionType.TargetSlot8Amount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemAmount( 8 ):0;
			case TargetSelectorExpressionType.TargetSlot9Amount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemAmount( 9 ):0;

			case TargetSelectorExpressionType.TargetSlot0MaxAmount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemMaxAmount( 0 ):0;
			case TargetSelectorExpressionType.TargetSlot1MaxAmount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemMaxAmount( 1 ):0;
			case TargetSelectorExpressionType.TargetSlot2MaxAmount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemMaxAmount( 2 ):0;
			case TargetSelectorExpressionType.TargetSlot3MaxAmount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemMaxAmount( 3 ):0;
			case TargetSelectorExpressionType.TargetSlot4MaxAmount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemMaxAmount( 4 ):0;
			case TargetSelectorExpressionType.TargetSlot5MaxAmount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemMaxAmount( 5 ):0;
			case TargetSelectorExpressionType.TargetSlot6MaxAmount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemMaxAmount( 6 ):0;
			case TargetSelectorExpressionType.TargetSlot7MaxAmount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemMaxAmount( 7 ):0;
			case TargetSelectorExpressionType.TargetSlot8MaxAmount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemMaxAmount( 8 ):0;
			case TargetSelectorExpressionType.TargetSlot9MaxAmount:
				return (_target.Inventory() != null)?_target.Inventory().SlotItemMaxAmount( 9 ):0;

				// TARGET CREATURE AGE
			case TargetSelectorExpressionType.CreatureAge:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.Age:0;

				// TARGET CREATURE ODOUR INTENSITY
			case TargetSelectorExpressionType.CreatureOdourIntensity:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.Odour.Intensity:0;
				// TARGET CREATURE ODOUR INTENSITY BY DISTANCE
			case TargetSelectorExpressionType.CreatureOdourIntensityByDistance:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.Odour.Intensity / (_target.TargetDistanceTo( m_Owner.transform.position )+1):0;
				// TARGET CREATURE ODOUR INTENSITY NET
			case TargetSelectorExpressionType.CreatureOdourIntensityNet:
				return (_target.Creature() != null)?(_target.Creature().Creature.Status.Odour.Intensity / (_target.TargetDistanceTo( m_Owner.transform.position )+1) * Status.SenseOlfactoryInPercent * 0.01f ):0;

				// TARGET CREATURE ODOUR RANGE
			case TargetSelectorExpressionType.CreatureOdourRange:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.Odour.Range:0;

				// TARGET CREATURE TEMPERATURE DEVIATION
			case TargetSelectorExpressionType.CreatureEnvTemperatureDeviation:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.TemperatureDeviationInPercent:0;

				// TARGET CREATURE FITNESS
			case TargetSelectorExpressionType.CreatureFitness:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.FitnessInPercent:0;
				// TARGET CREATURE HEALTH
			case TargetSelectorExpressionType.CreatureHealth:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.HealthInPercent:0;
				// CREATURE POWER
			case TargetSelectorExpressionType.CreaturePower:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.PowerInPercent:0;
				// TARGET CREATURE STAMINA
			case TargetSelectorExpressionType.CreatureStamina:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.StaminaInPercent:0;

				// TARGET CREATURE DAMAGE
			case TargetSelectorExpressionType.CreatureDamage:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.DamageInPercent:0;			
				// TARGET CREATURE DEBILITY
			case TargetSelectorExpressionType.CreatureDebility:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.DebilityInPercent:0;
				// CREATURE STRESS
			case TargetSelectorExpressionType.CreatureStress:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.StressInPercent:0;
				// TARGET CREATURE HUNGER
			case TargetSelectorExpressionType.CreatureHunger:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.HungerInPercent:0;
				// TARGET CREATURE THIRST
			case TargetSelectorExpressionType.CreatureThirst:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.ThirstInPercent:0;

				// TARGET CREATURE AGGRESSIVITY
			case TargetSelectorExpressionType.CreatureAggressivity:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.AggressivityInPercent:0;
				// TARGET CREATURE EXPERIENCE
			case TargetSelectorExpressionType.CreatureExperience:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.ExperienceInPercent:0;
				// TARGET CREATURE ANXIETY
			case TargetSelectorExpressionType.CreatureAnxiety:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.AggressivityInPercent:0;
				// TARGET CREATURE NOSINESS
			case TargetSelectorExpressionType.CreatureNosiness:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.NosinessInPercent:0;

				// TARGET CREATURE VISUAL
			case TargetSelectorExpressionType.CreatureVisualSense:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.SenseVisualInPercent:0;
				// CREATURE AUDITORY
			case TargetSelectorExpressionType.CreatureAuditorySense:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.SenseAuditoryInPercent:0;
				// CREATURE OLFACTORY
			case TargetSelectorExpressionType.CreatureOlfactorySense:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.SenseOlfactoryInPercent:0;
				// TARGET CREATURE GUSTATORY
			case TargetSelectorExpressionType.CreatureGustatorySense:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.SenseGustatoryInPercent:0;
				// TARGET CREATURE TOUCH
			case TargetSelectorExpressionType.CreatureTouchSense:
				return (_target.Creature() != null)?_target.Creature().Creature.Status.SenseTouchInPercent:0;



				// TARGET CREATURE ACTIVE TARGET TIME
			case TargetSelectorExpressionType.CreatureActiveTargetTime:
				return (_target.Creature() != null && _target.Creature().Creature.ActiveTarget != null)?_target.Creature().Creature.ActiveTarget.ActiveTime:0;
				// TARGET CREATURE ACTIVE TARGET TIME TOTAL
			case TargetSelectorExpressionType.CreatureActiveTargetTimeTotal:
				return (_target.Creature() != null && _target.Creature().Creature.ActiveTarget != null)?_target.Creature().Creature.ActiveTarget.ActiveTimeTotal:0;


				// TARGET MARKER ODOUR INTENSITY
			case TargetSelectorExpressionType.MarkerOdourIntensity:
				return (_target.Marker() != null)?_target.Marker().Odour.Intensity:0;
				// TARGET MARKER ODOUR INTENSITY BY DISTANCE
			case TargetSelectorExpressionType.MarkerOdourIntensityByDistance:
				return (_target.Marker() != null)?_target.Marker().Odour.Intensity / (_target.TargetDistanceTo( m_Owner.transform.position )+1):0;
				// TARGET MARKER ODOUR INTENSITY NET
			case TargetSelectorExpressionType.MarkerOdourIntensityNet:
				return (_target.Marker() != null)?(_target.Marker().Odour.Intensity / (_target.TargetDistanceTo( m_Owner.transform.position )+1) * Status.SenseOlfactoryInPercent * 0.01f ):0;
				// TARGET MARKER ODOUR RANGE
			case TargetSelectorExpressionType.MarkerOdourRange:
				return (_target.Marker() != null)?_target.Marker().Odour.Range:0;

				// TARGET DISTANCE
			case TargetSelectorExpressionType.TargetDistance:
				return _target.TargetDistanceTo( m_Owner.transform.position );
				// TARGET MOVE POSITION DISTANCE
			case TargetSelectorExpressionType.TargetOffsetPositionDistance:
				return _target.TargetOffsetPositionDistanceTo( m_Owner.transform.position );
				// TARGET MOVE POSITION DISTANCE
			case TargetSelectorExpressionType.TargetMovePositionDistance:
				return _target.TargetMovePositionDistanceTo( m_Owner.transform.position );

				// ENVIRONMENT
			case TargetSelectorExpressionType.EnvironmentDateDay:
				return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.DateDay:0);
			case TargetSelectorExpressionType.EnvironmentDateMonth:
				return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.DateMonth:0);
			case TargetSelectorExpressionType.EnvironmentDateYear:
				return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.DateYear:0);

			case TargetSelectorExpressionType.EnvironmentTimeHour:
				return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.TimeHour:0);
			case TargetSelectorExpressionType.EnvironmentTimeMinute:
				return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.TimeMinutes:0);
			case TargetSelectorExpressionType.EnvironmentTimeSecond:
				return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.TimeSeconds:0);

			case TargetSelectorExpressionType.EnvironmentTemperature:
				return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.Temperature:0);


			default:
				return 0;
			}
		}

		private bool CompareBooleanValue( TargetObject _target, TargetSelectorConditionObject _condition )
		{
			return CompareBoolean( GetBooleanValue( _target, _condition.ExpressionType ), _condition.BooleanValue, _condition.Operator );
		}

		private bool CompareStringValue( TargetObject _target, TargetSelectorConditionObject _condition )
		{
			if( _condition.UseDynamicValue )
				return CompareString( GetStringValue( _target, _condition.ExpressionType ), GetStringValue( _target, _condition.ExpressionValue ), _condition.Operator );
			else
				return CompareString( GetStringValue( _target, _condition.ExpressionType ), _condition.StringValue, _condition.Operator );
		}

		private bool CompareKeyCodeValue( TargetObject _target, TargetSelectorConditionObject _condition )
		{
			return CompareKeyCode( _condition.KeyCodeValue, _condition.Operator );
		}

		private bool CompareEnumValue( TargetObject _target, TargetSelectorConditionObject _condition )
		{
			return CompareNumber( GetEnumValue( _target, _condition.ExpressionType ), _condition.IntegerValue, _condition.Operator );
		}

		private bool CompareNumericValue( TargetObject _target, TargetSelectorConditionObject _condition )
		{
			if( _condition.UseDynamicValue )
				return CompareNumber( GetFloatValue( _target, _condition.ExpressionType ), GetFloatValue( _target, _condition.ExpressionValue ), _condition.Operator );
			else
				return CompareNumber( GetFloatValue( _target, _condition.ExpressionType ), _condition.FloatValue, _condition.Operator );
		}

		private bool CompareObjects( TargetObject _target, TargetSelectorConditionObject _condition )
		{
			return CompareObjects( GetGameObject( _target, _condition.ExpressionType ), GetGameObject( _target, _condition.ExpressionValue ), _condition.Operator );
		}

		public static bool CompareString( string _value_1, string _value_2, LogicalOperatorType _operator )
		{
			bool _result = false;

			if( _value_1 == _value_2 )
				_result = true;

			if( _operator == LogicalOperatorType.NOT )
				_result = ! _result;

			return _result;
		}

		public static bool CompareKeyCode( KeyCode _key, LogicalOperatorType _operator )
		{
			bool _result = Input.GetKey( _key );

			if( _operator == LogicalOperatorType.NOT )
				_result = ! _result;

			return _result;
		}

		public static bool CompareBoolean( bool _value_1, bool _value_2, LogicalOperatorType _operator )
		{
			bool _result = false;

			if( _value_1 == _value_2 )
				_result = true;

			if( _operator == LogicalOperatorType.NOT )
				_result = ! _result;

			return _result;
		}

		public static bool CompareNumber( float _value_1, float _value_2, LogicalOperatorType _operator )
		{
			bool _result = false;
			switch( _operator )
			{
			case LogicalOperatorType.EQUAL:
				_result = ( _value_1 == _value_2 )?true:false;
				break;
			case LogicalOperatorType.GREATER:
				_result = ( _value_1 > _value_2 )?true:false;
				break;
			case LogicalOperatorType.GREATER_OR_EQUAL:
				_result = ( _value_1 >= _value_2 )?true:false;
				break;
			case LogicalOperatorType.LESS:
				_result = ( _value_1 < _value_2 )?true:false;
				break;
			case LogicalOperatorType.LESS_OR_EQUAL:
				_result = ( _value_1 <= _value_2 )?true:false;
				break;
			case LogicalOperatorType.NOT:
				_result = ( _value_1 != _value_2 )?true:false;
				break;
			}

			return _result;
		}

		public static bool CompareObjects( GameObject _obj_1, GameObject _obj_2, LogicalOperatorType _operator )
		{
			if( _obj_1 == null || _obj_2 == null )
				return false;

			bool _result = false;
			switch( _operator )
			{
			case LogicalOperatorType.EQUAL:
				_result = ( _obj_1.GetInstanceID() == _obj_2.GetInstanceID() )?true:false;
				break;
			case LogicalOperatorType.NOT:
				_result = ( _obj_1.GetInstanceID() != _obj_2.GetInstanceID() )?true:false;
				break;
			}

			return _result;
		}
	}

	[System.Serializable]
	public class CreatureObject : System.Object
	{
		/*
		public CreatureObject( GameObject gameObject )
		{
			m_Owner = gameObject;
			
			Essentials.Init ( m_Owner );
			
			Genitor.Init ( m_Owner );
			
			Status.Init( m_Owner );
			Behaviour.Init( m_Owner );
			Move.Init( m_Owner );
			Environment.Init( m_Owner );
			Interaction.Init( m_Owner );
			
			ICECreatureRegister.Instance.Register( m_Owner );
		}*/

		private GameObject m_Owner = null;
		public GameObject Owner{
			get{return m_Owner;}
		}
		private ReferenceGroupObject m_Group = null;

		public void Init( GameObject _owner )
		{
			m_Owner = _owner;

			Move.Init( m_Owner );
			Essentials.Init ( m_Owner );
			Status.Init( m_Owner );
			Behaviour.Init( m_Owner );
			Characteristics.Init( m_Owner );
			Missions.Init ( m_Owner );
			Interaction.Init( m_Owner );
			Environment.Init( m_Owner );

			if( ICECreatureRegister.Instance != null )
				m_Group = ICECreatureRegister.Instance.Register( m_Owner );

			if( Essentials.BehaviourModeSpawn != "" )
				Behaviour.SetBehaviourModeByKey( Essentials.BehaviourModeSpawn );
		}

		public void Reset()
		{
			if( Application.isPlaying )
				return;

			m_Interaction = new InteractionObject();
			m_Essentials = new EssentialsObject();
			m_Status = new StatusObject();
			m_Behaviour = new BehaviourObject ();
			m_Missions = new MissionsObject();
			m_Move = new MoveObject();
			m_Environment = new EnvironmentObject();
		}

		public void Bye()
		{
			if( ICECreatureRegister.Instance != null )
				ICECreatureRegister.Instance.Deregister( m_Owner );
		}

		public void SendGroupMessage( BroadcastMessageDataObject _msg )
		{
			if( _msg == null )
				return;

			Debug.Log ( "send from ID : " + m_Owner.GetInstanceID() + " - " + _msg.Type.ToString() );

			if( m_Group != null )
				m_Group.Message( m_Owner, _msg );
		}

		public string LastGroupCommand = "";

		public void ReceiveGroupMessage( ReferenceGroupObject _group, GameObject _sender, BroadcastMessageDataObject _msg )
		{
			Debug.Log ( "receive from ID : " + _sender.GetInstanceID() + " - " + _msg.Type.ToString() );

			switch( _msg.Type )
			{
				case BroadcastMessageType.COMMAND:
					LastGroupCommand = _msg.Command;
					break;
				default:
					break;
			}
		}
/*
		~CreatureObject()
		{
			ICECreatureRegister.Instance.Deregister( m_Owner );
		}*/

		public bool DontDestroyOnLoad = false;
		public bool UseCoroutine = true;

		public void AutoDetectInteractors()
		{
			if( ICECreatureRegister.Instance == null )
				return;

			foreach( ReferenceGroupObject _group in ICECreatureRegister.Instance.ReferenceGroupObjects )
			{
				Interaction.AddInteractor( _group.Reference , Status );
			}
				
		}

		[SerializeField]
		private InteractionObject m_Interaction = new InteractionObject();
		public InteractionObject Interaction{
			set{ m_Interaction = value; }
			get{ return m_Interaction; }
		}

		[SerializeField]
		private CharacteristicsObject m_Characteristics = new CharacteristicsObject();
		public CharacteristicsObject Characteristics{
			set{ m_Characteristics = value; }
			get{ return m_Characteristics; }
		}

		[SerializeField]
		private EssentialsObject m_Essentials = new EssentialsObject();
		public EssentialsObject Essentials{
			set{ m_Essentials = value; }
			get{ return m_Essentials; }
		}

		[SerializeField]
		private StatusObject m_Status = new StatusObject();
		public StatusObject Status{
			set{ m_Status = value; }
			get{ return m_Status; }
		}

		[SerializeField]
		private BehaviourObject  m_Behaviour = new BehaviourObject ();
		public BehaviourObject Behaviour{
			set{ m_Behaviour = value; }
			get{ return m_Behaviour; }
		}

		[SerializeField]
		private MissionsObject m_Missions = new MissionsObject();
		public MissionsObject Missions{
			set{ m_Missions = value; }
			get{ return m_Missions; }
		}

		[SerializeField]
		private MoveObject m_Move = new MoveObject();
		public MoveObject Move{
			set{ m_Move = value; }
			get{ return m_Move; }
		}

		[SerializeField]
		private EnvironmentObject m_Environment = new EnvironmentObject();
		public EnvironmentObject Environment{
			set{ m_Environment = value; }
			get{ return m_Environment; }
		}

		private TargetObject m_ActiveTarget = null;				
		public TargetObject ActiveTarget{
			get{ return m_ActiveTarget; }
		}
					
		public float ActiveTargetMovePositionDistance{
			get{ return ( ActiveTarget.IsValid?ActiveTarget.TargetMovePositionDistanceTo( m_Owner.transform.position ):0 ); }
		}

		public float ActiveTargetOffsetPositionDistance{
			get{ return ( ActiveTarget.IsValid?ActiveTarget.TargetOffsetPositionDistanceTo( m_Owner.transform.position ):0 ); }
		}

		public float ActiveTargetDistance{
			get{ return ( ActiveTarget.IsValid?ActiveTarget.TargetDistanceTo( m_Owner.transform.position ):0 ); }
		}

		private TargetObject m_PreviousTarget = null;				
		public TargetObject PreviousTarget{
			get{ return m_PreviousTarget; }
		}

		public bool m_TargetChanged = false;
		public bool TargetChanged{
			get{ return m_TargetChanged; }
		}

		public string ActiveTargetName
		{
			get{
				if( m_ActiveTarget != null && m_ActiveTarget.TargetGameObject != null )
					return m_ActiveTarget.TargetGameObject.name;
				else
					return "";
			}
		}

		public float ActiveTargetVelocity
		{
			get{
				if( m_ActiveTarget != null && m_ActiveTarget.TargetGameObject != null )
					return m_ActiveTarget.TargetVelocity;
				else
					return 0;
			}
		}

		public string PreviousTargetName
		{
			get{
				if( m_PreviousTarget != null && m_PreviousTarget.TargetGameObject != null )
					return m_PreviousTarget.TargetGameObject.name;
				else
					return "";
			}
		}

		public bool IsSenseTime( Transform transform )
		{
			return Status.IsSenseTime();
		}
			
		public bool IsReactionTime( Transform transform )
		{
			if( Status.IsReactionTime() ) 
				return true;
			else if ( ActiveTarget == null )
				return true;
			else if( ActiveTarget.TargetMoveComplete )
				return true;
			else
				return false;			
		}

		public TargetObject FindTargetByName( string _name )
		{
			if( _name == "" )
				return null;

			TargetObject _target = null;

			if( _target == null && Essentials.TargetReady() && Essentials.Target.TargetGameObject.name == _name )
				_target = Essentials.Target;
			if( _target == null && Missions.Outpost.Enabled && Missions.Outpost.TargetReady() && Missions.Outpost.Target.TargetGameObject.name == _name )
				_target = Missions.Outpost.Target;
			if( _target == null && Missions.Escort.Enabled && Missions.Escort.TargetReady() && Missions.Escort.Target.TargetGameObject.name == _name )
				_target = Missions.Escort.Target;
			if( _target == null && Missions.Patrol.Enabled )
				_target = Missions.Patrol.Waypoints.GetWaypointByName( _name );
			/*if( _target == null )
				_target = Interaction.GetInteractorByName( _name );*/

			return _target;
		}

		public void AddAvailableTarget( TargetObject _target )
		{
			if( _target != null && _target.IsValid && _target.TargetGameObject.activeInHierarchy && _target.TargetGameObject.transform.IsChildOf( m_Owner.transform ) == false && Behaviour.BehaviourModeExists( _target.BehaviourModeKey ) )
				AvailableTargets.Add ( _target );
		}

		private List<TargetObject> m_AvailableTargets = new List<TargetObject>();
		public List<TargetObject> AvailableTargets{
			get{return m_AvailableTargets; }
		}

		public void SelectBestTarget()
		{
			SelectorObject _selector = new SelectorObject( this );
			SetActiveTarget( _selector.SelectBestTarget( m_AvailableTargets ) );
		}

		/// <summary>
		/// Sets the active target.
		/// </summary>
		/// <param name="_target">_target.</param>
		public void SetActiveTarget( TargetObject _target )
		{
			if( Status.IsDead || Status.IsSpawning )
				return;

			if( Move.Deadlocked )
			{
				Move.ResetDeadlock();
				if( Move.DeadlockAction == DeadlockActionType.BEHAVIOUR )
				{
					Behaviour.SetBehaviourModeByKey( Move.DeadlockBehaviour );
					return;
				}
				else if( Move.DeadlockAction == DeadlockActionType.UPDATE && m_ActiveTarget != null )
					m_ActiveTarget.UpdateTargetMovePosition();
				else
				{
					Status.Kill();
					return;
				}
			}


			if( _target == null || Status.RecreationRequired )
				_target = Essentials.Target;

			if( _target == null || ! _target.IsValid )
			{
				Debug.Log( "Sorry, your creature '" + m_Owner.name + "' have no target!" );
				return;
			}

			if( IsTargetUpdatePermitted( _target ) )
			{
				if( m_ActiveTarget != _target )
				{
					m_PreviousTarget = m_ActiveTarget;

					if( m_PreviousTarget != null )
						m_PreviousTarget.SetActive( false );
			
					m_ActiveTarget = _target;

					if( m_ActiveTarget != null )
					{
						m_ActiveTarget.SetActive( true );

						if( m_ActiveTarget.GroupMessage.Type != BroadcastMessageType.NONE )
						{
							BroadcastMessageDataObject _data = new BroadcastMessageDataObject();

							_data.Type = m_ActiveTarget.GroupMessage.Type;
							_data.TargetGameObject = m_ActiveTarget.TargetGameObject;

							SendGroupMessage( _data );
						}
					}
			
					m_TargetChanged = true;
				}

				if( m_ActiveTarget != null )
				{
					if( ActiveTarget.BehaviourModeKey == "" )
					{
						Debug.LogWarning( "CAUTION : The Active Target " + ActiveTarget.TargetName + " of creature '" + m_Owner.gameObject.name.ToUpper() + "' HAVE NO BEHAVIOUR!");

					}
					else
					{
						Behaviour.SetBehaviourModeByKey( ActiveTarget.BehaviourModeKey );
					}
				}
					
			}
		}

		private bool IsTargetUpdatePermitted( TargetObject _target )
		{
			if( _target == null )
				return false;
			
			if( m_ActiveTarget == null || Behaviour.BehaviourMode == null || Behaviour.BehaviourMode.Favoured.Enabled == false)
				return true;
			
			bool _permitted = true;
			
			if( ( Behaviour.BehaviourMode.Favoured.Enabled == true ) && (
				( Behaviour.BehaviourMode.Favoured.Runtime > 0 && Behaviour.BehaviourTimer < Behaviour.BehaviourMode.Favoured.Runtime ) ||
				( Behaviour.BehaviourMode.Favoured.FavouredUntilNextMovePositionReached && ! Move.MovePositionReached ) ||
				( Behaviour.BehaviourMode.Favoured.FavouredUntilTargetMovePositionReached && ! m_ActiveTarget.TargetMoveComplete ) ||
				( Behaviour.BehaviourMode.Favoured.FavouredUntilNewTargetInRange( _target, Vector3.Distance( _target.TargetGameObject.transform.position, m_Owner.transform.position ) ) ) ||
				( Behaviour.BehaviourMode.HasActiveDetourRule && Behaviour.BehaviourMode.Favoured.FavouredUntilDetourPositionReached && ! Move.DetourComplete ) ) )
				_permitted = false;
			else
				_permitted = true;
			
			//mode check - the new mode could be also forced, so we have to check this here 
			if( _permitted == false )
			{
				BehaviourModeObject _mode = Behaviour.GetBehaviourModeByKey( _target.BehaviourModeKey );
				
				if( _mode != null && _mode.Favoured.Enabled == true )
				{
					if( Behaviour.BehaviourMode.Favoured.FavouredPriority > _mode.Favoured.FavouredPriority )
						_permitted = false;
					else if( Behaviour.BehaviourMode.Favoured.FavouredPriority < _mode.Favoured.FavouredPriority ) 
						_permitted = true;
					else 
						_permitted = (Random.Range(0,1) == 0?false:true);
				}
			}
			
			
			return _permitted;
		}

		//--------------------------------------------------

		public void UpdateMove()
		{
			if( Status.IsDead )
			{
				Move.StopMove();
				return;
			}

			Move.UpdateMove( m_ActiveTarget, Behaviour.ActiveBehaviourModeRule );

			Environment.SurfaceHandler.Update( this );
		}

		//--------------------------------------------------

		public void UpdateBegin()
		{
			Status.UpdateBegin( Move.MoveVelocity );
			Behaviour.UpdateBegin();
						
			if( m_ActiveTarget != null )
				m_ActiveTarget.Update( m_Owner );	
		}

		public void UpdateComplete()
		{
			if( Behaviour.BehaviourMode != null && Behaviour.BehaviourMode.Rule != null )
				Status.Inventory.Action( Behaviour.BehaviourMode.Rule.Inventory );
		}

		//--------------------------------------------------
		public void UpdateStatusInfluences( StatusContainer _status )
		{
			Status.AddDamage( _status.Damage );
			Status.AddStress( _status.Stress );
			Status.AddDebility( _status.Debility );
			Status.AddHunger( _status.Hunger );
			Status.AddThirst( _status.Thirst );

			Status.AddAggressivity( _status.Aggressivity );
			Status.AddAnxiety( _status.Anxiety );
			Status.AddExperience( _status.Experience );
			Status.AddNosiness( _status.Nosiness );

			if( _status.Odour.Enabled )
				Status.Odour.SetOdour( _status.Odour );
		}

		//--------------------------------------------------
		public void FixedUpdate()
		{
			//TODO: obsolete
			if( Missions.Outpost.TargetReady() )
				Missions.Outpost.Target.FixedUpdate();
			if( Missions.Escort.TargetReady() )
				Missions.Escort.Target.FixedUpdate();
			if( Missions.Patrol.TargetReady() )
				Missions.Patrol.Target.FixedUpdate();
			if( Interaction.TargetsReady() )
				Interaction.FixedUpdate();

			if( ActiveTarget != null && ActiveTarget.IsValid )
			{
				ActiveTarget.FixedUpdate();

				if( ActiveTarget.Influences.Enabled &&
					ActiveTarget.Influences.Ready() )
				{
					UpdateStatusInfluences( ActiveTarget.Influences );
				}
			}

			if( Behaviour.BehaviourMode != null && 
			   Behaviour.BehaviourMode.Rule != null && 
			   Behaviour.BehaviourMode.Rule.Influences.Enabled &&
			   Behaviour.BehaviourMode.Rule.Influences.Ready() )
			{
				UpdateStatusInfluences( Behaviour.BehaviourMode.Rule.Influences );
			}

			if( Environment.SurfaceHandler.ActiveSurface != null )
			{
				if( Environment.SurfaceHandler.ActiveSurface.Influences.Enabled == true &&
			  		Environment.SurfaceHandler.ActiveSurface.Influences.Ready() )
				{
					UpdateStatusInfluences( Environment.SurfaceHandler.ActiveSurface.Influences );
				}

				if( Environment.SurfaceHandler.ActiveSurface.BehaviourModeKey != "" )
					Behaviour.SetBehaviourModeByKey( Environment.SurfaceHandler.ActiveSurface.BehaviourModeKey );
			}



			Status.FixedUpdate();

			if( Status.IsDead )
			{
				Behaviour.SetBehaviourModeByKey( Essentials.BehaviourModeDead );
				Status.RespawnRequest();
			}

			if( Status.IsRespawnTime )
				Status.Respawn();

		}

		public bool m_IsVisible = false;
		public void HandleVisibility( bool _visible )
		{	/*
				m_IsVisible = _visible;
				if( m_IsVisible )
					Debug.Log( m_Owner.name + " is visible!" );
				else
					Debug.Log( m_Owner.name + " is invisible!" );
		
			*/
		}

		public void HandleCollider( Collider _collider, string _contact = "" )
		{
			if( Status.IsDead )
				return;

			//Move.HandleCollision( _collision );

			if( Environment.CollisionHandler.Enabled )
			{
				foreach( CollisionDataObject _data in Environment.CollisionHandler.CheckCollider( _collider, _contact ) )
				{

					if( _data != null )
					{
						if( _data.Influences.Ready() )
							UpdateStatusInfluences( _data.Influences );
		
						if( _data.BehaviourModeKey != "" )
							Behaviour.SetBehaviourModeByKey( _data.BehaviourModeKey );
					}
				}
			}
		}
	}
}
