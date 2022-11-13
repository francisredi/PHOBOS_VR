#pragma strict

var walkSpeed: float = 7; // regular speed
var crchSpeed: float = 3; // crouching speed
var runSpeed: float = 20; // run speed

private var chMotor: CharacterMotor;
private var tr: Transform;
private var dist: float; // distance to ground
private var input : Rewired.Player;
private var vScaleHeight: float;

function Start(){
	chMotor = GetComponent(CharacterMotor);
	input = Rewired.ReInput.players.GetPlayer(0);
	tr = transform;
	vScaleHeight = tr.localScale.y;
	var ch:CharacterController = GetComponent(CharacterController);
	dist = ch.height/2; // calculate distance to ground
}

function Update(){

	var vScale = vScaleHeight; // originalHeight
	var speed = walkSpeed;

	if (chMotor.grounded && input.GetButton("Run")){
	 speed = runSpeed;
	}
	if (input.GetButton("Crouch")){ // crouch
	 vScale = vScaleHeight/2.0f;
	 speed = crchSpeed; // slow down when crouching
	}
	chMotor.movement.maxForwardSpeed = speed; // set max speed
	var ultScale = tr.localScale.y; // crouch/stand up smoothly
	tr.localScale.y = Mathf.Lerp(tr.localScale.y, vScale, 5*Time.deltaTime);
	tr.position.y += dist * (tr.localScale.y-ultScale); // fix vertical position
}