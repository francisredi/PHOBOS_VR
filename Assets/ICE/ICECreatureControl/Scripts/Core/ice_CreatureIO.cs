﻿// ##############################################################################
//
// ice_CreatureIO.cs
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;

#if UNITY_EDITOR
using UnityEditor;

#endif

using System.Xml;
using System.Xml.Serialization;


namespace ICE
{
	namespace Creatures
	{
		namespace Objects
		{
			#if UNITY_EDITOR
			[System.Serializable]
			public static class CreatureIO : System.Object
			{
				private static string path = "";

				/// <summary>
				/// Saves the creature to file.
				/// </summary>
				/// <param name="status">Status.</param>
				public static void SaveCreatureToFile( CreatureObject _creature, string owner  )
				{
					path = UnityEditor.EditorUtility.SaveFilePanelInProject( "Save File As", owner.ToLower() + ".cc_preset", "cc_preset", "");
					
					if( path.Length == 0 )
						return;
					
					XmlSerializer serializer = new XmlSerializer( typeof( CreatureObject ) );
					FileStream stream = new FileStream( path, FileMode.Create);
					serializer.Serialize( stream, _creature );
					stream.Close();
				}
				
				/// <summary>
				/// Loads the creature from file.
				/// </summary>
				/// <returns>The status from file.</returns>
				/// <param name="status">Status.</param>
				public static CreatureObject LoadCreatureFromFile( CreatureObject _creature )
				{
					path = UnityEditor.EditorUtility.OpenFilePanel( "Open File", Application.dataPath, "cc_preset");
					
					if( path.Length == 0 )
						return _creature;
					
					XmlSerializer serializer = new XmlSerializer(typeof( CreatureObject ));
					FileStream stream = new FileStream( path, FileMode.Open);
					_creature = serializer.Deserialize(stream) as CreatureObject;
					stream.Close();
					
					return _creature;
					
				}

				/// <summary>
				/// Saves the status to file.
				/// </summary>
				/// <param name="status">Status.</param>
				public static void SaveStatusToFile( StatusObject status, string owner  )
				{
					path = UnityEditor.EditorUtility.SaveFilePanelInProject( "Save File As", owner.ToLower() + ".cc_status", "cc_status", "");

					if( path.Length == 0 )
						return;

					XmlSerializer serializer = new XmlSerializer( typeof( StatusObject ) );
					FileStream stream = new FileStream( path, FileMode.Create);
					serializer.Serialize( stream, status );
					stream.Close();
				}

				/// <summary>
				/// Loads the status from file.
				/// </summary>
				/// <returns>The status from file.</returns>
				/// <param name="status">Status.</param>
				public static StatusObject LoadStatusFromFile( StatusObject status )
				{
					path = UnityEditor.EditorUtility.OpenFilePanel( "Open File", Application.dataPath, "status");

					if( path.Length == 0 )
						return status;

					XmlSerializer serializer = new XmlSerializer(typeof( StatusObject ));
					FileStream stream = new FileStream( path, FileMode.Open);
					status = serializer.Deserialize(stream) as StatusObject;
					stream.Close();

					return status;
					
				}

				/// <summary>
				/// Saves the memory to file.
				/// </summary>
				/// <param name="memory">Memory.</param>
				public static void SaveMemoryToFile( MemoryObject memory, string owner  )
				{
					path = UnityEditor.EditorUtility.SaveFilePanelInProject( "Save File As", owner.ToLower() + ".cc_memory", "cc_memory", "");
					
					if( path.Length == 0 )
						return;
					
					XmlSerializer serializer = new XmlSerializer( typeof( MemoryObject ) );
					FileStream stream = new FileStream( path, FileMode.Create);
					serializer.Serialize( stream, memory );
					stream.Close();
				}
				
				/// <summary>
				/// Loads the memory from file.
				/// </summary>
				/// <returns>The memory from file.</returns>
				/// <param name="status">Memory.</param>
				public static MemoryObject LoadMemoryFromFile( MemoryObject memory )
				{
					path = UnityEditor.EditorUtility.OpenFilePanel( "Open File", Application.dataPath, "memory");
					
					if( path.Length == 0 )
						return memory;
					
					XmlSerializer serializer = new XmlSerializer(typeof( MemoryObject ));
					FileStream stream = new FileStream( path, FileMode.Open);
					memory = serializer.Deserialize(stream) as MemoryObject;
					stream.Close();
					
					return memory;
					
				}

				/// <summary>
				/// Saves the behaviour to file.
				/// </summary>
				/// <param name="status">Status.</param>
				public static void SaveBehaviourToFile( BehaviourObject behaviour, string owner )
				{
					path = UnityEditor.EditorUtility.SaveFilePanelInProject( "Save Behaviour File As", owner.ToLower() + ".cc_behaviours", "cc_behaviours", "");
					
					if( path.Length == 0 )
						return;
					
					XmlSerializer serializer = new XmlSerializer( typeof( BehaviourObject ) );
					FileStream stream = new FileStream( path, FileMode.Create);
					serializer.Serialize( stream, behaviour );
					stream.Close();
				}

				/// <summary>
				/// Loads the behaviour from file.
				/// </summary>
				/// <returns>The behaviour from file.</returns>
				/// <param name="behaviour">Behaviour.</param>
				public static BehaviourObject LoadBehaviourFromFile( BehaviourObject behaviour )
				{
					path = UnityEditor.EditorUtility.OpenFilePanel( "Open Behaviour File", Application.dataPath, "cc_behaviours");
					
					if( path.Length == 0 )
						return behaviour;
					
					XmlSerializer serializer = new XmlSerializer(typeof( BehaviourObject ));
					FileStream stream = new FileStream( path, FileMode.Open);
					behaviour = serializer.Deserialize(stream) as BehaviourObject;
					stream.Close();
					
					return behaviour;
					
				}



				/// <summary>
				/// Saves an escort mission to file.
				/// </summary>
				/// <param name="escort">Escort.</param>
				/// <param name="owner">Owner.</param>
				public static void SaveMissionEscortToFile( EscortObject escort, string owner )
				{
					path = UnityEditor.EditorUtility.SaveFilePanelInProject( "Save Escort Mission File As", owner.ToLower() + ".cc_escort", "cc_escort", "");
					
					if( path.Length == 0 )
						return;
					
					XmlSerializer serializer = new XmlSerializer( typeof( EscortObject ) );
					FileStream stream = new FileStream( path, FileMode.Create);
					serializer.Serialize( stream, escort );
					stream.Close();
				}
				
				/// <summary>
				/// Loads an escort mission from file.
				/// </summary>
				/// <returns>The mission escort from file.</returns>
				/// <param name="escort">Escort.</param>
				public static EscortObject LoadMissionEscortFromFile( EscortObject escort )
				{
					path = UnityEditor.EditorUtility.OpenFilePanel( "Open Escort Mission File", Application.dataPath, "cc_escort");
					
					if( path.Length == 0 )
						return escort;
					
					XmlSerializer serializer = new XmlSerializer(typeof( EscortObject ));
					FileStream stream = new FileStream( path, FileMode.Open);
					escort = serializer.Deserialize(stream) as EscortObject;
					stream.Close();
					
					return escort;
					
				}

				/// <summary>
				/// Saves an outpost mission to file.
				/// </summary>
				/// <param name="outpost">Outpost.</param>
				/// <param name="owner">Owner.</param>
				public static void SaveMissionOutpostToFile( OutpostObject _outpost, string owner )
				{
					path = UnityEditor.EditorUtility.SaveFilePanelInProject( "Save Outpost Mission File As", owner.ToLower() + ".cc_outpost", "cc_outpost", "");
					
					if( path.Length == 0 )
						return;
					
					XmlSerializer serializer = new XmlSerializer( typeof( OutpostObject ) );
					FileStream stream = new FileStream( path, FileMode.Create);
					serializer.Serialize( stream, _outpost );
					stream.Close();
				}
				
				/// <summary>
				/// Loads an outpost mission from file.
				/// </summary>
				/// <returns>The mission outpost from file.</returns>
				/// <param name="outpost">Outpost.</param>
				public static OutpostObject LoadMissionOutpostFromFile( OutpostObject _outpost )
				{
					path = UnityEditor.EditorUtility.OpenFilePanel( "Open Outpost Mission File", Application.dataPath, "cc_outpost");
					
					if( path.Length == 0 )
						return _outpost;
					
					XmlSerializer serializer = new XmlSerializer(typeof( OutpostObject ));
					FileStream stream = new FileStream( path, FileMode.Open);
					_outpost = serializer.Deserialize(stream) as OutpostObject;
					stream.Close();
					
					return _outpost;
					
				}

				/// <summary>
				/// Saves an patrol mission to file.
				/// </summary>
				/// <param name="patrol">Patrol.</param>
				/// <param name="owner">Owner.</param>
				public static void SaveMissionPatrolToFile( PatrolObject _patrol, string owner )
				{
					path = UnityEditor.EditorUtility.SaveFilePanelInProject( "Save Outpost Mission File As", owner.ToLower() + ".cc_patrol", "cc_patrol", "");
					
					if( path.Length == 0 )
						return;
					
					XmlSerializer serializer = new XmlSerializer( typeof( PatrolObject ) );
					FileStream stream = new FileStream( path, FileMode.Create);
					serializer.Serialize( stream, _patrol );
					stream.Close();
				}
				
				/// <summary>
				/// Loads an patrol mission from file.
				/// </summary>
				/// <returns>The mission patrol from file.</returns>
				/// <param name="patrol">Patrol.</param>
				public static PatrolObject LoadMissionPatrolFromFile( PatrolObject _patrol )
				{
					path = UnityEditor.EditorUtility.OpenFilePanel( "Open Patrol Mission File", Application.dataPath, "cc_patrol");
					
					if( path.Length == 0 )
						return _patrol;
					
					XmlSerializer serializer = new XmlSerializer(typeof( PatrolObject ));
					FileStream stream = new FileStream( path, FileMode.Open);
					_patrol = serializer.Deserialize(stream) as PatrolObject;
					stream.Close();
					
					return _patrol;
					
				}

				// INTERACTIONS BEGIN
				public static void SaveInteractionToFile( InteractionObject _interaction, string owner )
				{
					path = UnityEditor.EditorUtility.SaveFilePanelInProject( "Save Interaction As", owner.ToLower() + ".cc_interactions", "cc_interactions", "");
					
					if( path.Length == 0 )
						return;
					
					XmlSerializer serializer = new XmlSerializer( typeof( InteractionObject ) );
					FileStream stream = new FileStream( path, FileMode.Create);
					serializer.Serialize( stream, _interaction );
					stream.Close();
				}
				

				public static InteractionObject LoadInteractionFromFile( InteractionObject _interaction )
				{
					path = UnityEditor.EditorUtility.OpenFilePanel( "Open Interaction File", Application.dataPath, "cc_interactions");
					
					if( path.Length == 0 )
						return _interaction;
					
					XmlSerializer serializer = new XmlSerializer(typeof( InteractionObject ));
					FileStream stream = new FileStream( path, FileMode.Open);
					_interaction = serializer.Deserialize(stream) as InteractionObject;
					stream.Close();
					
					return _interaction;
					
				}
				// INTERACTIONS END

				// INTERACTOR BEGIN
				public static void SaveInteractorToFile( InteractorObject _interactor, string owner )
				{
					path = UnityEditor.EditorUtility.SaveFilePanelInProject( "Save Interactor As", owner.ToLower() + ".cc_interactor", "cc_interactor", "");
					
					if( path.Length == 0 )
						return;
					
					XmlSerializer serializer = new XmlSerializer( typeof( InteractorObject ) );
					FileStream stream = new FileStream( path, FileMode.Create);
					serializer.Serialize( stream, _interactor );
					stream.Close();
				}
				
				
				public static InteractorObject LoadInteractorFromFile( InteractorObject _interactor )
				{
					path = UnityEditor.EditorUtility.OpenFilePanel( "Open Interactor File", Application.dataPath, "cc_interactor");
					
					if( path.Length == 0 )
						return _interactor;
					
					XmlSerializer serializer = new XmlSerializer(typeof( InteractorObject ));
					FileStream stream = new FileStream( path, FileMode.Open);
					_interactor = serializer.Deserialize(stream) as InteractorObject;
					stream.Close();
					
					return _interactor;
					
				}
				// INTERACTOR END


				public static void SaveAudioContainerToFile( AudioDataObject _audio, string owner )
				{
					path = UnityEditor.EditorUtility.SaveFilePanelInProject( "Save Audio Data As", owner.ToLower() + ".audio", "audio", "");
					
					if( path.Length == 0 )
						return;
					
					XmlSerializer serializer = new XmlSerializer( typeof( AudioDataObject ) );
					FileStream stream = new FileStream( path, FileMode.Create);
					serializer.Serialize( stream, _audio );
					stream.Close();
				}
				
				
				public static AudioDataObject LoadAudioContainernFromFile( AudioDataObject _audio )
				{
					path = UnityEditor.EditorUtility.OpenFilePanel( "Open Audio Data", Application.dataPath, "audio");
					
					if( path.Length == 0 )
						return _audio;
					
					XmlSerializer serializer = new XmlSerializer(typeof( AudioDataObject ));
					FileStream stream = new FileStream( path, FileMode.Open);
					_audio = serializer.Deserialize(stream) as AudioDataObject;
					stream.Close();
					
					return _audio;
					
				}
			}
		
			#endif
		}
	}
}