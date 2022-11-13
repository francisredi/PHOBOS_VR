using UnityEngine;
using System.Collections;

public class VolumeControl : MonoBehaviour {
	
	public AudioSource[] OutdoorSources;
	public AudioSource IndoorSource;
	
	private float[] mOriginalOutdoorVolumes;
	private float mOriginalIndoorVolume;
	
	void Start () {
	
		mOriginalOutdoorVolumes = new float[OutdoorSources.Length];
		for(int i = 0; i < OutdoorSources.Length; i++)
			mOriginalOutdoorVolumes[i] = OutdoorSources[i].volume;
		mOriginalIndoorVolume = IndoorSource.volume;
		IndoorSource.volume = 0.01f;
	}

	
	void OnTriggerEnter(Collider other)
	{
		if( other.tag != "Player" )
			return;
				
		StartCoroutine(IndoorVolumes());
	}
	
	void OnTriggerExit(Collider other)
	{
		if( other.tag != "Player" )
			return;
		
		StartCoroutine(OutdoorVolumes());
	}
	
	IEnumerator IndoorVolumes()
	{
		const int steps = 15;
		float[] deltas = new float[mOriginalOutdoorVolumes.Length];
		for(int i = 0; i < deltas.Length; i++)
			deltas[i] = mOriginalOutdoorVolumes[i] / steps;
		
		for(int i = 0; i < steps; i++)
		{
			int j = 0;
			foreach(AudioSource source in OutdoorSources)
				source.volume -= deltas[j++];
			
			yield return new WaitForSeconds(0.05f);
		}
		
		IndoorSource.volume = mOriginalIndoorVolume;
	}
	
	IEnumerator OutdoorVolumes()
	{
		const int steps = 15;
		float[] deltas = new float[mOriginalOutdoorVolumes.Length];
		for(int i = 0; i < deltas.Length; i++)
			deltas[i] = mOriginalOutdoorVolumes[i] / steps;
		
		for(int i = 0; i < steps; i++)
		{
			int j = 0;
			foreach(AudioSource source in OutdoorSources)
				source.volume += deltas[j++];
			
			yield return new WaitForSeconds(0.05f);
		}
		
		IndoorSource.volume = 0.01f;
	}
}
