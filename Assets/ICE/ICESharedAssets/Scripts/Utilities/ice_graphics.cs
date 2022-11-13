// ##############################################################################
//
// ice_CreatureTools.cs
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
using System.Collections;
using System.Collections.Generic;
using ICE.Utilities.EnumTypes;

namespace ICE.Utilities
{
	public static class GraphicTools 
	{


		public static LayerMask GetLayerMask( List<string> _layers, LayerMask _mask, bool _water = false )
		{
			if( _mask == -1 )
			{
				if( _layers.Count > 0 )
				{
					_mask = 0;
					foreach( string _layer in _layers )
					{
						int _index = LayerMask.NameToLayer( _layer );
						if( _index != -1 )
							_mask |= (1 << _index );
					}
				}
				else
					_mask = Physics.DefaultRaycastLayers;

				if( _water )
					_mask |= (1 << 4 );
				else if( IsInLayerMask( 4, _mask ) )
					_mask |= (1 >> 4 );
			}
				
			return _mask;
		}

		public static Vector3 GetRandomCirclePosition( Vector3 _center, float _min, float _max ) 
		{ 
			float _distance = Random.Range( _min, _max );
			float _angle = Random.Range( 0, 360 );

			return GetAnglePosition( _center, _angle, _distance );
		}

		//--------------------------------------------------
		
		public static Vector3 GetRandomRectPosition( Vector3 origin, float x = 10.0f, float z = 10.0f, bool centered = false ) 
		{ 
			Vector3 new_position = origin;
			
			if( centered )
			{
				new_position.x += UnityEngine.Random.Range( -(x/2), (x/2) );
				new_position.z += UnityEngine.Random.Range( -(z/2), (z/2) );
			}
			else
			{
				new_position.x += UnityEngine.Random.Range( 0, x );
				new_position.z += UnityEngine.Random.Range( 0, z );
			}
			
			//new_position.y = GetGroundLevel( new_position );
			
			return new_position;
		}

		//--------------------------------------------------

		public static Vector3 GetRandomPosition( Vector3 _position, float _radius ) { 
			
			if( _radius == 0 )
				return _position;
			
			Vector2 _new_circle_point = UnityEngine.Random.insideUnitCircle * _radius;
			
			Vector3 _new_position = Vector3.zero;
			_new_position.x = _position.x + _new_circle_point.x;
			_new_position.z = _position.z + _new_circle_point.y;
			_new_position.y = 0;

			return _new_position;
			
		}

		public static Vector3 GetDirectionPosition( Transform _transform, float _angle, float _distance )
		{
			if( _transform == null )
				return Vector3.zero;
			
			_angle += _transform.eulerAngles.y;
			
			if( _angle > 360 )
				_angle = _angle - 360;
			
			Vector3 _world_offset = GetAnglePosition( _transform.position, _angle, _distance );
			
			return _world_offset;
		}

		public static float GetGroundLevel( Vector3 position, GroundCheckType _type, LayerMask _layerMask )
		{
			if( _type == GroundCheckType.NONE )
				return position.y;
			
			Vector3 pos = position;
			pos.y = 1000;
			
			if( _type == GroundCheckType.RAYCAST )
			{
				RaycastHit hit;
				if (Physics.Raycast( pos, Vector3.down, out hit, Mathf.Infinity, _layerMask ) )
					position.y = hit.point.y;
			}
			else 
				position.y = Terrain.activeTerrain.SampleHeight( position );
			
			return position.y;
		}

		public static float GetDirectionAngle( Transform _target, Vector3 _position )
		{
			Vector3 _heading = _position - _target.position;
			float _angle = GraphicTools.GetOffsetAngle( _heading );
			_angle -= _target.eulerAngles.y;// _target.transform.eulerAngles.y;
			
			if( _angle < 0 )
				_angle = _angle + 360;
			
			if( _angle > 180 )
				_angle -= 360; 

			return _angle;
		}



		public static Vector3 GetOffsetPosition( Transform _transform, Vector3 _offset )
		{
			if( _transform == null )
				return Vector3.zero;
			
			Vector3 _local_offset = _offset;
			
			_local_offset.x = _local_offset.x/_transform.lossyScale.x;
			_local_offset.y = _local_offset.y/_transform.lossyScale.y;
			_local_offset.z = _local_offset.z/_transform.lossyScale.z;
			
			return _transform.TransformPoint( _local_offset );
		}

		//returns negative value when left, positive when right, and 0 for forward/backward
		public static float AngleDirection( Vector3 _forward, Vector3 _up, Vector3 _heading )
		{
			Vector3 _perpendicular = Vector3.Cross( _forward, _heading );
			float _direction = Vector3.Dot( _perpendicular , _up  );

			return _direction;
		}   

		//returns negative value when left, positive when right, and 0 for forward/backward
		public static float AngleDirectionExt( Vector3 _forward, Vector3 _up, Vector3 _heading )
		{
			Vector3 _perpendicular = Vector3.Cross( _forward, _heading );
			float _direction = Vector3.Dot( _perpendicular , _up  );

			if( _direction > 1f )
				_direction = 1;
			else if( _direction < -1f )
				_direction = -1;
			else
				_direction = 0;
			
			return _direction;
		}  

		public static Vector3 GetAnglePosition( Vector3 _center, float _angle, float _radius )
		{ 
			float _a = _angle * Mathf.PI / 180f;			
			return _center + new Vector3(Mathf.Sin(_a) * _radius, 0, Mathf.Cos(_a) * _radius );
		}

		public static float GetOffsetAngle( Vector3 _offset )
		{ 
			float _local_angle = Mathf.Atan2( _offset.x, _offset.z) * Mathf.Rad2Deg;

			if( _local_angle < 0 )
				_local_angle = 360 + _local_angle;
			
			return _local_angle;
		}

		public static float GetOffsetAngleRaw( Vector3 _offset )
		{ 
			return Mathf.Atan2( _offset.x, _offset.z) * Mathf.Rad2Deg;
		}

		public static float GetPositionAngle( Transform _transform, Vector3 _world_offset )
		{ 
			Vector3 _local_offset = _transform.InverseTransformPoint( _world_offset );

			_local_offset.x = _world_offset.x*_transform.lossyScale.x;
			_local_offset.y = _world_offset.y*_transform.lossyScale.y;
			_local_offset.z = _world_offset.z*_transform.lossyScale.z;	

			var _local_angle = Mathf.Atan2( _local_offset.x, _local_offset.z) * Mathf.Rad2Deg;


			if( _local_angle < 0 )
				_local_angle = 360 + _local_angle;

			return _local_angle;
		}

		//********************************************************************************
		// GetHorizontalDistance
		//********************************************************************************
		public static float GetHorizontalDistance( Vector3 pos1, Vector3 pos2 )
		{
			Vector3 pos_1 = pos1;
			Vector3 pos_2 = pos2;
			
			pos_1.y = 0;
			pos_2.y = 0;
			
			return Vector3.Distance(pos_1, pos_2);
		}

		public static GameObject GetRandomObjectByTag( string _tag )
		{
			if( _tag.Trim() == "" )
				return null;

			GameObject[] _objects = GameObject.FindGameObjectsWithTag( _tag );

			if( _objects != null && _objects.Length > 0 )
				return _objects[ (int)Random.Range( 0, _objects.Length ) ];
			else
				return null;
		}

		public static GameObject GetRandomObjectByName( string _name )
		{
			if( _name.Trim() == "" )
				return null;

			GameObject[] _objects = GameObject.FindObjectsOfType<GameObject>();

			if( _objects != null && _objects.Length > 0 ) 
				return _objects[ (int)Random.Range( 0, _objects.Length ) ];
			else
				return GameObject.Find( _name );
		}

		public static GameObject GetNearestObjectByName( string _name, Vector3 _position, float _distance )
		{
			if( _name.Trim() == "" )
				return null;

			if( _distance == 0 )
				_distance = Mathf.Infinity;

			GameObject _best_object = null;
			float _best_distance = _distance;

			GameObject[] _objects = GameObject.FindObjectsOfType<GameObject>();

			foreach( GameObject _tmp_object in _objects )
			{
				if( _tmp_object != null && _tmp_object.name == _name && _tmp_object.transform.position != _position && _tmp_object.activeInHierarchy  )
				{
					float _tmp_distance = GetHorizontalDistance( _position, _tmp_object.transform.position );

					if( _tmp_distance < _best_distance )
					{
						_best_distance = _tmp_distance;					
						_best_object = _tmp_object;
					}
				}
			}

			return _best_object;
		}


		public static GameObject GetNearestObject( GameObject[] _objects, Vector3 _position, float _distance )
		{
			if( _objects == null || _objects.Length == 0 )
				return null;

			if( _distance == 0 )
				_distance = Mathf.Infinity;

			GameObject _best_object = null;
			float _best_distance = _distance;

			for( int i = 0; i < _objects.Length; i++ )
			{
				GameObject _tmp_object = _objects[i];

				if( _tmp_object != null && _tmp_object.activeInHierarchy && _tmp_object.transform.position != _position )
				{
					float _tmp_distance = GetHorizontalDistance( _position, _tmp_object.transform.position );

					if( _tmp_distance < _best_distance )
					{
						_best_distance = _tmp_distance;					
						_best_object = _tmp_object;
					}
				}
			}

			return _best_object;
		}


		public static bool IsInLayerMask(GameObject _object, LayerMask _layerMask )
		{
			// Convert the object's layer to a bitfield for comparison
			int _mask = (1 << _object.layer);
			if ((_layerMask.value & _mask) > 0)  // Extra round brackets required!
				return true;
			else
				return false;
		}

		public static bool IsInLayerMask(int layer, int layerMask)
		{
			return (layerMask & 1<<layer) == 0;
		}

		public static LayerMask NamesToMask(params string[] layerNames)
		{
			LayerMask ret = (LayerMask)0;
			foreach(var name in layerNames)
			{
				ret |= (1 << LayerMask.NameToLayer(name));
			}
			return ret;
		}

		public static float FahrenheitToCelsius( float _fahrenheit )
		{
			return (5f / 9f) * (_fahrenheit - 32f);
		}

		public static float CelsiusToFahrenheit( float _celsius )
		{
			return _celsius * (9f / 5f) + 32f;
		}

		/// <summary>
		/// Gets the main texture of the terrain.
		/// </summary>
		/// <returns>The main terrain texture index</returns>
		/// <param name="_world_pos">_world_pos.</param>
		/// <param name="_terrain">_terrain.</param>
		/// <description>
		/// http://answers.unity3d.com/questions/34328/terrain-with-multiple-splat-textures-how-can-i-det.html
		/// </description>
		public static int GetMainTerrainTexture( Vector3 _world_pos, Terrain _terrain )
		{
			TerrainData _terrain_data = _terrain.terrainData;
			Vector3 _terrain_position = _terrain.transform.position;
			
			// evaluate the splat map cell 
			int _map_x = (int)((( _world_pos.x - _terrain_position.x ) / _terrain_data.size.x ) * _terrain_data.alphamapWidth );
			int _map_z = (int)((( _world_pos.z - _terrain_position.z ) / _terrain_data.size.z ) * _terrain_data.alphamapHeight );
			
			// get the splat map data 
			float[,,] _map_data = _terrain_data.GetAlphamaps(_map_x,_map_z,1,1);			
			
			// extracting the _map_data array data to the 1d mix array:
			float[] _mix = new float[_map_data.GetUpperBound(2)+1];
			for( int i=0; i<_mix.Length; ++i )
				_mix[i] = _map_data[0,0,i];    
			
			float _max_mix = 0;
			int _max_index = 0;
			
			// find the maximum by looping through the mix values 
			for( int _i = 0; _i < _mix.Length; ++_i)
			{
				if( _mix[_i] > _max_mix )
				{
					_max_index = _i;
					_max_mix = _mix[_i];
				}
			}
			
			return _max_index;
			
		}
	}

}
