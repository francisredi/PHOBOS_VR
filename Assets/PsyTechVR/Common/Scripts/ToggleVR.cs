using UnityEngine;
using UnityEngine.VR;

public class ToggleVR : MonoBehaviour
{
    //public GameObject OculusPlayer;

    private GameObject player;

    void Awake()
    {
        print(VRSettings.loadedDeviceName);

        /*player = GameObject.FindGameObjectWithTag("Player");

        if (string.CompareOrdinal(VRSettings.loadedDeviceName,"Oculus") == 0)
        {
            //OculusPlayer.SetActive(true);
            OVRPlayerController ovrPlayerController = player.AddComponent<OVRPlayerController>();
            ovrPlayerController.Acceleration = 0.0f;
            ovrPlayerController.JumpForce = 0.0f;
            ovrPlayerController.GravityModifier = 0.0f;
        }

        if (string.CompareOrdinal(VRSettings.loadedDeviceName, "OpenVR") == 0)
        {
            //
        }*/


    }

    void Update()
    {
        //If V is pressed, toggle VRSettings.enabled
        if (Input.GetKeyDown(KeyCode.V))
        {
            VRSettings.enabled = !VRSettings.enabled;
            Debug.Log("Changed VRSettings.enabled to:" + VRSettings.enabled);
        }
    }
}