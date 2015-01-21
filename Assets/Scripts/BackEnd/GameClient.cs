 using UnityEngine;
using System.Collections;

public class GameClient : MonoBehaviour {

	public Player playerPrefab;
	// Use this for initialization
	void Awake () 
	{
		// in case we started this demo with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.connected)
		{
			Application.LoadLevel(GameMenu.SceneNameMenu);
			return;
		}
		if(!GameManager.Instaniated)
		{
			GameManager.Init();
		}
		// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		PhotonNetwork.Instantiate(playerPrefab.name, transform.position, Quaternion.identity, 0);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(GameManager.PlayersInLobby > 0)
		{
			GameManager.GameLoop ();
		}
	}

	void OnGUI()
	{
		if(GameManager.PlayersTurn <= GameManager.PlayersInLobby)
		{
			GUI.Button(new Rect(10,400,100 ,50),"Players Turn " + (GameManager.PlayersTurn+1).ToString());
		}
		else
		{
			GUI.Button(new Rect(10,400,100 ,50),"AI Turn");
		}
	}
}
