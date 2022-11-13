using UnityEngine;
using System.Collections;

// warning, a lot of magic numbers and frame rate dependant stuff here.
public class SitWhenClose : MonoBehaviour {
	
	public float CanSitDist = 1f;
	public GameObject SitRef;
	public GameObject Spider;
	
	enum EState
	{
		Walking,
		Sitting,
		Sit
	}
	
	private EState mState;
	private bool mColliding;
	private bool mDisplayingHint;
	private Vector3 mCollideNormal;
	private Rewired.Player mInput;
	
	
	void Start () {
		Spider.SetActive (false); // spider
		mInput = Rewired.ReInput.players.GetPlayer(0);
		mState = EState.Walking;
		mColliding = false;
		mDisplayingHint = false;
	}
	
	void Update () {

		Transform refTransform = transform;

		if (mInput.GetButtonDown ("X")) {
			Spider.SetActive (!Spider.activeSelf); // spider
		}
		
		if( mState == EState.Sitting && !GetComponent<Animation>().isPlaying) // we don't want to do anything while the animation is playing
		{			
			/*if( mColliding )
			{
				Vector3 xzFwd = refTransform.forward;
				xzFwd.y = 0; xzFwd.Normalize();
				refTransform.RotateAround(Vector3.up, Vector3.Dot(mCollideNormal, xzFwd) * 0.04f);
				//float direction = transform.InverseTransformPoint(SitRef.transform.position).z < 0? -1 : 1;
				//direction *= Mathf.Min(1f, Utils.GetDist(gameObject, SitRef));
				GetComponent<CharacterController>().Move(refTransform.forward * Time.deltaTime);
			}
			else
			{
				refTransform.forward = Vector3.Lerp(refTransform.forward, (SitRef.transform.position - refTransform.position).normalized, 0.05f);
				GetComponent<CharacterController>().Move(refTransform.forward * Time.deltaTime * 2f);
			}*/
			
			
			if( Utils.GetDist(gameObject, SitRef) < 0.3f )
			{
				GetComponent<CharacterMotor>().enabled = false;
				GetComponent<CharacterController>().enabled = false;
								
				GetComponent<Animation>().Play("SitDown");

				// stop walking sound
				if(GetComponent<AudioSource>())
					GetComponent<AudioSource>().Stop();
				
				// StartCoroutine(EnableMouseLook()); // now done via animation event
			}
			
			return;
		}


		
		float lookatDot = Vector3.Dot (refTransform.forward, SitRef.transform.forward );
		if( Utils.GetDist(gameObject, SitRef) < CanSitDist && lookatDot > 0.7f ) // we just want to enable sitting while looking in the same direction as the sit reference
		{
			if (mInput.GetButtonDown("B"))
			{
				if(mState == EState.Walking) 
				{
					mState = EState.Sitting;
				}
				else if( mState == EState.Sit && !GetComponent<Animation>().isPlaying)
				{
					GetComponent<Animation>().Play("StandUp");
					
					//StartCoroutine(EnableControl()); // now done via animation event
				}
			}
			
			if(!mDisplayingHint)
			{
				mDisplayingHint = true;
				SceneGUI.Instance.addRenderCallback(DisplayText);
			}
		}
		else if(mDisplayingHint)
		{
			mDisplayingHint = false;
			SceneGUI.Instance.removeRenderCallback(DisplayText);
		}

	}
	
	
	
	void OnTriggerExit(Collider other)
	{
		mColliding = false;
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		// we want to get our player around the chair only when sitting.
		if( mState != EState.Sitting )
			return;
		
		if( Vector3.Dot( hit.normal, Vector3.up ) > 0.9f )
			return; // we don't care about collisioning with the floor
		
		if( hit.gameObject.name == "mesa" )
			return; // ignore this obj!
		
		mCollideNormal = hit.normal;		
		mColliding = true;
	}

	void EnableMouseLook ()
	{
		//yield return new WaitForSeconds(2f);
		
		mState = EState.Sit;

		GetComponent<MouseLook>().enabled = true;
	}
	
	void EnableControl ()
	{
		//yield return new WaitForSeconds(2f);
		
		//SendMessage("SetControllable", true);
		GetComponent<CharacterMotor>().enabled = true;
		GetComponent<CharacterController>().enabled = true;
		mState = EState.Walking;
	}
	
	
	void DisplayText(bool isStereo)
	{
		if( mState == EState.Sitting )
			return;
		
		/*Transform refTransform = SwitchOVR.GetIsOVRMode()? transform.Find("ForwardDirection") : transform;
		float lookatDot = Vector3.Dot (refTransform.forward, SitRef.transform.forward );*/
		
		string text = "";
		if( mState == EState.Walking /*&& Utils.GetDist(gameObject, SitRef) < CanSitDist && lookatDot > 0.7*/)
			text = "Press 'B', then get closer to the chair to sit down";
		else if( mState == EState.Sit )
			text = "Press 'X' to show or hide the spider\nPress 'B' to stand up";
		
		if( text == "") return;

		SceneGUI.Instance.drawText(250, 300, 800, 90,ref  text, Color.white);
	}
}
