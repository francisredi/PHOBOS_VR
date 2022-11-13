using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ApplicationControls : MonoBehaviour {

	private Rewired.Player input;

	// Use this for initialization
	void Awake () {
		input = Rewired.ReInput.players.GetPlayer(0);
	}
	
	// Update is called once per frame
	void Update () {
		if(input.GetButtonDown("Recenter")){ // center tracking on current pose
			UnityEngine.VR.InputTracking.Recenter ();
		}
		if(input.GetButtonDown("Back")){
            SceneManager.LoadScene("HomeScene", LoadSceneMode.Single); // load without delay
            /*#if UNITY_EDITOR
			//Debug.Break(); // pause the game
			UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play");
			#elif UNITY_WEBPLAYER
			string webplayerQuitURL = "http://mind.kaist.ac.kr/Francis";
			Application.OpenURL(webplayerQuitURL);
			#elif UNITY_STANDALONE_WIN
			Application.Quit();
			#endif*/
        }
	}
}
