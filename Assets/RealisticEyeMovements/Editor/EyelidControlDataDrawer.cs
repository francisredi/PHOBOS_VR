using UnityEngine;
using UnityEditor;

namespace RealisticEyeMovements {

	[CustomPropertyDrawer (typeof(EyelidControlData))]
	public class EyelidControlDataDrawer : PropertyDrawer
	{

		#region fields

			const float kLineBuffer = 2;
			EyelidControlData.EyelidControl eyelidControl;
			readonly GUIStyle redTextStyle  = new GUIStyle (GUI.skin.label) {normal = {textColor = Color.red}};
			readonly string[] eyelidControlStringList = { "None", "Eyelid bones", "Blendshapes" };

		#endregion


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			int linesNeeded = 1;
			if ( eyelidControl == EyelidControlData.EyelidControl.Bones )
				linesNeeded = 8;
			else if ( eyelidControl == EyelidControlData.EyelidControl.Blendshapes )
				linesNeeded = 3;

			return EditorGUIUtility.singleLineHeight * linesNeeded + kLineBuffer * (linesNeeded-1);
		}



	   public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	   {
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.indentLevel = 0;
			EyelidControlData eyelidControlData = EditorUtils.GetBaseProperty<EyelidControlData>(property);

			eyelidControl = eyelidControlData.eyelidControl;

			EditorGUI.BeginChangeCheck ();
					int selectedIndex = EditorGUI.Popup (new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "Eyelid control", (int) eyelidControl, eyelidControlStringList);
			if (EditorGUI.EndChangeCheck ())
				eyelidControl = eyelidControlData.eyelidControl = (EyelidControlData.EyelidControl) selectedIndex;

			float y = position.y + kLineBuffer + EditorGUIUtility.singleLineHeight;

			if ( eyelidControl == EyelidControlData.EyelidControl.Bones )
			{
				Rect boneRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
				EditorGUI.PropertyField( boneRect, property.FindPropertyRelative("upperEyeLidLeft") );
				y += kLineBuffer + EditorGUIUtility.singleLineHeight;

				boneRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
				EditorGUI.PropertyField( boneRect, property.FindPropertyRelative("upperEyeLidRight") );
				y += kLineBuffer + EditorGUIUtility.singleLineHeight;

				boneRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
				EditorGUI.PropertyField( boneRect, property.FindPropertyRelative("lowerEyeLidLeft") );
				y += kLineBuffer + EditorGUIUtility.singleLineHeight;

				boneRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
				EditorGUI.PropertyField( boneRect, property.FindPropertyRelative("lowerEyeLidRight") );
				y += kLineBuffer + EditorGUIUtility.singleLineHeight;
			}

			bool isBoneControl = eyelidControl == EyelidControlData.EyelidControl.Bones;
			bool isBlendshapeControl = eyelidControl == EyelidControlData.EyelidControl.Blendshapes;
			bool areTransformsMissing = isBoneControl &&
				(	property.FindPropertyRelative("upperEyeLidLeft").objectReferenceValue == null ||
					property.FindPropertyRelative("upperEyeLidRight").objectReferenceValue == null );


			if ( isBoneControl || isBlendshapeControl )
			{
				const float kButtonWidth = 80;
				const float kLabelWidth = 130;
				//if ( redTextStyle == null )
				//	redTextStyle = new GUIStyle (GUI.skin.label) {normal = {textColor = Color.red}};

				if ( areTransformsMissing )
					EditorGUI.LabelField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), "At least the upper eyelid bones need to be assigned", redTextStyle);
				else
				{
					bool isDefaultSet = property.FindPropertyRelative(isBoneControl ? "isBonesDefaultSet" : "isBlendshapeDefaultSet").boolValue;

					//*** Default
					{
						EditorGUI.LabelField(new Rect(position.x, y, kLabelWidth, EditorGUIUtility.singleLineHeight), "Default lid opening");
						if ( GUI.Button( new Rect(position.x + kLabelWidth, y, kButtonWidth, EditorGUIUtility.singleLineHeight), "Save") )
							eyelidControlData.SaveDefault( property.serializedObject.targetObject);
						if ( isDefaultSet )
						{
							if ( GUI.Button( new Rect(position.x + kLabelWidth + kButtonWidth + 20, y, kButtonWidth, EditorGUIUtility.singleLineHeight), "Load") )
								eyelidControlData.RestoreDefault();
						}
						else
							EditorGUI.LabelField( new Rect( position.x + kLabelWidth + kButtonWidth + 20, y, 200, EditorGUIUtility.singleLineHeight), "Not saved yet", redTextStyle);
						y += kLineBuffer + EditorGUIUtility.singleLineHeight;
					}

					if ( isDefaultSet )
					{
						if ( isBoneControl )
						{
							//*** Max open
							{
								EditorGUI.LabelField(new Rect(position.x, y, kLabelWidth, EditorGUIUtility.singleLineHeight), "Max lid opening");
								if ( GUI.Button( new Rect(position.x + kLabelWidth, y, kButtonWidth, EditorGUIUtility.singleLineHeight), "Save") )
									eyelidControlData.SaveOpen( );
								bool isOpenSet = property.FindPropertyRelative("isBonesOpenSet").boolValue;
								if ( isOpenSet )
								{
									if ( GUI.Button( new Rect(position.x + kLabelWidth + kButtonWidth + 20, y, kButtonWidth, EditorGUIUtility.singleLineHeight), "Load") )
										eyelidControlData.RestoreOpen();
								}
								else
									EditorGUI.LabelField( new Rect( position.x + kLabelWidth + kButtonWidth + 20, y, 200, EditorGUIUtility.singleLineHeight), "Not saved yet", redTextStyle);
								y += kLineBuffer + EditorGUIUtility.singleLineHeight;
							}
						}

						//*** Closed
						{
							EditorGUI.LabelField(new Rect(position.x, y, kLabelWidth, EditorGUIUtility.singleLineHeight), "Closed lids");
							if ( GUI.Button( new Rect(position.x + kLabelWidth, y, kButtonWidth, EditorGUIUtility.singleLineHeight), "Save") )
								eyelidControlData.SaveClosed(property.serializedObject.targetObject );
							bool isClosedSet = property.FindPropertyRelative(isBoneControl ? "isBonesClosedSet" : "isBlendshapeClosedSet").boolValue;
							if ( isClosedSet )
							{
								if ( GUI.Button( new Rect(position.x + kLabelWidth + kButtonWidth + 20, y, kButtonWidth, EditorGUIUtility.singleLineHeight), "Load") )
									eyelidControlData.RestoreClosed();
							}
							else
								EditorGUI.LabelField( new Rect( position.x + kLabelWidth + kButtonWidth + 20, y, 200, EditorGUIUtility.singleLineHeight), "Not saved yet", redTextStyle);
							y += kLineBuffer + EditorGUIUtility.singleLineHeight;
						}
					}
				}
			}

			EditorGUI.EndProperty();
		}

	}
}
