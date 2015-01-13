 using UnityEngine;
using System.Collections;

public class GameClient : MonoBehaviour 
{

	public GameObject player1Prefab;
	public GameObject player2Prefab;
	public GameObject player3Prefab;
	public GameObject player4Prefab;
	
	// Use this for initialization
	void Awake () 
	{
		// in case we started this demo with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.connected)
		{
			Application.LoadLevel(GameMenu.SceneNameMenu);
			return;
		}
		if(!GameManager.sInstaniated)
		{
			GameManager.Init();
		}
		// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		switch (GameManager.sPlayersInLobby + 1)
		{
			case 1:
				PhotonNetwork.Instantiate(player1Prefab.name, transform.position, Quaternion.identity, 0);
				break;
			case 2:
				PhotonNetwork.Instantiate(player2Prefab.name, transform.position, Quaternion.identity, 0);
				break;
			case 3:
				PhotonNetwork.Instantiate(player3Prefab.name, transform.position, Quaternion.identity, 0);
				break;
			case 4:
				PhotonNetwork.Instantiate(player4Prefab.name, transform.position, Quaternion.identity, 0);
				break;
				
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(GameManager.sPlayersInLobby > 0)
		{
			GameManager.GameLoop ();
		}
	}

	void OnGUI()
	{
		if(GameManager.sPlayersTurn <= GameManager.sPlayersInLobby)
		{
			GUI.Button(new Rect(10,400,100 ,50),"Players Turn " + (GameManager.sPlayersTurn+1).ToString());
		}
		else
		{
			GUI.Button(new Rect(10,400,100 ,50),"AI Turn");
		}
	}
}
