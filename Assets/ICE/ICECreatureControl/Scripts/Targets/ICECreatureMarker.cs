// ##############################################################################
//
// ICECreatureMarker.cs
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

	public class ICECreatureMarker : MonoBehaviour {

		// LIFESPAN
		public bool UseLimitedLifespan = false;
		public float LifespanMin = 0;
		public float LifespanMax = 0;
		public float MaxLifespan = 360;
		public bool DetachChildren = false;

		[SerializeField]
		private OdourObject m_Odour = new OdourObject();
		public OdourObject Odour{
			set{ m_Odour = value; }
			get{ return m_Odour; }
		}

		void Start () {
			if( ICECreatureRegister.Instance != null )
				ICECreatureRegister.Instance.Register( gameObject );
		}
		
		void OnDestroy() {
			if( ICECreatureRegister.Instance != null )
				ICECreatureRegister.Instance.Deregister( gameObject );
		}


		private void Awake()
		{
			if( Mathf.Max( LifespanMin, LifespanMax ) > 0 && UseLimitedLifespan )
				Invoke("DestroyNow", Random.Range( LifespanMin, LifespanMax ) );

			Odour.StartEffect( transform );
		}
		
		
		private void DestroyNow()
		{
			Odour.StopEffect();
			
			if( DetachChildren )
				transform.DetachChildren();

			DestroyObject(gameObject);
		}
	}
}
