// EyeAndHeadAnimator.cs
// Tore Knabe
// ioccam@ioccam.com


using UnityEngine;
using Random = UnityEngine.Random;

namespace RealisticEyeMovements {

	public class EyeAndHeadAnimator : MonoBehaviour
	{
		#region fields


			[Tooltip("How much the looking controls head direction.")]
			[Range(0,1)]
			public float headWeight = 1;


			public bool useMicroSaccades = true;
			public bool useMacroSaccades = true;
			public bool kDrawSightlinesInEditor = false;
			public EyeControlData eyeControlData;

			#region eye lids

				public EyelidControlData eyelidControlData;

				[Tooltip("Minimum seconds until next blink")]
				public float kMinNextBlinkTime = 3.0f;
				[Tooltip("Maximum seconds until next blink")]
				public float kMaxNextBlinkTime = 15.0f;

				public float blink01 { get; private set; }		
	
				bool useUpperEyelids;
				bool useLowerEyelids;

				float upperEyeLidBaseAngle;
				float lowerEyeLidBaseAngle;

				float timeOfNextBlink;
				bool isBlinking;
			
				enum BlinkState {
					Idle,
					Closing,
					KeepingClosed,
					Opening
				}
				BlinkState blinkState = BlinkState.Idle;
				float blinkStateTime;
				float blinkDuration;
				bool isShortBlink;
			
				const float kUpperEyelidFollowEyeFactor = 0.65f;
				const float kLowerEyelidFollowEyeFactor = 0.5f;

				const float kBlinkCloseTimeShort = 0.035f;
				const float kBlinkOpenTimeShort = 0.055f;
				const float kBlinkCloseTimeLong = 0.07f;
				const float kBlinkOpenTimeLong = 0.1f;
				const float kBlinkKeepingClosedTime = 0.008f;
	
			#endregion

			[Tooltip("Maximum angle for eye when looking up")]
			public float maxEyeUpAngle = 20;
			[Tooltip("Maximum angle for eye when looking down")]
			public float maxEyeDownAngle = 20;


			public float eyeDistance { get; private set; }


			Transform leftEyeAnchor;
			Transform rightEyeAnchor;

			float leftMaxSpeedHoriz;
			float leftHorizDuration;
			float leftMaxSpeedVert;
			float leftVertDuration;
			float leftCurrentSpeedX;
			float leftCurrentSpeedY;

			float rightMaxSpeedHoriz;
			float rightHorizDuration;
			float rightMaxSpeedVert;
			float rightVertDuration;
			float rightCurrentSpeedX;
			float rightCurrentSpeedY;

			float startLeftEyeHorizDuration;
			float startLeftEyeVertDuration;
			float startLeftEyeMaxSpeedHoriz;
			float startLeftEyeMaxSpeedVert;

			float startRightEyeHorizDuration;
			float startRightEyeVertDuration;
			float startRightEyeMaxSpeedHoriz;
			float startRightEyeMaxSpeedVert;

			float timeOfEyeMovementStart;

			float headMaxSpeedHoriz;
			float headMaxSpeedVert;
			float headHorizDuration;
			float headVertDuration;

			float startHeadHorizDuration;
			float startHeadVertDuration;
			float startHeadMaxSpeedHoriz;
			float startHeadMaxSpeedVert;

			float timeOfHeadMovementStart;
			float currentHeadHorizSpeed;
			float currentHeadVertSpeed;
			float currentHeadZSpeed;


			float headLatency;
			float eyeLatency;

			float ikWeight = 1;

			Animator animator;

			Vector2 eyeMaxSpeed;
			Vector2 headMaxSpeed;

			Vector3 currentHeadLocalAngles;


			#region Transforms for target
				Transform currentHeadTargetPOI;
				Transform currentEyeTargetPOI;
				Transform nextHeadTargetPOI;
				Transform nextEyeTargetPOI;
				Transform currentTargetLeftEyeXform;
				Transform currentTargetRightEyeXform;
				Transform nextTargetLeftEyeXform;
				Transform nextTargetRightEyeXform;
				readonly Transform[] createdTargetXforms = new Transform[2];
				int createdTargetXformIndex;
			#endregion


			Transform leftEyeParentXform;
			Transform rightEyeParentXform;
			Transform leftEyeSocketXform;
			Transform rightEyeSocketXform;
			Transform eyesRootXform;
			Transform headParentXform;
			Transform headTargetPivotXform;

			Quaternion leftAnchorToEyeQ;
			Quaternion rightAnchorToEyeQ;
			Quaternion leftEyeToAnchorQ;
			Quaternion rightEyeToAnchorQ;
			Vector3 currentLeftEyeLocalEuler;
			Vector3 currentRightEyeLocalEuler;
			Quaternion originalLeftEyeLocalQ;
			Quaternion originalRightEyeLocalQ;
			Quaternion lastLeftEyeLocalRotation;
			Quaternion lastRightEyeLocalQ;

			Vector3 macroSaccadeTargetLocal;
			Vector3 microSaccadeTargetLocal;

			float timeOfEnteringClearingPhase;
			float timeOfLastMacroSaccade;
			float timeToMicroSaccade;
			float timeToMacroSaccade;
		
			public enum HeadSpeed
			{
				Slow,
				Fast
			}
			HeadSpeed headSpeed = HeadSpeed.Slow;

			public enum EyeDelay
			{
				Simultaneous,
				EyesFirst,
				HeadFirst
			}

			enum LookTarget
			{
				StraightAhead,
				ClearingTargetPhase1,
				ClearingTargetPhase2,
				GeneralDirection,
				SpecificThing,
				Face
			}
			LookTarget lookTarget = LookTarget.StraightAhead;

			enum FaceLookTarget
			{
				EyesCenter,
				LeftEye,
				RightEye,
				Mouth
			}
			FaceLookTarget faceLookTarget = FaceLookTarget.EyesCenter;

		#endregion



		void Awake()
		{
			eyeDistance = 0.064f;

			eyeControlData = new EyeControlData (); // new
			eyelidControlData = new EyelidControlData (); // new

			animator = GetComponentInChildren<Animator>();

			eyeControlData.CheckConsistency( animator );
			eyelidControlData.CheckConsistency();



			createdTargetXforms[0] = new GameObject(name + "_createdEyeTarget_1").transform;
			createdTargetXforms[0].gameObject.hideFlags = HideFlags.HideInHierarchy;
			createdTargetXforms[1] = new GameObject(name + "_createdEyeTarget_2").transform;
			createdTargetXforms[1].gameObject.hideFlags = HideFlags.HideInHierarchy;

			Transform headXform =  (animator != null )	? (animator.GetBoneTransform(HumanBodyBones.Head) ?? transform)
																			: transform;
			Transform spineXform = (animator != null )	? (animator.GetBoneTransform(HumanBodyBones.Chest) ?? (animator.GetBoneTransform(HumanBodyBones.Spine) ?? transform) )
																			: transform;
			headParentXform = new GameObject(name + " head parent").transform;
			headParentXform.gameObject.hideFlags = HideFlags.HideInHierarchy;
			headParentXform.parent = spineXform;
			headParentXform.position = headXform.position;
			headParentXform.rotation = transform.rotation;

			headTargetPivotXform = new GameObject(name + " head target").transform;
			headTargetPivotXform.gameObject.hideFlags = HideFlags.HideInHierarchy;
			headTargetPivotXform.parent = headParentXform;
			headTargetPivotXform.localPosition = Vector3.zero;
			headTargetPivotXform.localRotation = Quaternion.identity;


			//*** Eyes
			{
				if ( eyeControlData.eyeControl == EyeControlData.EyeControl.MecanimEyeBones || eyeControlData.eyeControl == EyeControlData.EyeControl.SelectedObjects )
				{
					if ( eyeControlData.eyeControl == EyeControlData.EyeControl.MecanimEyeBones )
					{
						Transform leftEyeBoneXform = animator.GetBoneTransform(HumanBodyBones.LeftEye);
						Transform rightEyeBoneXform = animator.GetBoneTransform(HumanBodyBones.RightEye);
						leftEyeAnchor = leftEyeBoneXform;
						rightEyeAnchor = rightEyeBoneXform;
					}
					else if ( eyeControlData.eyeControl == EyeControlData.EyeControl.SelectedObjects )
					{
						leftEyeAnchor = eyeControlData.leftEye;
						rightEyeAnchor = eyeControlData.rightEye;
					}
				}


				eyesRootXform = new GameObject(name + "_eyesRoot").transform;
				eyesRootXform.gameObject.hideFlags = HideFlags.HideInHierarchy;
				eyesRootXform.rotation = transform.rotation;

				if ( leftEyeAnchor != null && rightEyeAnchor != null )
				{
					eyeDistance = Vector3.Distance( leftEyeAnchor.position, rightEyeAnchor.position );

					Quaternion inverse = Quaternion.Inverse(transform.rotation);
					leftAnchorToEyeQ = inverse * leftEyeAnchor.rotation;
					rightAnchorToEyeQ = inverse * rightEyeAnchor.rotation;
					leftEyeToAnchorQ = Quaternion.Inverse(leftAnchorToEyeQ);
					rightEyeToAnchorQ = Quaternion.Inverse(rightAnchorToEyeQ);

					originalLeftEyeLocalQ = leftEyeAnchor.localRotation;
					originalRightEyeLocalQ = rightEyeAnchor.localRotation;

					eyesRootXform.position = 0.5f * (leftEyeAnchor.position + rightEyeAnchor.position);
					eyesRootXform.parent = Utils.GetCommonAncestor( leftEyeAnchor, rightEyeAnchor ) ?? leftEyeAnchor.parent;
				}
				else if ( animator != null )
				{
					if ( headXform != null )
					{
						eyesRootXform.position = headXform.position;
						eyesRootXform.parent = headXform;
					}
					else
					{
						eyesRootXform.position = transform.position;
						eyesRootXform.parent = transform;
					}
				}
				else
				{
					eyesRootXform.position = transform.position;
					eyesRootXform.parent = transform;
				}
			}


			//*** Eye lids
			{
				if ( eyelidControlData.eyelidControl == EyelidControlData.EyelidControl.Bones )
				{
					if ( eyelidControlData.upperEyeLidLeft != null && eyelidControlData.upperEyeLidRight != null )
						useUpperEyelids = true;

					if ( eyelidControlData.lowerEyeLidLeft != null && eyelidControlData.lowerEyeLidRight != null )
						useLowerEyelids = true;
				}

				blink01 = 0;
				timeOfNextBlink = Time.time + Random.Range(3f, 6f);
				ikWeight = headWeight;
			}

		}



		public void Blink( bool isShortBlink =true)
		{
			if ( blinkState != BlinkState.Idle )
				return;

			this.isShortBlink = isShortBlink;
			blinkState = BlinkState.Closing;
			blinkStateTime = 0;
			blinkDuration = isShortBlink ? kBlinkCloseTimeShort : kBlinkCloseTimeLong;
		}



		public bool CanChangePointOfAttention()
		{
			return Time.time-timeOfLastMacroSaccade >= 2f;
		}



		// If eye latency is greater than zero, the head starts turning towards new target and the eyes keep looking at the old target for a while.
		// If head latency is greater than zero, the eyes look at the new target first and the head turns later.
		void CheckLatencies()
		{
			if ( eyeLatency > 0 )
			{
				eyeLatency -= Time.deltaTime;
				if ( eyeLatency <= 0 )
				{
					currentEyeTargetPOI = nextEyeTargetPOI;
					currentTargetLeftEyeXform = nextTargetLeftEyeXform;
					currentTargetRightEyeXform = nextTargetRightEyeXform;
					StartEyeMovement(currentEyeTargetPOI);
				}
			}
			else if ( headLatency > 0 )
			{
				headLatency -= Time.deltaTime;
				if ( headLatency <= 0 )
					StartHeadMovement( nextHeadTargetPOI );
			}
		}



		void CheckMacroSaccades()
		{
			if ( lookTarget == LookTarget.SpecificThing )
				return;

			if ( eyeControlData.eyeControl == EyeControlData.EyeControl.None )
				return;

			if ( eyeLatency > 0 )
				return;

			timeToMacroSaccade -= Time.deltaTime;
			if ( timeToMacroSaccade <= 0 )
			{
				if ( lookTarget == LookTarget.GeneralDirection && useMacroSaccades)
				{
							const float kMacroSaccadeAngle = 10;
							bool hasBoneEyelidControl = eyelidControlData.eyelidControl == EyelidControlData.EyelidControl.Bones;
							float angleVert = Random.Range(-kMacroSaccadeAngle * (hasBoneEyelidControl ? 0.65f : 0.3f), kMacroSaccadeAngle * (hasBoneEyelidControl ? 0.65f : 0.4f));
							float angleHoriz = Random.Range(-kMacroSaccadeAngle,kMacroSaccadeAngle);
					SetMacroSaccadeTarget( eyesRootXform.TransformPoint(	Quaternion.Euler( angleVert, angleHoriz, 0)
																												* eyesRootXform.InverseTransformPoint( GetCurrentEyeTargetPos() )));

					timeToMacroSaccade = Random.Range(5.0f, 8.0f);
				}
				else if ( lookTarget == LookTarget.Face )
				{
					if ( currentEyeTargetPOI == null )
					{
						//*** Social triangle: saccade between eyes and mouth (or chest, if actor isn't looking back)
						{
							switch( faceLookTarget )
							{
								case FaceLookTarget.LeftEye:
									faceLookTarget = Random.value < 0.75f ? FaceLookTarget.RightEye : FaceLookTarget.Mouth;
									break;
								case FaceLookTarget.RightEye:
									faceLookTarget = Random.value < 0.75f ? FaceLookTarget.LeftEye : FaceLookTarget.Mouth;
									break;
								case FaceLookTarget.Mouth:
								case FaceLookTarget.EyesCenter:
									faceLookTarget = Random.value < 0.5f ? FaceLookTarget.LeftEye : FaceLookTarget.RightEye;
									break;
							}
							SetMacroSaccadeTarget( GetLookTargetPosForSocialTriangle( faceLookTarget ) );
							timeToMacroSaccade = (faceLookTarget == FaceLookTarget.Mouth)	? Random.Range(0.4f, 0.9f)
																																: Random.Range(1.0f, 3.0f);
						}
					}
				}																																				
			}
		}



		void CheckMicroSaccades()
		{
			if ( false == useMicroSaccades )
				return;

			if ( eyeControlData.eyeControl == EyeControlData.EyeControl.None )
				return;

			if ( eyeLatency > 0 )
				return;

			if ( lookTarget == LookTarget.GeneralDirection || lookTarget == LookTarget.SpecificThing || (lookTarget == LookTarget.Face && currentEyeTargetPOI != null) )
			{
				timeToMicroSaccade -= Time.deltaTime;
				if ( timeToMicroSaccade <= 0 )
				{
					const float kMicroSaccadeAngle = 3;
					bool hasBoneEyelidControl = eyelidControlData.eyelidControl == EyelidControlData.EyelidControl.Bones;
					float angleVert = Random.Range(-kMicroSaccadeAngle * (hasBoneEyelidControl ? 0.8f : 0.5f), kMicroSaccadeAngle * (hasBoneEyelidControl ? 0.85f : 0.6f));
					float angleHoriz = Random.Range(-kMicroSaccadeAngle,kMicroSaccadeAngle);
					if ( lookTarget == LookTarget.Face )
					{
						angleVert *= 0.5f;
						angleHoriz *= 0.5f;
					}

					SetMicroSaccadeTarget ( eyesRootXform.TransformPoint(	Quaternion.Euler(angleVert, angleHoriz, 0)
																												* eyesRootXform.InverseTransformPoint( currentEyeTargetPOI.TransformPoint(macroSaccadeTargetLocal) )));
				}
			}
		}


		
		float ClampHorizEyeAngle( float angle )
		{
			const float kHorizLimit = 40;
			return Mathf.Clamp(Utils.NormalizedDegAngle(angle), -kHorizLimit, kHorizLimit);
		}



		float ClampVertEyeAngle( float angle )
		{

			return Mathf.Clamp(Utils.NormalizedDegAngle(angle), -maxEyeUpAngle, maxEyeDownAngle);
		}



		public void ClearLookTarget()
		{
			LookAtAreaAround( GetOwnEyeCenter() + transform.forward * 1000 * eyeDistance );
			lookTarget = LookTarget.ClearingTargetPhase1;
			timeOfEnteringClearingPhase = Time.time;
		}



		void DrawSightlinesInEditor()
		{
			if ( eyeControlData.eyeControl != EyeControlData.EyeControl.None )
			{
				Vector3 leftDirection = (leftEyeAnchor.parent.rotation * leftEyeAnchor.localRotation * leftEyeToAnchorQ) * Vector3.forward;
				Vector3 rightDirection = (rightEyeAnchor.parent.rotation * rightEyeAnchor.localRotation * rightEyeToAnchorQ) * Vector3.forward;
				Debug.DrawLine(leftEyeAnchor.position, leftEyeAnchor.position + leftDirection * 10);
				Debug.DrawLine(rightEyeAnchor.position, rightEyeAnchor.position + rightDirection * 10);
			}

			// Debug.DrawLine(eyesRootXform.position, eyesRootXform.position + GetOwnLookDirection() * 10  );
		}



		Vector3 GetCurrentEyeTargetPos()
		{
			return ( currentEyeTargetPOI != null )	?	currentEyeTargetPOI.position
																	:	0.5f * ( currentTargetLeftEyeXform.position + currentTargetRightEyeXform.position );
		}



		Vector3 GetCurrentHeadTargetPos()
		{
			return ( currentHeadTargetPOI != null )	?	currentHeadTargetPOI.position
																		:	0.5f * ( currentTargetLeftEyeXform.position + currentTargetRightEyeXform.position );
		}



		Vector3 GetLookTargetPosForSocialTriangle( FaceLookTarget playerFaceLookTarget )
		{
			if ( currentTargetLeftEyeXform == null || currentTargetRightEyeXform == null )
				return currentEyeTargetPOI.position;

			Vector3 faceTargetPos = Vector3.zero;

			Vector3 eyeCenter = 0.5f * (currentTargetLeftEyeXform.position + currentTargetRightEyeXform.position);

			switch( playerFaceLookTarget )
			{
				case FaceLookTarget.EyesCenter:
					faceTargetPos = GetCurrentEyeTargetPos();
					break;
				case FaceLookTarget.LeftEye:
					faceTargetPos = Vector3.Lerp(eyeCenter, currentTargetLeftEyeXform.position, 0.75f);
					break;
				case FaceLookTarget.RightEye:
					faceTargetPos = Vector3.Lerp(eyeCenter, currentTargetRightEyeXform.position, 0.75f);
					break;
				case FaceLookTarget.Mouth:
					Vector3 eyeUp = 0.5f * (currentTargetLeftEyeXform.up + currentTargetRightEyeXform.up);
					faceTargetPos = eyeCenter - eyeUp * 0.4f * Vector3.Distance( currentTargetLeftEyeXform.position, currentTargetRightEyeXform.position );
					break;
			}

			return faceTargetPos;
		}



		public Vector3 GetOwnEyeCenter()
		 {
			return eyesRootXform.position;
		 }



		Vector3 GetOwnLookDirection()
		{
			return ( leftEyeAnchor != null && rightEyeAnchor != null )	?  (Quaternion.Slerp(	leftEyeAnchor.rotation * leftEyeToAnchorQ,
																																rightEyeAnchor.rotation * rightEyeToAnchorQ, 0.5f)) * Vector3.forward
																								:	eyesRootXform.forward;
		}



		public float GetStareAngleMeAtTarget( Vector3 target )
		{
			return Vector3.Angle(GetOwnLookDirection(), target - eyesRootXform.position);
		}



		public float GetStareAngleTargetAtMe( Transform targetXform )
		{
			return Vector3.Angle(targetXform.forward, GetOwnEyeCenter() - targetXform.position);
		}



		public bool IsInView( Vector3 target )
		{
			const float kMaxHorizAngle = 80f;
			const float kMaxVertAngle = 40;

			if ( leftEyeAnchor == null || rightEyeAnchor == null )
			{
							Vector3 localAngles = Quaternion.LookRotation(eyesRootXform.InverseTransformDirection(target - GetOwnEyeCenter()), eyesRootXform.up).eulerAngles;
							float vertAngle = Utils.NormalizedDegAngle(localAngles.x);
							float horizAngle = Utils.NormalizedDegAngle(localAngles.y);
				bool seesTarget = Mathf.Abs(vertAngle) <= kMaxVertAngle && Mathf.Abs(horizAngle) <= kMaxHorizAngle;

				return seesTarget;
			}
			else
			{
							Vector3 localAnglesLeft = (leftAnchorToEyeQ * Quaternion.Inverse(leftEyeAnchor.rotation) * Quaternion.LookRotation(target - leftEyeAnchor.position, leftEyeAnchor.up)).eulerAngles;
							float vertAngleLeft = Utils.NormalizedDegAngle(localAnglesLeft.x);
							float horizAngleLeft = Utils.NormalizedDegAngle(localAnglesLeft.y);
				bool leftEyeSeesTarget = Mathf.Abs(vertAngleLeft) <= kMaxVertAngle && Mathf.Abs(horizAngleLeft) <= kMaxHorizAngle;

							Vector3 localAnglesRight = (rightAnchorToEyeQ * Quaternion.Inverse(rightEyeAnchor.rotation) * Quaternion.LookRotation(target - rightEyeAnchor.position, rightEyeAnchor.up)).eulerAngles;
							float vertAngleRight = Utils.NormalizedDegAngle(localAnglesRight.x);
							float horizAngleRight = Utils.NormalizedDegAngle(localAnglesRight.y);
				bool rightEyeSeesTarget = Mathf.Abs(vertAngleRight) <= kMaxVertAngle && Mathf.Abs(horizAngleRight) <= kMaxHorizAngle;

				return leftEyeSeesTarget || rightEyeSeesTarget;
			}
		}



		public bool IsLookingAtFace()
		{
			return lookTarget == LookTarget.Face;
		}
	
	
	
		void LateUpdate()
		{
			if ( lookTarget == LookTarget.StraightAhead )
				return;

			UpdateHeadMovement();
			if ( eyeControlData.eyeControl != EyeControlData.EyeControl.None )
				UpdateEyeMovement();
			UpdateBlinking();
			UpdateEyelids();
		}



		float LimitHeadAngle( float headAngle )
		{
			const float kMaxUnlimitedHeadAngle = 90;
			const float kMaxLimitedHeadAngle = 55;
			const float kExponent = 1.5f;

			headAngle = Utils.NormalizedDegAngle(headAngle);
			float absAngle = Mathf.Abs(headAngle);
			float limitedAngle = Mathf.Sign(headAngle) *
										(absAngle - (kMaxUnlimitedHeadAngle-kMaxLimitedHeadAngle)/Mathf.Pow(kMaxUnlimitedHeadAngle, kExponent) * Mathf.Pow(absAngle, kExponent));

			return limitedAngle;
		}



		public void LookAtFace( Transform eyeCenterXform )
		{
			lookTarget = LookTarget.Face;
			headSpeed = HeadSpeed.Fast;
			faceLookTarget = FaceLookTarget.EyesCenter;
			nextHeadTargetPOI = eyeCenterXform;
			headLatency = Random.Range(0.05f, 0.1f);
			currentTargetLeftEyeXform = currentTargetRightEyeXform = null;
			nextTargetLeftEyeXform = nextTargetRightEyeXform = null;

			StartEyeMovement( eyeCenterXform );
		}



		public void LookAtFace(	Transform leftEyeXform,
											Transform rightEyeXform )
		{
			lookTarget = LookTarget.Face;
			headSpeed = HeadSpeed.Fast;
			faceLookTarget = FaceLookTarget.EyesCenter;
			headLatency = Random.Range(0.05f, 0.1f);
			currentTargetLeftEyeXform = leftEyeXform;
			currentTargetRightEyeXform = rightEyeXform;
			nextTargetLeftEyeXform = nextTargetRightEyeXform = null;
			nextHeadTargetPOI = null;

			StartEyeMovement( );
		}



		public void LookAtSpecificThing( Transform poi )
		{
			lookTarget = LookTarget.SpecificThing;
			headSpeed = HeadSpeed.Fast;
			headLatency = Random.Range(0.05f, 0.1f);
			nextHeadTargetPOI = poi;
			currentTargetLeftEyeXform = currentTargetRightEyeXform = null;
			nextTargetLeftEyeXform = nextTargetRightEyeXform = null;

			StartEyeMovement( poi );
		}



		public void LookAtSpecificThing( Vector3 point )
		{
			createdTargetXformIndex = (createdTargetXformIndex+1) % createdTargetXforms.Length;
			createdTargetXforms[createdTargetXformIndex].position = point;
			LookAtSpecificThing( createdTargetXforms[createdTargetXformIndex] );
		}



		public void LookAtAreaAround( Transform poi )
		{
			lookTarget = LookTarget.GeneralDirection;
			headSpeed = HeadSpeed.Slow;
			eyeLatency = Random.Range(0.05f, 0.1f);

			nextEyeTargetPOI = poi;
			currentTargetLeftEyeXform = currentTargetRightEyeXform = null;
			nextTargetLeftEyeXform = nextTargetRightEyeXform = null;

			StartHeadMovement( poi );
		}



		public void LookAtAreaAround( Vector3 point )
		{
			createdTargetXformIndex = (createdTargetXformIndex+1) % createdTargetXforms.Length;
			createdTargetXforms[createdTargetXformIndex].position = point;
			LookAtAreaAround( createdTargetXforms[createdTargetXformIndex] );
		}



		void OnAnimatorIK()
		{
			if ( headWeight <= 0 )
				return;

					float targetIKWeight = (lookTarget == LookTarget.StraightAhead || lookTarget == LookTarget.ClearingTargetPhase2 || lookTarget == LookTarget.ClearingTargetPhase1 ) ? 0 : headWeight;
					ikWeight = Mathf.Lerp( ikWeight, targetIKWeight, Time.deltaTime);		
			animator.SetLookAtWeight(1, 0.01f, ikWeight);
		
			animator.SetLookAtPosition(headTargetPivotXform.TransformPoint( Vector3.forward ));
		}



		void SetMacroSaccadeTarget( Vector3 targetGlobal )
		{	
			macroSaccadeTargetLocal = (currentEyeTargetPOI ?? currentTargetLeftEyeXform).InverseTransformPoint( targetGlobal );

			timeOfLastMacroSaccade = Time.time;

			SetMicroSaccadeTarget( targetGlobal );
			timeToMicroSaccade += 0.75f;
		}



		void SetMicroSaccadeTarget( Vector3 targetGlobal )
		{
			microSaccadeTargetLocal = (currentEyeTargetPOI ?? currentTargetLeftEyeXform).InverseTransformPoint( targetGlobal );

			Vector3 targetLeftEyeLocalAngles = Quaternion.LookRotation(eyesRootXform.InverseTransformDirection( targetGlobal - leftEyeAnchor.position), eyesRootXform.up).eulerAngles;

					targetLeftEyeLocalAngles = new Vector3(	ClampVertEyeAngle(targetLeftEyeLocalAngles.x),
																				ClampHorizEyeAngle(targetLeftEyeLocalAngles.y),
																				targetLeftEyeLocalAngles.z);

			float leftHorizDistance = Mathf.Abs(Mathf.DeltaAngle(currentLeftEyeLocalEuler.y, targetLeftEyeLocalAngles.y));

					// From "Realistic Avatar and Head Animation Using a Neurobiological Model of Visual Attention", Itti, Dhavale, Pighin
			leftMaxSpeedHoriz = 473 * (1 - Mathf.Exp(-leftHorizDistance/7.8f));

					// From "Eyes Alive", Lee, Badler
					const float D0 = 0.025f;
					const float d = 0.00235f;
			leftHorizDuration = D0 + d * leftHorizDistance;

			float leftVertDistance = Mathf.Abs(Mathf.DeltaAngle(currentLeftEyeLocalEuler.x, targetLeftEyeLocalAngles.x));
			leftMaxSpeedVert = 473 * (1 - Mathf.Exp(-leftVertDistance/7.8f));
			leftVertDuration = D0 + d * leftVertDistance;


			Vector3 targetRightEyeLocalAngles = Quaternion.LookRotation(eyesRootXform.InverseTransformDirection( targetGlobal - rightEyeAnchor.position), eyesRootXform.up).eulerAngles;

						targetRightEyeLocalAngles = new Vector3(	ClampVertEyeAngle(targetRightEyeLocalAngles.x),
																						ClampHorizEyeAngle(targetRightEyeLocalAngles.y),
																						targetRightEyeLocalAngles.z);

			float rightHorizDistance = Mathf.Abs(Mathf.DeltaAngle(currentRightEyeLocalEuler.y, targetRightEyeLocalAngles.y));
			rightMaxSpeedHoriz = 473 * (1 - Mathf.Exp(-rightHorizDistance/7.8f));
			rightHorizDuration = D0 + d * rightHorizDistance;

			float rightVertDistance = Mathf.Abs(Mathf.DeltaAngle(currentRightEyeLocalEuler.x, targetRightEyeLocalAngles.x));
			rightMaxSpeedVert = 473 * (1 - Mathf.Exp(-rightVertDistance/7.8f));
			rightVertDuration = D0 + d * rightVertDistance;

			leftMaxSpeedHoriz = rightMaxSpeedHoriz = Mathf.Max( leftMaxSpeedHoriz, rightMaxSpeedHoriz );
			leftMaxSpeedVert = rightMaxSpeedVert = Mathf.Max( leftMaxSpeedVert, rightMaxSpeedVert );
			leftHorizDuration = rightHorizDuration = Mathf.Max( leftHorizDuration, rightHorizDuration );
			leftVertDuration = rightVertDuration = Mathf.Max( leftVertDuration, rightVertDuration );

			timeToMicroSaccade = (lookTarget == LookTarget.Face)	? Random.Range(0.8f, 1.75f)
																								: Random.Range(0.8f, 1.75f);
			//*** Blink if eyes move enough
			{
				if ( useUpperEyelids || useLowerEyelids )
				{
							float distance = Mathf.Max(leftHorizDistance, Mathf.Max(rightHorizDistance, Mathf.Max(leftVertDistance, rightVertDistance)));
							const float kMinBlinkDistance = 25.0f;
					if ( distance >= kMinBlinkDistance )
						Blink( isShortBlink: false );
				}
			}

			//*** For letting the eyes keep tracking the target after they saccaded to it
			{
				startLeftEyeHorizDuration = leftHorizDuration;
				startLeftEyeVertDuration = leftVertDuration;
				startLeftEyeMaxSpeedHoriz = leftMaxSpeedHoriz;
				startLeftEyeMaxSpeedVert = leftMaxSpeedVert;

				startRightEyeHorizDuration = rightHorizDuration;
				startRightEyeVertDuration = rightVertDuration;
				startRightEyeMaxSpeedHoriz = rightMaxSpeedHoriz;
				startRightEyeMaxSpeedVert = rightMaxSpeedVert;

				timeOfEyeMovementStart = Time.time;
			}

		}



		void StartEyeMovement( Transform targetXform=null )
		{
			eyeLatency = 0;
			currentEyeTargetPOI = targetXform;
			nextEyeTargetPOI = null;
			nextTargetLeftEyeXform = nextTargetRightEyeXform = null;

			if ( eyeControlData.eyeControl != EyeControlData.EyeControl.None )
			{
				SetMacroSaccadeTarget ( GetCurrentEyeTargetPos() );
				timeToMacroSaccade = Random.Range(1.5f, 2.5f);
			}

			if ( currentHeadTargetPOI == null )
				currentHeadTargetPOI = currentEyeTargetPOI;
		}



		void StartHeadMovement( Transform targetXform=null )
		{
			headLatency = 0;
			currentHeadTargetPOI = targetXform;
			nextHeadTargetPOI = null;

			Vector3 localAngles = headTargetPivotXform.localEulerAngles;
			Vector3 targetLocalAngles = Quaternion.LookRotation( headParentXform.InverseTransformPoint( GetCurrentHeadTargetPos() ), headParentXform.up ).eulerAngles;
						targetLocalAngles = new Vector3(	LimitHeadAngle(targetLocalAngles.x),
																			LimitHeadAngle(targetLocalAngles.y),
																			LimitHeadAngle(targetLocalAngles.z) );

			float horizDistance = Mathf.Abs(Mathf.DeltaAngle(localAngles.y, targetLocalAngles.y));
			float vertDistance = Mathf.Abs(Mathf.DeltaAngle(localAngles.x, targetLocalAngles.x));
			bool isQuickMove = headSpeed == HeadSpeed.Fast;

					// From my own experiments
					const float kSpeedIdle = 8.08f;
					const float mSpeedIdle = 1.824f;
					const float kSpeedProbeTo = 54.86f;
					const float mSpeedProbeTo = 1.94f;
					float kSpeed = isQuickMove ? kSpeedProbeTo : kSpeedIdle;
					float mSpeed = isQuickMove ? mSpeedProbeTo : mSpeedIdle;
			headMaxSpeedHoriz = kSpeed + mSpeed * horizDistance;
			headMaxSpeedVert = kSpeed + mSpeed * vertDistance;

					// From my own experiments
					const float kDurationIdle = 0.6231f;
					const float mDurationIdle = 0.009858f;
					const float kDurationProbeTo = 0.321f;
					const float mDurationProbeTo = 0.0116f;
					float kDuration = isQuickMove ? kDurationProbeTo : kDurationIdle;
					float mDuration = isQuickMove ? mDurationProbeTo : mDurationIdle;
			headHorizDuration = kDuration + mDuration * horizDistance;
			headVertDuration = kDuration + mDuration * vertDistance;

			//*** For letting the head keep tracking the target after orienting towards it
			{
				startHeadHorizDuration = headHorizDuration;
				startHeadVertDuration = headVertDuration;
				startHeadMaxSpeedHoriz = headMaxSpeedHoriz;
				startHeadMaxSpeedVert = headMaxSpeedVert;
				timeOfHeadMovementStart = Time.time;
			}

			if ( currentEyeTargetPOI == null && currentTargetLeftEyeXform == null )
				currentEyeTargetPOI = currentHeadTargetPOI;
		}




		void Update()
		{
			CheckLatencies();

			if ( eyeControlData.eyeControl != EyeControlData.EyeControl.None )
			{
				CheckMicroSaccades();
				CheckMacroSaccades();
			}

			if ( kDrawSightlinesInEditor )
				DrawSightlinesInEditor();
		}



		void UpdateBlinking()
		{
			if ( blinkState != BlinkState.Idle )
			{
				blinkStateTime += Time.deltaTime;
			
				if ( blinkStateTime >= blinkDuration )
				{
					blinkStateTime = 0;

					if ( blinkState == BlinkState.Closing )
					{
						if ( isShortBlink )
						{
							blinkState = BlinkState.Opening;
							blinkDuration = isShortBlink ? kBlinkOpenTimeShort : kBlinkOpenTimeLong;
							blink01 = 1;
						}
						else
						{
							blinkState = BlinkState.KeepingClosed;
							blinkDuration = kBlinkKeepingClosedTime;
							blink01 = 1;
						}
					}
					else if ( blinkState == BlinkState.KeepingClosed )
					{
						blinkState = BlinkState.Opening;
						blinkDuration = isShortBlink ? kBlinkOpenTimeShort : kBlinkOpenTimeLong;
					}
					else if ( blinkState == BlinkState.Opening )
					{
						blinkState = BlinkState.Idle;
						float minTime = Mathf.Max( 0.1f, Mathf.Min(kMinNextBlinkTime, kMaxNextBlinkTime));
						float maxTime = Mathf.Max( 0.1f, Mathf.Max(kMinNextBlinkTime, kMaxNextBlinkTime));
						timeOfNextBlink = Time.time + Random.Range( minTime, maxTime);
						blink01 = 0;
					}
				}
				else
					blink01 = Utils.EaseSineIn(	blinkStateTime,
																blinkState == BlinkState.Closing ? 0 : 1,
																blinkState == BlinkState.Closing ? 1 : -1,
																blinkDuration );
			}
		
		
			if ( Time.time >= timeOfNextBlink )
				Blink();
		}



		void UpdateEyelids()
		{
			if ( eyelidControlData.eyelidControl == EyelidControlData.EyelidControl.Bones || eyelidControlData.eyelidControl == EyelidControlData.EyelidControl.Blendshapes )
			{
				float leftEyeLocalAngle = -currentLeftEyeLocalEuler.x;

				float upperAngle = 0;
				float lowerAngle = 0;
				if ( useUpperEyelids )
				{
					float upperEyeLidTargetBaseAngle =Utils.NormalizedDegAngle(leftEyeLocalAngle)*kUpperEyelidFollowEyeFactor;
					upperEyeLidBaseAngle = Mathf.Lerp(upperEyeLidBaseAngle, upperEyeLidTargetBaseAngle, Time.deltaTime * 40);
					upperAngle = upperEyeLidBaseAngle;
				}

				if ( useLowerEyelids )
				{
					float lowerEyeLidTargetBaseAngle = Utils.NormalizedDegAngle(leftEyeLocalAngle)*kLowerEyelidFollowEyeFactor;
					lowerEyeLidBaseAngle = Mathf.Lerp(lowerEyeLidBaseAngle, lowerEyeLidTargetBaseAngle, Time.deltaTime * 40);
					lowerAngle = lowerEyeLidBaseAngle;
				}

				eyelidControlData.UpdateEyelids( upperAngle, lowerAngle, blink01 );
			}
		}



		void UpdateEyeMovement()
		{
			if ( lookTarget == LookTarget.ClearingTargetPhase2 )
			{
				if ( Time.time - timeOfEnteringClearingPhase >= 1 )
					lookTarget = LookTarget.StraightAhead;
				else
				{
					leftEyeAnchor.localRotation = lastLeftEyeLocalRotation = Quaternion.Slerp(lastLeftEyeLocalRotation, originalLeftEyeLocalQ, Time.deltaTime);
					rightEyeAnchor.localRotation = lastRightEyeLocalQ = Quaternion.Slerp(lastRightEyeLocalQ, originalRightEyeLocalQ, Time.deltaTime);
				}

				return;
			}

			if ( lookTarget == LookTarget.ClearingTargetPhase1 )
			{
				if ( Time.time - timeOfEnteringClearingPhase >= 2 )
				{
					lookTarget = LookTarget.ClearingTargetPhase2;
					timeOfEnteringClearingPhase = Time.time;
				}
			}
		
					bool shouldDoSocialTriangle =		lookTarget == LookTarget.Face &&
																	faceLookTarget != FaceLookTarget.EyesCenter;
					Transform trans = currentEyeTargetPOI ?? currentTargetLeftEyeXform;
					Vector3 eyeTargetGlobal = shouldDoSocialTriangle	? GetLookTargetPosForSocialTriangle( faceLookTarget )
																								: trans.TransformPoint(microSaccadeTargetLocal);

					//*** Prevent cross-eyes
					{
						Vector3 ownEyeCenter = GetOwnEyeCenter();
						Vector3 eyeCenterToTarget = eyeTargetGlobal - ownEyeCenter;
						float scale =  eyeDistance/0.064f;
						float distance = eyeCenterToTarget.magnitude / scale;
						const float kMinDistCrosseye = 0.6f;
						if ( distance < kMinDistCrosseye )
						{
							float x = kMinDistCrosseye - distance + 1;
							float modifiedDistance = kMinDistCrosseye - (1.0f/x - (2-x));
							eyeTargetGlobal = ownEyeCenter + modifiedDistance * (eyeCenterToTarget/distance);
						}
					}

					
			//*** After the eyes saccaded to the new POI, adjust eye duration and speed so they keep tracking the target quickly enough.
			{
				const float kEyeDurationForTracking = 0.005f;
				const float kEyeMaxSpeedForTracking = 600;

				float timeSinceLeftEyeHorizInitiatedMovementStop = Time.time-(timeOfEyeMovementStart + 1.5f * startLeftEyeHorizDuration);
				if ( timeSinceLeftEyeHorizInitiatedMovementStop > 0 )
				{
					leftHorizDuration = kEyeDurationForTracking + startLeftEyeHorizDuration/(1 + timeSinceLeftEyeHorizInitiatedMovementStop);
					leftMaxSpeedHoriz = kEyeMaxSpeedForTracking - startLeftEyeMaxSpeedHoriz/(1 + timeSinceLeftEyeHorizInitiatedMovementStop);
				}

				float timeSinceLeftEyeVertInitiatedMovementStop = Time.time-(timeOfEyeMovementStart + 1.5f * startLeftEyeVertDuration);
				if ( timeSinceLeftEyeVertInitiatedMovementStop > 0 )
				{
					leftVertDuration = kEyeDurationForTracking + startLeftEyeVertDuration/(1 + timeSinceLeftEyeVertInitiatedMovementStop);
					leftMaxSpeedVert = kEyeMaxSpeedForTracking - startLeftEyeMaxSpeedVert/(1 + timeSinceLeftEyeVertInitiatedMovementStop);
				}

				float timeSinceRightEyeHorizInitiatedMovementStop = Time.time-(timeOfEyeMovementStart + 1.5f * startRightEyeHorizDuration);
				if ( timeSinceRightEyeHorizInitiatedMovementStop > 0 )
				{
					rightHorizDuration = kEyeDurationForTracking + startRightEyeHorizDuration/(1 + timeSinceRightEyeHorizInitiatedMovementStop);
					rightMaxSpeedHoriz = kEyeMaxSpeedForTracking - startRightEyeMaxSpeedHoriz/(1 + timeSinceRightEyeHorizInitiatedMovementStop);
				}

				float timeSinceRightEyeVertInitiatedMovementStop = Time.time-(timeOfEyeMovementStart + 1.5f * startRightEyeVertDuration);
				if ( timeSinceRightEyeVertInitiatedMovementStop > 0 )
				{
					rightVertDuration = kEyeDurationForTracking + startRightEyeVertDuration/(1 + timeSinceRightEyeVertInitiatedMovementStop);
					rightMaxSpeedVert = kEyeMaxSpeedForTracking - startRightEyeMaxSpeedVert/(1 + timeSinceRightEyeVertInitiatedMovementStop);
				}
			}


					Vector3 leftEyeTargetAngles = Quaternion.LookRotation(eyesRootXform.InverseTransformDirection( eyeTargetGlobal - leftEyeAnchor.position), eyesRootXform.up).eulerAngles;
					leftEyeTargetAngles = new Vector3(ClampVertEyeAngle(leftEyeTargetAngles.x),
																		ClampHorizEyeAngle(leftEyeTargetAngles.y),
																		leftEyeTargetAngles.z);
			currentLeftEyeLocalEuler = new Vector3(	ClampVertEyeAngle(Mathf.SmoothDampAngle(	currentLeftEyeLocalEuler.x,
																																			leftEyeTargetAngles.x,
																																			ref leftCurrentSpeedX,
																																			leftVertDuration,
																																			Mathf.Max(4*headMaxSpeedVert, leftMaxSpeedVert))),
																		ClampHorizEyeAngle(Mathf.SmoothDampAngle(	currentLeftEyeLocalEuler.y,
																																				leftEyeTargetAngles.y,
																																				ref leftCurrentSpeedY,
																																				leftHorizDuration,
																																				Mathf.Max(4*headMaxSpeedHoriz, leftMaxSpeedHoriz))),
																		leftEyeTargetAngles.z);

			leftEyeAnchor.localRotation = Quaternion.Inverse(leftEyeAnchor.parent.rotation) * eyesRootXform.rotation * Quaternion.Euler( currentLeftEyeLocalEuler ) * leftAnchorToEyeQ;

					Vector3 rightEyeTargetAngles = Quaternion.LookRotation(eyesRootXform.InverseTransformDirection( eyeTargetGlobal - rightEyeAnchor.position), eyesRootXform.up).eulerAngles;
					rightEyeTargetAngles = new Vector3(	ClampVertEyeAngle(rightEyeTargetAngles.x),
																			ClampHorizEyeAngle(rightEyeTargetAngles.y),
																			rightEyeTargetAngles.z);
			currentRightEyeLocalEuler= new Vector3( ClampVertEyeAngle(Mathf.SmoothDampAngle(	currentRightEyeLocalEuler.x,
																																			rightEyeTargetAngles.x,
																																			ref rightCurrentSpeedX,
																																			rightVertDuration,
																																			Mathf.Max(4*headMaxSpeedVert, rightMaxSpeedVert))),
																		ClampHorizEyeAngle(Mathf.SmoothDampAngle(currentRightEyeLocalEuler.y,
																																			rightEyeTargetAngles.y,
																																			ref rightCurrentSpeedY,
																																			rightHorizDuration,
																																			Mathf.Max(4*headMaxSpeedHoriz, rightMaxSpeedHoriz))),
																		rightEyeTargetAngles.z);

			rightEyeAnchor.localRotation = Quaternion.Inverse(rightEyeAnchor.parent.rotation) * eyesRootXform.rotation * Quaternion.Euler( currentRightEyeLocalEuler ) * rightAnchorToEyeQ;

			lastLeftEyeLocalRotation = leftEyeAnchor.localRotation;
			lastRightEyeLocalQ = rightEyeAnchor.localRotation;
		}



		void UpdateHeadMovement()
		{
			if ( ikWeight <= 0 )
				return;

			//*** After the head moved to the new POI, adjust head duration so the head keeps tracking the target quickly enough.
			{
				const float kHeadDurationForTracking = 0.1f;
				const float kHeadMaxSpeedForTracking = 150;
				float timeSinceHorizInitiatedHeadMovementStop = Time.time-(timeOfHeadMovementStart + 1.5f * startHeadHorizDuration);
				if ( timeSinceHorizInitiatedHeadMovementStop > 0 )
				{
					headHorizDuration = kHeadDurationForTracking + startHeadHorizDuration/(1 + timeSinceHorizInitiatedHeadMovementStop);
					headMaxSpeedHoriz = kHeadMaxSpeedForTracking - startHeadMaxSpeedHoriz/(1 + timeSinceHorizInitiatedHeadMovementStop);
				}


				float timeSinceVertInitiatedHeadMovementStop = Time.time-(timeOfHeadMovementStart + 1.5f * startHeadVertDuration);
				if ( timeSinceVertInitiatedHeadMovementStop  > 0 )
				{
					headVertDuration = kHeadDurationForTracking + startHeadVertDuration/(1 + timeSinceVertInitiatedHeadMovementStop);
					headMaxSpeedVert = kHeadMaxSpeedForTracking - startHeadMaxSpeedVert/(1 + timeSinceVertInitiatedHeadMovementStop);
				}
			}

			Vector3 localAngles = headTargetPivotXform.localEulerAngles;
			Vector3 targetLocalAngles = Quaternion.LookRotation( headParentXform.InverseTransformPoint( GetCurrentHeadTargetPos() ), headParentXform.up ).eulerAngles;
			targetLocalAngles = new Vector3(	LimitHeadAngle(targetLocalAngles.x),
																LimitHeadAngle(targetLocalAngles.y),
																LimitHeadAngle(targetLocalAngles.z) );

			headTargetPivotXform.localEulerAngles = new Vector3( Mathf.SmoothDampAngle(	localAngles.x,
																																	targetLocalAngles.x,
																																	ref currentHeadVertSpeed,
																																	headVertDuration,
																																	headMaxSpeedVert),
																							Mathf.SmoothDampAngle(	localAngles.y,
																																	targetLocalAngles.y,
																																	ref currentHeadHorizSpeed,
																																	headHorizDuration,
																																	headMaxSpeedHoriz),
																							Mathf.SmoothDampAngle(	localAngles.z,
																																	targetLocalAngles.z,
																																	ref currentHeadZSpeed,
																																	headHorizDuration,
																																	headMaxSpeedHoriz) );
		}

	}
}