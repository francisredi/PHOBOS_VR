// ##############################################################################
//
// ice_CreatureEnvironment.cs
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
	//--------------------------------------------------
	// ICECreatureObject.FootstepDataObject
	//--------------------------------------------------
	[System.Serializable]
	public class EnvironmentObject : System.Object
	{
		private GameObject m_Owner = null;

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;

			SurfaceHandler.Init( m_Owner );
			CollisionHandler.Init( m_Owner );

		}

		[SerializeField]
		private CollisionObject m_Collision = new CollisionObject();
		public CollisionObject CollisionHandler{
			set{ m_Collision = value; }
			get{ return m_Collision; }
		}

		[SerializeField]
		private SurfaceObject m_Surface = new SurfaceObject();
		public SurfaceObject SurfaceHandler{
			set{ m_Surface = value; }
			get{ return m_Surface; }
		}


	}
}
