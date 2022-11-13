// ##############################################################################
//
// ice_system.cs
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
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace ICE.Utilities
{

	
	public static class DateTools
	{
		public static string LocalizeDateTime( string _key, DateTime _datetime )
		{
			//       en-US: 6/19/2015 10:03:06 AM
			//       en-GB: 19/06/2015 10:03:06
			//       fr-FR: 19/06/2015 10:03:06
			//       de-DE: 19.06.2015 10:03:06
		
			return _datetime.ToString( new CultureInfo( _key ) );
		}
	}



	public static class MathTools 
	{
		public static float Normalize( float _value, float _min,float _max) {
			return ( _value - _min) / (_max - _min);
		}

		public static float Denormalize( float _normalized, float _min, float _max) {
			return ( _normalized * ( _max - _min ) + _min );
		}
	}

	public static class SystemTools 
	{
		public static bool Destroy( GameObject _object )
		{
			if( _object == null )
				return false;

			if( Application.isEditor )
				GameObject.DestroyImmediate( _object );
			else
				GameObject.Destroy( _object );

			return true;
		}

		public static bool AttachToTransform( GameObject _object, Transform _parent )
		{
			if( _object == null ||  _parent == null )
				return false;

			_object.transform.parent = _parent;
			_object.transform.position = _parent.position;
			_object.transform.rotation = _parent.rotation;
			_object.SetActive( true );

			if( _object.GetComponent<Rigidbody>() != null )
			{
				_object.GetComponent<Rigidbody>().useGravity = false;
				_object.GetComponent<Rigidbody>().isKinematic = true;
				_object.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			}

			return true;
		}

		public static bool DetachFromTransform( GameObject _object )
		{
			if( _object == null )
				return false;

			_object.transform.SetParent( null, true );
			_object.SetActive( true );

			if( _object.GetComponent<Rigidbody>() != null )
			{
				_object.GetComponent<Rigidbody>().useGravity = true;
				_object.GetComponent<Rigidbody>().isKinematic = false;
				_object.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			}

			return true;
		}

		private static void ProcessChild<T>(Transform aObj, ref List<T> aList) where T : Component
		{
			T c = aObj.GetComponent<T>();
			if (c != null)
				aList.Add(c);
			foreach(Transform child in aObj)
				ProcessChild<T>(child,ref aList);
		}

		public static T[] GetAllChildren<T>(this Transform aObj) where T : Component
		{
			List<T> result = new List<T>();
			ProcessChild<T>(aObj, ref result);
			return result.ToArray();
		}
		
		public static T[] GetAllChildren<T>(this GameObject aObj) where T : Component
		{
			List<T> result = new List<T>();
			ProcessChild<T>(aObj.transform, ref result);
			return result.ToArray();
		}

		public static void GetChildren( List<Transform> _list, Transform _transform )
		{
			if( _list == null || _transform == null )
				return;

			_list.Add( _transform );
			
			foreach (Transform _child in _transform )
				GetChildren( _list, _child );
		}

		public static Transform FindChildByName( string _name, Transform _parent )
		{
			if( _name == null || _name.Trim() == "" || _parent == null )
				return null;

			if( _parent.name == _name )
				return _parent;

			foreach (Transform _child in _parent )
			{
				Transform _result = FindChildByName( _name, _child );

				if( _result != null && _result.name == _name )
					return _result;
			}

			return null;
		}
		
		public static GameObject[] FindGameObjectsByLayer( int _layer ) 
		{
			GameObject[] _objects = GameObject.FindObjectsOfType<GameObject>();

			List<GameObject> _result_objects = new List<GameObject>();

			for( int i = 0; i < _objects.Length; i++) 
			{
			 	if( _objects[i].layer == _layer )
			    	_result_objects.Add(_objects[i]);
		    }
		     
			if( _result_objects.Count == 0 )
				return null;
		     
		     return _result_objects.ToArray();
		 }

		public static void CopyTransforms( Transform _source_transform , Transform _target_transform )
		{
			if( _source_transform == null || _target_transform == null )
				return;

			_target_transform.position = _source_transform.position;
			_target_transform.rotation = _source_transform.rotation;
			
			foreach( Transform _child in _target_transform) 
			{
				Transform _source = _source_transform.Find( _child.name );
				if( _source )
					CopyTransforms( _source, _child );
			}
		}

		/// <summary>
		/// Copies the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="_original">_original.</param>
		/// <param name="_destination">_destination.</param>
		public static Component CopyComponent( Component _original, GameObject _destination )
		{
			System.Type _type = _original.GetType();
			Component _copy = _destination.AddComponent(_type);
			// Copied fields can be restricted with BindingFlags
			System.Reflection.FieldInfo[] _fields = _type.GetFields(); 
			foreach (System.Reflection.FieldInfo _field in _fields)
				_field.SetValue( _copy, _field.GetValue(_original));
			return _copy;
		}

		/// <summary>
		/// Copies the component (generic).
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="_original">_original.</param>
		/// <param name="_destination">_destination.</param>
		public static T CopyComponent<T>(T _original, GameObject _destination) where T : Component
		{
			System.Type _type = _original.GetType();
			Component _copy = _destination.AddComponent(_type);
			System.Reflection.FieldInfo[] _fields = _type.GetFields();
			foreach( System.Reflection.FieldInfo _field in _fields )
				_field.SetValue( _copy, _field.GetValue(_original) );
			return _copy as T;
		}
		
		public static void ListAssemblies( bool _types = false )
		{
			System.Reflection.Assembly[] _assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			foreach( System.Reflection.Assembly _assembly in _assemblies )
			{
				Debug.Log( "Name : " + _assembly.FullName );

				if( _types == true )
					ListAssemblyTypes( _assembly );
			}
		}

		public static void ListAssemblyTypes( System.Reflection.Assembly _assembly )
		{
			if( _assembly == null )
				return;

			foreach( System.Type _type in _assembly.GetTypes() )
			{
				Debug.Log( "    Type : " + _type.Name );
			}
		}

		public static System.Reflection.Assembly GetAssemblyByTypeName( string _name )
		{
			System.Reflection.Assembly[] _assemblies = GetAssemblies();
			
			foreach( System.Reflection.Assembly _assembly in _assemblies )
			{
				foreach( System.Type _type in _assembly.GetTypes() )
				{
					if ( _type.Name == _name )
						return _assembly;
				}
			}
			
			return null;
		}

		public static System.Reflection.Assembly[] GetAssemblies()
		{
			return System.AppDomain.CurrentDomain.GetAssemblies();
		}
		
		public static bool DoesTypeExist( string _name )
		{
			System.Reflection.Assembly[] _assemblies = GetAssemblies();
			
			foreach( System.Reflection.Assembly _assembly in _assemblies )
			{
				foreach( System.Type _type in _assembly.GetTypes() )
				{
					if ( _type.Name == _name )
						return true;
				}
			}
			
			return false;
		}
	}
}
