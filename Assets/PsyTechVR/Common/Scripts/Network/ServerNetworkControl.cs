using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ServerNetworkControl : MonoBehaviour {

	public UnityEngine.UI.Text useMessageText = null;

	// Use this for initialization
	void Start () {
		SetupServer();
		print ("Message ID #" + MyMsgType.CityParkSceneMsgType);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Create a server and listen on a port
	public void SetupServer()
	{
		NetworkServer.RegisterHandler(MyMsgType.CityParkSceneMsgType, OnServerUpdate);
		NetworkServer.Listen(4444);
		//isAtStartup = false;
	}

	/*void Rpc_DoDamage()
	{
		print ("it works");
	}*/

	public void OnServerUpdate(NetworkMessage netMsg)
	{
		DrControlsMsg msg = netMsg.ReadMessage<DrControlsMsg>();
		//useMessageText.text = "Scene name is: " + msg.SceneName;
		print (msg.SceneName);
	}


	/*void CmdSendCommand ()
	{
		NetworkBehaviour.print ("server is doing command");
	}*/
}
