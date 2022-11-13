using UnityEngine;
using System.Collections.Generic;

namespace RealisticEyeMovements {


	public class Utils
	{

		static readonly Dictionary<string, GameObject> dummyObjects = new Dictionary<string, GameObject>();



		/*
		@t is the current time (or position) of the tween.
		@b is the beginning value of the property.
		@c is the change between the beginning and destination value of the property.
		@d is the total time of the tween.
		*/			
		public static float EaseSineIn( float t, float b, float c, float d )
		{
			return -c * (float)Mathf.Cos( t / d * ( Mathf.PI / 2 ) ) + c + b;
		}
	
	
	
		public static GameObject FindChildInHierarchy(GameObject go, string name)
		{
			if (go.name == name)
				return go;
		
			foreach (Transform t in go.transform)
			{
				GameObject foundGO = FindChildInHierarchy(t.gameObject, name);
				if (foundGO != null)
					return foundGO;
			}
		
			return null;
		}
	
	
	
		public static Transform GetCommonAncestor( Transform xform1, Transform xform2 )
		{
			List<Transform> ancestors1 = new List<Transform> { xform1 };

			while ( xform1.parent != null )
			{
				xform1 = xform1.parent;
				ancestors1.Add( xform1 );
			}

			while ( xform2.parent != null && false == ancestors1.Contains( xform2 ))
				xform2 = xform2.parent;

			return ancestors1.Contains( xform2) ? xform2 : null;
		}



		// returns the angle in the range -180 to 180
		public static float NormalizedDegAngle ( float degrees )
		{
			int factor = (int) (degrees/360);
			degrees -= factor * 360;
			if ( degrees > 180 )
				return degrees - 360;
		
			if ( degrees < -180 )
				return degrees + 360;
		
			return degrees;
		}
	


		public static void PlaceDummyObject ( string name, Vector3 pos, float scale = 0.1f, Quaternion? rotation = null )
		{
			GameObject dummyObject = null;
		
			if ( dummyObjects.ContainsKey(name) )
				dummyObject = dummyObjects[ name ];
			else
			{
				dummyObject = GameObject.CreatePrimitive( PrimitiveType.Cube );
				dummyObject.transform.localScale = scale * Vector3.one;
				dummyObject.GetComponent<Renderer>().material = Resources.Load ("DummyObjectMaterial") as Material;
				Object.Destroy (dummyObject.GetComponent<Collider>());
				dummyObject.name = name;
			
				dummyObjects[ name ] = dummyObject;
			}
		
			dummyObject.transform.position = pos;
			dummyObject.transform.rotation = rotation ?? Quaternion.identity;
		}

	}

}
