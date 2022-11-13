// ##############################################################################
//
// ice_CreatureEditorStatus.cs
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
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using ICE;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Styles;
using ICE.Layouts;
using ICE.Creatures.EditorInfos;
using ICE.Utilities.EnumTypes;
using ICE.Utilities;


namespace ICE.Creatures.EditorHandler
{
	
	public static class EditorStatus
	{	
		public static void Print( ICECreatureControl _control ){
			
			if( ! _control.Display.ShowStatus )
				return;
			
			ICEEditorStyle.SplitterByIndent( 0 );
			ICEEditorLayout.BeginHorizontal();
				_control.Display.FoldoutStatus = ICEEditorLayout.Foldout( _control.Display.FoldoutStatus, "Status" );
				if (GUILayout.Button("SAVE", ICEEditorStyle.ButtonMiddle ))
					CreatureIO.SaveStatusToFile( _control.Creature.Status, _control.gameObject.name );
				if (GUILayout.Button("LOAD", ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Status = CreatureIO.LoadStatusFromFile( _control.Creature.Status );				
				if (GUILayout.Button("RESET", ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Status.ResetDefaultValues();
				_control.Creature.Status.UseAdvanced = ICEEditorLayout.ButtonCheck( "ADVANCED", "", _control.Creature.Status.UseAdvanced, ICEEditorStyle.ButtonMiddle );
			ICEEditorLayout.EndHorizontal( Info.STATUS );
			
			if( _control.Display.FoldoutStatus ) 
			{
				EditorGUILayout.Separator();
				
				DrawStatus( _control );
				DrawStatusBasics( _control );
				DrawStatusAdvanced( _control );
				DrawStatusMemory( _control );
				DrawStatusInventory( _control );
	
				EditorGUILayout.Separator();
			}
		}

		private static void DrawStatus( ICECreatureControl _control )
		{
			if( ! _control.Display.ShowStatus )
				return;

			EditorGUI.indentLevel++;

			_control.Display.FoldoutStatusInfos = ICEEditorLayout.Foldout( _control.Display.FoldoutStatusInfos, "Infos", Info.STATUS_INFLUENCE_INDICATORS );				
			if( _control.Display.FoldoutStatusInfos )
			{
				if( _control.Creature.Status.UseAdvanced )
				{
					if( _control.Creature.Status.UseAging )
					{
						EditorGUILayout.LabelField( "Age", "Current age : " + (int)(_control.Creature.Status.Age/60) + "min. / Max. age : " + (int)(_control.Creature.Status.MaxAge/60) + "min." );
						EditorGUI.indentLevel++;					
							ICEEditorLayout.DrawProgressBar("Lifespan", _control.Creature.Status.LifespanInPercent);					
						EditorGUI.indentLevel--;
						EditorGUILayout.Separator();
					}					

					ICEEditorLayout.DrawProgressBar("Senses (major sensory signs)", _control.Creature.Status.SensesInPercent);				
					EditorGUILayout.Separator();				
					EditorGUI.indentLevel++;				
						ICEEditorLayout.DrawProgressBar("Visual", _control.Creature.Status.SenseVisualInPercent );
						ICEEditorLayout.DrawProgressBar("Auditory", _control.Creature.Status.SenseAuditoryInPercent );
						ICEEditorLayout.DrawProgressBar("Olfactory", _control.Creature.Status.SenseOlfactoryInPercent );	
						ICEEditorLayout.DrawProgressBar("Gustatory", _control.Creature.Status.SenseGustatoryInPercent );	
						ICEEditorLayout.DrawProgressBar("Touch", _control.Creature.Status.SenseTouchInPercent );	
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
					
					ICEEditorLayout.DrawProgressBar("Fitness (major vital sign)", _control.Creature.Status.FitnessInPercent);				
					EditorGUILayout.Separator();				
					EditorGUI.indentLevel++;				
						ICEEditorLayout.DrawProgressBar("Health", _control.Creature.Status.HealthInPercent );
						ICEEditorLayout.DrawProgressBar("Stamina", _control.Creature.Status.StaminaInPercent );
						ICEEditorLayout.DrawProgressBar("Power", _control.Creature.Status.PowerInPercent );				
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();

					ICEEditorLayout.Label( "Character (major character traits)", false );
					EditorGUI.indentLevel++;
						ICEEditorLayout.DrawProgressBar("Aggressivity", _control.Creature.Status.AggressivityInPercent );
						ICEEditorLayout.DrawProgressBar("Anxiety", _control.Creature.Status.AnxietyInPercent );
						ICEEditorLayout.DrawProgressBar("Experience", _control.Creature.Status.ExperienceInPercent );
						ICEEditorLayout.DrawProgressBar("Nosiness", _control.Creature.Status.NosinessInPercent );
					EditorGUI.indentLevel--;
				}
				else
				{
					ICEEditorLayout.DrawProgressBar("Health", _control.Creature.Status.HealthInPercent );
				}
				EditorGUILayout.Separator();
			}
			
			EditorGUI.indentLevel--;

		}


		private static void DrawStatusInventory( ICECreatureControl _control )
		{
			if( ! _control.Display.ShowStatus )
				return;
			
			EditorGUI.indentLevel++;
			ICEEditorLayout.BeginHorizontal();
				_control.Display.FoldoutInventory = ICEEditorLayout.Foldout( _control.Display.FoldoutInventory, "Inventory", Info.STATUS_INFLUENCE );
				_control.Creature.Status.Inventory.Enabled = ICEEditorLayout.ButtonCheck( "ENABLED", "", _control.Creature.Status.Inventory.Enabled, ICEEditorStyle.ButtonMiddle );
			ICEEditorLayout.EndHorizontal( Info.STATUS_INVENTORY );
			if( _control.Display.FoldoutInventory )
			{
				EditorGUI.BeginDisabledGroup( _control.Creature.Status.Inventory.Enabled == false );
					EditorSharedTools.DrawInventoryObjectContent( _control.gameObject, _control.Creature.Status.Inventory );
				EditorGUI.EndDisabledGroup();
			}
			EditorGUI.indentLevel--;
		}
		
		private static void DrawStatusBasics( ICECreatureControl _control )
		{
			if( ! _control.Display.ShowStatus )
				return;

			EditorGUI.indentLevel++;
			_control.Display.FoldoutStatusBasics = ICEEditorLayout.Foldout( _control.Display.FoldoutStatusBasics, "Basics", Info.STATUS_BASICS );				
			if( _control.Display.FoldoutStatusBasics )
			{
				EditorGUI.indentLevel++;
		
				EditorGUILayout.Separator();	

				_control.Creature.Status.DefaultPerceptionTime = ICEEditorLayout.DefaultSlider( "Perception Time (secs.)","", _control.Creature.Status.DefaultPerceptionTime, Init.STATUS_PERCEPTION_TIME_STEP, Init.STATUS_PERCEPTION_TIME_MIN, Init.STATUS_PERCEPTION_TIME_MAX, Init.STATUS_PERCEPTION_TIME_DEFAULT, Info.STATUS_PERCEPTION_TIME  );

				EditorGUI.indentLevel++;
				_control.Creature.Status.PerceptionTimeVariance = ICEEditorLayout.DefaultSlider("Variance Multiplier","", _control.Creature.Status.PerceptionTimeVariance,0.025f, 0,1, 0.25f, Info.STATUS_PERCEPTION_TIME_VARIANCE );

				if( _control.Creature.Status.UseAdvanced )
				{
					_control.Creature.Status.PerceptionTimeFitnessMultiplier = ICEEditorLayout.DefaultSlider("Fitness Multiplier (inv. +)","", _control.Creature.Status.PerceptionTimeFitnessMultiplier,0.025f, 0,1, 0.3f, Info.STATUS_PERCEPTION_TIME_MULTIPLIER );
					EditorGUILayout.Separator();
				}
				EditorGUI.indentLevel--;

				_control.Creature.Status.DefaultReactionTime = ICEEditorLayout.DefaultSlider("Reaction Time (secs.)","", _control.Creature.Status.DefaultReactionTime, Init.STATUS_REACTION_TIME_STEP, Init.STATUS_REACTION_TIME_MIN, Init.STATUS_REACTION_TIME_MAX, Init.STATUS_REACTION_TIME_DEFAULT, Info.STATUS_REACTION_TIME );

				EditorGUI.indentLevel++;
					_control.Creature.Status.ReactionTimeVariance = ICEEditorLayout.DefaultSlider("Variance Multiplier","", _control.Creature.Status.ReactionTimeVariance,0.025f, 0,1, 0.25f, Info.STATUS_REACTION_TIME_VARIANCE );
				EditorGUI.indentLevel--;

				if( _control.Creature.Status.UseAdvanced )
				{
					EditorGUI.indentLevel++;
					_control.Creature.Status.ReactionTimeFitnessMultiplier = ICEEditorLayout.DefaultSlider("Fitness Multiplier (inv. +)","", _control.Creature.Status.ReactionTimeFitnessMultiplier,0.025f, 0,1, 0.3f, Info.STATUS_REACTION_TIME_MULTIPLIER );
					EditorGUI.indentLevel--;
				}
				else
				{
					EditorGUILayout.Separator();
					_control.Creature.Status.DamageInPercent = ICEEditorLayout.Slider( "Damage","", _control.Creature.Status.DamageInPercent,1, 0,100, Info.STATUS_DAMAGE_IN_PERCENT );
				}

				EditorGUILayout.Separator();

				_control.Creature.Status.FitnessRecreationLimit = ICEEditorLayout.DefaultSlider("Recreation Limit (%)","If the fitness value reached this limit your creature will go home to recreate.", _control.Creature.Status.FitnessRecreationLimit, 0.5f, 0, 100,0, Info.STATUS_FITNESS_RECREATION_LIMIT );
				_control.Creature.Status.RecoveryPhase = ICEEditorLayout.MaxDefaultSlider("Recovery Phase (secs.)","Defines how long the creature will be defenceless after spawning.", _control.Creature.Status.RecoveryPhase, 0.5f, 0, ref _control.Creature.Status.RecoveryPhasePeriodMax, 20, Info.STATUS_RECOVERY_PHASE );

				_control.Creature.Status.RespawnDelay = ICEEditorLayout.MaxDefaultSlider("Respawn Delay (secs.)","Defines how long the creature will be visible after dying and before respawning.", _control.Creature.Status.RespawnDelay, 0.5f, 0, ref _control.Creature.Status.RespawnDelayMax, 20, Info.STATUS_RESPAWN_DELAY );
				EditorGUI.indentLevel++;
				_control.Creature.Status.RespawnDelayVariance = ICEEditorLayout.DefaultSlider("Variance Multiplier","", _control.Creature.Status.RespawnDelayVariance,0.025f, 0,1, 0.25f, Info.STATUS_RESPAWN_DELAY_VARIANCE );
				EditorGUI.indentLevel--;

				_control.Creature.Status.UseCorpse = ICEEditorLayout.Toggle( "Use Corpse", "" , _control.Creature.Status.UseCorpse , Info.STATUS_RESPAWN_CORPSE );

				if( _control.Creature.Status.UseCorpse )
				{
					EditorGUI.indentLevel++;
						_control.Creature.Status.CorpseReferencePrefab = (GameObject)EditorGUILayout.ObjectField( "Corpse Prefab", _control.Creature.Status.CorpseReferencePrefab, typeof(GameObject), false );
					EditorGUI.indentLevel--;
				}
		
				EditorGUI.indentLevel--;
				
				EditorGUILayout.Separator();
			}
			EditorGUI.indentLevel--;
		}

		private static void DrawStatusAdvanced( ICECreatureControl _control )
		{
			if( ! _control.Creature.Status.UseAdvanced )
				return;

			EditorGUI.indentLevel++;
			_control.Display.FoldoutStatusAdvanced = ICEEditorLayout.Foldout( _control.Display.FoldoutStatusAdvanced, "Advanced", Info.STATUS_ADVANCED );				
			if( _control.Display.FoldoutStatusAdvanced )
			{
				EditorGUILayout.Separator();
				EditorGUI.indentLevel++;

					// FEED TYPE BEGIN
					ICEEditorLayout.BeginHorizontal();
					_control.Creature.Status.TrophicLevel = (CreatureTrophicLevelType)ICEEditorLayout.EnumPopup( "Trophic Level","", _control.Creature.Status.TrophicLevel  ); 
					if( ICEEditorLayout.Button( "RND", "", ICEEditorStyle.CMDButtonDouble ) )
						_control.Creature.Status.CalculateRandomStatusValues( _control.Creature.Status.TrophicLevel );
						_control.Creature.Status.UseDynamicInitialisation = ICEEditorLayout.ButtonCheck( "DYN", "", _control.Creature.Status.UseDynamicInitialisation, ICEEditorStyle.CMDButtonDouble );
					ICEEditorLayout.EndHorizontal( Info.STATUS_FEEDTYPE );
					EditorGUI.indentLevel++;
						if( _control.Creature.Status.TrophicLevel == CreatureTrophicLevelType.OMNIVORES || _control.Creature.Status.TrophicLevel == CreatureTrophicLevelType.CARNIVORE )
							_control.Creature.Status.IsCannibal = ICEEditorLayout.Toggle( "Is Cannibal","", _control.Creature.Status.IsCannibal, Info.STATUS_FEEDTYPE_CANNIBAL  ); 
						else
							_control.Creature.Status.IsCannibal = false;
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
					// FEED TYPE END

					EditorSharedTools.DrawOdourObject( "Odour", "", _control.Creature.Status.Odour , "" );


					_control.Creature.Status.UseAging = ICEEditorLayout.Toggle("Use Aging","", _control.Creature.Status.UseAging, Info.STATUS_AGING );			
					if( _control.Creature.Status.UseAging )
					{					
						EditorGUI.indentLevel++;
							float _age = _control.Creature.Status.Age/60;
							float _max_age = _control.Creature.Status.MaxAge/60;
							_control.Creature.Status.SetAge( ICEEditorLayout.MaxDefaultSlider( "Age (minutes)", "", _age , 1, 0, ref _max_age, 0, Info.STATUS_AGING_AGE )*60 );
							_control.Creature.Status.MaxAge = _max_age*60;
						EditorGUI.indentLevel--;
						EditorGUILayout.Separator();
					}

					_control.Creature.Status.UseEnvironmentTemperature = ICEEditorLayout.Toggle( "Use Environment Temperature","", _control.Creature.Status.UseEnvironmentTemperature, Info.STATUS_TEMPERATURE );			
					if( _control.Creature.Status.UseEnvironmentTemperature )
					{
						EditorGUI.indentLevel++;
							_control.Creature.Status.ComfortEnvironmentTemperature = ICEEditorLayout.Slider( "Comfort Environment Temperature","", _control.Creature.Status.ComfortEnvironmentTemperature, 1,_control.Creature.Status.MinEnvironmentTemperature,_control.Creature.Status.MaxEnvironmentTemperature, Info.STATUS_TEMPERATURE_BEST );
							EditorGUI.indentLevel++;
								ICEEditorLayout.MinMaxGroup( "Temperature Scope", "Minimal and maximal Temperatures", 
									ref _control.Creature.Status.MinEnvironmentTemperature, 
									ref _control.Creature.Status.MaxEnvironmentTemperature,
									_control.Creature.Status.EnvironmentMinTemperature,
									_control.Creature.Status.EnvironmentMaxTemperature,
									1,
					              	Info.STATUS_TEMPERATURE_SCOPE );
							EditorGUI.indentLevel--;
						EditorGUI.indentLevel--;

						EditorGUILayout.Separator();
					}

					_control.Creature.Status.UseArmor = ICEEditorLayout.Toggle( "Use Armor","", _control.Creature.Status.UseArmor, Info.STATUS_ARMOR );			
					if( _control.Creature.Status.UseArmor )
					{
						EditorGUI.indentLevel++;
						_control.Creature.Status.ArmorInPercent = ICEEditorLayout.DefaultSlider( "Armor","", _control.Creature.Status.ArmorInPercent, 1,0,100, 100, Info.STATUS_ARMOR_IN_PERCENT );
						EditorGUI.indentLevel--;
					}

			
					_control.Creature.Status.UseShelter = ICEEditorLayout.Toggle("Use Shelter","", _control.Creature.Status.UseShelter, Info.STATUS_SHELTER );				
					if( _control.Creature.Status.UseShelter )
					{					
						EditorGUI.indentLevel++;

						if( _control.Creature.Status.IsSheltered )
							GUI.backgroundColor = Color.green;						
						_control.Creature.Status.ShelterTag = ICEEditorLayout.Tag( "Shelter Tag", "", _control.Creature.Status.ShelterTag, Info.STATUS_SHELTER_TAG );
						GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
						EditorGUI.indentLevel--;
						EditorGUILayout.Separator();
					}

					_control.Creature.Status.UseIndoor = ICEEditorLayout.Toggle("Use Indoor","", _control.Creature.Status.UseIndoor, Info.STATUS_INDOOR );	
					if( _control.Creature.Status.UseIndoor )
					{					
						EditorGUI.indentLevel++;
							if( _control.Creature.Status.IsIndoor )
								GUI.backgroundColor = Color.green;	
							_control.Creature.Status.ShelterTag = ICEEditorLayout.Tag( "Indoor Tag", "", _control.Creature.Status.IndoorTag, Info.STATUS_INDOOR_TAG );
							GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
						EditorGUI.indentLevel--;
						EditorGUILayout.Separator();
					}

					EditorGUILayout.Separator();
					_control.Display.FoldoutStatusInfluence = ICEEditorLayout.Foldout( _control.Display.FoldoutStatusInfluence, "Initial Influence Indicators", Info.STATUS_INFLUENCE_INDICATORS );				
					if( _control.Display.FoldoutStatusInfluence )
					{
						EditorGUI.indentLevel++;
							_control.Creature.Status.DamageInPercent = ICEEditorLayout.DefaultSlider( "Damage","", _control.Creature.Status.DamageInPercent,0.025f, 0,100,0, Info.STATUS_DAMAGE_IN_PERCENT);
							_control.Creature.Status.StressInPercent = ICEEditorLayout.DefaultSlider("Stress","", _control.Creature.Status.StressInPercent,0.025f, 0,100,0, Info.STATUS_STRESS_IN_PERCENT);
							_control.Creature.Status.DebilityInPercent = ICEEditorLayout.DefaultSlider("Debility","", _control.Creature.Status.DebilityInPercent,0.025f, 0,100,0, Info.STATUS_DEBILITY_IN_PERCENT);
							_control.Creature.Status.HungerInPercent = ICEEditorLayout.DefaultSlider( "Hunger","", _control.Creature.Status.HungerInPercent,0.025f, 0,100,0, Info.STATUS_HUNGER_IN_PERCENT);
							_control.Creature.Status.ThirstInPercent = ICEEditorLayout.DefaultSlider("Thirst","", _control.Creature.Status.ThirstInPercent,0.025f, 0,100,0, Info.STATUS_THIRST_IN_PERCENT);

							EditorGUILayout.Separator();

							_control.Creature.Status.Aggressivity = ICEEditorLayout.DefaultSlider("Aggressivity","", _control.Creature.Status.Aggressivity, 0.025f, 0, 100, _control.Creature.Status.DefaultAggressivity, Info.STATUS_INFLUENCES_AGGRESSIVITY );
							_control.Creature.Status.Experience = ICEEditorLayout.DefaultSlider("Experience","", _control.Creature.Status.Experience, 0.025f, 0, 100, _control.Creature.Status.DefaultExperience, Info.STATUS_INFLUENCES_EXPERIENCE );
							_control.Creature.Status.Nosiness = ICEEditorLayout.DefaultSlider("Nosiness","", _control.Creature.Status.Nosiness, 0.025f, 0, 100, _control.Creature.Status.DefaultNosiness, Info.STATUS_INFLUENCES_NOSINESS );
							_control.Creature.Status.Anxiety = ICEEditorLayout.DefaultSlider("Anxiety","", _control.Creature.Status.Anxiety, 0.025f, 0, 100, _control.Creature.Status.DefaultAnxiety, Info.STATUS_INFLUENCES_ANXIETY );

						EditorGUI.indentLevel--;
						EditorGUILayout.Separator();
					}

					_control.Display.FoldoutStatusSensory = ICEEditorLayout.Foldout( _control.Display.FoldoutStatusSensory, "Sensory Indicators", Info.STATUS_SENSES );				
					if( _control.Display.FoldoutStatusSensory )
					{
						EditorGUI.indentLevel++;
							_control.Creature.Status.DefaultSenseVisual  = (int)ICEEditorLayout.DefaultSlider( "Visual", "Optical sense", _control.Creature.Status.DefaultSenseVisual,0.025f, 0,100,100, Info.STATUS_SENSES_VISUAL );
							EditorGUI.indentLevel++;
							if( _control.Creature.Status.UseAging )
								_control.Creature.Status.SenseVisualAgeMultiplier = ICEEditorLayout.Slider("Age Multiplier (-)","", _control.Creature.Status.SenseVisualAgeMultiplier, 0.025f,0,1);
							_control.Creature.Status.SenseVisualFitnessMultiplier = ICEEditorLayout.Slider("Fitness Multiplier (-)","", _control.Creature.Status.SenseVisualFitnessMultiplier, 0.025f,0,1);
							EditorGUI.indentLevel--;
							EditorGUILayout.Separator();

							_control.Creature.Status.DefaultSenseAuditory = (int)ICEEditorLayout.DefaultSlider( "Auditory", "Hearing sense", _control.Creature.Status.DefaultSenseAuditory,0.025f, 0,100,100, Info.STATUS_SENSES_AUDITORY );
							EditorGUI.indentLevel++;
								if( _control.Creature.Status.UseAging )
									_control.Creature.Status.SenseAuditoryAgeMultiplier = ICEEditorLayout.Slider("Age Multiplier (-)","", _control.Creature.Status.SenseAuditoryAgeMultiplier, 0.025f,0,1);
								_control.Creature.Status.SenseAuditoryFitnessMultiplier = ICEEditorLayout.Slider("Fitness Multiplier (-)","", _control.Creature.Status.SenseAuditoryFitnessMultiplier, 0.025f,0,1);
							EditorGUI.indentLevel--;
							EditorGUILayout.Separator();

							_control.Creature.Status.DefaultSenseOlfactory  = (int)ICEEditorLayout.DefaultSlider( "Olfactory", "Olfactory sense", _control.Creature.Status.DefaultSenseOlfactory,0.025f, 0,100,100, Info.STATUS_SENSES_OLFACTORY );
							EditorGUI.indentLevel++;
								if( _control.Creature.Status.UseAging )
									_control.Creature.Status.SenseOlfactoryAgeMultiplier = ICEEditorLayout.Slider("Age Multiplier (-)","", _control.Creature.Status.SenseOlfactoryAgeMultiplier, 0.025f,0,1);
									_control.Creature.Status.SenseOlfactoryFitnessMultiplier = ICEEditorLayout.Slider("Fitness Multiplier (-)","", _control.Creature.Status.SenseOlfactoryFitnessMultiplier, 0.025f,0,1);
							EditorGUI.indentLevel--;
							EditorGUILayout.Separator();

							_control.Creature.Status.DefaultSenseGustatory  = (int)ICEEditorLayout.DefaultSlider( "Gustatory", "Taste sense", _control.Creature.Status.DefaultSenseGustatory,0.025f, 0,100,100, Info.STATUS_SENSES_GUSTATORY );
							EditorGUI.indentLevel++;
								if( _control.Creature.Status.UseAging )
									_control.Creature.Status.SenseGustatoryAgeMultiplier = ICEEditorLayout.Slider("Age Multiplier (-)","", _control.Creature.Status.SenseGustatoryAgeMultiplier, 0.025f,0,1);
								_control.Creature.Status.SenseGustatoryFitnessMultiplier = ICEEditorLayout.Slider("Fitness Multiplier (-)","", _control.Creature.Status.SenseGustatoryFitnessMultiplier, 0.025f,0,1);
							EditorGUI.indentLevel--;
							EditorGUILayout.Separator();

							_control.Creature.Status.DefaultSenseTactile = (int)ICEEditorLayout.DefaultSlider( "Tactile", "Touch sense", _control.Creature.Status.DefaultSenseTactile,0.025f, 0,100,100, Info.STATUS_SENSES_TACTILE );
							EditorGUI.indentLevel++;
								if( _control.Creature.Status.UseAging )
									_control.Creature.Status.SenseTouchAgeMultiplier = ICEEditorLayout.Slider("Age Multiplier (-)","", _control.Creature.Status.SenseTouchAgeMultiplier, 0.025f,0,1);
								_control.Creature.Status.SenseTouchFitnessMultiplier = ICEEditorLayout.Slider("Fitness Multiplier (-)","", _control.Creature.Status.SenseTouchFitnessMultiplier, 0.025f,0,1);
							EditorGUI.indentLevel--;
							EditorGUILayout.Separator();

						EditorGUI.indentLevel--;
						EditorGUILayout.Separator();
					}
				
					_control.Display.FoldoutStatusVital = ICEEditorLayout.Foldout( _control.Display.FoldoutStatusVital, "Vital Indicators", Info.STATUS_VITAL_INDICATORS );
					if( _control.Display.FoldoutStatusVital )
					{
						EditorGUI.indentLevel++;
							_control.Creature.Status.DefaultHealth = ICEEditorLayout.Integer("Health","", _control.Creature.Status.DefaultHealth, Info.STATUS_VITAL_INDICATOR_HEALTH );
							EditorGUI.indentLevel++;			
								_control.Creature.Status.HealthDamageMultiplier = ICEEditorLayout.Slider("Damage Multiplier (-)","", _control.Creature.Status.HealthDamageMultiplier, 0.025f,0,1);
								_control.Creature.Status.HealthStressMultiplier = ICEEditorLayout.Slider("Stress Multiplier (-)","", _control.Creature.Status.HealthStressMultiplier, 0.025f,0,1);
								_control.Creature.Status.HealthDebilityMultiplier = ICEEditorLayout.Slider("Debility Multiplier (-)","", _control.Creature.Status.HealthDebilityMultiplier, 0.025f,0,1);
								_control.Creature.Status.HealthHungerMultiplier = ICEEditorLayout.Slider("Hunger Multiplier (-)","", _control.Creature.Status.HealthHungerMultiplier, 0.025f,0,1);
								_control.Creature.Status.HealthThirstMultiplier = ICEEditorLayout.Slider("Thirst Multiplier (-)","", _control.Creature.Status.HealthThirstMultiplier, 0.025f,0,1);
								_control.Creature.Status.HealthRecreationMultiplier = ICEEditorLayout.Slider("Recreation Multiplier (+)","", _control.Creature.Status.HealthRecreationMultiplier, 0.025f,0,1);

								if( _control.Creature.Status.UseEnvironmentTemperature )
									_control.Creature.Status.HealthTemperaturMultiplier = ICEEditorLayout.Slider("Temperatur Multiplier (+)","", _control.Creature.Status.HealthTemperaturMultiplier, 0.025f,0,1);
								if( _control.Creature.Status.UseAging )
									_control.Creature.Status.HealthAgeMultiplier = ICEEditorLayout.Slider("Aging Multiplier (+)","", _control.Creature.Status.HealthAgeMultiplier, 0.025f,0,1);
							EditorGUI.indentLevel--;
						
							EditorGUILayout.Separator();			
							_control.Creature.Status.DefaultStamina = ICEEditorLayout.Integer("Stamina","", _control.Creature.Status.DefaultStamina, Info.STATUS_VITAL_INDICATOR_STAMINA);
							EditorGUI.indentLevel++;			
								_control.Creature.Status.StaminaDamageMultiplier = ICEEditorLayout.Slider("Damage Multiplier (-)","", _control.Creature.Status.StaminaDamageMultiplier, 0.025f,0,1);
								_control.Creature.Status.StaminaStressMultiplier = ICEEditorLayout.Slider("Stress Multiplier  (-)","", _control.Creature.Status.StaminaStressMultiplier, 0.025f,0,1);
								_control.Creature.Status.StaminaDebilityMultiplier = ICEEditorLayout.Slider("Debility Multiplier (-)","", _control.Creature.Status.StaminaDebilityMultiplier, 0.025f,0,1);
								_control.Creature.Status.StaminaHealthMultiplier = ICEEditorLayout.Slider("Health Multiplier (-)","", _control.Creature.Status.StaminaHealthMultiplier, 0.025f,0,1);
								_control.Creature.Status.StaminaHungerMultiplier = ICEEditorLayout.Slider("Hunger Multiplier (-)","", _control.Creature.Status.StaminaHungerMultiplier, 0.025f,0,1);
								_control.Creature.Status.StaminaThirstMultiplier = ICEEditorLayout.Slider("Thirst Multiplier (-)","", _control.Creature.Status.StaminaThirstMultiplier, 0.025f,0,1);

								if( _control.Creature.Status.UseEnvironmentTemperature )
									_control.Creature.Status.StaminaTemperaturMultiplier = ICEEditorLayout.Slider("Temperatur Multiplier (+)","", _control.Creature.Status.StaminaTemperaturMultiplier, 0.025f,0,1);
								if( _control.Creature.Status.UseAging )
									_control.Creature.Status.StaminaAgeMultiplier = ICEEditorLayout.Slider("Aging Multiplier (+)","", _control.Creature.Status.StaminaAgeMultiplier, 0.025f,0,1);
							EditorGUI.indentLevel--;
						
							EditorGUILayout.Separator();			
							_control.Creature.Status.DefaultPower = ICEEditorLayout.Integer("Power","", _control.Creature.Status.DefaultPower, Info.STATUS_VITAL_INDICATOR_POWER);
							EditorGUI.indentLevel++;
								_control.Creature.Status.PowerDamageMultiplier = ICEEditorLayout.Slider("Damage Multiplier (-)","", _control.Creature.Status.PowerDamageMultiplier, 0.025f,0,1);
								_control.Creature.Status.PowerStressMultiplier = ICEEditorLayout.Slider("Stress Multiplier (+)","", _control.Creature.Status.PowerStressMultiplier, 0.025f,0,1);
								_control.Creature.Status.PowerDebilityMultiplier = ICEEditorLayout.Slider("Debility Multiplier (-)","", _control.Creature.Status.PowerDebilityMultiplier, 0.025f,0,1);
								_control.Creature.Status.PowerHealthMultiplier = ICEEditorLayout.Slider("Health Multiplier (-)","", _control.Creature.Status.PowerHealthMultiplier, 0.025f,0,1);
								_control.Creature.Status.PowerStaminaMultiplier = ICEEditorLayout.Slider("Stamina Multiplier (-)","", _control.Creature.Status.PowerStaminaMultiplier, 0.025f,0,1);
								_control.Creature.Status.PowerHungerMultiplier = ICEEditorLayout.Slider("Hunger Multiplier (-)","", _control.Creature.Status.PowerHungerMultiplier, 0.025f,0,1);
								_control.Creature.Status.PowerThirstMultiplier = ICEEditorLayout.Slider("Thirst Multiplier (-)","", _control.Creature.Status.PowerThirstMultiplier, 0.025f,0,1);

								if( _control.Creature.Status.UseEnvironmentTemperature )
									_control.Creature.Status.PowerTemperaturMultiplier = ICEEditorLayout.Slider("Temperatur Multiplier (+)","", _control.Creature.Status.PowerTemperaturMultiplier, 0.025f,0,1);
								if( _control.Creature.Status.UseAging )
									_control.Creature.Status.PowerAgeMultiplier = ICEEditorLayout.Slider("Aging Multiplier (+)","", _control.Creature.Status.PowerAgeMultiplier, 0.025f,0,1);
							EditorGUI.indentLevel--;	
						EditorGUI.indentLevel--;
						EditorGUILayout.Separator();
					}

					_control.Display.FoldoutStatusCharacter = ICEEditorLayout.Foldout( _control.Display.FoldoutStatusCharacter, "Character Indicators", Info.STATUS_CHARACTER_INDICATORS );				
					if( _control.Display.FoldoutStatusCharacter )
					{
						EditorGUI.indentLevel++;
							_control.Creature.Status.DefaultAggressivity = ICEEditorLayout.DefaultSlider("Aggressivity","", _control.Creature.Status.DefaultAggressivity, 0.25f, 0, 100, 25, Info.STATUS_CHARACTER_DEFAULT_AGGRESSITY );
							EditorGUI.indentLevel++;			
								_control.Creature.Status.AggressivityDamageMultiplier = ICEEditorLayout.DefaultSlider("Damage Multiplier","", _control.Creature.Status.AggressivityDamageMultiplier,0.025f,-1,1,0);
								_control.Creature.Status.AggressivityStressMultiplier = ICEEditorLayout.DefaultSlider("Stress Multiplier","", _control.Creature.Status.AggressivityStressMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.AggressivityDebilityMultiplier = ICEEditorLayout.DefaultSlider("Debility Multiplier","", _control.Creature.Status.AggressivityDebilityMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.AggressivityHungerMultiplier = ICEEditorLayout.DefaultSlider("Hunger Multiplier","", _control.Creature.Status.AggressivityHungerMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.AggressivityThirstMultiplier = ICEEditorLayout.DefaultSlider("Thirst Multiplier","", _control.Creature.Status.AggressivityThirstMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.AggressivityHealthMultiplier = ICEEditorLayout.DefaultSlider("Health Multiplier","", _control.Creature.Status.AggressivityHealthMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.AggressivityStaminaMultiplier = ICEEditorLayout.DefaultSlider("Stamina Multiplier","", _control.Creature.Status.AggressivityStaminaMultiplier, 0.025f,-1,1,0);
								
								if( _control.Creature.Status.UseEnvironmentTemperature )
									_control.Creature.Status.AggressivityTemperaturMultiplier = ICEEditorLayout.DefaultSlider("Temperatur Multiplier","", _control.Creature.Status.AggressivityTemperaturMultiplier, 0.025f,-1,1,0);
								if( _control.Creature.Status.UseAging )
									_control.Creature.Status.AggressivityAgeMultiplier = ICEEditorLayout.DefaultSlider("Aging Multiplier","", _control.Creature.Status.AggressivityAgeMultiplier, 0.025f,-1,1,0);
							EditorGUI.indentLevel--;

							EditorGUILayout.Separator();
							_control.Creature.Status.DefaultAnxiety = ICEEditorLayout.DefaultSlider("Anxiety","", _control.Creature.Status.DefaultAnxiety, 0.25f, 0, 100, 0, Info.STATUS_CHARACTER_DEFAULT_ANXIETY );
							EditorGUI.indentLevel++;			
								_control.Creature.Status.AnxietyDamageMultiplier = ICEEditorLayout.DefaultSlider("Damage Multiplier","", _control.Creature.Status.AnxietyDamageMultiplier,0.025f,-1,1,0);
								_control.Creature.Status.AnxietyStressMultiplier = ICEEditorLayout.DefaultSlider("Stress Multiplier","", _control.Creature.Status.AnxietyStressMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.AnxietyDebilityMultiplier = ICEEditorLayout.DefaultSlider("Debility Multiplier","", _control.Creature.Status.AnxietyDebilityMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.AnxietyHungerMultiplier = ICEEditorLayout.DefaultSlider("Hunger Multiplier","", _control.Creature.Status.AnxietyHungerMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.AnxietyThirstMultiplier = ICEEditorLayout.DefaultSlider("Thirst Multiplier","", _control.Creature.Status.AnxietyThirstMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.AnxietyHealthMultiplier = ICEEditorLayout.DefaultSlider("Health Multiplier","", _control.Creature.Status.AnxietyHealthMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.AnxietyStaminaMultiplier = ICEEditorLayout.DefaultSlider("Stamina Multiplier","", _control.Creature.Status.AnxietyStaminaMultiplier, 0.025f,-1,1,0);
								
								if( _control.Creature.Status.UseEnvironmentTemperature )
									_control.Creature.Status.AnxietyTemperaturMultiplier = ICEEditorLayout.DefaultSlider("Temperatur Multiplier","", _control.Creature.Status.AnxietyTemperaturMultiplier, 0.025f,-1,1,0);
								if( _control.Creature.Status.UseAging )
									_control.Creature.Status.AnxietyAgeMultiplier = ICEEditorLayout.DefaultSlider("Aging Multiplier","", _control.Creature.Status.AnxietyAgeMultiplier, 0.025f,-1,1,0);
							EditorGUI.indentLevel--;

							EditorGUILayout.Separator();
							_control.Creature.Status.DefaultExperience = ICEEditorLayout.DefaultSlider("Experience","", _control.Creature.Status.DefaultExperience, 0.25f, 0, 100, 0, Info.STATUS_CHARACTER_DEFAULT_EXPERIENCE );
							EditorGUI.indentLevel++;			
								_control.Creature.Status.ExperienceDamageMultiplier = ICEEditorLayout.DefaultSlider("Damage Multiplier","", _control.Creature.Status.ExperienceDamageMultiplier,0.025f,-1,1,0);
								_control.Creature.Status.ExperienceStressMultiplier = ICEEditorLayout.DefaultSlider("Stress Multiplier","", _control.Creature.Status.ExperienceStressMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.ExperienceDebilityMultiplier = ICEEditorLayout.DefaultSlider("Debility Multiplier","", _control.Creature.Status.ExperienceDebilityMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.ExperienceHungerMultiplier = ICEEditorLayout.DefaultSlider("Hunger Multiplier","", _control.Creature.Status.ExperienceHungerMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.ExperienceThirstMultiplier = ICEEditorLayout.DefaultSlider("Thirst Multiplier","", _control.Creature.Status.ExperienceThirstMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.ExperienceHealthMultiplier = ICEEditorLayout.DefaultSlider("Health Multiplier","", _control.Creature.Status.ExperienceHealthMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.ExperienceStaminaMultiplier = ICEEditorLayout.DefaultSlider("Stamina Multiplier","", _control.Creature.Status.ExperienceStaminaMultiplier, 0.025f,-1,1,0);
								
								if( _control.Creature.Status.UseEnvironmentTemperature )
									_control.Creature.Status.ExperienceTemperaturMultiplier = ICEEditorLayout.DefaultSlider("Temperatur Multiplier","", _control.Creature.Status.ExperienceTemperaturMultiplier, 0.025f,-1,1,0);
								if( _control.Creature.Status.UseAging )
									_control.Creature.Status.ExperienceAgeMultiplier = ICEEditorLayout.DefaultSlider("Aging Multiplier","", _control.Creature.Status.ExperienceAgeMultiplier, 0.025f,-1,1,0);
							EditorGUI.indentLevel--;

							EditorGUILayout.Separator();
							_control.Creature.Status.DefaultNosiness = ICEEditorLayout.DefaultSlider("Nosiness","", _control.Creature.Status.DefaultNosiness, 0.25f, 0, 100, 0, Info.STATUS_CHARACTER_DEFAULT_NOSINESS );
							EditorGUI.indentLevel++;			
								_control.Creature.Status.NosinessDamageMultiplier = ICEEditorLayout.DefaultSlider("Damage Multiplier","", _control.Creature.Status.NosinessDamageMultiplier,0.025f,-1,1,0);
								_control.Creature.Status.NosinessStressMultiplier = ICEEditorLayout.DefaultSlider("Stress Multiplier","", _control.Creature.Status.NosinessStressMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.NosinessDebilityMultiplier = ICEEditorLayout.DefaultSlider("Debility Multiplier","", _control.Creature.Status.NosinessDebilityMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.NosinessHungerMultiplier = ICEEditorLayout.DefaultSlider("Hunger Multiplier","", _control.Creature.Status.NosinessHungerMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.NosinessThirstMultiplier = ICEEditorLayout.DefaultSlider("Thirst Multiplier","", _control.Creature.Status.NosinessThirstMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.NosinessHealthMultiplier = ICEEditorLayout.DefaultSlider("Health Multiplier","", _control.Creature.Status.NosinessHealthMultiplier, 0.025f,-1,1,0);
								_control.Creature.Status.NosinessStaminaMultiplier = ICEEditorLayout.DefaultSlider("Stamina Multiplier","", _control.Creature.Status.NosinessStaminaMultiplier, 0.025f,-1,1,0);
								
								if( _control.Creature.Status.UseEnvironmentTemperature )
									_control.Creature.Status.NosinessTemperaturMultiplier = ICEEditorLayout.DefaultSlider("Temperatur Multiplier","", _control.Creature.Status.NosinessTemperaturMultiplier, 0.025f,-1,1,0);
								if( _control.Creature.Status.UseAging )
									_control.Creature.Status.NosinessAgeMultiplier = ICEEditorLayout.DefaultSlider("Aging Multiplier","", _control.Creature.Status.NosinessAgeMultiplier, 0.025f,-1,1,0);
							EditorGUI.indentLevel--;
						EditorGUI.indentLevel--;
						EditorGUILayout.Separator();
					}

					_control.Display.FoldoutStatusDynamicInfluences = ICEEditorLayout.Foldout( _control.Display.FoldoutStatusDynamicInfluences, "Dynamic Influences", Info.STATUS_DYNAMIC_INFLUENCES );
					if( _control.Display.FoldoutStatusDynamicInfluences )
					{
						EditorGUI.indentLevel++;
							EditorGUILayout.LabelField( "Fitness" );
							EditorGUI.indentLevel++;
								_control.Creature.Status.FitnessSpeedMultiplier = ICEEditorLayout.Slider("Velocity Multiplier (-)","", _control.Creature.Status.FitnessSpeedMultiplier, 0.025f,0,1);
							EditorGUI.indentLevel--;
						EditorGUI.indentLevel--;
					}

				EditorGUI.indentLevel--;
			}
			EditorGUI.indentLevel--;

		}

		private static void DrawStatusMemory( ICECreatureControl _control )
		{
			EditorGUI.indentLevel++;
			_control.Display.FoldoutStatusMemory = ICEEditorLayout.Foldout( _control.Display.FoldoutStatusMemory, "Memory", Info.STATUS_MEMORY );
			if( _control.Display.FoldoutStatusMemory )
			{
				EditorGUI.indentLevel++;
					float _max_capacity = _control.Creature.Status.SpatialMemory.CapacityMax;
					_control.Creature.Status.SpatialMemory.Capacity = (int)ICEEditorLayout.MaxDefaultSlider( "Spatial Memory", "", _control.Creature.Status.SpatialMemory.Capacity , 1, 0, ref _max_capacity, 0, Info.STATUS_MEMORY_SPATIAL );
					_control.Creature.Status.SpatialMemory.CapacityMax = (int)_max_capacity;

					_max_capacity = _control.Creature.Status.ShortTermMemory.CapacityMax;
					_control.Creature.Status.ShortTermMemory.Capacity = (int)ICEEditorLayout.MaxDefaultSlider( "Short-Term Memory", "", _control.Creature.Status.ShortTermMemory.Capacity , 1, 0, ref _max_capacity, 0, Info.STATUS_MEMORY_SHORT );
					_control.Creature.Status.ShortTermMemory.CapacityMax = (int)_max_capacity;
					
					_max_capacity = _control.Creature.Status.LongTermMemory.CapacityMax;
					_control.Creature.Status.LongTermMemory.Capacity = (int)ICEEditorLayout.MaxDefaultSlider( "Long-Term Memory", "", _control.Creature.Status.LongTermMemory.Capacity , 1, 0, ref _max_capacity, 0, Info.STATUS_MEMORY_LONG );
					_control.Creature.Status.LongTermMemory.CapacityMax = (int)_max_capacity;
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();
			}
			EditorGUI.indentLevel--;
		}
		
	}
}