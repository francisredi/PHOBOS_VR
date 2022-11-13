using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

    private Rewired.Player input;

    public GameObject positionCam;

    //public string activeFearSelection = "";

    public GameObject toolTip;
    private Text guitext;
    private string prevText;
    private string temp;

    private Dictionary<string,string> fearScenesMapping;
    public GameObject[] sceneCanvases;
    private int sceneCanvasesSize;

    // Use this for initialization
    void Start()
    {
        input = Rewired.ReInput.players.GetPlayer(0);
        guitext = toolTip.GetComponent<Text>();
        prevText = "";

        sceneCanvasesSize = sceneCanvases.Length;
        //StartCoroutine(Wait());

        fearScenesMapping = new Dictionary<string,string>(); // fear name, active scene list separated by commas
        fearScenesMapping.Add("Baseline", "0");
        fearScenesMapping.Add("Elevator Phobia", "2");
        fearScenesMapping.Add("Chiroptophobia", ""); // bats
        fearScenesMapping.Add("Ornithophobia", "2,8"); // birds
        fearScenesMapping.Add("Ailurophobia", ""); // cats
        fearScenesMapping.Add("Bovinophobia", ""); // cows
        fearScenesMapping.Add("Cynophobia", "3,4,7"); // dogs
        fearScenesMapping.Add("Ranidaphobia", ""); // frogs
        fearScenesMapping.Add("Musophobia", "7"); // mouse
        fearScenesMapping.Add("Swinophobia", ""); // pigs
        fearScenesMapping.Add("Ophidiophobia", ""); // snakes
        fearScenesMapping.Add("Arachnophobia", "2,4,7"); // spiders
        fearScenesMapping.Add("Myrmecophobia", ""); // ants
        fearScenesMapping.Add("Apiphobia", ""); // bees
        fearScenesMapping.Add("Pteronarcophobia", ""); // flies
        fearScenesMapping.Add("Anopheliphobia", ""); // mosquitos
        fearScenesMapping.Add("Scoleciphobia", ""); // worms
        fearScenesMapping.Add("Gerascophobia", ""); // getting old
        fearScenesMapping.Add("Acrophobia", "2,6,8"); // heights
        fearScenesMapping.Add("Astraphobia", ""); // thunder and lightning
        fearScenesMapping.Add("Aquaphobia", "2,4,8"); // water
        fearScenesMapping.Add("Agoraphobia", "0,2,8"); // open spaces
        fearScenesMapping.Add("Gephyrophobia", "8"); // bridges
        fearScenesMapping.Add("Nyctophobia", "3"); // darkness
        fearScenesMapping.Add("Vehophobia", ""); // driving
        fearScenesMapping.Add("Aviophobia", ""); //flying
        fearScenesMapping.Add("Claustrophobia", "1,3,4,5,6");
        fearScenesMapping.Add("Glossophobia", ""); // public speaking
        fearScenesMapping.Add("Enochlophobia", "2,3"); // crowds
        fearScenesMapping.Add("Hemophobia", ""); // blood
        fearScenesMapping.Add("Trypanophobia", ""); // needle, getting shots
        fearScenesMapping.Add("Nosophobia", ""); // contracting disease
        fearScenesMapping.Add("Masklophobia", ""); // costume characters
        fearScenesMapping.Add("Phonophobia", ""); // voices, load sound
        fearScenesMapping.Add("Emetophobia", ""); // vomit
    }

    private bool loadScene = false;

    // Update is called once per frame
    void Update()
    {
        if (loadScene == false) {
            /*if (input.GetButtonDown("Recenter"))
            { // center tracking on current pose
                UnityEngine.VR.InputTracking.Recenter();
            }*/
            if (input.GetButtonDown("Back"))
            {
                SceneManager.LoadScene("Intro", LoadSceneMode.Single); // load without delay
            }

            if (string.Equals(guitext.text, ""))
            {
                temp = "";
            }
            else temp = guitext.text.Split('-')[0].Trim();

            if (!string.Equals(prevText, temp)) // update scene filter if there is change
            {
                if (temp.Equals("")) // activate all, show all scenes
                {
                    for (int i = 0; i < sceneCanvasesSize; i++)
                    {
                        sceneCanvases[i].SetActive(true);
                    }
                }
                else // look up dictionary to see which scenes are activated
                {
                    string list;
                    for (int i = 0; i < sceneCanvasesSize; i++)
                    {
                        sceneCanvases[i].SetActive(false);
                    }

                    if (fearScenesMapping.TryGetValue(temp, out list))
                    {
                        // success, show scenes in list
                        if (!list.Equals(""))
                        {
                            string[] arr = list.Split(',');
                            for (int j = 0; j < arr.Length; j++)
                            {
                                sceneCanvases[int.Parse(arr[j])].SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        // failure, show no scenes
                    }
                }

                prevText = temp;
            }
        }
    }

    /*IEnumerator Wait()
    {
        yield return new WaitForSeconds(5);
    }*/

    IEnumerator LoadNewScene(string sceneName)
    {
        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName); // LoadSceneMode.Single

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            yield return null;
        }

    }

    public void BaseLine()
    {
        if (loadScene == false)
        {
            loadScene = true;
            positionCam.transform.position = new Vector3(0.0f, 3000.0f, -300.0f);
            StartCoroutine(LoadNewScene("Baseline"));
        }
    }

    public void CityPark()
    {
        if (loadScene == false)
        {
            loadScene = true;
            positionCam.transform.position = new Vector3(0.0f, 3000.0f, -300.0f);
            StartCoroutine(LoadNewScene("CityPark"));
        }
    }

    public void ClaustrophobicApartment()
    {
        if (loadScene == false)
        {
            loadScene = true;
            positionCam.transform.position = new Vector3(0.0f, 3000.0f, -300.0f);
            StartCoroutine(LoadNewScene("Claustrophobic_Apartment"));
        }
    }

    public void MRIRoom()
    {
        if (loadScene == false)
        {
            loadScene = true;
            positionCam.transform.position = new Vector3(0.0f, 3000.0f, -300.0f);
            StartCoroutine(LoadNewScene("MRI-ROOM"));
        }
    }

    public void PassageToOffice()
    {
        if (loadScene == false)
        {
            loadScene = true;
            positionCam.transform.position = new Vector3(0.0f, 3000.0f, -300.0f);
            StartCoroutine(LoadNewScene("PassageToOffice"));
        }
    }

    public void Pit()
    {
        if (loadScene == false)
        {
            loadScene = true;
            positionCam.transform.position = new Vector3(0.0f, 3000.0f, -300.0f);
            StartCoroutine(LoadNewScene("Pit"));
        }
    }

    public void SubliminalProcessing()
    {
        if (loadScene == false)
        {
            loadScene = true;
            positionCam.transform.position = new Vector3(0.0f, 3000.0f, -300.0f);
            StartCoroutine(LoadNewScene("SubliminalProcessing"));
        }
    }

    public void UrbanTown()
    {
        if (loadScene == false)
        {
            loadScene = true;
            positionCam.transform.position = new Vector3(0.0f, 3000.0f, -300.0f);
            StartCoroutine(LoadNewScene("UrbanTown"));
        }
    }

    public void BigApartment()
    {
        if (loadScene == false)
        {
            loadScene = true;
            positionCam.transform.position = new Vector3(0.0f, 3000.0f, -300.0f);
            StartCoroutine(LoadNewScene("BigApartment"));
        }
    }

    public void Biomes()
    {
        if (loadScene == false)
        {
            loadScene = true;
            positionCam.transform.position = new Vector3(0.0f, 3000.0f, -300.0f);
            StartCoroutine(LoadNewScene("Biomes"));
        }
    }
}
