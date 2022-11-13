using UnityEngine;
using System.Collections;

public class FloorOpenClose : MonoBehaviour {

	public AudioClip ledgeSound;
    private Rewired.Player mInput;

    public float Accel = 0.012f;
    private bool mMoving;
    private float mSpd;
    
    public float mPos1 = 43.0f;
    public float mPos2 = 0.0f;

    public string Action = "X";

    public bool opened = false;

    void Start()
    {
        mMoving = false;
        mSpd = 0;
        mInput = Rewired.ReInput.players.GetPlayer(0);
    }

    void FixedUpdate()
    {
        if (mInput.GetButton(Action) && !opened)
        {
            if (!mMoving)
            { // first movement
                //SceneGUI.Instance.removeRenderCallback(DrawInstructions);
                mMoving = true;
                GetComponent<AudioSource>().Play();
                mSpd -= Accel;
                opened = true;
            }
        }
        else if (mInput.GetButton(Action) && opened)
        {
            if (!mMoving)
            { // first movement
                //SceneGUI.Instance.removeRenderCallback(DrawInstructions);
                mMoving = true;
                GetComponent<AudioSource>().Play();
                mSpd += Accel;
                opened = false;
            }
        }

        float deAccel = 1f; // used when we're reaching top or bottom
        float distance = 0.0f;
        if (mSpd > 0)
        {
            distance = mPos2 - transform.position.x;
            deAccel = Mathf.Clamp(distance, 0f, 1f);
        }
        else if (mSpd < 0)
        {
            distance = transform.position.x - mPos1;
            deAccel = Mathf.Clamp(distance, 0f, 1f);
        }
        Vector3 spd = new Vector3(mSpd,0,0);
        transform.Translate(spd * deAccel);

        if (mMoving)
        {
            if (mSpd < 0f && distance < 0.05f)
            {
                mSpd = 0;
                mMoving = false;
                GetComponent<AudioSource>().Stop();
            }
            else if (mSpd > 0f && distance < 0.05f)
            {
                mSpd = 0;
                mMoving = false;
                GetComponent<AudioSource>().Stop();
            }
        }

    }
}