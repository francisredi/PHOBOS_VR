using System;
using UnityEngine;

public class Utils
{
	public static GameObject GetNearest(GameObject[] objs, GameObject rf)
	{
		GameObject closest = null;
		float minDist = 0f;
		foreach(GameObject go in objs)
		{
			float dist = (go.transform.position - rf.transform.position).magnitude;
			if(closest == null || dist < minDist)
			{
				minDist = dist;
				closest = go;
			}
		}
		
		return closest;
	}
	
	
	public static float GetDist(GameObject a, GameObject b)
	{
		return (a.transform.position - b.transform.position).magnitude;
	}
	
}

