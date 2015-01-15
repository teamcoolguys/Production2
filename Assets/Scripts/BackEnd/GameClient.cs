 using UnityEngine;
using System.Collections;

public class GameClient : MonoBehaviour 
{
	public GameObject player1Prefab;
	public GameObject player2Prefab;
	public GameObject player3Prefab;
	public GameObject player4Prefab;
	
	private GameManager mManager;
	// Use this for initialization
	void Awake () 
	{
		// in case we started this demo with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.connected)
		{
			Application.LoadLevel(GameMenu.SceneNameMenu);
			return;
		}
		mManager = GameObject.Find ("GameManager").GetComponent<GameManager>();
		if(!mManager.sInstaniated)
		{
			mManager.Init();
		}
		// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		switch (mManager.sPlayersInLobby)
		{
			case 0:
				PhotonNetwork.Instantiate(player1Prefab.name, transform.position, Quaternion.identity, 0);
				break;
			case 1:
				PhotonNetwork.Instantiate(player2Prefab.name, transform.position, Quaternion.identity, 0);
				break;
			case 2:
				PhotonNetwork.Instantiate(player3Prefab.name, transform.position, Quaternion.identity, 0);
				break;
			case 3:
				PhotonNetwork.Instantiate(player4Prefab.name, transform.position, Quaternion.identity, 0);
				break;
		}
	}
	void OnPlayerConnected()
	{

	}
	// Update is called once per frame
	void Update () 
	{
		if(mManager.sPlayersInLobby > 0)
		{
			mManager.GameLoop ();
		}
	}

	void OnGUI()
	{
		if(mManager.sPlayersTurn <= mManager.sPlayersInLobby)
		{
			GUI.Button(new Rect(10,400,100 ,50),"Players Turn " + (mManager.sPlayersTurn+1).ToString());
		}
		else
		{
			GUI.Button(new Rect(10,400,100 ,50),"AI Turn");
		}

		if(GUI.Button (new Rect (10, 500, 50, 100), "End Turn"))
		{
			mManager.sPlayersTurn++;
		}
	}
}
