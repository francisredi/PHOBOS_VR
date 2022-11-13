using UnityEngine;
using System.Collections;
     
// Allows Control Over WindZone Component
     
// Note: The WindZone & This Script Must Be Attached To The Same Game Object
     


public class WindZoneController : WindZoneModify {

    public float windMain = 1.0f;
    public float windTurbolence = 0.2f;    
 
    // Use this for initialization
    void Start () {
        // Tell the ScriptableWindzoneInterface to initialize
        base.Init() ;
           
    }
       
    // Update is called once per frame
    void Update () {
           
        // Example: Setting each of the values
        WindMain = windMain;
        WindTurbulence =windTurbolence;
//        WindPulseMagnitude = WindMain ;
//        WindPulseFrequency = WindMain ;
//        Radius = WindMain;
    }
}
