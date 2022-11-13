var music : AudioClip;
private var musicOnOff : boolean = true;

function Update () 
{

if(Input.GetKeyDown(KeyCode.M)){

	musicOnOff = !musicOnOff;
	GetComponent.<AudioSource>().Play();
	
		if(musicOnOff == false){
		musicOnOff = false;
		GetComponent.<AudioSource>().Stop();
		}	
}
}