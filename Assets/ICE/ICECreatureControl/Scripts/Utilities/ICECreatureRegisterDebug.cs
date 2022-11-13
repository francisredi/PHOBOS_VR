// ##############################################################################
//
// ICECreatureRegisterDebug.cs
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
using ICE.Creatures;
using ICE.Creatures.Objects;
using ICE.Creatures.EnumTypes;
using ICE.Utilities;

namespace ICE.Creatures.Utilities
{
	[RequireComponent (typeof (ICECreatureRegister))]
	public class ICECreatureRegisterDebug : MonoBehaviour {

		private void OnDrawGizmosSelected()
		{
			ICECreatureRegister _register = ICECreatureRegister.Instance;
			if( _register == null )
				return;
			
			if( _register.UseDrawSelected )
				DrawRegisterGizmos();
		}

		private void OnDrawGizmos()
		{
			ICECreatureRegister _register = ICECreatureRegister.Instance;
			if( _register == null )
				return;
			
			if( ! _register.UseDrawSelected )
				DrawRegisterGizmos();
		}

		private void DrawRegisterGizmos()
		{
			ICECreatureRegister _register = ICECreatureRegister.Instance;
			if( _register == null )
				return;
			
			if( ! _register.UseDebug )
				return;

			Gizmos.color = Color.blue;
			Gizmos.DrawSphere( transform.position, 0.5f );
			CustomGizmos.Text( "CREATURE REGISTER", transform.position, Gizmos.color );


			foreach( ReferenceGroupObject _group in _register.ReferenceGroupObjects )
			{
				if( _group.Reference == null )
					continue;

				if( _register.ShowReferenceGizmos )
				{
					Gizmos.color = _register.ColorReferences;
					Vector3 _pos = _group.Reference.transform.position;
					Gizmos.DrawSphere( _pos, 0.5f);
					_pos.y += 2;
					if(_register.ShowReferenceGizmosText )
						CustomGizmos.Text( _group.Reference.name + " (REFERENCE)", _pos , Gizmos.color );
				}

				if( _register.ShowCloneGizmos )
				{
					foreach( GameObject _item in _group.Items )
					{
						if( _group.Reference == _item )
							continue;

						Gizmos.color = _register.ColorClones;
						Vector3 _pos = _item.transform.position;
						Gizmos.DrawSphere( _item.transform.position, 0.5f);
						_pos.y += 2;
						if(_register.ShowCloneGizmosText )
							CustomGizmos.Text( _item.name + " (CLONE)", _pos , Gizmos.color );
					}
				}

				if(_register.ShowSpawnPointGizmos )
				{
					foreach( SpawnPointObject _point in _group.SpawnPoints )
					{
						if( _point.SpawnPointGameObject == null )
							continue;
						
						Gizmos.color = _register.ColorSpawnPoints;
						Vector3 _pos = _point.SpawnPointGameObject.transform.position;
						Gizmos.DrawSphere( _pos, 0.5f);
						CustomGizmos.Circle( _pos,_point.MinSpawningRange,CustomGizmos.GetBestDegrees(_point.MinSpawningRange, 360), false );
						CustomGizmos.BeamCircle( _pos,_point.MaxSpawningRange,CustomGizmos.GetBestDegrees(_point.MaxSpawningRange, 360), false, _point.MaxSpawningRange - _point.MinSpawningRange, "", false, true );

						_pos.z += _point.MaxSpawningRange;
						_pos.y += 4;
						if(_register.ShowSpawnPointGizmosText )
							CustomGizmos.Text( _point.SpawnPointGameObject.name + " (SP)", _pos , Gizmos.color );
						//UnityEditor.Handles.
					}
				}
			}
		}
	}
}
