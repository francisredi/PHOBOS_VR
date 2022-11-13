// ##############################################################################
//
// ICECreaturePlayer.cs
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
using ICE;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;

namespace ICE.Creatures{

	public class ICECreaturePlayer : MonoBehaviour {

		[SerializeField]
		private OdourObject m_Odour = new OdourObject();
		public OdourObject Odour{
			set{ m_Odour = value; }
			get{ return m_Odour; }
		}

		[SerializeField]
		private InventoryObject m_Inventory = new InventoryObject();
		public InventoryObject Inventory{
			set{ m_Inventory = value; }
			get{ return m_Inventory; }
		}

		void Start () {
			if( ICECreatureRegister.Instance != null )
				ICECreatureRegister.Instance.Register( gameObject );
		}

		void OnDestroy() {

			if( ICECreatureRegister.Instance != null )
				ICECreatureRegister.Instance.Deregister( gameObject );
		}
	}
}
