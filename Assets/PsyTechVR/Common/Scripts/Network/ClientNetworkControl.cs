using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ClientNetworkControl : MonoBehaviour {

	public UnityEngine.UI.Text useMessageText = null;
	
	private bool doonce = true;
	
	NetworkClient myClient;
	
	// Use this for initialization
	void Start () {
		SetupClient ();
	}
	
	// Update is called once per frame
	void Update () {
		if (doonce) {
			//myClient.Send<Msg>(short,Msg);
			//myClient
			//NetworkClient.Send<MSG>(short,MSG);
			//Cmd_Damage ();
			
			DrControlsMsg cmsg = new DrControlsMsg ();
			cmsg.SceneName = "It works!";
			bool result = myClient.Send (MyMsgType.CityParkSceneMsgType,cmsg);
			if(result){
				print ("client sent message");
				//CmdSendCommand ();
				doonce = false;
			}
			
		}
		
	}
	
	// Create a client and connect to the server port
	public void SetupClient()
	{
		myClient = new NetworkClient();
		myClient.RegisterHandler(MsgType.Connect, OnConnected);     
		myClient.Connect("127.0.0.1", 4444);
		
		print ("Message ID #" + MyMsgType.CityParkSceneMsgType);
	}
	
	// client function
	public void OnConnected(NetworkMessage netMsg)
	{
		Debug.Log("Connected to server");
		print ("Connected now");
	}
	
	/*public void OnServerUpdate(NetworkMessage netMsg)
	{
		useMessageText.text = "You have been tricked by client!";
	}*/
	
	/*[Command]
	void CmdSendCommand ()
	{
		NetworkBehaviour.print ("client is doing this - FAKE");
	}*/
	
	//[Command]
	/*void Cmd_Damage()//sent to server
	{
		Rpc_DoDamage();//server executes
	}
	
	//[ClientRpc]
	void Rpc_DoDamage()
	{
		///health -= 10;//happens on all clients
	}*/
}
