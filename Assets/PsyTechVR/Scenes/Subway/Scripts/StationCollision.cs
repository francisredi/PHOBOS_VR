using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StationCollision : MonoBehaviour {
	
	public SubwayAIControl AIControl;
	public string Station;
		
	private GameObject mLeftSideCollisionObj;
	private GameObject mRightSideCollisionObj;
	
	void Awake()
	{
		mLeftSideCollisionObj = transform.Find("platformFullLeft").gameObject;
		mRightSideCollisionObj = transform.Find("platformFullRight").gameObject;
	}
	
	void Start()
	{
		AIControl.Register( this );
	}
	
	
	public void EnableCollisions(string side)
	{
		if(side == "Right")
			mRightSideCollisionObj.SetActive(true);
		else
			mLeftSideCollisionObj.SetActive(true);
	}
	
	public void DisableCollisions(string side)
	{
		if(side == "Right")
			mRightSideCollisionObj.SetActive(false);
		else
			mLeftSideCollisionObj.SetActive(false);
	}
}
