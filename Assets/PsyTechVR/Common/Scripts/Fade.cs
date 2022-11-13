using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour {
	
	public Color FadeColor;
	public float Length;
	
	public delegate void DVoidVoid();
	static public DVoidVoid OnFadeEnd;
	static public float Bright = 1f; // well, is not good that this class handles the screen bright, but
									 // this is the easiest place to add it. Should create a separate class.
	
	private bool mActive;
	private float mElapsed;
	private float mStartAlpha;
	private float mEndAlpha;
	public static Fade Instance; // kind of singleton, should do it right... later.
	private Texture2D mWhiteTex;
	private Texture2D mBlackTex;
	private Rect mFullScreen;
	
	public void OnEnable()
	{	
		Instance = this;
		mWhiteTex = new Texture2D(1,1, TextureFormat.ARGB32, false);
		mBlackTex = new Texture2D(1,1, TextureFormat.ARGB32, false);
		
		mFullScreen = new Rect(0,0, Screen.width, Screen.height);
	}
	
	
	public static void FadeOut()
	{	
		Instance.mActive = true;
		Instance.mStartAlpha = 0;
		Instance.mEndAlpha = 1;
		Instance.mElapsed = 0;
	}
	
	
	public static void FadeIn()
	{
		Instance.mActive = true;
		Instance.mStartAlpha = 1;
		Instance.mEndAlpha = 0;
		Instance.mElapsed = 0;
	}
	
	
	void OnGUI()
	{	
		if(!mActive && mEndAlpha == 0) // only for fade ins
		{
			ControlScreenBright();
			return;
		}
		
		mElapsed += Time.deltaTime;
		
		// Kind of hack here, we want to do this stuff only once
		// so we check for OnFadeEnd assuming that always have a callback.
		// We want to keep doing the fade GUI drawing because sometimes the camera
		// still draws some stuff, so doing this we assure that the fade will continue
		// as the gameobject is destroyed on level load and we always use fade on level load
		// its safe to do it.
		if(mElapsed >= Length && OnFadeEnd != null)
		{
			if(mEndAlpha == 1) // fade out
				foreach(Camera cam in Camera.allCameras)
				{
					cam.cullingMask = 0;
					cam.backgroundColor = FadeColor;
				}
			

			OnFadeEnd();
			OnFadeEnd = null;
		}
		
		FadeColor.a = Mathf.Lerp(mStartAlpha, mEndAlpha, mElapsed / Length);
		
		
		GUI.depth = 0; // want the fade to be rendered always on top
		
		// i would use GUI.color, but we're drawing two textures on this same call
		mWhiteTex.SetPixel(0,0, FadeColor);
		mWhiteTex.Apply();
		GUI.DrawTexture(mFullScreen, mWhiteTex, ScaleMode.StretchToFill, true);
		
		ControlScreenBright();
	}
	
	
	void ControlScreenBright ()
	{
		// just drawing a black texture to lower monitor luminance.
		// we're going to use a big screen and maybe the bright could bother people's eyes.
		
		
		mBlackTex.SetPixel(0,0, new Color(0,0,0, 1f - Bright));
		mBlackTex.Apply();
		GUI.DrawTexture(mFullScreen, mBlackTex, ScaleMode.StretchToFill, true);
	}
}
