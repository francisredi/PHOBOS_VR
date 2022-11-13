// ##############################################################################
//
// ICECreatureControlMenu.cs
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

using UnityEditor;
using UnityEngine;

using ICE;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Creatures.Attributes;
using ICE.Creatures.Extensions;
using ICE.Styles;
using ICE.Layouts;
using ICE.Creatures.EditorInfos;
using ICE.Creatures.Windows;
using ICE.Environment;

namespace ICE.Creatures.Menus
{
	public class ICECreatureControlMenu : MonoBehaviour {



		// REGISTER
		[MenuItem ( "Window/ICECreatureControl/Components/Add Creature Register", false, 1001 )]
		static void AddCreatureRegister() 
		{
			ICECreatureRegister _register = ICECreatureRegister.Instance;

			if( _register == null )
			{
				GameObject _object = new GameObject();
				_register = _object.AddComponent<ICECreatureRegister>();
				_object.name = "CreatureRegister";
				
				if( _register != null )
					_register.UpdateAllReferences();
			}
		}
			
		[MenuItem ( "Window/ICECreatureControl/Components/Add Creature Register", true)]
		static bool ValidateAddCreatureRegister() {
			if( ICECreatureRegister.Instance == null )
				return true;
			else
				return false;
		}

		// ENVIRONMENT
		[MenuItem ( "Window/ICECreatureControl/Components/Add Environment Controller", false, 1001 )]
		static void AddEnvironmentController() 
		{
			ICEEnvironmentController _environment = ICEEnvironmentController.Environment;
			
			if( _environment == null )
			{
				GameObject _object = new GameObject();
				_environment = _object.AddComponent<ICEEnvironmentController>();
				_object.name = "EnvironmentController";
			}
		}
	
		[MenuItem ( "Window/ICECreatureControl/Components/Add Environment Controller", true)]
		static bool ValidateAddEnvironmentController() {
			if( ICEEnvironmentController.Environment == null )
				return true;
			else
				return false;
		}

		// CRETAURES AND MORE
		[MenuItem ( "Window/ICECreatureControl/Components/Add Creature Component", false, 1101 )]
		static void AddCreatureControl() 
		{
			GameObject _object = Selection.activeObject as GameObject;

			if( _object != null && _object.GetComponent<ICECreatureControl>() == null )
				_object.AddComponent<ICECreatureControl>();
		}

		[MenuItem ( "Window/ICECreatureControl/Components/Add Creature Component", true)]
		static bool ValidateAddCreatureControl() {
			return ValidateObject();
		}

		[MenuItem ( "Window/ICECreatureControl/Components/Add Player Component", false, 1101 )]
		static void AddPlayer() 
		{
			GameObject _object = Selection.activeObject as GameObject;
			
			if( _object != null && _object.GetComponent<ICECreaturePlayer>() == null )
				_object.AddComponent<ICECreaturePlayer>();
		}
		
		[MenuItem ( "Window/ICECreatureControl/Components/Add Player Component", true)]
		static bool ValidateAddPlayer(){
			return ValidateObject();
		}

		[MenuItem ( "Window/ICECreatureControl/Components/Add Item Component", false, 1101 )]
		static void AddItem() 
		{
			GameObject _object = Selection.activeObject as GameObject;
			
			if( _object != null && _object.GetComponent<ICECreatureItem>() == null )
				_object.AddComponent<ICECreatureItem>();
		}
		
		[MenuItem ( "Window/ICECreatureControl/Components/Add Item Component", true)]
		static bool ValidateAddItem(){
			return ValidateObject();
		}

		[MenuItem ( "Window/ICECreatureControl/Components/Add Location Component", false, 1101 )]
		static void AddLocation() 
		{
			GameObject _object = Selection.activeObject as GameObject;
			
			if( _object != null && _object.GetComponent<ICECreatureLocation>() == null )
				_object.AddComponent<ICECreatureLocation>();
		}
		
		[MenuItem ( "Window/ICECreatureControl/Components/Add Location Component", true)]
		static bool ValidateAddLocation(){
			return ValidateObject();
		}

		[MenuItem ( "Window/ICECreatureControl/Components/Add Waypoint Component", false, 1101 )]
		static void AddWaypoint() 
		{
			GameObject _object = Selection.activeObject as GameObject;
			
			if( _object != null && _object.GetComponent<ICECreatureWaypoint>() == null )
				_object.AddComponent<ICECreatureWaypoint>();
		}
		
		[MenuItem ( "Window/ICECreatureControl/Components/Add Waypoint Component", true)]
		static bool ValidateAddWaypoint(){
			return ValidateObject();
		}

		[MenuItem ( "Window/ICECreatureControl/Components/Add Marker Component", false, 1101 )]
		static void AddMarker() 
		{
			GameObject _object = Selection.activeObject as GameObject;
			
			if( _object != null && _object.GetComponent<ICECreatureMarker>() == null )
				_object.AddComponent<ICECreatureMarker>();
		}
		
		[MenuItem ( "Window/ICECreatureControl/Components/Add Marker Component", true)]
		static bool ValidateAddMarker(){
			return ValidateObject();
		}

		static bool ValidateObject() 
		{
			GameObject _obj = Selection.activeObject as GameObject;
			
			if( _obj != null && 
			   _obj.GetComponent<ICECreatureControl>() == null && 
			   _obj.GetComponent<ICECreatureRegister>() == null && 
			   _obj.GetComponent<ICECreaturePlayer>() == null && 
			   _obj.GetComponent<ICECreatureLocation>() == null && 
			   _obj.GetComponent<ICECreatureWaypoint>() == null && 
			   _obj.GetComponent<ICECreatureMarker>() == null && 
			   _obj.GetComponent<ICECreatureItem>() == null )
				return true;
			else
				return false;
		}


		// ATTRIBUTES
		[MenuItem ( "Window/ICECreatureControl/Components/Add Target Attribute", false, 1201 )]
		static void AddTargetAttribute() 
		{
			GameObject _object = Selection.activeObject as GameObject;
			
			if( _object != null && _object.GetComponent<ICECreatureTargetAttribute>() == null )
				_object.AddComponent<ICECreatureTargetAttribute>();
		}
		
		[MenuItem ( "Window/ICECreatureControl/Components/Add Target Attribute", true)]
		static bool ValidateTargetAttribute(){
			return ValidateAttributeObject();
		}

		[MenuItem ( "Window/ICECreatureControl/Components/Add Influence Attribute", false, 1201 )]
		static void AddInfluenceAttribute() 
		{
			GameObject _object = Selection.activeObject as GameObject;
			
			if( _object != null && _object.GetComponent<ICECreatureInfluenceAttribute>() == null )
				_object.AddComponent<ICECreatureInfluenceAttribute>();
		}
		
		[MenuItem ( "Window/ICECreatureControl/Components/Add Influence Attribute", true)]
		static bool ValidateInfluenceAttribute(){
			return ValidateAttributeObject();
		}

		[MenuItem ( "Window/ICECreatureControl/Components/Add Selection Attribute", false, 1201 )]
		static void AddSelectionAttribute() 
		{
			GameObject _object = Selection.activeObject as GameObject;
			
			if( _object != null && _object.GetComponent<ICECreatureSelectionAttribute>() == null )
				_object.AddComponent<ICECreatureSelectionAttribute>();
		}
		
		[MenuItem ( "Window/ICECreatureControl/Components/Add Selection Attribute", true)]
		static bool ValidateSelectionAttribute(){
			return ValidateAttributeObject();
		}


		static bool ValidateAttributeObject() 
		{
			GameObject _obj = Selection.activeObject as GameObject;
			
			if( _obj != null && 
			   _obj.GetComponent<ICECreatureControl>() == null && 
			   _obj.GetComponent<ICECreatureRegister>() == null )
				return true;
			else
				return false;
		}

		// EXTENSIONS
		[MenuItem ( "Window/ICECreatureControl/Components/Add Inventory Extension", false, 1301 )]
		static void AddInventoryExtension() 
		{
			GameObject _object = Selection.activeObject as GameObject;
			
			if( _object != null && _object.GetComponent<ICECreatureInventoryExtension>() == null )
				_object.AddComponent<ICECreatureInventoryExtension>();
		}
		
		[MenuItem ( "Window/ICECreatureControl/Components/Add Inventory Extension", true)]
		static bool ValidateInventoryExtension(){
			return ValidateExtension();
		}
		
		
		static bool ValidateExtension() 
		{
			GameObject _obj = Selection.activeObject as GameObject;
			
			if( _obj != null && 
			   _obj.GetComponent<ICECreatureRegister>() == null )
				return true;
			else
				return false;
		}


		// SUPPORT 201

		[MenuItem ("Window/ICECreatureControl/Manual (online)", false, 2001 )]
		static void ManualOnline ()
		{
			Application.OpenURL("http://www.ice-technologies.de/unity/ICECreatureControl/ICECreatureControlManual.pdf");
		}

		[MenuItem ("Window/ICECreatureControl/Homepage", false, 2001 )]
		static void Homepage ()
		{
			Application.OpenURL("http://www.icecreaturecontrol.com");
		}

		[MenuItem ("Window/ICECreatureControl/FAQ", false, 2001 )]
		static void FAQ ()
		{
			Application.OpenURL("http://www.icecreaturecontrol.com/FAQ/" );
		}

		[MenuItem ("Window/ICECreatureControl/Tutorials", false, 2001 )]
		static void Tutorials ()
		{
			Application.OpenURL("http://www.icecreaturecontrol.com/TUTORIALS/" );
		}

		[MenuItem ("Window/ICECreatureControl/Bug Report", false, 2001 )]
		static void BugReport ()
		{
			Application.OpenURL("http://www.ice-technologies.de/mantis/");
		}

		[MenuItem ("Window/ICECreatureControl/Unity Forum", false, 2001 )]
		static void UnityForum ()
		{
			Application.OpenURL("http://forum.unity3d.com/threads/347147/");
		}

		// WIZARD
		[MenuItem ("Window/ICECreatureControl/Wizard", false, 8000 )]
		static void Wizard ()
		{
			ice_CreatureWizard.Create();
		}

		// ABOUT
		[MenuItem ("Window/ICECreatureControl/About", false, 9000 )]
		static void AboutICE ()
		{
			ice_CreatureAbout.Create();
		}

	
	
	}
}