// ##############################################################################
//
// ICECreatureItem.cs
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
using ICE.Creatures.Objects;

namespace ICE.Creatures{

	public class ICECreatureLocation : MonoBehaviour {

		void Start () {
			if(ICECreatureRegister.Instance != null )
				ICECreatureRegister.Instance.Register( gameObject );
		}
		
		void OnDestroy() {
			if(ICECreatureRegister.Instance != null )
				ICECreatureRegister.Instance.Deregister( gameObject );
		}
	}
}
