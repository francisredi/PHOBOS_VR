// ##############################################################################
//
// ICECreatureOdourAttribute.cs
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

namespace ICE.Creatures.Attributes
{
	public class ICECreatureOdourAttribute : MonoBehaviour {

		public OdourType Odour = OdourType.NONE;
		public float OdourIntensity = 50f;
		public float OdourIntensityMax = 100f;
		public float OdourRange = 25f;
		public float OdourRangeMax = 100f;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
