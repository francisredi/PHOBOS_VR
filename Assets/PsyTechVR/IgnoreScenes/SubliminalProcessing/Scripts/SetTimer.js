
function Update() {

if(Input.GetKeyDown(KeyCode.Alpha1)){
gameObject.GetComponent("DisappearAppear").timeDisabled = 0.0166;
}

if(Input.GetKeyDown(KeyCode.Alpha2)){
gameObject.GetComponent("DisappearAppear").timeDisabled = 0.2;
}

if(Input.GetKeyDown(KeyCode.Alpha5)){
gameObject.GetComponent("DisappearAppear").timeDisabled = 0.5;
}

if(Input.GetKeyDown(KeyCode.Alpha6)){
gameObject.GetComponent("DisappearAppear").timeDisabled = 1;
}

if(Input.GetKeyDown(KeyCode.Alpha7)){
gameObject.GetComponent("DisappearAppear").timeDisabled = 5;
}


if(Input.GetKeyDown(KeyCode.Alpha8)){
gameObject.GetComponent("DisappearAppear").timeDisabled = 10;
}

if(Input.GetKeyDown(KeyCode.Alpha0)){
gameObject.GetComponent("DisappearAppear").timeDisabled = 800;
}


}
