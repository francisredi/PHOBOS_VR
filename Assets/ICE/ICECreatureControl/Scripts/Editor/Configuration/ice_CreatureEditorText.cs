// ##############################################################################
//
// ice_CreatureEditorText.cs
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
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using ICE;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Styles;
using ICE.Layouts;

namespace ICE.Creatures.EditorInfos
{

	public static class Info
	{
		public static string Version = "1.1.15";

		public static bool HelpEnabled = false;
		public static bool DescriptionEnabled = true;

		public static void Desc( string _text )
		{
			if( _text != "" )
				EditorGUILayout.HelpBox( _text , MessageType.None); 
		}

		public static void Help( string _text )
		{
			if( HelpEnabled && _text != "" )
				EditorGUILayout.HelpBox( _text , MessageType.None); 
		}

		public static void Note( string _text )
		{
			EditorGUILayout.HelpBox( _text , MessageType.None); 
		}

		public static void Warning( string _text )
		{
			EditorGUILayout.HelpBox( _text , MessageType.Warning); 
		}

		public static int HelpButtonIndex = 0;
		public static bool[] HelpFlag = new bool[1000];
		public static void HelpButton()
		{
			if( HelpEnabled == true )
				return;

			HelpButtonIndex++;

			if( HelpFlag[HelpButtonIndex] == true )
				GUI.backgroundColor = Color.yellow;
			else
				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

			if (GUILayout.Button( "?", ICEEditorStyle.InfoButton ))
				HelpFlag[HelpButtonIndex] = ! HelpFlag[HelpButtonIndex];

			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
		}

		public static void HelpButton( Rect _rect )
		{
			if( HelpEnabled == true )
				return;
			
			HelpButtonIndex++;
			
			if( HelpFlag[HelpButtonIndex] == true )
				GUI.backgroundColor = Color.yellow;
			else
				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			
			if( GUI.Button( _rect, new GUIContent( "?", ""), ICEEditorStyle.InfoButton ) )
				HelpFlag[HelpButtonIndex] = ! HelpFlag[HelpButtonIndex];
			
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
		}

		public static void ShowHelp( string _text )
		{
			if( ( HelpEnabled || HelpFlag[HelpButtonIndex] ) && _text != "" )
				EditorGUILayout.HelpBox( _text , MessageType.None); 
		}


		// ################################################################################
		// TARGET
		// ################################################################################
		public static readonly string TARGET = "";

		public static readonly string TARGET_OBJECT = "Targets represents potential destinations and interaction objects and contains as fundamental elements " +
			"all relevant information about motion and behaviour of your creature. Please note that the behaviour of your creature is target-driven, therefore " +
			"it is fundamental that your creature have at least one reachable target.A Target Object can be each static or movable GameObject in your scene or " +
			"a Prefab as well. The only requirement is here that the given position should be reachable for your creature and please consider also the typical " +
			"characteristics of scene objects and prefabs (e.g. nested prefabs etc.)";

		public static readonly string TARGET_SELECTION_CRITERIA = "Your creature could have several targets at the same time in such cases it can use a set of selection criteria to " +
			"evaluate the most suitable target related to the given situation. Here you can define the priority and relevance of a target.";

		public static readonly string TARGET_SELECTION_CRITERIA_HOME = "* Please consider, the HOME target should normally have the lowest priority, because it should be rather a " +
			"side show than the main event, a secluded place where your creature can spawn or become modified invisible to the player.";

		public static readonly string TARGET_SELECTION_CRITERIA_ADVANCED = "The advanced Target Selection Criteria provide you to define multiple selectors with customized " +
			"conditions";

		public static readonly string TARGET_SELECTION_CRITERIA_PRIORITY = "In cases that your creature will have several valid targets at the same time, the priority " +
			"determines the relevance of the targets and the creature will select the target with the highest priority. If there are two or more targets with the same " +
			"priority, the selection will be randomized.";
		public static readonly string TARGET_SELECTION_CRITERIA_RANGE = "The Selection Range defines the maximum distance in which the creature could detect the target. If " +
			"the Selection Range is adjusted to zero, the Selection Range will be ignored and the condition will be always true.";
		public static readonly string TARGET_SELECTION_CRITERIA_ANGLE = "While the Field Of View defines the view angle of the creature, the Selection Angle deals with a " +
			"notional view angle of the target, in which the creature must be inside to fulfil the condition. To adjust this angle to zero will have the same effect as " +
			"an adjustment of 360 degrees, in both cases the selection angle will be ignored and the condition will be true.";

		public static readonly string TARGET_MOVE_SPECIFICATIONS = "Your creature always try to reach the TargetMovePosition of the given target " +
			"object. By default the Target Move Position will be the transform position of a target, but in the majority of cases the transform " +
			"position will be suboptimal or simply non-practical as access point and therefore the Target Move Specifications provides several " +
			"settings to adapt the TargetMovePosition as desired and allows you to define a fixed point related to the target or a dynamic and " +
			"randomized position as well.";
		public static readonly string TARGET_MOVE_SPECIFICATIONS_OFFSET = "The Offset values specifies a local position related to the transform position of " +
			"the target. The offset settings are optional and allows you to adapt the target position if the original transform position of an object is not " +
			"reachable or in another way suboptimal or not usable. The TargetOffsetPosition contains the world coordinates of the local offset, which will " +
			"used as centre for the randomized positioning and finally as TargetMovePosition as well.";
		public static readonly string TARGET_MOVE_SPECIFICATIONS_OFFSET_ANGLE = "Additional to adapt the offset position by enter the coordinates, you can use distance and angle to define the " +
			"desired position. Angle defines the offset angle related to the transform position of the target. Zero (or 360) degrees defines a position in front of " +
			"the target, 180 degrees consequently a position behind it etc.";
		public static readonly string TARGET_MOVE_SPECIFICATIONS_OFFSET_DISTANCE = "Additional to adapt the offset position by enter the coordinates, you can use distance and angle to define the " +
			"desired position. Distance defines the offset distance related to the transform position of the target.";
		public static readonly string TARGET_MOVE_SPECIFICATIONS_OFFSET_UPDATE = "While using a random position you can define the update conditions. Update On Active will refresh the " +
			"position whenever the target becomes active.\n" +
			"While using a random position you can define the update conditions. Update On Reached will refresh the " +
			"position whenever the creature has reached the given TargetMovePosition.\n" +
			"While using a random position you can define the update conditions. Update On Timer will refresh the position " +
			"according to the defined interval.";
		public static readonly string TARGET_MOVE_SPECIFICATIONS_OFFSET_UPDATE_ACTIVATE = "While using a random position you can define the update conditions. Update On Active will refresh the " +
			"position whenever the target becomes active.";
		public static readonly string TARGET_MOVE_SPECIFICATIONS_OFFSET_UPDATE_REACHED = "While using a random position you can define the update conditions. Update On Reached will refresh the " +
			"position whenever the creature has reached the given TargetMovePosition.";
		public static readonly string TARGET_MOVE_SPECIFICATIONS_OFFSET_UPDATE_TIMER = "While using a random position you can define the update conditions. Update On Timer will refresh the position " +
			"according to the defined interval.";
		public static readonly string TARGET_MOVE_SPECIFICATIONS_RANDOM_RANGE = "Random Positioning Range specifies the radius of a circular area around the TargetOffsetPosition to provide a " +
			"randomized positioning. The combination of TargetOffsetPosition and Random Range produced the TargetMovePosition as the final target position, which will used for all target " +
			"related moves. While using a random position you can define the update conditions to reposition the TargetMovePosition during the runtime. Please note, that you can also combine two " +
			"or all conditions as well.";
		public static readonly string TARGET_MOVE_SPECIFICATIONS_STOP_DISTANCE = "The Target Stopping Distance defines the minimum distance related to the TargetMovePosition " +
			"to complete the current move. If your creature is within this distance, the TargetMovePosition was reached and the move is complete (that’s the precondition to " +
			"run a RENDEZVOUS behaviour).";
		public static readonly string TARGET_MOVE_SPECIFICATIONS_IGNORE_LEVEL_DIFFERENCE = "While Ignore Level Differences is flagged, the distance between your creature and the selected target will " +
			"measured without differences in height. By default, this option is ON because it covers the most cases and tolerates also roughly target position settings, but in some cases (e.g. levels or " +
			"buildings with walkable surfaces on several elevations etc.) you will need also the differences of y-axis. ";
		public static readonly string TARGET_MOVE_SPECIFICATIONS_SMOOTHING = "The Smoothing Multiplier affects step-size and update speed of the TargetMovePosition. " +
			"If Smoothing is adjusted to zero the TargetMovePosition will relocated directly during an update, if Smoothing is adjusted to one the " +
			"TargetMovePosition will changed extremely slow and soft.";

		public static readonly string TARGET_INFLUENCES = "The Target Inluences enables to adapt influences if the target is active (e.g. your prey creature have detected a predator target and therefore " +
			"its stress level goes up)";

		public static readonly string TARGET_GROUP_MESSAGE = "The Target Group Message is a new feature which allows your creature to communicate with other creatures in its " +
			"group.";
		public static readonly string TARGET_GROUP_MESSAGE_COMMAND = "";

		public static string GetTargetSelectionExpressionTypeHint( TargetSelectorExpressionType _type )
		{
			string _hint = "";

			switch( _type )
			{
				case TargetSelectorExpressionType.OwnAge:
					_hint = "Creature Age";
					break;
			case TargetSelectorExpressionType.OwnPosition:
				_hint = "Creature Position";
					break;
			case TargetSelectorExpressionType.OwnFitness:
				_hint = "Creature Fitness";
				break;

			case TargetSelectorExpressionType.OwnHealth:
				_hint = "Creature Health";
				break;

			case TargetSelectorExpressionType.OwnStamina:
				_hint = "Creature Stamina";
				break;

			case TargetSelectorExpressionType.OwnPower:
				_hint = "Creature Power";
				break;

			case TargetSelectorExpressionType.OwnDamage:
				_hint = "Creature Damage";
				break;

			case TargetSelectorExpressionType.OwnStress:
				_hint = "Creature Stress";
				break;

			case TargetSelectorExpressionType.OwnDebility:
				_hint = "Creature Debility";
				break;

			case TargetSelectorExpressionType.OwnHunger:
				_hint = "Creature Hunger";
				break;

			case TargetSelectorExpressionType.OwnThirst:
				_hint = "Creature Thirst";
				break;

			case TargetSelectorExpressionType.OwnAggressivity:
				_hint = "Creature Aggressivity";
				break;

			case TargetSelectorExpressionType.OwnBehaviour:
				_hint = "Creature Behaviour";
				break;

			default:
				_hint = _type.ToString();
				break;
			}

			return _hint;
		}


		// ################################################################################
		// COCKPIT
		// ################################################################################
		public static readonly string DEBUG = "The debug options provide you several tools to monitoring the movement and " +
			"behaviour of your creature, so it’s easier to you to detect and avoid misfeature and nonconformities, such as " +
				"potential deadlocks, collisions etc. ";

		public static readonly string CREATURE_PRESETS = "Save and load creature settings to use it for other creatures or situations.";
		public static readonly string DISPLAY_OPTIONS = "Here you can choose your individual display options, dependent to your tasks and " +
			"requirements. Hide unneeded feature and reduce the component to the relevant parts, so you will never lose the track.";

		public static readonly string REGISTER = "The Creature Register popup allows you to switching directly between all your " +
			"registered creatures. Use this feature in conjunction with the ‘glabal’ display options, to get a quick access to " +
			"your focused settings sections.";
		public static readonly string REGISTER_MISSING =  "Sorry, it looks like that there is no active Creature " +
			"Register in your Scene, do you want to add one?";
		public static readonly string REGISTER_DISABLED = "Sorry, it looks like that there is no active Creature " +
			"Register in your Scene, do you want to activate it?";


		// ################################################################################
		// WIZARD
		// ################################################################################
		public static readonly string WIZARD = "";
		

		// ################################################################################
		// ESSENTIALS
		// ################################################################################
		public static readonly string ESSENTIALS = "Essentials covers all fundamental settings your creature needs for its first steps, " +
			"such as a home location, basic behaviours and the general motion and pathfinding settings as well.";
		public static readonly string ESSENTIALS_HOME = "Here you have to define the home location of your creature. This place will be its starting " +
			"point and also the area where it will go to rest and to respawn after dying. Whenever your creature is not busy or for any reason not " +
				"ready for action (e.g. wounded, too weak etc.) it will return to this place.";

		public static readonly string ESSENTIALS_BEHAVIOURS = "The Home Behaviours represents the proposed minimum performance requirements your " +
			"creature should fulfil. Leisure is an idle behaviour if your creature is at home and have no other tasks. Travel contains the move " +
			"behaviour if it’s on the way.";
		public static readonly string ESSENTIALS_SYSTEM = "Motion and Pathfinding contains the basic settings for physics, motion and pathfinding, " +
			"which are relevant for the correct technical behaviour, such as grounded movements, surface detection etc. ";

		public static readonly string ESSENTIALS_SYSTEM_GROUND_ORIENTATION = "Here you can specify the vertical direction of your creature relative " +
			"to the ground, which is important for movements on slanted surfaces or hilly grounds. The Plus flag activates a more extensive method to " +
			"handle the Ground Orientation for Crawler and Quadruped creature types. ";
		//public static readonly string ESSENTIALS_SYSTEM_GROUND_ORIENTATION = "";
		public static readonly string ESSENTIALS_SYSTEM_GROUND_CHECK = "Here you can define the desired method to handle ground related checks and " +
			"movements.";
		public static readonly string ESSENTIALS_SYSTEM_GRAVITY = "Here you can activate/deactivate the internal gravity. If you are using the internal " +
			"gravity you don’t need additional components to handle it.";

		public static readonly string ESSENTIALS_SYSTEM_AVOIDANCE = "While Obstacle Avoidance is active your creature can detect and avoid obstacles " +
			"automatically without additional pathfinding tools. Set the Obstacle Avoidance Popup to BASIC to activate this feature and add all available " +
			"and/or desired obstacle layers. In addition you can adapt the Scanning Range and Angle options to improve the result and/or to optimize the " +
			"performance.";
		public static readonly string ESSENTIALS_SYSTEM_AVOIDANCE_RANGE = "Scanning Range defines the maximum radius your creature will scan. The default " +
			"values should be suitable for the majority of cases but finally the result will be dependent on the given scenario and situation e.g. if there " +
			"are a lot of obstacles closely spaced you should decrease the range, but if the range is too small it could be that your creature will detect " +
			"an obstacle to late and can’t avoid in time.";
		public static readonly string ESSENTIALS_SYSTEM_AVOIDANCE_ANGLE = "The Scanning Angle defines the steps in degrees within a full-circle and with " +
			"this the density of the scan, a lower value will improve the result but also increase the required load, a higher value could be too imprecise. ";
		public static readonly string ESSENTIALS_SYSTEM_AVOIDANCE_SAVE_POSITION = "Use Free Position considers obstacles while finding the next move position";

		// ################################################################################
		// DEADLOCK
		// ################################################################################
		public static readonly string DEADLOCK = "A deadlock is a situation in which your creature can’t reach its desired move position. This could " +
			"have several reasons, such as the typical case that its route is blocked by obstacles etc., but it could also be that its forward velocity " +
			"is too high or the angular speed too low, so that your creature have - for the given situation - not the required manoeuvrability to reach " +
			"the Stopping Distance to complete this move. In such cases you can observe two typical behaviours – if the path is simply blocked your " +
			"creature will still walking or running on the same spot, but if the manoeuvrability is not suitable to the given stopping distance, your " +
			"creature will moving in a circle or a loop. The Deadlock Handling allows your creature to detect such mistakes and you can define how your " +
			"creature should react.";
		public static readonly string DEADLOCK_MOVE_DISTANCE = "Move Distance defines the minimum distance which your create have to covered within the " +
			"specified time. Please note that distance and interval should be suitable to the lowest forward speed of your creature.";
		public static readonly string DEADLOCK_MOVE_INTERVAL = "Test Interval defines the time in which your creature have to cover the Move Distance.";
		public static readonly string DEADLOCK_MOVE_CRITICAL_POSITION = "Max. Critical Positions defines the upper tolerance limit to trigger a deadlock. " +
			"If this value is adjusted to zero, each critical event will directly trigger a deadlock, otherwise the limit must be reached.";
		public static readonly string DEADLOCK_LOOP_RANGE = "Loop Range works analogue to the Move Distance Test but in larger dimensions. The Loop " +
			"Range should be larger than the given Stopping Distance and the interval suitable to the lowest walking speed of your creature. ";
		public static readonly string DEADLOCK_LOOP_INTERVAL = "Test Interval defines the time in which your creature have to leave the Loop Range.";
		public static readonly string DEADLOCK_LOOP_CRITICAL_POSITION = "Max. Critical Positions defines the upper tolerance limit to trigger a deadlock. " +
			"If this value is adjusted to zero, each critical event will directly trigger a deadlock, otherwise the limit must be reached.";
		public static readonly string DEADLOCK_ACTION = "Deadlock Action defines the desired procedure in cases of deadlocks.";
		public static readonly string DEADLOCK_ACTION_BEHAVIOUR = "";
		public static readonly string DEADLOCK_ACTION_DIE = "";
		public static readonly string DEADLOCK_ACTION_UPDATE = "";

		// ################################################################################
		// FIELD OF VIEW
		// ################################################################################

		public static readonly string FOV = "The Field Of View represents the maximum horizontal angle your creature can sense the surrounding environment. " +
			"By default this value is adjusted to zero, which suspends the FOV restrictions and allows sensing in 360 degrees, alternative you could set the " +
			"value also directly to 360 degrees, this will have the same effect except that the FOV still active and you can see the FOV gizmos. Please note, " +
			"that the FOV settings will not automatically use to sense (select) a target. You have to activate the FOV flag of the Target Selection Criteria " +
			"to use this feature. That’s because to provide more flexibility.";
		public static readonly string FOV_VISUAL_RANGE = "Visual Range defines the maximum sighting distance of your creature. By adjusting this value to zero, " +
			"the sighting distance will be infinite.";

		// ################################################################################
		// LEAN ANGLE
		// ################################################################################
		public static readonly string ESSENTIALS_SYSTEM_BASE_OFFSET = "Base Offset defines the relative vertical displacement of the owning GameObject.";
		public static readonly string ESSENTIALS_SYSTEM_RAYCAST_VERTICAL_OFFSET = "The vertical Raycast Offset defines the height of the raycast origin " +
			"related to the given creature position. By default this value is 0.5 but dependent to the terrain it could be helpful to increase this value " +
			"to reach better results.";
		public static readonly string ESSENTIALS_SYSTEM_LEAN_ANGLE = "The Leaning Turn options allows your creature to lean into the turn. The lean " +
			"(roll) angle is related to the current speed but the angle can adjusted and limited by changing the multiplier and the maximum value.";
		public static readonly string ESSENTIALS_SYSTEM_LEAN_ANGLE_WARNING = "Please note: This feature works well with Legacy Animations but " +
			"shows no results by using the Mecanim Animation System. That’s because Mecanim handles the Root Transform Rotations according to the given " +
			"animation curves and ignores external changes. I’ll fixed this in the course of the further integration of the Mecanim Animation System.";
		public static readonly string ESSENTIALS_SLOPE_LIMITS = "While Use Slope Limits is flagged you can define the maximum slope limit your creature " +
			"can use and in addition to that you can also adapt the maximum walkable slope angle, so that your creature will try to find a walkable way.";
		public static readonly string ESSENTIALS_MAX_SLOPE_ANGLE = "Maximum slope angle your creature can use.";
		public static readonly string ESSENTIALS_MAX_WALKABLE_SLOPE_ANGLE = "Maximum walkable slope angle your creature will use to find a walkable way.";
		public static readonly string ESSENTIALS_AVOID_WATER = "While Avoid Water is flagged your creature will avoid surfaces with the water layer.";


		// ################################################################################
		// MOVE
		// ################################################################################

		public static readonly string MOVE = "The spatial movements of your creature are basically just position changes from one point to " +
			"another or rather from the current transform position of your creature to the given TargetMovePosition. The raw results are consequently " +
			"straight-line paths between these two points, which are usually insufficient for realistic movements and so the control provides several " +
			"options to optimize movements.";

		public static readonly string MOVE_DEFAULT = "The Default Move settings will be used for all standard situations and describes the " +
			"manoeuvre form the current transform position to the TargetMovePosition.";

		public static readonly string MOVE_SEGMENT_LENGTH = "The final destination point is basically the given TargetMovePosition and as long as there " +
			"are no obstacles or other influences, your creature will follow a straight-line path to reach this position. If Move Segment Length is not " +
			"adjusted to zero, the linear path will subdivided in segments of the defined length and the outcome of this is a sub-ordinate MovePosition " +
			"which can be used to modulate the path.";
		public static readonly string MOVE_SEGMENT_VARIANCE = "Use the Segment Variance Multiplier to randomize the Segment Length during the runtime. The " +
			"length will be updated when the stopping distance at the end of a segment was reached.";
		public static readonly string MOVE_LATERAL_VARIANCE = "Use the Lateral Variance Multiplier to force a randomized sideward drift. The random value " +
			"will be refreshed when the stopping distance at the end of a segment was reached.";
		public static readonly string MOVE_STOPPING_DISTANCE = "The Move Stopping Distance determined the minimum distance related to the actual MovePosition " +
			"to complete the current move. If your creature is within this distance, the MovePosition was reached and the move is complete.";
		public static readonly string MOVE_IGNORE_LEVEL_DIFFERENCE = "While Ignore Level Differences is flagged, the distance between your creature and the " +
			"given MovePosition will measured without differences in height. By default, this option is ON because it covers the most cases and tolerates also " +
			"roughly target position settings, but in some cases (e.g. levels or buildings with walkable surfaces on several elevations etc.) you will need " +
			"also the differences of y-axis. ";

		// ################################################################################
		// RUNTIME BEHAVIOUR
		// ################################################################################

		public static readonly string RUNTIME_COROUTINE = "ICECreatureControl is using coroutines to handle all sense and preparing procedures separated " +
			"from Unity’s frame update. You can deactivate the coroutines for debugging or adjusting your creature settings. ";
		public static readonly string RUNTIME_DONTDESTROYONLOAD = "Makes that your creature will not be destroyed automatically when loading " +
			"a new scene.";

		// ################################################################################
		// EXTERNAL COMPONENTS
		// ################################################################################
		public static readonly string MOTION_CONTROL = "ICECreatureControl works fine without additional Components and can handle a lot of situations " +
			"autonomously, that’s particularly important if you need a large crowd of performance friendly supporting actors, but it will definitely not covering " +
			"all potential situations and for such cases ICECreatureControl supports several Unity Components to enhance the functionality.";
		public static readonly string MOTION_CONTROL_NAVMESHAGENT = "The internal pathfinding technics are sufficient for open environments, such as natural " +
			"terrains or large-scaled platforms, but less helpful for closed facilities, areas covered by buildings and/or constructions or walkable surfaces with " +
			"numerous obstacles. For such environments you could activate the NavMeshAgent, so that your creature will using Unity’s Navigation Mesh. Activating " +
			"‘Use NavMeshAgent’ will automatically add the required NavMeshAgent Component to your creature and handle the complete steering. The only things you " +
			"have to do is to check and adjust the ‘Agent Size’, the desired ‘Obstacle Avoidance’ and ‘Path Finding’ settings. Needless to say, that the " +
			"NavMeshAgent Component requires a valid Navigation Mesh.";
		public static readonly string MOTION_CONTROL_NAVMESHAGENT_MISSING = "NavMeshAgent component is missing, please assign this component to your creature!";
		public static readonly string MOTION_CONTROL_RIGIDBODY = "Basically, each moveable object in your scene should have a Rigidbody and especially if it " +
			"has also a collider to detect collisions. Without Rigidbody Unity’s physics engine assumes that an object is static and static (immovable) objects " +
			"should consequently not have collisions with other static (immovable) objects and therefore Unity will not testing collision between such supposed " +
			"static objects, which means, that at least one of the colliding objects must have a Rigidbody additional to a collider, otherwise collisions will " +
			"not detected. But no rule without exception and there are absolutely some cases your creature really doesn’t need a Rigidbody or even a Collider as " +
			"well. \n\nApart from that is a Rigidbody more than a supporting element to detect collisions and you can use the physical attributes of the Rigidbody " +
			"to affect the behaviour of your creature but please note, that the steering by forces is not implemented in the current version (coming soon) and " +
			"using the physics with custom settings could yield funny results.\n\nFor a quick setup you can use the Preset Buttons. FULL (not implemented in the " +
			"current version) will prepare Rigidbody and ICECreatureControl to control your creature in a physically realistic way. SEMI deactivates the gravity, " +
			"enables the kinematic flag and allows position changes. OFF deactivates the gravity, checks the kinematic flag and restricts position changes. In " +
			"all cases rotations around all axes should be blocked.";
		public static readonly string MOTION_CONTROL_RIGIDBODY_MISSING = "Rigidbody component is missing, please assign this component to your creature!";
		public static readonly string MOTION_CONTROL_CHARACTER_CONTROLLER = "In addition to the other listed controller types you can also use Unity’s CharacterController " +
			"component to steering your creature.";
		public static readonly string MOTION_CONTROL_CHARACTER_CONTROLLER_MISSING = "CharacterController component is missing, please assign this component to your creature!";
		public static readonly string MOTION_CONTROL_CUSTOM = "While using CUSTOM motion control, ICECreatureControl will calculate the move positions but the final " +
			"movement of your creature will be handled by an external component, such as A*Pathfinding. Please add the desired pathfinding or character controller " +
			"component and the assiociated adapter to your creature. If there is no suitable adapter available please contact the developer for further information.";


		// ################################################################################
		// STATUS
		// ################################################################################
		public static readonly string STATUS = "Status represents the physical fitness of the creature, which is a dynamic attribute consists of several " +
			"different settings and measurements to affect the behaviour during the runtime. You can use this component section to adapt species-typical " +
			"characteristics, such as the sense and reaction time, maximum age etc. Dependent to the desired complexity and/or the given requirements, " +
			"ICECreatureControl provides you a Basic and an Advanced Status. Please note, that all these settings are optional.";

		public static readonly string STATUS_BASICS = "The Basics Status contains the essential elements your creature will need for a basic life-cycle, " +
			"which allows your creature to sense and react, to receive damage and to recreate, to die and also to respawn. You can activate the Advanced " +
			"Status Settings to use the extensive Status System.  ";
		public static readonly string STATUS_ADVANCED = "Dependent to the desired complexity and the given requirements, ICECreatureControl provides you " +
			"additional to the Basic Settings an enhanced Status System. While using the Basic Status, the fitness of your creature will be always identical " +
			"with the health value and finally just the antipode of damage – increasing the damage will directly reduce the health and consequently the " +
			"Fitness as well. By using the Advanced Status the procedure to evaluate the Fitness is affected by several different initial values, indicators, " +
			"multipliers and random variances.";

		public static readonly string STATUS_SENSES = "Sensory Indicators represents the perceptive capabilities of your creatures. By default all values " +
			"are adjusted to 100 % but you can adapt the values manual to restrict specific senses. Also you can adjust the multiplier to restrict senses " +
			"dynamically during the runtime.";
		public static readonly string STATUS_SENSES_AUDITORY = "";
		public static readonly string STATUS_SENSES_VISUAL = "";
		public static readonly string STATUS_SENSES_OLFACTORY = "";
		public static readonly string STATUS_SENSES_GUSTATORY = "";
		public static readonly string STATUS_SENSES_TACTILE = "";

		public static readonly string STATUS_FEEDTYPE = "" +
			"Herbivores are animals which only eat plant material. This means leaves, flowers, fruits or even wood. Sheep, horses, rabbits and " +
			"snails are well known examples of herbivores which eat grass and leaves. A parrot, however, which eats fruits and nuts can also be " +
			"called a herbivore" +
			"Omnivores eat both plants and meat. Chickens are omnivores. They eat seeds, but they can also eat worms. human beings are also omnivores, " +
			"although some people choose not to eat meat. These people are called vegetarians." +
			"Carnivores eat meat. A carnivore is a predator because it has to find and catch its prey. Some carnivores, such as wolves, hunt in a " +
			"group called a pack. They move silently and slowly to form a circle around their prey before they attack.";
		public static readonly string STATUS_FEEDTYPE_CANNIBAL = "If 'Cannibal' is flagged Omnivores and Carnivores will be hunt, attack and eat its own species";


		public static readonly string STATUS_PERCEPTION_TIME = "Perception Time defines the necessary time to sense a situation, which in particular means, " +
			"the interval in which your creature will sense the surrounding environment to detect potential interactor objects.";
		public static readonly string STATUS_PERCEPTION_TIME_VARIANCE = "The Variance Multiplier defines the threshold variance value, which will be used to " +
			"randomize the associated interval during the runtime.";
		public static readonly string STATUS_PERCEPTION_TIME_MULTIPLIER = "The Fitness Multiplier defines the influence ratio of the fitness value on the " +
			"associated interval.";
		public static readonly string STATUS_REACTION_TIME = "Reaction Time defines the necessary time to identify a situation, which in particular means, " +
			"the interval in which your creature will decide the kind of reaction and start the action through selection and activation of the most relevant " +
			"target and the related behaviour.";
		public static readonly string STATUS_REACTION_TIME_VARIANCE = "The Variance Multiplier defines the threshold variance value, which will be used to " +
			"randomize the associated interval during the runtime.";
		public static readonly string STATUS_REACTION_TIME_MULTIPLIER = "The Fitness Multiplier defines the influence ratio of the fitness value on the " +
			"associated interval.";
		
		public static readonly string STATUS_RECOVERY_PHASE = "Recovery Phase defines a warm-up period in seconds after the spawning process in which the creature " +
			"will be completely defenceless while running its respawn behaviour without any target. You can adjust this value to provide your player to " +
			"detect a new spawned creature at the right time, otherwise it could be that a spawned creature appear from nowhere and will start its attack " +
			"without any heads-up.";
			
		public static readonly string STATUS_RESPAWN_DELAY = "Respawn Delay defines the delay time in seconds until a deceased creature will added to the " +
			"respawn queue. Within this time-span the creature will be visible in the scene and can be used for loot activities.";
		public static readonly string STATUS_RESPAWN_DELAY_VARIANCE = "The Variance Multiplier defines the threshold variance value, which will be used to " +
			"randomize the associated delay time during the runtime.";
		public static readonly string STATUS_RESPAWN_CORPSE = "If Use Corpse is flagged you can assign a GameObject which will be used if your creature dies " +
			"(e.g. a Ragdoll Object of your creature which will be used instead of the original model). The corpse object have to be a prefab which will be " +
			"instantiate automatically if your creature dies.";

		public static readonly string STATUS_INFLUENCE_INDICATORS = "Influence Indicators represents all status attributes which can be affected by direct " +
			"influences, based on internal activities or external forces as well. You can modify this Indicators to customize initial values or to test the " +
			"status settings. By default all this indicators are adjusted to zero.";
		public static readonly string STATUS_DAMAGE_IN_PERCENT = "The Damage attribute represents the effective damage level of your creature in percent. " +
			"Depending on the associated multiplier the value will affect default indicators. ";
		public static readonly string STATUS_STRESS_IN_PERCENT = "The Stress attribute represents the effective stress level of your creature in percent. " +
			"Depending on the associated multiplier the value will affect default indicators. ";
		public static readonly string STATUS_DEBILITY_IN_PERCENT = "The Debility attribute represents the effective debility level of your creature in percent. " +
			"Depending on the associated multiplier the value will affect default indicators. ";
		public static readonly string STATUS_HUNGER_IN_PERCENT = "The Hunger attribute represents the effective hunger level of your creature in percent. " +
			"Depending on the associated multiplier the value will affect default indicators. ";
		public static readonly string STATUS_THIRST_IN_PERCENT = "The Thirst attribute represents the effective hunger level of your creature in percent. " +
			"Depending on the associated multiplier the value will affect default indicators. ";

		public static readonly string STATUS_INFLUENCES = "Influences defines the impact of a triggering event to your creature. These impacts could " +
			"be positive for your creature, such as a recreation processes by reducing the damage while your creature is sleeping or eating, or " +
				"negative through increasing the damage or stress values while your creature is fighting. Impacts will be directly affect the status " +
				"values of your creature and consequently also the selection criteria for a target or interactor. While a triggering event is active and the " +
				"interval is adjusted to zero, the influences will refresh the status values during the framerate-independent update cycle of FixedUpdate " +
				"(default 0.02 secs.), so please make sure that your defined impact values suitable to the short time-span or adjust the interval value, " +
				"otherwise your creature will not acting as expected (e.g. die in seconds or will not select the expected target etc.)";
		public static readonly string STATUS_INFLUENCES_INTERVAL = "Interval defines the time delay in seconds between two influence calls. By default this value " +
			"is adjusted to zero, which means that an influence call will affect your creature in each framerate-independent update cycle of FixedUpdate (default 0.02 secs. .), " +
			"so please make sure, that your defined impact values are suitable to this short time-span or increase the interval value, otherwise your " +
			"creature could die immediately.";
		public static readonly string STATUS_INFLUENCES_STRESS = "Stress specifies the impact to the stress status attribute and depending on the associated " +
			"multiplier to the default indicators as well.";
		public static readonly string STATUS_INFLUENCES_DEBILITY = "Debility specifies the impact to the debility status attribute and depending on the associated " +
			"multiplier to the default indicators as well.";
		public static readonly string STATUS_INFLUENCES_DAMAGE = "Damage specifies the impact to the damage status attribute and depending on the associated " +
			"multiplier to the default indicators as well.";
		public static readonly string STATUS_INFLUENCES_HUNGER = "Hunger specifies the impact to the hunger status attribute and depending on the associated " +
			"multiplier to the default indicators as well.";
		public static readonly string STATUS_INFLUENCES_THIRST = "Thirst specifies the impact to the thirst status attribute and depending on the associated " +
			"multiplier to the default indicators as well.";

		public static readonly string STATUS_INFLUENCES_AGGRESSIVITY = "Aggressivity specifies the impact to the aggressivity status attribute and depending on the associated " +
			"multiplier to the default indicators as well.";
		public static readonly string STATUS_INFLUENCES_ANXIETY = "Anxiety specifies the impact to the anxiety status attribute and depending on the associated " +
			"multiplier to the default indicators as well.";
		public static readonly string STATUS_INFLUENCES_EXPERIENCE = "Experience specifies the impact to the experience status attribute and depending on the associated " +
			"multiplier to the default indicators as well.";
		public static readonly string STATUS_INFLUENCES_NOSINESS = "Nosiness specifies the impact to the nosiness status attribute and depending on the associated " +
			"multiplier to the default indicators as well.";

		public static readonly string STATUS_VITAL_INDICATORS = "Vital Indicators represents calculated status attributes which will be indirect affected " +
			"by the influence indicators and the associated multipliers. By default the reference values are adjusted to 100, but you can modify this values " +
			"as desired to adapt the indicators to your existing environment. The calculated result will be expressed in percent.";

		public static readonly string STATUS_VITAL_INDICATOR_HEALTH = "";
		public static readonly string STATUS_VITAL_INDICATOR_STAMINA = "";
		public static readonly string STATUS_VITAL_INDICATOR_POWER = "";

		public static readonly string STATUS_CHARACTER_INDICATORS = "";
		public static readonly string STATUS_CHARACTER_DEFAULT_AGGRESSITY = "";
		public static readonly string STATUS_CHARACTER_DEFAULT_ANXIETY = "";
		public static readonly string STATUS_CHARACTER_DEFAULT_EXPERIENCE = ""; 
		public static readonly string STATUS_CHARACTER_DEFAULT_NOSINESS = "";


		public static readonly string STATUS_FITNESS_RECREATION_LIMIT = "The Recreation Limit defines the fitness threshold value at which your creature " +
			"will return automatically to its home location to recreate its fitness. If this value is adjusted to zero, the Recreation Limit will ignored, " +
			"otherwise the home target will be handled with the highest priority in cases the value goes below to the limit.";

		public static readonly string STATUS_AGING = "The ‘Use Aging’ flag activates the aging process, which will limited the life-cycle of your creature and " +
			"will have additional influence to the Fitness as well. Please note, that a limited life-cycle consequently means that your creature will die at the " +
			"end of the cycle. ";
		public static readonly string STATUS_AGING_AGE = "Current Age represents the age of your creature at runtime. You can adjust this value to define an " +
			"initial age or you can modify the value also during the runtime. Please note, that the effective time data are in seconds, the use of minutes is " +
			"for the editor mask only.";
		public static readonly string STATUS_AGING_MAXAGE = "Maximum Age defines the maximum length of the life-cycle and consequently the time of death as well. " +
			"Please note, that the effective time data are in seconds, the use of minutes is for the editor mask only.";

		public static readonly string STATUS_SHELTER = "While using ‘Use Shelter’ your creature will be protected against environment influences (e.g. rain, storm, " +
			"low temperatures etc.) or attacks while enter a safe area which you can define by a tagged trigger. If your creature enter such a trigger the IsSheltered " +
			"flag will be true and will reset to false again in cases it leaves such areas.";
		public static readonly string STATUS_SHELTER_TAG = "";

		public static readonly string STATUS_INDOOR = "While using ‘Use Indoor’ your creature will be protected against environment influences (e.g. rain, storm, " +
			"low temperatures etc.) or attacks while enter a safe area which you can define by a tagged trigger. If your creature enter such a trigger the IsIndoor " +
			"flag will be true and will reset to false again in cases it leaves such areas.";
		public static readonly string STATUS_INDOOR_TAG = "";
			 
		public static readonly string STATUS_ODOUR = "Odour represents volatilized chemical compounds that other creature could perceive by their sense " +
			"of olfaction.";
		public static readonly string STATUS_ODOUR_INTENSITY = "";
		public static readonly string STATUS_ODOUR_RANGE = "";
		public static readonly string STATUS_ODOUR_MARKER = "";
		public static readonly string STATUS_ODOUR_MARKER_INTERVAL = "";

		public static readonly string STATUS_TEMPERATURE = "The ‘Use Temperature’ flag activates the thermal sensation of your creature, which will have additional " +
			"influence to the fitness and finally to the behaviour as well. While ‘Use Temperature’ is active, your creature will receive and evaluate temperature " +
			"values based on the environment data of the creature register, which you can easiest combine with your own scripts, third party products or external " +
			"data sources. You can find the ICECreatureUniStormAdapter attached to your ICECreatureControl package (please note, that this adapter requires a " +
			"valid licence of UniStorm)";
		public static readonly string STATUS_TEMPERATURE_SCALE = "Temperature Scale defines the desired measuring unit FAHRENHEIT or CELSIUS in degrees";
		public static readonly string STATUS_TEMPERATURE_SCOPE = "The Temperature Scope defines the lowest and highest environment temperature values your creature can survive.";
		public static readonly string STATUS_TEMPERATURE_BEST = "Comfort Temperature defines the ideal temperature value for your creature.";
		public static readonly string STATUS_TEMPERATURE_CURRENT = "Temperature represents the current environment temperature, which your creature receives via " +
			"the environment data of the creature register. This editor field is for testing only, the value will overwrite during the runtime.";

		public static readonly string STATUS_ARMOR = "The ‘Use Armour’ flag activates the Armour of your creature. Armour works as a buffer by absorbing incoming " +
			"damage values. As long as the armour value is larger zero the damage value will remain unaffected.";
		public static readonly string STATUS_ARMOR_IN_PERCENT = "Armour defines the initial armour value in percent and represents the armour during the runtime as " +
			"well. You can adapt this value to customize the initial armour.";

		public static readonly string STATUS_INFLUENCE = "";

		public static readonly string STATUS_DYNAMIC_INFLUENCES = "";

		public static readonly string STATUS_MEMORY = "The Memory represents different kind of memorizations your creature can use to remember e.g. specific situations, " +
			"other creatures or locations but also your player. This feature is currently under construction and will be full available in one of the next versions.";
		public static readonly string STATUS_MEMORY_SPATIAL = "";
		public static readonly string STATUS_MEMORY_SHORT = "";
		public static readonly string STATUS_MEMORY_LONG = "";

		public static readonly string STATUS_INVENTORY = "The Inventory represents a list of items your creature (but also other ICE objects) can have with it. You can " +
			"define an empty list, your creature can fill during the runtime or you can also define default items, your creature can lose while looting by another creature " +
			"or your player but also while distribute items (e.g. sow seed or deliver a newspaper or pizza etc.)";

		public static readonly string STATUS_INVENTORY_SLOTS = "Slots represents repositories your creatures but also other ICE objects as well can use to store " +
			"items. To adapt the desired slots just set the maximum number and increase or decrease the slider or press the RES Button to remove all slots.";
		public static readonly string STATUS_INVENTORY_SLOT = "A slot represents a repository for an item type. By default a slot and especially the deposited items are " +
			"virtual constructs without existing GameObjects, in this case the given amount is just a theoretical value which represents imaginary items. But you can also " +
			"specify a parent object by using the Slot Popup which represents the creatures’ hierarchy, so that your creature can use the defined parent as a real handle " +
			"(e.g. hand, holster, chest holder etc.) and as long as the item amount is larger than zero the defined item will be represented as an instantiated object, " +
			"assigned to the given handle and visible and detectable for other creatures. This allows you to equip or re-equip your creature during the runtime by using " +
			"the inventory settings of the behaviour rules. ";
		public static readonly string STATUS_INVENTORY_SLOT_ITEM = "Item represents the type of a stored object. Basically an item can be each GameObject or Prefab but in " +
			"each case it must be listed as reference in the Creature Register and requires the ICECreatureItem script, if both is given the item will be listed in the Item " +
			"Popup. Furthermore you can activate the EXCL Button, to mark the item as exclusive and reserved for the given slot. By default EXCL is deactivated and the slot " +
			"is open for each kind of items. If you assign now an item to such an open slot the amount will automatically increase to 1 and the slot will be open again if " +
			"the amount will reset to zero. By activate the exclusive flag the slot stays in each case reserved for the assigned item type independent of the amount. If the " +
			"DOD Button is flagged all available items will be dropped in cases the inventory owner dies or its object will be destroyed.";
		public static readonly string STATUS_INVENTORY_SLOT_AMOUNT = "Amount represents the current number of items. You can adjust the maximum number to restrict the " +
			"capacity. ";

		public static readonly string CHARACTERISTICS_SPEED_RUNNING = "Default running speed of the creature!";
		public static readonly string CHARACTERISTICS_SPEED_WALKING = "Default walking speed of the creature!";
		public static readonly string CHARACTERISTICS_SPEED_TURNING = "Default turning speed of the creature!";


		// ################################################################################
		// MISSIONS
		// ################################################################################
		public static readonly string MISSIONS = "Missions represents the standard duties your creature have to fulfil.";

		public static readonly string MISSION_ENABLED = "The Enabled flag allows you to activate or deactivate the complete Mission, without losing the " +
			"data. As long as a Mission is disabled, the creature will ignore them during the runtime. You could use this feature also by your own scripts to " +
				"manipulate the gameplay.";

		// ################################################################################
		// MISSIONS OUTPOST
		// ################################################################################
		public static readonly string MISSION_OUTPOST = "The Outpost Mission is absolutely boring for any high motivated creature and as expected the " +
			"job-description is really short: go home and wait for action! But you could enlarge the Random Positioning Range to give your creature a " +
			"larger scope, add additional rules for LEISURE and RENDEZVOUS and your creature will spend its idle time with some leisure activities. " +
			"Furthermore you could use the Pool Management of the Creature Register to generate some clones, so that your creature isn’t alone. On this " +
			"way you could use the Outpost Mission to populate a village, to setup a camp with soldiers, some animals for a farm or a pack of wolves " +
			"somewhere in a forest etc.";

		public static readonly string MISSION_OUTPOST_TARGET = "The Outpost object could be any reachable object in the scene and btw. movable objects " +
			"as well. Adapt the distances so that your creature feel comfortable, have sufficient space for his idle activities and don't blunder into " +
			"a conflict with the object size.";

		// ################################################################################
		// MISSIONS ESCORT
		// ################################################################################
		public static readonly string MISSION_ESCORT = "The Escort Mission means entertainment for your creature. It have to search and follow the leader " +
			"wherever he is and goes! You could use this mission to specify a faithful and brave companion to your player or to any another NPC as well. " +
			"You could also combine this mission with other Targets, such as the Patrol Mission, to use your creature as a guide which can show your player " +
			"secret places.";
		public static readonly string MISSION_ESCORT_TARGET = "The Leader object could be any reachable object in the scene. Adapt the distances so that your " +
			"creature have enough space for his activities and don't blunder into a conflict with the leader moves. ";
		public static readonly string MISSION_ESCORT_BEHAVIOUR = "";

		public static readonly string MISSION_SCOUT = "";

		// ################################################################################
		// MISSIONS PATROL
		// ################################################################################
		public static readonly string MISSION_PATROL = "The Patrol Mission represents a typical Waypoint Scenario and is - up to now - the most varied standard " +
			"task for your creature, so the job-description is a little bit more comprehensive: Find out and TRAVEL to the nearest waypoint. If you are reach " +
			"the Max. Range (Random Positioning Range + Stopping Distance) and it’s a transit-point, ignore LEISURE and RENDEZVOUS, find out the next waypoint " +
			"accordind to the given path-type and start to PATROL. If it’s not a transit-point, follow the LEISURE rules until reaching the RENDEZVOUS position " +
			"(TargetMovePosition + Stopping Distance) and execute the RENDEZVOUS instructions over the given period of time (Duration Of Stay). Afterwards find " +
				"out the next waypoint accordind to the given path-type and start to PATROL. Repeat these instructions for each waypoint.";
		public static readonly string MISSION_PATROL_ORDER_TYPE = "The Order Type defines the desired sequence in which your creature have to visit the single " +
			"waypoints. Please consider, that your creature will always starts with the nearest waypoint, so if you want that it will start with a special one " +
			"you should place it in the near. By default the cycle sequence is ordered in ascending order, activate the DESC button to change it to descending.";
		public static readonly string MISSION_PATROL_TARGET = "";
		public static readonly string MISSION_PATROL_WAYPOINTS = "To prepare a Patrol Mission you can add single waypoints, which could be any reachable objects " +
			"in your scene, or you can add a complete waypoint group, which is a parent object with its children. By using this way, the children will be used as " +
			"waypoints, while the parent will be ignored.";
		public static readonly string MISSION_PATROL_ADD_WAYPOINT_GROUP = "";
		public static readonly string MISSION_PATROL_ADD_WAYPOINT = "";
		public static readonly string MISSION_PATROL_WAYPOINT = "A Patrol Mission can basically have any number of waypoints. Each waypoint represents a separate " +
			"target and will also be listed with all target features in the inspector. You can move each waypoint item within the list up or down to change the " +
			"order or you can delete completely as well.  Furthermore, you can activate and deactivate each single waypoint as desired, in such a case, your " +
			"creature will skip deactivated waypoints to visit the next ones. ";
		public static readonly string MISSION_PATROL_CUSTOM_BEHAVIOUR = "The ‘Use Custom Behaviour’ flag allows you to overwrite the default patrol behaviour " +
			"rules for the selected waypoint. Activate the ‘Custom Behaviour’ flag to define your additional behaviour rules. Please note, that these rules will " +
			"be used for the selected waypoint only.";

		// ################################################################################
		// INTERACTION
		// ################################################################################
		public static readonly string INTERACTION = "Additional to the standard situations defined in the home and mission settings, you can teach your creature " +
			"to interact with several other objects in your scene, such as the Player Character, other NPCs, static construction elements etc. The Interaction " +
			"Settings provides you to design complex interaction scenarios with each object in your scene.\n\n" +
			"To using the interaction system you have to add one or more Interactors. An Interactor represents another GameObject as potential Target for your " +
			"creature and contains a set of conditions and instructions to define the desired behaviour during a meeting. By default interactors are neutral, " +
			"they could be best friends or deadly enemies as well and basically interactors can be everything your creature has to interact with it, such as a " +
			"football your creature has to play with it or a door, which it has to destroy.\n\n" +
			"After adding a new interactor you will see primarily the familiar target settings as they will be used in the home and mission settings, but instead " +
			"of the object field the interactor settings provides a popup to select the target game object. That’s because, interactors are normally OOIs (objects " +
			"of interest), which could be also interesting for other objects of interest, such as the Player Character or other NPCs and therefore such objects " +
			"have to be registered in the creature register to provide a quick access to relevant data during the runtime. So you have to use the popup to add " +
			"the desired interactor.\n\n" +
			"But the pivotal difference to the home and missions settings is, that you can define an arbitrary number of additional interaction rules, which " +
			"allows you to overwrite the initial target related selection and position settings for each rule. By using this feature you could define a nearly " +
			"endless number of conditions and behaviours for each imaginable situation, but in the majority of cases 3-5 additional rules will be absolutely " +
			"sufficient to fulfil the desired requirements.";

		public static readonly string INTERACTION_INTERACTOR = "An Interactor represents another GameObject as potential Target for your creature and contains a " +
			"set of conditions and instructions to define the desired behaviour during a meeting. By default interactors are neutral, they could be best friends " +
			"or deadly enemies as well and basically interactors can be everything your creature has to interact with it, such as a football your creature has to " +
			"play with it or a door, which it has to destroy.";

		public static readonly string INTERACTION_INTERACTOR_TARGET = "Interactors are mostly objects of interest, which will be normally interesting for other " +
			"objects of interest as well, such as the Player Character or other NPCs and therefore such objects have to be registered in the creature register to " +
			"provide a quick access to relevant data during the runtime. For this reason, you have to use the popup to select your desired interactor. If your " +
			"desired interactor isn’t listed currently, switch to your creature register to add the interactor. Please note, that your interactor object doesn’t " +
			"need additional scripts to be listed in the register, unless your interactor is a player character or a NPCs, which is not controlled by " +
			"ICECreatureControl, in such a case you should add the ICECreatureResident Script to your interactor, which will handle the registration and " +
			"deregistration procedures during the runtime.";
		public static readonly string INTERACTION_INTERACTOR_ENABLED = "The Interactor Enabled flag allows you to activate or deactivate the Interactor, without " +
			"losing the data. As long as an Interactor is disabled, the creature will ignore it during the runtime. You could use this feature also by your own " +
			"scripts to manipulate the gameplay.";
		public static readonly string INTERACTION_INTERACTOR_RULE = "";
		public static readonly string INTERACTION_INTERACTOR_RULE_TARGET = "";
		public static readonly string INTERACTION_INTERACTOR_ADD_RULE = "";

		public static readonly string INTERACTION_INTERACTOR_NO_RULES = "There are currently no additional interaction rules for these creature group " +
			"available. Add further rules to enhanced the interaction scenario!";
		public static readonly string INTERACTION_INTERACTOR_RULE_BLOCK = "If 'Block Next Rule' is flaged the rule will still active until your creature have " +
			"reached the given move-position. This feature allows you to define a move position outside of the respective Selection Range without influences of " +
			"further Selection Ranges and behaviour changes. Please make sure, that all potential positions " +
			"reachable for your creature, otherwise you will provoke a deadlock!";

		// ################################################################################
		// ENVIROMENT
		// ################################################################################
		public static readonly string ENVIROMENT = "Complementary to the HOME, MISSIONS and INTERACTION features, which are all dealing with the interaction " +
			"between your creature and other GameObjects, the Environment section handles the interaction with the surrounding environment. The current " +
			"Environment System provides your creature two different abilities to sense its surrounding space – SURFACE and COLLISION detection.";
		
		// ################################################################################
		// ENVIROMENT SURFACE
		// ################################################################################
		public static readonly string ENVIROMENT_SURFACE = "The Surface Rules specify the reaction to the specified textures. You could use this feature for " +
			"example to handle footstep sounds and/or footprint effects, but you could also start explosion effects to simulate a " +
				"minefield, or dust effects for a dessert, or you could define textures as fertile soil, where your creature can appease " +
				"one's hunger and thirst etc. ";
		public static readonly string ENVIROMENT_SURFACE_SCAN_INTERVAL = "The Scan Interval value defines the desired time period in seconds to check the current " +
			"ground texture. It’s not required and recommended to do this scan in each frame (value adjusted to zero), by default the creature will do this scan each " +
			"second, which should be suitable for the most cases. You should increase this value if you are using large herds or crowds.";
		public static readonly string ENVIROMENT_SURFACE_RULE = "";
		public static readonly string ENVIROMENT_SURFACE_RULE_NAME = "Name defines just the display name of the rule and have further impact. " +
			"You can rename it to use a more comprehensible and context related term.";
		public static readonly string ENVIROMENT_SURFACE_RULE_INTERVAL = "The Interval value defines the desired repeating time period in seconds.";
		public static readonly string ENVIROMENT_SURFACE_RULE_TEXTURES = "The Trigger Textures specifies the conditions to activate the assigned procedures. As " +
			"soon as your creature comes in contact with one of the defined textures, the specified procedures will start. Use the Interval settings " +
			"to adjust the desired repeating interval.";
		public static readonly string ENVIROMENT_SURFACE_RULE_PROCEDURES = "Each Surface Rule can initiate several procedures, in cases the given trigger conditions are " +
			"fulfilled. You can adapt the Procedure setting to define the desired behaviour. You could use the procedure settings for example to define footstep " +
			"sounds and/or footprint effects, but you could also start explosion effects to simulate a minefield, or dust effects for a dessert, or " +
			"you could define textures as fertile soil, where your creature can appease its hunger or thirst etc. ";
		public static readonly string ENVIROMENT_SURFACE_BEHAVIOUR = "";
		public static readonly string ENVIROMENT_SURFACE_AUDIO = "";
		public static readonly string ENVIROMENT_SURFACE_EFFECT = "";
		public static readonly string ENVIROMENT_SURFACE_INFLUENCES = "";
		
		// ################################################################################
		// ENVIROMENT IMPACT
		// ################################################################################
		public static readonly string ENVIROMENT_COLLISION = "The Collision Rules defines the reaction to detected collisions. You could use this " +
			"feature for example to adjust the damage if your creature was hit by a bullet, or comes in contact with a melee weapon or a spike wall.";
		public static readonly string ENVIROMENT_COLLISION_RULE_CONDITIONS = "";
		public static readonly string ENVIROMENT_COLLISION_RULE = "The Collision Rules defines the reaction to detected collisions. You could use this " +
			"feature for example to adjust the damage if your creature was hit by a bullet, or comes in contact with a melee weapon or a spike wall.";
		public static readonly string ENVIROMENT_COLLISION_RULE_NAME = "Name defines just the display name of the rule and have further impact. " +
			"You can rename it to use a more comprehensible and context related term.";
		public static readonly string ENVIROMENT_COLLISION_RULE_TYPE = "Type specifies the condition type, which will be using to filter the incoming " +
			"collisions. Currently you can filter incoming collision objects by TAG, LAYER or TAG&LAYER ";
		public static readonly string ENVIROMENT_COLLISION_RULE_TAG = "";
		public static readonly string ENVIROMENT_COLLISION_RULE_BODYPART = "";
		public static readonly string ENVIROMENT_COLLISION_RULE_TAG_PRIORITY = "";
		public static readonly string ENVIROMENT_COLLISION_RULE_LAYER = "";
		public static readonly string ENVIROMENT_COLLISION_RULE_LAYER_PRIORITY = "";
		public static readonly string ENVIROMENT_COLLISION_RULE_PROCEDURES = "";
		public static readonly string ENVIROMENT_COLLISION_BEHAVIOUR = "";
		public static readonly string ENVIROMENT_COLLISION_INFLUENCES = "";

		// ################################################################################
		// BEHAVIOUR
		// ################################################################################
		public static readonly string BEHAVIOUR = "While a Target represents a goal, Behaviours defines the way to reach it. The Behaviour settings " +
			"provides you to design and manage complex behaviour instructions and procedures, to reach your needs and goals and finally a realistic " +
			"and natural behaviour of your creature.";

		// ################################################################################
		// BEHAVIOUR MODE
		// ################################################################################
		public static readonly string BEHAVIOUR_MODE = "The behaviour of your creature is subdivided into single Behaviour Modes. Each of these " +
			"modes contains the instructions for specific situations and can be assigned to target-related or condition-based events. Furthermore " +
			"Behaviour Modes are not bounded to specific assignments and can generally be used for several targets and situations, in case they " +
			"are suitable for them.\n\nEach Behaviour Mode will have at least one Behaviour Rule, but to reach a more realistic behaviour can add " +
			"additional rules, which allows your creature to do things in different ways, break and resume running activities, run intermediate " +
			"animation sequences, start effects or to play audio files as well.";
		public static readonly string BEHAVIOUR_MODE_RENAME = "Renames allows you to change the key of the selected Behaviour Mode. Please note, " +
			"that renaming will remove all existing assignments.";
		public static readonly string BEHAVIOUR_MODE_FAVOURED = "The ‘Favoured’ flag allows you to block other targets and behaviours until the " +
			"defined conditions of the active mode are fulfilled. By using this feature you can force a specific behaviour independent of " +
			"higher-prioritised targets, which will normally determines the active behaviour. You can select several conditions, in this case the " +
			"selected ones will combined with OR, so that just one of them must be true. Please consider, that the active mode will in fact stay " +
			"active until the conditions are fulfilled, so please make sure, that your creature can fulfil the conditions, otherwise you will " +
			"provide a deadlock.";
		public static readonly string BEHAVIOUR_MODE_FAVOURED_PRIORITY = "";

		public static readonly string BEHAVIOUR_MODE_FAVOURED_PERIOD = "";
		public static readonly string BEHAVIOUR_MODE_FAVOURED_MOVE_POSITION_REACHED = "";
		public static readonly string BEHAVIOUR_MODE_FAVOURED_TARGET_MOVE_POSITION_REACHED = "";
		public static readonly string BEHAVIOUR_MODE_FAVOURED_TARGET = "";
		public static readonly string BEHAVIOUR_MODE_FAVOURED_TARGET_POPUP = "";
		public static readonly string BEHAVIOUR_MODE_FAVOURED_TARGET_RANGE = "";
		public static readonly string BEHAVIOUR_MODE_FAVOURED_DETOUR = "";

		// ################################################################################
		// BEHAVIOUR MODE RULE
		// ################################################################################
		public static readonly string BEHAVIOUR_MODE_RULE = "To provide a more realistic behaviour each mode can contains several different rules of " +
			"instructions at the same time, which allows your creature to do things in different ways, break and resume running activities, run " +
			"intermediate animation sequences, start effects or to play audio files as well.";

		public static readonly string BEHAVIOUR_MODE_RULES_ORDER = "Adapt the order type to define the desired sequence order of the given rules.";

		public static readonly string BEHAVIOUR_ANIMATION = "Here you can define the desired animation you want use for the selected rule. " +
			"Simply choose the desired type and adapt the required settings.";
		public static readonly string BEHAVIOUR_ANIMATION_NONE = "Animations are optional and not obligatory required, so you can " +
			"control also each kind of unanimated objects, such as dummies for testing and prototyping, simple bots and turrets or " +
			"movable waypoints.  ";
		public static readonly string BEHAVIOUR_ANIMATION_ANIMATOR = "By choosing the ANIMATOR ICECreatureControl will using the Animator " +
			"Interface to control Unity’s powerful Mecanim animation system. To facilitate setup and handling, ICECreatureControl provide " +
			"three different options to working with Mecanim: \n\n" +
				" - DIRECT – similar to the legacy animation handling \n" +
				" - CONDITIONS – triggering by specified values (float, integer, Boolean and trigger) \n" +
				" - ADVANCED – similar to CONDITIONS with additional settings for IK (ALPHA) \n";

		public static readonly string BEHAVIOUR_ANIMATION_ANIMATOR_CONTROL_TYPE_DIRECT = "DIRECT - similar to the legacy animation handling";
		public static readonly string BEHAVIOUR_ANIMATION_ANIMATOR_CONTROL_TYPE_CONDITIONS = "CONDITIONS – triggering by specified values (float, integer, Boolean and trigger)";
		public static readonly string BEHAVIOUR_ANIMATION_ANIMATOR_CONTROL_TYPE_ADVANCED = "ADVANCED – similar to CONDITIONS with additional settings for IK (ALPHA)";
		public static readonly string BEHAVIOUR_ANIMATION_ANIMATOR_CONTROL_TYPE_ADVANCED_INFO = "COMING SOON!";

		public static readonly string BEHAVIOUR_ANIMATION_ANIMATOR_ERROR_NO_CLIPS = "There are no clips available. Please check your Animator Controller!";
		public static readonly string BEHAVIOUR_ANIMATION_ANIMATION = "Working with legacy animations is the easiest and fastest way " +
			"to get nice-looking results. Simply select the desired animation, set the correct WrapMode and go.";
		public static readonly string BEHAVIOUR_ANIMATION_CLIP = "The direct use of animation clips is inadvisable and here only " +
			"implemented for the sake of completeness and for some single cases it could be helpful to have it. But apart from that it " +
			"works like the animation list. Simply assign the desired animation clip, set the correct WrapMode and go.";
		public static readonly string BEHAVIOUR_ANIMATION_CUSTOM = "";

		public static readonly string BEHAVIOUR_LENGTH = "Here you can define the play length of a rule by setting the ‘min’ and " +
			"‘max’ range. If both values are identical, the rule will be playing exactly for the specified time-span, otherwise " +
			"the length will be randomized based on the given values. Please note, that these settings are only available, if your " +
			"selected ‘Behaviour Mode’ contains more than one rule. If you ignore the play length settings while you have more " +
			"than one rule, the control tries to use the animation length but this could originate unlovely results and is " +
			"inadvisable.";
		public static readonly string BEHAVIOUR_AUDIO = "";
		public static readonly string BEHAVIOUR_EFFECT = "";
		public static readonly string BEHAVIOUR_INVENTORY = "";



		public static readonly string BEHAVIOUR_INVENTORY_TYPE = "";
		public static readonly string BEHAVIOUR_INVENTORY_TYPE_COLLECT = "";
		public static readonly string BEHAVIOUR_INVENTORY_TYPE_DISTRIBUTE = "";
		public static readonly string BEHAVIOUR_INVENTORY_TYPE_DISTRIBUTE_INTERVAL = "";
		public static readonly string BEHAVIOUR_INVENTORY_TYPE_EQUIP = "";
		public static readonly string BEHAVIOUR_INVENTORY_ITEM = "";
		public static readonly string BEHAVIOUR_INVENTORY_ITEM_PARENT = "";
	

		public static readonly string BEHAVIOUR_INFLUENCES = "Each Behaviour Rule can have an impact to your creature, these impacts " +
			"could be positive, such as the recreation (reducing of damage) while sleeping or eating, or negative, such as the increase " +
			"of damage while your creature is fighting. ";
		public static readonly string BEHAVIOUR_LINK = "Link provides the forwarding to a specific Rule or another Behaviour Mode as well.";
		public static readonly string BEHAVIOUR_LINK_SELECT = "";
		public static readonly string BEHAVIOUR_LINK_MODE = "";
		public static readonly string BEHAVIOUR_LINK_RULE = "";

		public static readonly string BEHAVIOUR_MOVEMENTS = "Additional to the Default Move, which you can adapt in the Essential section, each " +
			"Behaviour Rule provides enhanced Movement Options to customize the spatial movements of your creature according to the selected Animation, " +
			"the desired behaviour or other needs and requirements.\n\nIn difference to the Default Move settings, the Behaviour Movements contains in " +
			"addition to the known move specifications, further settings to define advanced movements, the viewing direction and the velocity, which " +
			"is absolutely essential if a desired behaviour is to be provided spatial position changes. In such cases it’s indispensable to adapt the " +
			"velocity settings.";
		public static readonly string BEHAVIOUR_MOVE_VELOCITY = "Velocity defines the desired speed of your creature in z-direction. Please note, that " +
			"the adjustment of the velocity is absolutely essential for all spatial movements. By activating the AUTO function, your creature will adjusts " +
			"its velocity according to the given target. The NEG flag allows you to use negative velocity values. Make sure that the velocity values are " +
			"suitable to the defined animation, otherwise your creature will do a moonwalk and please consider, that a zero value means no move.";

		public static readonly string BEHAVIOUR_MOVE_VELOCITY_SIDEWARDS = "";
		public static readonly string BEHAVIOUR_MOVE_VELOCITY_DRIFT = "";
		public static readonly string BEHAVIOUR_MOVE_VELOCITY_VERTICAL = "";

		public static readonly string BEHAVIOUR_MOVE_VELOCITY_VARIANCE = "Use the Velocity Variance Multiplier to randomize the Forward Velocity Vector " +
			"during the runtime, to force non-uniform movements of your creature (this will be helpful while using several instances of your creature)";

		public static readonly string BEHAVIOUR_MOVE_VELOCITY_INERTIA = "The Inertia value will be used to simulate the mass inertia to avoid abrupt " +
			"movements while the speed value changed.";

		public static readonly string BEHAVIOUR_MOVE_VIEWING_DIRECTION = "Viewing Direction defines the direction your creature will look at while the " +
			"behaviour is active. By default your creature will look at the move direction, but in some cases it can be helpful to force a specific direction " +
			"independent of the move direction.";

		public static readonly string BEHAVIOUR_MOVE_ANGULAR_VELOCITY = "Angular Velocity defines the desired rotational speed of your creature around its " +
			"y axis. This value affects the turning radius of your creature – the smaller the value, the larger the radius and vice versa. For a realistic " +
			"behaviour, this value should be consider the given physical facts and therefore suitable to the specified speed and the naturally to the animation " +
			"and the kind of creature as well.";

		public static readonly string BEHAVIOUR_MOVE_DEFAULT = "By default ICECreatureControl will use the Default Move for all standard situations, which describes " +
			"a direct manoeuvre form the current transform position to the TargetMovePosition. This manoeuvre will be sufficient in the majority of cases, but " +
			"it is less helpful if your creature have to veer away from a target, such as in an escape situation.";

		public static readonly string BEHAVIOUR_MOVE_RANDOM = "";
		public static readonly string BEHAVIOUR_MOVE_ORBIT = "Orbit Move defines an orbital move around the TargetMovePosition. You can adjust the initial " +
			"radius, a positive or negative shift value, so that your creature will following a spirally path and the associated minimum and maximum distances, " +
			"which specifies the end of the move. Please consider, that an orbital move with a zero shift value will not have a logical end, so you should make " +
			"sure that your creature will be not circling around the target infinitely. You could do this, for example, by setting a limited play length of the rule.";
		public static readonly string BEHAVIOUR_MOVE_DETOUR = "";
		public static readonly string BEHAVIOUR_MOVE_ESCAPE = "By using the Escape Move, your creature will move away from the target in the opposite direction of " +
			"the initial sighting line. You can randomize this escape direction by adapt the RandomEscapeAngle. The EscapeDistance defines the desired move distance, " +
			"which will added to the given SelectionRange of the target. Please consider, that you can affect the Escape behaviour by adjust the angular restriction " +
			"settings of the Target Selection Criteria and/or the Field Of View of your creature.";
		public static readonly string BEHAVIOUR_MOVE_ESCAPE_DISTANCE = "";
		public static readonly string BEHAVIOUR_MOVE_ESCAPE_ANGLE = "";
		public static readonly string BEHAVIOUR_MOVE_CUSTOM = "";
		public static readonly string BEHAVIOUR_MOVE_AVOID = "By using the Avoid Move, your creature will try to avoid the target by moving to the side, left or right " +
			"being based on the initial sighting line. Please consider, that you can affect the Avoid behaviour by adjust the angular restriction settings of the Target " +
			"Selection Criteria and/or the Field Of View of your creature.";
		public static readonly string BEHAVIOUR_MOVE_AVOID_DISTANCE = "";

		//public static readonly string REGISTER = "TODO";
		public static readonly string REGISTER_OPTIONS = "Options contains several optional features which could be helpful to you to organize your project and to reach the " +
			"desired goals without custom scripts, but in any case you are free to implement also your own solutions to handle these functions.";
		public static readonly string REGISTER_OPTIONS_GROUPS = "By using the Hierarchy Management the ICECreatureRegister makes sure that your scene stay clean and tidy " +
			"during the runtime. If UseHierarchyManagement is flagged all spawned Objects will be sorted according to the given structure. You are free to modify the given " +
			"structure as desired to adapt it to your project. ";
		public static readonly string REGISTER_OPTIONS_GROUPS_ROOT = "By default the root node will be the CreatureRegister element but you can define also your own object " +
			"or deactivate this node to arrange all groups to the top level of your scene hierarchy.";
		public static readonly string REGISTER_OPTIONS_GROUPS_CREATURES = "The Creatures node contains all GameObjects who using the ICECreatureControl script for their " +
			"registration (see also ICECreatureControl).";
		public static readonly string REGISTER_OPTIONS_GROUPS_ITEMS = "The Items node contains all GameObjects who using the ICECreatureItem script for their registration " +
			"(see also ICECreatureItem).";
		public static readonly string REGISTER_OPTIONS_GROUPS_LOCATIONS = "The Locations node contains all GameObjects who using the ICECreatureLocation script for their " +
			"registration (see also ICECreatureLocation).";
		public static readonly string REGISTER_OPTIONS_GROUPS_WAYPOINTS = "The Waypoints node contains all GameObjects who using the ICECreatureWaypoint script for their " +
			"registration (see also ICECreatureWaypoint).";
		public static readonly string REGISTER_OPTIONS_GROUPS_MARKERS = "The Markers node contains all GameObjects who using the ICECreatureMarker script for their registration " +
			"(see also ICECreatureMarker).";
		public static readonly string REGISTER_OPTIONS_GROUPS_OTHER = "The Other node contains all other GameObjects without specific ICE scripts.";
		public static readonly string REGISTER_OPTIONS_GROUPS_PLAYER = "The Players node contains all GameObjects who using the ICECreaturePlayer script for their registration " +
			"(see also ICECreaturePlayer).";

		public static readonly string REGISTER_OPTIONS_POOL_MANAGEMENT = "By using the Pool Management the Register can handle the population of your creatures and all your other " +
			"related objects such as locations, waypoints and items etc. as well. While UsePoolManagement is flagged you can activate the POOL functions for each reference " +
			"object to define the desired spawn and respawn settings. The Pool Management is an optional feature, you are free to handle it also by your own scripts or " +
			"third party products. ";
		public static readonly string REGISTER_OPTIONS_POOL_MANAGEMENT_GROUND_CHECK = "Use Spawn Ground Check to define how the ground level will be detected during a spawning " +
			"process. ";
		public static readonly string REGISTER_OPTIONS_POOL_MANAGEMENT_OBSTACLE_CHECK = "Use Spawn Obstacle Check to define the obstacle layers which should be avoided during a " +
			"spawning process. ";

		public static readonly string REGISTER_OPTIONS_SCENE_MANAGEMENT = "";
		public static readonly string REGISTER_OPTIONS_DONTDESTROYONLOAD = "While DontDestroyOnLoad is flagged the CreatureRegister will not be destroyed automatically when " +
			"loading a new scene.\n\nWhen loading a new level all objects in the scene are destroyed, then the objects in the new level are loaded. In order to preserve an " +
			"object during level loading call DontDestroyOnLoad on it. If the object is a component or game object then its entire transform hierarchy will not be destroyed " +
			"either.";
		public static readonly string REGISTER_OPTIONS_RANDOMSEED = "Random Seed defines the seed for the random number generator." +
			"The random number generator is not truly random but produces numbers in a preset sequence (the values in the sequence " +
			"'jump' around the range in such a way that they appear random for most purposes)." +
			"The point in the sequence where a particular run of pseudo-random values begins is selected using an integer called the " +
			"seed value. The seed is normally set from some arbitrary value like the system clock before the random number functions are " +
			"used. This prevents the same run of values from occurring each time a game is played and thus avoids predictable gameplay. " +
			"However, it is sometimes useful to produce the same run of pseudo-random values on demand by setting the seed yourself.";
		public static readonly string REGISTER_OPTIONS_RANDOMSEED_CUSTOM = "You might set your own seed to make sure that the same 'random'" +
			"pattern is produced each time the game is played.";

		public static readonly string REGISTER_OPTIONS_DEBUG = "Activate the Debug feature to show the Reference and SpawnPoint Gizmos.";
		public static readonly string REGISTER_OPTIONS_DEBUG_GIZMOS_MODE = "While ‘Draw Selected Only’ is flagged the Gizmos will be only " +
			"drawn when their GameObjects are selected.";
		public static readonly string REGISTER_OPTIONS_DEBUG_REFERENCES = "Use the Reference settings to adapt the colour of the REFERENCE " +
			"gizmos, also you can activate/deactivate TEXT to display or hide the description and use ENABLED to activate or deactivate " +
			"the REFERENCE gizmos.";
		public static readonly string REGISTER_OPTIONS_DEBUG_CLONES = "Use the Clones settings to adapt the colour of the CLONE gizmos, " +
			"also you can activate/deactivate TEXT to display or hide the description and use ENABLED to activate or deactivate the CLONE " +
			"gizmos.";
		public static readonly string REGISTER_OPTIONS_DEBUG_SPAWNPOINTS = "Use the SpawnPoints settings to adapt the colour of the SPAWNPOINT " +
			"gizmos, also you can activate/deactivate TEXT to display or hide the description and use ENABLED to activate or deactivate " +
			"the SPAWNPOINT gizmos.";

		public static readonly string REGISTER_REFERENCE_OBJECTS = "Reference Objects represents a list with all different types of GameObjects " +
			"your creatures should interact with it during the runtime (e.g. your Player, other creatures and NPCs, locations and waypoints, loot " +
			"items etc.). In Editor Mode this list provides a Popup with all object names while using the target access by name, also you can use the " +
			"internal pool management of the register to adapt the spawning and population management. During the runtime the list contains all spawned " +
			"objects, provides a quick and performance friendly access to the superior groups and also to each single element, handles the population " +
			"management and the communication between groups and objects. Therefore you should add at least one reference object of each GameObject " +
			"which you want to use as potential target or interaction object. Basically you can add each desired GameObject (scene objects or prefabs) " +
			"as a reference, which will be listed also as potential target in the selection popup while using the target access by name but please " +
			"consider that all objects have to handle their registration and deregistration during the runtime alone, otherwise it could be that a target " +
			"will not detect correctly and your creatures will ignore it. ICE provides several target scripts (e.g. Player, Location, Waypoints, Marker " +
			"and Items etc.) to handle these registration procedures and you should classify all unknown objects by assigning one of these scripts to your " +
			"object according to its function (Tip: you can use the 'C' buttons to add or remove the desired script)";

		public static readonly string REGISTER_REFERENCE_OBJECTS_ADD = "Use ‘Add Reference Object’ to add a new reference object to the register. Each " +
			"Reference Object represents a group of objects with the same characteristics, such as a specific species or item classes etc.";

		public static readonly string REGISTER_REFERENCE_OBJECT = "The ‘Reference Object’ represents the prototype and will be used as original for all runtime " +
			"initialized clones. The ‘Reference Object’ should be a Prefab, to make sure that it will not be destroyed during the runtime but you are free to use " +
			"also scene objects, but please consider that this could trigger a couple of problems if it got lost during the runtime. ";
		public static readonly string REGISTER_REFERENCE_OBJECT_GROUP = "A ‘Reference Object Group’ represents a group of objects with the same characteristics, such " +
			"as a specific species, an item class or a location etc. which should be used as potential target during the runtime. Basically a Reference Object Group " +
			"based on a single reference object which will be used as original for all runtime initialized clones. Such a reference object could be each GameObject " +
			"but the ICE framework provides several types of harmonised objects to increase the functionality and to optimize the interplay but also to simplify the " +
			"usability.";
		public static readonly string REGISTER_REFERENCE_OBJECT_POOL_SPAWN_MAX = "Adapt 'Max. Spawn Objects’ to defines the maximum number of objects which should be " +
			"spawn during the runtime. Activate ‘INITIAL’ to spawn all objects on start, otherwise they will spawn according to the given spawn interval.";
		public static readonly string REGISTER_REFERENCE_OBJECT_POOL_SPAWN_INTERVAL = "Adapt the ‘Spawn Interval’ values to define the minimum and maximum range in " +
			"seconds in which the objects should be spawn.";
		public static readonly string REGISTER_REFERENCE_OBJECT_POOL_SPAWN_PRIORITY = "If INITIAL is flagged you can adjust the ‘Spawn Priority’ to specify in which " +
			"order the given reference groups should spawn their clones. This could be important to make sure that locations and waypoints are already exists before " +
			"spawning your creatures.";
		public static readonly string REGISTER_REFERENCE_OBJECT_POOL_RANDOM_SIZE = "To force a more natural scenario you can activate ‘Random Size’ " +
			"to randomize the size of your initialized objects. ";
		public static readonly string REGISTER_REFERENCE_OBJECT_POOL_RANDOM_SIZE_VARIANCE = "Use ‘Size Variance’ to define the minimum and maximum limits.";
		public static readonly string REGISTER_REFERENCE_OBJECT_POOL_SOFTRESPAWN = "Activate ‘Soft Respawn’ to reuse already initialized objects " +
			"during the runtime. If ‘Soft Respawn’ is flagged already initialized but unused objects will be reset and reused without destroying " +
			"and creating new instances. ";
		public static readonly string REGISTER_REFERENCE_OBJECT_POOL_GROUP_USE = "Activate ‘Use Hierarchy Group’ to use an own group node for " +
			"all spawned instances of the reference object.";
		public static readonly string REGISTER_REFERENCE_OBJECT_POOL_GROUP_CUSTOM = "By default the Hierarchy Group Node will be a child of the " +
			"given reference node, but you can also define a custom GameObject as group node, in this case the instances will only initialized if " +
			"the defined node is active and available inside your scene and they will be also hidden or removed if the custom group node will be " +
			"deleted or deactivated. You can use this feature for your zone management to steering the visibility of initialized items, just by " +
			"activate or deactivate or load or unload the custom group node.";

		public static readonly string REGISTER_REFERENCE_OBJECT_SPAWN_POINT = "";
		public static readonly string REGISTER_REFERENCE_OBJECT_SPAWN_POINT_RANGE = "";


		// ################################################################################
		// LOCATION
		// ################################################################################
		public static readonly string LOCATION_NAME = "";
		public static readonly string ITEM_NAME = "";



		public static readonly string ATTRIBUTE_ADD = "";

		public static readonly string LIFESPAN = "";
		public static readonly string LIFESPAN_INTERVAL = "";
		public static readonly string LIFESPAN_DETACH = "";

		public static readonly string ODOUR_EFFECT = "";


		public static readonly string DEFAULT = "";
	}
/*
	public enum InfoID{
		COCKPIT,
		BEHAVIOUR,
		BEHAVIOUR_MODE,
		BEHAVIOUR_RULE,
		MISSIONS,
		INTERACTION,

		ENVIRONMENT,
		ENVIRONMENT_SURFACE,
		ENVIRONMENT_COLLISION
	}

	public static class ICEInfo {

		private static List<string> _library;

		static ICEInfo()
		{
			_library = new List<string>();

			int count = System.Enum.GetValues(typeof(InfoID)).Length;

			for( int i = 0; i < count ; i++)
				_library.Add( "" );

			Fill();
		}


		public static string Text( InfoID _id )
		{
			return _library[(int)_id];
		}

		private static void Add( InfoID _id )
		{
		}

		private static void Fill()
		{
			_library[ (int)InfoID.COCKPIT ] = "jl jlej ljlel" +
				"jjejkjejlejljljl";
		}

		public const string MY_NAME = "Gideon";



	}*/

}

