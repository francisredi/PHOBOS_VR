// ##############################################################################
//
// ICECreatureTargetAttribute.cs
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

namespace ICE.Creatures.Attributes
{
	public class ICECreatureTargetAttribute : MonoBehaviour {

		[SerializeField]
		private TargetObject m_Target = new TargetObject();
		public TargetObject Target{
			get{ 
				if( m_Target.TargetGameObject == null )
					m_Target.OverrideTargetGameObject( transform.gameObject );
				
				return m_Target; 
			}
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}