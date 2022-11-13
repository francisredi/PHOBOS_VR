﻿using UnityEngine;

public class Webcam : MonoBehaviour
{
	public bool getDeviceByName = false;
	public int deviceIndex = 0;
	public string deviceName = "";
	public bool setRequestResolution = false;
	public int resolutionWidth = 800;
	public int resolutionHeight = 600;
	public bool setFPS = false;
	public int requestedFPS = 30;
	public bool mirrorHorizontal = false;
	[HideInInspector]
	public bool running = false;
	private WebCamTexture wcTexture;
	private bool foundDevice = false;
	void Start()
	{

		if(WebCamTexture.devices.Length > 0)
		{
			wcTexture = new WebCamTexture();
			if(setRequestResolution)
			{
				wcTexture.requestedWidth = resolutionWidth;
				wcTexture.requestedHeight = resolutionHeight;
			}
			if(setFPS)
			{
				wcTexture.requestedFPS = requestedFPS;
			}
			if(getDeviceByName)
			{
				foreach (WebCamDevice a in WebCamTexture.devices)
				{
					if(a.name == deviceName)
					{
						foundDevice = true;
					}
				}
				if(!foundDevice)
				{
					Debug.LogWarning("WebCam - Cannot find named device");
				}
				else
				{
					wcTexture.deviceName = deviceName;
				}
			}
			else
			{
				if(deviceIndex < WebCamTexture.devices.Length)
				{
					wcTexture.deviceName = WebCamTexture.devices[deviceIndex].name;
					deviceName = WebCamTexture.devices[deviceIndex].name;
					foundDevice = true;
				}
				else
				{
					Debug.LogWarning("WebCam - Index out of range. Webcams Detected: " + WebCamTexture.devices.Length.ToString());
				}
			}
			if(foundDevice)
			{
				GetComponent<Renderer>().material.mainTexture = wcTexture;
				wcTexture.Play();

				if(!wcTexture.isPlaying)
				{
					Webcam[] wcs = GameObject.FindObjectsOfType<Webcam>();
					foreach(Webcam a in wcs)
					{
						if(a.running)
						{
							if(a.deviceName == deviceName)
							{
								GetComponent<Renderer>().material.mainTexture = a.GetComponent<Renderer>().material.mainTexture;
							}
						}
					}
				}
				else
				{
					running = true;
				}
				if(mirrorHorizontal)
				{
					Vector2 tempScale = GetComponent<Renderer>().material.mainTextureScale;
					Vector2 tempOffset = GetComponent<Renderer>().material.mainTextureOffset;
					tempOffset.x += tempScale.x;
					tempScale.x *= -1;

					GetComponent<Renderer>().material.mainTextureScale = tempScale;
					GetComponent<Renderer>().material.mainTextureOffset = tempOffset;
				}
			}


		}
		else
		{
			Debug.LogWarning("WebCam - No Webcam Device Detected");
		}
	}
	void OnDestroy()
	{
		if(running)
		{
			wcTexture.Stop();
		}
	}
}