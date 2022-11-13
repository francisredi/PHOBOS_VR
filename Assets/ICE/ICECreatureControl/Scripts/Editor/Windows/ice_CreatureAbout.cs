// ##############################################################################
//
// ice_CreatureAbout.cs
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
using UnityEditor;
using ICE.Creatures.EditorInfos;
using ICE.Styles;
using ICE.Layouts;

namespace ICE.Creatures.Windows
{

	public class ice_CreatureAbout : EditorWindow {

		public static Texture2D m_ICECCLogo = (Texture2D)Resources.Load("ICECC_LOGO", typeof(Texture2D));
		public static Texture2D m_ICELogo = (Texture2D)Resources.Load("ICE_LOGO", typeof(Texture2D));

		private static Vector2 m_DialogSize = new Vector2(520, 260);
		private static string m_Version = "Version " + Info.Version;
		private static string m_Copyright = "© " + System.DateTime.Now.Year + " Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.";

		/// <summary>
		/// 
		/// </summary>
		public static void Create()
		{
			
			ice_CreatureAbout msgBox = (ice_CreatureAbout)EditorWindow.GetWindow(typeof(ice_CreatureAbout), true);

			msgBox.titleContent = new GUIContent( "About ICE Creature Control", "");
		
			msgBox.minSize = new Vector2(m_DialogSize.x, m_DialogSize.y);
			msgBox.maxSize = new Vector2(m_DialogSize.x + 1, m_DialogSize.y + 1);
			msgBox.position = new Rect(
				(Screen.currentResolution.width / 2) - (m_DialogSize.x / 2),
				(Screen.currentResolution.height / 2) - (m_DialogSize.y / 2),
				m_DialogSize.x,
				m_DialogSize.y);
			msgBox.Show();
			
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		void OnGUI()
		{
			if( m_ICECCLogo != null )
				GUI.DrawTexture(new Rect(10, 10, m_ICECCLogo.width, m_ICECCLogo.height), m_ICECCLogo);
			
			GUILayout.BeginArea(new Rect(20, 140, Screen.width - 40, Screen.height - 40));
				GUI.backgroundColor = Color.clear;
				GUILayout.Label(m_Version);
				GUILayout.Label(m_Copyright  + "\n\n", ICEEditorStyle.SmallTextStyle );

				if (GUILayout.Button( "Contact Pit Vetterick (Skype:pit.vetterick)", ICEEditorStyle.LinkStyle)) { Application.OpenURL("skype:pit.vetterick?add"); }
				if (GUILayout.Button("https://twitter.com/CreatureAI", ICEEditorStyle.LinkStyle)) { Application.OpenURL("https://twitter.com/CreatureAI"); }
				if (GUILayout.Button("http://www.icecreaturecontrol.com", ICEEditorStyle.LinkStyle)) { Application.OpenURL("http://www.icecreaturecontrol.com"); }
				GUI.color = ICEEditorLayout.DefaultGUIColor;
				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			GUILayout.EndArea();
			

			if( m_ICELogo != null )
				GUI.DrawTexture(new Rect(270, 190, m_ICELogo.width, m_ICELogo.height), m_ICELogo);
			
			
		}
	}
}
