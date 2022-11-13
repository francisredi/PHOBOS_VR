// ##############################################################################
//
// ice_CreatureTarget.cs
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
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Attributes;
using ICE.Utilities;

namespace ICE.Creatures.Objects
{

	[System.Serializable]
	public class TargetSelectorStatementObject : System.Object
	{
		public TargetSelectorStatementObject(){}
		public TargetSelectorStatementObject( TargetSelectorStatementObject _statement )
		{
			StatementType = _statement.StatementType;
			
			SuccessorType = _statement.SuccessorType;
			SuccessorTargetTag = _statement.SuccessorTargetTag;
			SuccessorTargetName = _statement.SuccessorTargetName;
			SuccessorTargetType = _statement.SuccessorTargetType;

			Priority = _statement.Priority;
			PriorityMultiplier = _statement.PriorityMultiplier;
		}

		public int Priority = 0;
		public float PriorityMultiplier = 0;

		public TargetSelectorStatementType StatementType = TargetSelectorStatementType.NONE;

		public TargetSuccessorType SuccessorType = TargetSuccessorType.TYPE;
		public string SuccessorTargetTag = "";
		public string SuccessorTargetName = "";
		public TargetType SuccessorTargetType = TargetType.UNDEFINED;
	}


	[System.Serializable]
	public class TargetSelectorConditionObject : System.Object
	{
		public TargetSelectorConditionObject(){}
		public TargetSelectorConditionObject( ConditionalOperatorType _condition_type ){
			ConditionType = _condition_type;
		}

		public TargetSelectorConditionObject( ConditionalOperatorType _condition_type, TargetSelectorExpressionType _expression_type ){

			ConditionType = _condition_type;
			ExpressionType = _expression_type;
		}

		public TargetSelectorConditionObject( TargetSelectorConditionObject _selector )
		{
			if( _selector == null )
				return;

			IsValid = false;
			Enabled = _selector.Enabled;
			ExpressionType = _selector.ExpressionType;
			UseDynamicValue = _selector.UseDynamicValue;
			ExpressionValue = _selector.ExpressionValue;
			ConditionType = _selector.ConditionType;
			Operator = _selector.Operator;
			
			FloatValue = _selector.FloatValue;
			StringValue = _selector.StringValue;
			PositionType = _selector.PositionType;
			PositionVector = _selector.PositionVector;

			PrecursorTargetTag = _selector.PrecursorTargetTag;
			PrecursorTargetName = _selector.PrecursorTargetName;
			PrecursorTargetType = _selector.PrecursorTargetType;

		}

		public bool Enabled = true;

		public SelectionStatus Status = SelectionStatus.UNCHECKED;
		public void ResetStatus()
		{
			Status = SelectionStatus.UNCHECKED;
			m_IsValid = false;
		}

		private bool m_IsValid = false;
		public bool IsValid{
			set{
				m_IsValid = value;

				if( m_IsValid )
					Status = SelectionStatus.VALID;
				else
					Status = SelectionStatus.INVALID; 
			}
			get{ return m_IsValid; }
		}
	

		[SerializeField]
		private string m_ExpressionTypeKey = TargetSelectorExpressionType.OwnFitness.ToString();
		private TargetSelectorExpressionType m_ExpressionType = TargetSelectorExpressionType.OwnFitness;
		public TargetSelectorExpressionType ExpressionType{
			set{ m_ExpressionTypeKey = TargetSelectorExpression.TypeToStr( value ); }
			get{ return TargetSelectorExpression.Type( ref m_ExpressionType, m_ExpressionTypeKey ); }
		}

		[SerializeField]
		private string m_ExpressionValueKey = TargetSelectorExpressionType.OwnFitness.ToString();
		private TargetSelectorExpressionType m_ExpressionValue = TargetSelectorExpressionType.OwnFitness;
		public TargetSelectorExpressionType ExpressionValue{
			set{ m_ExpressionValueKey = TargetSelectorExpression.TypeToStr( value ); }
			get{ return TargetSelectorExpression.Type( ref m_ExpressionValue, m_ExpressionValueKey ); }
		}




		public ConditionalOperatorType ConditionType = ConditionalOperatorType.AND;	
		public LogicalOperatorType Operator = LogicalOperatorType.EQUAL;

		public float FloatValue = 0;
		public int IntegerValue = 0;
		public string StringValue = "";
		public KeyCode KeyCodeValue = KeyCode.Escape;
		public bool BooleanValue = false;
		public TargetSelectorPositionType PositionType = TargetSelectorPositionType.TargetMovePosition;
		public Vector3 PositionVector = Vector3.zero;

		public string PrecursorTargetTag = "";
		public string PrecursorTargetName = "";
		public TargetType PrecursorTargetType = TargetType.UNDEFINED;
		public bool UseDynamicValue = false;

		public string ConditionToString()
		{
			string _condition = "";
			switch( ConditionType )
			{
				case ConditionalOperatorType.OR:
					_condition = "OR";
					break;
				default:
					_condition = "AND";
					break;
			}

			return _condition;
		}

		public string OperatorToString()
		{
			string _operator = "";

			if( TargetSelectorExpression.NeedLogicalOperator( ExpressionType ) )
			{
				switch( Operator )
				{
					case LogicalOperatorType.EQUAL:
						_operator = "==";
						break;
					case LogicalOperatorType.NOT:
						_operator = "!=";
						break;
					case LogicalOperatorType.LESS:
						_operator = "<";
						break;
					case LogicalOperatorType.LESS_OR_EQUAL:
						_operator = "<=";
						break;				
					case LogicalOperatorType.GREATER:
						_operator = ">";
						break;
					case LogicalOperatorType.GREATER_OR_EQUAL:
						_operator = ">=";
						break;
				}
			}
			else
			{
				switch( Operator )
				{
					case LogicalOperatorType.NOT:
						_operator = "IS NOT";
						break;
					default:
						_operator = "IS";
						break;
				}
			}

			return _operator;
		}

		public bool ComparePrecursorTarget( TargetObject _precursor )
		{
			if( _precursor == null || _precursor.TargetGameObject == null )
				return false;

			bool _result = false;

			// using EQUAL to get true if one of the conditions are correct 
			if( ExpressionType == TargetSelectorExpressionType.PreviousTargetType && PrecursorTargetType == _precursor.Type )
				_result = true;
			else if( ExpressionType == TargetSelectorExpressionType.PreviousTargetName && PrecursorTargetName == _precursor.TargetGameObject.name )
				_result = true;
			else if( ExpressionType == TargetSelectorExpressionType.PreviousTargetTag && _precursor.TargetGameObject.CompareTag( PrecursorTargetTag ) )
				_result = true;

			// if the desired operator is NOT we have to invert the result of the EQUAL check
			if( Operator == LogicalOperatorType.NOT )
				_result = ! _result;
		
			return _result;
		}
		/*
		public bool CompareSuccessorTarget( TargetObject _successor )
		{
			if( _successor == null || _successor.TargetGameObject == null )
				return false;
			
			if( SuccessorType == TargetSuccessorType.TYPE && SuccessorTargetType == _successor.Type )
				return true;
			else if( SuccessorType == TargetSuccessorType.NAME && SuccessorTargetName == _successor.TargetGameObject.name )
				return true;
			else if( SuccessorType == TargetSuccessorType.TAG && _successor.TargetGameObject.CompareTag( SuccessorTargetTag ) )
				return true;
			else
				return false;
		}*/
	}

	[System.Serializable]
	public class TargetSelectorObject : System.Object
	{
		public void ResetStatus()
		{
			Status = SelectionStatus.UNCHECKED;
			m_IsValid = false;
		}

		public SelectionStatus Status = SelectionStatus.UNCHECKED;
		private bool m_IsValid = false;
		public bool IsValid{
			set{
				m_IsValid = value;

				if( m_IsValid )
					Status = SelectionStatus.VALID;
				else
					Status = SelectionStatus.INVALID; 
			}
			get{ return m_IsValid; }
		}
		public TargetSelectorObject(){}
		public TargetSelectorObject( ConditionalOperatorType _condition_type )
		{
			m_Conditions.Add( new TargetSelectorConditionObject( _condition_type, TargetSelectorExpressionType.OwnFitness ) );
		}

		public TargetSelectorObject( TargetSelectorObject _selector )
		{
			if( _selector == null )
				return;

			m_Conditions.Clear();
			foreach( TargetSelectorConditionObject _condition in _selector.Conditions )
				m_Conditions.Add( new TargetSelectorConditionObject( _condition ) );

			m_Statements.Clear();
			foreach( TargetSelectorStatementObject _statement in _selector.Statements )
				m_Statements.Add( new TargetSelectorStatementObject( _statement ) );
		}

		public ConditionalOperatorType InitialOperatorType{
			get{
				if( m_Conditions.Count > 0 )
					return m_Conditions[0].ConditionType;
				else
					return ConditionalOperatorType.AND;
			}
		}
		
		[SerializeField]
		private List<TargetSelectorConditionObject> m_Conditions = new List<TargetSelectorConditionObject>();
		public List<TargetSelectorConditionObject> Conditions{
			set{ m_Conditions = value;}
			get{return m_Conditions; }
		}

		[SerializeField]
		private List<TargetSelectorStatementObject> m_Statements = new List<TargetSelectorStatementObject>();
		public List<TargetSelectorStatementObject> Statements{
			set{ m_Statements = value;}
			get{return m_Statements; }
		}
	}

	[System.Serializable]
	public class TargetSelectorsObject : System.Object
	{
		public TargetSelectorsObject(){}
		public TargetSelectorsObject( TargetSelectorsObject _selectors ){
			Copy( _selectors );
		}

		public TargetSelectorsObject( TargetType _type ){
			this.m_TargetType = _type;
		}

		private TargetType m_TargetType = TargetType.UNDEFINED; 
		public TargetType TargetType{
			get{return m_TargetType;}
		}

		public bool Foldout = true;

		public bool UseSelectionCriteriaForHome;
		public bool UseVisibilityCheck;

		public int Priority = 0;
		public int DefaultPriority = 0;

		public float SelectionRange = 0;
		public float SelectionRangeMax = 0;
		public bool UseFieldOfView = false;
		public float SelectionAngle = 0;
		public bool CanUseDefaultPriority = false;
		public bool UseDefaultPriority = false;
		public bool UseAdvanced = false;

		public bool TotalCheckIsValid = false;


		public void ResetStatus()
		{
			Status = SelectionStatus.UNCHECKED;
			m_IsValid = false;
			foreach( TargetSelectorObject _selector in Selectors )
			{
				_selector.ResetStatus();
				foreach( TargetSelectorConditionObject _condition in _selector.Conditions )
					_condition.ResetStatus();
			}
		}

		public SelectionStatus Status = SelectionStatus.UNCHECKED;
		private bool m_IsValid = false;
		public bool IsValid{
			set{
				m_IsValid = value;

				if( m_IsValid )
					Status = SelectionStatus.VALID;
				else
					Status = SelectionStatus.INVALID; 
			}
			get{ return m_IsValid; }
		}

		private float m_DynamicPriority = 0;
		public float DynamicPriority{
			get{ return UpdateRelevance( 0 ); }
		}

		private float m_RelevanceMultiplier = 0;
		public float RelevanceMultiplier{
			get{ 
				if( m_RelevanceMultiplier > 1 )
					m_RelevanceMultiplier = 1;				
				else if( m_RelevanceMultiplier < -1 )
					m_RelevanceMultiplier = -1;

				return m_RelevanceMultiplier; 			
			}
		}

		public void ResetRelevanceMultiplier(){
			m_RelevanceMultiplier = 0;
		}

		public ConditionalOperatorType InitialOperatorType{
			get{
				if( m_SelectorGroups.Count > 0 )
					return m_SelectorGroups[0].InitialOperatorType;
				else
					return ConditionalOperatorType.AND;
			}
		}

		public void SetRelevanceMultiplier( float _relevance_multiplier ){
			m_RelevanceMultiplier = _relevance_multiplier;
		}


		[SerializeField]
		private List<TargetSelectorObject> m_SelectorGroups = new List<TargetSelectorObject>();
		public List<TargetSelectorObject> Selectors{
			set{ m_SelectorGroups = value;}
			get{return m_SelectorGroups; }
		}

		[SerializeField]
		private List<TargetSelectorStatementObject> m_Statements = new List<TargetSelectorStatementObject>();
		public List<TargetSelectorStatementObject> Statements{
			set{ m_Statements = value;}
			get{return m_Statements; }
		}

		public float UpdateRelevance( float _relevance_multiplier )
		{
			m_RelevanceMultiplier += _relevance_multiplier;

			m_DynamicPriority = Priority + ( Priority * RelevanceMultiplier );

			if( m_DynamicPriority > 100 )
				m_DynamicPriority = 100;
			
			if( m_DynamicPriority < 0 )
				m_DynamicPriority = 0;

			return m_DynamicPriority;
		}

		public float CompareSuccessorTargets( TargetObject _target, Vector3 _position )
		{
			/*
			if( _target == null || _target.TargetGameObject == null || UseAdvanced == false )
				return 0;

			float _relevance_multiplier = 0;

			foreach( TargetSelectorObject _group in m_SelectorGroups )
			{
				foreach( TargetSelectorConditionObject _selector in _group.Conditions )
				{
					if( _selector.Enabled == true && _selector.ExpressionType == TargetSelectorExpressionType.SUCCESSOR )
					{
						bool _valid = false;

						if( ( _selector.SuccessorType == TargetSuccessorType.NAME && _target.TargetGameObject.name == _selector.SuccessorTargetName ) ||
							( _selector.SuccessorType == TargetSuccessorType.TAG && _target.TargetGameObject.CompareTag( _selector.SuccessorTargetName ) ) ||
							( _selector.SuccessorType == TargetSuccessorType.TYPE && _target.Type == _selector.SuccessorTargetType ) )
						{
							if(  _selector.UseRange )
								_valid = _target.TargetInSelectionRange( _position, _selector.Distance );
							else
								_valid = true;
						}

						if( _valid == _selector.Included )
							_valid = true;
						else
							_valid = false;

						if( _valid )
						{
							if( _selector.UseMultiplier )
								_relevance_multiplier += _selector.RelevanceMultiplier;
							else
								_relevance_multiplier += 1;
						}
					}
				}
			}
			_target.Selectors.UpdateRelevance( _relevance_multiplier );

			return _relevance_multiplier;*/
			return 0;

		}
		/*
		public float SelectionRange{
			set{
				if( UseComplex )
					AddSelectionRange( value );
				else
					m_SelectionRange = value;
			}
			get{
				if( UseComplex )
					return GetFirstRangeSelectorValue();
				else
					return m_SelectionRange;
			}
		}
		
		public void AddSelectionRange( float _distance )
		{
			if( _distance == 0 )
				return;

			TargetSelectorObject _range_selector = GetFirstRangeSelector();
			
			if( _range_selector == null )
			{
				_range_selector = new TargetSelectorObject( TargetSelectorType.RANGE );
				m_Selectors.Add( _range_selector );
			}
			
			_range_selector.Range = _distance;
		}

		private TargetSelectorObject GetMaxRangeSelector()
		{
			TargetSelectorObject _range_selector = null;
			foreach( TargetSelectorObject _selector in m_Selectors )
			{
				if( _selector.Type == TargetSelectorType.RANGE && ( _range_selector == null ||_selector.Range >= _range_selector.Range ) )
					_range_selector = _selector;
			}
			
			return _range_selector;
		}

		private TargetSelectorObject GetFirstRangeSelector()
		{
			TargetSelectorObject _range_selector = null;

			for( int i = 0; i < m_Selectors.Count ; i++ )
			{
				if( m_Selectors[i] != null && m_Selectors[i].Type == TargetSelectorType.RANGE )
					return  m_Selectors[i];
			}

			return null;
		}

		private float GetFirstRangeSelectorValue()
		{
			TargetSelectorObject _selector = GetFirstRangeSelector();

			if( _selector != null )
				return _selector.Range;
			else
				return 0;
		}*/




		public int GetPriority( TargetType _type )
		{
			if( _type == TargetType.HOME && UseSelectionCriteriaForHome == false )
				return GetDefaultPriorityByType( _type );
			else if( UseAdvanced )
				return (int)DynamicPriority;
			else
				return Priority;
		}

		public int GetDefaultPriorityByType( TargetType _type )
		{
			int _priority = 0;
			if( _type == TargetType.HOME )
				_priority = 0;
			else if( _type == TargetType.INTERACTOR )
				_priority = 60;
			else if( _type == TargetType.PATROL )
				_priority = 50;
			else if( _type == TargetType.WAYPOINT )
				_priority = 50;
			else if( _type == TargetType.OUTPOST )
				_priority = 50;
			else if( _type == TargetType.ESCORT )
				_priority = 55;
			
			return _priority;
		}
		
		public float GetDefaultRangeByType( TargetType _type )
		{
			float _range = 0;
			if( _type == TargetType.HOME )
				_range = 0;	
			else if( _type == TargetType.INTERACTOR )
				_range = 20;
			else if( _type == TargetType.PATROL )
				_range = 0;
			else if( _type == TargetType.WAYPOINT )
				_range = 0;
			else if( _type == TargetType.OUTPOST )
				_range = 0;
			else if( _type == TargetType.ESCORT )
				_range = 0;
			
			return _range;
		}

		public float GetDefaultAngleByType( TargetType _type )
		{
			float _angle = 0;
			if( _type == TargetType.HOME )
				_angle = 0;	
			else if( _type == TargetType.INTERACTOR )
				_angle = 50;
			else if( _type == TargetType.PATROL )
				_angle = 0;
			else if( _type == TargetType.WAYPOINT )
				_angle = 0;
			else if( _type == TargetType.OUTPOST )
				_angle = 0;
			else if( _type == TargetType.ESCORT )
				_angle = 0;
			
			return _angle;
		}

		public void Copy( TargetSelectorsObject _selectors )
		{
			if( _selectors == null )
				return;

			UseSelectionCriteriaForHome = _selectors.UseSelectionCriteriaForHome;
			//CanUseDefaultPriority = _selectors.CanUseDefaultPriority;

			Priority = _selectors.Priority;
			UseFieldOfView = _selectors.UseFieldOfView;
			UseVisibilityCheck = _selectors.UseVisibilityCheck;
			SelectionRange = _selectors.SelectionRange;
			SelectionAngle = _selectors.SelectionAngle;
			DefaultPriority = _selectors.DefaultPriority;
			UseDefaultPriority = _selectors.UseDefaultPriority;
			UseAdvanced = _selectors.UseAdvanced;
			SetRelevanceMultiplier( _selectors.RelevanceMultiplier );

			Selectors.Clear();
			foreach( TargetSelectorObject _selector in _selectors.Selectors )
				Selectors.Add( new TargetSelectorObject( _selector ) );
		}

	}

	[System.Serializable]
	public class TargetMoveObject : System.Object
	{
		public TargetMoveObject(){}
		public TargetMoveObject( TargetMoveObject _data ){
			Copy( _data );
		}
		
		public void Copy( TargetMoveObject _data )
		{
			Enabled = _data.Enabled;
			Foldout = _data.Foldout;

			Offset = _data.Offset;

			StopDistance = _data.StopDistance;
			StopDistanceZoneRestricted = _data.StopDistanceZoneRestricted;
			IgnoreLevelDifference = _data.IgnoreLevelDifference;

			RandomRange = _data.RandomRange;
			MaxRandomRange = _data.MaxRandomRange;

			SmoothingMultiplier = _data.SmoothingMultiplier;

			UseUpdateOffsetOnActivateTarget = _data.UseUpdateOffsetOnActivateTarget;
			UseUpdateOffsetOnMovePositionReached = _data.UseUpdateOffsetOnMovePositionReached;
			UseUpdateOffsetOnRandomizedTimer = _data.UseUpdateOffsetOnRandomizedTimer;

			OffsetUpdateTimeMin = _data.OffsetUpdateTimeMin;
			OffsetUpdateTimeMax = _data.OffsetUpdateTimeMax;

			UseDynamicOffsetAngle = _data.UseDynamicOffsetAngle;
			UseRandomOffsetAngle= _data.UseRandomOffsetAngle;
			MinOffsetAngle = _data.MinOffsetAngle;
			MaxOffsetAngle = _data.MaxOffsetAngle;
			DynamicOffsetAngleUpdateSpeed = _data.DynamicOffsetAngleUpdateSpeed;
			
			UseDynamicOffsetDistance = _data.UseDynamicOffsetDistance;
			UseRandomOffsetDistance = _data.UseRandomOffsetDistance;
			MinOffsetDistance = _data.MinOffsetDistance;
			MaxOffsetDistance = _data.MaxOffsetDistance;
			DynamicOffsetDistanceUpdateSpeed = _data.DynamicOffsetDistanceUpdateSpeed;
		}

		public bool Enabled = true;
		public bool Foldout = true;

		public Vector3 Offset = Vector3.zero;

		public float RandomRange = 0;
		public float MaxRandomRange = 250;

		public float StopDistance = 2;
		public bool StopDistanceZoneRestricted = false;
		public bool IgnoreLevelDifference = true;

		public float SmoothingMultiplier = 0;

		public bool UseUpdateOffsetOnActivateTarget = false;
		public bool UseUpdateOffsetOnMovePositionReached = false;
		public bool UseUpdateOffsetOnRandomizedTimer = false;

		public float OffsetUpdateTimeMin = 5;
		public float OffsetUpdateTimeMax = 15;
		
		public bool UseDynamicOffsetAngle = false;
		public bool UseRandomOffsetAngle= false;
		public float MinOffsetAngle = 0;
		public float MaxOffsetAngle = 360;
		public float DynamicOffsetAngleUpdateSpeed = 0.05f;
		
		public bool UseDynamicOffsetDistance = false;
		public bool UseRandomOffsetDistance = false;
		public float MinOffsetDistance = 5;
		public float MaxOffsetDistance = 15;
		public float DynamicOffsetDistanceUpdateSpeed = 0.05f;


		/*
		UpdateOffset( _target.Move.Offset );
		TargetStopDistance = _target.TargetStopDistance;
		TargetIgnoreLevelDifference = _target.TargetIgnoreLevelDifference;
		TargetStopDistanceZoneRestricted = _target.TargetStopDistanceZoneRestricted;
		TargetRandomRange = _target.TargetRandomRange;
		TargetSmoothingMultiplier = _target.TargetSmoothingMultiplier;

		 */
	}

	//--------------------------------------------------
	// TargetDataObject is the data container for TargetObject
	//--------------------------------------------------
	[System.Serializable]
	public class TargetDataObject : System.Object
	{
		public TargetDataObject(){}
		public TargetDataObject( TargetType _type ){
			m_Type = _type;
		}
		public TargetDataObject( TargetDataObject _data ){
			Copy( _data );
		}

		public void Copy( TargetDataObject _data )
		{
			if( _data == null )
				return;

			InfoText = _data.InfoText;
			BehaviourModeKey = _data.BehaviourModeKey;

			IsPrefab = _data.IsPrefab;

			m_TargetName = _data.TargetName;
			m_TargetTag = _data.TargetTag;
			m_TargetGameObject = _data.TargetGameObject;
			AccessType = _data.AccessType;

			GroupMessage = _data.GroupMessage;

			Influences = _data.Influences;

			Selectors.Copy( _data.Selectors );
			Move.Copy( _data.Move );
		}

		public TargetAccessType AccessType = TargetAccessType.OBJECT;

		[SerializeField]
		private string m_TargetTag = "";
		public string TargetTag{
			set{
				if( AccessType == TargetAccessType.TAG )
					SetTargetByTag( value );
			}
			get{ 

				if( m_TargetTag == "" && m_TargetGameObject != null )
					m_TargetTag = m_TargetGameObject.tag;
				
				return m_TargetTag; 
			}
		}

		[SerializeField]
		private string m_TargetName = "";
		public string TargetName{
			set{
				if( AccessType == TargetAccessType.NAME )
					SetTargetByName( value );
			}
			get{ 

				if( m_TargetName == "" && m_TargetGameObject != null )
					m_TargetName = m_TargetGameObject.name;
				
				return m_TargetName; 
			}
		}

		[SerializeField]
		public string TargetParentName{
			get{ 
				if( m_TargetGameObject != null && m_TargetGameObject.transform.parent != null )
					return m_TargetGameObject.transform.parent.name;
				else
					return "";
			}
		}

		[SerializeField]
		public bool TargetHasParent{
			get{ 
				if( m_TargetGameObject != null && m_TargetGameObject.transform.parent != null )
					return true;
				else
					return false;
			}
		}


		private int m_LastTargetID = 0;
		public int LastTargetID{
			get{ return m_LastTargetID; }
		}
		private int m_TargetID = 0;
		public int TargetID{
			get{return m_TargetID;}
		}

		[SerializeField]
		private GameObject m_TargetGameObject = null;
		public  GameObject TargetGameObject{
			get{ return m_TargetGameObject; }
		}

		public void SetTargetByTag( string _tag, GameObject _owner = null )
		{
			if( _tag == null || _tag == "" || _tag == "Undefined" )
				return;

			AccessType = TargetAccessType.TAG;
			m_TargetTag = _tag;

			if( ICECreatureRegister.Instance != null )
			{
				if( Application.isPlaying == false )
					OverrideTargetGameObject( ICECreatureRegister.Instance.GetReferenceGameObjectByTag( m_TargetTag ) );
				else if( _owner != null )
					OverrideTargetGameObject( ICECreatureRegister.Instance.FindNearestTargetByTag( _owner, m_TargetTag, Selectors.SelectionRange ) );
			}
		}

		public void SetTargetByName( string _name, GameObject _owner = null )
		{
			AccessType = TargetAccessType.NAME;
			m_TargetName = _name;

			if( m_TargetName.Trim() != "" )
			{
				if( Application.isPlaying == false && ICECreatureRegister.Instance != null )
					OverrideTargetGameObject( ICECreatureRegister.Instance.GetReferenceGameObjectByName( m_TargetName ) );
				else if( _owner != null )
					OverrideTargetGameObject( ICECreatureRegister.Instance.FindNearestTargetByName( _owner, m_TargetName, Selectors.SelectionRange ) );
			}
			else
			{
				m_TargetGameObject = null;
				m_TargetName = "";
				m_TargetTag = "";	
			}
		}

		public void SetTargetByGameObject( GameObject _object )
		{
			if( m_TargetGameObject != _object )
				Selectors.ResetStatus();
			
			m_TargetGameObject = _object;

			if( m_TargetGameObject != null )
			{
				AccessType = TargetAccessType.OBJECT;
				m_TargetName = m_TargetGameObject.name;
				m_TargetTag = m_TargetGameObject.tag;					

				if( ICECreatureRegister.Instance != null )
					ICECreatureRegister.Instance.AddReference( m_TargetGameObject );
			}
			else
			{
				m_TargetName = "";
				m_TargetTag = "Untagged";
			}	
		}

		public void OverrideTargetGameObject( GameObject _object )
		{
			if( _object == null )
				return;
			
			m_TargetGameObject = _object;
			m_TargetName = m_TargetGameObject.name;
			m_TargetTag = m_TargetGameObject.tag;	
			m_TargetID = m_TargetGameObject.GetInstanceID();

			if( m_TargetID != m_LastTargetID )
			{
				m_LastTargetID = m_TargetID;
				Selectors.ResetStatus();
			}
		}

		public void ResetTargetGameObject()
		{
			m_TargetGameObject = null;
		}

		public string TargetTitle{
			get{

				if( AccessType == TargetAccessType.TAG )
					return TargetTag;
				else if( AccessType == TargetAccessType.NAME )
					return TargetName;
				else if( TargetGameObject != null )
					return TargetGameObject.name;
				else
					return "INVALID";
			}
		}

		public StatusContainer Influences;

		[SerializeField]
		private TargetType m_Type = TargetType.UNDEFINED;
		public TargetType Type{
			get{ return m_Type; }
		}

		public bool ShowInfoText = false;
		public string InfoText = "";
		public string BehaviourModeKey = "";

		public bool UseLastKnownPosition = false;
		private Vector3 m_LastKnownPosition = Vector3.zero;
		public Vector3 LastKnownPosition{
			get{ return m_LastKnownPosition;}
		}

		[SerializeField]
		private BroadcastMessageObject m_GroupMessage = new BroadcastMessageObject();
		public BroadcastMessageObject GroupMessage{
			set{ m_GroupMessage = value;}
			get{ return m_GroupMessage;}
		}
		public BroadcastMessageObject LastReceivedGroupMessage;

		public bool IsPrefab = false;

		[SerializeField]
		private TargetSelectorsObject m_Selectors = new TargetSelectorsObject();
		public TargetSelectorsObject Selectors{
			set{ m_Selectors = value;}
			get{ return m_Selectors;}
		}

		[SerializeField]
		private TargetMoveObject m_Move = new TargetMoveObject();
		public TargetMoveObject Move{
			set{ m_Move = value;}
			get{ return m_Move;}
		}
	}

	//--------------------------------------------------
	// TargetObject is the data container for all potential targets (home, escort, defender, attacker)
	//--------------------------------------------------
	[System.Serializable]
	public class TargetObject : TargetDataObject
	{
		public TargetObject() : base() {}
		public TargetObject( TargetType _type ) : base( _type ){}
		public TargetObject( TargetObject _target ) : base( _target as TargetDataObject ){}
		public TargetObject( TargetDataObject _data ) : base( _data ){}

		public bool Foldout = true;

		public bool TargetMoveComplete = false;


		private float m_OffsetTime = 0;
		private float m_OffsetTimer = 0;
		private Vector3 m_LocalTargetMovePosition = Vector3.zero;
		private float m_SmoothingSpeed = 0;
		private float m_TargetMovePositionLevel = 0;
		private Vector3 m_TargetMovePositionRaw = Vector3.zero;
		private Vector3 m_TargetMovePosition = Vector3.zero;
		private float m_DynamicOffsetDistanceUpdateSpeed = 0;
		private float m_DynamicOffsetAngleUpdateSpeed = 0;
		private float m_TargetOffsetAngle = 0;
		private float m_TargetOffsetDistance = 0;		
		private float m_LastTargetVelocity = 0.0f;
		private float m_TargetVelocity = 0.0f;
		private Vector3 m_LastTargetPosition = Vector3.zero;

		/// <summary>
		/// Prepares the target.
		/// </summary>
		/// <returns>The target.</returns>
		/// <param name="_owner">_owner.</param>
		public GameObject GetBestTargetGameObject( GameObject _owner )
		{
			return GetBestTargetGameObject( _owner, 0 );
		}


		/// <summary>
		/// Prepares the target.
		/// </summary>
		/// <returns>The target.</returns>
		/// <param name="_owner">_owner.</param>
		/// <param name="_distance">_distance.</param>
		public GameObject GetBestTargetGameObject( GameObject _owner, float _distance )
		{
			if( TargetGameObject == null && TargetName == "" && TargetTag == "" )
				return null;

			if( _distance == 0 )
				_distance = Selectors.SelectionRange;

			GameObject _target_game_object = null;

			if( ICECreatureRegister.Instance != null )
			{
				if( AccessType == TargetAccessType.NAME || ( AccessType == TargetAccessType.OBJECT && TargetGameObject == null ) )
					_target_game_object = ICECreatureRegister.Instance.FindNearestTargetByName( _owner, TargetName, _distance );
				else if( AccessType == TargetAccessType.TAG )
					_target_game_object = ICECreatureRegister.Instance.FindNearestTargetByTag( _owner, TargetTag, _distance );
			}

			if( _target_game_object != null )
			{
				if( TargetGameObject != _target_game_object )
				{
					m_Creature = null;
					m_Player = null;
					m_Item = null;
					m_Location = null;
					m_Waypoint = null;
					m_Marker = null;
				}

				OverrideTargetGameObject( _target_game_object );
			}
	

			return TargetGameObject;
		}

		/// <summary>
		/// Gets all target game objects.
		/// </summary>
		/// <description>GetAllTargetGameObjects will be used e.g. by the debug class to get all interactors</description>
		/// <returns>All target game objects according to the given AccessType</returns>
		/// <param name="_owner">Owner.</param>
		public List<GameObject> GetAllTargetGameObjects( GameObject _owner )
		{
			List<GameObject> _objects = null;
			
			if( AccessType == TargetAccessType.NAME || ( AccessType == TargetAccessType.OBJECT && TargetGameObject == null ) )
				_objects = ICECreatureRegister.Instance.GetActiveGroupItemsByName( TargetName );
			else if( AccessType == TargetAccessType.TAG )
				_objects = ICECreatureRegister.Instance.GetActiveGroupItemsByTag( TargetTag );
	
			return _objects;
		}

		/// <summary>
		/// Sets the target default values.
		/// </summary>
		/// <param name="_targets">_targets.</param>
		public void SetTargetDefaultValues( List<TargetObject> _targets )
		{
			if( _targets == null || _targets.Count == 0 )
				return;

			// makes a random copy of the target settings   
			Copy( _targets[ Random.Range( 0, _targets.Count ) ] );
		}

		/// <summary>
		/// Reads the target default values.
		/// </summary>
		/// <returns>A list of target objects with default values based on the ICECreatureTarget scripts of the given target</returns>
		public List<TargetObject> ReadTargetAttributeData()
		{
			if( ! IsValid || TargetGameObject.GetComponent<ICECreatureTargetAttribute>() == null )
				return null;

			ICECreatureTargetAttribute[] _values = TargetGameObject.GetComponents<ICECreatureTargetAttribute>();

			List<TargetObject> _targets = new List<TargetObject>();
			foreach( ICECreatureTargetAttribute _data in _values )
			{
				if( _data != null && _data.Target != null && _data.isActiveAndEnabled == true )
					_targets.Add( _data.Target );
			}

			if( _targets.Count > 0 )
				return _targets;
			else
				return null;
		}

		[XmlIgnore]
		public Transform TargetTransform{
			get{ 
			
				if( IsValid )
					return TargetGameObject.transform;
				else
					return null;				
			}
		}


		public RegisterReferenceType TargetReferenceType{
			get{
				if( Creature() != null )
					return RegisterReferenceType.CREATURE;
				else if( Player() != null )
					return RegisterReferenceType.PLAYER;
				else if( Item() != null )
					return RegisterReferenceType.ITEM;
				else if( Location() != null )
					return RegisterReferenceType.LOCATION;
				else if( Waypoint() != null )
					return RegisterReferenceType.WAYPOINT;
				else if( Marker() != null )
					return RegisterReferenceType.MARKER;
				else
					return RegisterReferenceType.UNDEFINED;
			}
		}

		private ICECreatureControl m_Creature = null;
		public ICECreatureControl Creature()
		{
			if( TargetGameObject == null )
				return null;

			if( m_Creature == null )
				m_Creature = TargetGameObject.GetComponent<ICECreatureControl>();

			return m_Creature;
		}

		private ICECreaturePlayer m_Player = null;
		public ICECreaturePlayer Player()
		{
			if( TargetGameObject == null )
				return null;
			
			if( m_Player == null )
				m_Player = TargetGameObject.GetComponent<ICECreaturePlayer>();
			
			return m_Player;
		}
			
		private ICECreatureItem m_Item = null;
		public ICECreatureItem Item()
		{
			if( TargetGameObject == null )
				return null;
			
			if( m_Item == null )
				m_Item = TargetGameObject.GetComponent<ICECreatureItem>();
			
			return m_Item;
		}

		private ICECreatureLocation m_Location = null;
		public ICECreatureLocation Location()
		{
			if( TargetGameObject == null )
				return null;
			
			if( m_Location == null )
				m_Location = TargetGameObject.GetComponent<ICECreatureLocation>();
			
			return m_Location;
		}

		private ICECreatureWaypoint m_Waypoint = null;
		public ICECreatureWaypoint Waypoint()
		{
			if( TargetGameObject == null )
				return null;
			
			if( m_Waypoint == null )
				m_Waypoint = TargetGameObject.GetComponent<ICECreatureWaypoint>();
			
			return m_Waypoint;
		}

		private ICECreatureMarker m_Marker = null;
		public ICECreatureMarker Marker()
		{
			if( TargetGameObject == null )
				return null;

			if( m_Marker == null )
				m_Marker = TargetGameObject.GetComponent<ICECreatureMarker>();

			return m_Marker;
		}

		public InventoryObject Inventory()
		{
			if( TargetGameObject == null )
				return null;

			InventoryObject _inventory = new InventoryObject();

			if( Creature() != null )
				_inventory.Copy( Creature().Creature.Status.Inventory );
			else if( Item() != null )
				_inventory.Copy( Item().Inventory );

			//if( ! _inventory.IgnoreInventoryOwner )
			_inventory.Insert( TargetGameObject.name, 1 );

			return _inventory;
		}

		public bool TargetIsDead{
			get{
				if( Creature() != null )
					return Creature().Creature.Status.IsDead;
				else
					return false;
			}
		}

		public MoveType TargetMoveType{
			get{
				MoveType _move = MoveType.DEFAULT;

				switch( Type )
				{
					case TargetType.INTERACTOR:
						_move = MoveType.DEFAULT;
						break;
					case TargetType.OUTPOST:
						_move = MoveType.RANDOM;
						break;
					case TargetType.ESCORT:
						_move = MoveType.DEFAULT;
						break;
					case TargetType.PATROL:
						_move = MoveType.DEFAULT;
						break;
					case TargetType.WAYPOINT:
						_move = MoveType.DEFAULT;
						break;
					default:
						_move = MoveType.DEFAULT;
						break;
					
				}
				return _move;
			}

		}



		public int SelectionPriority{
			get{ return Selectors.GetPriority( Type ); }
		}

		public bool TargetInFieldOfView( Transform _transform, float _fov_angle, float _fov_distance )
		{
			if( IsValid == false || _transform == null )
				return false;

			// FOV check isn't required or creatures fov settings OFF or adjusted to a full-circle with an infinity distance
			if( Selectors.UseFieldOfView == false || _fov_angle == 0 || ( _fov_angle == 180 && _fov_distance == 0 ) )
				return true;

			float _target_distance = TargetDistanceTo( _transform.position );

			//distance test - if the target is too far, we don't need further checks ... 
			if( _fov_distance == 0 || _fov_distance >= _target_distance )
			{
				float _angle = GraphicTools.GetDirectionAngle( _transform , TargetGameObject.transform.position );

				// FOV test - the target must be inside the given range
				if( Mathf.Abs( _angle ) <= _fov_angle )
					return true;
				else
					return false;
			}
			else
				return false;
		}

		public bool TargetInSelectionRange( Transform _transform, float _fov_angle, float _fov_distance, float _eye_height )
		{
			if( TargetInFieldOfView( _transform, _fov_angle, _fov_distance ) && TargetInSelectionRange( _transform ) && TargetIsVisible( _transform, _eye_height ) )
				return true;
			else
				return false;
		}

		public bool TargetIsVisible( Transform _transform, float _eye_height )
		{
			if( IsValid == false || _transform == null )
				return false;

			// if the visibilty check isn't required the condition is fulfilled 
			if( Selectors.UseVisibilityCheck == false )
				return true;

			Vector3 _creature_pos = _transform.position;
			Vector3 _target_pos = TargetTransform.position;
			float _distance = Vector3.Distance( _creature_pos, _target_pos );

			Renderer _creature_renderer = _transform.root.GetComponentInChildren<Renderer>();
			Renderer _target_renderer = TargetTransform.root.GetComponentInChildren<Renderer>();

			// consider object size because the cast may not intersect the own collider  
			float _creature_radius = 1;
			if( _creature_renderer != null && _creature_renderer.bounds.extents != Vector3.zero )
				_creature_radius = _creature_renderer.bounds.extents.magnitude + 0.25f;

			// consider object size because the cast may not intersect the own collider  
			float _target_radius = 1;
			if( _target_renderer != null && _target_renderer.bounds.extents != Vector3.zero )
				_target_radius = _target_renderer.bounds.extents.magnitude + 0.25f;


			_creature_radius = _creature_radius/(_distance/100)/100;
			_creature_pos.y += _creature_radius;

			_target_radius = _creature_radius/(_distance/100)/100;
			_target_pos.y += _target_radius;

			_creature_pos = Vector3.Lerp( _creature_pos, _target_pos, _creature_radius );
			_target_pos = Vector3.Lerp( _target_pos, _creature_pos, _target_radius );

			RaycastHit _hit;

			//Debug.DrawLine(_creature_pos, _target_pos, Color.red );

			if( Physics.Linecast( _creature_pos, _target_pos, out _hit ) ) 
			{
				if( _hit.transform.IsChildOf( TargetTransform ) || _hit.transform.name == TargetName )
					return true;
				else
				{
					if( _hit.transform.IsChildOf( _transform ) )
						Debug.Log ( "Warning: Visual Check between '" + _transform.name + "' and '" + TargetTransform.name + "' will not working. Linecast hits own collider!" );

					return false;	

				}
			} 
			else 
				return true;
		}

		public bool TargetInSelectionRange( Transform _transform )
		{
			if( IsValid == false || _transform == null )
				return false;

			// if there are no custom criteria for home, the home target is always available ...
			if( Type == TargetType.HOME && Selectors.UseSelectionCriteriaForHome == false )
				return true;

			// unlimited - the target is always available
			if( Selectors.SelectionRange == 0 || Selectors.SelectionRange == 180 )
				return true;


			float _distance = TargetDistanceTo( _transform.position );

			// additional to the range we have also to check the correct field of view angle
			if( Selectors.SelectionAngle > 0 && Selectors.SelectionAngle < 180 )
			{
				float _angle = GraphicTools.GetDirectionAngle( TargetGameObject.transform, _transform.position );

				if( _distance <= Selectors.SelectionRange && Mathf.Abs( _angle ) <= Selectors.SelectionAngle )
					return true;
				else
					return false;
			}

			// here we will check the distance only
			else if( _distance <= Selectors.SelectionRange )
				return true;

			else
				return false;
		}

		//public bool UseOffsetAngle = false;





		public void UpdateRandomRange( float _random_range )
		{
			if( Move.RandomRange != _random_range || _random_range == 0 )
			{
				Move.RandomRange = _random_range;
				UpdateTargetMovePosition();
			}
			else
			{
				Move.RandomRange = _random_range;
			}
		}

		public float TargetMaxRange{
			get{ return Move.RandomRange + Move.StopDistance; }
		}

		

		private float m_ActiveTimeTotal = 0;
		public float ActiveTimeTotal{
			get{ return m_ActiveTimeTotal; }
		}

		private float m_ActiveTime = 0;
		public float ActiveTime{
			get{ return m_ActiveTime; }
		}

		private bool m_Active = false;
		public bool Active{
			get{ return m_Active; }
		}
		public void SetActive( bool _value )
		{
			if( m_Active != _value && m_Active == false && ( Move.UseUpdateOffsetOnActivateTarget == true || m_LocalTargetMovePosition == Vector3.zero ) ) 
				UpdateTargetMovePosition();

			if( m_Active != _value )
				m_ActiveTime = 0;

			m_Active = _value;
		}

		public bool IsValid{
			get{
				if( TargetGameObject == null )
					return false;
				else
					return true;
			}
		}


		//--------------------------------------------------
		
		public float TargetVelocity
		{
			get{ return m_TargetVelocity; }
		}

		//--------------------------------------------------
		
		public Vector3 TargetPosition
		{
			get{ 
				if( IsValid )
					return TargetGameObject.transform.position;
				else
					return Vector3.zero;
			}
		}

		//--------------------------------------------------

		public Vector3 TargetDirection
		{
			get{ 
				if( IsValid )
					return TargetGameObject.transform.forward;
				else
					return Vector3.forward;
			}
		}

		//--------------------------------------------------
		
		public Vector3 TargetOffsetPosition
		{
			get{ 
				 if( IsValid == false)
					return Vector3.zero;

				Vector3 _local_offset = Move.Offset;
		
				_local_offset.x = _local_offset.x/TargetGameObject.transform.lossyScale.x;
				_local_offset.y = _local_offset.y/TargetGameObject.transform.lossyScale.y;
				_local_offset.z = _local_offset.z/TargetGameObject.transform.lossyScale.z;

				return TargetGameObject.transform.TransformPoint( _local_offset );
			}
		}

		//--------------------------------------------------




		public bool UpdateTargetMovePosition()
		{
			if( ! IsValid )
				return false;

			m_LocalTargetMovePosition = Move.Offset;

			if( Move.RandomRange > 0 )
			{
				Vector2 _point = UnityEngine.Random.insideUnitCircle * Move.RandomRange;
				
				m_LocalTargetMovePosition.x += _point.x;
				m_LocalTargetMovePosition.z += _point.y;
			}

			return true;

		}

		private float m_OffsetAngle = 0;
		public float OffsetAngle{
			get{ return m_OffsetAngle;}
		}
		
		private float m_OffsetDistance = 0;
		public float OffsetDistance{
			get{ return m_OffsetDistance;}
		}

		public void UpdateOffset( Vector3 _offset )
		{
			Move.Offset = _offset;

			m_OffsetAngle = GraphicTools.GetOffsetAngle( Move.Offset );
			m_OffsetDistance = GraphicTools.GetHorizontalDistance( Vector3.zero, Move.Offset );
		}
		
		public void UpdateOffset( float _angle , float _distance )
		{
			if( ! IsValid )
				return;
			
			m_OffsetAngle = _angle;
			m_OffsetDistance = _distance;
			
			float _offset_angle = OffsetAngle;
			_offset_angle += TargetGameObject.transform.eulerAngles.y;
			
			if( _offset_angle > 360 )
				_offset_angle = _offset_angle - 360;
			
			Vector3 _world_offset = GraphicTools.GetAnglePosition( TargetGameObject.transform.position, _offset_angle, m_OffsetDistance );
			Vector3 _local_offset = TargetGameObject.transform.InverseTransformPoint( _world_offset );
			
			_local_offset.x = _local_offset.x*TargetGameObject.transform.lossyScale.x;
			_local_offset.y = _local_offset.y*TargetGameObject.transform.lossyScale.y;
			_local_offset.z = _local_offset.z*TargetGameObject.transform.lossyScale.z;	

			Vector3 _diff = m_LocalTargetMovePosition - Move.Offset;
			Move.Offset.z = _local_offset.z;
			Move.Offset.x = _local_offset.x;

			m_LocalTargetMovePosition = Move.Offset + _diff;
		}



		public Vector3 TargetMovePositionRaw{
			get{ 
				if( ! IsValid )
					return Vector3.zero;
				
				if( m_LocalTargetMovePosition == Vector3.zero )
					m_LocalTargetMovePosition = Move.Offset;
				
				Vector3 _local_offset = m_LocalTargetMovePosition;
				
				_local_offset.x = _local_offset.x/TargetGameObject.transform.lossyScale.x;
				_local_offset.y = _local_offset.y/TargetGameObject.transform.lossyScale.y;
				_local_offset.z = _local_offset.z/TargetGameObject.transform.lossyScale.z;

				m_TargetMovePositionRaw = TargetGameObject.transform.TransformPoint( _local_offset ); 
				m_TargetMovePositionRaw.y = m_TargetMovePositionLevel;

				//if( Controller() != null )
				//	m_TargetMovePositionRaw = Controller().Creature.Move.GetFreeGroundPosition( m_TargetMovePositionRaw );

				return m_TargetMovePositionRaw; 		
			}
		}


		public Vector3 TargetMovePosition{
			get{ 

				Vector3 _old_position = m_TargetMovePosition;
				m_TargetMovePosition = TargetMovePositionRaw; 

				if( Move.SmoothingMultiplier > 0 )
				{
					float _distance = Vector3.Distance( _old_position, m_TargetMovePosition );

					if( _distance > 0 )
					{
						// TODO: dependent of several distances not really nice 
						float speed = _distance - ( _distance * Move.SmoothingMultiplier ); 
						if( speed > 0 )
							m_SmoothingSpeed = speed;

						if( m_SmoothingSpeed > 0 )
							m_TargetMovePosition = Vector3.Lerp( _old_position, m_TargetMovePosition, m_SmoothingSpeed * Time.deltaTime );
					}
				}

				return m_TargetMovePosition;
			}
		}

		public void TargetMovePositionUpdateLevel( float _level )
		{
			m_TargetMovePositionLevel = _level;
		}

		//--------------------------------------------------
		
		public bool TargetInMaxRange( Vector3 position )
		{
			if( TargetOffsetPositionDistanceTo( position) <= TargetMaxRange )
				return true;
			else
				return false;
		}

		public bool TargetMovePositionReached( Vector3 position )
		{
			if( TargetMovePositionDistanceTo( position) <= Move.StopDistance )
				return true;
			else
				return false;
		}

		public float TargetMovePositionDistanceTo( Vector3 position )
		{
			Vector3 pos_1 = TargetMovePosition;
			Vector3 pos_2 = position;
			
			if( Move.IgnoreLevelDifference )
			{
				pos_1.y = 0;
				pos_2.y = 0;
			}
			
			return Vector3.Distance( pos_1, pos_2 );
		}

		public float TargetOffsetPositionDistanceTo( Vector3 position )
		{
			Vector3 pos_1 = TargetOffsetPosition;
			Vector3 pos_2 = position;

			if( Move.IgnoreLevelDifference )
			{
				pos_1.y = 0;
				pos_2.y = 0;
			}

			return Vector3.Distance( pos_1, pos_2 );
		}

		public float TargetDistanceTo( Vector3 position )
		{
			Vector3 pos_1 = TargetPosition;
			Vector3 pos_2 = position;
			
			if( Move.IgnoreLevelDifference )
			{
				pos_1.y = 0;
				pos_2.y = 0;
			}
			
			return Vector3.Distance( pos_1, pos_2 );

		}


		public void Update( GameObject _owner )
		{
			TargetMoveComplete = TargetMovePositionReached( _owner.transform.position );

			if( m_Active == true )
			{
			   	m_ActiveTime += Time.deltaTime;
				m_ActiveTimeTotal += Time.deltaTime;
			}

			if( Move.UseDynamicOffsetAngle || Move.UseDynamicOffsetDistance )
			{
				float _offset_angle = m_OffsetAngle;
				float _offset_distance = m_OffsetDistance;

				if( Move.UseDynamicOffsetDistance && ( Move.MinOffsetDistance > 0 || Move.MaxOffsetDistance > 0 ) )
				{
					if( Move.MinOffsetDistance == Move.MaxOffsetDistance )
						Move.MinOffsetDistance = 0;


					if( Move.MinOffsetDistance < Move.MaxOffsetDistance )
					{
						if( ( m_DynamicOffsetDistanceUpdateSpeed < 0 && _offset_distance <= m_TargetOffsetDistance ) || 
						   ( m_DynamicOffsetDistanceUpdateSpeed > 0 && _offset_distance >= m_TargetOffsetDistance ) )
						{
							if( Move.UseRandomOffsetDistance )
								m_TargetOffsetDistance = Random.Range( Move.MinOffsetDistance, Move.MaxOffsetDistance );
							else if( m_TargetOffsetDistance >= Move.MaxOffsetDistance )
								m_TargetOffsetDistance = Move.MinOffsetDistance;
							else
								m_TargetOffsetDistance = Move.MaxOffsetDistance;
						}
						
					}

					m_DynamicOffsetDistanceUpdateSpeed = Move.DynamicOffsetDistanceUpdateSpeed;
					if( _offset_distance >= m_TargetOffsetDistance )
						m_DynamicOffsetDistanceUpdateSpeed *= -1;
					
					_offset_distance += m_DynamicOffsetDistanceUpdateSpeed;
				}

				if( Move.UseDynamicOffsetAngle && _offset_distance > 0 )
				{
					if( Move.MinOffsetAngle == Move.MaxOffsetAngle )
						m_TargetOffsetAngle = Move.MinOffsetAngle;
					else if( Move.MinOffsetAngle < Move.MaxOffsetAngle )
					{
						if( ( m_DynamicOffsetAngleUpdateSpeed < 0 && _offset_angle - Move.DynamicOffsetAngleUpdateSpeed <= m_TargetOffsetAngle ) || 
						   ( m_DynamicOffsetAngleUpdateSpeed > 0 && _offset_angle + Move.DynamicOffsetAngleUpdateSpeed >= m_TargetOffsetAngle ) )
						{
							if( Move.UseRandomOffsetAngle )
								m_TargetOffsetAngle = Random.Range( Move.MinOffsetAngle, Move.MaxOffsetAngle );
							else if( m_TargetOffsetAngle >= Move.MaxOffsetAngle )
								m_TargetOffsetAngle = Move.MinOffsetAngle;
							else
								m_TargetOffsetAngle = Move.MaxOffsetAngle;
						}

					}
					else
						m_TargetOffsetAngle = 360;
					 
					m_DynamicOffsetAngleUpdateSpeed = Move.DynamicOffsetAngleUpdateSpeed;
					if( _offset_angle >= m_TargetOffsetAngle )
						m_DynamicOffsetAngleUpdateSpeed *= -1;
		
					_offset_angle += m_DynamicOffsetAngleUpdateSpeed;

					if( _offset_angle >= 360 )
						_offset_angle = _offset_angle - 360;
				}

				if( _offset_angle != OffsetAngle || _offset_distance != OffsetDistance )
					UpdateOffset( _offset_angle, _offset_distance );
			}
	
			if( Move.UseUpdateOffsetOnRandomizedTimer )
			{
				if( m_OffsetTimer >= m_OffsetTime )
				{
					UpdateTargetMovePosition();
					m_OffsetTimer = 0;

					m_OffsetTime = Random.Range( Move.OffsetUpdateTimeMin, Move.OffsetUpdateTimeMax );
				}

				m_OffsetTimer += Time.deltaTime;
			}

			if( Move.UseUpdateOffsetOnMovePositionReached && TargetMoveComplete )
				UpdateTargetMovePosition();
		}

		//--------------------------------------------------
		
		public float FixedUpdate()
		{
			if( ! IsValid )
				return 0.0f;
			
			m_LastTargetVelocity = m_TargetVelocity;
			float _velocity = (( TargetGameObject.transform.position - m_LastTargetPosition ).magnitude) / Time.deltaTime;
			
			if( _velocity == 0 && m_TargetVelocity > 0.5f && m_LastTargetVelocity > 0.75f )
				_velocity = ( _velocity + m_TargetVelocity + m_LastTargetVelocity ) / 3;
			
			m_LastTargetVelocity = m_TargetVelocity;
			m_TargetVelocity = _velocity;
			
			m_LastTargetPosition = TargetGameObject.transform.position;
			
			//UnityEngine.Debug.Log ( "TargetVelocityUpdate - " + m_TargetGameObject.name + " (" +m_TargetVelocity+ "/" + m_LastTargetVelocity + ")");
			
			return m_TargetVelocity;
		}
	}

}
