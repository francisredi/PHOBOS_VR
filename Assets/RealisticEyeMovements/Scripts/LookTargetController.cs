using UnityEngine;


namespace RealisticEyeMovements {

	public class LookTargetController : MonoBehaviour
	{
		#region fields

			[Tooltip("Drag objects here for the actor to look at. If empty, actor will look in random directions.")]
			public Transform[] pointsOfInterest;

			[Tooltip("Ratio of how often to look at player vs elsewhere. 0: never, 1: always")]
			[Range(0,1)]
			public float lookAtPlayerRatio = 0.1f;

			[Tooltip("How likely the actor is to look back at the player when player stares at actor.")]
			[Range(0,1)]
			public float stareBackFactor = 0;

			[Tooltip("If player is closer than this, look at him")]
			[Range(0, 100)]
			public float noticePlayerDistance = 0;

			[Tooltip("Minimum time to look at a target")]
			[Range(1f, 100f)]
			public float minLookTime = 3f;

			[Tooltip("Maximum time to look at a target")]
			[Range(1f, 100f)]
			public float maxLookTime = 10f;

			EyeAndHeadAnimator eyeAndHeadAnimator;

			const float minLookAtMeTimeToReact = 4;

			Transform targetPOI;
		
			Transform playerEyeCenterXform;
			Transform playerLeftEyeXform;
			Transform playerRightEyeXform;
		
			bool isLookingAtPlayer;

			float lastDistanceToPlayer = -1;
			float playerLookingAtMeTime;
			float nextChangePOITime;
			float stareBackDeadtime;	
			float timeOfLastNoticeCheck = -1000;
			float timeOfLastLookBackCheck = -1000;
			float timeOutsideOfAwarenessZone = 1000;

		#endregion
	
	

		/*public virtual void Awake()
		{
			eyeAndHeadAnimator = GetComponent<EyeAndHeadAnimator>();
		
			//*** Player eyes
			{
				GameObject ovrRigGO = GameObject.Find ("OVRCameraRig");
				if ( ovrRigGO != null ) // Using Oculus VR?
				{
					Transform ovrXform = ovrRigGO.transform;
					playerLeftEyeXform = ovrXform.Find("LeftEyeAnchor");
					playerRightEyeXform = ovrXform.Find("RightEyeAnchor");
					playerEyeCenterXform = ovrXform.Find("CenterEyeAnchor");
				}
				else
					playerEyeCenterXform = GameObject.Find ("FirstPersonCharacter").transform;
				
				if ( playerEyeCenterXform == null && lookAtPlayerRatio > 0 )
					Debug.LogError("Player camera not found");
			}
		}*/

		public virtual void Awake()
		{
			eyeAndHeadAnimator = GetComponent<EyeAndHeadAnimator>();
			playerEyeCenterXform = Camera.main.transform;
		
			if ( playerEyeCenterXform == null && lookAtPlayerRatio > 0 ) Debug.LogError("Player camera not found");
		}

	

	
		public void Blink()
		{
			eyeAndHeadAnimator.Blink();
		}



		Vector3 ChooseNextHeadTargetPoint()
		{
			float scale = eyeAndHeadAnimator.eyeDistance / 0.064f;
			bool hasBoneEyelidControl = eyeAndHeadAnimator.eyelidControlData.eyelidControl == EyelidControlData.EyelidControl.Bones;
			float angleVert = Random.Range(-0.5f * (hasBoneEyelidControl ? 6f : 3f), hasBoneEyelidControl ? 6f : 4f);
			float angleHoriz = Random.Range(-10f, 10f);
			return eyeAndHeadAnimator.GetOwnEyeCenter() + scale * Random.Range(3.0f, 5.0f) *
																						transform.TransformDirection((Quaternion.Euler(	angleVert, angleHoriz, 0) *  Vector3.forward));
		}



		Transform ChooseNextHeadTargetPOI()
		{
			if ( pointsOfInterest == null || pointsOfInterest.Length == 0 )
				return null;

			int index = Random.Range(0, pointsOfInterest.Length);
			if ( pointsOfInterest[ index ] == targetPOI )
				index = (index + Random.Range(0, pointsOfInterest.Length-1)) % pointsOfInterest.Length;
			
			return pointsOfInterest[ index ];
		}



		public void ClearLookTarget()
		{
			eyeAndHeadAnimator.ClearLookTarget();
			nextChangePOITime = -1;
		}



		public bool IsPlayerInView()
		{
			return (playerEyeCenterXform != null) && eyeAndHeadAnimator.IsInView( playerEyeCenterXform.position );
		}



		public void LookAtPlayer()
		{
			if ( playerLeftEyeXform != null && playerRightEyeXform	!= null )
				eyeAndHeadAnimator.LookAtFace( playerLeftEyeXform, playerRightEyeXform );
			else
				eyeAndHeadAnimator.LookAtFace( playerEyeCenterXform );
			
			nextChangePOITime = Time.time + Random.Range(Mathf.Min(minLookTime, maxLookTime), Mathf.Max(minLookTime, maxLookTime));

			targetPOI = null;
			isLookingAtPlayer = true;
			timeOutsideOfAwarenessZone = 0;
		}
	
	
	
		public void LookAroundIdly()
		{
			if ( isLookingAtPlayer )
				stareBackDeadtime = Random.Range(10.0f, 30.0f);
			
			targetPOI = ChooseNextHeadTargetPOI();
			if ( targetPOI != null )
				eyeAndHeadAnimator.LookAtAreaAround( targetPOI );
			else
				eyeAndHeadAnimator.LookAtAreaAround(ChooseNextHeadTargetPoint());

			nextChangePOITime = Time.time + Random.Range(Mathf.Min(minLookTime, maxLookTime), Mathf.Max(minLookTime, maxLookTime));
		
			isLookingAtPlayer = false;
		}



		public void LookAtPoiDirectly( Transform poiXform )
		{
			eyeAndHeadAnimator.LookAtSpecificThing( poiXform );
			nextChangePOITime = -1;
			isLookingAtPlayer = false;
		}
	
	
	
		public void LookAtPoiDirectly( Vector3 poi )
		{
			eyeAndHeadAnimator.LookAtSpecificThing( poi );
			nextChangePOITime = -1;
			isLookingAtPlayer = false;
		}
	
	
	
		void Start()
		{
			if ( lookAtPlayerRatio >= 1 && IsPlayerInView() )
				LookAtPlayer();
			else
				LookAroundIdly();
		}



		void Update()
		{
	
			//*** Finished looking at current target?
			{
				if ( Time.time >= nextChangePOITime && eyeAndHeadAnimator.CanChangePointOfAttention() )
				{
					if ( Random.value <= lookAtPlayerRatio && IsPlayerInView() )
						LookAtPlayer();
					else
						LookAroundIdly();

					return;
				}
			}
		
		
			bool shouldLookBackAtPlayer = false;
			bool shouldNoticePlayer = false;

			Vector3 playerTargetPos = playerEyeCenterXform.position;
			float distanceToPlayer = Vector3.Distance(eyeAndHeadAnimator.GetOwnEyeCenter(), playerTargetPos);
			bool isPlayerInView = eyeAndHeadAnimator.IsInView( playerEyeCenterXform.position );
			bool isPlayerInAwarenessZone = isPlayerInView && distanceToPlayer < noticePlayerDistance;


			//*** Awareness zone
			{
				if ( isPlayerInAwarenessZone )
				{
					if ( Time.time - timeOfLastNoticeCheck > 0.1f && false == isLookingAtPlayer )
					{
						timeOfLastNoticeCheck = Time.time;
					
						bool isPlayerApproaching = lastDistanceToPlayer > distanceToPlayer;
						float closenessFactor01 = (noticePlayerDistance - distanceToPlayer)/noticePlayerDistance;
						float noticeProbability = Mathf.Lerp (0.1f, 0.5f, closenessFactor01);
						shouldNoticePlayer = isPlayerApproaching && timeOutsideOfAwarenessZone > 1 && Random.value < noticeProbability; 
					}
				}
				else
					timeOutsideOfAwarenessZone += Time.deltaTime;
			}


			//*** If the player keeps staring at us, stare back?		
			{
				if ( lookAtPlayerRatio > 0 && playerEyeCenterXform != null )
				{
					float playerLookingAtMeAngle = eyeAndHeadAnimator.GetStareAngleTargetAtMe( playerEyeCenterXform );
					bool isPlayerLookingAtMe = playerLookingAtMeAngle < 15;
		
					playerLookingAtMeTime = (isPlayerInView && isPlayerLookingAtMe	)	? Mathf.Min(10, playerLookingAtMeTime + Mathf.Cos(Mathf.Deg2Rad * playerLookingAtMeAngle) * Time.deltaTime)
																														: Mathf.Max(0, playerLookingAtMeTime - Time.deltaTime);
			
					if ( false == eyeAndHeadAnimator.IsLookingAtFace() )
					{
						if ( stareBackDeadtime > 0 )
							stareBackDeadtime -= Time.deltaTime;
						
						if (	stareBackDeadtime <= 0  &&
							Time.time - timeOfLastLookBackCheck > 0.1f &&
							playerLookingAtMeTime > minLookAtMeTimeToReact  &&
							eyeAndHeadAnimator.CanChangePointOfAttention() &&
							isPlayerLookingAtMe )
						{
							timeOfLastLookBackCheck = Time.time;
							
							float lookTimeProbability = stareBackFactor * 2 * (Mathf.Min(10, playerLookingAtMeTime) - minLookAtMeTimeToReact) / (10-minLookAtMeTimeToReact);
							shouldLookBackAtPlayer = Random.value < lookTimeProbability;
						}
					}
				}
			}

			if ( shouldLookBackAtPlayer || shouldNoticePlayer )
				LookAtPlayer();

			lastDistanceToPlayer = distanceToPlayer;


		}

	}
}