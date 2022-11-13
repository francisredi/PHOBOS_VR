using UnityEngine;
using System.Collections;

public class MRIScanner : MonoBehaviour {

	private bool enter = false;
	private bool moveBody = false;
	private bool rotateStage1 = false;
	private bool rotateStage2 = false;
	private bool stage3 = false;
	private bool stage4 = false;
	private bool stage5 = false;
	private bool stage6 = false;
	private bool stage7 = false;
	private bool scanning = false;
	private Rewired.Player mInput;
	private GameObject PlayerCache;
	public GameObject MRIBed;
	public GameObject drShelly;
	public GameObject drHobbs;

	private Vector3 faceDir;
	private Quaternion targetRotation;
	private Quaternion originalRotation;
	private Vector3 targetPositionOnBed;
	private Vector3 targetPosition;
	private Vector3 originalPosition;
	private CapsuleCollider cc;
	private OVRPlayerController cm;
	//private UserControl ml;
	//private Animator an;

	public GameObject soundSrc1;
	public GameObject soundSrc2;
	public GameObject soundSrc3;
	public GameObject soundSrc4;

	public GameObject door1;
	public GameObject door2;

	public string Msg = "Please lie down and face up by pressing 'A'";
	private string prevMsg;
	private int desiredSound = 1;

	private AudioSource tempAudio;
	private AudioClip sound;

	private float wait;
	private Vector3 MRIstop = new Vector3(0.0f,68.0f,0.0f);

	//private float totalRotation = 0.0f;
	
	void Awake()
	{
		mInput = Rewired.ReInput.players.GetPlayer(0);
	}

	// Use this for initialization
	void Start () {
		prevMsg = Msg;
	}

	public void setDesiredSound(int num){
		desiredSound = num;
	}

    private Vector3 temp = new Vector3();
	
	// Update is called once per frame
	void Update () {

		/*if (!scanning) {
			if (mInput.GetButtonDown ("Up Arrow")) {
				desiredSound++;
				if (desiredSound > 11)
					desiredSound = 1;
			} else if (mInput.GetButtonDown ("Down Arrow")) {
				desiredSound--;
				if (desiredSound < 1)
					desiredSound = 11;
			}
		}*/

		if (enter) {
	
			if (mInput.GetButtonDown("A") && !moveBody /*&& !scanning*/) { // initiate lay on bed, disable player input

				if(door1.GetComponent<SingleDoor>().isOpen() || door2.GetComponent<SingleDoor>().isOpen()){
					Msg = "Please close all the doors in the room first!";
					return;
				}

				SceneGUI.Instance.removeRenderCallback( onDisplay );

                ///////originalRotation = PlayerCache.transform.rotation;
				///////originalRotation = Quaternion.Inverse(originalRotation);
				///////originalPosition = PlayerCache.transform.position;

				//Quaternion.l
				///////////////////////PlayerCache.transform.Rotate (Vector3.up * Time.deltaTime);

				//open = !open;
				//if(GetComponent<AudioSource>()) GetComponent<AudioSource>().Play();


				faceDir = MRIBed.transform.up * -1.0f;
				targetRotation = Quaternion.LookRotation (faceDir, Vector3.up);
				moveBody = true;

				PlayerCache.transform.GetComponent<Rigidbody>().useGravity = false;

				cc = PlayerCache.transform.GetComponent<CapsuleCollider>();
				cc.enabled = false;

				cm = PlayerCache.transform.GetComponent<OVRPlayerController>();
				cm.enabled = false;

				//ml = PlayerCache.transform.GetComponent<UserControl>();
				//ml.enabled = false;

				//an = PlayerCache.transform.GetComponent<Animator>();
				//an.enabled = false;

				rotateStage1 = true;
			}

			if (moveBody) { // move body to lay on bed

                if (rotateStage1)
                {
                    rotateStage1 = false;
                    targetPosition = new Vector3(0.0f, 68.3f, 0.0f); // 71.3

                    Fader.Instance.FadeOutAndIn(() =>
                    {
                        drShelly.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(new Vector3(-2.253f, 0.0f, 0.751f)); // original location when scene starts (in case do MRI again)
                        PlayerCache.transform.rotation = targetRotation;
                        targetRotation = Quaternion.LookRotation(Vector3.up, -Vector3.right);
                        targetPositionOnBed = new Vector3(-2.157f, 0.9f, -3.477f);
                        PlayerCache.transform.rotation = targetRotation;
                        PlayerCache.transform.position = targetPositionOnBed;
                        PlayerCache.transform.parent = MRIBed.transform; // attach
                    });

                    Msg = "Please keep still at all times\nfor a successful scan.";
                    SceneGUI.Instance.addRenderCallback(onDisplay);
                    stage4 = true;
                }
                /*if(rotateStage1){
					PlayerCache.transform.rotation = Quaternion.Slerp (PlayerCache.transform.rotation, targetRotation, Time.deltaTime * 2.0f);
					if(Quaternion.Angle(targetRotation, PlayerCache.transform.rotation) < 0.0001){
                        print("Rotate Stage 1");
						rotateStage1 = false;
						rotateStage2 = true;
						//targetRotation = Quaternion.LookRotation (Vector3.up, Vector3.up); // up up
						//Vector3 localRightAxis = Vector3.Cross();
						targetRotation = Quaternion.LookRotation(Vector3.up,-Vector3.right);
						targetPositionOnBed = new Vector3(-1.808f,0.852f,-3.477f);
					}
				}
				if(rotateStage2){
                    print("Rotate Stage 2");
                    PlayerCache.transform.rotation = Quaternion.Slerp(PlayerCache.transform.rotation,targetRotation,Time.deltaTime);
					PlayerCache.transform.position = Vector3.Slerp(PlayerCache.transform.position,targetPositionOnBed,Time.deltaTime);

					if(Vector3.Distance(PlayerCache.transform.position,targetPositionOnBed) < 0.01f){
						stage3 = true;
						rotateStage2 = false;
						targetPositionOnBed = new Vector3(-2.157f,0.852f,-3.477f);
					}
					//if(Mathf.Abs (totalRotation) < 90.0f){
					//	float currentAngle = PlayerCache.transform.rotation.eulerAngles.x; // x
					//	PlayerCache.transform.rotation = Quaternion.AngleAxis(currentAngle - (Time.deltaTime * 5), Vector3.right); // right
					//	totalRotation += Time.deltaTime * 5;
					//}
				}*/

                /*if (stage3){ // move body so that head is on head piece
                    print("Move body for head on head piece");
                    PlayerCache.transform.position = Vector3.Slerp(PlayerCache.transform.position,targetPositionOnBed,Time.deltaTime * 2.0f);
					if(Vector3.Distance(PlayerCache.transform.position,targetPositionOnBed) < 0.01f){
						stage3 = false;
						stage4 = true;
						targetPosition = new Vector3(0.0f,68.3f,0.0f); // 71.3
						PlayerCache.transform.parent = MRIBed.transform; // attach
						Msg = "Please keep still at all times while in the tube for a successful scan.";
						SceneGUI.Instance.addRenderCallback(onDisplay);
					}
				}*/

				if(stage4){ // move bed and body into tube
                    //print("Move bed into tube "+ MRIBed.transform.localPosition.ToString() + " " + targetPosition.ToString());
                    //MRIBed.transform.localPosition = Vector3.Slerp(MRIBed.transform.localPosition,targetPosition,Time.deltaTime * 0.2f);
                    temp.Set(0.0f, MRIBed.transform.localPosition.y + Time.deltaTime * 1.5f, 0.0f);
                    MRIBed.transform.localPosition = temp;
                    //MRIBed.transform.localPosition = Vector3.Lerp(MRIBed.transform.localPosition, targetPosition, Time.deltaTime * 0.2f);
                    if (Vector3.Distance(MRIBed.transform.localPosition,MRIstop) < 0.1f){ // fix here targetposition versus local
						SceneGUI.Instance.removeRenderCallback( onDisplay );
						stage4 = false;
						scanning = true;
						moveBody = false;

						/*switch(desiredSound){
						case 1:
							tempAudio = soundSrc1.GetComponent<AudioSource>();
							break;
						case 2:
							tempAudio = soundSrc2.GetComponent<AudioSource>();
							break;
						case 3:*/
							tempAudio = soundSrc3.GetComponent<AudioSource>();
							/*break;
						case 4:
							tempAudio = soundSrc4.GetComponent<AudioSource>();
							break;
						}*/

						sound = tempAudio.clip;
						wait = sound.length; //set wait to be clip's length
						
						tempAudio.Play();
					}
				}

				if(stage5){ // move bed and body out of the tube

                    drShelly.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(new Vector3(-1.751f, 0.0f, -1.64f));
                    drShelly.GetComponent<PixelCrushers.DialogueSystem.ConversationTrigger>().conversation = "Done";
                    drHobbs.GetComponent<PixelCrushers.DialogueSystem.ConversationTrigger>().conversation = "SeeYouOutside";

                    //MRIBed.transform.localPosition = Vector3.Slerp(MRIBed.transform.localPosition,targetPosition,Time.deltaTime * 0.1f);
                    temp.Set(0.0f, MRIBed.transform.localPosition.y - Time.deltaTime * 1.5f, 0.0f);
                    MRIBed.transform.localPosition = temp;
                    if (Vector3.Distance(MRIBed.transform.localPosition,targetPosition) < 0.1f){ // 0.5
						SceneGUI.Instance.removeRenderCallback( onDisplay );
						Msg = "You are free to go.";
                        //targetPositionOnBed = new Vector3(-1.808f, 0.852f, -3.477f);
                        
                        Fader.Instance.FadeOutAndIn(() =>
                        {
                            MRIBed.transform.localPosition = Vector3.zero; // reset perfect
                            PlayerCache.transform.parent = null; // detach from MRI bed
                            PlayerCache.transform.rotation = originalRotation;
                            PlayerCache.transform.position = originalPosition;
                            cm = PlayerCache.transform.GetComponent<OVRPlayerController>();
                            cm.enabled = true;

                            cc = PlayerCache.transform.GetComponent<CapsuleCollider>();
                            cc.enabled = true;

                            PlayerCache.transform.GetComponent<Rigidbody>().useGravity = true;
                        });

                        stage5 = false;
                        enter = false; // avoid accidental repeat
                    }
				}

				/*if(stage7){
					PlayerCache.transform.rotation = Quaternion.Slerp(PlayerCache.transform.rotation,originalRotation,Time.deltaTime);
					PlayerCache.transform.position = Vector3.Slerp(PlayerCache.transform.position,originalPosition,Time.deltaTime);
					if((Vector3.Distance(PlayerCache.transform.position,originalPosition) < 0.01f)&&(Quaternion.Angle(originalRotation, PlayerCache.transform.rotation) < 0.0001)){
						stage7 = false;
						enter = false; // avoid accidental repeat

						//an = PlayerCache.transform.GetComponent<Animator>();
						//an.enabled = true;

						//ml = PlayerCache.transform.GetComponent<UserControl>();
						//ml.enabled = true;

						cm = PlayerCache.transform.GetComponent<OVRPlayerController>();
						cm.enabled = true;

						cc = PlayerCache.transform.GetComponent<CapsuleCollider>();
						cc.enabled = true;

						PlayerCache.transform.GetComponent<Rigidbody>().useGravity = true;

						drShelly.GetComponent<NavMeshAgent>().SetDestination(new Vector3(-1.751f,0.0f,-1.64f));
						drShelly.GetComponent<PixelCrushers.DialogueSystem.ConversationTrigger>().conversation = "Done";
						drHobbs.GetComponent<PixelCrushers.DialogueSystem.ConversationTrigger>().conversation = "SeeYouOutside";
					}
				}*/
			}

			if(scanning){
				wait-=Time.deltaTime; //reverse count
				if(wait < 0.0f){
					scanning = false;
					Msg = "The scan finished. Stay put until you\nare completely out of the tube.";
					SceneGUI.Instance.addRenderCallback(onDisplay);
					moveBody = true;
					targetPosition = new Vector3(0.0f,0.0f,0.0f);
					stage5 = true;
				}
			}

		}


	}

	/*void OnGUI(){
		if(enter){
			if(!moveBody){
				GUI.Label(new Rect(Screen.width/2 - 75, Screen.height - 100, 250, 30), Msg);
			}
		}
	}*/

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			enter = true;
			PlayerCache = other.gameObject;
            originalRotation = PlayerCache.transform.rotation;
            originalPosition = PlayerCache.transform.position;
            SceneGUI.Instance.addRenderCallback( onDisplay );
		}
	}

	void OnTriggerExit (Collider other){
		if (other.gameObject.tag == "Player") {
			enter = false;
			moveBody = false;
			SceneGUI.Instance.removeRenderCallback( onDisplay );
			Msg = prevMsg;
		}
	}

	void onDisplay(bool isStereo)
	{
		SceneGUI.Instance.drawText(300, 300, 600, 60,ref  Msg, Color.white);
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
