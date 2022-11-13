using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelWarp : MonoBehaviour {
	
	public float Dist;
	private GameObject Ref;
	public string TargetScene;
	public string Target;
	
	private bool mWarping = false;

    void Awake()
    {
        Ref = GameObject.FindGameObjectWithTag("Player");
    }

    // The coroutine runs on its own at the same time as Update() and takes an integer indicating which scene to load.
    IEnumerator LoadNewScene()
    {

        // This line waits for 3 seconds before executing the next line in the coroutine.
        // This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.
        //yield return new WaitForSeconds(3);

        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        //AsyncOperation async = Application.LoadLevelAsync(scene);
        AsyncOperation async = SceneManager.LoadSceneAsync(TargetScene, LoadSceneMode.Single);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            yield return null;
        }

    }

    void Update () {

		if(Ref == null) return;
		
		if( !mWarping && Utils.GetDist(gameObject, Ref) < Dist)
		{

			GameObject obj = new GameObject("WarpFromPreviousScene");
			obj.tag = "LevelWarp";
			LevelWarp lwp = obj.AddComponent<LevelWarp>();
			lwp.Target = Target;
			DontDestroyOnLoad(obj);

            Fader.Instance.FadeOut(() =>
            {
                //SceneManager.LoadScene(TargetScene, LoadSceneMode.Single);
                // start a coroutine that will load the desired scene.
                StartCoroutine(LoadNewScene());
            });

            //Fade.OnFadeEnd = delegate() { SceneManager.LoadScene(TargetScene, LoadSceneMode.Single); };
			//Fade.FadeOut();
            	
			mWarping = true;
		}
	}
	
	
	void OnGUI()
	{
		if( mWarping == false )
			return;
		
		GUI.depth = -1; // draw over the fade layer
		
	    GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
	    GUILayout.FlexibleSpace();
	    GUILayout.BeginHorizontal();
	    GUILayout.FlexibleSpace();
		
		GUILayout.Label("LOADING...");
		
	    GUILayout.FlexibleSpace();
	    GUILayout.EndHorizontal();
	    GUILayout.FlexibleSpace();
	    GUILayout.EndArea();
	}
}
