using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof (MecanimCharacter))]
public class UserControl : MonoBehaviour
{
    private MecanimCharacter m_Character;	  // A reference to the MecanimCharacter on the object
    private Transform m_Cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward;             // The current forward direction of the camera
    private Vector3 m_Move;
    private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

	private Rewired.Player input;

    // Use this for initialization
    void Awake()
	{
		input = Rewired.ReInput.players.GetPlayer(0);
	}
    
    private void Start()
    {
		m_Cam = GameObject.FindGameObjectWithTag("MainCamera").transform; // works for VR and non-VR
        m_Character = GetComponent<MecanimCharacter>();
    }


    private void Update()
    {
        if (!m_Jump)
        {
            //m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
			m_Jump = input.GetButtonDown("Jump");
        }
    }


    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        // read inputs
		float h = input.GetAxis ("Horizontal");///16.0f;
		float v = input.GetAxis ("Vertical")/2.0f;
		bool crouch = input.GetButton ("Crouch");

        // calculate move direction to pass to character
        if (m_Cam != null) // relative to camera
        {
            // calculate camera relative direction to move:
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v*m_CamForward + h*m_Cam.right;

			if(Vector3.Angle(m_CamForward,m_Move) > 120){ // if backwards do nothing
				m_Move = new Vector3(0f,0f,0f); // don't move at all
			}
        }
        else // absolute world direction
        {
            // we use world-relative directions in the case of no main camera
            m_Move = v*Vector3.forward + h*Vector3.right;
        }

        // pass all parameters to the character control script
        m_Character.Move(m_Move, crouch, m_Jump);
		m_Jump = false;
    }
}