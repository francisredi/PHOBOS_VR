// ##############################################################################
//
// ice_CreatureStatus.cs
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
using ICE.Utilities.EnumTypes;
using ICE.Utilities;

namespace ICE
{
	namespace Creatures
	{
		namespace Objects
		{
			[System.Serializable]
			public class StatusDataObject : MemoryObject
			{
				protected float m_TotalLifetime = 0;
				public float TotalLifetime{
					get{ return m_TotalLifetime; }
				}

				protected float m_Lifetime = 0;
				public float Lifetime{
					get{ return m_Lifetime; }
				}
					
				public float PerceptionTime = 0.5f;
				public float DefaultPerceptionTime = 0.5f;
				public float PerceptionTimeVariance = 0.25f;
				public bool AutoPerceptionTime = true;
				public float PerceptionTimeFitnessMultiplier = 0.0f;
				
				public float ReactionTime = 0.5f;
				public float DefaultReactionTime = 0.5f;
				public float ReactionTimeVariance = 0.25f;
				public bool AutoReactionTime = true;
				public float ReactionTimeFitnessMultiplier = 0.0f;

				public float RecoveryPhase = 0;
				public float RecoveryPhasePeriodMax = 30;
				public float RespawnDelayMax = 300.0f;
				public float RespawnDelay = 20.0f;
				public float RespawnDelayVariance = 0.25f;
				public bool UseCorpse = false;
				[XmlIgnore]
				public GameObject CorpseReferencePrefab = null;

				public float FitnessRecreationLimit = 0;

				public CreatureTrophicLevelType TrophicLevel = CreatureTrophicLevelType.UNDEFINED;
				public bool IsCannibal = false;
				public bool UseDynamicInitialisation = false;
				
				public bool UseAdvanced = false;
				public bool UseArmor = false;

				public bool UseEnvironmentTemperature = false;
				public float MinEnvironmentTemperature = -25;
				public float MaxEnvironmentTemperature = 50;
				public float ComfortEnvironmentTemperature = 25;

				public bool UseAging = false;
				public float MaxAge = 60f;		

				public bool UseTime = false;
				public bool UseDate = false;

				public bool UseShelter = false;
				public string ShelterTag = "Untagged";
				public bool IsSheltered = false;

				public bool UseIndoor = false;
				public string IndoorTag = "Untagged";
				public bool IsIndoor = false;
				//public float ShelterTemperature = false;

				//public bool ConsiderBreathing = false;

				
				public int DefaultSenseVisual = 100; 
				public float SenseVisualAgeMultiplier = 0.0f;
				public float SenseVisualFitnessMultiplier = 0.0f;
				
				public int DefaultSenseAuditory = 100;
				public float SenseAuditoryAgeMultiplier = 0.0f;
				public float SenseAuditoryFitnessMultiplier = 0.0f;
				
				public int DefaultSenseOlfactory = 100; 
				public float SenseOlfactoryAgeMultiplier = 0.0f;
				public float SenseOlfactoryFitnessMultiplier = 0.0f;
				
				public int DefaultSenseGustatory = 100; 
				public float SenseGustatoryAgeMultiplier = 0.0f;
				public float SenseGustatoryFitnessMultiplier = 0.0f;
				
				public int DefaultSenseTactile = 100; 
				public float SenseTouchAgeMultiplier = 0.0f;
				public float SenseTouchFitnessMultiplier = 0.0f;
				
				public float Aggressivity = 25;
				public float DefaultAggressivity = 25;
				public float AggressivityHealthMultiplier = 1.0f;
				public float AggressivityDamageMultiplier = 0.01f;
				public float AggressivityStaminaMultiplier = 0.01f;
				public float AggressivityStressMultiplier = 0.01f;
				public float AggressivityDebilityMultiplier = 0.25f;
				public float AggressivityHungerMultiplier = 0.0f;
				public float AggressivityThirstMultiplier = 0.0f;
				public float AggressivityTemperaturMultiplier = 0.0f;
				public float AggressivityAgeMultiplier = 0.0f;
				
				public float Anxiety = 0;
				public float DefaultAnxiety = 0;
				public float AnxietyHealthMultiplier = 1.0f;
				public float AnxietyDamageMultiplier = 0.01f;
				public float AnxietyStaminaMultiplier = 0.01f;
				public float AnxietyStressMultiplier = 0.01f;
				public float AnxietyDebilityMultiplier = 0.25f;
				public float AnxietyHungerMultiplier = 0.0f;
				public float AnxietyThirstMultiplier = 0.0f;
				public float AnxietyTemperaturMultiplier = 0.0f;
				public float AnxietyAgeMultiplier = 0.0f;
				
				public float Experience = 0;
				public float DefaultExperience = 0;
				public float ExperienceHealthMultiplier = 0.0f;
				public float ExperienceDamageMultiplier = 0.0f;
				public float ExperienceStaminaMultiplier = 0.0f;
				public float ExperienceStressMultiplier = 0.0f;
				public float ExperienceDebilityMultiplier = 0.0f;
				public float ExperienceHungerMultiplier = 0.0f;
				public float ExperienceThirstMultiplier = 0.0f;
				public float ExperienceTemperaturMultiplier = 0.0f;
				public float ExperienceAgeMultiplier = 0.0f;
				
				public float Nosiness = 0;
				public float DefaultNosiness = 0;
				public float NosinessHealthMultiplier = 0.0f;
				public float NosinessDamageMultiplier = 0.0f;
				public float NosinessStaminaMultiplier = 0.0f;
				public float NosinessStressMultiplier = 0.0f;
				public float NosinessDebilityMultiplier = 0.0f;
				public float NosinessHungerMultiplier = 0.0f;
				public float NosinessThirstMultiplier = 0.0f;
				public float NosinessTemperaturMultiplier = 0.0f;
				public float NosinessAgeMultiplier = 0.0f;
				
				// Default Fitness
				public int DefaultHealth = 100;
				public float HealthDamageMultiplier = 1.0f;
				public float HealthStressMultiplier = 1.0f;
				public float HealthDebilityMultiplier = 1.0f;
				public float HealthHungerMultiplier = 0.0f;
				public float HealthThirstMultiplier = 0.0f;
				public float HealthRecreationMultiplier = 0.2f;
				public float HealthTemperaturMultiplier = 0.0f;
				public float HealthAgeMultiplier = 0.0f;
				
				public int DefaultPower = 100;
				public float PowerHealthMultiplier = 1.0f;
				public float PowerDamageMultiplier = 0.01f;
				public float PowerStaminaMultiplier = 0.01f;
				public float PowerStressMultiplier = 0.01f;
				public float PowerDebilityMultiplier = 0.01f;
				public float PowerHungerMultiplier = 0.0f;
				public float PowerThirstMultiplier = 0.0f;
				public float PowerTemperaturMultiplier = 0.0f;
				public float PowerAgeMultiplier = 0.0f;
				
				public int DefaultStamina = 100;
				public float StaminaHealthMultiplier = 0.01f;
				public float StaminaDamageMultiplier = 0.01f;
				public float StaminaStressMultiplier = 0.01f;
				public float StaminaDebilityMultiplier = 0.01f;
				public float StaminaHungerMultiplier = 0.0f;
				public float StaminaThirstMultiplier = 0.0f;
				public float StaminaTemperaturMultiplier = 0.0f;
				public float StaminaAgeMultiplier = 0.0f;

				//public float RecreationMultiplier = 0.01f;
				public float FitnessSpeedMultiplier = 0.01f;


				[SerializeField]
				private OdourObject m_Odour = new OdourObject();
				public OdourObject Odour{
					set{ m_Odour = value; }
					get{ return m_Odour; }
				}

				[SerializeField]
				private InventoryObject m_Inventory = new InventoryObject();
				public InventoryObject Inventory{
					set{ m_Inventory = value; }
					get{ return m_Inventory; }
				}
			}

			[System.Serializable]
			public class StatusObject : StatusDataObject
			{
				public void Init( GameObject gameObject )
				{
					m_Lifetime = 0;
					m_TotalLifetime = 0;

					m_Owner = gameObject;
					
					PerceptionTime = DefaultPerceptionTime + ( DefaultPerceptionTime * Random.Range( - PerceptionTimeVariance, PerceptionTimeVariance ) );
					ReactionTime = DefaultReactionTime + ( DefaultReactionTime * Random.Range( - ReactionTimeVariance, ReactionTimeVariance ) );
					RespawnDelay = RespawnDelay + ( RespawnDelay * Random.Range( - RespawnDelayVariance, RespawnDelayVariance ) );
					
					m_PerceptionTimer = PerceptionTime;
					m_ReactionTimer = ReactionTime;
					m_RespawnTimer = RespawnDelay;

					Inventory.Init( m_Owner );
					Odour.Init( m_Owner );
					
					Reset();
					
				}
				


				public float SpeedMultiplier{
					get{
						float current_value = 100;						
						current_value -= ( 100 - FitnessInPercent ) * FitnessSpeedMultiplier;						
						return FixedMultiplier( current_value / 100 );
					}
				}

				public bool RecreationRequired{
					get{
						if( FitnessRecreationLimit == 0 )
							return false;
						else if( FitnessInPercent <= FitnessRecreationLimit )
							return true;
						else
							return false;
					}
				}

				private TemperatureScaleType m_TemperatureScale;
				public TemperatureScaleType TemperatureScale
				{
					get{
						TemperatureScaleType _scale = (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.TemperatureScale:TemperatureScaleType.CELSIUS);
		
						if( m_TemperatureScale != _scale )
						{
							if( _scale == TemperatureScaleType.CELSIUS && m_TemperatureScale == TemperatureScaleType.FAHRENHEIT )
							{
								MaxEnvironmentTemperature = GraphicTools.FahrenheitToCelsius( MaxEnvironmentTemperature );
								MinEnvironmentTemperature = GraphicTools.FahrenheitToCelsius( MinEnvironmentTemperature );
								ComfortEnvironmentTemperature = GraphicTools.FahrenheitToCelsius( ComfortEnvironmentTemperature );
							}
							else if( _scale == TemperatureScaleType.FAHRENHEIT && m_TemperatureScale == TemperatureScaleType.CELSIUS )
							{
								MaxEnvironmentTemperature = GraphicTools.CelsiusToFahrenheit( MaxEnvironmentTemperature );
								MinEnvironmentTemperature = GraphicTools.CelsiusToFahrenheit( MinEnvironmentTemperature );
								ComfortEnvironmentTemperature = GraphicTools.CelsiusToFahrenheit( ComfortEnvironmentTemperature );
							}
						}

						m_TemperatureScale = _scale;

						return m_TemperatureScale;
					}

				}

				public float EnvironmentTemperature{
					get{ return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.Temperature:0); }
				}

				public float EnvironmentMinTemperature{
					get{ return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.MinTemperature:0); }
				}

				public float EnvironmentMaxTemperature{
					get{ return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.MaxTemperature:0); }
				}

				public float TimeHour{
					get{ return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.TimeHour:0); }
				}

				public float TimeMinutes{
					get{ return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.TimeMinutes:0); }
				}

				public float TimeSeconds{
					get{ return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.TimeSeconds:0); }
				}

				public float DateDay{
					get{ return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.DateDay:0); }
				}

				public float DateMonth{
					get{ return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.DateMonth:0); }
				}

				public float DateYear{
					get{ return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.DateYear:0); }
				}

				public WeatherType Weather{
					get{ return (ICEGlobalEnvironmentInfo.Instance != null?ICEGlobalEnvironmentInfo.Instance.Weather:WeatherType.CLEAR); }
				}


				private float m_Age = 0.0f;
				public float Age{ 
					get{ return m_Age; }
				}

				public void SetAge( float _age )
				{ 
					if( _age >= 0 && _age <= MaxAge )
						m_Age = _age;
				}

				public float LifespanInPercent{		
					get{ return FixedPercent( 100 - ( 100/MaxAge*m_Age) ); }
				}
		
				public float TemperatureDeviationInPercent
				{
					get{ 
						float _tmp_1 = 0;
						float _tmp_2 = 0;
						if( EnvironmentTemperature < ComfortEnvironmentTemperature )
						{
							_tmp_1 = Mathf.Abs(MinEnvironmentTemperature - ComfortEnvironmentTemperature);
							_tmp_2 = Mathf.Abs(MinEnvironmentTemperature - EnvironmentTemperature);
						}
						else
						{
							_tmp_1 = Mathf.Abs(MaxEnvironmentTemperature - ComfortEnvironmentTemperature);
							_tmp_2 = Mathf.Abs(MaxEnvironmentTemperature - EnvironmentTemperature);
						}

						return FixedPercent( 100 - ( 100/_tmp_1*_tmp_2 ) ); 
					}
				}

				private float m_PerceptionTimer = 0.0f;
				public bool IsSenseTime()
				{
					if( IsDead )
						return false;

					float _delay = 0;
					if( UseAdvanced && ReactionTimeFitnessMultiplier > 0 )
						_delay = ( ( 100 - FitnessInPercent ) * ReactionTimeFitnessMultiplier * 0.1f );
					
					if( m_PerceptionTimer >= PerceptionTime + _delay )
					{
						PerceptionTime = DefaultPerceptionTime + ( DefaultPerceptionTime * Random.Range( - PerceptionTimeVariance, PerceptionTimeVariance ) );
						m_PerceptionTimer = 0;

						return true;
					}
					else
					{
						return false;
					}
					
				}

				public bool IsReactionForced = false;
				private float m_ReactionTimer = 0.0f;
				public bool IsReactionTime()
				{
					if( IsDead )
						return false;

					float _delay = 0;
					if( UseAdvanced && ReactionTimeFitnessMultiplier > 0 )
						_delay = ( ( 100 - FitnessInPercent ) * ReactionTimeFitnessMultiplier * 0.1f );

					if( m_ReactionTimer >= ReactionTime + _delay || IsReactionForced )
					{
						ReactionTime = DefaultReactionTime + ( DefaultReactionTime * Random.Range( - ReactionTimeVariance, ReactionTimeVariance ) );
						m_ReactionTimer = 0;
						IsReactionForced = false;
						return true;
					}
					else
					{
						return false;
					}
				}

				private bool m_RespawnRequired = false;
				private float m_RespawnTimer = 0.0f;
				public bool IsRespawnTime{
					get{ return ( m_RespawnTimer >= RespawnDelay )?true:false; }
				}




				public void RespawnRequest()
				{
					if( m_RespawnRequired || ! IsDead )
						return;

					m_RespawnTimer = 0.0f;
					m_RespawnRequired = true;

					Inventory.DetachOnDie();

					if( UseCorpse && CorpseReferencePrefab != null )
					{
						ReplaceBody();
						Respawn();
					}
				}
				
				public void Respawn()
				{
					if( ! m_RespawnRequired || ! IsDead )
						return;

					m_RespawnTimer = 0.0f;
					m_RespawnRequired = false;
					
					ICECreatureRegister.Instance.Cleanup( m_Owner );
				}

				private GameObject m_Corpse = null;
				public void ReplaceBody()
				{
					if( m_Corpse != null || m_Owner.activeInHierarchy == false )
						return;

					m_Corpse = (GameObject)GameObject.Instantiate( CorpseReferencePrefab, m_Owner.transform.position, m_Owner.transform.rotation );
							
					m_Corpse.name = CorpseReferencePrefab.name;
					SystemTools.CopyTransforms( m_Owner.transform, m_Corpse.transform );

					if( RespawnDelay > 0 )
						GameObject.Destroy( m_Corpse, RespawnDelay ); 
				}



				/// <summary>
				/// Reset the status values.
				/// </summary>
				public void Reset()
				{
					Inventory.Reset();

					m_PerceptionTimer = PerceptionTime;
					m_ReactionTimer = ReactionTime;

					m_Lifetime = 0.0f;
					m_RespawnTimer = 0.0f;
					m_RespawnRequired = false;

					m_Age = 0;
					m_DamageInPercent = 0;
					m_StressInPercent = 0;
					m_DebilityInPercent = 0;
					m_HungerInPercent = 0;
					m_ThirstInPercent = 0;

					Aggressivity = DefaultAggressivity;
					Nosiness = DefaultNosiness;
					Experience = DefaultExperience;
					Anxiety = DefaultAnxiety;

					if( ! UseArmor )
						m_ArmorInPercent = 100;

					if( UseDynamicInitialisation )
						CalculateRandomStatusValues( TrophicLevel );

					m_Corpse = null;
				}

				public void Kill()
				{
					m_Age = MaxAge;
					m_DamageInPercent = 100;
					m_StressInPercent = 100;
					m_DebilityInPercent = 100;
					m_HungerInPercent = 100;
					m_ThirstInPercent = 100;
					
					if( ! UseArmor )
						m_ArmorInPercent = 0;
				}

				private float m_ArmorInPercent = 0;
				public float ArmorInPercent
				{
					set{ m_ArmorInPercent = value; }
					get{ return FixedPercent( m_ArmorInPercent ); }
				}

				private float HandleArmor( float _damage )
				{
					if( ! UseArmor )
						return _damage;

					if( _damage < 0 )
						return _damage;

					m_ArmorInPercent -= _damage;

					if( ArmorInPercent > 0 )
						_damage -= m_ArmorInPercent / 100;
		
					return _damage;
				}

				public void ResetArmor()
				{
					if( UseArmor )
						m_ArmorInPercent = 100;
					else
						m_ArmorInPercent = 0;
				}


				private float m_DamageInPercent = 0;
				public float DamageInPercent
				{
					set{ m_DamageInPercent = value; }
					get{ return FixedPercent( m_DamageInPercent ); }
				}

				public void AddDamage( float _damage )
				{
					_damage = HandleArmor( _damage );

					m_DamageInPercent += _damage;
					m_DamageInPercent = FixedPercent( m_DamageInPercent );
				}

				private float m_StressInPercent = 0;
				public float StressInPercent
				{
					set{ m_StressInPercent = value; }
					get{ return FixedPercent( m_StressInPercent ); }
				}
			
				public void AddStress( float _stress )
				{
					m_StressInPercent += _stress;
					m_StressInPercent = FixedPercent( m_StressInPercent );
				}

				private float m_DebilityInPercent = 0;
				public float DebilityInPercent
				{
					set{ m_DebilityInPercent = value; }
					get{ return FixedPercent( m_DebilityInPercent ); }
				}

				public void AddDebility( float _debility )
				{
					m_DebilityInPercent += _debility;
					m_DebilityInPercent = FixedPercent( m_DebilityInPercent );
				}


				private float m_HungerInPercent = 0;
				public float HungerInPercent
				{
					set{ m_HungerInPercent = value; }
					get{ return FixedPercent( m_HungerInPercent ); }
				}

				public void AddHunger( float _hunger )
				{
					m_HungerInPercent += _hunger;
					m_HungerInPercent = FixedPercent( m_HungerInPercent );
				}

				private float m_ThirstInPercent = 0;
				public float ThirstInPercent
				{
					set{ m_ThirstInPercent = value; }
					get{ return FixedPercent( m_ThirstInPercent ); }
				}

				public void AddThirst( float _thirst )
				{
					m_ThirstInPercent += _thirst;
					m_ThirstInPercent = FixedPercent( m_ThirstInPercent );
				}

				public float SensesInPercent
				{
					get{
						float _max_senses = ( DefaultSenseTactile + DefaultSenseGustatory + DefaultSenseOlfactory + DefaultSenseAuditory + DefaultSenseVisual ) / 5;
						float _current_senses = ( 
						                         ( DefaultSenseTactile * SenseTouchInPercent / 100 ) + 
						                         ( DefaultSenseGustatory * SenseGustatoryInPercent / 100 ) + 
						                         ( DefaultSenseOlfactory * SenseOlfactoryInPercent / 100 ) + 
						                         ( DefaultSenseAuditory * SenseAuditoryInPercent / 100 ) + 
						                         ( DefaultSenseVisual * SenseVisualInPercent / 100 ) ) / 5;
						
						if( _max_senses > 0 )
							_current_senses = ( 100 / _max_senses ) * _current_senses;
						else
							_current_senses = 0;
						
						if( IsDead )
							_current_senses = 0;
						
						return FixedPercent( _current_senses );
					}
				}

				public float SenseTouchInPercent
				{
					get{
						float max_value = DefaultSenseTactile;
						float current_value = max_value;
						
						if( max_value > 0 )
							current_value = ( 100 / max_value ) * current_value;
						else
							current_value = 0;
						
						if( UseAdvanced )
						{
							current_value -= ( 100 - FitnessInPercent ) * SenseTouchFitnessMultiplier;
							
							if( UseAging )
								current_value -= ( 100 - LifespanInPercent ) * SenseTouchAgeMultiplier;
							
						}
						
						return FixedPercent( current_value );
					}
				}

				public float SenseGustatoryInPercent
				{
					get{
						float _max_value = DefaultSenseGustatory;
						float _current_value = _max_value;
						
						if( _max_value > 0 )
							_current_value = ( 100 / _max_value ) * _current_value;
						else
							_current_value = 0;
						
						if( UseAdvanced )
						{
							_current_value -= ( 100 - FitnessInPercent ) * SenseGustatoryFitnessMultiplier;
							
							if( UseAging )
								_current_value -= ( 100 - LifespanInPercent ) * SenseGustatoryAgeMultiplier;
							
						}
						
						return FixedPercent( _current_value );
					}
				}

				public float SenseOlfactoryInPercent
				{
					get{
						float _max_value = DefaultSenseOlfactory;
						float _current_value = _max_value;
						
						if( _max_value > 0 )
							_current_value = ( 100 / _max_value ) * _current_value;
						else
							_current_value = 0;
						
						if( UseAdvanced )
						{
							_current_value -= ( 100 - FitnessInPercent ) * SenseOlfactoryFitnessMultiplier;
							
							if( UseAging )
								_current_value -= ( 100 - LifespanInPercent ) * SenseOlfactoryAgeMultiplier;
							
						}
						
						return FixedPercent( _current_value );
					}
				}

				public float SenseAuditoryInPercent
				{
					get{
						float _max_value = DefaultSenseAuditory;
						float _current_value = _max_value;
						
						if( _max_value > 0 )
							_current_value = ( 100 / _max_value ) * _current_value;
						else
							_current_value = 0;
						
						if( UseAdvanced )
						{
							_current_value -= ( 100 - FitnessInPercent ) * SenseAuditoryFitnessMultiplier;
							
							if( UseAging )
								_current_value -= ( 100 - LifespanInPercent ) * SenseAuditoryAgeMultiplier;
							
						}
						
						return FixedPercent( _current_value );
					}
				}

				public float SenseVisualInPercent
				{
					get{
						float _max_value = DefaultSenseVisual;
						float _current_value = _max_value;
						
						if( _max_value > 0 )
							_current_value = ( 100 / _max_value ) * _current_value;
						else
							_current_value = 0;
						
						if( UseAdvanced )
						{
							_current_value -= ( 100 - FitnessInPercent ) * SenseVisualFitnessMultiplier;

							if( UseAging )
								_current_value -= ( 100 - LifespanInPercent ) * SenseVisualAgeMultiplier;

						}

						return FixedPercent( _current_value );
					}
				}

				public float HealthInPercent
				{
					get{
						float _max_value = DefaultHealth;
						float _current_value = _max_value;
										
						if( _max_value > 0 )
							_current_value = ( 100 / _max_value ) * _current_value;
						else
							_current_value = 0;

						if( UseAdvanced )
						{
							_current_value -= DamageInPercent * HealthDamageMultiplier;
							_current_value -= StressInPercent * HealthStressMultiplier;
							_current_value -= DebilityInPercent * HealthDebilityMultiplier;
							_current_value -= HungerInPercent * HealthHungerMultiplier;
							_current_value -= ThirstInPercent * HealthThirstMultiplier;

							if( UseEnvironmentTemperature )
								_current_value -= TemperatureDeviationInPercent * HealthTemperaturMultiplier;
							
							if( UseAging )
								_current_value -= ( 100 - LifespanInPercent ) * HealthAgeMultiplier;

							if( UseAging && m_Age >= MaxAge )
								_current_value = 0;
						}
						else
						{
							_current_value -= DamageInPercent;
						}

									
						return FixedPercent( _current_value );
					}
				}

				public float StaminaInPercent
				{
					get{
						float max_value = DefaultStamina;
						float current_value = max_value;
							
						if( max_value > 0 )
							current_value = ( 100 / max_value ) * current_value;
						else
							current_value = 0;

						if( UseAdvanced )
						{
							if( UseEnvironmentTemperature )
								current_value -= TemperatureDeviationInPercent * StaminaTemperaturMultiplier;

							if( UseAging )
								current_value -= ( 100 - LifespanInPercent ) * StaminaAgeMultiplier;

							current_value -= DamageInPercent * StaminaDamageMultiplier;
							current_value -= StressInPercent * StaminaStressMultiplier;
							current_value -= HungerInPercent * StaminaHungerMultiplier;
							current_value -= ThirstInPercent * StaminaThirstMultiplier;
							current_value -= DebilityInPercent * StaminaDebilityMultiplier;

							current_value -= ( ( 100 - HealthInPercent ) * StaminaHealthMultiplier );
						}

						if( IsDead )
							current_value = 0;

						return FixedPercent( current_value );
					}
				}

				public float PowerInPercent
				{
					get{
						float max_value = DefaultPower;
						float current_value = max_value;

						if( max_value > 0 )
							current_value = ( 100 / max_value ) * current_value;
						else
							current_value = 0;

						if( UseAdvanced )
						{
							if( UseEnvironmentTemperature )
								current_value -= TemperatureDeviationInPercent * PowerTemperaturMultiplier;

							if( UseAging )
								current_value -= ( 100 - LifespanInPercent ) * PowerAgeMultiplier;

							current_value += StressInPercent * PowerStressMultiplier;
							current_value -= DebilityInPercent * PowerDebilityMultiplier;
							current_value -= DamageInPercent * PowerDamageMultiplier;
							current_value -= HungerInPercent * PowerHungerMultiplier;
							current_value -= ThirstInPercent * PowerThirstMultiplier;
							current_value -= ( ( 100 - StaminaInPercent ) * PowerStaminaMultiplier );
							current_value -= ( ( 100 - HealthInPercent ) * PowerHealthMultiplier );
						}

						if( IsDead )
							current_value = 0;

						return FixedPercent( current_value );
					}
				}

				public bool IsSpawning{
					get{ return ( Lifetime <= RecoveryPhase )?true:false; }
				}

				public bool IsDead
				{
					get{ return ( HealthInPercent <= 0 )?true:false; }
				}

				public void AddAggressivity( float _value )
				{
					Aggressivity += _value;
					Aggressivity = FixedPercent( Aggressivity );
				}

				public float AggressivityInPercent
				{
					get{
						float _default_value = 100;
						float _current_value = Aggressivity;

						if( _default_value > 0 )
							_current_value = ( 100 / _default_value ) * _current_value;
						else
							_current_value = 0;

						if( UseAdvanced )
						{
							if( UseEnvironmentTemperature )
								_current_value -= TemperatureDeviationInPercent * AggressivityTemperaturMultiplier;

							if( UseAging )
								_current_value -= ( 100 - LifespanInPercent ) * AggressivityAgeMultiplier;

							_current_value += DamageInPercent * AggressivityDamageMultiplier;
							_current_value += StressInPercent * AggressivityStressMultiplier;
							_current_value += DebilityInPercent * AggressivityDebilityMultiplier;
							_current_value += HungerInPercent * AggressivityHungerMultiplier;
							_current_value += ThirstInPercent * AggressivityThirstMultiplier;
							_current_value += ( ( 100 - StaminaInPercent ) * AggressivityStaminaMultiplier );
							_current_value += ( ( 100 - HealthInPercent ) * AggressivityHealthMultiplier );

						}

						if( IsDead )
							_current_value = 0;
							
						return FixedPercent( _current_value );
					}
				}

				public void AddAnxiety( float _value )
				{
					Anxiety += _value;
					Anxiety = FixedPercent( Anxiety );
				}

				public float AnxietyInPercent
				{
					get{
						float _default_value = 100;
						float _current_value = Anxiety;
						
						if( _default_value > 0 )
							_current_value = ( 100 / _default_value ) * _current_value;
						else
							_current_value = 0;
						
						if( UseAdvanced )
						{
							if( UseEnvironmentTemperature )
								_current_value += TemperatureDeviationInPercent * AnxietyTemperaturMultiplier;
							
							if( UseAging )
								_current_value += ( 100 - LifespanInPercent ) * AnxietyAgeMultiplier;
							
							_current_value += DamageInPercent * AnxietyDamageMultiplier;
							_current_value += StressInPercent * AnxietyStressMultiplier;
							_current_value += DebilityInPercent * AnxietyDebilityMultiplier;
							_current_value += HungerInPercent * AnxietyHungerMultiplier;
							_current_value += ThirstInPercent * AnxietyThirstMultiplier;
							_current_value += ( ( 100 - StaminaInPercent ) * AnxietyStaminaMultiplier );
							_current_value += ( ( 100 - HealthInPercent ) * AnxietyHealthMultiplier );
						}
						
						if( IsDead )
							_current_value = 0;
						
						return FixedPercent( _current_value );
					}
				}

				public void AddExperience( float _value )
				{
					Experience += _value;
					Experience = FixedPercent( Experience );
				}
				
				public float ExperienceInPercent
				{
					get{
						float _default_value = 100;
						float _current_value = Experience;
						
						if( _default_value > 0 )
							_current_value = ( 100 / _default_value ) * _current_value;
						else
							_current_value = 0;
						
						if( UseAdvanced )
						{
							if( UseEnvironmentTemperature )
								_current_value += TemperatureDeviationInPercent * ExperienceTemperaturMultiplier;
							
							if( UseAging )
								_current_value += ( 100 - LifespanInPercent ) * ExperienceAgeMultiplier;
							
							_current_value += DamageInPercent * ExperienceDamageMultiplier;
							_current_value += StressInPercent * ExperienceStressMultiplier;
							_current_value += DebilityInPercent * ExperienceDebilityMultiplier;
							_current_value += HungerInPercent * ExperienceHungerMultiplier;
							_current_value += ThirstInPercent * ExperienceThirstMultiplier;
							_current_value += ( ( 100 - StaminaInPercent ) * ExperienceStaminaMultiplier );
							_current_value += ( ( 100 - HealthInPercent ) * ExperienceHealthMultiplier );
						}
						
						if( IsDead )
							_current_value = 0;
						
						return FixedPercent( _current_value );
					}
				}

				public void AddNosiness( float _value )
				{
					Nosiness += _value;
					Nosiness = FixedPercent( Nosiness );
				}
				
				public float NosinessInPercent
				{
					get{
						float _default_value = 100;
						float _current_value = Nosiness;
						
						if( _default_value > 0 )
							_current_value = ( 100 / _default_value ) * _current_value;
						else
							_current_value = 0;
						
						if( UseAdvanced )
						{
							if( UseEnvironmentTemperature )
								_current_value += TemperatureDeviationInPercent * NosinessTemperaturMultiplier;
							
							if( UseAging )
								_current_value += ( 100 - LifespanInPercent ) * NosinessAgeMultiplier;
							
							_current_value += DamageInPercent * NosinessDamageMultiplier;
							_current_value += StressInPercent * NosinessStressMultiplier;
							_current_value += DebilityInPercent * NosinessDebilityMultiplier;
							_current_value += HungerInPercent * NosinessHungerMultiplier;
							_current_value += ThirstInPercent * NosinessThirstMultiplier;
							_current_value += ( ( 100 - StaminaInPercent ) * NosinessStaminaMultiplier );
							_current_value += ( ( 100 - HealthInPercent ) * NosinessHealthMultiplier );
						}
						
						if( IsDead )
							_current_value = 0;
						
						return FixedPercent( _current_value );
					}
				}

				public float FitnessInPercent
				{
					get{
						float max_fitness = ( DefaultPower + DefaultHealth + DefaultStamina ) / 3;
						float current_fitness = ( ( DefaultPower * PowerInPercent / 100 ) + ( DefaultHealth * HealthInPercent / 100 ) + ( DefaultStamina * StaminaInPercent / 100 ) ) / 3;

						if( ! UseAdvanced )
						{
							max_fitness = DefaultHealth;
							current_fitness = ( DefaultHealth * HealthInPercent / 100 );
						}

						if( max_fitness > 0 )
							current_fitness = ( 100 / max_fitness ) * current_fitness;
						else
							current_fitness = 0;

						if( IsDead )
							current_fitness = 0;

						return FixedPercent( current_fitness );
					}
				}


				public void UpdateBegin( Vector3 _velocity )
				{
					m_Lifetime += Time.deltaTime;
					m_TotalLifetime += Time.deltaTime;

					Odour.HandleOdourMarker( m_Owner.transform );
			
					// if respawn is required the creature is dead and doesn't need anymore information 
					if( m_RespawnRequired )
					{
						m_RespawnTimer += Time.deltaTime;
						m_ReactionTimer = 0;
						m_PerceptionTimer = 0;
					}
					else
					{
						m_ReactionTimer += Time.deltaTime;
						m_PerceptionTimer += Time.deltaTime;

						if( UseAging )
						{
							m_Age +=  Time.deltaTime;

							if( m_Age >= MaxAge )
							{
								AddDamage( 100 );
								AddStress( 100 );
								AddHunger( 100 );
								AddThirst( 100 );
								AddDebility( 100 );

								AddAggressivity( 0 );
								AddAnxiety( 0 );
								AddExperience( 0 );
								AddNosiness( 0 );
							}
						}
					}
				}

				public void FixedUpdate()
				{
				}

				float FixedPercent( float _value )
				{
					/*
					float _f = 0.05f;
					_value = (int)(_value/_f+0.5f)*_f;*/
					if( _value < 0 ) _value = 0;
					if( _value > 100 ) _value = 100;

					return (float)System.Math.Round( _value, 2 );
				}

				float FixedMultiplier( float _value )
				{
					if( _value < 0 ) _value = 0;
					if( _value > 1 ) _value = 1;
					
					return _value;
				}

				public void ResetDefaultValues()
				{
					SetAge( 0 );
					MaxAge = 3600;
					
					ComfortEnvironmentTemperature = 25;
					MinEnvironmentTemperature = -25;
					MaxEnvironmentTemperature = 50;
					UseArmor = false;
					ArmorInPercent = 100;
					
					PerceptionTime = 0.5f;
					PerceptionTimeFitnessMultiplier = 0;
					ReactionTime = 0.5f;
					ReactionTimeFitnessMultiplier = 0;
					RespawnDelay = 20;

					CalculateRandomStatusValues( CreatureTrophicLevelType.UNDEFINED );
				}

				public void CalculateRandomStatusValues( CreatureTrophicLevelType _type )
				{
					if( _type == CreatureTrophicLevelType.CARNIVORE )
					{
						if( UseAging )
							SetAge( Random.Range( 0, MaxAge ) );

						DamageInPercent = Random.Range( 0,25 );
						StressInPercent = Random.Range( 0,10 );
						DebilityInPercent = Random.Range( 0.05f,10 ); 
						HungerInPercent = Random.Range( 0.25f,50f );
						ThirstInPercent = Random.Range( 0.25f,50f ); 
						
						DefaultSenseVisual = 100; 
						SenseVisualAgeMultiplier = 0.0f;
						SenseVisualFitnessMultiplier = 0.0f;
						
						DefaultSenseAuditory = 100;
						SenseAuditoryAgeMultiplier = 0.0f;
						SenseAuditoryFitnessMultiplier = 0.0f;
						
						DefaultSenseOlfactory = 100; 
						SenseOlfactoryAgeMultiplier = 0.0f;
						SenseOlfactoryFitnessMultiplier = 0.0f;
						
						DefaultSenseGustatory = 100; 
						SenseGustatoryAgeMultiplier = 0.0f;
						SenseGustatoryFitnessMultiplier = 0.0f;
						
						DefaultSenseTactile = 100; 
						SenseTouchAgeMultiplier = 0.0f;
						SenseTouchFitnessMultiplier = 0.0f;

						// CHARACTER
						Aggressivity = Random.Range( 50,95 );
						DefaultAggressivity = Aggressivity;
							AggressivityHealthMultiplier = Random.Range( -0.25f,-0.05f );
							AggressivityStaminaMultiplier = Random.Range( 0.25f,0.45f );
							AggressivityDamageMultiplier = Random.Range( 0.55f,0.75f );
							AggressivityStressMultiplier = Random.Range( 0.50f,0.75f );
							AggressivityDebilityMultiplier = Random.Range( 0.25f,0.45f );
							AggressivityHungerMultiplier = Random.Range( 0.05f,0.25f );
							AggressivityThirstMultiplier = Random.Range( 0.05f,0.25f );
							AggressivityAgeMultiplier = Random.Range( 0.2f,0.55f );
							AggressivityTemperaturMultiplier = Random.Range( 0.2f,0.35f );
						
						Anxiety = Random.Range( 5,15 );
						DefaultAnxiety = Anxiety;
							AnxietyHealthMultiplier = Random.Range( 0.25f,0.45f );
							AnxietyStaminaMultiplier = Random.Range( 0.25f,0.45f );
							AnxietyDamageMultiplier  = Random.Range( 0.25f,0.45f );
							AnxietyStressMultiplier = Random.Range( 0.25f,0.45f );
							AnxietyDebilityMultiplier = Random.Range( 0.25f,0.45f );
							AnxietyHungerMultiplier = Random.Range( 0.25f,0.45f );
							AnxietyThirstMultiplier = Random.Range( 0.25f,0.45f );
							AnxietyAgeMultiplier = Random.Range( 0.25f,0.45f );
							AnxietyTemperaturMultiplier = Random.Range( 0.25f,0.45f );
						
						Experience = Random.Range( 25,45 );
						DefaultExperience = Experience;
							ExperienceHealthMultiplier = Random.Range( 0.25f,0.45f );
							ExperienceStaminaMultiplier = Random.Range( 0.25f,0.45f );
							ExperienceDamageMultiplier  = Random.Range( 0.25f,0.45f );
							ExperienceStressMultiplier = Random.Range( 0.25f,0.45f );
							ExperienceDebilityMultiplier = Random.Range( 0.25f,0.45f );
							ExperienceHungerMultiplier = Random.Range( 0.25f,0.45f );
							ExperienceThirstMultiplier = Random.Range( 0.25f,0.45f );
							ExperienceAgeMultiplier = Random.Range( 0.25f,0.45f );
							ExperienceTemperaturMultiplier = Random.Range( 0.25f,0.45f );
						
						Nosiness = Random.Range( 75,100 );
						DefaultNosiness = Nosiness;
							NosinessHealthMultiplier = Random.Range( 0.25f,0.45f );
							NosinessStaminaMultiplier = Random.Range( 0.25f,0.45f );
							NosinessDamageMultiplier  = Random.Range( 0.25f,0.45f );
							NosinessStressMultiplier = Random.Range( 0.25f,0.45f );
							NosinessDebilityMultiplier = Random.Range( 0.25f,0.45f );
							NosinessHungerMultiplier = Random.Range( 0.25f,0.45f );
							NosinessThirstMultiplier = Random.Range( 0.25f,0.45f );
							NosinessAgeMultiplier = Random.Range( 0.25f,0.45f );
							NosinessTemperaturMultiplier = Random.Range( 0.25f,0.45f );

						// VITAL SIGNS
						DefaultHealth = 100;
							HealthDamageMultiplier = Random.Range( 0.5f,0.9f );
							HealthStressMultiplier = Random.Range( 0.35f,0.55f );
							HealthDebilityMultiplier = Random.Range( 0.15f,0.25f );
							HealthHungerMultiplier = Random.Range( 0.55f,0.75f );
							HealthThirstMultiplier = Random.Range( 0.65f,0.80f );
							HealthAgeMultiplier = Random.Range( 0.5f,0.75f );
							HealthTemperaturMultiplier = Random.Range( 0.25f,0.35f );
						
						DefaultStamina = 100;
							StaminaHealthMultiplier = Random.Range( 0.15f,0.25f );
							StaminaDamageMultiplier = Random.Range( 0.25f,0.35f );
							StaminaStressMultiplier = Random.Range( 0.15f,0.25f );
							StaminaDebilityMultiplier = Random.Range( 0.15f,0.25f );
							StaminaHungerMultiplier = Random.Range( 0.15f,0.25f );
							StaminaThirstMultiplier = Random.Range( 0.15f,0.25f );
							StaminaAgeMultiplier = Random.Range( 0.5f,0.75f );
							StaminaTemperaturMultiplier = Random.Range( 0.15f,0.25f );
						
						DefaultPower = 100;
							PowerHealthMultiplier = Random.Range( 0.15f,0.25f );
							PowerStaminaMultiplier = Random.Range( 0.15f,0.25f );
							PowerDamageMultiplier  = Random.Range( 0.15f,0.25f );
							PowerStressMultiplier = Random.Range( 0.15f,0.25f );
							PowerDebilityMultiplier = Random.Range( 0.15f,0.25f );
							PowerHungerMultiplier = Random.Range( 0.15f,0.25f );
							PowerThirstMultiplier = Random.Range( 0.15f,0.25f );
							PowerAgeMultiplier = Random.Range( 0.5f,0.75f );
							PowerTemperaturMultiplier = Random.Range( 0.15f,0.25f );
						

					}
					else if( _type == CreatureTrophicLevelType.HERBIVORE )
					{
					}
					else if( _type == CreatureTrophicLevelType.OMNIVORES )
					{
					}
					else 
					{
						DamageInPercent = 0;
						StressInPercent = 0;
						DebilityInPercent = 0; 
						HungerInPercent = 0;
						ThirstInPercent = 0; 
						
						DefaultSenseVisual = 100; 
						SenseVisualAgeMultiplier = 0.0f;
						SenseVisualFitnessMultiplier = 0.0f;
						
						DefaultSenseAuditory = 100;
						SenseAuditoryAgeMultiplier = 0.0f;
						SenseAuditoryFitnessMultiplier = 0.0f;
						
						DefaultSenseOlfactory = 100; 
						SenseOlfactoryAgeMultiplier = 0.0f;
						SenseOlfactoryFitnessMultiplier = 0.0f;
						
						DefaultSenseGustatory = 100; 
						SenseGustatoryAgeMultiplier = 0.0f;
						SenseGustatoryFitnessMultiplier = 0.0f;
						
						DefaultSenseTactile = 100; 
						SenseTouchAgeMultiplier = 0.0f;
						SenseTouchFitnessMultiplier = 0.0f;
						
						Aggressivity = 25;
						DefaultAggressivity = 25;
							AggressivityHealthMultiplier = 0;
							AggressivityStaminaMultiplier = 0;
							AggressivityDamageMultiplier = 0;
							AggressivityStressMultiplier = 0;
							AggressivityDebilityMultiplier = 0;
							AggressivityHungerMultiplier = 0;
							AggressivityThirstMultiplier = 0;
							AggressivityAgeMultiplier = 0;
							AggressivityTemperaturMultiplier = 0;
						
						Anxiety = 0;
						DefaultAnxiety = 0;
							AnxietyHealthMultiplier = 0;
							AnxietyStaminaMultiplier = 0;
							AnxietyDamageMultiplier  = 0;
							AnxietyStressMultiplier = 0;
							AnxietyDebilityMultiplier = 0;
							AnxietyHungerMultiplier = 0;
							AnxietyThirstMultiplier = 0;
							AnxietyAgeMultiplier = 0;
							AnxietyTemperaturMultiplier = 0;
						
						Experience = 0;
						DefaultExperience = 0;
							ExperienceHealthMultiplier = 0;
							ExperienceStaminaMultiplier = 0;
							ExperienceDamageMultiplier  = 0;
							ExperienceStressMultiplier = 0;
							ExperienceDebilityMultiplier = 0;
							ExperienceHungerMultiplier = 0;
							ExperienceThirstMultiplier = 0;
							ExperienceAgeMultiplier = 0;
							ExperienceTemperaturMultiplier = 0;
						
						Nosiness = 0;
						DefaultNosiness = 0;
							NosinessHealthMultiplier = 0;
							NosinessStaminaMultiplier = 0;
							NosinessDamageMultiplier  = 0;
							NosinessStressMultiplier = 0;
							NosinessDebilityMultiplier = 0;
							NosinessHungerMultiplier = 0;
							NosinessThirstMultiplier = 0;
							NosinessAgeMultiplier = 0;
							NosinessTemperaturMultiplier = 0;
						
						DefaultHealth = 100;
							HealthDamageMultiplier = 1;
							HealthStressMultiplier = 0;
							HealthDebilityMultiplier = 0;
							HealthHungerMultiplier = 0;
							HealthThirstMultiplier = 0;
							HealthAgeMultiplier = 0;
							HealthTemperaturMultiplier = 0;
						
						DefaultStamina = 100;
							StaminaHealthMultiplier = 0;
							StaminaDamageMultiplier = 0;
							StaminaStressMultiplier = 0;							
							StaminaDebilityMultiplier = 0;
							StaminaHungerMultiplier = 0;
							StaminaThirstMultiplier = 0;
							StaminaAgeMultiplier = 0;
							StaminaTemperaturMultiplier = 0;
						
						DefaultPower = 100;
							PowerHealthMultiplier = 0;
							PowerStaminaMultiplier = 0;
							PowerDamageMultiplier  = 0;
							PowerStressMultiplier = 0;
							PowerDebilityMultiplier = 0;
							PowerHungerMultiplier = 0;
							PowerThirstMultiplier = 0;
							PowerAgeMultiplier = 0;
							PowerTemperaturMultiplier = 0;
						

					}
				}

			}
		}
	}
}
