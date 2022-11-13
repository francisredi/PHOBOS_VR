var plane : GameObject;

//var light1:Light;
//var light2:Light;
//private var lightOn : boolean = true;

function Update () {
if(Input.GetKeyDown(KeyCode.W)){

  	//lightOn = !lightOn;
	//light1.enabled = !lightOn;
  	//light2.enabled = !lightOn;
  	
    Camera.main.backgroundColor = Color.white;
	plane.GetComponent.<Renderer>().material.color = Color.white;
}

if(Input.GetKeyDown(KeyCode.B)){

		Camera.main.backgroundColor = Color.black;
		plane.GetComponent.<Renderer>().material.color = Color.black;

	}
}