using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour {
//---------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------

    [HideInInspector] public GameObject sfxThunder;
    [HideInInspector] public GameObject mainCamera;
    [HideInInspector] public VisSky visky;

    float sfxTimeDelay;
    float brightness;

    int mode;
    bool sfxCreated;

    Renderer skyLightning;

//---------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------
void Start()
{
    mode = 0;    

    float distance = Vector3.Distance(mainCamera.transform.position, gameObject.transform.position);
    sfxTimeDelay = distance / 2500.0f;

    skyLightning = transform.Find("lightning").gameObject.GetComponent<Renderer>();

    brightness = 0.0f;

    sfxCreated = false;
}
//---------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------
void Update()
{
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    //
    // Mode 0: hell werden lassen
    //
    if(mode == 0)
    {
        brightness += Time.deltaTime * 8.0f;
        if(brightness >= 1.0f)
        {
            brightness = 1.0f;
            mode = 1;
        }
    }
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    //
    // Mode 1: dunkel werden lassen
    //
    if(mode == 1)
    {
        brightness -= Time.deltaTime * 2.0f;
        if(brightness <= 0.0f)
        {   
            brightness = 0.0f;
            mode = 2;
        }
    }
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    //
    // Wait for sfx creation
    //
    if(sfxCreated == false)
    {
        sfxTimeDelay -= Time.deltaTime;
        if(sfxTimeDelay <= 0.0f)
        {
            GameObject sound = (GameObject)Instantiate(sfxThunder);
            sound.transform.parent = gameObject.transform.parent.transform;
            sound.transform.localPosition = Vector3.zero;
            sound.GetComponent<ThunderSfx>().sfxVolume = visky.sfxVolume;
            sound.GetComponent<ThunderSfx>().enabled = true;
            sfxCreated = true;
        }
    }
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------
    //
    // Wait until color flash done and sfx created
    //
    if( (mode == 2) && (sfxCreated == true))
    {
        Destroy(gameObject);
        return;
    }
    //---------------------------------------------------------------------------------------------
    gameObject.transform.eulerAngles = new Vector3(0.0f, mainCamera.transform.eulerAngles.y, 0.0f);
    Color color = new Color(brightness, brightness, brightness, brightness);
    gameObject.transform.GetComponent<Renderer>().materials[0].SetColor("_TintColor", color);
    color = new Color(brightness / 2.0f, brightness / 2.0f, brightness / 1.5f, brightness / 3.0f);
    skyLightning.materials[0].SetColor("_TintColor", color);
    gameObject.GetComponent<Light>().intensity = brightness * 8.0f;
}
//---------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------
}
