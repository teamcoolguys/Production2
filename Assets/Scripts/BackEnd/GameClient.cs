 using UnityEngine;
using System.Collections;

public class GameClient : MonoBehaviour {

	// Use this for initialization
	void Awake () 
	{
		GameManager.Init ();
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
