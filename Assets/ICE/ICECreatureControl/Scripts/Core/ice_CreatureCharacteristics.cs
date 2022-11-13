// ##############################################################################
//
// ice_CreatureCharacteristic.cs
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
using ICE.Utilities.EnumTypes;
using ICE.Creatures.Objects;


namespace ICE.Creatures.Objects
{

	[System.Serializable]
	public class CharacteristicsObject : System.Object
	{
		//private GameObject m_Owner = null;

		public void Init( GameObject gameObject )
		{
			//m_Owner = gameObject;			
		}

		public float DefaultRunningSpeed = 7;
		public float DefaultWalkingSpeed = 3;
		public float DefaultTurningSpeed = 4;

		public bool IgnoreAnimationRun = false;
		public AnimationContainer AnimationRun;

		public bool IgnoreAnimationWalk = false;
		public AnimationContainer AnimationWalk;

		public bool IgnoreAnimationIdle = false;
		public AnimationContainer AnimationIdle;

		public bool IgnoreAnimationJump = true;
		public AnimationContainer AnimationJump;

		public bool IgnoreAnimationCrawlMove = true;
		public AnimationContainer AnimationCrawlMove;

		public bool IgnoreAnimationCrawlIdle = true;
		public AnimationContainer AnimationCrawlIdle;

		public bool IgnoreAnimationCrouchMove = true;
		public AnimationContainer AnimationCrouchMove;

		public bool IgnoreAnimationCrouchIdle = true;
		public AnimationContainer AnimationCrouchIdle;

		public bool IgnoreAnimationDead = true;
		public AnimationContainer AnimationDead;

		public bool IgnoreAnimationAttack = true;
		public AnimationContainer AnimationAttack;

		public bool IgnoreAnimationImpact = true;
		public AnimationContainer AnimationImpact;

		public MotionControlType MotionControl = MotionControlType.INTERNAL;
		public GroundOrientationType GroundOrientation = GroundOrientationType.BIPED;

		public CreatureTrophicLevelType TrophicLevel = CreatureTrophicLevelType.UNDEFINED;
		public bool IsCannibal = false;

		public bool UseAutoDetectInteractors = false;

		[SerializeField]
		private List<string> m_GroundLayers = new List<string>();
		public List<string> GroundLayers{
			get{ return m_GroundLayers; }
		}
	}
}
