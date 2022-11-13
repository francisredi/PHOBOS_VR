using UnityEngine;
using UnityEditor;

namespace RealisticEyeMovements {

	[CustomPropertyDrawer (typeof(EyeControlData))]
	public class EyeControlDataDrawer : PropertyDrawer
	{

		#region fields

			const float kLineBuffer = 2;
			EyeControlData.EyeControl eyeControl;
			readonly GUIStyle redTextStyle  = new GUIStyle (GUI.skin.label) {normal = {textColor = Color.red}};
			readonly string[] eyeControlStringList = { "None", "Mecanim eye bones", "Eye gameobjects" };
			Animator animator;

		#endregion



		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			int linesNeeded = 1;
			if ( eyeControl == EyeControlData.EyeControl.MecanimEyeBones )
			{
				if ( animator == null )
					animator = (property.serializedObject.targetObject as MonoBehaviour).GetComponentInChildren<Animator>();
				bool isAnimatorMissing = animator == null;
				bool areBonesMissing = animator != null &&
					(null == animator.GetBoneTransform(HumanBodyBones.LeftEye) || null == animator.GetBoneTransform(HumanBodyBones.LeftEye) );
				linesNeeded = (isAnimatorMissing || areBonesMissing) ? 2 : 1;
			}
			else if ( eyeControl == EyeControlData.EyeControl.SelectedObjects )
			{
				bool areTransformsMissing =	property.FindPropertyRelative("leftEye").objectReferenceValue == null ||
															property.FindPropertyRelative("rightEye").objectReferenceValue == null;
				linesNeeded = areTransformsMissing ? 4 : 3;
			}

			return EditorGUIUtility.singleLineHeight * linesNeeded + kLineBuffer * (linesNeeded-1);
		}



	   public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	   {
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.indentLevel = 0;
			EyeControlData eyeControlData = EditorUtils.GetBaseProperty<EyeControlData>(property);

			eyeControl = eyeControlData.eyeControl;

			EditorGUI.BeginChangeCheck ();
					int selectedIndex = EditorGUI.Popup (new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "Eye control", (int) eyeControl, eyeControlStringList);
			if (EditorGUI.EndChangeCheck ())
				eyeControl = eyeControlData.eyeControl = (EyeControlData.EyeControl) selectedIndex;

			float y = position.y + kLineBuffer + EditorGUIUtility.singleLineHeight;

			bool isBoneControl = eyeControl == EyeControlData.EyeControl.MecanimEyeBones;
			bool isEyeballControl = eyeControl == EyeControlData.EyeControl.SelectedObjects;

			if ( isEyeballControl )
			{
				Rect boneRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
				EditorGUI.PropertyField( boneRect, property.FindPropertyRelative("leftEye") );
				y += kLineBuffer + EditorGUIUtility.singleLineHeight;

				boneRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
				EditorGUI.PropertyField( boneRect, property.FindPropertyRelative("rightEye") );
				y += kLineBuffer + EditorGUIUtility.singleLineHeight;
			}

			if ( isBoneControl && animator == null )
				animator = (property.serializedObject.targetObject as MonoBehaviour).GetComponent<Animator>();

			bool isAnimatorMissing = isBoneControl && animator == null;
			bool areTransformsMissing = isEyeballControl &&
				(	property.FindPropertyRelative("leftEye").objectReferenceValue == null ||
					property.FindPropertyRelative("rightEye").objectReferenceValue == null );

			bool areBonesMissing = isBoneControl && animator != null &&
				(null == animator.GetBoneTransform(HumanBodyBones.LeftEye) || null == animator.GetBoneTransform(HumanBodyBones.LeftEye) );


			if ( isBoneControl || isEyeballControl )
			{

				if ( areTransformsMissing )
					EditorGUI.LabelField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), "The eyeballs need to be assigned.", redTextStyle);
				else if ( isAnimatorMissing )
					EditorGUI.LabelField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), "No Animator found.", redTextStyle);
				else if ( areBonesMissing )
					EditorGUI.LabelField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), "Eye bones not found; is the Mecanim rig set up correctly?", redTextStyle);
			}

			EditorGUI.EndProperty();
		}

	}
}
