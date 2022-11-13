// ##############################################################################
//
// ice_CreatureInventory.cs
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
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Utilities;



namespace ICE.Creatures.Objects
{
	public enum InventoryActionType{
		CollectActiveItem,
		DistributeItem,
		ChangeParent
	}

	[System.Serializable]
	public class InventoryActionDataObject
	{
		public InventoryActionDataObject(){}
		public InventoryActionDataObject( InventoryActionDataObject _data ){
			Copy( _data );
		}

		public void Copy( InventoryActionDataObject _data )
		{
			Type = _data.Type;
			ItemName = _data.ItemName;
			ParentName = _data.ParentName;
			DistributionIntervalMin = _data.DistributionIntervalMin;
			DistributionIntervalMax = _data.DistributionIntervalMax;
			DistributionMaximumInterval = _data.DistributionMaximumInterval;
			DistributionOffset = _data.DistributionOffset;
			DistributionRotation = _data.DistributionRotation;
			DistributionOffsetType = _data.DistributionOffsetType;
			DistributionOffsetRadius = _data.DistributionOffsetRadius;
		}

		public InventoryActionType Type;
		public string ItemName;
		public string ParentName;

		private GameObject m_DistributionItem;
		public float DistributionIntervalMin;
		public float DistributionIntervalMax;
		public float DistributionMaximumInterval;

		public Vector3 DistributionOffset;
		public Quaternion DistributionRotation;
		public RandomOffsetType DistributionOffsetType;
		public float DistributionOffsetRadius;

		private float m_IntervalTimer;
		private float m_Interval;

		public bool DistributionUpdateRequired()
		{
			if( Type == InventoryActionType.DistributeItem && Mathf.Max( DistributionIntervalMin, DistributionIntervalMax ) >= 0 )
			{
				m_IntervalTimer += Time.deltaTime;
				if( m_IntervalTimer > m_Interval )
				{
					m_IntervalTimer = 0;
					m_Interval = UnityEngine.Random.Range( DistributionIntervalMin, DistributionIntervalMax );
					return true;
				}
			}

			return false;
		}

		public bool ParentUpdateRequired()
		{
			if( Type == InventoryActionType.ChangeParent )
			{
				m_IntervalTimer += Time.deltaTime;
				if( m_IntervalTimer > m_Interval )
				{
					m_IntervalTimer = 0;
					m_Interval = UnityEngine.Random.Range( DistributionIntervalMin, DistributionIntervalMax );
					return true;
				}
			}

			return false;
		}

		public bool CollectActiveItemRequired()
		{
			if( Type == InventoryActionType.CollectActiveItem )
				return true;
			else
				return false;
		}
	}

	[System.Serializable]
	public class InventoryActionObject
	{
		public InventoryActionObject(){}
		public InventoryActionObject( InventoryActionObject _data ){
			Copy( _data );
		}

		public void Copy( InventoryActionObject _data )
		{
			Enabled = _data.Enabled;

			m_ActionList = new List<InventoryActionDataObject>();
			foreach( InventoryActionDataObject _action in _data.ActionList )
				m_ActionList.Add( new InventoryActionDataObject( _action ) );
		}

		public bool Enabled;

		[SerializeField]
		private List<InventoryActionDataObject> m_ActionList = new List<InventoryActionDataObject>();
		public List<InventoryActionDataObject> ActionList{
			set{ m_ActionList = value; }
			get{ return m_ActionList; }
		}


	}

	[System.Serializable]
	public class InventorySlotObject : System.Object
	{
		public InventorySlotObject(){
			ItemName = "";
			Amount = 0;
		}
		public InventorySlotObject( string _name ){
			ItemName = _name;	
			Amount = 1;
		}
		public InventorySlotObject( string _name, int _amount ){
			ItemName = _name;	
			Amount = _amount;
		}
		public InventorySlotObject( InventorySlotObject _slot ){
			Copy( _slot );
		}

		private GameObject m_Owner = null;
		public GameObject Owner{
			get{ return m_Owner; }
		}

		public void Init( GameObject _owner )
		{
			m_Owner = _owner;
		}

		public bool IsExclusive = false;
		public bool UseDetachOnDie = false;
		public bool IsTransferable = true;

		[SerializeField]
		private string m_SlotName = "";
		public string SlotName{
			set{
				m_SlotName = value;

				if( m_SlotName.Trim() == "" )
					m_SlotTransform = null;
				else if( m_SlotTransform == null || m_SlotTransform.name.Trim() != m_SlotName )
					m_SlotTransform = ICE.Utilities.SystemTools.FindChildByName( m_SlotName, m_Owner.transform );

				ItemObjectUpdate();
			}
			get{return m_SlotName;}
		}
			
		private Transform m_SlotTransform = null;
		public Transform SlotTransform{
			get{ 

				if( m_Owner == null || SlotName.Trim() == "" )
					m_SlotTransform = null;
				else if( m_SlotTransform == null || m_SlotTransform.name != SlotName || ! Application.isPlaying )
					m_SlotTransform = ICE.Utilities.SystemTools.FindChildByName( SlotName, m_Owner.transform );

				return m_SlotTransform;

			}
		}

		[SerializeField]
		private string m_ItemName = "";
		public string ItemName{
			set{
				m_ItemName = value;

				if( ! IsExclusive && m_ItemName != "" && m_Amount == 0 )
					m_Amount = 1; 
				
				ItemObjectUpdate();
			}
			get{ return m_ItemName; }
		}
			
		private GameObject m_ItemObject = null;
		public GameObject ItemObject{
			get{ return ItemObjectUpdate(); }
		}

		public GameObject ItemObjectUpdate()
		{
			if( m_ItemName.Trim() == "" || Amount == 0 || IsNotional || ItemUpdateRequired || ( SlotTransform != null && m_ItemObject != null && m_ItemObject.transform.IsChildOf( SlotTransform ) == false ) ) 
			{
				if( m_ItemObject != null )
					ICE.Utilities.SystemTools.Destroy( m_ItemObject );

				m_ItemObject = null;
			}

			if( m_ItemName.Trim() != "" && m_ItemObject == null && IsNotional == false && Amount > 0 )
			{
				Transform _parent = ICE.Utilities.SystemTools.FindChildByName( m_ItemName, SlotTransform );
				if( _parent != null )
					m_ItemObject = _parent.gameObject;

				if( m_ItemObject == null )
				{
					m_ItemObject = (GameObject)GameObject.Instantiate( ItemReferenceObject, SlotTransform.position, SlotTransform.rotation );
					m_ItemObject.name = ItemReferenceObject.name;
					m_ItemObject.SetActive( true );

					AttachToSlot( m_ItemObject );
				}
			}

			return m_ItemObject;
		}

		public bool ItemUpdateRequired{
			get{
				if( m_ItemObject != null && m_ItemObject.name.Trim() != m_ItemName.Trim() )
					return true;
				else
					return false;
			}
		}

		public GameObject ItemReferenceObject{
			get{
				if( ICECreatureRegister.Instance != null )
					return ICECreatureRegister.Instance.GetReferenceGameObjectByName( ItemName );
				else
					return null;
			}
		}



		[SerializeField]
		private int m_Amount = 0;
		public int Amount{
			set{ 
				m_Amount = ( ItemName != null && ItemName.Trim() != ""?value:0); 

				if( m_Amount < 0 )
					m_Amount = 0;
				else if( m_Amount > MaxAmount )
					m_Amount = MaxAmount;

				if( ! IsExclusive && m_Amount == 0 )
					m_ItemName = "";
				
				ItemObjectUpdate();
			}
			get{ return ( ItemName != null && ItemName.Trim() != ""?m_Amount:0); }
		}
		[SerializeField]
		private int m_MaxAmount = 1;
		public int MaxAmount{
			set{ m_MaxAmount = (value < 1?1:value); }
			get{ return (m_MaxAmount < 1?1:m_MaxAmount); }
		}
		public bool UseRandomAmount = false;
		public int FreeCapacity{
			get{ return MaxAmount - Amount;	}
		}

		private bool m_IsChild = false;
		public bool IsChild{
			get{ return m_IsChild; }
		}

		public bool IsNotional{
			get{
				if( SlotTransform == null )
					return true;
				else
					return false;
			}
		}

		public bool IsEquipped{
			get{
				if( SlotTransform != null && ItemObject != null && ItemObject.transform.IsChildOf( SlotTransform ) )
					return true;
				else
					return false;
			}
		}

		public bool AttachToSlot( GameObject _object ){
			return ICE.Utilities.SystemTools.AttachToTransform( _object, SlotTransform );
		}

		public bool DetachFromSlot( GameObject _object ){
			return ICE.Utilities.SystemTools.DetachFromTransform( _object );
		}

		public void DropItem()
		{
			if( Amount <= 0 || m_Owner == null || ItemReferenceObject == null )
				return;
			
			Vector3 _position = ( SlotTransform != null?SlotTransform.position:m_Owner.transform.position );
			Quaternion _rotation = ( SlotTransform != null?SlotTransform.rotation:m_Owner.transform.rotation );


			GameObject _item = (GameObject)GameObject.Instantiate( ItemReferenceObject, _position, _rotation );
			if( _item != null )
			{
				_item.name = ItemReferenceObject.name;

				if( DetachFromSlot( _item ) )
					Amount--;
			}
		}

		public bool Copy( InventorySlotObject _slot )
		{
			if( _slot == null )
				return false;

			IsExclusive = _slot.IsExclusive;
			ItemName = _slot.ItemName;
			Amount = _slot.Amount;
			MaxAmount = _slot.MaxAmount;	
			UseRandomAmount = _slot.UseRandomAmount;
			UseDetachOnDie = _slot.UseDetachOnDie;

			return true;
		}
		/*
		public int Merge( InventorySlotObject _slot )
		{
			if( _slot == null )
				return 0;

			IsExclusive = _slot.IsExclusive;
			Key = _slot.Key;
			Amount = Amount + _slot.Amount;
			MaxAmount = _slot.MaxAmount;	
			UseRandomAmount = _slot.UseRandomAmount;

			return true;
		}*/

		public int TryUpdateAmount( string _name, int _amount )
		{
			if( _name.Trim() == "" || ( ItemName.Trim() != "" && ItemName != _name ) )
				return _amount;
			else
			{
				if( ItemName.Trim() == "" )
					ItemName = _name;
				
				return UpdateAmount( _amount );
			}
		}

		public int UpdateAmount( int _amount )
		{
			int _rest = 0;
			int _new_amount = Amount + _amount;

			if( _new_amount < 0 ){
				_rest = _new_amount;
				Amount = 0;
			}
			else if( _new_amount > MaxAmount ){
				_rest = _new_amount - MaxAmount;
				Amount = MaxAmount;
			}
			else{
				Amount = _new_amount;
			}

			return _rest;
		}
	}

	[System.Serializable]
	public class InventoryDataObject : System.Object
	{
		public bool Enabled = false;
		public bool Foldout = false;
		public List<InventorySlotObject> Slots = new List<InventorySlotObject>();
		public int AvailableSlots{
			get{ return Slots.Count; }
		}
		public int MaxSlots = 9;
		public bool IgnoreInventoryOwner = false;

		public int LastCollectedObjectID = 0;

		public List<string> AvailableItems{
			get{
				List<string> _list = new List<string>();

				foreach( InventorySlotObject _slot in Slots )
				{
					if( _slot.ItemName != null && _slot.ItemName != "" && _list.Contains( _slot.ItemName ) == false )
						_list.Add( _slot.ItemName );
				}

				return _list;
			}
		}
	}

	[System.Serializable]
	public class InventoryObject : InventoryDataObject
	{
		private GameObject m_Owner = null;
		public GameObject Owner{
			get{ return m_Owner; }
		}

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;

			foreach( InventorySlotObject _slot in Slots )
				_slot.Init( m_Owner );
		}

		public void Reset()
		{
		}

		public int Insert( string _name, int _amount )
		{
			int _rest = _amount;
			InventorySlotObject _named_slot = GetSlotByItemName( _name );
			if( _named_slot != null )
				_rest = _named_slot.TryUpdateAmount( _name, _rest );
	
			if( _rest > 0 )
			{
				foreach( InventorySlotObject _slot in Slots )
					_rest = _slot.TryUpdateAmount( _name, _rest );
			}

			return _rest;
		}

		/*
		public InventoryObject Transfer()
		{
			InventoryObject _inventory = new InventoryObject();

			foreach( InventorySlotObject _slot in _inventory.Slots )
			{
				if( _slot.IsTransferable )
				{
					_inventory.Slots.Add( new InventorySlotObject( _slot ) );
					_slot.Amount = 0;
				}
			}

			if( ! _inventory.IgnoreInventoryOwner )
				_inventory.Insert( m_Owner.name, 1 );

			return _inventory;
		}*/

		public void Copy( InventoryObject _inventory )
		{
			if( _inventory == null )
				return;
			
			Slots.Clear();
			foreach( InventorySlotObject _slot in _inventory.Slots )
				Slots.Add( new InventorySlotObject( _slot ) );
		}

		public void Add( InventoryObject _inventory )
		{
			if( _inventory == null )
				return;

			//Update( _inventory.n, _slot.Amount );

			foreach( InventorySlotObject _slot in _inventory.Slots )
				_slot.Amount = Insert( _slot.ItemName, _slot.Amount );
		}

		public InventorySlotObject GetSlotByItemName( string _name )
		{
			foreach( InventorySlotObject _slot in Slots )
			{
				if( _slot != null && _slot.ItemName == _name )
					return _slot;
			}

			return null;
		}

		public int SlotItemAmount( int _index )
		{
			if( _index >= 0 && _index < Slots.Count )
				return Slots[_index].Amount;
			else
				return 0;
		}

		public int SlotItemMaxAmount( int _index )
		{
			if( _index >= 0 && _index < Slots.Count )
				return Slots[_index].MaxAmount;
			else
				return 0;
		}

		public string SlotItemName( int _index )
		{
			if( _index >= 0 && _index < Slots.Count )
				return Slots[_index].ItemName;
			else
				return "";
		}

		public void Detach()
		{
		}

		public void DetachOnDie()
		{
			foreach( InventorySlotObject _slot in Slots )
			{
				if( _slot != null && _slot.UseDetachOnDie == true && _slot.Amount > 0 )
				{
					GameObject _reference = _slot.ItemReferenceObject;
					if( _reference != null )
					{
						Vector3 _position = m_Owner.transform.TransformPoint(Vector3.zero);
						Quaternion _rotation = m_Owner.transform.rotation;

						while( _slot.Amount > 0 )
						{
							GameObject _item = (GameObject)GameObject.Instantiate( _reference, _position, _rotation );
							if( _item != null )
							{
								_item.name = _reference.name;
					
								if( _item.GetComponent<Rigidbody>() != null )
								{
									_item.GetComponent<Rigidbody>().useGravity = true;
									_item.GetComponent<Rigidbody>().isKinematic = false;
									_item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
								}
							}

							_slot.Amount--;
						}

						if( _slot.ItemObject )
							GameObject.Destroy( _slot.ItemObject );
					}
				}
			}
		}

		public void Action( InventoryActionObject _inventory )
		{
			foreach( InventoryActionDataObject _action in _inventory.ActionList )
			{
				if( _action.DistributionUpdateRequired() )
				{
					InventorySlotObject _slot = GetSlotByItemName( _action.ItemName );
					if( _slot != null )
						_slot.DropItem();
				}
				else if( _action.ParentUpdateRequired() )
				{
					InventorySlotObject _slot = GetSlotByItemName( _action.ItemName );
					if( _slot != null && _slot.Amount > 0 )
					{
						if( _slot.ItemObject != null )
						{
							_slot.SlotName = _action.ParentName;
							/*
							Transform _parent = SystemTools.FindChildByName( _action.ParentName, m_Owner.transform );

							if( _parent != null )
							{
								_slot.ItemObject.transform.parent = _parent;	
								_slot.ItemObject.transform.position = _parent.position;
								_slot.ItemObject.transform.rotation = _parent.rotation;
							}*/
						}
					}
				}
				else if( _action.CollectActiveItemRequired() )
				{
					ICECreatureControl _control = m_Owner.GetComponent<ICECreatureControl>();

					TargetObject _target = _control.Creature.ActiveTarget;


					if( _control != null && ICECreatureRegister.Instance != null && _target != null && _target.Selectors.TotalCheckIsValid  )
					{
						Add( _target.Inventory() );
						ICECreatureRegister.Instance.Cleanup( _target.TargetGameObject );
						_target.ResetTargetGameObject();
					}
				}
			}
		}


		public bool ItemExists( string _key ) 
		{
			if( _key == "" )
				return false;
			
			foreach( InventorySlotObject _item in Slots )
			{
				if( _item.ItemName == _key )
					return true;
			}

			return false;
		}
	}
}
