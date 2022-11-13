using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HotelElevator : MonoBehaviour {
	
	public float MaxSpeed = 0.1f;
	public float Accel = 0.01f;
	public GameObject Player;
	public ExternalElevator SecondaryElevator;
	public GameObject[] Floors;
	public AudioClip[] Sounds;
	
	private bool mIsPlayerInside;
	private bool mMoving;
	private bool mWarping;	
	private ElevatorDoors mDoorControl;
	private int mCurrentFloor = 1;
	private Rewired.Player input;

	public const int FIRST_FLOOR = 1;
	public const int ROOM_FLOOR = 25;
	public const int ROOFTOP_FLOOR = 29;

	private int desiredFloorDestination = 1;
	
	void Awake()
	{
        Player = GameObject.FindGameObjectWithTag("Player");
		mDoorControl = GetComponentInChildren<ElevatorDoors>();
		input = Rewired.ReInput.players.GetPlayer(0);
	}
	
	void Start () {
		mIsPlayerInside = false;
		mMoving = false;
		mWarping = false;
    }
	
	void FixedUpdate () {
		
		if( mMoving ) return; // no input if elevator is already moving!!!!
		if(!mIsPlayerInside || mWarping) return;
		
		if( input.GetButtonDown("B") && mCurrentFloor == ROOM_FLOOR )
		{
            Fade.OnFadeEnd = delegate () {SceneManager.LoadSceneAsync("PassageToOffice",LoadSceneMode.Single); };
            Fade.FadeOut();
			mWarping = true;
		}

		int destFloor = mCurrentFloor;

		if (input.GetButtonDown ("A")) {
			destFloor = desiredFloorDestination;
		}
		else if(input.GetButtonDown("X")) {
			desiredFloorDestination+=4;
			if(desiredFloorDestination > 29) desiredFloorDestination = 29;
		}
		else if(input.GetButtonDown("Y")){
			desiredFloorDestination-=4;
			if(desiredFloorDestination < 1) desiredFloorDestination = 1;
		}
		
		
		if( destFloor > mCurrentFloor )
		{			
			float waitTime = 0f;
			if( mDoorControl.IsDoorOpen )
			{
				mDoorControl.CloseDoors();
				waitTime = 2f;
			}
			
			StartCoroutine(MoveElevatorUp(destFloor, waitTime));
			
			mMoving = true;			
		}
		else if (destFloor < mCurrentFloor )
		{			
			float waitTime = 0f;
			if( mDoorControl.IsDoorOpen )
			{
				mDoorControl.CloseDoors();
				waitTime = 2f;
			}
			
			StartCoroutine(MoveElevatorDown(destFloor, waitTime));
				
			mMoving = true;
		}
	}
	
	IEnumerator MoveElevatorUp(int destFloor, float waitTime)
	{
		SecondaryElevator.InstantGoTo( ExternalElevator.TOP ); // we need the other elevator to be there if we get into the rooftop

        SceneGUI.Instance.removeRenderCallback(drawOptions);

        yield return new WaitForSeconds(waitTime);
		
		PlayStartSound();
		
		
		float maxY = Floors[(destFloor-1)/4].transform.position.y;
		Vector3 spd = new Vector3(0,0,0);
		
		const float epsilon = 0.001f;
		while( Mathf.Abs(transform.position.y - maxY) > epsilon)
		{	
			spd.y += Accel;
			spd.y = Mathf.Min(spd.y, MaxSpeed);
			transform.Translate( spd * Mathf.Clamp(maxY - transform.position.y, 0f, 1f), Space.World);
			
			yield return new WaitForFixedUpdate();
		}

		mMoving = false;
		GetComponent<AudioSource>().clip = Sounds[1];
		GetComponent<AudioSource>().loop = false;
		GetComponent<AudioSource>().Play();
		mCurrentFloor = destFloor;

        if (destFloor == ROOFTOP_FLOOR)
        {
            mDoorControl.OpenDoors();
        }
        else SceneGUI.Instance.addRenderCallback(drawOptions);
    }
	
	IEnumerator MoveElevatorDown(int destFloor, float waitTime)
	{
        SceneGUI.Instance.removeRenderCallback(drawOptions);

        yield return new WaitForSeconds(waitTime);
		
		PlayStartSound();
		
		
		Vector3 spd = new Vector3(0,0,0);
		
		float minY = Floors[(destFloor-1)/4].transform.position.y;
		
		const float epsilon = 0.001f;
		while( Mathf.Abs(transform.position.y - minY) > epsilon)
		{			
			spd.y -= Accel;
			spd.y = Mathf.Max(spd.y, -MaxSpeed);
			transform.Translate( spd * Mathf.Clamp(transform.position.y - minY, 0f, 1f), Space.World);
			
			yield return new WaitForFixedUpdate();
		}
		
		mMoving = false;
		GetComponent<AudioSource>().clip = Sounds[1];
		GetComponent<AudioSource>().loop = false;
		GetComponent<AudioSource>().Play();
		mCurrentFloor = destFloor;

        if (destFloor == 1 /* bottom level */ )
        {
            mDoorControl.OpenDoors();
        }
        else SceneGUI.Instance.addRenderCallback(drawOptions);

        SecondaryElevator.InstantGoTo( ExternalElevator.BOTTOM ); // we need the other elevator to be there if we go outside.
	}

    public void InstantGoToFloor(int floor)
    {
        if (floor != ROOFTOP_FLOOR && floor != FIRST_FLOOR) {
            mDoorControl.IgnoreFirstHit(); // ignore the door's trigger which would open the doors!
        }
		Vector3 elevPos = transform.position;
		elevPos.y = Floors[(floor-1)/4].transform.position.y;
		transform.position = elevPos;
		mCurrentFloor = floor;

		mIsPlayerInside = true;
		desiredFloorDestination = mCurrentFloor;
	}

	void drawOptions(bool isStereo)
	{
		if( mMoving )
			return;
		string text;
		if( mWarping )
		{
			text = "LOADING...";
		}
		else
		{
			string end;
			switch(desiredFloorDestination){
				case 1:
					end = " Ground Floor";
					break;
				case 25:
					end = " "+(desiredFloorDestination)+" (To Spider Room)";
					break;
				case 29:
					end = " Rooftop";
					break;
				default:
					end = " "+(desiredFloorDestination); // convert to American floor system
					break;
			}
			text = "Use 'X' for up or 'Y' for down to\nselect the desired floor destination.\nThen press 'A' to confirm\nFLOOR DESTINATION:"+end;

			if(mCurrentFloor == (ROOM_FLOOR)) text += "\nAccess Room: Press 'B'";
		}
		

		SceneGUI.Instance.drawText(300, 500, 800, 200,ref  text, Color.white);
	}
	
	void OnTriggerEnter (Collider other )
	{	
		GameObject character = other.gameObject;
		if(character.tag != "Player") return;

        character.transform.parent = transform;

        mIsPlayerInside = true;
		SceneGUI.Instance.addRenderCallback(drawOptions);
	}
	
	void OnTriggerExit (Collider other )
	{
		GameObject character = other.gameObject;
		if(character.tag != "Player") return;

        character.transform.parent = null;

        mIsPlayerInside = false;
		SceneGUI.Instance.removeRenderCallback(drawOptions);
	}

	void PlayStartSound ()
	{
		GetComponent<AudioSource>().clip = Sounds[0];
		GetComponent<AudioSource>().loop = true;
		GetComponent<AudioSource>().Play();
	}
}
