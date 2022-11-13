using UnityEngine;
using System.Collections;

public class PlayAnimWhenCloseAndKey : MonoBehaviour {
	
	private GameObject Ref;
	public float MinDist = 3f;
	public string Anim;
	//public string Msg1;
	//public string Msg2;
	
	private bool mToggle = false;
	private Rewired.Player mInput;
	bool display;

	void Awake()
	{
		mInput = Rewired.ReInput.players.GetPlayer(0);
        Ref = GameObject.FindGameObjectWithTag("Player");
	}
	
	/*void Update () {
		
		if( Utils.GetDist(gameObject, Ref) < MinDist)
		{
			if(!display)
			{
				display = true;
				SceneGUI.Instance.addRenderCallback( onDisplay );
			}
		}
		else if( display)
		{
			display = false;
			SceneGUI.Instance.removeRenderCallback( onDisplay );
		}
	}
	
	void onDisplay(bool isStereo)
	{
			if( mToggle)
				SceneGUI.Instance.drawText(300, 300, 600, 60,ref  Msg1, Color.white);
			else
				SceneGUI.Instance.drawText(300, 300, 600, 60,ref  Msg2, Color.white);
	}*/

    public void OnPointerClick()
    {
        if (Utils.GetDist(gameObject, Ref) < MinDist)
        {
            GetComponent<Animation>()[Anim].speed = mToggle ? -1 : 1;
            GetComponent<Animation>()[Anim].time = mToggle ? GetComponent<Animation>()[Anim].length : 0;
            mToggle = !mToggle;
            GetComponent<Animation>().Play(Anim);

            if (GetComponent<AudioSource>())
                GetComponent<AudioSource>().Play();
        }
    }


}
