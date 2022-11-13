// ##############################################################################
//
// ice_editor_styles.cs
// Version 1.1.15
//
// © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.ice-technologies.com
// mailto:support@ice-technologies.com
// 
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// ##############################################################################

using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace ICE.Shared
{
	public static class ICEEditorTools {

		public static void AddTag( string _tag )
		{
			UnityEngine.Object[] _asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
			if( ( _asset != null ) && ( _asset.Length > 0 ) )
			{
				SerializedObject _object = new SerializedObject(_asset[0]);
				SerializedProperty _tags = _object.FindProperty("tags");

				for( int i = 0; i < _tags.arraySize; ++i )
				{
					if( _tags.GetArrayElementAtIndex(i).stringValue == _tag )
						return;    
				}

				_tags.InsertArrayElementAtIndex(0);
				_tags.GetArrayElementAtIndex(0).stringValue = _tag;
				_object.ApplyModifiedProperties();
				_object.Update();
			}
		}

		public static bool AddLayer( string _name )
        {
			if( LayerMask.NameToLayer( _name ) != -1 )
				return true;

			SerializedObject _tag_manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
 
			SerializedProperty _layers = _tag_manager.FindProperty("layers");
            if( _layers == null || ! _layers.isArray )
            {
				Debug.LogWarning( "Sorry, can't set up the layers! It's possible the format of the layers and tags data has changed in this version of Unity. Please add the required layer '" + _name + "' by hand!" );
                return false;
            }
 
			int _layer_index = -1;
			for ( int _i = 8 ; _i < 32 ; _i++ )
			{
				_layer_index = _i;
				SerializedProperty _layer = _layers.GetArrayElementAtIndex(_i);

				//Debug.Log( _layer_index + " - " + _layer.stringValue );

				if( _layer.stringValue == "" )
				{
					Debug.Log( "Setting up layers.  Layer " + _layer_index + " is now called " + _name );
					_layer.stringValue = _name;
					break;
				}
			}
 
            _tag_manager.ApplyModifiedProperties();

			if( LayerMask.NameToLayer( _name ) != -1 )
				return true;
			else
				return false;
        }

		public static bool HasAnimations( GameObject _object )
		{
			if( _object != null && (
				( _object.GetComponentInChildren<Animation>() != null && _object.GetComponentInChildren<Animation>().GetClipCount() > 0 ) ||
				( _object.GetComponentInChildren<Animator>() != null && _object.GetComponentInChildren<Animator>().runtimeAnimatorController != null ) ) )
			   return true;
			else
			   return false;
		}

		public static bool IsPrefab( GameObject _object )
		{
			if( _object != null && PrefabUtility.GetPrefabParent( _object ) == null && PrefabUtility.GetPrefabObject( _object ) != null ) // Is a prefab
				return true;
			else
				return false;
		}
	}
}
