using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TrafficControl : MonoBehaviour {
	
	public float GreenLightTime = 10f;
	public float StreetDelay = 3f;
	
	private static int sHorizontalStreetCount = 2;
	private static int sVerticalStreetCount = 5;
	
	
	private static List<Semaphore> [,]sSemaphores;
	
	public static void RegisterSemaphore(Semaphore semaphore)
	{
		if (sSemaphores == null)
		{
			sSemaphores = new List<Semaphore>[sHorizontalStreetCount,sVerticalStreetCount];
			for(int i = 0; i < sHorizontalStreetCount; i++)
				for(int j = 0; j < sVerticalStreetCount; j++)
					sSemaphores[i,j] = new List<Semaphore>();
		}
		
		sSemaphores[ semaphore.HorizontalStreet, semaphore.VerticalStreet ].Add( semaphore );
	}
	
	public static void UnregisterSemaphore(Semaphore semaphore)
	{
		sSemaphores[ semaphore.HorizontalStreet, semaphore.VerticalStreet ].Remove( semaphore );
	}
	
	
	

	void Start () {
	
		for(int i = 0; i < sHorizontalStreetCount; i++)
			for(int j = 0; j < sVerticalStreetCount; j++)
				foreach(Semaphore sem in sSemaphores[i,j])
				{
					float aux = sem.SwitchTime;
					sem.SwitchTime = 0f; // little hack so initialization doesn't use any delay.
					if(sem.Orientation == "horizontal")
						sem.GoRed();
					else
						sem.GoGreen();
					sem.SwitchTime = aux;
				}
		
		StartCoroutine(controlSemaphores());		
	}
	
	
	private IEnumerator controlSemaphores()
	{
		List<Semaphore> []street;
		for(int i = 0; i < sHorizontalStreetCount; i++)
		{
			street = new List<Semaphore>[sVerticalStreetCount];
			for(int j = 0; j < sVerticalStreetCount; j++)
				street[j] = sSemaphores[i, j];
			
			//Debug.Log("enabling h street: " + i);
			StartCoroutine(goGreenStreet(street, "horizontal"));
			
			yield return new WaitForSeconds(StreetDelay);
		}
		
		
		yield return new WaitForSeconds(GreenLightTime);
		
		
		for(int i = 0; i < sVerticalStreetCount; i++)
		{
			street = new List<Semaphore>[sHorizontalStreetCount];
			for(int j = 0; j < sHorizontalStreetCount; j++)
				street[j] = sSemaphores[j, i];
			
			//Debug.Log("enabling v street: " + i);
			StartCoroutine(goGreenStreet(street, "vertical"));
			
			yield return new WaitForSeconds(StreetDelay);
		}
		
		
		yield return new WaitForSeconds(GreenLightTime);
		
		
		StartCoroutine( controlSemaphores() );
	}
	
	private IEnumerator goGreenStreet(List<Semaphore> []street, string direction)
	{
		foreach(List<Semaphore> corner in street)
		{	
			//Debug.Log("Enabling: " + corner[0].HorizontalStreet + " " + corner[0].VerticalStreet );
			
			foreach(Semaphore sem in corner)
				if(sem.Orientation == direction)
					sem.GoGreen();
				else
					sem.GoRed();
			
			yield return new WaitForSeconds(StreetDelay);
		}
	}
	
}
