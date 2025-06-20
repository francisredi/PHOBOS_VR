﻿/* djfunkey did an excelent job in converting the script to C#. Thanks to him even more people can take advantage of the microphone in game. http://forum.unity3d.com/members/118660-djfunkey
The Editor Script for the C# version is below.
To talk in game using either of the methods press "T"*
C# features:
1: A menu so that the user can pick the microphone they wish to use (handy if hey have a web-cam microphone, and a headset microphone)
2: The user can now adjust the volume of their microphone, it will also affect the loudness variable proportionally.
3: The user can choose from 3 settings: PushtToTalk, HoldToTalk, ConstantTalk. These determine what the user has to do for the microphone to be activated.
4: If there is only 1 device detected it will make that device default.
5: Call microphone data from any script with MicrophoneInput.loudness.
6: The microphone RAM will still be flushed even if Time.timeScale = 0;
7: You can set how often the michrophone RAM is flushed.*/

using UnityEngine;
using System.Collections;
 
[RequireComponent(typeof(AudioSource))]
public class MicControlC : MonoBehaviour {
 
	public enum micActivation {
		HoldToSpeak,
		PushToSpeak,
		ConstantSpeak
	}

    new AudioSource audio;
 
	public float sensitivity = 100;
	public float ramFlushSpeed = 5;//The smaller the number the faster it flush's the ram, but there might be performance issues...
	[Range(0,100)]
	public float sourceVolume = 100;//Between 0 and 100
	public bool GuiSelectDevice = true;
	public micActivation micControl;
	//
	public string selectedDevice { get; private set; }	
	public float loudness { get; private set; } //dont touch
	//
	private bool micSelected = false;
	private float ramFlushTimer;
	private int amountSamples = 256; //increase to get better average, but will decrease performance. Best to leave it
	private int minFreq, maxFreq; 
 
    void Start() {
        //audio = new AudioSource();
        audio = GetComponent<AudioSource>();
		audio.loop = true; // Set the AudioClip to loop
		audio.mute = false; // Mute the sound, we don't want the player to hear it
		selectedDevice = Microphone.devices[0].ToString();
		micSelected = true;
		GetMicCaps();
    }
 
	void OnGUI() {
		MicDeviceGUI((Screen.width/2)-150, (Screen.height/2)-75, 300, 100, 10, -300);
		if (Microphone.IsRecording(selectedDevice)) {
			ramFlushTimer += Time.fixedDeltaTime;
			RamFlush();
		}
	}
 
	public void MicDeviceGUI (float left, float top, float width, float height, float buttonSpaceTop, float buttonSpaceLeft) {
		if (Microphone.devices.Length > 1 && GuiSelectDevice == true || micSelected == false)//If there is more than one device, choose one.
			for (int i = 0; i < Microphone.devices.Length; ++i)
				if (GUI.Button(new Rect(left + ((width + buttonSpaceLeft) * i), top + ((height + buttonSpaceTop) * i), width, height), Microphone.devices[i].ToString())) {
					StopMicrophone();
					selectedDevice = Microphone.devices[i].ToString();
					GetMicCaps();
					StartMicrophone();
					micSelected = true;
				}
		if (Microphone.devices.Length < 2 && micSelected == false) {//If there is only 1 decive make it default
			selectedDevice = Microphone.devices[0].ToString();
			GetMicCaps();
			micSelected = true;
		}
	}
 
	public void GetMicCaps () {
		Microphone.GetDeviceCaps(selectedDevice, out minFreq, out maxFreq);//Gets the frequency of the device
		if ((minFreq + maxFreq) == 0)//These 2 lines of code are mainly for windows computers
			maxFreq = 44100;
	}
 
	public void StartMicrophone () {
		audio.clip = Microphone.Start(selectedDevice, true, 10, maxFreq);//Starts recording
		while (!(Microphone.GetPosition(selectedDevice) > 0)){} // Wait until the recording has started
		audio.Play(); // Play the audio source!
	}
 
	public void StopMicrophone () {
		audio.Stop();//Stops the audio
		Microphone.End(selectedDevice);//Stops the recording of the device	
	}		
 
    void Update() {
		audio.volume = (sourceVolume/100);
		loudness = GetAveragedVolume() * sensitivity * (sourceVolume/10);
		//Hold To Speak!!
		if (micControl == micActivation.HoldToSpeak) {
			if (Microphone.IsRecording(selectedDevice) && Input.GetKey(KeyCode.T) == false)
				StopMicrophone();
			//
			if (Input.GetKeyDown(KeyCode.T)) //Push to talk
				StartMicrophone();
			//
			if (Input.GetKeyUp(KeyCode.T))
				StopMicrophone();
			//
		}
		//Push To Talk!!
		if (micControl == micActivation.PushToSpeak) {
			if (Input.GetKeyDown(KeyCode.T)) {
				if (Microphone.IsRecording(selectedDevice)) 
					StopMicrophone();
 
				else if (!Microphone.IsRecording(selectedDevice)) 
					StartMicrophone();
			}
			//
		}
		//Constant Speak!!
		if (micControl == micActivation.ConstantSpeak)
			if (!Microphone.IsRecording(selectedDevice)) 
				StartMicrophone();
		//
		if (Input.GetKeyDown(KeyCode.G))
			micSelected = false; 
    }
 
	private void RamFlush () {
		if (ramFlushTimer >= ramFlushSpeed && Microphone.IsRecording(selectedDevice)) {
			StopMicrophone();
			StartMicrophone();
			ramFlushTimer = 0;
		}
	}
 
	float GetAveragedVolume() {
        float[] data = new float[amountSamples];
        float a = 0;
        audio.GetOutputData(data,0);
        foreach(float s in data) {
            a += Mathf.Abs(s);
        }
        return a/amountSamples;
    }
}