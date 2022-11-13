// ##############################################################################
//
// ice_CreatureAnimation.cs
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

namespace ICE.Creatures.Objects
{
	[System.Serializable]
	public struct AnimationContainer
	{
		public void Copy( AnimationContainer _data )
		{
			AllowInterfaceSelector = _data.AllowInterfaceSelector;
			InterfaceType = _data.InterfaceType;

			Animator.Copy( _data.Animator );
			Animation.Copy( _data.Animation );
			Clip.Copy( _data.Clip );
		}

		public bool AllowInterfaceSelector;
		public AnimationInterfaceType InterfaceType;

		
		public AnimatorDataContainer Animator;
		public AnimationDataContainer Animation;
		public AnimationClipDataContainer Clip;

		public float GetAnimationLength()
		{
			if( InterfaceType == AnimationInterfaceType.LEGACY )
				return Animation.Length;
			else if( InterfaceType == AnimationInterfaceType.MECANIM )
				return Animator.Length;
			else if( InterfaceType == AnimationInterfaceType.CLIP && Clip.Clip != null )
				return Clip.Clip.length;
			else 
				return 0;
		}

		public string GetAnimationName()
		{
			if( InterfaceType == AnimationInterfaceType.LEGACY )
				return Animation.Name;
			else if( InterfaceType == AnimationInterfaceType.MECANIM )
				return Animator.Name;
			else if( InterfaceType == AnimationInterfaceType.CLIP && Clip.Clip != null )
				return Clip.Clip.name;
			else 
				return "";
		}

		public bool UseRootMotion{
			get{ 
				if( InterfaceType == AnimationInterfaceType.MECANIM )
					return Animator.ApplyRootMotion;
				else
					return false;
			}
		}

		public void Init()
		{
			Animator.Init();
			Animation.Init();
			Clip.Init();
		}
	}

	[System.Serializable]
	public class AnimatorParameterObject
	{
		public AnimatorParameterObject(){}
		public AnimatorParameterObject( AnimatorParameterObject _parameter ){
			Copy( _parameter );
		}

		public void Copy( AnimatorParameterObject _parameter )
		{
			Enabled = _parameter.Enabled;
			UseDynamicValue = _parameter.UseDynamicValue;
			Name = _parameter.Name;
			Type = _parameter.Type;
			IntegerValueType = _parameter.IntegerValueType;
			IntegerValue = _parameter.IntegerValue;
			FloatValueType = _parameter.FloatValueType;
			FloatValue = _parameter.FloatValue;
			BooleanValueType = _parameter.BooleanValueType;
			BooleanValue = _parameter.BooleanValue;
		}

		public bool Enabled = true;
		public bool UseDynamicValue = false;
		public string Name = "";
		public AnimatorControllerParameterType Type = AnimatorControllerParameterType.Float;

		public DynamicIntegerValueType IntegerValueType = DynamicIntegerValueType.undefined;
		public int IntegerValue = 0;

		public DynamicFloatValueType FloatValueType = DynamicFloatValueType.CreatureAngularSpeed;
		public float FloatValue = 0;

		public DynamicBooleanValueType BooleanValueType = DynamicBooleanValueType.CreatureIsGrounded;
		public bool BooleanValue = false;
	}

	[System.Serializable]
	public struct AnimatorDataContainer
	{
		public void Copy( AnimatorDataContainer _data )
		{
			Name = _data.Name;
			Index = _data.Index;
			Length = _data.Length;
			DefaultWrapMode = _data.DefaultWrapMode;
			AutoSpeed = _data.AutoSpeed;
			Speed = _data.Speed;
			TransitionDuration = _data.TransitionDuration;
			Type = _data.Type;
			ApplyRootMotion = _data.ApplyRootMotion;

			m_Parameters = new List<AnimatorParameterObject>();
			foreach( AnimatorParameterObject _parameter in _data.Parameters ){
				m_Parameters.Add( new AnimatorParameterObject( _parameter ) );
			}
		}

		[SerializeField]
		private List<AnimatorParameterObject> m_Parameters;
		public List<AnimatorParameterObject> Parameters{
			set{ m_Parameters = value;}
			get{

				if( m_Parameters == null )
					m_Parameters = new List<AnimatorParameterObject>();

				return m_Parameters;
			}
		}

		public string Name;
		public int Index;
		public float Length;
		public WrapMode DefaultWrapMode;
		public bool AutoSpeed;
		public float Speed;
		public float TransitionDuration;

		public AnimatorControlType Type;
		public bool ApplyRootMotion;

		public void Init()
		{
			this.AutoSpeed = false;
			this.Speed = 1;
			this.TransitionDuration = 0.5f;
		}
	}
	
	[System.Serializable]
	public struct AnimationDataContainer
	{
		public void Copy( AnimationDataContainer _data )
		{
			Name = _data.Name;
			Index = _data.Index;
			Length = _data.Length;
			wrapMode = _data.wrapMode;
			DefaultSpeed = _data.DefaultSpeed;
			DefaultWrapMode = _data.DefaultWrapMode;
			Speed = _data.Speed;
			AutoSpeed = _data.AutoSpeed;
			TransitionDuration = _data.TransitionDuration;
		}

		public string Name;
		public int Index;
		public float Length;
		public WrapMode wrapMode;
		public float DefaultSpeed;
		public WrapMode DefaultWrapMode;
		public float Speed;
		public bool AutoSpeed;
		public float TransitionDuration;

		public void Init()
		{
			this.AutoSpeed = false;
			this.Speed = 1;
			this.TransitionDuration = 0.5f;
		}
	}
	
	[System.Serializable]
	public struct AnimationClipDataContainer
	{
		public void Copy( AnimationClipDataContainer _data )
		{
			Clip = _data.Clip;
			TransitionDuration = _data.TransitionDuration;
		}

		[XmlIgnore]
		public AnimationClip Clip;

		public  float TransitionDuration;

		public void Init()
		{
			//this.AutoSpeed = false;
			//this.Speed = 1;
			this.TransitionDuration = 0.5f;
		}
	}

	public class AnimationObject 
	{
		public AnimationObject( GameObject _game_object )
		{
			Init( _game_object );
		}

		private GameObject m_Owner = null;
		private Animation m_Animation = null;
		private Animator m_Animator = null;

		private ICECreatureController m_Controller = null;
		public ICECreatureController Controller()
		{
			if( m_Controller == null )
				m_Controller = m_Owner.GetComponent<ICECreatureController>();

			return m_Controller;
		}

		public void Init( GameObject _game_object )
		{
			m_Owner = _game_object;
			m_Animation = m_Owner.GetComponentInChildren<Animation>();
			m_Animator = m_Owner.GetComponentInChildren<Animator>();
		}

		public delegate void OnCustomAnimationEvent();
		public event OnCustomAnimationEvent OnCustomAnimation;

		public delegate void OnCustomAnimationUpdateEvent();
		public event OnCustomAnimationUpdateEvent OnCustomAnimationUpdate;

		//private bool m_AnimatorAutoSpeed = false;
		//private bool m_AnimationAutoSpeed = false;

		public void Play()
		{
			if( m_Animator != null  )
				m_Animator.StartPlayback();
			else if( m_Animation != null && ! m_Animation.isPlaying )
				m_Animation.Play();				
		}

		public void Break()
		{
			if( m_Animator != null  )
			{
				m_Animator.StopPlayback();
				/*
				AnimationInfo[] _animator_info = m_Animator.GetCurrentAnimationClipState(0);
				
				
				if( _animator_info.Length >> ){

				}else{
					for(int idx=0;idx<_animator_info.Length;idx++){

						_animator_info[idx].
					}
				}*/
			}
			else if( m_Animation != null && m_Animation.isPlaying )
				m_Animation.Stop();				
		}

		public void Play( BehaviourModeRuleObject _rule )
		{
			if( _rule == null || _rule.Animation.InterfaceType == AnimationInterfaceType.NONE )
				return;

			if( _rule.Animation.InterfaceType == AnimationInterfaceType.LEGACY )
			{
				if ( m_Animation == null ) 
				{	
					Debug.LogError( "CAUTION : Missing Animation Component on " + m_Owner.gameObject.name );
					return;
				}

				WrapMode _mode = _rule.Animation.Animation.wrapMode;


				m_Animation[ _rule.Animation.Animation.Name ].wrapMode = _mode;
				m_Animation[ _rule.Animation.Animation.Name ].speed = _rule.Animation.Animation.Speed;//Mathf.Clamp( m_BehaviourData.MoveVelocity. controller.velocity.magnitude, 0.0, runMaxAnimationSpeed);
				m_Animation.CrossFade( _rule.Animation.Animation.Name, _rule.Animation.Animation.TransitionDuration );	

			}
			else if( _rule.Animation.InterfaceType == AnimationInterfaceType.CLIP )
			{
				if ( m_Animation == null ) 
					m_Animation = m_Owner.AddComponent<Animation>();

				if ( m_Animation != null && _rule.Animation.Clip.Clip != null ) 
				{
					m_Animation.AddClip( _rule.Animation.Clip.Clip, _rule.Animation.Clip.Clip.name );
					m_Animation.CrossFade( _rule.Animation.Clip.Clip.name, _rule.Animation.Clip.TransitionDuration );	
				}

			}
			else if( _rule.Animation.InterfaceType == AnimationInterfaceType.MECANIM )
			{
				if ( m_Animator == null ) 
				{	
					Debug.LogError( "Missing Animator Component!" );
					return;
				}
			
				if( _rule.Animation.Animator.Type == AnimatorControlType.DIRECT )
				{			
					m_Animator.CrossFade( _rule.Animation.Animator.Name, _rule.Animation.Animator.TransitionDuration, -1, 0); 
					m_Animator.speed = _rule.Animation.Animator.Speed;
					//m_AnimatorAutoSpeed = _rule.Animation.Animator.AutoSpeed;
					/*if( _rule.Animation.Animator.AutoSpeed )
						m_Animator.speed = Mathf.Clamp( m_BehaviourData.MoveVelocity. controller.velocity.magnitude, 0.0, runMaxAnimationSpeed);
					else*/

				}
				else if( _rule.Animation.Animator.Type == AnimatorControlType.ADVANCED )
				{
					m_Animator.applyRootMotion = _rule.Animation.Animator.ApplyRootMotion;

					foreach( AnimatorParameterObject _parameter in _rule.Animation.Animator.Parameters )
					{
						if( _parameter.Enabled )
						{
							if( _parameter.Type == AnimatorControllerParameterType.Bool )
							{
								if( _parameter.UseDynamicValue )
									m_Animator.SetBool( _parameter.Name, Controller().GetDynamicBooleanValue( _parameter.BooleanValueType ) );
								else
									m_Animator.SetBool( _parameter.Name, _parameter.BooleanValue );
							}
							else if( _parameter.Type == AnimatorControllerParameterType.Float )
							{
								if( _parameter.UseDynamicValue )
									m_Animator.SetFloat( _parameter.Name, Controller().GetDynamicFloatValue( _parameter.FloatValueType ) );
								else
									m_Animator.SetFloat( _parameter.Name, _parameter.FloatValue );
							}
							else if( _parameter.Type == AnimatorControllerParameterType.Int )
							{
								if( _parameter.UseDynamicValue )
									m_Animator.SetInteger( _parameter.Name, Controller().GetDynamicIntegerValue( _parameter.IntegerValueType ) );
								else
									m_Animator.SetInteger( _parameter.Name, _parameter.IntegerValue );
							}
							else if( _parameter.Type == AnimatorControllerParameterType.Trigger )
							{
								m_Animator.SetTrigger( _parameter.Name );
							}
						}
					}
				}
			}
			else if( _rule.Animation.InterfaceType == AnimationInterfaceType.CUSTOM )
			{
				if( OnCustomAnimation != null )
					OnCustomAnimation();
			}
		}
				
	

		//--------------------------------------------------
		// Update
		//--------------------------------------------------
		public void UpdateBegin( BehaviourModeRuleObject _rule )
		{
			if( _rule == null || _rule.Animation.InterfaceType == AnimationInterfaceType.NONE )
				return;


			if( _rule.Animation.InterfaceType == AnimationInterfaceType.LEGACY )
			{

			}
			else if( _rule.Animation.InterfaceType == AnimationInterfaceType.CLIP )
			{

			}
			else if( _rule.Animation.InterfaceType == AnimationInterfaceType.MECANIM )
			{
				if ( m_Animator == null ) 
					return;
					
				if( _rule.Animation.Animator.Type == AnimatorControlType.DIRECT )
				{

				}
				else if( _rule.Animation.Animator.Type == AnimatorControlType.ADVANCED )
				{
					foreach( AnimatorParameterObject _parameter in _rule.Animation.Animator.Parameters )
					{
						if( _parameter.Enabled )
						{
							if( _parameter.Type == AnimatorControllerParameterType.Bool )
							{
								if( _parameter.UseDynamicValue )
									m_Animator.SetBool( _parameter.Name, Controller().GetDynamicBooleanValue( _parameter.BooleanValueType ) );
								else
									m_Animator.SetBool( _parameter.Name, _parameter.BooleanValue );
							}
							else if( _parameter.Type == AnimatorControllerParameterType.Float )
							{
								if( _parameter.UseDynamicValue )
									m_Animator.SetFloat( _parameter.Name, Controller().GetDynamicFloatValue( _parameter.FloatValueType ) );
								else
									m_Animator.SetFloat( _parameter.Name, _parameter.FloatValue );
							}
							else if( _parameter.Type == AnimatorControllerParameterType.Int )
							{
								if( _parameter.UseDynamicValue )
									m_Animator.SetInteger( _parameter.Name, Controller().GetDynamicIntegerValue( _parameter.IntegerValueType ) );
								else
									m_Animator.SetInteger( _parameter.Name, _parameter.IntegerValue );
							}
							else if( _parameter.Type == AnimatorControllerParameterType.Trigger )
							{
								m_Animator.SetTrigger( _parameter.Name );
							}
						}
					}
				}
			}
			else if( _rule.Animation.InterfaceType == AnimationInterfaceType.CUSTOM )
			{
				if( OnCustomAnimationUpdate != null )
					OnCustomAnimationUpdate();
			}

			/*
			if( m_AnimatorAutoSpeed && m_Animator != null )
				m_Animator.speed = Mathf.Clamp( m_BehaviourData.MoveVelocity. controller.velocity.magnitude, 0.0, runMaxAnimationSpeed);*/



		}
	}

}
