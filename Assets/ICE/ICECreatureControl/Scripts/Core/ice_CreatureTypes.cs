// ##############################################################################
//
// ice_CreatureTypes.cs
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

namespace ICE.Creatures.EnumTypes
{
	/*
	public enum OdourType
	{
		NONE=0,
		SWEATY,
		SPERMOUS,
		FISHY,
		MALTY,
		MUSKY,
		URINOUS,
		MINTY,
		CAMPHORACEOUS,
		UNDEFINED
	}*/

	public enum OdourType
	{
		NONE=0,
		CAMPHORACEOUS, //Camphoraceous – mothballs
		MUSKY, //Musky – perfumes/aftershave
		FLORAL, //Floral – roses
		MINTY, //Pepperminty – mint gum
		ETHEREAL,//Ethereal – dry cleaning fluid
		PUNGENT, //Pungent – vinegar
		PUTRID, //Putrid – rotten eggs
		UNDEFINED
	}










	public enum HierarchyGroupType
	{
		Creatures,
		Players,
		Items,
		Locations,
		Waypoints,
		Markers,
		Other
	}

	public enum AttributeType
	{
		TARGET,
		INFLUENCE
	}

	public enum TargetAccessType
	{
		OBJECT,
		NAME,
		TAG
	}

	public enum RegisterReferenceType
	{
		NONE,
		PLAYER,
		CREATURE,
		ITEM,
		LOCATION,
		WAYPOINT,
		MARKER,
		UNDEFINED,
		ERROR
	}

	public enum SelectionStatus
	{
		UNCHECKED,
		VALID,
		INVALID
	}

	public enum NetworkAdapterType
	{
		NONE,
		UN,
		PUN	
	}

	public enum MotionControlType
	{
		INTERNAL,
		NAVMESHAGENT,
		RIGIDBODY,
		CHARACTERCONTROLLER,
		CUSTOM
	}

	public enum DeadlockActionType
	{
		DIE=0,
		BEHAVIOUR,
		UPDATE
	}

	public enum ViewingDirectionType
	{
		DEFAULT = 0,
		OFFSET,
		MOVE,
		CENTER,
		POSITION,
		NONE
	}

	public enum RandomSeedType
	{
		DEFAULT = 0,
		TIME,
		CUSTOM
	}

	public enum CreatureTrophicLevelType
	{
		UNDEFINED=0,
		HERBIVORE,
		OMNIVORES,
		CARNIVORE
	}

	public enum DisplayOptionType
	{
		START,
		BASIC,
		FULL,
		MENU
	}

	public enum SequenceOrderType
	{
		CYCLE,
		RANDOM,
		PINGPONG
	}

	public enum RandomOffsetType
	{
		EXACT,
		CIRCLE,
		HEMISPHERE,
		SPHERE
	}

	public enum NavMeshType
	{
		NONE,
		PERMANENT,
		AVOIDANCE,
		COLLISION,
		DEADLOCKED
	}

	public enum ObstacleCheckType
	{
		NONE,
		BASIC
	}

	public enum GroundOrientationType
	{
		DEFAULT=0,
		BIPED,
		QUADRUPED,
		CRAWLER,
		AVIAN
	}

	public enum CollisionType
	{
		NONE,
		TERRAIN,
		MESH,
		UNKNOWN
	}

	public enum WaypointOrderType
	{
		PINGPONG,
		CYCLE,
		RANDOM
	}

	public enum MissionType
	{
		HOME,
		ESCORT,
		PATROL
	}

	public enum TargetSelectionType
	{
		OBJECT,
		NAME,
		TAG
	}

	public enum TargetType
	{
		UNDEFINED,
		HOME,
		OUTPOST,
		ESCORT,
		PATROL,
		WAYPOINT,
		INTERACTOR,
		ITEM,
		PLAYER,
		CREATURE
	}

	public enum BooleanValueType
	{
		TRUE=0,
		FALSE
	}

	public enum StringOperatorType
	{
		EQUAL,
		NOT
	}
	
	public enum LogicalOperatorType
	{
		EQUAL = 0,
		NOT = 1,
		LESS = 2,
		LESS_OR_EQUAL = 3,
		GREATER = 4,
		GREATER_OR_EQUAL = 5
	}
	
	public enum ConditionalOperatorType
	{
		AND = 0,
		OR = 1
	}

	public enum TargetSuccessorType
	{
		TYPE=0,
		NAME,
		TAG
	}

	public enum DynamicBooleanValueType
	{
		CreatureIsGrounded,
		CreatureDeadlocked,
		CreatureMovePositionReached,
		CreatureMovePositionUpdateRequired,
		CreatureTargetMovePositionReached
	}

	public enum DynamicIntegerValueType
	{
		undefined
	}

	public enum DynamicFloatValueType
	{
		CreatureForwardSpeed,
		CreatureAngularSpeed,
		CreatureDirection,
		CreatureMovePositionDistance
	}

	public static class TargetSelectorExpression{

		public static TargetSelectorExpressionType Type( ref TargetSelectorExpressionType _type, string _key )
		{
			if( _type.ToString() != _key )
				_type = StrToType( _key );

			return _type;
		}

		public static TargetSelectorExpressionType StrToType( string _key )
		{
			int _count = (int)TargetSelectorExpressionType.InputKey;
			for( int _i = 0 ; _i <= _count; _i++ )
			{
				TargetSelectorExpressionType _type = (TargetSelectorExpressionType)(_i);
				if( _type.ToString() == _key )
					return _type;
			}

			return TargetSelectorExpressionType.OwnGameObject;
		}

		public static string TypeToStr( TargetSelectorExpressionType _type ){
			return _type.ToString();
		}



		public static string[] ToArray(){
			int _count = (int)TargetSelectorExpressionType.EnvironmentTemperature;

			string[] _array = new string[_count];
			for( int _i = 0 ; _i <= _count; _i++ )
			{
				TargetSelectorExpressionType _type = (TargetSelectorExpressionType)(_i);
				_array[_i] = _type.ToString();
			}

			return _array;
		}

		public static bool IsObjectValue( TargetSelectorExpressionType _type )
		{
			if( DataType( _type ) == TargetSelectorExpressionDataType.OBJECT )
				return true;
			else
				return false;
		}

		public static bool IsNumericValue( TargetSelectorExpressionType _type )
		{
			if( DataType( _type ) == TargetSelectorExpressionDataType.NUMBER ||
			  	DataType( _type ) == TargetSelectorExpressionDataType.DYNAMICNUMBER )
				return true;
			else
				return false;
		}

		public static bool IsEnumValue( TargetSelectorExpressionType _type )
		{
			if( DataType( _type ) == TargetSelectorExpressionDataType.ENUM )
				return true;
			else
				return false;
		}

		public static bool IsStringValue( TargetSelectorExpressionType _type )
		{
			if( DataType( _type ) == TargetSelectorExpressionDataType.STRING )
				return true;
			else
				return false;
		}

		public static bool IsKeyCodeValue( TargetSelectorExpressionType _type )
		{
			if( DataType( _type ) == TargetSelectorExpressionDataType.KEYCODE )
				return true;
			else
				return false;
		}

		public static bool IsBooleanValue( TargetSelectorExpressionType _type )
		{
			if( DataType( _type ) == TargetSelectorExpressionDataType.BOOLEAN )
				return true;
			else
				return false;
		}

		public static bool IsDynamicValue( TargetSelectorExpressionType _type )
		{
			if( DataType( _type ) == TargetSelectorExpressionDataType.DYNAMICNUMBER )
				return true;
			else
				return false;
		}

		public static bool NeedLogicalOperator( TargetSelectorExpressionType _type )
		{
			if( IsNumericValue( _type ) )
				return true;
			else
				return false;
		}

		public static TargetSelectorExpressionDataType DataType( TargetSelectorExpressionType _type )
		{
			switch( _type )
			{
				// DYNAMIC NUMBERS
				case TargetSelectorExpressionType.OwnAge:
				case TargetSelectorExpressionType.OwnOdourIntensity:
				case TargetSelectorExpressionType.OwnOdourRange:
				case TargetSelectorExpressionType.OwnDamage:
				case TargetSelectorExpressionType.OwnDebility:
				case TargetSelectorExpressionType.OwnFitness:
				case TargetSelectorExpressionType.OwnHealth:
				case TargetSelectorExpressionType.OwnHunger:
				case TargetSelectorExpressionType.OwnPower:
				case TargetSelectorExpressionType.OwnStamina:
				case TargetSelectorExpressionType.OwnStress:
				case TargetSelectorExpressionType.OwnThirst:
				case TargetSelectorExpressionType.OwnAggressivity:
				case TargetSelectorExpressionType.OwnExperience:
				case TargetSelectorExpressionType.OwnAnxiety:
				case TargetSelectorExpressionType.OwnNosiness:
				case TargetSelectorExpressionType.OwnVisualSense:
				case TargetSelectorExpressionType.OwnAuditorySense:
				case TargetSelectorExpressionType.OwnOlfactorySense:
				case TargetSelectorExpressionType.OwnGustatorySense:
				case TargetSelectorExpressionType.OwnTactileSense:

				case TargetSelectorExpressionType.OwnSlot0Amount:
				case TargetSelectorExpressionType.OwnSlot1Amount:
				case TargetSelectorExpressionType.OwnSlot2Amount:
				case TargetSelectorExpressionType.OwnSlot3Amount:
				case TargetSelectorExpressionType.OwnSlot4Amount:
				case TargetSelectorExpressionType.OwnSlot5Amount:
				case TargetSelectorExpressionType.OwnSlot6Amount:
				case TargetSelectorExpressionType.OwnSlot7Amount:
				case TargetSelectorExpressionType.OwnSlot8Amount:
				case TargetSelectorExpressionType.OwnSlot9Amount:

				case TargetSelectorExpressionType.OwnSlot0MaxAmount:
				case TargetSelectorExpressionType.OwnSlot1MaxAmount:
				case TargetSelectorExpressionType.OwnSlot2MaxAmount:
				case TargetSelectorExpressionType.OwnSlot3MaxAmount:
				case TargetSelectorExpressionType.OwnSlot4MaxAmount:
				case TargetSelectorExpressionType.OwnSlot5MaxAmount:
				case TargetSelectorExpressionType.OwnSlot6MaxAmount:
				case TargetSelectorExpressionType.OwnSlot7MaxAmount:
				case TargetSelectorExpressionType.OwnSlot8MaxAmount:
				case TargetSelectorExpressionType.OwnSlot9MaxAmount:


				
				case TargetSelectorExpressionType.OwnEnvTemperatureDeviation:

				case TargetSelectorExpressionType.TargetSlot0Amount:
				case TargetSelectorExpressionType.TargetSlot1Amount:
				case TargetSelectorExpressionType.TargetSlot2Amount:
				case TargetSelectorExpressionType.TargetSlot3Amount:
				case TargetSelectorExpressionType.TargetSlot4Amount:
				case TargetSelectorExpressionType.TargetSlot5Amount:
				case TargetSelectorExpressionType.TargetSlot6Amount:
				case TargetSelectorExpressionType.TargetSlot7Amount:
				case TargetSelectorExpressionType.TargetSlot8Amount:
				case TargetSelectorExpressionType.TargetSlot9Amount:

				case TargetSelectorExpressionType.TargetSlot0MaxAmount:
				case TargetSelectorExpressionType.TargetSlot1MaxAmount:
				case TargetSelectorExpressionType.TargetSlot2MaxAmount:
				case TargetSelectorExpressionType.TargetSlot3MaxAmount:
				case TargetSelectorExpressionType.TargetSlot4MaxAmount:
				case TargetSelectorExpressionType.TargetSlot5MaxAmount:
				case TargetSelectorExpressionType.TargetSlot6MaxAmount:
				case TargetSelectorExpressionType.TargetSlot7MaxAmount:
				case TargetSelectorExpressionType.TargetSlot8MaxAmount:
				case TargetSelectorExpressionType.TargetSlot9MaxAmount:
				
				case TargetSelectorExpressionType.CreatureAge:
				case TargetSelectorExpressionType.CreatureOdourIntensity:
				case TargetSelectorExpressionType.CreatureOdourIntensityNet:
				case TargetSelectorExpressionType.CreatureOdourIntensityByDistance:
				case TargetSelectorExpressionType.CreatureOdourRange:
				case TargetSelectorExpressionType.CreatureDamage:
				case TargetSelectorExpressionType.CreatureDebility:
				case TargetSelectorExpressionType.CreatureFitness:
				case TargetSelectorExpressionType.CreatureHealth:
				case TargetSelectorExpressionType.CreatureHunger:
				case TargetSelectorExpressionType.CreaturePower:
				case TargetSelectorExpressionType.CreatureStamina:
				case TargetSelectorExpressionType.CreatureStress:
				case TargetSelectorExpressionType.CreatureThirst:
				case TargetSelectorExpressionType.CreatureAggressivity:
				case TargetSelectorExpressionType.CreatureExperience:
				case TargetSelectorExpressionType.CreatureAnxiety:
				case TargetSelectorExpressionType.CreatureNosiness:
				case TargetSelectorExpressionType.CreatureVisualSense:
				case TargetSelectorExpressionType.CreatureAuditorySense:
				case TargetSelectorExpressionType.CreatureOlfactorySense:
				case TargetSelectorExpressionType.CreatureGustatorySense:
				case TargetSelectorExpressionType.CreatureTouchSense:
				case TargetSelectorExpressionType.CreatureEnvTemperatureDeviation:

				case TargetSelectorExpressionType.MarkerOdourIntensity:
				case TargetSelectorExpressionType.MarkerOdourIntensityNet:
				case TargetSelectorExpressionType.MarkerOdourIntensityByDistance:
				case TargetSelectorExpressionType.MarkerOdourRange:
				
				case TargetSelectorExpressionType.TargetDistance:
				case TargetSelectorExpressionType.TargetOffsetPositionDistance:
				case TargetSelectorExpressionType.TargetMovePositionDistance:
					return TargetSelectorExpressionDataType.DYNAMICNUMBER;

				// STATIC NUMBERS
				case TargetSelectorExpressionType.ActiveTargetTime:
				case TargetSelectorExpressionType.ActiveTargetTimeTotal:

				case TargetSelectorExpressionType.CreatureActiveTargetTime:
				case TargetSelectorExpressionType.CreatureActiveTargetTimeTotal:

				case TargetSelectorExpressionType.EnvironmentTimeHour:
				case TargetSelectorExpressionType.EnvironmentTimeMinute:
				case TargetSelectorExpressionType.EnvironmentTimeSecond:
				case TargetSelectorExpressionType.EnvironmentDateYear:
				case TargetSelectorExpressionType.EnvironmentDateMonth:
				case TargetSelectorExpressionType.EnvironmentDateDay:
				case TargetSelectorExpressionType.EnvironmentTemperature:
					return TargetSelectorExpressionDataType.NUMBER;

				// STRINGS
				case TargetSelectorExpressionType.OwnCommand:
				case TargetSelectorExpressionType.OwnBehaviour:
				case TargetSelectorExpressionType.ActiveTargetName:
				case TargetSelectorExpressionType.ActiveTargetParentName:
				case TargetSelectorExpressionType.TargetName:
				case TargetSelectorExpressionType.TargetParentName:
				case TargetSelectorExpressionType.CreatureBehaviour:
				case TargetSelectorExpressionType.CreatureCommand:
					return TargetSelectorExpressionDataType.STRING;

				// BOOLEAN
				case TargetSelectorExpressionType.OwnIsDead:
				case TargetSelectorExpressionType.OwnIsSheltered:
				case TargetSelectorExpressionType.OwnIsIndoor:
				case TargetSelectorExpressionType.CreatureIsDead:
				case TargetSelectorExpressionType.CreatureIsSheltered:
				case TargetSelectorExpressionType.CreatureIsIndoor:
				case TargetSelectorExpressionType.ActiveTargetHasParent:
				case TargetSelectorExpressionType.TargetHasParent:
					return TargetSelectorExpressionDataType.BOOLEAN;

				// ENUM
				case TargetSelectorExpressionType.TargetReferenceType:
				case TargetSelectorExpressionType.OwnOdour:
				case TargetSelectorExpressionType.CreatureOdour:
				case TargetSelectorExpressionType.MarkerOdour:
				case TargetSelectorExpressionType.EnvironmentWeather:
					return TargetSelectorExpressionDataType.ENUM;

				// OBJECTS
				case TargetSelectorExpressionType.OwnGameObject:
				case TargetSelectorExpressionType.ActiveTarget:					
				case TargetSelectorExpressionType.TargetGameObject:
				case TargetSelectorExpressionType.Creature:
				case TargetSelectorExpressionType.CreatureActiveTarget:
				case TargetSelectorExpressionType.Player:
				case TargetSelectorExpressionType.Location:
				case TargetSelectorExpressionType.Waypoint:
				case TargetSelectorExpressionType.Item:
				case TargetSelectorExpressionType.Marker:
					return TargetSelectorExpressionDataType.OBJECT;

				// KEYCODE
				case TargetSelectorExpressionType.InputKey:
					return TargetSelectorExpressionDataType.KEYCODE;

				default:
					return TargetSelectorExpressionDataType.UNDEFINED;
			}
		}
	}

	public enum TargetSelectorExpressionDataType
	{
		UNDEFINED,
		NUMBER,
		DYNAMICNUMBER,
		STRING,
		BOOLEAN,
		ENUM,
		OBJECT,
		KEYCODE
	}
	
	public enum TargetSelectorExpressionType
	{
		OwnGameObject,
		OwnBehaviour,
		OwnCommand,
		OwnAge,
		OwnOdour,
		OwnOdourIntensity,
		OwnOdourRange,
		OwnEnvTemperatureDeviation,
		OwnFitness,
		OwnHealth,
		OwnStamina,
		OwnPower,
		OwnDamage,
		OwnStress,
		OwnDebility,
		OwnHunger,
		OwnThirst,
		OwnAggressivity,
		OwnExperience,
		OwnAnxiety,
		OwnNosiness,

		OwnVisualSense,
		OwnAuditorySense,
		OwnOlfactorySense,
		OwnGustatorySense,
		OwnTactileSense,

		OwnSlot0Amount,
		OwnSlot1Amount,
		OwnSlot2Amount,
		OwnSlot3Amount,
		OwnSlot4Amount,
		OwnSlot5Amount,
		OwnSlot6Amount,
		OwnSlot7Amount,
		OwnSlot8Amount,
		OwnSlot9Amount,

		OwnSlot0MaxAmount,
		OwnSlot1MaxAmount,
		OwnSlot2MaxAmount,
		OwnSlot3MaxAmount,
		OwnSlot4MaxAmount,
		OwnSlot5MaxAmount,
		OwnSlot6MaxAmount,
		OwnSlot7MaxAmount,
		OwnSlot8MaxAmount,
		OwnSlot9MaxAmount,

		OwnPosition,
		OwnIsDead,
		OwnIsSheltered,
		OwnIsIndoor,

		ActiveTarget,
		ActiveTargetName,
		ActiveTargetTime,
		ActiveTargetTimeTotal,
		ActiveTargetHasParent,
		ActiveTargetParentName,

		TargetGameObject,
		TargetName,
		TargetHasParent,
		TargetParentName,
		TargetDistance,
		TargetOffsetPositionDistance,
		TargetMovePositionDistance,
		TargetReferenceType,

		TargetSlot0Amount,
		TargetSlot1Amount,
		TargetSlot2Amount,
		TargetSlot3Amount,
		TargetSlot4Amount,
		TargetSlot5Amount,
		TargetSlot6Amount,
		TargetSlot7Amount,
		TargetSlot8Amount,
		TargetSlot9Amount,

		TargetSlot0MaxAmount,
		TargetSlot1MaxAmount,
		TargetSlot2MaxAmount,
		TargetSlot3MaxAmount,
		TargetSlot4MaxAmount,
		TargetSlot5MaxAmount,
		TargetSlot6MaxAmount,
		TargetSlot7MaxAmount,
		TargetSlot8MaxAmount,
		TargetSlot9MaxAmount,

		Creature,
		CreatureActiveTarget,
		CreatureActiveTargetTime,
		CreatureActiveTargetTimeTotal,
		CreatureBehaviour,
		CreatureCommand,
		CreatureAge,
		CreatureOdour,
		CreatureOdourIntensity,
		CreatureOdourIntensityNet,
		CreatureOdourIntensityByDistance,
		CreatureOdourRange,
		CreatureEnvTemperatureDeviation,
		CreatureFitness,
		CreatureHealth,
		CreatureStamina,
		CreaturePower,
		CreatureDamage,
		CreatureStress,
		CreatureDebility,
		CreatureHunger,
		CreatureThirst,
		CreatureAggressivity,
		CreatureExperience,
		CreatureAnxiety,
		CreatureNosiness,
		
		CreatureVisualSense,
		CreatureAuditorySense,
		CreatureOlfactorySense,
		CreatureGustatorySense,
		CreatureTouchSense,
		
		CreaturePosition,
		CreatureIsDead,
		CreatureIsSheltered,
		CreatureIsIndoor,

		Player,
		Location,
		Waypoint,
		Item,

		Marker,
		MarkerOdour,
		MarkerOdourIntensity,
		MarkerOdourIntensityNet,
		MarkerOdourIntensityByDistance,
		MarkerOdourRange,

		PreviousTargetType,
		PreviousTargetName,
		PreviousTargetTag,


		EnvironmentTimeHour,
		EnvironmentTimeMinute,
		EnvironmentTimeSecond,
		EnvironmentDateYear,
		EnvironmentDateMonth,
		EnvironmentDateDay,
		EnvironmentTemperature,
		EnvironmentWeather,

		InputKey

	}
	
	public enum TargetSelectorStatementType
	{
		NONE = 0,
		PRIORITY,
		MULTIPLIER,
		SUCCESSOR
	}
	
	
	public enum TargetSelectorPositionType
	{
		TargetMovePosition = 0,
		TargetMaxRange,
		ActiveTargetMovePosition,
		ActiveTargetMaxRange,
		HomeTargetMovePosition,
		HomeTargetMaxRange,
		OutpostTargetMovePosition,
		OutpostTargetMaxRange,
		EscortTargetMovePosition,
		EscortTargetMaxRange,
		PatrolTargetMovePosition,
		PatrolTargetMaxRange
	}

	public enum TargetSelectorParentType
	{
		Empty,


	}


	public enum LinkType
	{
		MODE,
		RULE
	}

	public enum VelocityType
	{
		DEFAULT = 0,
		ADVANCED
	}


	public enum MoveType
	{
		DEFAULT = 0,
		CUSTOM,
		ESCAPE,
		AVOID,
		ORBIT,
		DETOUR,
		RANDOM
	}

	public enum MoveCompleteType
	{
		DEFAULT,
		NEXTRULE,
		CHANGE_MODE,
		FORCE_SENSE,
		FORCE_REACT
	}

	public enum AnimationInterfaceType
	{
		NONE=0,
		MECANIM=1,
		LEGACY=2,
		CLIP=3,
		CUSTOM=4
	}

	public enum AnimatorControlType
	{
		DIRECT,
		ADVANCED
	}

	public enum LabelType {
		Gray = 0,
		Blue,
		Teal,
		Green,
		Yellow,
		Orange,
		Red,
		Purple,
		None
	}
}


