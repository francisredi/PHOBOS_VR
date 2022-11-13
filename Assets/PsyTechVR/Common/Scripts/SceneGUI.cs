using UnityEngine;
using System.Collections;

public class SceneGUI : MonoBehaviour {

	// new code
	public UnityEngine.UI.Text useMessageText = null;
	public UnityEngine.Canvas canvas = null;
	
	//private bool mLoading;
	Rewired.Player mInput;

	public static SceneGUI Instance = null;
	
	// Create a delegate for update functions
	public delegate void guiFunctions(bool isStereo);
	private guiFunctions GuiFunctions;

	private bool displayMessage = false;

	void OnEnable(){
		Instance = this;
	}

	void Awake() {
		useMessageText.text = ""; // reset to nothing
		
		//mLoading = true;		
		//addRenderCallback(drawLoading); // Loading...
		addRenderCallback(showText); // show the instructions
        StartCoroutine(RemoveInstructions());

        mInput = Rewired.ReInput.players.GetPlayer(0);
	}

	/*void Start () {
		Fade.OnFadeEnd = delegate() { StartCoroutine(SetLoading(false)); }; // to avoid OnGUI issues.
		Fade.FadeIn();
	}*/

	public void addRenderCallback( guiFunctions callback){
		GuiFunctions += callback;
        //displayMessage = true;
        //canvas.enabled = true;
    }

	public void removeRenderCallback( guiFunctions callback){
		GuiFunctions -= callback;
        //displayMessage = false;
        //canvas.enabled = false;
    }

	/*IEnumerator SetLoading(bool value){
		yield return new WaitForEndOfFrame();
		mLoading = value;
		removeRenderCallback(drawLoading);		
		StartCoroutine( RemoveInstructions());
	}*/
	
	IEnumerator RemoveInstructions(){
		yield return new WaitForSeconds(5);
		removeRenderCallback( showText );
	}

	void OnGUI(){
		if( GuiFunctions != null && GuiFunctions.GetInvocationList().Length > 0)
		{
			// WE ONLY DISPLAY A SINGLE METHOD AT THE SAME TIME BECASE WE'VE ALL OUR STUFF CENTERED ON THE SAME SPOT!
			guiFunctions first = GuiFunctions.GetInvocationList()[0] as guiFunctions;
			first( false );
		}

		if (canvas.enabled == false && displayMessage == true) { // enable message
			canvas.enabled = true;
		}
		else if (canvas.enabled == true && displayMessage == false){ // disable message
			canvas.enabled = false;
		}
		displayMessage = false; // reset
	}

	/*void drawLoading(bool isStereo){
		useMessageText.text = "LOADING...";
		//displayMessage = true;
	}*/

	void showText(bool isStereo){
        string Msg = "Press 'BACK' anytime to exit this scene";
        drawText(300, 300, 600, 60, ref Msg, Color.white);
    }

	public void drawText(int X, int Y, int wX, int hY, ref string text, Color color){
		useMessageText.text = text;
		displayMessage = true;
        useMessageText.color = color;
	}
}
