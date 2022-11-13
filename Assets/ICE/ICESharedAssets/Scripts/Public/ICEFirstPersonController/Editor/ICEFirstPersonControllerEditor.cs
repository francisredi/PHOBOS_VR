// ##############################################################################
//
// ICEFirstPersonControllerEditor.cs
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
using UnityEditor;
using ICE;
using ICE.Layouts;
using ICE.Styles;

namespace ICE.Shared
{
	[CustomEditor(typeof(ICEFirstPersonController))]
	public class ICEFirstPersonControllerEditor : Editor {

		public virtual void OnEnable()
		{
			
		}

		public override void OnInspectorGUI()
		{
			ICEFirstPersonController _controller = (ICEFirstPersonController)target;

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Movement", true );
			EditorGUI.indentLevel++;
				_controller.WalkSpeed = ICEEditorLayout.DefaultSlider( "Walk Speed", "", _controller.WalkSpeed, 0.025f, 0, 25, 3, "" );
				_controller.RunSpeed = ICEEditorLayout.DefaultSlider( "Run Speed", "", _controller.RunSpeed, 0.025f, 0, 25, 7, "" );
				EditorGUI.indentLevel++;
					_controller.RunstepLenghten = ICEEditorLayout.DefaultSlider( "Runstep Lenghten", "", _controller.RunstepLenghten, 0.025f, 0, 25, 7, "" );
				EditorGUI.indentLevel--;	
				_controller.JumpSpeed = ICEEditorLayout.DefaultSlider( "Jump Speed", "", _controller.JumpSpeed, 0.025f, 0, 100, 25, "" );
				EditorGUI.indentLevel++;
					_controller.StickToGroundForce = ICEEditorLayout.DefaultSlider( "Stick To Ground Force", "", _controller.StickToGroundForce, 0.025f, 0, 25, 7, "" );
					_controller.GravityMultiplier = ICEEditorLayout.DefaultSlider( "Gravity", "", _controller.GravityMultiplier, 0.025f, 0, 100, 9.825f, "" );
				EditorGUI.indentLevel--;
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Sounds", true );
			EditorGUI.indentLevel++;
				ICEEditorLayout.Label( "Footsteps", false );
				EditorGUI.indentLevel++;
					_controller.StepInterval = ICEEditorLayout.DefaultSlider( "Footstep Interval", "", _controller.StepInterval, 0.025f, 0, 25, 3, "" );

					for( int _i = 0 ; _i < _controller.FootstepSounds.Count ; _i++ )
					{
						AudioClip _clip = _controller.FootstepSounds[_i];
						_clip = (AudioClip)EditorGUILayout.ObjectField("Footstep Clip #" + _i, _clip, typeof(AudioClip), false );
						if( _clip == null )
						{
							_controller.FootstepSounds.RemoveAt(_i);
							return;
						}
					}
					AudioClip _new_clip = (AudioClip)EditorGUILayout.ObjectField("Add Footstep Clip", null, typeof(AudioClip), false );
					if( _new_clip != null )
						_controller.FootstepSounds.Add( _new_clip );
				EditorGUI.indentLevel--;


				
				EditorGUILayout.Separator();
				_controller.JumpSound = (AudioClip)EditorGUILayout.ObjectField("Jump", _controller.JumpSound, typeof(AudioClip), false );
				_controller.LandSound = (AudioClip)EditorGUILayout.ObjectField("Land", _controller.LandSound, typeof(AudioClip), false );           // the sound played when character touches back on ground.
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "MouseLook", true );
			EditorGUI.indentLevel++;
				_controller.MouseLook.XSensitivity = ICEEditorLayout.DefaultSlider( "X Sensitivity", "", _controller.MouseLook.XSensitivity, 0.025f, 0, 100, 2, "" );
				_controller.MouseLook.YSensitivity = ICEEditorLayout.DefaultSlider( "Y Sensitivity", "", _controller.MouseLook.YSensitivity, 0.025f, 0, 100, 2, "" );
				_controller.MouseLook.ClampVerticalRotation = ICEEditorLayout.Toggle( "Clamp Vertical Rotation", "", _controller.MouseLook.ClampVerticalRotation , "" );

				ICEEditorLayout.MinMaxGroup( "X (min/max)", "", ref _controller.MouseLook.MinimumX, ref _controller.MouseLook.MaximumX, -90, 90, 1,"" );

				_controller.MouseLook.Smooth = ICEEditorLayout.Toggle( "Smooth", "", _controller.MouseLook.Smooth , "" );
				EditorGUI.indentLevel++;
					_controller.MouseLook.SmoothTime = ICEEditorLayout.DefaultSlider( "Smooth Time", "", _controller.MouseLook.SmoothTime, 0.025f, 0, 25, 5, "" );
				EditorGUI.indentLevel--;
				_controller.MouseLook.lockCursor = ICEEditorLayout.Toggle( "Lock Cursor", "", _controller.MouseLook.lockCursor , "" );
			EditorGUI.indentLevel--;


	
		}
	}
}
