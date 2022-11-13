using UnityEngine;
using System.Collections;

public class ClipAvoidance : MonoBehaviour
{

    public CharacterController cc;
    public GameObject irCameraPosition;
    private MeshRenderer mesh;

    //private float oldMagnitude;
    //private float newMagnitude;

    private string Msg = "";

    // Use this for initialization
    void Start () {
        mesh = irCameraPosition.transform.GetComponent<MeshRenderer>();
    }

    int doOne = 0;
	
    // Update is called once per frame
    void Update()
    {
        if(transform.localPosition.z > 0.2f){
            doOne++;
            Msg = "Turn your body towards the\nwhite circle, then move backwards\nuntil this message disappears"; // ask user to move farther back from the positional tracking camera
        }
        else if (transform.localPosition.x < -0.2f){
            doOne++;
            Msg = "Turn your body towards the\nwhite circle, then shift right\nuntil this message disappears";
        }
        else if (transform.localPosition.x > 0.2f){
            doOne++;
            Msg = "Turn your body towards the\nwhite circle, then shift left\nuntil this message disappears";
        }
        else{
            doOne = 0;
            mesh.enabled = false;
            SceneGUI.Instance.removeRenderCallback(onDisplay);
            Msg = "";
        }

        if (doOne == 1)
        {
            mesh.enabled = true;
            SceneGUI.Instance.addRenderCallback(onDisplay);
        }

            /*if (Msg == ""){
                SceneGUI.Instance.removeRenderCallback(onDisplay);
            }
            else{
                SceneGUI.Instance.addRenderCallback(onDisplay);
            }*/
            //////SceneGUI.Instance.drawText(300, 300, 600, 60, ref Msg, Color.white);
    }

    private int objEnter = 0;

    void OnTriggerEnter(Collider other) {

        if (other.isTrigger) return;
        if (other.transform.tag == "Player") return;

            //oldMagnitude = Vector3.Magnitude(transform.localPosition - cc.transform.localPosition);
            //print("Enter person");

            //cc.radius = 0.5f;

            //if (other.GetType() == typeof(CapsuleCollider))
            //if(other.transform.tag == "Pedestrian")
            //{ // it is a person or other moving object

            /*objEnter++;

            if (objEnter != 1) return;

            Fader.Instance.FadeOut(() =>
            {
                //SceneManager.LoadScene(TargetScene, LoadSceneMode.Single);
                // start a coroutine that will load the desired scene.
                //StartCoroutine(LoadNewScene());
            });*/
        //}

    }

    void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return;
        if (other.transform.tag == "Player") return;

        //cc.radius += 0.001f;
        //if (cc.radius > 0.5f) cc.radius = 0.5f;

        //newMagnitude = Vector3.Magnitude(transform.localPosition - cc.transform.localPosition);

        //if (other.GetType() == typeof(CapsuleCollider)) { // it is a person or other moving object

        /*Fader.Instance.FadeOut(() =>
        {
            //SceneManager.LoadScene(TargetScene, LoadSceneMode.Single);
            // start a coroutine that will load the desired scene.
            //StartCoroutine(LoadNewScene());
        });*/

        // expand the radius of character controller to avoid camera clipping with surroundings
        //cc.radius += 0.001f;
        //if (cc.radius > 0.5f) cc.radius = 0.5f;//
        //cc.radius = 0.5f;

        //cc.transform.position = Vector3.Slerp(cc.transform.position, transform.position, Time.deltaTime);
        //transform.position = Vector3.Slerp(transform.position, cc.transform.position, Time.deltaTime);

        /*if (newMagnitude > oldMagnitude)
        {
            // expand the radius of character controller to avoid camera clipping with surroundings
            cc.radius += 0.001f;
            if (cc.radius > 0.5f) cc.radius = 0.5f;
        }
        else if (newMagnitude < oldMagnitude)
        {
            // shrink character controller to minimum
            cc.radius -= 0.001f;
            if (cc.radius < 0.3f) cc.radius = 0.3f;
        }*/
        //}

        //oldMagnitude = newMagnitude;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;
        if (other.transform.tag == "Player") return;

        //cc.radius -= 0.001f;
        //if (cc.radius < 0.3f) cc.radius = 0.3f;
        //cc.radius = 0.3f;



        //cc.radius = 0.3f;
        //if (other.GetType() == typeof(CapsuleCollider)) {
        /*objEnter--;

            if (objEnter != 0) return;
            
            Fader.Instance.FadeIn(() =>
            {
                //SceneManager.LoadScene(TargetScene, LoadSceneMode.Single);
                // start a coroutine that will load the desired scene.
                //StartCoroutine(LoadNewScene());
            });*/
        //}
    }

    void onDisplay(bool isStereo)
    {

        SceneGUI.Instance.drawText(300, 300, 600, 60, ref Msg, Color.white);
        
        /*if(isStereo){
			SceneGUI.Instance.drawText(300, 300, 600, 60,ref  Msg, Color.white);
		}
		else{
			GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.Box(Msg);
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndArea();
		}*/
    }

}