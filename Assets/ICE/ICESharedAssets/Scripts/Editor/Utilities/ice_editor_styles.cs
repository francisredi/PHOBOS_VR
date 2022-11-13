using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ICE.Styles
{


	public static class ICEEditorStyle
	{
		static ICEEditorStyle() {


		}

		private static GUIStyle[] m_SplitterArray = new GUIStyle[10];

		private static GUIStyle m_SplitterFull;
		private static GUIStyle m_SplitterOffset30;

		private static GUIStyle m_Button;
		private static GUIStyle m_InfoButton;
		private static GUIStyle m_CMDButton;
		private static GUIStyle m_CMDButtonDouble;
		private static GUIStyle m_ButtonLarge;
		private static GUIStyle m_ButtonSmall;
		private static GUIStyle m_ButtonMiddle;
		private static GUIStyle m_Foldout;
		private static GUIStyle m_FoldoutNormal;
		private static GUIStyle m_LabelBold;
		private static GUIStyle m_ToggleBold;
		private static GUIStyle m_ButtonExtraLarge;
		private static GUIStyle m_LinkStyle;
		private static GUIStyle m_SmallTextStyle;
		private static GUIStyle m_Popup;
		private static GUIStyle m_ButtonFlex;


		private static readonly Color m_DefaultLineColor = EditorGUIUtility.isProSkin ? new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);
		private static readonly Color m_DefaultTextColor = EditorGUIUtility.isProSkin ? new Color(0.157f, 0.157f, 0.157f) : new Color(0.0f, 0.0f, 0.0f);

		public static Color DefaultLineColor{
			get{return m_DefaultLineColor;}
		}

		public static Color DefaultTextColor{
			get{return m_DefaultTextColor;}
		}

		public static GUIStyle SmallTextStyle
		{
			get
			{
				if( m_SmallTextStyle == null )
				{
					m_SmallTextStyle = new GUIStyle("Label");
					m_SmallTextStyle.fontSize = 9;
				}
				return m_SmallTextStyle;
			}
		}
		
		public static GUIStyle LinkStyle
		{
			get
			{
				if( m_LinkStyle == null )
				{
					m_LinkStyle = new GUIStyle("Label");
					m_LinkStyle.normal.textColor = (EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 1.0f, 1.0f) : Color.blue);
				}
				return m_LinkStyle;
			}
		}

		public static GUIStyle SplitterFull
		{
			get
			{
				if (m_SplitterFull == null)
				{
					m_SplitterFull = new GUIStyle();
					m_SplitterFull.normal.background = EditorGUIUtility.whiteTexture;
					m_SplitterFull.stretchWidth = true;
					m_SplitterFull.margin = new RectOffset(0, 0, 7, 7);
					
				}
				return m_SplitterFull;
			}
		}

		public static GUIStyle SplitterOffset30
		{
			get
			{
				if (m_SplitterOffset30 == null)
				{
					m_SplitterOffset30 = new GUIStyle();
					m_SplitterOffset30.normal.background = EditorGUIUtility.whiteTexture;
					m_SplitterOffset30.stretchWidth = true;
					m_SplitterOffset30.margin = new RectOffset( EditorGUI.indentLevel * 30 , 0, 7, 7);
					
				}
				return m_SplitterOffset30;
			}
		}

		public static GUIStyle Popup{
			get
			{
				if (m_Popup == null)
				{
					m_Popup = new GUIStyle("Popup");
					m_Popup.fontSize = 8;
					m_Popup.alignment = TextAnchor.MiddleLeft;
					m_Popup.margin.top = 2;
					m_Popup.margin.left = 1;
					m_Popup.margin.right = 1;
					m_Popup.padding = new RectOffset(5, 4, 2, 2);
					m_Popup.fixedHeight = 15;
					
				}
				return m_Popup;
			}

		}

		public static GUIStyle Button( int _width )
		{
			if (m_Button == null)
			{
				m_Button = new GUIStyle("Button");
				m_Button.fontSize = 8;
				m_Button.alignment = TextAnchor.MiddleCenter;
				m_Button.margin.top = 2;
				m_Button.margin.left = 1;
				m_Button.margin.right = 1;
				m_Button.padding = new RectOffset(0, 4, 0, 2);
				m_Button.fixedWidth = _width;
				m_Button.fixedHeight = 15;
			
			}
			return m_Button;
		}

		public static GUIStyle CMDButtonDouble
		{
			get
			{
				if (m_CMDButtonDouble == null)
				{
					m_CMDButtonDouble = new GUIStyle("Button");
					m_CMDButtonDouble.fontSize = 8;
					m_CMDButtonDouble.alignment = TextAnchor.MiddleCenter;
					m_CMDButtonDouble.margin.top = 2;
					m_CMDButtonDouble.margin.left = 1;
					m_CMDButtonDouble.margin.right = 1;
					m_CMDButtonDouble.padding = new RectOffset(0, 0, 0, 2);
					m_CMDButtonDouble.fixedWidth = 30;
					m_CMDButtonDouble.fixedHeight = 15;
				}
				return m_CMDButtonDouble;
			}
		}

		public static GUIStyle ButtonExtraLarge
		{
			get
			{
				if (m_ButtonExtraLarge == null)
				{
					m_ButtonExtraLarge = new GUIStyle("Button");
					//m_ButtonExtraLarge.fontSize = 8;
					m_ButtonExtraLarge.fontStyle = FontStyle.Bold;
					m_ButtonExtraLarge.alignment = TextAnchor.MiddleCenter;
					//m_ButtonExtraLarge.margin.top = 1;
					m_ButtonExtraLarge.margin.left = 1;
					m_ButtonExtraLarge.margin.right = 1;
					m_ButtonExtraLarge.padding = new RectOffset(10, 10, 0, 2);
					//m_ButtonExtraLarge.fixedWidth = 15;
					m_ButtonExtraLarge.fixedHeight = 25;
				}
				return m_ButtonExtraLarge;
			}
		}

		public static GUIStyle CMDButton
		{
			get
			{
				if (m_CMDButton == null)
				{
					m_CMDButton = new GUIStyle("Button");
					m_CMDButton.fontSize = 8;
					m_CMDButton.alignment = TextAnchor.MiddleCenter;
					m_CMDButton.margin.top = 2;
					m_CMDButton.margin.left = 1;
					m_CMDButton.margin.right = 1;
					m_CMDButton.padding = new RectOffset(0, 0, 0, 2);
					m_CMDButton.fixedWidth = 15;
					m_CMDButton.fixedHeight = 15;
				}
				return m_CMDButton;
			}
		}

		public static GUIStyle InfoButton
		{
			get
			{
				if (m_InfoButton == null)
				{
					m_InfoButton = new GUIStyle("Button");
					m_InfoButton.fontSize = 10;
					m_InfoButton.fontStyle = FontStyle.Bold;
					m_InfoButton.alignment = TextAnchor.MiddleCenter;
					m_InfoButton.margin.top = 2;
					m_InfoButton.margin.left = 0;
					m_InfoButton.margin.right = 0;
					m_InfoButton.padding = new RectOffset(0, 0, 0, 1);
					m_InfoButton.fixedWidth = 15;
					m_InfoButton.fixedHeight = 15;
				}
				return m_InfoButton;
			}
		}

		public static GUIStyle ButtonFlex
		{
			get
			{
				if (m_ButtonFlex == null)
				{
					m_ButtonFlex = new GUIStyle("Button");
					m_ButtonFlex.fontSize = 8;
					m_ButtonFlex.alignment = TextAnchor.MiddleCenter;
					m_ButtonFlex.margin.top = 2;
					m_ButtonFlex.margin.left = 1;
					m_ButtonFlex.margin.right = 1;
					m_ButtonFlex.padding = new RectOffset(0, 0, 0, 2);
					m_ButtonFlex.fixedHeight = 15;
					m_ButtonFlex.stretchWidth = true;
			
				}
				return m_ButtonFlex;
			}
		}

		public static GUIStyle ButtonLarge
		{
			get
			{
				if (m_ButtonLarge == null)
				{
					m_ButtonLarge = new GUIStyle("Button");
					m_ButtonLarge.fontSize = 8;
					m_ButtonLarge.alignment = TextAnchor.MiddleCenter;
					m_ButtonLarge.margin.top = 2;
					m_ButtonLarge.margin.left = 1;
					m_ButtonLarge.margin.right = 1;
					m_ButtonLarge.padding = new RectOffset(0, 0, 0, 2);
					m_ButtonLarge.fixedWidth = 100;
					m_ButtonLarge.fixedHeight = 15;
				}
				return m_ButtonLarge;
			}
		}

		public static GUIStyle ButtonMiddle
		{
			get
			{
				if (m_ButtonMiddle == null)
				{
					m_ButtonMiddle = new GUIStyle("Button");
					m_ButtonMiddle.fontSize = 8;
					m_ButtonMiddle.alignment = TextAnchor.MiddleCenter;
					m_ButtonMiddle.margin.top = 2;
					m_ButtonMiddle.margin.left = 1;
					m_ButtonMiddle.margin.right = 1;
					m_ButtonMiddle.padding = new RectOffset(0, 0, 0, 2);
					m_ButtonMiddle.fixedWidth = 50;
					m_ButtonMiddle.fixedHeight = 15;
				}
				return m_ButtonMiddle;
			}
		}

		public static GUIStyle ButtonSmall
		{
			get
			{
				if (m_ButtonSmall == null)
				{
					m_ButtonSmall = new GUIStyle("Button");
					m_ButtonSmall.fontSize = 8;
					m_ButtonSmall.alignment = TextAnchor.MiddleCenter;
					m_ButtonSmall.margin.top = 2;
					m_ButtonSmall.margin.left = 1;
					m_ButtonSmall.margin.right = 1;
					m_ButtonSmall.padding = new RectOffset(0, 0, 0, 2);
					m_ButtonSmall.fixedWidth = 25;
					m_ButtonSmall.fixedHeight = 15;
				}
				return m_ButtonSmall;
			}
		}

		public static GUIStyle FoldoutNormal
		{
			get
			{
				if (m_FoldoutNormal == null)
				{
					m_FoldoutNormal = new GUIStyle(EditorStyles.foldout);
					
					m_FoldoutNormal.fontStyle = FontStyle.Normal;
					m_FoldoutNormal.normal.textColor = m_DefaultTextColor;
					m_FoldoutNormal.onNormal.textColor = m_DefaultTextColor;
					m_FoldoutNormal.hover.textColor = m_DefaultTextColor;
					m_FoldoutNormal.onHover.textColor = m_DefaultTextColor;
					m_FoldoutNormal.focused.textColor = m_DefaultTextColor;
					m_FoldoutNormal.onFocused.textColor = m_DefaultTextColor;
					m_FoldoutNormal.active.textColor = m_DefaultTextColor;
					m_FoldoutNormal.onActive.textColor = m_DefaultTextColor;
				}
				return m_FoldoutNormal;
			}
		}

		public static GUIStyle Foldout
		{
			get
			{
				if (m_Foldout == null)
				{
					m_Foldout = new GUIStyle(EditorStyles.foldout);

					m_Foldout.fontStyle = FontStyle.Bold;
					m_Foldout.normal.textColor = m_DefaultTextColor;
					m_Foldout.onNormal.textColor = m_DefaultTextColor;
					m_Foldout.hover.textColor = m_DefaultTextColor;
					m_Foldout.onHover.textColor = m_DefaultTextColor;
					m_Foldout.focused.textColor = m_DefaultTextColor;
					m_Foldout.onFocused.textColor = m_DefaultTextColor;
					m_Foldout.active.textColor = m_DefaultTextColor;
					m_Foldout.onActive.textColor = m_DefaultTextColor;
				}
				return m_Foldout;
			}
		}
		
		public static GUIStyle LabelBold
		{
			get
			{
				if (m_LabelBold == null)
				{
					m_LabelBold = new GUIStyle(EditorStyles.label );
					m_LabelBold.fontStyle = FontStyle.Bold;
					
				}
				return m_LabelBold;
			}
		}

		public static GUIStyle ToggleBold
		{
			get
			{
				if (m_ToggleBold == null)
				{
					m_ToggleBold = new GUIStyle(EditorStyles.toggle );
					m_ToggleBold.fontStyle = FontStyle.Bold;
					
					m_ToggleBold.active.textColor = Color.cyan;
					m_ToggleBold.focused.textColor = Color.cyan;
					m_ToggleBold.fontSize = 12;// .textColor = Color.cyan;
					
				}

				return m_ToggleBold;
			}
		}

		public static void Splitter(Color splitter_rgb, float splitter_thickness = 1) 
		{
			Rect tmp_splitter_position = GUILayoutUtility.GetRect(GUIContent.none, SplitterFull, GUILayout.Height(splitter_thickness));
			
			if (Event.current.type == EventType.Repaint) 
			{
				Color tmp_restore_color = GUI.color;
				GUI.color = splitter_rgb;
				SplitterFull.Draw(tmp_splitter_position, false, false, false, false);
				GUI.color = tmp_restore_color;
			}
		}
		
		public static void Splitter(float splitter_thickness, GUIStyle splitter_style) 
		{
			Rect tmp_splitter_position = GUILayoutUtility.GetRect(GUIContent.none, splitter_style, GUILayout.Height( splitter_thickness ));
			
			if (Event.current.type == EventType.Repaint) 
			{
				Color tmp_restore_color = GUI.color;
				GUI.color = m_DefaultLineColor;
				splitter_style.Draw( tmp_splitter_position, false, false, false, false);
				GUI.color = tmp_restore_color;
			}
		}

		public static void Splitter(float splitter_thickness, int splitter_offset_left, GUIStyle splitter_style) 
		{
			Rect tmp_splitter_position = GUILayoutUtility.GetRect(GUIContent.none, splitter_style, GUILayout.Height( splitter_thickness ));
			
			if (Event.current.type == EventType.Repaint) 
			{
				Color tmp_restore_color = GUI.color;
				GUI.color = m_DefaultLineColor;
				splitter_style.Draw( tmp_splitter_position, false, false, false, false);
				splitter_style.margin = new RectOffset( splitter_offset_left, 0, 7, 7);
				GUI.color = tmp_restore_color;
			}
		}

		public static void Splitter( float splitter_thickness, int splitter_offset_left) 
		{
			Splitter( splitter_thickness, splitter_offset_left, SplitterFull);
		}
		
		public static void Splitter(float splitter_thickness = 1) 
		{
			Splitter( splitter_thickness, SplitterFull);
		}

		public static void SplitterByIndent( int indent_level ) 
		{
			if( indent_level < 0 || indent_level > m_SplitterArray.Length )
				indent_level = 0;

			if( m_SplitterArray[ indent_level ] == null )
			{
				m_SplitterArray[ indent_level ] = new GUIStyle();
				m_SplitterArray[ indent_level ].normal.background = EditorGUIUtility.whiteTexture;
				m_SplitterArray[ indent_level ].normal.background = EditorGUIUtility.whiteTexture;
				m_SplitterArray[ indent_level ].stretchWidth = true;
				m_SplitterArray[ indent_level ].margin = new RectOffset( indent_level * 15 , 0, 7, 7);
			}

			Splitter( 1.0f, m_SplitterArray[ indent_level ] ); 
		}

		public static void Splitter( Rect splitter_position ) 
		{
			if( Event.current.type == EventType.Repaint ) 
			{
				Color tmp_restore_color = GUI.color;
				GUI.color = m_DefaultLineColor;
				SplitterFull.Draw( splitter_position, false, false, false, false );
				GUI.color = tmp_restore_color;
			}
		}
	}
}
