// ##############################################################################
//
// ice_CreatureEditorSharedTools.cs
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
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.Serialization;
using ICE;
using ICE.Styles;
using ICE.Layouts;
using ICE.Shared;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;

using ICE.Creatures.EditorInfos;
using ICE.Creatures.Attributes;
using ICE.Utilities;
using ICE.Utilities.EnumTypes;



namespace ICE.Creatures.EditorHandler
{


	public static class EditorSharedTools
	{	
		public static OdourContainer DrawOdourContainer( string _title, string _hint, OdourContainer _odour, string _help = "" )
		{
			//ODOUR BEGIN
			ICEEditorLayout.BeginHorizontal();
			_odour.Type = (OdourType)ICEEditorLayout.EnumPopup("Odour","", _odour.Type );	
			_odour.Enabled = ICEEditorLayout.ButtonCheck( "ENABLED", "", _odour.Enabled, ICEEditorStyle.ButtonMiddle );
			ICEEditorLayout.EndHorizontal( Info.STATUS_ODOUR );
			if( _odour.Type != OdourType.NONE )
			{					
				EditorGUI.indentLevel++;
				_odour.Intensity = ICEEditorLayout.MaxDefaultSlider( "Intensity", "", _odour.Intensity , 1, 0, ref _odour.IntensityMax, 0, Info.STATUS_ODOUR_INTENSITY );
				_odour.Range = ICEEditorLayout.MaxDefaultSlider( "Range", "", _odour.Range , 1, 0, ref _odour.RangeMax, 0, Info.STATUS_ODOUR_RANGE );

				_odour.UseMarker = ICEEditorLayout.Toggle( "Use Odour Marker", "" , _odour.UseMarker , Info.STATUS_ODOUR_MARKER );
				/*if( _odour.UseMarker )
				{
					EditorGUI.indentLevel++;
					ICEEditorLayout.RandomMinMaxGroupExt( "Interval", "", ref _odour.MarkerMinInterval, ref _odour.MarkerMaxInterval, 0, ref _odour.MarkerIntervalMax, 2, 5, 30, 0.25f, Info.STATUS_ODOUR_MARKER_INTERVAL );
					_odour.MarkerPrefab = (ICECreatureMarker)EditorGUILayout.ObjectField( "Marker Prefab", _odour.MarkerPrefab, typeof(ICECreatureMarker), false );
					EditorGUI.indentLevel--;
				}*/

				_odour.UseEffect = ICEEditorLayout.Toggle( "Use Odour Effect", "" , _odour.UseEffect , Info.ODOUR_EFFECT );
				/*if( _odour.UseEffect )
				{
					EditorGUI.indentLevel++;
					_odour.EffectPrefab = (GameObject)EditorGUILayout.ObjectField( "Effect Prefab", _odour.EffectPrefab, typeof(GameObject), false );
					EditorGUI.indentLevel--;
				}*/
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();
			}
			// ODOUR END

			return _odour;
		}

		public static void DrawOdourObject( string _title, string _hint, OdourObject _odour, string _help = "" )
		{
			if( _odour == null )
				return;
			
			//ODOUR BEGIN
			_odour.Type = (OdourType)ICEEditorLayout.EnumPopup("Odour","", _odour.Type, Info.STATUS_ODOUR );			
			if( _odour.Type != OdourType.NONE )
			{					
				EditorGUI.indentLevel++;
				_odour.Intensity = ICEEditorLayout.MaxDefaultSlider( "Intensity", "", _odour.Intensity , 1, 0, ref _odour.IntensityMax, 0, Info.STATUS_ODOUR_INTENSITY );
				_odour.Range = ICEEditorLayout.MaxDefaultSlider( "Range", "", _odour.Range , 1, 0, ref _odour.RangeMax, 0, Info.STATUS_ODOUR_RANGE );

				_odour.UseMarker = ICEEditorLayout.Toggle( "Use Odour Marker", "" , _odour.UseMarker , Info.STATUS_ODOUR_MARKER );
				if( _odour.UseMarker )
				{
					EditorGUI.indentLevel++;
					ICEEditorLayout.RandomMinMaxGroupExt( "Interval", "", ref _odour.MarkerMinInterval, ref _odour.MarkerMaxInterval, 0, ref _odour.MarkerIntervalMax, 2, 5, 30, 0.25f, Info.STATUS_ODOUR_MARKER_INTERVAL );
					_odour.MarkerPrefab = (ICECreatureMarker)EditorGUILayout.ObjectField( "Marker Prefab", _odour.MarkerPrefab, typeof(ICECreatureMarker), false );
					EditorGUI.indentLevel--;
				}

				_odour.UseEffect = ICEEditorLayout.Toggle( "Use Odour Effect", "" , _odour.UseEffect , Info.ODOUR_EFFECT );
				if( _odour.UseEffect )
				{
					EditorGUI.indentLevel++;
					_odour.EffectPrefab = (GameObject)EditorGUILayout.ObjectField( "Effect Prefab", _odour.EffectPrefab, typeof(GameObject), false );
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();
			}
			// ODOUR END
		}

		public static void DrawObstacleCheck( ref ObstacleCheckType _type, List<string> _layers, string _help = ""  )
		{
			ICEEditorLayout.BeginHorizontal();
			_type = (ObstacleCheckType)ICEEditorLayout.EnumPopup("Obstacle Check", "Method to handle obstacle checks and avoidances", _type );
			if( _type == ObstacleCheckType.BASIC )
			{
				if (GUILayout.Button("Add Layer", ICEEditorStyle.ButtonMiddle ))
					_layers.Add( (LayerMask.NameToLayer("Obstacle") != -1?"Obstacle":"Default") );

			}				
			ICEEditorLayout.EndHorizontal( _help );

			if( _type == ObstacleCheckType.BASIC )
			{
				EditorGUI.indentLevel++;
					DrawLayers( _layers );
				EditorGUI.indentLevel--;
			}
		}

		public static void DrawGroundCheck( ref GroundCheckType _type, List<string> _layers, string _help = "" )
		{
			ICEEditorLayout.BeginHorizontal();
				_type = (GroundCheckType)ICEEditorLayout.EnumPopup("Ground Check", "Method to handle ground related checks and movements", _type );
				if( _type == GroundCheckType.RAYCAST )
				{
					if (GUILayout.Button("Add Layer", ICEEditorStyle.ButtonMiddle ))
						_layers.Add( (LayerMask.NameToLayer("Terrain") != -1?"Terrain":"Default") );

				}				
			ICEEditorLayout.EndHorizontal( _help );

			if( _type == GroundCheckType.RAYCAST )
			{
				EditorGUI.indentLevel++;
					DrawLayers( _layers );
				EditorGUI.indentLevel--;
			}
		}

		/// <summary>
		/// Draws the ground layers.
		/// </summary>
		/// <param name="_layers">_layers.</param>
		public static void DrawLayers( List<string> _layers )
		{
			if( _layers.Count == 0 )
			{
				ICEEditorLayout.BeginHorizontal();
				GUILayout.FlexibleSpace();					
				EditorGUILayout.LabelField( new GUIContent( " - No Layer defined -", "" ) );
				GUILayout.FlexibleSpace();
				ICEEditorLayout.EndHorizontal();
			}
			else
			{
				for( int i = 0 ; i < _layers.Count; i++ )
				{
					ICEEditorLayout.BeginHorizontal();
					GUI.backgroundColor = new Vector4( 0.7f, 0.9f, 0.9f, 0.5f);

					string _title = "Layer";
					if( _layers[i] == "Water" )
						_title = "Water Layer";

					int _layer = LayerMask.NameToLayer(_layers[i]);

					if( _layer == -1 )
					{
						GUI.backgroundColor = Color.red;
						EditorGUILayout.PrefixLabel( new GUIContent( _title + " (MISSING)", "CREATE MISSING '" + _layers[i] + "' LAYER" ) );
						if( GUILayout.Button("CREATE MISSING '" + _layers[i] + "' LAYER", ICEEditorStyle.ButtonFlex ) )
						{
							ICEEditorTools.AddLayer( _layers[i] );
						}
					}
					else
					{
						_layers[i] = LayerMask.LayerToName( EditorGUILayout.LayerField( _title, _layer ) );
					}
			
					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
					if (GUILayout.Button("X", ICEEditorStyle.CMDButton ))
					{
						_layers.RemoveAt(i);
						--i;
						return;
					}
					ICEEditorLayout.EndHorizontal();
				}
			}
		}

		public static BodyContainer DrawBody( ICECreatureControl _control, BodyContainer _body )
		{
			if( _body.Type == GroundOrientationType.DEFAULT  || _body.Type == GroundOrientationType.BIPED )
				_body.UseAdvanced = false;

			ICEEditorLayout.BeginHorizontal();
				_body.Type = (GroundOrientationType)ICEEditorLayout.EnumPopup("Body Orientation", "Vertical direction of the body relative to the ground", _body.Type );
				EditorGUI.BeginDisabledGroup( _body.Type == GroundOrientationType.DEFAULT  || _body.Type == GroundOrientationType.BIPED );
				_body.UseAdvanced = ICEEditorLayout.ButtonCheck( "ADV", "", _body.UseAdvanced, ICEEditorStyle.CMDButtonDouble );

				EditorGUI.EndDisabledGroup();
			ICEEditorLayout.EndHorizontal( Info.ESSENTIALS_SYSTEM_GROUND_ORIENTATION );

			if( _body.Type != GroundOrientationType.DEFAULT )
			{
				if( _body.UseAdvanced )
				{
					if( _body.DefaultWidth == 0 || _body.DefaultLength == 0 || _body.DefaultHeight == 0 || _body.DefaultWidth != _body.Width || _body.DefaultLength != _body.Length || _body.DefaultHeight != _body.Height )
						_body.GetDefaultSize( _control.gameObject );

					EditorGUI.indentLevel++;
						_body.Width = ICEEditorLayout.DefaultSlider( "Width", "", _body.Width, 0.01f, 0, 45, _body.DefaultWidth );
						EditorGUI.indentLevel++;
						_body.WidthOffset = ICEEditorLayout.DefaultSlider( "x-Offset", "", _body.WidthOffset, 0.01f, -10, 10, 0 );
						EditorGUI.indentLevel--;
						_body.Length = ICEEditorLayout.DefaultSlider( "Depth", "", _body.Length, 0.01f, 0, 45, _body.DefaultLength );
						EditorGUI.indentLevel++;
						_body.DepthOffset = ICEEditorLayout.DefaultSlider( "z-Offset", "", _body.DepthOffset, 0.01f, -10, 10, 0 );
						EditorGUI.indentLevel--;			
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}

				EditorGUI.indentLevel++;
					_body.UseLeaningTurn = ICEEditorLayout.Toggle("Use Leaning Turn", "Allows to lean into a turn", _body.UseLeaningTurn ,  Info.ESSENTIALS_SYSTEM_LEAN_ANGLE );
					EditorGUI.indentLevel++;
					if( _body.UseLeaningTurn )
					{
						Info.Warning( Info.ESSENTIALS_SYSTEM_LEAN_ANGLE_WARNING );
						_body.LeanAngleMultiplier = ICEEditorLayout.DefaultSlider( "Lean Angle Multiplier", "Lean angle multiplier", _body.LeanAngleMultiplier, 0.05f, 0, 1, 0.5f );
						_body.MaxLeanAngle = ICEEditorLayout.DefaultSlider( "Max. Lean Angle", "Maximum lean angle", _body.MaxLeanAngle, 0.25f, 0, 45, 35 );
						EditorGUILayout.Separator();
					}
					EditorGUI.indentLevel--;
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();
			}

			return _body;
		}

		public static void DrawInventoryObject( string _title, string _hint, InventoryObject _inventory, GameObject _owner, string _help = "" )
		{
			if( _inventory == null )
				return;

			_inventory.Enabled = ICEEditorLayout.Toggle( _title, _hint, _inventory.Enabled  , _help );
			if( _inventory.Enabled )
			{
				EditorGUI.indentLevel++;
				DrawInventoryObjectContent( _owner, _inventory );
				EditorGUI.indentLevel--;
			}
		}

		public static void DrawInventoryObjectContent( GameObject _owner, InventoryObject _inventory )
		{
			if( _inventory == null )
				return;					

			// HEAD BEGIN
			ICEEditorLayout.BeginHorizontal();

				float _max_slots = _inventory.MaxSlots;
				int _slots = (int)ICEEditorLayout.MaxBasicSlider( "Slots (" + _inventory.Slots.Count + ")", "", _inventory.AvailableSlots, 1,0, ref _max_slots, "" );
				_inventory.MaxSlots = (int)_max_slots;

				while( _slots < _inventory.AvailableSlots )
					_inventory.Slots.RemoveAt(_inventory.Slots.Count-1);
				while( _slots > _inventory.AvailableSlots )
					_inventory.Slots.Add( new InventorySlotObject() );

				if( ICEEditorLayout.Button( "RES", "", ICEEditorStyle.CMDButtonDouble ) )
					_inventory.Slots.Clear();

				//_inventory.IgnoreInventoryOwner = ICEEditorLayout.ButtonCheck( "", "", _inventory.IgnoreInventoryOwner, ICEEditorStyle.CMDButtonDouble );
	
			ICEEditorLayout.EndHorizontal( Info.STATUS_INVENTORY_SLOTS );
			// HEAD END

			// LIST BEGIN
			EditorGUI.indentLevel++;
				for( int _i = 0;_i < _inventory.Slots.Count ; _i++ )
				{
					InventorySlotObject _slot = _inventory.Slots[_i];

					if( _slot == null )
						continue;

					_slot.Init( _owner );

					ICEEditorLayout.BeginHorizontal();

					_slot.SlotName = ICEEditorLayout.TransformPopup( "Slot #" + (_i + 1) + (_slot.IsNotional?" [notional]":""), "", _slot.SlotName, _owner.transform, true );
			/*
					EditorGUI.BeginDisabledGroup( _slot.IsNotional == true );

						if( _slot.IsEquipped )
						{
							if( ICEEditorLayout.Button( "FREE", "", ICEEditorStyle.ButtonMiddle ) )
								_slot.Remove( _owner );
						}
						else
						{
							if( ICEEditorLayout.Button( "EQUIP", "", ICEEditorStyle.ButtonMiddle ) )
								_slot.Equip( _owner );
						}

					EditorGUI.EndDisabledGroup();*/

					if( ICEEditorLayout.ButtonUp() )
					{
						InventorySlotObject _tmp_obj = _inventory.Slots[_i]; 
						_inventory.Slots.RemoveAt( _i );

						if( _i - 1 < 0 )
							_inventory.Slots.Add( _tmp_obj );
						else
							_inventory.Slots.Insert( _i - 1, _tmp_obj );

						return;
					}	
					if( ICEEditorLayout.ButtonDown() )
					{
						InventorySlotObject _tmp_obj = _inventory.Slots[_i]; 
						_inventory.Slots.RemoveAt( _i );

						if( _i + 1 > _inventory.Slots.Count )
							_inventory.Slots.Insert( 0, _tmp_obj );
						else
							_inventory.Slots.Insert( _i +1, _tmp_obj );

						return;
					}	

					
					if( ICEEditorLayout.Button( "DEL", "", ICEEditorStyle.CMDButtonDouble ) )
					{
						_inventory.Slots.Remove( _slot );
						return;
					}

				ICEEditorLayout.EndHorizontal( Info.STATUS_INVENTORY_SLOT );

				EditorGUI.indentLevel++;

					ICEEditorLayout.BeginHorizontal();

						if( _slot.ItemObject )
							GUI.backgroundColor = Color.green;
						else if( _slot.ItemReferenceObject )
							GUI.backgroundColor = Color.yellow;
						else
							GUI.backgroundColor = Color.blue;

						_slot.ItemName = ItemPopup( "Item", "Assigned item for slot #" + ( _i + 1 ) + "", _slot.ItemName );
		
						GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

						EditorGUI.BeginDisabledGroup( _slot.ItemObject == null );
							//ICEEditorLayout.ButtonShowTransform( "DTL", "Show details", _item, ICEEditorStyle.CMDButtonDouble );
							ICEEditorLayout.ButtonSelectObject( _slot.ItemObject , ICEEditorStyle.CMDButtonDouble  );
						EditorGUI.EndDisabledGroup();

						EditorGUI.BeginDisabledGroup( _slot.IsNotional == true || _slot.IsEquipped == false );								
							if( ICEEditorLayout.Button( "DROP", "", ICEEditorStyle.ButtonMiddle ) )
								_slot.DropItem();										
						EditorGUI.EndDisabledGroup();

						EditorGUI.BeginDisabledGroup( _slot.ItemName == "" );	
							_slot.IsExclusive = ICEEditorLayout.ButtonCheck( "EXCL", "Reserved slot for the specified item type", _slot.IsExclusive, ICEEditorStyle.CMDButtonDouble );
						EditorGUI.EndDisabledGroup();
			
	
	
						_slot.UseDetachOnDie = ICEEditorLayout.ButtonCheck( "DOD", "Detach item on die", _slot.UseDetachOnDie, ICEEditorStyle.CMDButtonDouble );


						if( ICEEditorLayout.Button( "CLR", "", ICEEditorStyle.CMDButtonDouble ) )
							_slot.ItemName = "";
	
				ICEEditorLayout.EndHorizontal( Info.STATUS_INVENTORY_SLOT_ITEM );

					ICEEditorLayout.BeginHorizontal();
					
							float _max = _slot.MaxAmount;
							_slot.Amount = (int)ICEEditorLayout.MaxDefaultSlider( "Amount", "Current amount and maximum capacity for slot #" + ( _i + 1 ), _slot.Amount, 1, 0, ref _max, 0, "" );
							_slot.MaxAmount = (int)_max;

							EditorGUI.BeginDisabledGroup( _slot.ItemName == "" );	
								_slot.UseRandomAmount = ICEEditorLayout.ButtonCheck( "RND", "", _slot.UseRandomAmount, ICEEditorStyle.CMDButtonDouble );
							EditorGUI.EndDisabledGroup();
							
		
					ICEEditorLayout.EndHorizontal( Info.STATUS_INVENTORY_SLOT_AMOUNT );

				EditorGUI.indentLevel--;
	
				}
			EditorGUI.indentLevel--;
			// LIST END

		}

		public static string InventoryItemPopup( ICECreatureControl _control, string _title, string _hint, string _name, string _help = "" )
		{
			if( _control == null )
				return "";

			InventoryDataObject _inventory = _control.Creature.Status.Inventory;

			if( _inventory == null || _inventory.Slots.Count == 0  )
			{
				EditorGUILayout.LabelField( _title + " (- no reference item - check register!)"  );
				return "";
			}
			else
			{
				List<string> _keys = _inventory.AvailableItems;

				_keys.Insert(0, " " );

				string[] _names = new string[_keys.Count];

				int _index = 0;
				for(int i=0;i < _keys.Count ;i++)
				{
					if( _keys[i] == " " && i == 0 )
						_names[i] = " ";
					else if( _keys[i] == " " )
						_names[i] = "";
					else
						_names[i] = _keys[i];

					if( _name == _names[i] )
						_index = i;

				}

				if( ICE.Utilities.SystemTools.FindChildByName( _name, _control.gameObject.transform ) != null )
					GUI.backgroundColor = Color.green;

				if( _title.Trim() != "" )
					_index = ICEEditorLayout.Popup( _title, _hint, _index, _names, _help );
				else
					_index = EditorGUILayout.Popup( _index, _names );

				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

				return _names[_index].Trim();

			}	
		}
			
		public static string ItemPopup( string _title, string _hint, string _group, string _help = "" )
		{
			ICECreatureRegister _register = ICECreatureRegister.Instance;

			if( _register == null || _register.ReferenceItemNames.Count == 0  )
			{
				EditorGUILayout.LabelField( _title + " (- no reference item - check register!)"  );
				return null;
			}
			else
			{
				List<string> _keys = _register.ReferenceItemNames;

				_keys.Insert(0, " " );

				string[] _names = new string[_keys.Count];

				int _index = 0;
				for(int i=0;i < _keys.Count ;i++)
				{
					if( _keys[i] == " " && i == 0 )
						_names[i] = " ";
					else if( _keys[i] == " " )
						_names[i] = "";
					else
						_names[i] = _keys[i];

					if( _group == _names[i] )
						_index = i;

				}

				if( _title.Trim() != "" )
					_index = ICEEditorLayout.Popup( _title, _hint, _index, _names, _help );
				else
					_index = EditorGUILayout.Popup( _index, _names );


				return _names[_index].Trim();

			}	
		}

		public static string TargetPopup( string _group ){
			return TargetPopup( "", "", _group, "" );
		}

		public static string TargetPopup( string _title, string _hint, string _group, string _help = "" )
		{
			ICECreatureRegister _register = ICECreatureRegister.Instance;
			
			if( _register == null || _register.ReferenceGameObjects.Count == 0 )
			{
				EditorGUILayout.LabelField( _title );
				return null;
			}
			else
			{
				List<string> _keys = _register.ReferenceGameObjectNames;
				
				string[] _names = new string[_keys.Count];
				
				int _index = 0;
				for(int i=0;i < _keys.Count ;i++)
				{
					if( _keys[i] == " " && i == 0 )
						_names[i] = " ";
					else if( _keys[i] == " " )
						_names[i] = "";
					else
						_names[i] = _keys[i];
					
					if( _group == _names[i] )
						_index = i;
					
				}
				/*
				if( _targets[_index] != null && _targets[_index].Status.isPrefab )
					_title += " (prefab)";
				else
					_title += " (scene)";*/

				if( _title.Trim() != "" )
					_index = ICEEditorLayout.Popup( _title, _hint, _index, _names, _help );
				else
					_index = EditorGUILayout.Popup( _index, _names );
				
				return _names[_index];
				
			}	
		}
		/*
		public static string RegisterPopup( string _title, string _group, string _help = "" )
		{
			ICECreatureRegister _register = ICECreatureRegister.Instance;// GameObject.FindObjectOfType<ICECreatureRegister>();
			
			if( _register == null )
			{
				EditorGUILayout.LabelField( _title );
				return null;
			}
			else if( _register.ReferenceCreatures.Count == 0 )
			{
				EditorGUILayout.LabelField( _title );
				return null;
			}
			else
			{
				List<CreatureReferenceObject> _creatures = _register.ReferenceCreatures;
								
				string[] _names = new string[_creatures.Count];

				int _index = 0;
				for(int i=0;i < _creatures.Count ;i++)
				{
					_names[i] = _creatures[i].Name;
					
					if( _group == _names[i] )
						_index = i;
					
				}

				if( _creatures[_index] != null && _creatures[_index].Status.isPrefab )
					_title += " (prefab)";
				else
					_title += " (scene)";

				_index = ICEEditorLayout.Popup( _title,"", _index, _names, _help );
				
				return _names[_index];
				
			}		
		}
*/
		public static void DrawInRangeBehaviour( ICECreatureControl _control, ref string _leisure, ref string rendezvous, ref float _duration, ref bool _transit, float _range, int _index = 0 )
		{
			_duration = ICEEditorLayout.DurationSlider( "Duration Of Stay", "Desired duration of stay", _duration, Init.DURATION_OF_STAY_STEP, Init.DURATION_OF_STAY_MIN, Init.DURATION_OF_STAY_MAX,Init.DURATION_OF_STAY_DEFAULT, ref _transit );

			if( _transit == false )
			{
				EditorGUI.indentLevel++;

				if( _range > 0 )
				{
					_leisure = EditorBehaviour.BehaviourSelect( _control, "Leisure", "Randomized leisure activities after reaching the Random Range of the target. Please note, if the Random Range is adjusted to zero, leisure is not available.", _leisure, "WP_LEISURE" + (_index>0?"_"+_index:"") );
				}

				EditorGUI.BeginDisabledGroup( _duration == 0 );
				rendezvous = EditorBehaviour.BehaviourSelect( _control, "Rendezvous", "Action behaviour after reaching the Stop Distance of the given target move position.", rendezvous, "WP_RENDEZVOUS" + (_index>0?"_"+_index:"") );
				EditorGUI.EndDisabledGroup();
				
				EditorGUI.indentLevel--;
			}
		}

		public static InventoryActionObject DrawSharedInventory( ICECreatureControl _control, InventoryActionObject _inventory, string _help )
		{
			_inventory.Enabled = ICEEditorLayout.ToggleLeft( "Inventory","", _inventory.Enabled, true, _help );
			if( ! _inventory.Enabled )
				return _inventory;

			foreach( InventoryActionDataObject _action in _inventory.ActionList )
			{
				string _help_type = Info.BEHAVIOUR_INVENTORY_TYPE;
				if( _action.Type == InventoryActionType.CollectActiveItem )
					_help_type = Info.BEHAVIOUR_INVENTORY_TYPE_COLLECT;
				else if( _action.Type == InventoryActionType.DistributeItem )
					_help_type = Info.BEHAVIOUR_INVENTORY_TYPE_DISTRIBUTE;
				else if( _action.Type == InventoryActionType.ChangeParent )
					_help_type = Info.BEHAVIOUR_INVENTORY_TYPE_EQUIP;

				EditorGUI.indentLevel++;
					ICEEditorLayout.BeginHorizontal();
						_action.Type = (InventoryActionType)ICEEditorLayout.EnumPopup( "Action Type","Collects the given active target item object", _action.Type );
					
						if( ICEEditorLayout.Button( "X", "Delete Action", ICEEditorStyle.CMDButtonDouble ) )
						{
							_inventory.ActionList.Remove( _action );
							return _inventory;
						}
					ICEEditorLayout.EndHorizontal( _help_type );
					EditorGUI.indentLevel++;
						if( _action.Type == InventoryActionType.CollectActiveItem )
						{
						}
						else if( _action.Type == InventoryActionType.DistributeItem )
						{
							if( _action.DistributionMaximumInterval <= 0 )
								_action.DistributionMaximumInterval = 60;

							ICEEditorLayout.BeginHorizontal();
								ICEEditorLayout.MinMaxGroupSimple( "Distribution Interval (secs.)", "", ref _action.DistributionIntervalMin, ref _action.DistributionIntervalMax, 0, ref _action.DistributionMaximumInterval, 0.25f, 40 );
								if( ICEEditorLayout.Button( "RND", "", ICEEditorStyle.CMDButtonDouble ) )
								{
									_action.DistributionIntervalMin = UnityEngine.Random.Range( 0, _action.DistributionIntervalMax );
									_action.DistributionIntervalMax = UnityEngine.Random.Range( _action.DistributionIntervalMin, _action.DistributionMaximumInterval );
								}
								if( ICEEditorLayout.Button( "D", "", ICEEditorStyle.CMDButtonDouble ) )
								{
									_action.DistributionIntervalMin = 5;
									_action.DistributionIntervalMax = 10;
								}
							ICEEditorLayout.EndHorizontal( Info.BEHAVIOUR_INVENTORY_TYPE_DISTRIBUTE_INTERVAL );

							_action.ItemName = InventoryItemPopup( _control, "Distribution Item", "", _action.ItemName, Info.BEHAVIOUR_INVENTORY_ITEM ); 
							EditorGUI.indentLevel++;
								_action.ParentName = ICEEditorLayout.TransformPopup( "Parent", "", _action.ParentName, _control.transform, false );
								_action.DistributionOffsetType = (RandomOffsetType)ICEEditorLayout.EnumPopup( "Offset Type","", _action.DistributionOffsetType );
								EditorGUI.indentLevel++;
									if( _action.DistributionOffsetType == RandomOffsetType.EXACT )
										_action.DistributionOffset = EditorGUILayout.Vector3Field( "Offset", _action.DistributionOffset );
									else 
										_action.DistributionOffsetRadius = ICEEditorLayout.Slider( "Offset Radius", "", _action.DistributionOffsetRadius, 0.25f, 0, 100 );
								EditorGUI.indentLevel--;
							EditorGUI.indentLevel--;

						}
						else if( _action.Type == InventoryActionType.ChangeParent )
						{
							_action.ItemName = InventoryItemPopup( _control, "Inventory Item", "", _action.ItemName, Info.BEHAVIOUR_INVENTORY_ITEM ); 
							_action.ParentName = ICEEditorLayout.TransformPopup( "Desired Parent", "", _action.ParentName, _control.transform, false );
						}
					EditorGUI.indentLevel--;
				EditorGUI.indentLevel--;
			}	

			ICEEditorLayout.BeginHorizontal();
			ICEEditorLayout.Label( "Add Inventory Action", false );
				if( ICEEditorLayout.Button( "ADD", "", ICEEditorStyle.CMDButtonDouble ) )
					_inventory.ActionList.Add( new InventoryActionDataObject() );
			ICEEditorLayout.EndHorizontal( Info.BEHAVIOUR_INVENTORY_TYPE_DISTRIBUTE_INTERVAL );

			return _inventory;
		}

		public static EffectContainer DrawSharedEffect( ICECreatureControl _control, EffectContainer _effect, string _help )
		{
			_effect.Enabled = ICEEditorLayout.ToggleLeft("Effect","", _effect.Enabled, true, _help );
			
			if( _effect.Enabled )
			{
				if( _effect.MaxInterval == 0 )
					_effect.MaxInterval = 60;

				EditorGUI.indentLevel++;
						
					ICEEditorLayout.BeginHorizontal();
						_effect.ReferenceObject = (GameObject)EditorGUILayout.ObjectField( "Reference", _effect.ReferenceObject, typeof(GameObject), false);
						EditorGUI.BeginDisabledGroup( _effect.ReferenceObject == null );
							_effect.Detach = ICEEditorLayout.ButtonCheck( "DETACH", "Detaches the effect instance and will create further ones acording to the given interval" , _effect.Detach, ICEEditorStyle.ButtonMiddle ); 
						EditorGUI.EndDisabledGroup();
					ICEEditorLayout.EndHorizontal();
					EditorGUI.BeginDisabledGroup( _effect.ReferenceObject == null );
						EditorGUI.indentLevel++;
							if( _effect.Detach )
								ICEEditorLayout.MinMaxGroupSimple( "Interval (secs.)", "", ref _effect.IntervalMin, ref _effect.IntervalMax, 0, ref _effect.MaxInterval, 0.25f, 40, "" );
							else
								_effect.ParentName = ICEEditorLayout.TransformPopup( "Parent", "", _effect.ParentName, _control.transform, false );
						EditorGUI.indentLevel--;
						_effect.OffsetType = (RandomOffsetType)ICEEditorLayout.EnumPopup( "Offset Type","", _effect.OffsetType );

						EditorGUI.indentLevel++;
							if( _effect.OffsetType == RandomOffsetType.EXACT )
								_effect.Offset = EditorGUILayout.Vector3Field( "Offset", _effect.Offset );
							else 
								_effect.OffsetRadius = ICEEditorLayout.Slider( "Offset Radius", "", _effect.OffsetRadius, 0.25f, 0, 100 );
						EditorGUI.indentLevel--;
					EditorGUI.EndDisabledGroup();
				EditorGUI.indentLevel--;

			}
			
			return _effect;
		}
		
		public static AudioDataObject DrawSharedAudio( AudioDataObject _audio, string _help )
		{
			ICEEditorLayout.BeginHorizontal();
			
			_audio.Enabled = ICEEditorLayout.ToggleLeft("Audio", "", _audio.Enabled, true );



			/*
			if( _audio.Enabled )
			{
				if (GUILayout.Button("SAVE", ICEEditorStyle.ButtonMiddle ))
					CreatureIO.SaveAudioContainerToFile( _audio, m_creature_control.gameObject.name );
				
				if (GUILayout.Button("LOAD", ICEEditorStyle.ButtonMiddle ))
					_audio = CreatureIO.LoadAudioContainernFromFile( _audio );
			}*/
			ICEEditorLayout.EndHorizontal( _help );

			if( _audio.Enabled )
			{
				EditorGUI.indentLevel++;

				EditorGUILayout.Separator();
				
				for( int i = 0 ; i < _audio.Clips.Count ; i++ )
				{
					ICEEditorLayout.BeginHorizontal();
					
						_audio.Clips[i] = (AudioClip)EditorGUILayout.ObjectField("Audio Clip #" + (int)(i+1), _audio.Clips[i], typeof(AudioClip), false);
						
						if ( GUILayout.Button("x", ICEEditorStyle.CMDButton ) )
						{
							_audio.DeleteClip( i );
							--i;
						}
					ICEEditorLayout.EndHorizontal( Info.BEHAVIOUR_AUDIO );
				}
				
				
				ICEEditorLayout.BeginHorizontal();
				
				_audio.AddClip( (AudioClip)EditorGUILayout.ObjectField("New Audio Clip", null, typeof(AudioClip), false) );
				
				if ( GUILayout.Button("ADD", ICEEditorStyle.ButtonMiddle ) )
					_audio.AddClip();
				
				
				ICEEditorLayout.EndHorizontal();
				
				
				
				EditorGUILayout.Separator();
				
				_audio.Volume = ICEEditorLayout.DefaultSlider( "Volume", "The volume of the sound at the MinDistance",_audio.Volume, 0.05f, 0, 1, 0.5f );
				
				EditorGUILayout.Separator();
				
				_audio.MinPitch = ICEEditorLayout.DefaultSlider( "Min. Pitch", "Lowest value of the random pitch of the audio source.", _audio.MinPitch, 0.05f, -3, _audio.MaxPitch, 1 );
				_audio.MaxPitch = ICEEditorLayout.DefaultSlider( "Max. Pitch", "Highest value of the random pitch of the audio source.", _audio.MaxPitch, 0.05f, _audio.MinPitch, 3, 1.5f );
				
				EditorGUILayout.Separator();
				
				_audio.MinDistance = ICEEditorLayout.DefaultSlider( "Min. Distance", "Within the Min distance the AudioSource will cease to grow louder in volume. Outside the min distance the volume starts to attenuate.", _audio.MinDistance, 0.25f, 0, _audio.MaxDistance, 10 );
				_audio.MaxDistance = ICEEditorLayout.DefaultSlider( "Max. Distance", "Dependent to the RolloffMode, the MaxDistance is the distance where the sound is completely inaudible.", _audio.MaxDistance, 0.25f, _audio.MinDistance, 250, 50 );
				
				EditorGUILayout.Separator();
				
				_audio.Loop = ICEEditorLayout.Toggle("Loop","Is the audio clip looping?", _audio.Loop);
				_audio.RolloffMode = (AudioRolloffMode)ICEEditorLayout.EnumPopup("RolloffMode", "Rolloff modes that a 3D sound can have in an audio source.", _audio.RolloffMode );
				
				EditorGUI.indentLevel--;
				
				EditorGUILayout.Separator();
			}
			
			return _audio;
		}
		
		public static StatusContainer DrawSharedInfluences( ICECreatureControl _control, StatusContainer _influences, string _title, string _hint, string _help )
		{
			if( _title == "" )
				_title = "Influences";
			
			if( _hint == "" )
				_hint = "Influences";

			ICEEditorLayout.BeginHorizontal();
				EditorGUI.BeginDisabledGroup( _influences.Enabled == false );
					_influences.Foldout = ICEEditorLayout.Foldout( _influences.Foldout, _title, "", false );
				EditorGUI.EndDisabledGroup();
				_influences.Enabled = ICEEditorLayout.ButtonCheck( "ENABLED", "Enables/disables the influences", _influences.Enabled, ICEEditorStyle.ButtonMiddle );
			ICEEditorLayout.EndHorizontal( _help );

			return DrawShareInfluencesContent( _influences, _help, _control.Creature.Status.UseAdvanced );
		}

		public static StatusContainer DrawSharedInfluences( StatusContainer _influences, string _help = "", bool _advanced = true )
		{
			_influences.Foldout = true;
			_influences.Enabled = ICEEditorLayout.ToggleLeft( "Influences", "", _influences.Enabled, true, Info.STATUS_INFLUENCES );
			
			return DrawShareInfluencesContent( _influences, _help, _advanced );
		}

		public static StatusContainer DrawShareInfluencesContent( StatusContainer _influences, string _help = "", bool _advanced = true )
		{
			if( _influences.Enabled && _influences.Foldout )
			{
				EditorGUI.indentLevel++;

					string _advanced_help =  "";
					if( ! _advanced )
						_advanced_help += "\n\nPlease note: You can activate the Advanced Status to get further options!";
				
					_influences.Interval = ICEEditorLayout.DefaultSlider( "Interval (sec.)", "Interval in seconds", _influences.Interval, 0.0025f, 0, 60,0, Info.STATUS_INFLUENCES_INTERVAL + _advanced_help );		
					EditorGUILayout.Separator();

					_influences.Damage = ICEEditorLayout.DefaultSlider( "Damage (%)", "Damage influence in percent", _influences.Damage, 0.0025f, -100, 100,0, Info.STATUS_INFLUENCES_DAMAGE + _advanced_help );		
					
					if( _advanced )
					{
						_influences.Stress = ICEEditorLayout.DefaultSlider( "Stress (%)", "Stress influence in percent", _influences.Stress, 0.0025f, -100, 100,0, Info.STATUS_INFLUENCES_STRESS );				
						_influences.Debility = ICEEditorLayout.DefaultSlider( "Debility (%)", "Debility influence in percent", _influences.Debility, 0.0025f, -100, 100,0,Info.STATUS_INFLUENCES_DEBILITY );
						_influences.Hunger = ICEEditorLayout.DefaultSlider( "Hunger (%)", "Hunger influence in percent", _influences.Hunger, 0.0025f, -100, 100,0, Info.STATUS_INFLUENCES_HUNGER );				
						_influences.Thirst = ICEEditorLayout.DefaultSlider( "Thirst (%)", "Thirst influence in percent", _influences.Thirst, 0.0025f, -100, 100,0, Info.STATUS_INFLUENCES_THIRST );				

						EditorGUILayout.Separator();

						_influences.Aggressivity = ICEEditorLayout.DefaultSlider( "Aggressivity (%)", "Aggressivity influence in percent", _influences.Aggressivity, 0.0025f, -100, 100,0, Info.STATUS_INFLUENCES_AGGRESSIVITY );				
						_influences.Anxiety = ICEEditorLayout.DefaultSlider( "Anxiety (%)", "Anxiety influence in percent", _influences.Anxiety, 0.0025f, -100, 100,0,Info.STATUS_INFLUENCES_ANXIETY );
						_influences.Experience = ICEEditorLayout.DefaultSlider( "Experience (%)", "Experience influence in percent", _influences.Experience, 0.0025f, -100, 100,0, Info.STATUS_INFLUENCES_EXPERIENCE );				
						_influences.Nosiness = ICEEditorLayout.DefaultSlider( "Nosiness (%)", "Nosiness influence in percent", _influences.Nosiness, 0.0025f, -100, 100,0, Info.STATUS_INFLUENCES_NOSINESS );				

						EditorGUILayout.Separator();

						_influences.Odour = DrawOdourContainer( "Odour", "", _influences.Odour, "" );
					}
					
				EditorGUI.indentLevel--;
				
				EditorGUILayout.Separator();
			}
			
			return _influences;
		}
		/*
		public static string DrawTargetPopup( ICECreatureControl _control, string _title,string _hint, string _name, string _help = "" )
		{
			List<string> _targets = new List<string>();

			if( _control.Creature.Essentials.TargetReady() )
				_targets.Add( _control.Creature.Essentials.Target.TargetGameObject.name );
			if( _control.Creature.Missions.Outpost.Enabled && _control.Creature.Missions.Outpost.TargetReady() )
				_targets.Add( _control.Creature.Missions.Outpost.Target.TargetGameObject.name );
			if( _control.Creature.Missions.Escort.Enabled && _control.Creature.Missions.Escort.TargetReady() )
				_targets.Add( _control.Creature.Missions.Escort.Target.TargetGameObject.name );

			if( _control.Creature.Missions.Patrol.Enabled && _control.Creature.Missions.Patrol.TargetReady() )
			{
				foreach( WaypointObject _wp in _control.Creature.Missions.Patrol.Waypoints.GetValidWaypoints())
				{
					if( _wp.TargetGameObject != null )
						_targets.Add( _wp.TargetGameObject.name );
				}
			}

			GUIContent[] _options = new GUIContent[ _targets.Count + 1];
			int _selected = 0;
			
			_options[0] = new GUIContent( " ");
			for( int i = 0 ; i < _targets.Count ; i++ )
			{
				int _index = i + 1;

				if( _targets[i] != "" )
				{		
					_options[ _index ] = new GUIContent( _targets[i] );
					
					if( _targets[i] == _name )
						_selected = _index;
				}
				else
				{
					_options[ _index ] = new GUIContent( "ERROR" );
				}
			}

			_selected = ICEEditorLayout.Popup( _title, _hint, _selected , _options, _help  );
			
			return _options[ _selected ].text;
		}*/

		public static LogicalOperatorType LogicalOperatorPopup( LogicalOperatorType _selected, params GUILayoutOption[] _options )
		{

			string[] _values = new string[6];
			_values[(int)LogicalOperatorType.EQUAL ] = "==";
			_values[(int)LogicalOperatorType.NOT ] = "!=";
			_values[(int)LogicalOperatorType.LESS ] = "<";
			_values[(int)LogicalOperatorType.LESS_OR_EQUAL ] = "<=";
			_values[(int)LogicalOperatorType.GREATER ] = ">";
			_values[(int)LogicalOperatorType.GREATER_OR_EQUAL ] = ">=";

			return (LogicalOperatorType)EditorGUILayout.Popup( (int)_selected, _values, _options ); 
		}

		public static LogicalOperatorType OperatorPopup( LogicalOperatorType _selected, params GUILayoutOption[] _options )
		{
			
			string[] _values = new string[2];
			_values[(int)LogicalOperatorType.EQUAL ] = "IS";
			_values[(int)LogicalOperatorType.NOT ] = "NOT";

			return (LogicalOperatorType)EditorGUILayout.Popup( (int)_selected, _values, _options ); 
		}

		public static void DrawTargetSelectors( ICECreatureControl _control, TargetSelectorsObject _selectors, TargetType _type, float _min_distance , float _max_distance  )
		{
			// TARGET SELECTION CRITERIAS

			string _help = Info.TARGET_SELECTION_CRITERIA ;
			if( _type == TargetType.HOME )
				_help = Info.TARGET_SELECTION_CRITERIA + "\n\n" + Info.TARGET_SELECTION_CRITERIA_HOME;

			Color _color_true = Color.green;
			Color _color_false = (Application.isPlaying?Color.red:ICEEditorLayout.DefaultBackgroundColor);
			Color _color_unchecked = (Application.isPlaying?Color.yellow:ICEEditorLayout.DefaultBackgroundColor);

			ICEEditorLayout.BeginHorizontal();

				EditorGUI.BeginDisabledGroup( _type == TargetType.HOME && _selectors.UseSelectionCriteriaForHome == false );
				_selectors.Foldout = ICEEditorLayout.Foldout( _selectors.Foldout, "Target Selection Criteria" , "", false );
				_selectors.UseAdvanced = ICEEditorLayout.ButtonCheck( "ADVANCED", "Use advanced selector settings", _selectors.UseAdvanced, ICEEditorStyle.ButtonMiddle );
				EditorGUI.EndDisabledGroup();

				if( _type == TargetType.HOME )
					_selectors.UseSelectionCriteriaForHome = ICEEditorLayout.ButtonCheck( "ENABLE", "The HOME target should always have the lowest priority, but if you want you could adapt these settings also.", _selectors.UseSelectionCriteriaForHome, ICEEditorStyle.ButtonMiddle );

			ICEEditorLayout.EndHorizontal( _help );

			if( ! _selectors.Foldout )
				return;
	
			if( _type == TargetType.HOME && _selectors.UseSelectionCriteriaForHome == false )
			{
				_selectors.Priority = 0;
				_selectors.SelectionRange = 0;
				_selectors.SelectionAngle = 0;
				_selectors.UseAdvanced = false;
				return;
			}

			EditorGUI.indentLevel++;

				// PRIORITY BEGIN
				ICEEditorLayout.BeginHorizontal();				
					if( _selectors.CanUseDefaultPriority )
						_selectors.Priority = (int)ICEEditorLayout.AutoSlider( "Priority","", _selectors.Priority, 1,0, 100, ref _selectors.UseDefaultPriority, _selectors.DefaultPriority );
					else
						_selectors.Priority = (int)ICEEditorLayout.DefaultSlider( "Priority", "Priority to select this target!", _selectors.Priority, 1, 0, 100, _selectors.GetDefaultPriorityByType( _type ) );
				ICEEditorLayout.EndHorizontal( Info.TARGET_SELECTION_CRITERIA_PRIORITY );
				// PRIORITY END

				string _range_title = "Selection Range";
				if( _selectors.SelectionRange == 0 )
					_range_title += " (infinite)";
				else
					_range_title += " (limited)";

				// SELECTION RANGE BEGIN
				ICEEditorLayout.BeginHorizontal();					
					GUI.backgroundColor = (_selectors.Status == SelectionStatus.UNCHECKED?_color_unchecked:(_selectors.IsValid?_color_true:_color_false) );
						_selectors.SelectionRange = ICEEditorLayout.DefaultSlider( _range_title , "If the selection range greater than 0 this target will only select if the creature is within the specified range", _selectors.SelectionRange, Init.SELECTION_RANGE_STEP, _min_distance, _max_distance, _selectors.GetDefaultRangeByType( _type ) );
						_selectors.UseFieldOfView = ICEEditorLayout.ButtonCheck( "FOV", "Field Of View - the target must be in the field of view", _selectors.UseFieldOfView , ICEEditorStyle.CMDButtonDouble );
						_selectors.UseVisibilityCheck = ICEEditorLayout.ButtonCheck( "VC", "Visibility Check - the target must be visible for the creature", _selectors.UseVisibilityCheck , ICEEditorStyle.CMDButtonDouble );
					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
				ICEEditorLayout.EndHorizontal( Info.TARGET_SELECTION_CRITERIA_RANGE );
				// SELECTION RANGE END

				if( _selectors.SelectionRange > 0 )
				{
						string _angle_title = "Selection Angle";
						if( _selectors.SelectionAngle == 0 || _selectors.SelectionAngle == 180 )
							_angle_title += " (full-circle)";
						else if( _selectors.SelectionAngle == 90 )
							_angle_title += " (semi-circle)";
						else if( _selectors.SelectionAngle == 45 )
							_angle_title += " (quadrant)";
						else
							_angle_title += " (sector)";

						// SELECTION ANGLE BEGIN
						ICEEditorLayout.BeginHorizontal();	
							GUI.backgroundColor = (_selectors.Status == SelectionStatus.UNCHECKED?_color_unchecked:(_selectors.IsValid?_color_true:_color_false) );

								_selectors.SelectionAngle = ICEEditorLayout.DefaultSlider( _angle_title , "", _selectors.SelectionAngle * 2, Init.SELECTION_ANGLE_STEP, Init.SELECTION_ANGLE_MIN, Init.SELECTION_ANGLE_MAX, 0 ) / 2;
					

								if (GUILayout.Button( new GUIContent( "90", "" ) , ICEEditorStyle.CMDButtonDouble ) )
									_selectors.SelectionAngle = 45;

								if (GUILayout.Button( new GUIContent( "180", "" ) , ICEEditorStyle.CMDButtonDouble ) )
									_selectors.SelectionAngle = 90;

								if (GUILayout.Button( new GUIContent( "360", "" ) , ICEEditorStyle.CMDButtonDouble ) )
									_selectors.SelectionAngle = 180;

							GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

						ICEEditorLayout.EndHorizontal( Info.TARGET_SELECTION_CRITERIA_ANGLE );
				}
				else
				{
					EditorGUI.indentLevel++;
						ICEEditorLayout.BeginHorizontal();
						GUILayout.FlexibleSpace();

						EditorGUILayout.LabelField( new GUIContent( "Selection Range adjusted to zero - no regional selection restriction!", "" ), EditorStyles.wordWrappedMiniLabel );

						GUILayout.FlexibleSpace();
						ICEEditorLayout.EndHorizontal();
					EditorGUI.indentLevel--;
				}
				// SELECTION ANGLE END

				if( _selectors.UseAdvanced )
				{
					// SELECTOR GROUPS BEGIN
					foreach( TargetSelectorObject _selector in _selectors.Selectors )
					{
						for( int i = 0 ; i < _selector.Conditions.Count ; i++ )
						{
							TargetSelectorConditionObject _condition = _selector.Conditions[i];

							var indent = EditorGUI.indentLevel;
							EditorGUI.indentLevel = 0;
							
							
							// CONDITION LINE BEGIN
							ICEEditorLayout.BeginHorizontal();
									
								int _offset = 30;
								int _condition_size = 65;
								if( i > 0 ) 
								{
									_offset = 45;
									_condition_size = 50;
								}
		
								GUI.backgroundColor = (_condition.Status == SelectionStatus.UNCHECKED?_color_unchecked:(_condition.IsValid?_color_true:_color_false) );

								EditorGUILayout.LabelField("",GUILayout.Width( _offset ) );
								_condition.ConditionType = (ConditionalOperatorType)EditorGUILayout.EnumPopup(  _condition.ConditionType , GUILayout.Width( _condition_size ) );
								_condition.ExpressionType = (TargetSelectorExpressionType)EditorGUILayout.EnumPopup( _condition.ExpressionType , GUILayout.MinWidth( 120 ), GUILayout.MaxWidth( 220 ) );
								
								if( TargetSelectorExpression.NeedLogicalOperator( _condition.ExpressionType ) )	
									_condition.Operator = LogicalOperatorPopup(  _condition.Operator, GUILayout.Width( 45 ) );
								else
									_condition.Operator = OperatorPopup(  _condition.Operator, GUILayout.Width( 45 ) );
					


								// CREATURE BEHAVIOUR
								if( _condition.ExpressionType == TargetSelectorExpressionType.OwnBehaviour )								
									_condition.StringValue = EditorBehaviour.BehaviourPopup( _control, _condition.StringValue );

								// CREATURE BEHAVIOUR
								else if( _condition.ExpressionType == TargetSelectorExpressionType.ActiveTargetName ||
									_condition.ExpressionType == TargetSelectorExpressionType.TargetName ||
									_condition.ExpressionType == TargetSelectorExpressionType.ActiveTargetParentName ||
									_condition.ExpressionType == TargetSelectorExpressionType.TargetParentName )								
									_condition.StringValue = EditorSharedTools.TargetPopup( _condition.StringValue );
						
								// CREATURE POSITION
								else if( _condition.ExpressionType == TargetSelectorExpressionType.OwnPosition )								
									_condition.PositionType = (TargetSelectorPositionType)EditorGUILayout.EnumPopup( _condition.PositionType );

								// STRING VALUES
								else if( TargetSelectorExpression.IsStringValue( _condition.ExpressionType ) )
								{
									_condition.StringValue = EditorGUILayout.TextField( _condition.StringValue );
								}

								// ENUM VALUES
								else if( TargetSelectorExpression.IsEnumValue( _condition.ExpressionType ) )
								{
									if( _condition.ExpressionType == TargetSelectorExpressionType.TargetReferenceType )
										_condition.IntegerValue = (int)(RegisterReferenceType)EditorGUILayout.EnumPopup( (RegisterReferenceType)_condition.IntegerValue );
									else if( _condition.ExpressionType == TargetSelectorExpressionType.EnvironmentWeather )
										_condition.IntegerValue = (int)(WeatherType)EditorGUILayout.EnumPopup( (WeatherType)_condition.IntegerValue );
									else if( _condition.ExpressionType == TargetSelectorExpressionType.OwnOdour )
										_condition.IntegerValue = (int)(OdourType)EditorGUILayout.EnumPopup( (OdourType)_condition.IntegerValue );
									else if( _condition.ExpressionType == TargetSelectorExpressionType.CreatureOdour )
										_condition.IntegerValue = (int)(OdourType)EditorGUILayout.EnumPopup( (OdourType)_condition.IntegerValue );
									else if( _condition.ExpressionType == TargetSelectorExpressionType.MarkerOdour )
										_condition.IntegerValue = (int)(OdourType)EditorGUILayout.EnumPopup( (OdourType)_condition.IntegerValue );
								}

								// ENUM VALUES
								else if( TargetSelectorExpression.IsBooleanValue( _condition.ExpressionType ) )
								{
									BooleanValueType _boolean_value = (BooleanValueType)EditorGUILayout.EnumPopup( (_condition.BooleanValue?BooleanValueType.TRUE:BooleanValueType.FALSE) );
									
									_condition.BooleanValue = (_boolean_value == BooleanValueType.TRUE?true:false);
								}

								// KEYCODE VALUES
								else if( TargetSelectorExpression.IsKeyCodeValue( _condition.ExpressionType ) )
								{
									_condition.KeyCodeValue = (KeyCode)EditorGUILayout.EnumPopup( _condition.KeyCodeValue );
								}

								// LOGICAL VALUES
								else if( TargetSelectorExpression.NeedLogicalOperator( _condition.ExpressionType ) )	
								{
									if( TargetSelectorExpression.IsDynamicValue( _condition.ExpressionType ) )
									{
										if( _condition.UseDynamicValue )
											_condition.ExpressionValue = (TargetSelectorExpressionType)EditorGUILayout.EnumPopup( _condition.ExpressionValue  );
										else
											_condition.FloatValue = EditorGUILayout.FloatField( _condition.FloatValue );
										
										GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;	
										_condition.UseDynamicValue = ICEEditorLayout.ButtonCheck( "DYN", "Use dynamic value", _condition.UseDynamicValue, ICEEditorStyle.CMDButtonDouble );
									}
									else
									{
										_condition.UseDynamicValue = false;
										_condition.FloatValue = EditorGUILayout.FloatField( _condition.FloatValue );
									}
								}

								// OBJECT VALUES
								else if( TargetSelectorExpression.IsObjectValue( _condition.ExpressionType ) )
								{
									_condition.ExpressionValue = (TargetSelectorExpressionType)EditorGUILayout.EnumPopup( _condition.ExpressionValue  );
								}

								// PRECURSOR TYPE
								else if( _condition.ExpressionType == TargetSelectorExpressionType.PreviousTargetType )
									_condition.PrecursorTargetType = (TargetType)EditorGUILayout.EnumPopup( _condition.PrecursorTargetType ); 

								// PRECURSOR TAG
								else if( _condition.ExpressionType == TargetSelectorExpressionType.PreviousTargetTag )
									_condition.PrecursorTargetTag = EditorGUILayout.TagField( _condition.PrecursorTargetTag );

								// PRECURSOR NAME
								else if( _condition.ExpressionType == TargetSelectorExpressionType.PreviousTargetName )
									_condition.PrecursorTargetName = TargetPopup( _condition.PrecursorTargetName );

								GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
														
								if (GUILayout.Button( new GUIContent( "DEL", "Removes this condition" ) , ICEEditorStyle.CMDButtonDouble ) )
								{
									if( _selector.Conditions.Count > 1 )
										_selector.Conditions.Remove( _condition );
									else
										_selectors.Selectors.Remove( _selector );
									return;
								}

								


							ICEEditorLayout.EndHorizontal( Info.GetTargetSelectionExpressionTypeHint( _condition.ExpressionType ) );
							// CONDITION LINE END



							EditorGUI.indentLevel = indent;

						}

						// ADD CONDITION LINE BEGIN
						ICEEditorLayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						
						EditorGUILayout.LabelField( new GUIContent( "Add Condition ", "" ), EditorStyles.wordWrappedMiniLabel );
						
			
							//GUILayout.FlexibleSpace();
							if (GUILayout.Button( new GUIContent( "ADD", "Add condition" ) , ICEEditorStyle.CMDButtonDouble ) )
								_selector.Conditions.Add( new TargetSelectorConditionObject( ConditionalOperatorType.AND ) );

								if (GUILayout.Button( new GUIContent( "DEL", "Removes all conditions" ) , ICEEditorStyle.CMDButtonDouble ) )
								{
									_selectors.Selectors.Remove( _selector );
									return;
								}

						ICEEditorLayout.EndHorizontal();
					}
					ICEEditorLayout.BeginHorizontal();						
						GUILayout.FlexibleSpace();
						EditorGUILayout.LabelField( new GUIContent( "Add Condition Group ", "" ), EditorStyles.wordWrappedMiniLabel );

						if (GUILayout.Button( new GUIContent( "ADD", "" ) , ICEEditorStyle.CMDButtonDouble ) )
							_selectors.Selectors.Add( new TargetSelectorObject( ConditionalOperatorType.AND ) );
						
						if (GUILayout.Button( new GUIContent( "RESET", "Removes all groups and conditions" ) , ICEEditorStyle.CMDButtonDouble ) )
							_selectors.Selectors.Clear();
					ICEEditorLayout.EndHorizontal();	

					
				}

			EditorGUI.indentLevel--;

		}

		public static void DrawTargetSelectorStatementContent( ICECreatureControl _control, TargetSelectorStatementObject _statement )
		{
			if( _statement == null )
				return;
			

				string _prefix = "THEN";
				
				if( _statement.StatementType == TargetSelectorStatementType.NONE )
				{
					_statement.StatementType = (TargetSelectorStatementType)ICEEditorLayout.EnumPopup( _prefix, "Select the desired statement type", TargetSelectorStatementType.NONE );
				}
				else if( _statement.StatementType == TargetSelectorStatementType.PRIORITY )
				{					
					_statement.Priority = (int)ICEEditorLayout.Slider( _prefix + " Priority", "Priority to select this target!", _statement.Priority, 1, 0, 100 );
					
				}
				/*else if( _statement.StatementType == TargetSelectorStatementType.MULTIPLIER )
				{		
					_statement.Priority = (int)ICEEditorLayout.DefaultSlider()
				}*/
				else if( _statement.StatementType == TargetSelectorStatementType.SUCCESSOR )
				{											
					if( _statement.SuccessorType == TargetSuccessorType.TYPE )
						_statement.SuccessorTargetType = (TargetType)ICEEditorLayout.EnumPopup( _prefix + " Successor Type", "This target will only select if the active target has the specified type.", _statement.SuccessorTargetType ); 
					else if( _statement.SuccessorType == TargetSuccessorType.TAG )
						_statement.SuccessorTargetTag = EditorGUILayout.TagField( _prefix + " Successor Tag", _statement.SuccessorTargetTag );
					else if( _statement.SuccessorType == TargetSuccessorType.NAME )
						_statement.SuccessorTargetName = TargetPopup( _prefix + " Successor Name", "This target will only select if the active target has the specified name.", _statement.SuccessorTargetName, "" );
					
					if (GUILayout.Button(  new GUIContent( _statement.SuccessorType.ToString(), "Select precursor type" ), ICEEditorStyle.CMDButtonDouble ) )
					{
						if( _statement.SuccessorType == TargetSuccessorType.TAG )
							_statement.SuccessorType = 0;
						else
							_statement.SuccessorType++;
					}
				}
				
		}

		public static void DrawTargetSelectorStatement( ICECreatureControl _control, List<TargetSelectorStatementObject> _statements )
		{
			if( _statements == null )
				return;

			for( int i = 0 ; i < _statements.Count ; i++ )
			{
				if( i > 0 ) EditorGUI.indentLevel++;
				
				ICEEditorLayout.BeginHorizontal();
				
				TargetSelectorStatementObject _statement = _statements[i];
				
				//string _prefix = "THEN";

				DrawTargetSelectorStatementContent( _control, _statement );
				/*
				if( _statement.StatementType == TargetSelectorStatementType.NONE )
				{
					_statement.StatementType = (TargetSelectorStatementType)ICEEditorLayout.EnumPopup( _prefix, "Select the desired statement type", TargetSelectorStatementType.NONE );
				}
				else if( _statement.StatementType == TargetSelectorStatementType.PRIORITY )
				{					
					_statement.Priority = (int)ICEEditorLayout.DefaultSlider( _prefix + " Priority", "Priority to select this target!", _statement.Priority, 1, 0, 100, 0 );

				}
				else if( _statement.StatementType == TargetSelectorStatementType.MULTIPLIER )
				{		
					_statement.Priority = (int)ICEEditorLayout.DefaultSlider()
				}
				else if( _statement.StatementType == TargetSelectorStatementType.SUCCESSOR )
				{											
					if( _statement.SuccessorType == TargetSuccessorType.TYPE )
						_statement.SuccessorTargetType = (TargetType)ICEEditorLayout.EnumPopup( _prefix + " Successor Type", "This target will only select if the active target has the specified type.", _statement.SuccessorTargetType ); 
					else if( _statement.SuccessorType == TargetSuccessorType.TAG )
						_statement.SuccessorTargetTag = EditorGUILayout.TagField( _prefix + " Successor Tag", _statement.SuccessorTargetTag );
					else if( _statement.SuccessorType == TargetSuccessorType.NAME )
						_statement.SuccessorTargetName = DrawTargetPopup( _control, _prefix + " Successor Name", "This target will only select if the active target has the specified name.", _statement.SuccessorTargetName );
					
					if (GUILayout.Button(  new GUIContent( _statement.SuccessorType.ToString(), "Select precursor type" ), ICEEditorStyle.CMDButtonDouble ) )
					{
						if( _statement.SuccessorType == TargetSuccessorType.TAG )
							_statement.SuccessorType = 0;
						else
							_statement.SuccessorType++;
					}
				}*/
				
				if (GUILayout.Button( new GUIContent( "DEL", "Removes selected statement" ) , ICEEditorStyle.CMDButtonDouble ) )
				{
					_statements.Remove( _statement );
					return;
				}
				
				
				ICEEditorLayout.EndHorizontal();
				
				if( i > 0 ) EditorGUI.indentLevel--;
			}

		}


		public static void DrawMove( ref float _segment, ref float _max_segment, ref float _stop , ref float _directional_variance, ref float _deviation, ref float _max_deviation, ref float _deviation_variance, ref bool _level, string _help = "" )
		{
			if( _help == "" )
				Info.Help ( Info.MOVE );
			else
				Info.Help ( Info.MOVE + "\n\n" + _help );

			if( _max_segment == 0 )
				_max_segment = Init.MOVE_MAX_DISTANCE;

			if( _max_deviation == 0 )
				_max_deviation = Init.MOVE_MAX_DISTANCE;

			_segment = ICEEditorLayout.MaxDefaultSlider( "Move Segment Length", "Subdivides the main path in segments of unitary length", _segment, Init.MOVE_DISTANCE_STEP, Init.MOVE_MIN_DISTANCE, ref _max_segment , Init.MOVE_DISTANCE_DEFAULT, Info.MOVE_SEGMENT_LENGTH );

			if( _segment > 0 )
			{
				EditorGUI.indentLevel++;
					_directional_variance = ICEEditorLayout.DefaultSlider( "Segment Variance Multiplier", "Generates randomized deviations along the path.", _directional_variance, Init.MOVE_INTERFERENCE_STEP, Init.MOVE_INTERFERENCE_MIN, Init.MOVE_INTERFERENCE_MAX, Init.MOVE_INTERFERENCE_DEFAULT, Info.MOVE_SEGMENT_VARIANCE);
				EditorGUI.indentLevel--;
			
				_deviation = ICEEditorLayout.MaxDefaultSlider( "Move Deviation Length", "Maximum value for the lateral deviation", _deviation, Init.MOVE_DISTANCE_STEP, Init.MOVE_MIN_DISTANCE, ref _max_deviation , Init.MOVE_DISTANCE_DEFAULT, Info.MOVE_SEGMENT_LENGTH );
				EditorGUI.indentLevel++;
					_deviation_variance = ICEEditorLayout.DefaultSlider( "Lateral Variance Multiplier", "Generates randomized deviations along the path.", _deviation_variance, Init.MOVE_INTERFERENCE_STEP, Init.MOVE_INTERFERENCE_MIN, Init.MOVE_INTERFERENCE_MAX, Init.MOVE_INTERFERENCE_DEFAULT,  Info.MOVE_LATERAL_VARIANCE );
				EditorGUI.indentLevel--;
	
				ICEEditorLayout.BeginHorizontal();
				_stop = ICEEditorLayout.DefaultSlider( "Move Stopping Distance (" + (_level?"circular":"spherical") + ")", "Stop within this distance from the target move position.", _stop, Init.STOP_DISTANCE_STEP, Init.STOP_MIN_DISTANCE, Init.STOP_MAX_DISTANCE, Init.STOP_DISTANCE_DEFAULT );

				_level = ! ICEEditorLayout.ButtonCheck( "3D", "Provides linear distances without consideration of level differences.", ! _level , ICEEditorStyle.CMDButtonDouble );
			/*	EditorGUI.indentLevel++;
					_level = ICEEditorLayout.Toggle( "Move Ignore Level Differences", "Provides linear distances without consideration of level differences.", _level, Info.MOVE_IGNORE_LEVEL_DIFFERENCE );
				EditorGUI.indentLevel--;*/
				ICEEditorLayout.EndHorizontal( Info.MOVE_STOPPING_DISTANCE );
			}
		}

		public static void DrawTargetObjectBlind( TargetObject _target, string _title = "", string _help = "" )
		{
			ICEEditorLayout.BeginHorizontal();

			_target.IsPrefab = ICEEditorTools.IsPrefab( _target.TargetGameObject );

			string _target_title = "Target Object " + (_target.IsValid?(_target.IsPrefab?"(prefab)":"(scene)"):"(null)");
			
			EditorGUILayout.PrefixLabel( _target_title );

			if( _target.Active )
				GUI.backgroundColor = Color.green;
			else if( _target.IsValid == false )
				GUI.backgroundColor = Color.red;

			ICEEditorLayout.ButtonShowObject(  _target.TargetGameObject, "SHOW " + _target.TargetName.ToUpper() + " (" + _target.TargetID + ")", ICEEditorStyle.ButtonFlex ); 

			//GUILayout.Button( new GUIContent( _target.TargetName ,"" ) );
			//if( ICEEditorLayout.Button( _target.TargetName ,"", ICEEditorStyle.ButtonFlex ) )
		

			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

			if( _target.TargetGameObject != null )
			{
				//ICEEditorLayout.ButtonShowObject(  _target.TargetGameObject.transform.position); 
				ICEEditorLayout.ButtonSelectObject( _target.TargetGameObject );
			}

			ICEEditorLayout.EndHorizontal( Info.INTERACTION_INTERACTOR_RULE_TARGET );

		}

		public static void BasicSpawnPointObject( SpawnPointObject _point, string _title, string _help = "" )
		{
			if( _point == null )
				return;

			_point.IsPrefab = ICEEditorTools.IsPrefab( _point.SpawnPointGameObject );

			if( _point.SpawnPointGameObject == null )
				GUI.backgroundColor = Color.red;

			string _target_title = "SpawnPoint " + (_point.IsValid?(_point.IsPrefab?"(prefab)":"(scene)"):"(null)");

			if( _point.AccessType == TargetAccessType.OBJECT )
			{
				_point.SpawnPointGameObject = (GameObject)EditorGUILayout.ObjectField( _target_title , _point.SpawnPointGameObject, typeof(GameObject), true);
			}
			else if( _point.AccessType == TargetAccessType.TAG )
			{
				_point.SpawnPointTag = EditorGUILayout.TagField( new GUIContent( _target_title, "" ), _point.SpawnPointTag );

				if( _point.SpawnPointTag != "" )
					_point.SpawnPointGameObject = ICECreatureRegister.Instance.GetReferenceGameObjectByTag( _point.SpawnPointTag );
				else
					_point.SpawnPointGameObject = null;
			}
			else
			{
				_point.SpawnPointName = TargetPopup( _target_title, "", _point.SpawnPointName, "" );

				if( _point.SpawnPointName != "" )
					_point.SpawnPointGameObject = ICECreatureRegister.Instance.GetReferenceGameObjectByName( _point.SpawnPointName );
				else
					_point.SpawnPointGameObject = null;
			}

			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

			// Type Enum Popup
			int _indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			_point.AccessType = (TargetAccessType)EditorGUILayout.EnumPopup( _point.AccessType, ICEEditorStyle.Popup, GUILayout.Width( 50 ) ); 
			EditorGUI.indentLevel = _indent;


			if( _point.SpawnPointGameObject != null )
			{
				ICEEditorLayout.ButtonShowObject( _point.SpawnPointGameObject.transform.position );
				ICEEditorLayout.ButtonSelectObject( _point.SpawnPointGameObject );
			}
		}

		public static void DrawTargetObject( ICECreatureControl _control, TargetObject _target, string _title, string _help = "" )
		{
			if( _control == null || _target == null )
				return;



			// PREPARATION BEGIN
/*
			if( _target.TargetGameObject != null && _target.TargetName == "" && _target.TargetTag == "" )
			{
				if( _target.TargetName != _target.TargetGameObject.name )
					_target.TargetName = _target.TargetGameObject.name;
				
				if( _target.TargetTag != _target.TargetGameObject.tag )
					_target.TargetTag = _target.TargetGameObject.tag;
				
				if( _target.Type == TargetType.HOME || 
				   _target.Type == TargetType.PATROL ||
				   _target.Type == TargetType.OUTPOST )
					ICECreatureRegister.Register.AddReferenceLocation( _target.TargetGameObject );
				else if( _target.Type == TargetType.INTERACTOR ||
				        _target.Type == TargetType.ESCORT ) 
					ICECreatureRegister.Register.AddReferenceCreature( _target.TargetGameObject );
			}*/

			_target.IsPrefab = ICEEditorTools.IsPrefab( _target.TargetGameObject );

			// PREPARATION END

			// TARGET OBJECT LINE BEGIN
			ICEEditorLayout.BeginHorizontal();

				if( _target.TargetGameObject == null )
					GUI.backgroundColor = Color.red;
				else if( _target.Active )
					GUI.backgroundColor = Color.green;
				
				string _target_title = "Target Object " + (_target.IsValid?(_target.IsPrefab?"(prefab)":"(scene)"):"(null)");
				
				if( ! Application.isPlaying )
				{
					if( _target.AccessType == TargetAccessType.NAME )
						_target.SetTargetByName( TargetPopup( _target_title, "", _target.TargetName, "" ) );
					else if( _target.AccessType == TargetAccessType.TAG )
						_target.SetTargetByTag( EditorGUILayout.TagField( new GUIContent( _target_title, "" ), _target.TargetTag ) );
					else
						_target.SetTargetByGameObject( (GameObject)EditorGUILayout.ObjectField( _target_title , _target.TargetGameObject, typeof(GameObject), true) );
				
						
				}
	
				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

				// Type Enum Popup
				int _indent = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 0;
				_target.AccessType = (TargetAccessType)EditorGUILayout.EnumPopup( _target.AccessType, ICEEditorStyle.Popup, GUILayout.Width( 50 ) ); 
				EditorGUI.indentLevel = _indent;
			
			
				if( _target.TargetGameObject != null )
				{
					ICEEditorLayout.ButtonShowObject( _target.TargetGameObject.transform.position );
					ICEEditorLayout.ButtonSelectObject( _target.TargetGameObject );
					
					if( _target.TargetGameObject.GetComponent<ICECreatureTargetAttribute>() != null )
					{
						EditorGUI.BeginDisabledGroup( _target.ReadTargetAttributeData() == null );
							GUI.backgroundColor = Color.green;
							if(  ICEEditorLayout.Button( "D", "Use target based default values" , ICEEditorStyle.CMDButtonDouble ) )
								_target.SetTargetDefaultValues( _target.ReadTargetAttributeData() );
							GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
						EditorGUI.EndDisabledGroup();
					}
				}
				else 
				{
					GUI.backgroundColor = Color.green;
					if(  ICEEditorLayout.Button( "AUTO", "Creates an empty GameObject as target with default settings" , ICEEditorStyle.ButtonMiddle ) )
						EditorWizard.WizardRandomTarget( _control, _target );
					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
				}
			
			ICEEditorLayout.EndHorizontal( Info.TARGET_OBJECT );
			// TARGET OBJECT LINE END
		}

		/// <summary>
		/// Draws the target.
		/// </summary>
		/// <returns>The target.</returns>
		/// <param name="m_creature_control">M_creature_control.</param>
		/// <param name="_target">_target.</param>
		/// <param name="_help">_help.</param>
		public static void DrawTarget( ICECreatureControl _control, TargetObject _target, string _title, string _help = ""  )
		{
			EditorGUILayout.Separator();

			//_target.Foldout = ICEEditorLayout.InfoFoldout( _target.Foldout, _title, ref _target.ShowInfoText, ref _target.InfoText, _help, true );
			_target.InfoText = ICEEditorLayout.InfoLabel( _title , true, _target.SelectionPriority, ref _target.ShowInfoText, _target.InfoText, _help );
			EditorGUI.indentLevel++;
				if( Application.isPlaying )
					DrawTargetObjectBlind( _target, "", "" );
				else
					DrawTargetObject( _control, _target, "", "" );
				DrawTargetContent( _control, _target );
			EditorGUI.indentLevel--;


		}

		/// <param name="_help">_help.</param>
		public static void DrawTargetContent( ICECreatureControl _control, TargetObject _target )
		{
			EditorGUI.indentLevel++;	
				
				DrawTargetSelectors( _control, _target.Selectors, _target.Type, Init.SELECTION_RANGE_MIN, Init.SELECTION_RANGE_MAX );
				DrawTargetMoveSettings( _control.gameObject, _target );	

				_target.Influences = DrawSharedInfluences( _control, _target.Influences, "Target Influences", "", Info.TARGET_INFLUENCES );

			EditorGUI.indentLevel--;

			DrawTargetGroupMessage( _control, _target );
		}

		public static void DrawTargetGroupMessage( ICECreatureControl _control, TargetObject _target, string _title = "", string _help = "" )
		{
			if( _control == null || _target == null )
				return;

			ICEEditorLayout.BeginHorizontal();
				_target.GroupMessage.Type = (BroadcastMessageType)ICEEditorLayout.EnumPopup( "Group Message", "", _target.GroupMessage.Type );
			ICEEditorLayout.EndHorizontal( Info.TARGET_GROUP_MESSAGE );

			if( _target.GroupMessage.Type == BroadcastMessageType.COMMAND )
			{
				EditorGUI.indentLevel++;
				_target.GroupMessage.Command = ICEEditorLayout.Text( "Command", "Command", _target.GroupMessage.Command, Info.TARGET_GROUP_MESSAGE_COMMAND ).ToUpper();
				
				EditorGUI.indentLevel--;
			}
		}



		/// <summary>
		/// Draws the target.
		/// </summary>
		/// <returns>The target.</returns>
		/// <param name="m_creature_control">M_creature_control.</param>
		/// <param name="_target">_target.</param>
		/// <param name="_help">_help.</param>
		public static TargetObject DrawTargetMoveSettings( GameObject _game_object, TargetObject _target )
		{

			ICEEditorLayout.BeginHorizontal();
				EditorGUI.BeginDisabledGroup( _target.Move.Enabled == false );
					_target.Move.Foldout = ICEEditorLayout.Foldout( _target.Move.Foldout, "Target Move Specification", "", false );
				EditorGUI.EndDisabledGroup();


				InteractorRuleObject _rule = _target as InteractorRuleObject;
				if( _rule != null )
				{
					_rule.OverrideTargetMovePosition = ICEEditorLayout.ButtonCheck( "OVERRIDE", "Overrides initial target move specifications", _rule.OverrideTargetMovePosition, ICEEditorStyle.ButtonMiddle );

					_target.Move.Enabled = _rule.OverrideTargetMovePosition;

					if( ! _rule.OverrideTargetMovePosition )
						_target.Move.Foldout = false;
				}
				else
					_target.Move.Enabled = ICEEditorLayout.ButtonCheck( "ENABLED", "Use move specification settings", _target.Move.Enabled, ICEEditorStyle.ButtonMiddle );

			ICEEditorLayout.EndHorizontal( Info.TARGET_MOVE_SPECIFICATIONS );

			if( ! _target.Move.Foldout )
				return _target;

			EditorGUI.BeginDisabledGroup( _target.TargetGameObject == null || _target.Move.Enabled == false );
			EditorGUI.indentLevel++;
				Vector3 _offset = ICEEditorLayout.OffsetGroup( "Offset", _target.Move.Offset, _game_object, _target.TargetGameObject, 0.5f, 25, Info.TARGET_MOVE_SPECIFICATIONS_OFFSET );

				if( _offset != _target.Move.Offset || ( _offset != Vector3.zero && _target.OffsetDistance == 0 ) || ( _offset == Vector3.zero && _target.OffsetDistance != 0 ) )
					_target.UpdateOffset( _offset );
				
				EditorGUI.indentLevel++;
				
				ICEEditorLayout.BeginHorizontal();
					EditorGUI.BeginDisabledGroup( _target.Move.UseDynamicOffsetDistance );
						float _distance = ICEEditorLayout.DefaultSlider( "Distance", "",_target.OffsetDistance, Init.TARGET_OFFSET_DISTANCE_STEP, Init.TARGET_OFFSET_DISTANCE_MIN, Init.TARGET_OFFSET_DISTANCE_MAX, Init.TARGET_OFFSET_DISTANCE_DEFAULT );
					EditorGUI.EndDisabledGroup();

					_target.Move.UseDynamicOffsetDistance = ICEEditorLayout.ButtonCheck( "DYN", "Dynamic Offset Distance", _target.Move.UseDynamicOffsetDistance, ICEEditorStyle.CMDButtonDouble );
				
					EditorGUI.BeginDisabledGroup( _target.Move.UseDynamicOffsetDistance == false );		
						_target.Move.UseRandomOffsetDistance = ICEEditorLayout.ButtonCheck( "RND", "Random Offset Distance", _target.Move.UseRandomOffsetDistance, ICEEditorStyle.CMDButtonDouble );
					EditorGUI.EndDisabledGroup();

				ICEEditorLayout.EndHorizontal( Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_DISTANCE  );
				if( _target.Move.UseDynamicOffsetDistance )
				{
					EditorGUI.indentLevel++;
						float _min = ICEEditorLayout.DefaultSlider( "Min", "", _target.Move.MinOffsetDistance, Init.TARGET_OFFSET_DISTANCE_STEP, Init.TARGET_OFFSET_DISTANCE_MIN, _target.Move.MaxOffsetDistance, Init.TARGET_OFFSET_DISTANCE_DEFAULT, Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_DISTANCE );					
						float _max = ICEEditorLayout.DefaultSlider( "Max", "", _target.Move.MaxOffsetDistance, Init.TARGET_OFFSET_DISTANCE_STEP, _target.Move.MinOffsetDistance, Init.TARGET_OFFSET_DISTANCE_MAX, Init.TARGET_OFFSET_DISTANCE_DEFAULT, Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_DISTANCE );					
						_target.Move.DynamicOffsetDistanceUpdateSpeed = ICEEditorLayout.DefaultSlider( "Speed", "", _target.Move.DynamicOffsetDistanceUpdateSpeed,  0.01f, 0, 36, 0, Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_ANGLE );					
					EditorGUI.indentLevel--;

					if( _target.Move.MinOffsetDistance != _min || _target.Move.MaxOffsetDistance != _max )
						_distance = Random.Range( _min, _max);

					_target.Move.MinOffsetDistance = _min;
					_target.Move.MaxOffsetDistance = _max;
				}
				
				EditorGUI.BeginDisabledGroup( _distance == 0 && _target.Move.UseRandomOffsetDistance == false );

				ICEEditorLayout.BeginHorizontal();
					EditorGUI.BeginDisabledGroup( _target.Move.UseDynamicOffsetAngle );
						float _angle = ICEEditorLayout.DefaultSlider( "Angle", "", _target.OffsetAngle, 0.01f, 0, 360, 0 );					
					EditorGUI.EndDisabledGroup();
					_target.Move.UseDynamicOffsetAngle = ICEEditorLayout.ButtonCheck( "DYN", "Dynamic Offset Angle", _target.Move.UseDynamicOffsetAngle, ICEEditorStyle.CMDButtonDouble );
						
					EditorGUI.BeginDisabledGroup( _target.Move.UseDynamicOffsetAngle == false );		
					_target.Move.UseRandomOffsetAngle = ICEEditorLayout.ButtonCheck( "RND", "Random Offset Angle", _target.Move.UseRandomOffsetAngle, ICEEditorStyle.CMDButtonDouble );
					EditorGUI.EndDisabledGroup();
				ICEEditorLayout.EndHorizontal( Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_ANGLE );

				if( _target.Move.UseRandomOffsetAngle || _target.Move.UseDynamicOffsetAngle )
				{
					EditorGUI.indentLevel++;
						float _min = ICEEditorLayout.DefaultSlider( "Min", "", _target.Move.MinOffsetAngle, 1, 0, _target.Move.MaxOffsetAngle, 0, Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_ANGLE );					
						float _max = ICEEditorLayout.DefaultSlider( "Max", "", _target.Move.MaxOffsetAngle,  1, _target.Move.MinOffsetAngle, 360, 0, Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_ANGLE );					
						_target.Move.DynamicOffsetAngleUpdateSpeed = ICEEditorLayout.DefaultSlider( "Speed", "", _target.Move.DynamicOffsetAngleUpdateSpeed,  0.01f, 0, 36, 0, Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_ANGLE );					

					EditorGUI.indentLevel--;
					
					if( _target.Move.MinOffsetAngle != _min || _target.Move.MaxOffsetAngle != _max )
						_angle = Random.Range( _min, _max);
					
					_target.Move.MinOffsetAngle = _min;
					_target.Move.MaxOffsetAngle = _max;
				}

					if( _distance != _target.OffsetDistance || _angle != _target.OffsetAngle )
						_target.UpdateOffset( _angle, _distance );	


				EditorGUI.EndDisabledGroup();
				EditorGUI.indentLevel--;

				EditorGUILayout.Separator();
				ICEEditorLayout.BeginHorizontal();
					_target.Move.StopDistance = ICEEditorLayout.BasicDefaultSlider( "Stopping Distance (" + (_target.Move.IgnoreLevelDifference?"circular":"spherical") + ")","Stop within this distance from the target move position.", _target.Move.StopDistance, Init.TARGET_STOP_DISTANCE_STEP, Init.TARGET_STOP_MIN_DISTANCE, Init.TARGET_STOP_MAX_DISTANCE, Init.TARGET_STOP_DISTANCE_DEFAULT );
					_target.Move.IgnoreLevelDifference = ! ICEEditorLayout.ButtonCheck( "3D", "Ignore Level Differences - provides linear distances without consideration of level differences.", ! _target.Move.IgnoreLevelDifference , ICEEditorStyle.CMDButtonDouble );
					_target.Move.StopDistanceZoneRestricted = ICEEditorLayout.ButtonCheck( "BAN", "Use Stopping Distance as restricted zone", _target.Move.StopDistanceZoneRestricted , ICEEditorStyle.CMDButtonDouble );
				ICEEditorLayout.EndHorizontal( Info.TARGET_MOVE_SPECIFICATIONS_STOP_DISTANCE );

				_target.Move.SmoothingMultiplier = ICEEditorLayout.DefaultSlider( "Smoothing Multiplier", "Smoothing affects step-size and update speed of the TargetMovePosition.", _target.Move.SmoothingMultiplier, 0.01f, 0, 1, 0.5f, Info.TARGET_MOVE_SPECIFICATIONS_SMOOTHING );

				ICEEditorLayout.BeginHorizontal();
					_target.UpdateRandomRange( ICEEditorLayout.MaxDefaultSlider( "Random Positioning Range", "", _target.Move.RandomRange, Init.TARGET_RANDOM_RANGE_STEP, Init.TARGET_RANDOM_RANGE_MIN, ref _target.Move.MaxRandomRange, Init.TARGET_RANDOM_RANGE_DEFAULT ) );
					if( ICEEditorLayout.Button( "RND", "", ICEEditorStyle.CMDButtonDouble ) )
				   		_target.UpdateRandomRange( Random.Range( 10, _target.Move.MaxRandomRange ) );
				ICEEditorLayout.EndHorizontal( Info.TARGET_MOVE_SPECIFICATIONS_RANDOM_RANGE );

				if( _target.Move.RandomRange > 0 )
				{
					EditorGUI.indentLevel++;
						ICEEditorLayout.BeginHorizontal();
							EditorGUILayout.PrefixLabel( new GUIContent( "Update Position on ... ", "" ) );
							_target.Move.UseUpdateOffsetOnActivateTarget = ICEEditorLayout.ButtonCheck( "ACTIVATE", "Update on activate target", _target.Move.UseUpdateOffsetOnActivateTarget, ICEEditorStyle.ButtonFlex );
							_target.Move.UseUpdateOffsetOnMovePositionReached = ICEEditorLayout.ButtonCheck( "REACHED", "Update on reached MovePosition", _target.Move.UseUpdateOffsetOnMovePositionReached, ICEEditorStyle.ButtonFlex );
							_target.Move.UseUpdateOffsetOnRandomizedTimer = ICEEditorLayout.ButtonCheck( "TIMER", "Update on timer interval", _target.Move.UseUpdateOffsetOnRandomizedTimer, ICEEditorStyle.ButtonFlex );
						ICEEditorLayout.EndHorizontal( Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_UPDATE );
						EditorGUI.indentLevel++;
						if( _target.Move.UseUpdateOffsetOnRandomizedTimer == true )
							ICEEditorLayout.MinMaxGroup( "Min/Max Interval", "", ref _target.Move.OffsetUpdateTimeMin, ref _target.Move.OffsetUpdateTimeMax, 0, 360, 0.25f, "" );
						EditorGUI.indentLevel--;
					EditorGUI.indentLevel--;
				}

				EditorGUILayout.Separator();
				




			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();

			return _target;
		}
	}
}