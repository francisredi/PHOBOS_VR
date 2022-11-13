// ##############################################################################
//
// ice_CreatureBehavior.cs
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
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

using ICE.Creatures.EnumTypes;
using ICE.Utilities;

namespace ICE.Creatures.Objects
{

	[System.Serializable]
	public struct StatusContainer
	{
		public bool Foldout;
		public bool Enabled;
		public float Interval;

		public float Damage;
		public float Stress;
		public float Debility;
		public float Hunger;
		public float Thirst;

		public float Aggressivity;
		public float Anxiety;
		public float Experience;
		public float Nosiness;

		public OdourContainer Odour;

		private float m_time;
		public bool Ready()
		{
			m_time += Time.deltaTime;
			if( Interval == 0 || m_time > Interval )
			{
				m_time = 0;
				return true;
			}
			else
				return false;
		}
	}

	[System.Serializable]
	public struct LinkContainer
	{
		public bool Enabled;

		public LinkType Link;
		public string BehaviourModeKey;
		public int RuleIndex;
	}


	[System.Serializable]
	public struct EffectContainer
	{
		public bool Enabled;
		private float m_IntervalTimer;
		private float m_Interval;

		[XmlIgnore]
		public GameObject ReferenceObject;
		public Vector3 Offset;
		public Quaternion Rotation;
		public RandomOffsetType OffsetType;
		public float OffsetRadius;
		public bool Detach;
		public float IntervalMin;
		public float IntervalMax;
		public float MaxInterval;
		public string ParentName;

		private Transform m_Parent;

		private GameObject InstantiateNewEffect( GameObject _owner )
		{
			if( _owner == null )
				return null;

			Vector3 _position = _owner.transform.position;
			Vector3 _local_offset = Vector3.zero;
			
			if( OffsetType == RandomOffsetType.EXACT )
			{
				_local_offset = Offset;
			}
			else if( OffsetRadius > 0 )
			{
				Vector2 _pos = Random.insideUnitCircle * OffsetRadius;
				
				_local_offset.x = _pos.x;
				_local_offset.z = _pos.y;
				
				if( OffsetType == RandomOffsetType.HEMISPHERE )
					_local_offset.y = Random.Range(0,OffsetRadius ); 
				else if( OffsetType == RandomOffsetType.SPHERE )
					_local_offset.y = Random.Range( - OffsetRadius , OffsetRadius ); 
			}
			
			if( _local_offset != Vector3.zero )
			{
				_local_offset.x = _local_offset.x/_owner.transform.lossyScale.x;
				_local_offset.y = _local_offset.y/_owner.transform.lossyScale.y;
				_local_offset.z = _local_offset.z/_owner.transform.lossyScale.z;
				
				_position = _owner.transform.TransformPoint( _local_offset );
			}
			
			if( Rotation == Quaternion.identity )
				Rotation = _owner.transform.rotation;
			
			GameObject _effect = (GameObject)Object.Instantiate( ReferenceObject, _position, Rotation );

			if( _effect == null )
				return null;

			_effect.name = ReferenceObject.name;

			if( Detach == false )
			{
				if( m_Parent == null || ( ParentName != null && m_Parent.name != ParentName ) )
					m_Parent = SystemTools.FindChildByName( ParentName, _owner.transform ); 
				else
					m_Parent = _owner.transform;

				_effect.transform.SetParent( m_Parent, true );

				/*
				 *_effect.transform.parent = m_Parent;
				_effect.transform.position = m_Parent.position;
				_effect.transform.rotation = m_Parent.rotation;*/
			}

			_effect.SetActive( true );

			return _effect;
		}

		[XmlIgnore]
		public GameObject CurrentEffect;
		public void StartEffect( GameObject _owner ) 
		{
			if( _owner == null )
				return;
		
			m_IntervalTimer = 0;
			m_Interval = Random.Range( IntervalMin, IntervalMax );

			if( Enabled == true && ReferenceObject != null )
			{
				if( CurrentEffect == null )
				{
					GameObject _effect = InstantiateNewEffect( _owner );

					if( Detach == false )
						CurrentEffect = _effect;
				}
				else
					CurrentEffect.SetActive( true );
			}
			else if( Enabled == false && CurrentEffect != null )
			{
				GameObject.Destroy( CurrentEffect );
				CurrentEffect = null;
			}
		}
		
		public void StopEffect() 
		{
			if( CurrentEffect != null && Detach == false )
				CurrentEffect.SetActive( false );
		}

		public void Update( GameObject _owner )
		{
			if( Mathf.Max( IntervalMin, IntervalMax ) > 0 && Detach == true )
			{
				m_IntervalTimer += Time.deltaTime;

				if( m_IntervalTimer > m_Interval )
					StartEffect( _owner );
			}
		}
	}

	[System.Serializable]
	public struct BehaviourFavouredContainer
	{
		public bool Enabled;

		private float m_Runtime; 
		public float Runtime{
			get{ return m_Runtime;}
		}
		public float MinRuntime;
		public float MaxRuntime;
		public float MaximumRuntime;

		public float FavouredPriority; 

		public bool  FavouredUntilNextMovePositionReached; 
		public bool FavouredUntilTargetMovePositionReached; 
		public bool FavouredUntilDetourPositionReached; 
		public bool FavouredTarget;
		public string FavouredTargetName;
		public float FavouredTargetRange;

		public float UpdateRuntime(){
			m_Runtime = ( Enabled == true?UnityEngine.Random.Range( MinRuntime, MaxRuntime ):0 );
			return m_Runtime;
		}

		public bool FavouredUntilNewTargetInRange( TargetObject _target, float _distance )
		{
			if( _target == null || _target.TargetGameObject == null || FavouredTarget == false || FavouredTargetRange <= 0 )
				return false;
			
			if( _distance <= FavouredTargetRange && ( FavouredTargetName.Trim() == "" || _target.TargetGameObject.name == FavouredTargetName ) )
				return false;
			else
				return true;
		}
	}

	//--------------------------------------------------
	// Animations
	//--------------------------------------------------
	[System.Serializable]
	public class BehaviourModeObject : System.Object 
	{
		public BehaviourModeObject(){}
		public BehaviourModeObject( BehaviourModeObject _mode )
		{
			Favoured = _mode.Favoured;
			Foldout = _mode.Foldout;
			Key = _mode.Key;

			Rules.Clear();
			foreach( BehaviourModeRuleObject _rule in _mode.Rules )
				Rules.Add( new BehaviourModeRuleObject( _rule ) );

		}

		[XmlAttribute("name")]
		public string Key = "";
		public bool Foldout = true;
		public bool ShowInfoText = false;
		public string InfoText = "";
		public SequenceOrderType RulesOrderType;
		public bool RulesOrderInverse;

		public BehaviourFavouredContainer Favoured;

		private bool m_Active = false;
		public bool Active{
			get{return m_Active;}
		}
		public bool SetActive( bool _active )
		{
			m_Active = _active;

			if( m_Active == false  )
			{
				if( Rule != null )
					Rule.Effect.StopEffect();
			}
			else
			{
				Favoured.UpdateRuntime();
			}

			return m_Active;
		}





		public bool HasActiveDetourRule{
			get{
				if( Rule.Move.Type == MoveType.DETOUR ) 
					return true;
				else
					return false;			
			}
		}

		public bool HasDetourRules{
			get{
				foreach( BehaviourModeRuleObject _rule in Rules )
				{
					if( _rule.Move.Type == MoveType.DETOUR ) 
						return true;
				}
	
				Favoured.FavouredUntilDetourPositionReached = false;

				return false;
			}
		}

		private bool m_RuleChanged = false;
		public bool RuleChanged{
			get{ return m_RuleChanged; }
		}

		private int m_RuleIndex = 0;
		public int RuleIndex{
			get{ return m_RuleIndex; }
		}

		private int m_LastRuleIndex = 0;
		public int LastRuleIndex{
			get{ return m_LastRuleIndex; }
		}

		private BehaviourModeRuleObject m_Rule = null;
		public BehaviourModeRuleObject Rule{
			get{ return m_Rule; }
		}

		private BehaviourModeRuleObject m_LastRule = null;
		public BehaviourModeRuleObject LastRule{
			get{ return m_LastRule; }
		}

		public float RuleLength()
		{
			if( m_Rule != null )
			{ 
				if( m_Rule.UseCustomLength && Mathf.Max( m_Rule.LengthMin, m_Rule.LengthMax ) > 0 )
					return UnityEngine.Random.Range ( Mathf.Min ( m_Rule.LengthMin, m_Rule.LengthMax ), Mathf.Max ( m_Rule.LengthMin, m_Rule.LengthMax ) );
				else if( m_Rule.Animation.InterfaceType == AnimationInterfaceType.LEGACY )
					return m_Rule.Animation.Animation.Length;
				else if( m_Rule.Animation.InterfaceType == AnimationInterfaceType.CLIP && m_Rule.Animation.Clip.Clip != null )
					return m_Rule.Animation.Clip.Clip.length;
				else if( m_Rule.Animation.InterfaceType == AnimationInterfaceType.MECANIM )
					return m_Rule.Animation.Animator.Length;
				else
					return 0.0f;
			}
			else
				return 0.0f;
		}




		[SerializeField]
		private List<BehaviourModeRuleObject> m_Rules = new List<BehaviourModeRuleObject>();

		[XmlArray("Rules"),XmlArrayItem("BehaviourModeRuleObject")]
		public List<BehaviourModeRuleObject> Rules{
			set{ m_Rules = value; }
			get{ return m_Rules; }
		}

		public List<BehaviourModeRuleObject> ValidRules{
			get{ 
				List<BehaviourModeRuleObject> _rules = new List<BehaviourModeRuleObject>();

				foreach( BehaviourModeRuleObject _rule in Rules )
				{
					if( _rule != null && _rule.Enabled == true )
						_rules.Add( _rule );	
				}

				return _rules; 
			
			}
		}

		public bool NextRule()
		{
			if( ValidRules == null || ValidRules.Count == 0 )
				return false;

			int new_rule_index = m_RuleIndex;
			
			if( ValidRules.Count == 1 )
			{
				new_rule_index = 0;
			}
			else if( Rule != null && Rule.Links.Enabled && Rule.Links.Link == LinkType.RULE && Rule.Links.RuleIndex < ValidRules.Count )
			{
				new_rule_index = Rule.Links.RuleIndex;
			}
			else if( RulesOrderType == SequenceOrderType.RANDOM )
			{
				new_rule_index = Random.Range(0, ValidRules.Count);
			}
			else 
			{
				if( RulesOrderType == SequenceOrderType.PINGPONG )
				{
					if( ! RulesOrderInverse && new_rule_index + 1 >= ValidRules.Count )
						RulesOrderInverse = true;
					else if( RulesOrderInverse && new_rule_index - 1 < 0 )
						RulesOrderInverse = false;
				}
				
				if( ! RulesOrderInverse )
				{
					new_rule_index++;
					
					if( new_rule_index >= ValidRules.Count )
						new_rule_index = 0;
				}
				else
				{
					new_rule_index--;
					
					if( new_rule_index < 0 )
						new_rule_index = ValidRules.Count - 1;
				}
			}
			
			return SetRuleByIndex( new_rule_index );
		}

		private bool SetRuleByIndex( int _index )
		{
			if( _index < 0 || _index >= ValidRules.Count )
				return false;

			BehaviourModeRuleObject new_rule = ValidRules[ _index ];

			if( m_Rule != new_rule || m_Rule == null )
			{
				m_LastRule = m_Rule;
				m_LastRuleIndex = m_RuleIndex;
				m_Rule = new_rule;
				m_RuleIndex = _index;

				m_RuleChanged = true;
			}
			else
				m_RuleChanged = false;

			return m_RuleChanged;
		}

		public void Update( GameObject _owner )
		{
			if( Rule != null )
			{
				Rule.Effect.Update( _owner );
			}
				
				
		}


	}


	//--------------------------------------------------
	// Animations
	//--------------------------------------------------
	[System.Serializable]
	public class BehaviourModeRuleObject : System.Object 
	{
		public BehaviourModeRuleObject(){
			Audio = new AudioDataObject();
			Inventory = new InventoryActionObject();
		}
		public BehaviourModeRuleObject( BehaviourModeRuleObject _rule )
		{
			Animation.Copy( _rule.Animation );

			Audio = new AudioDataObject( _rule.Audio );
			Inventory = new InventoryActionObject( _rule.Inventory );

			Effect = _rule.Effect;
			Foldout = _rule.Foldout;
			Influences = _rule.Influences;

			Key = _rule.Key;
			LengthMax = _rule.LengthMax;
			LengthMin = _rule.LengthMin;
			UseCustomLength = _rule.UseCustomLength;
			Links = _rule.Links;
			Move = _rule.Move;
			UseFootsteps = _rule.UseFootsteps;
		}



		public string Key = "";

		public bool Enabled = true;
		public bool Foldout = true;

		public bool UseFootsteps = false;

		public bool UseCustomLength = true;
		public float LengthMin = 0f;
		public float LengthMax = 0f;
		public float LengthMaximum = 20;


		public BodyContainer Body;
		public MoveContainer Move;
		public AnimationContainer Animation;
		public AudioDataObject Audio;			
		public StatusContainer Influences;
		public EffectContainer Effect;
		public LinkContainer Links;
		public InventoryActionObject Inventory;
	

		public bool MoveRequired
		{
			get{
				if( Move.Enabled && ( Move.Velocity.Velocity != Vector3.zero || Move.Velocity.AngularVelocity != 0 ) )
					return true;
				else
					return false;
			}
		}

		public bool UseRootMotion{
			get{ return Animation.UseRootMotion; }
	   	}

		/*
		private GameObject m_Effect = null;
		public void StartEffect( GameObject _owner ) 
		{
			if( _owner == null )
				return;

			if( Effect.ReferenceObject != null && Effect.Enabled == true )
			{
				Vector3 position = _owner.transform.position;

				if( Effect.OffsetType == RandomOffsetType.EXACT )
					position = GraphicTools.GetOffsetPosition( _owner.transform, Effect.Offset );
				else if( Effect.OffsetRadius > 0 )
				{
					Vector2 pos = Random.insideUnitCircle * Effect.OffsetRadius;

					position.x += pos.x;
					position.z += pos.y;

					if( Effect.OffsetType == RandomOffsetType.HEMISPHERE )
						position.y += Random.Range(0,Effect.OffsetRadius ); 
					else if( Effect.OffsetType == RandomOffsetType.SPHERE )
						position.y += Random.Range( - Effect.OffsetRadius , Effect.OffsetRadius ); 
				}


				if( m_Effect == null || Effect.Detach )
				{
					m_Effect = (GameObject)Object.Instantiate( Effect.ReferenceObject, position, _owner.transform.rotation);

					if( ! Effect.Detach )
						m_Effect.transform.parent = _owner.transform;
				}

				if( m_Effect != null )
					m_Effect.SetActive( true );
			}
		}

		public void StopEffect() 
		{
			if( m_Effect != null && Effect.Detach == false )
				m_Effect.SetActive( false );
		}

		public void UpdateEffect( GameObject _owner )
		{
			Effect.UpdateEffect( _owner );
		}*/
	}


	//--------------------------------------------------
	// Animations
	//--------------------------------------------------
	[System.Serializable]
	[XmlRoot("BehaviourObject")]
	public class BehaviourObject  : System.Object
	{
		private GameObject m_Owner = null;

		public void Init( GameObject _owner )
		{
			m_Owner = _owner;
			BehaviourAnimation.Init( m_Owner );

			for( int i = 0 ; i < BehaviourModes.Count ; i++ )
				m_BehaviourModesKeys.Add( BehaviourModes[i].Key, i );

		}

		public void Reset()
		{
			BehaviourModes.Clear();
		}

		public BehaviourModeObject CopyBehaviourMode( BehaviourModeObject _mode )
		{
			string _key = _mode.Key;

			_key = Regex.Replace( _key, "COPY_OF_", "" );

			_key = "COPY_OF_" + _key;

			if( BehaviourModeExists( _key ) )
			{
				int _index = 1;
				while( BehaviourModeExists( _key + "_" + _index ) )
				      _index++;

				_key += "_" + _index;
			}

			BehaviourModeObject _copy = new BehaviourModeObject( _mode );
			_copy.Key = _key;

			m_BehaviourModes.Add( _copy );

			return _copy;
		}

		public string GetFixedBehaviourModeKey( string _key )
		{
			return Regex.Replace( _key, "( )+", "").ToUpper(); 
		}

		public string AddBehaviourMode( string _key )
		{
			_key = GetFixedBehaviourModeKey( _key ); 
			
			if( _key != "" && ! BehaviourModeExists( _key ) )
			{
				BehaviourModeObject  _mode = new BehaviourModeObject ();
				_mode.Key = _key;
				_mode.Rules.Add( new BehaviourModeRuleObject() );
				
				m_BehaviourModes.Add( _mode );
			}
			
			return _key;
		}

		public bool BehaviourModeExists( string _key )
		{
			foreach( BehaviourModeObject  _mode in m_BehaviourModes )
			{
				if( _mode.Key == _key )
					return true;
			}
			
			return false;
		}

		//--------------------------------------------------
		// Animation
		//--------------------------------------------------
		private AnimationObject m_BehaviourAnimation = null;
		public AnimationObject BehaviourAnimation
		{
			get{
				if( m_BehaviourAnimation == null )
					m_BehaviourAnimation = new AnimationObject( m_Owner );
				
				return m_BehaviourAnimation;
			}
		}

		private AudioObject m_BehaviourAudio = null;
		public AudioObject BehaviourAudio
		{
			get{
				if( m_BehaviourAudio == null )
					m_BehaviourAudio = new AudioObject( m_Owner );
				
				return m_BehaviourAudio;
			}
		}

		private float m_BehaviourTimer = 0.0f;

		private string m_LastBehaviourModeKey = "";
		private string m_ActiveBehaviourModeKey = "";

		//private int m_LastBehaviourModeIndex = 0;
		private int m_ActiveBehaviourModeIndex = 0;

		private BehaviourModeObject m_LastBehaviourMode = null;
		private BehaviourModeObject m_ActiveBehaviourMode = null;
		public BehaviourModeObject BehaviourMode{
			get{return m_ActiveBehaviourMode;}
		}

		public BehaviourModeRuleObject ActiveBehaviourModeRule{
			get{
				if( m_ActiveBehaviourMode != null )
					return m_ActiveBehaviourMode.Rule;
				else
					return null;
			}
		}

		private bool ActiveBehaviourModeRuleHasModeLink{
			get{
				if( m_ActiveBehaviourMode != null && 
				   m_ActiveBehaviourMode.Rule != null && 
				   m_ActiveBehaviourMode.Rule.Links.Enabled && 
				   m_ActiveBehaviourMode.Rule.Links.Link == LinkType.MODE &&
				   m_ActiveBehaviourMode.Rule.Links.BehaviourModeKey != "" ) 
					return true;
				else
					return false;
			}
		}


		//private BehaviourModeRuleObject m_LastBehaviourModeRule = null;
		//private int m_LastBehaviourModeRuleIndex = 0;

		//private BehaviourModeRuleObject m_BehaviourModeRule = null;
		//private int m_BehaviourModeRuleIndex = 0;


		private float m_BehaviourModeRuleTimer = 0.0f;
		private float m_BehaviourModeRuleLength = 0.0f;

		private bool m_BehaviourModeChanged = false;
		private bool m_BehaviourModeRulesChanged = false;
		private bool m_BehaviourModeRuleChanged = false;

		private bool m_BehaviourModeValid = true;
		private bool BehaviourModeValid{
			get{return m_BehaviourModeValid; }
		}

		private bool m_BehaviourModeKeyValid = true;
		private bool BehaviourModeKeyValid{
			get{return m_BehaviourModeKeyValid; }
		}

		[SerializeField]
		private List<BehaviourModeObject > m_BehaviourModes = new List<BehaviourModeObject >();
		private Dictionary<string, int> m_BehaviourModesKeys = new Dictionary<string, int>();

		//--------------------------------------------------
		// BehaviourModes
		//--------------------------------------------------

		[XmlArray("BehaviourModes"),XmlArrayItem("BehaviourModeObject")]
		public List<BehaviourModeObject> BehaviourModes{
			set{ m_BehaviourModes = value; }
			get{ return m_BehaviourModes; }
		}


		//--------------------------------------------------
		public bool BehaviourModeRulesChanged{
			get{ return m_BehaviourModeRulesChanged; }
		}

		//--------------------------------------------------
		// BehaviourMode
		//--------------------------------------------------

		public BehaviourModeObject GetBehaviourModeByKey( string key )
		{
			if( key == null || key.Trim() == "" )
				return null;

			foreach( BehaviourModeObject _mode in BehaviourModes )
			{
				if( _mode.Key == key )
					return _mode;
			}

			return null;
		}

		/// <summary>
		/// Sets the behaviour mode by key.
		/// </summary>
		///<description>Use this function to change the behaviour of your creature.</description>
		/// <param name="key">Key.</param>
		public void SetBehaviourModeByKey( string _key )
		{

			if( _key == "" )
			{
				if( m_BehaviourModeKeyValid )
					Debug.LogWarning( "CAUTION : THE CREATURE '" + m_Owner.gameObject.name.ToUpper() + "' HAVE NO BEHAVIOUR!");
				m_BehaviourModeKeyValid = false;
				return;
			}
			else
				m_BehaviourModeKeyValid = true;

			if( BehaviourModeKey != _key || m_ActiveBehaviourMode == null || m_ActiveBehaviourMode.Rules == null || m_ActiveBehaviourMode.Rule == null )
			{
				m_LastBehaviourModeKey = m_ActiveBehaviourModeKey;
				m_LastBehaviourMode = m_ActiveBehaviourMode;
				//m_LastBehaviourModeIndex = m_ActiveBehaviourModeIndex;

				m_ActiveBehaviourModeKey = _key; 

				if( m_BehaviourModesKeys.TryGetValue( m_ActiveBehaviourModeKey, out m_ActiveBehaviourModeIndex ) )
					m_ActiveBehaviourMode = BehaviourModes[m_ActiveBehaviourModeIndex];

				if( m_LastBehaviourMode != null )
					m_LastBehaviourMode.SetActive( false );

				if( m_ActiveBehaviourMode != null )
					m_ActiveBehaviourMode.SetActive( true );

				m_BehaviourTimer = 0;

				m_BehaviourModeChanged = true;
				m_BehaviourModeRulesChanged = true;

				NextBehaviorModeRule( true );
			
				return;
			}
		}

		public void NextBehaviorModeRule( bool _forced = false )
		{
			if( m_ActiveBehaviourMode == null )
			{
				if( m_BehaviourModeValid )
					Debug.LogWarning( "CAUTION : INVALID BEHAVIOURMODE '" + BehaviourModeKey + "' AT CREATURE '" + m_Owner.gameObject.name.ToUpper() + "'!");

				m_BehaviourModeValid = false;
		
				return;
			}
			else
				m_BehaviourModeValid = true;
		

			if( m_ActiveBehaviourMode.NextRule() || _forced )
			{
				BehaviourAnimation.Play( m_ActiveBehaviourMode.Rule );
				BehaviourAudio.Play( m_ActiveBehaviourMode.Rule.Audio );
				
				if( m_ActiveBehaviourMode.LastRule != null )
					m_ActiveBehaviourMode.LastRule.Effect.StopEffect();
				
				if( m_ActiveBehaviourMode.Rule != null )
					m_ActiveBehaviourMode.Rule.Effect.StartEffect( m_Owner );
				
				m_BehaviourModeRuleLength = m_ActiveBehaviourMode.RuleLength();
				m_BehaviourModeRuleTimer = 0;

				m_BehaviourModeRuleChanged = m_ActiveBehaviourMode.RuleChanged;
			}
	
				
		}

		//--------------------------------------------------
		// BEHAVIOR MODE KEYS
		//--------------------------------------------------

		/// <summary>
		/// Returns the key of the current behaviour mode.
		/// </summary>
		/// <value>The behaviour mode key.</value>
		public string BehaviourModeKey
		{
			get{ return m_ActiveBehaviourModeKey; }
		} 

		//--------------------------------------------------

		/// <summary>
		/// Returns the key of the last behaviour mode.
		/// </summary>
		/// <value>The last behaviour mode key.</value>
		public string LastBehaviourModeKey{
			get{ return m_LastBehaviourModeKey; }
		}


		//--------------------------------------------------
		public bool BehaviourModeChanged{
			get{ return m_BehaviourModeChanged; }
		}

		//--------------------------------------------------
		public bool BehaviourModeRuleChanged{
			get{ return m_BehaviourModeRuleChanged; }
		}

		//--------------------------------------------------
		// Timer
		//--------------------------------------------------
		public float BehaviourTimer{
			get{ return m_BehaviourTimer; }
		}

		//--------------------------------------------------
		// Update
		//--------------------------------------------------
		public void UpdateBegin()
		{
			m_BehaviourModeChanged = false;					
			m_BehaviourModeRulesChanged = false;
			m_BehaviourModeRuleChanged = false;

			m_BehaviourTimer += Time.deltaTime;
			m_BehaviourModeRuleTimer += Time.deltaTime;

			if( BehaviourMode != null )
			{
				if( m_BehaviourModeRuleTimer >= m_BehaviourModeRuleLength )
				{
					if( ActiveBehaviourModeRuleHasModeLink )
						SetBehaviourModeByKey( BehaviourMode.Rule.Links.BehaviourModeKey );
					else if( BehaviourMode.Rules.Count > 1 )
						NextBehaviorModeRule();
				}

				BehaviourMode.Update( m_Owner );
			}

			if( BehaviourMode != null && BehaviourMode.Rule != null )
				BehaviourAnimation.UpdateBegin( BehaviourMode.Rule );

		}
	}
}
