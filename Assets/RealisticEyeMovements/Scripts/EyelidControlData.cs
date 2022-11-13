using UnityEngine;
using System.Collections.Generic;



namespace RealisticEyeMovements {

	[System.Serializable]
	class RotationLimiter
	{
		#region fields

			[SerializeField]
			Vector3 blinkAxis;

			[SerializeField]
			float blinkAngle;

			[SerializeField]
			float minAngle;

			[SerializeField]
			float maxAngle;


		#endregion



		public Quaternion GetRotation( float eyeFollowAngle, float blink01 )
		{
			float angle = Mathf.Clamp(Utils.NormalizedDegAngle(eyeFollowAngle + blink01*blinkAngle), minAngle, maxAngle);
			return Quaternion.AngleAxis(angle, blinkAxis);
		}



		public void SetLimitsFrom( Quaternion openRot, Quaternion closedRot, Quaternion defaultRot, bool isLower)
		{
			float closedAngle;
			Vector3 closedAxis;
			Quaternion closedRotationInDefaultSpace = Quaternion.Inverse( defaultRot ) * closedRot;
			closedRotationInDefaultSpace.ToAngleAxis(out closedAngle, out closedAxis);

			float openAngle;
			Vector3 openAxis;
			Quaternion openRotationInDefaultSpace = Quaternion.Inverse( defaultRot ) * openRot;
			openRotationInDefaultSpace.ToAngleAxis(out openAngle, out openAxis);

			if ( isLower == closedAngle < 0 )
			{
				blinkAngle = -closedAngle;
				blinkAxis = -closedAxis;
			}
			else
			{
				blinkAngle = closedAngle;
				blinkAxis = closedAxis;
			}

			float otherAngle = Vector3.Dot( blinkAxis, openAxis ) > 0 ? openAngle : -openAngle;
			minAngle = Mathf.Min(Utils.NormalizedDegAngle(blinkAngle), Utils.NormalizedDegAngle(otherAngle));
			maxAngle = Mathf.Max(Utils.NormalizedDegAngle(blinkAngle), Utils.NormalizedDegAngle(otherAngle));
		}

	}



	[System.Serializable]
	public class EyelidControlData
	{	
		#region fields

			public enum EyelidControl
			{
				None,
				Bones,
				Blendshapes
			}
			public EyelidControl eyelidControl = EyelidControl.None;

			#region MecanimEyeBones

				public Transform upperEyeLidLeft;
				public Transform upperEyeLidRight;
				public Transform lowerEyeLidLeft;
				public Transform lowerEyeLidRight;

				public bool isBonesDefaultSet;
				public bool isBonesOpenSet;
				public bool isBonesClosedSet;

				[SerializeField]
				RotationLimiter upperLeftLimiter = new RotationLimiter();
				[SerializeField]
				RotationLimiter upperRightLimiter = new RotationLimiter();
				[SerializeField]
				RotationLimiter lowerLeftLimiter = new RotationLimiter();
				[SerializeField]
				RotationLimiter lowerRightLimiter = new RotationLimiter();

				[SerializeField]
				Quaternion upperEyelidLeftDefault;
				[SerializeField]
				Quaternion upperEyelidLeftOpen;
				[SerializeField]
				Quaternion upperEyelidLeftClosed;

				[SerializeField]
				Quaternion lowerEyelidLeftDefault;
				[SerializeField]
				Quaternion lowerEyelidLeftOpen;
				[SerializeField]
				Quaternion lowerEyelidLeftClosed;
	
				[SerializeField]
				Quaternion upperEyelidRightDefault;
				[SerializeField]
				Quaternion upperEyelidRightOpen;
				[SerializeField]
				Quaternion upperEyelidRightClosed;

				[SerializeField]
				Quaternion lowerEyelidRightDefault;
				[SerializeField]
				Quaternion lowerEyelidRightOpen;
				[SerializeField]
				Quaternion lowerEyelidRightClosed;

			#endregion


			#region Blendshapes

				[System.Serializable]
				public class BlendShapeForBlinking
				{
					public SkinnedMeshRenderer skinnedMeshRenderer;
					public float openWeight;
					public float closedWeight;
					public int index;
				}
				[SerializeField]
				BlendShapeForBlinking[] blendShapesForBlinking;

				[System.Serializable]
				public class MeshRendererBlendshapeInfo
				{
					public SkinnedMeshRenderer skinnedMeshRenderer;
					public int blendShapeCount;
					public float[] blendshapeWeights;
				}

				[SerializeField]
				MeshRendererBlendshapeInfo[] meshRendererBlendshapeInfos;

				public bool isBlendshapeDefaultSet;
				public bool isBlendshapeClosedSet;

			#endregion

	
		#endregion



		public void CheckConsistency()
		{
			if ( eyelidControl == EyelidControl.Bones )
			{
				if ( upperEyeLidLeft == null || upperEyeLidRight == null )
					throw new System.Exception("The upper eyelid bones haven't been assigned.");
				if ( false == isBonesDefaultSet )
					throw new System.Exception("The default eyelid position hasn't been saved.");
				if ( false == isBonesOpenSet )
					throw new System.Exception("The eyes maximally open eyelid position hasn't been saved.");
				if ( false == isBonesClosedSet )
					throw new System.Exception("The eyes closed eyelid position hasn't been saved.");
			 }
			 else if ( eyelidControl == EyelidControl.Blendshapes )
			 {
				if ( false == isBlendshapeDefaultSet )
					throw new System.Exception("The default eyelid position hasn't been saved.");
				if ( false == isBlendshapeClosedSet )
					throw new System.Exception("The eyes closed eyelid position hasn't been saved.");
			}

		}


				 
		public void RestoreClosed()
		{
			if ( eyelidControl == EyelidControl.Bones )
			{
				upperEyeLidLeft.localRotation = upperEyelidLeftClosed;
				upperEyeLidRight.localRotation = upperEyelidRightClosed;

				if ( lowerEyeLidLeft != null )
					lowerEyeLidLeft.localRotation = lowerEyelidLeftClosed;
				if ( lowerEyeLidRight != null )
					lowerEyeLidRight.localRotation = lowerEyelidRightClosed;
			}
			else if ( eyelidControl == EyelidControl.Blendshapes )
			{
				foreach ( BlendShapeForBlinking blendShapeForBlinking in blendShapesForBlinking )
					blendShapeForBlinking.skinnedMeshRenderer.
						SetBlendShapeWeight(	blendShapeForBlinking.index,
															blendShapeForBlinking.closedWeight );
			}
		}



		public void RestoreDefault()
		{
			if ( eyelidControl == EyelidControl.Bones )
			{
				upperEyeLidLeft.localRotation = upperEyelidLeftDefault;
				upperEyeLidRight.localRotation = upperEyelidRightDefault;

				if ( lowerEyeLidLeft != null )
					lowerEyeLidLeft.localRotation = lowerEyelidLeftDefault;
				if ( lowerEyeLidRight != null )
					lowerEyeLidRight.localRotation = lowerEyelidRightDefault;
			}
			else if ( eyelidControl == EyelidControl.Blendshapes )
			{
				foreach ( BlendShapeForBlinking blendShapeForBlinking in blendShapesForBlinking )
					blendShapeForBlinking.skinnedMeshRenderer.
						SetBlendShapeWeight(	blendShapeForBlinking.index,
															blendShapeForBlinking.openWeight );
			}
		}



		public void RestoreOpen()
		{
			if ( eyelidControl == EyelidControl.Bones )
			{
				upperEyeLidLeft.localRotation = upperEyelidLeftOpen;
				upperEyeLidRight.localRotation = upperEyelidRightOpen;

				if ( lowerEyeLidLeft != null )
					lowerEyeLidLeft.localRotation = lowerEyelidLeftOpen;
				if ( lowerEyeLidRight != null )
					lowerEyeLidRight.localRotation = lowerEyelidRightOpen;
			}
		}

	

		public void SaveClosed(Object rootObject )
		{
			if ( eyelidControl == EyelidControl.Bones )
			{
				upperEyelidLeftClosed = upperEyeLidLeft.localRotation;
				upperEyelidRightClosed = upperEyeLidRight.localRotation;

				if ( lowerEyeLidLeft != null )
					lowerEyelidLeftClosed = lowerEyeLidLeft.localRotation;
				if ( lowerEyeLidRight != null )
					lowerEyelidRightClosed = lowerEyeLidRight.localRotation;

				UpdateBoneLimits();
				isBonesClosedSet = true;
			}
			else if ( eyelidControl == EyelidControl.Blendshapes )
			{
				SkinnedMeshRenderer[] skinnedMeshRenderers = (rootObject as MonoBehaviour).GetComponentsInChildren<SkinnedMeshRenderer>();
				List<BlendShapeForBlinking> blendShapeForBlinkingList = new List<BlendShapeForBlinking>();
				if ( skinnedMeshRenderers.Length != meshRendererBlendshapeInfos.Length )
				{
					Debug.LogError("The saved data for open eyelids is invalid. Please reset to open eyelids and resave 'Default lid opening'.");
					isBlendshapeDefaultSet = false;
				}
				else
				{
					for ( int i=0;  i<skinnedMeshRenderers.Length;  i++ )
					{
						SkinnedMeshRenderer skinnedMeshRenderer = skinnedMeshRenderers[i];
						MeshRendererBlendshapeInfo meshRendererBlendshapeInfo = meshRendererBlendshapeInfos[i];
						if ( skinnedMeshRenderer != meshRendererBlendshapeInfo.skinnedMeshRenderer || skinnedMeshRenderer.sharedMesh.blendShapeCount != meshRendererBlendshapeInfo.blendShapeCount )
						{
							Debug.LogError("The saved data for open eyelids is invalid. Please reset to open eyelids and resave 'Default lid opening'.");
							isBlendshapeDefaultSet = false;
						}
						else
						{
							for ( int j=0;  j<meshRendererBlendshapeInfo.blendShapeCount;  j++ )
							{
								const float kEpsilon = 0.0001f;
								if ( Mathf.Abs(meshRendererBlendshapeInfo.blendshapeWeights[j] - skinnedMeshRenderer.GetBlendShapeWeight(j)) >= kEpsilon )
								{
									BlendShapeForBlinking blendshapeForBlinking = new BlendShapeForBlinking();
									blendshapeForBlinking.skinnedMeshRenderer = skinnedMeshRenderer;
									blendshapeForBlinking.index = j;
									blendshapeForBlinking.openWeight = meshRendererBlendshapeInfo.blendshapeWeights[j];
									blendshapeForBlinking.closedWeight = skinnedMeshRenderer.GetBlendShapeWeight(j);
									blendShapeForBlinkingList.Add( blendshapeForBlinking );
								}
							}
						}
					}
					blendShapesForBlinking = blendShapeForBlinkingList.ToArray();
					Debug.Log("Found " + blendShapesForBlinking.Length + " blend shapes for blinking:");
					foreach ( BlendShapeForBlinking blendshapeForBlinking in blendShapesForBlinking )
						Debug.Log(blendshapeForBlinking.skinnedMeshRenderer.name + " --> "
								+ blendshapeForBlinking.skinnedMeshRenderer.sharedMesh.GetBlendShapeName( blendshapeForBlinking.index)
								+ " open: " + blendshapeForBlinking.openWeight + " closed: " + blendshapeForBlinking.closedWeight);
					isBlendshapeClosedSet = true;
				}
			}
		}



		public void SaveDefault( Object rootObject )
		{
			if ( eyelidControl == EyelidControl.Bones )
			{
				upperEyelidLeftDefault = upperEyeLidLeft.localRotation;
				upperEyelidRightDefault = upperEyeLidRight.localRotation;

				if ( lowerEyeLidLeft != null )
					lowerEyelidLeftDefault = lowerEyeLidLeft.localRotation;
				if ( lowerEyeLidRight != null )
					lowerEyelidRightDefault = lowerEyeLidRight.localRotation;

				UpdateBoneLimits();
				isBonesDefaultSet = true;
			}
			else if ( eyelidControl == EyelidControl.Blendshapes )
			{
				SkinnedMeshRenderer[] skinnedMeshRenderers = (rootObject as MonoBehaviour).GetComponentsInChildren<SkinnedMeshRenderer>();
				meshRendererBlendshapeInfos = new MeshRendererBlendshapeInfo[ skinnedMeshRenderers.Length ];
				for ( int i=0;  i<skinnedMeshRenderers.Length;  i++ )
				{
					MeshRendererBlendshapeInfo meshRendererBlendshapeInfo = new MeshRendererBlendshapeInfo();
					meshRendererBlendshapeInfo.skinnedMeshRenderer = skinnedMeshRenderers[i];
					meshRendererBlendshapeInfo.blendShapeCount = meshRendererBlendshapeInfo.skinnedMeshRenderer.sharedMesh.blendShapeCount;
					meshRendererBlendshapeInfo.blendshapeWeights = new float[meshRendererBlendshapeInfo.blendShapeCount ];
					for ( int j=0;  j<meshRendererBlendshapeInfo.blendShapeCount;  j++ )
						meshRendererBlendshapeInfo.blendshapeWeights[j] = meshRendererBlendshapeInfo.skinnedMeshRenderer.GetBlendShapeWeight(j);
					meshRendererBlendshapeInfos[i] = meshRendererBlendshapeInfo;
				}

				isBlendshapeDefaultSet = true;
				isBlendshapeClosedSet = false;
			}
		}



		public void SaveOpen( )
		{
			if ( eyelidControl == EyelidControl.Bones )
			{
				upperEyelidLeftOpen = upperEyeLidLeft.localRotation;
				upperEyelidRightOpen = upperEyeLidRight.localRotation;

				if ( lowerEyeLidLeft != null )
					lowerEyelidLeftOpen = lowerEyeLidLeft.localRotation;
				if ( lowerEyeLidRight != null )
					lowerEyelidRightOpen = lowerEyeLidRight.localRotation;

				UpdateBoneLimits();
				isBonesOpenSet = true;
			}
		}



		void UpdateBoneLimits()
		{
			upperLeftLimiter.SetLimitsFrom(upperEyelidLeftOpen, upperEyelidLeftClosed, upperEyelidLeftDefault, isLower:false);
			upperRightLimiter.SetLimitsFrom(upperEyelidRightOpen, upperEyelidRightClosed, upperEyelidRightDefault, isLower: false);

			if ( lowerEyeLidLeft != null )
				lowerLeftLimiter.SetLimitsFrom(lowerEyelidLeftOpen, lowerEyelidLeftClosed, lowerEyelidLeftDefault, isLower: true);

			if ( lowerEyeLidRight != null )
				lowerRightLimiter.SetLimitsFrom(lowerEyelidRightOpen, lowerEyelidRightClosed, lowerEyelidRightDefault, isLower: true);
		}



		public void UpdateEyelids( float upperAngle, float lowerAngle, float blink01 )
		{
			if ( eyelidControl == EyelidControl.Bones )
			{
				upperEyeLidLeft.localRotation = upperEyelidLeftDefault* upperLeftLimiter.GetRotation( upperAngle, blink01 );
				upperEyeLidRight.localRotation = upperEyelidRightDefault * upperRightLimiter.GetRotation( upperAngle, blink01 );

				if ( lowerEyeLidLeft != null )
					lowerEyeLidLeft.localRotation = lowerEyelidLeftDefault * lowerLeftLimiter.GetRotation( lowerAngle, blink01 );
				if ( lowerEyeLidRight != null )
					lowerEyeLidRight.localRotation = lowerEyelidRightDefault * lowerRightLimiter.GetRotation( lowerAngle, blink01 );
			}
			else if ( eyelidControl == EyelidControl.Blendshapes )
			{
				// For blendshapes we only use blink for now; eye follow angles are not supported because most blendshape setups
				// seem to not have separate blendshapes for upper and lower eyelids.

				foreach ( BlendShapeForBlinking blendShapeForBlinking in blendShapesForBlinking )
					blendShapeForBlinking.skinnedMeshRenderer.
						SetBlendShapeWeight(	blendShapeForBlinking.index,
															Mathf.Lerp( blendShapeForBlinking.openWeight, blendShapeForBlinking.closedWeight, blink01 ));
			}
		}


	}

}
