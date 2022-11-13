using UnityEngine;
using System.Collections;

public class ExternalElevator : MonoBehaviour {
	
	public GameObject MovementReference;
	public HotelElevator MainElevator;
	public float MaxSpeed = 0.075f;
	public float Accel = 0.01f;
	public GameObject Player;
	public AudioClip[] Sounds;

	private string Msg = "Press 'X' to go up.\nPress 'Y' to go down.\nPress 'A' or 'B' to stop the elevator.";
	
	private bool mIsPlayerInside;
	private bool mMoving;
	private float mSpd;
	private float mMiny;
	private float mMaxy;
	private HotelDoors[] mDoorControl;
	private Rewired.Player mInput;
	
	
	public static float TOP;
	public static float BOTTOM;
	
	private const float HINT_TIME = 5;


	void Awake()
	{
        Player = GameObject.FindGameObjectWithTag("Player");
		BOTTOM = transform.position.y;
		TOP = MovementReference.GetComponent<Renderer>().bounds.max.y - GetComponent<Renderer>().bounds.extents.y *2;
		
		mDoorControl = GetComponentsInChildren<HotelDoors>();
		mInput = Rewired.ReInput.players.GetPlayer(0);
	}
	
	
	void Start () {
		mIsPlayerInside = false;
		mMoving = false;
		mSpd = 0;
		
		mMiny = transform.position.y;
		mMaxy = MovementReference.GetComponent<Renderer>().bounds.max.y - GetComponent<Renderer>().bounds.extents.y *2;
	}
	
	void FixedUpdate () {
		
		if (!mIsPlayerInside) return;

		if(mInput.GetButton("X") && !isAtTop) { // if press up but not at top
			if(!mMoving){ // first movement
                SceneGUI.Instance.removeRenderCallback(DrawInstructions);
                foreach (HotelDoors doorCtrl in mDoorControl){
					if(doorCtrl.IsDoorOpen) doorCtrl.CloseDoors();
					doorCtrl.Disable(); // disable doors
				}

                // we need the other elevator to be in the rooftop if we're going there, because if not, there would be an empty space there.
                MainElevator.InstantGoToFloor(HotelElevator.ROOFTOP_FLOOR);

                mMoving = true;
                PlayStartSound();
			}
			
			mSpd += Accel;
			mSpd = Mathf.Min(mSpd, MaxSpeed);
		}
		else if(mInput.GetButton("Y") && !isAtBottom){
			if(!mMoving){ // first movement
                SceneGUI.Instance.removeRenderCallback(DrawInstructions);
                foreach (HotelDoors doorCtrl in mDoorControl){
					if(doorCtrl.IsDoorOpen) doorCtrl.CloseDoors();
					doorCtrl.Disable(); // disable doors
				}

                // we need the other elevator to be in the 1st floor if we're going down, because if not, there would be an empty space there
                MainElevator.InstantGoToFloor(HotelElevator.FIRST_FLOOR);

                mMoving = true;
                PlayStartSound();
            }
			
			mSpd -= Accel;
			mSpd = Mathf.Max(mSpd, -MaxSpeed);
		}
		else if(mInput.GetButtonDown("A") || mInput.GetButtonDown("B"))
        {
			if(mMoving){
				mSpd = 0;
				PlayStopSound();
				mMoving = false;
            }
		}
		
		
		
		if ( !mDoorControl[1].GetComponent<Collider>().enabled && !mDoorControl[1].IsAnimPlaying &&
			 !mDoorControl[0].GetComponent<Collider>().enabled && !mDoorControl[0].IsAnimPlaying) { // only move when front doors are closed
			float deAccel = 1f; // used when we're reaching top or bottom
			if( mSpd > 0 )
				deAccel = Mathf.Clamp(mMaxy - transform.position.y, 0f, 1f);
			else if( mSpd < 0 )
				deAccel = Mathf.Clamp(transform.position.y - mMiny, 0f, 1f);
			Vector3 spd = new Vector3(0, mSpd, 0);
			transform.Translate( spd * deAccel);
		}
		else
			mSpd = 0;
		
		
		if( mMoving ){			
			if( mSpd < 0f && isAtBottom ){
				mSpd = 0;
				mMoving = false;
				PlayStopSound();
				
				// enable front door, disable back door
				mDoorControl[0].Disable();
				mDoorControl[1].Enable();
				mDoorControl[1].OpenDoors();
            }
			else if( mSpd > 0f && isAtTop ){	
				mSpd = 0;
				mMoving = false;
				PlayStopSound();
				
				// disable front door, enable back door			
				mDoorControl[0].Enable();
				mDoorControl[1].Disable();
            }
		}
	}
	
	
	private const float DIST_EPSILON = 0.002f;
	bool isAtBottom { get{ return Mathf.Abs(transform.position.y - mMiny) < DIST_EPSILON; } }
	bool isAtTop { get{ return Mathf.Abs(transform.position.y - mMaxy) < DIST_EPSILON; }}
	
	void DrawInstructions(bool isStereo)
	{
		SceneGUI.Instance.drawText(300, 300, 600, 60,ref  Msg, Color.white);
	}
	
	void OnTriggerEnter (Collider other )
	{	
		GameObject character = other.gameObject;
		if(character.tag != "Player")
			return;

        character.transform.parent = transform;
		
		mIsPlayerInside = true;
		SceneGUI.Instance.addRenderCallback(DrawInstructions);
		//StartCoroutine(HideInstructions());
	}
	
	void OnTriggerExit (Collider other )
	{
		GameObject character = other.gameObject;
		if(character.tag != "Player")
			return;

        character.transform.parent = null;

        mIsPlayerInside = false;
		SceneGUI.Instance.removeRenderCallback(DrawInstructions);
	}

	void PlayStopSound ()
	{
		GetComponent<AudioSource>().Stop();// stop the "moving" loop
		
		GetComponent<AudioSource>().clip = Sounds[2];
        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().Play();
	}

	void PlayStartSound ()
	{
		GetComponent<AudioSource>().clip = Sounds[0];
		GetComponent<AudioSource>().loop = false;
		GetComponent<AudioSource>().Play();
		
		StartCoroutine(PlayDelayedLoop(GetComponent<AudioSource>().clip.length, Sounds[1]));
	}
	
	
	IEnumerator PlayDelayedLoop(float delay, AudioClip sound)
	{
		yield return new WaitForSeconds(delay);

        if (mMoving == true){ // if after the delay the elevator is still moving
            GetComponent<AudioSource>().clip = sound;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().Play();
        }
	}
	
	public void InstantGoTo( float dest )
	{
		transform.position = new Vector3(transform.position.x, dest, transform.position.z);
		
		// enable the door automatic open for the right door.
		if( dest == TOP )
		{
			mDoorControl[0].Enable();
			mDoorControl[1].Disable();
		}
		else if( dest == BOTTOM )
		{
			mDoorControl[1].Enable();
			mDoorControl[0].Disable();
		}
	}
	
	/*private IEnumerator HideInstructions()
	{
		yield return new WaitForSeconds(HINT_TIME);
		
		SceneGUI.Instance.removeRenderCallback(DrawInstructions);
	}*/
}
