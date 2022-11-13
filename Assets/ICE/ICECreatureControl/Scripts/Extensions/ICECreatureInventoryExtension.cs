// ##############################################################################
//
// ICECreatureInventoryExtension.cs
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
using ICE;
using ICE.Creatures;
using ICE.Creatures.Objects;

namespace ICE.Creatures.Extensions
{
	public class InventoryItem
	{
		public ICECreatureItem ReferenceItem = null;
	}

	public class ICECreatureInventoryExtension : MonoBehaviour 
	{
		public List<InventoryItem> Items = new List<InventoryItem>();

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
