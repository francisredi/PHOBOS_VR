var timeDisabled : float = 1.0;
var timeEnabled : float = 1.0;


function Update ()
{
if(Input.GetKeyDown("space"))
	{
	StartCoroutine( "Fading" );
	}
} 


	function Fading ()
	{
		while (true)
		{

		//appear
		GetComponent.<Renderer>().enabled = true;
		yield WaitForSeconds (timeEnabled);
		
		//disappear
		GetComponent.<Renderer>().enabled = false;
		yield WaitForSeconds (timeDisabled);


StopCoroutine("Fading");

}

}

