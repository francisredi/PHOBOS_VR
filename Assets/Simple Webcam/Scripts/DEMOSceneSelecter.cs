using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DEMOSceneSelecter : MonoBehaviour {

	void OnGUI()
	{
		if(GUI.Button(new Rect(10,10,200,60),"Scene 1"))
		{
            SceneManager.LoadScene("DemoSceneWebPlayer 2");
        }
		if(GUI.Button(new Rect(10,70,200,60),"Scene 2"))
		{
            SceneManager.LoadScene("DemoSceneWebPlayer");
        }
	}
}
