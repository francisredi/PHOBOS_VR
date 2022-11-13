// ##############################################################################
//
// ICECreatureMouseTarget.cs
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

	public class ICECreatureMouseTarget : ICECreatureLocation {

		public float SurfaceOffset = 1.5f;

		private void Update()
		{
			if( ! Input.GetMouseButtonDown(0) )
				return;
			
			Ray _ray = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit _hit;

			if( ! Physics.Raycast(_ray, out _hit) )
				return;
			
			transform.position = _hit.point + _hit.normal*SurfaceOffset;
		}

	}
}
