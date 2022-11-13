// ##############################################################################
//
// ice_CreatureEssentials.cs
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

namespace ICE.Creatures.Objects
{

	[System.Serializable]
	public class EssentialsObject : System.Object 
	{
		public EssentialsObject()
		{
			m_Target = new TargetObject( TargetType.HOME );
		}

		private GameObject m_Owner = null;

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;
		}

		[SerializeField]
		private TargetObject m_Target = null;
		public virtual TargetObject Target 
		{
			set{ m_Target = value; }
			get{ return m_Target; }
		}

		public string BehaviourModeRun;
		public string BehaviourModeIdle;
		public string BehaviourModeWalk;
		public string BehaviourModeSpawn;
		public string BehaviourModeJump;
		public string BehaviourModeDead;

		public bool TargetReady()
		{
			if( Target.IsValid )
				return true;
			else
				return false;
		}

		public TargetObject PrepareTarget( CreatureObject _creature )
		{
			if( Target.GetBestTargetGameObject( m_Owner ) == null || TargetReady() == false || m_Owner == null || _creature == null || _creature.Behaviour == null )
				return null;
			
			// if the active target is not a HOME or if the creature outside the max range it have to travel to reach its target
			if( ! Target.TargetInMaxRange( m_Owner.transform.position ))
				Target.BehaviourModeKey = BehaviourModeRun;
			
			// if the creature reached the TargetMovePosition it should do the rendezvous behaviour
			else if( Target.TargetMoveComplete )
				Target.BehaviourModeKey = BehaviourModeIdle;
			
			// in all other case the creature should be standby and do some leisure activities
			else //if( Target.TargetRandomRange > 0 )
				Target.BehaviourModeKey = BehaviourModeWalk;

			return Target;
		}
	}
}
