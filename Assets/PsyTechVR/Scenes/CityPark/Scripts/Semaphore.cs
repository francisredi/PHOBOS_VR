using UnityEngine;
using System.Collections;


public class Semaphore : MonoBehaviour {
	
	
	public enum EState
	{
		Red,
		Green,
		Yellow
	};
	
	
	public float SwitchTime = 2f;
	public int HorizontalStreet;
	public int VerticalStreet;
	public string Orientation; // horizontal or vertical. TODO: use an enum
	
	private GameObject mGreenLight;
	private GameObject mRedLight;
	private GameObject mYellowLight;
	private GameObject mGreenLightOp; // for the opposite lights
	private GameObject mRedLightOp;
	private GameObject mYellowLightOp;
	
	// Use this for initialization
	void Awake () {
		
		mGreenLight = transform.Find("greenlight").gameObject;
		mRedLight = transform.Find("redlight").gameObject;
		mYellowLight = transform.Find("yellowlight").gameObject;
		mGreenLightOp = transform.Find("greenlightop").gameObject;
		mRedLightOp = transform.Find("redlightop").gameObject;
		mYellowLightOp = transform.Find("yellowlightop").gameObject;
		
		mGreenLight.SetActive(false);
		mRedLight.SetActive(false);
		mYellowLight.SetActive(false);
		
		TrafficControl.RegisterSemaphore( this );
	}
	
	void OnDestroy()
	{
		TrafficControl.UnregisterSemaphore( this );
	}
	
	
	public void GoGreen()
	{
		StartCoroutine(goGreenSteps());
	}
	
	public void GoRed()
	{
		StartCoroutine(goRedSteps());
	}
	
	// Enable to debug semaphore state
	/*void OnDrawGizmos() 
	{
		if( mGreenLight == null )
			return; // not intialized yet
		
		// Draw a yellow sphere at the transform's position
		Gizmos.color = GetState() == EState.Red? Color.red : GetState() == EState.Yellow? Color.yellow : Color.green;
		Gizmos.DrawSphere (transform.position, 1);
	}*/
	
	public EState GetState()
	{
		return mGreenLight.activeSelf? EState.Green : mRedLight.activeSelf? EState.Red : EState.Yellow;
	}
	
	
	private IEnumerator goRedSteps()
	{
		mGreenLight.SetActive(false);
		mRedLightOp.SetActive(false);
		mYellowLight.SetActive(true);
		mYellowLightOp.SetActive(true);
		
		yield return new WaitForSeconds(SwitchTime);
		
		mYellowLight.SetActive(false);
		mYellowLightOp.SetActive(false);
		mRedLight.SetActive(true);
		mGreenLightOp.SetActive(true);
	}
	
	private IEnumerator goGreenSteps()
	{
		mRedLight.SetActive(false);
		mGreenLightOp.SetActive(false);
		mYellowLight.SetActive(true);
		mYellowLightOp.SetActive(true);
		
		yield return new WaitForSeconds(SwitchTime);
		
		mYellowLight.SetActive(false);
		mYellowLightOp.SetActive(false);
		mGreenLight.SetActive(true);
		mRedLightOp.SetActive(true);
	}
}
