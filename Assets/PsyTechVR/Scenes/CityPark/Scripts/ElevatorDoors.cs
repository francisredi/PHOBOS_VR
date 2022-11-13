using UnityEngine;
using System.Collections;

public class ElevatorDoors : MonoBehaviour {
	
	public string LeftAnim = "leftDoor";
	public string RightAnim = "rightDoor";
	
	
	private int mInCount;
	private bool mDoorsOpen = false; 
	
	void Awake () {	
		mInCount = 0;
	}
	
	
	public void CloseDoors()
	{
		Animation[] animations = GetComponentsInChildren<Animation>();
		AnimationState anim = animations[0][LeftAnim];
		anim.speed = -1;
		anim.time = anim.length;
		anim = animations[1][RightAnim];
		anim.speed = -1;
		anim.time = anim.length;
		animations[0].Play();
		animations[1].Play();
		
		mDoorsOpen = false;
	}
	
	public void OpenDoors()
	{
		Animation[] animations = GetComponentsInChildren<Animation>();
		AnimationState anim = animations[0][LeftAnim];
		anim.speed = 1;
		anim = animations[1][RightAnim];
		anim.speed = 1;
		animations[0].Play();
		animations[1].Play();
		
		mDoorsOpen = true;
	}
	
	public bool IsDoorOpen { get 
		{ return mDoorsOpen; }
	}
	public bool IsDoorClosed { get 
		{ return !mDoorsOpen && !GetComponentsInChildren<Animation>()[0][LeftAnim].enabled; }
	}
	
	public bool IsAnimPlaying { get
		{ return GetComponentInChildren<Animation>().isPlaying; }
	}
	
	public void Disable()
	{
		GetComponent<Collider>().enabled = false;
		mInCount = 0;
	}
	
	public void Enable()
	{
		GetComponent<Collider>().enabled = true;
		mInCount = 0;
	}
	

	public void IgnoreFirstHit ()
	{
		mInCount = 1;
		//mInCount = -1; // hack, when the player is warped in, we don't want to open the doors since it is already in
	}
	
	
	void OnTriggerEnter (Collider other )
	{
		GameObject character = other.gameObject;
		if(character.tag != "Player") return;

		//Debug.Log ("Enter trigger");
		if(mInCount == 0)
		{
			OpenDoors();
			mInCount++;
		}	
		
		//mInCount++;
	}
	
	void OnTriggerExit (Collider other )
	{
		GameObject character = other.gameObject;
		if(character.tag != "Player") return;

		mInCount--;
		
		if(mInCount == 0)
		{
			CloseDoors();
		}
	}
}
