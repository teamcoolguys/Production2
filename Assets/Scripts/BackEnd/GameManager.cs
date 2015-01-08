//Created December 10, 2014
//Updated December 16, 2014
//Copywrite Wyatt 2014
using UnityEngine;
using System.Collections;

static public class GameManager
{
    //publics
	public static int PlayersInLobby;
	public static int PlayersTurn;
	public static int TargetsAlive;
	public static bool Instaniated = false;
    //privates
	private static ArrayList Players;
	private static ArrayList Targets;
	private static Player LastPlayer;
	private static BaseTarget LastTarget;
	//Call this to restart the lobby
	static public void Init()
	{
		Players = new ArrayList();
		Targets = new ArrayList();
		PlayersInLobby = 0;
		PlayersTurn = 0;
		TargetsAlive = 0;
		Instaniated = true;
	}
	//Adds Players to the game
	static public bool AddPlayer(Player p)
	{
		if(Players.Count == 0)
		{
			Players.Add(p);
			LastPlayer = p;
			return true;
		}
		foreach(Player j in Players)
		{
			if(Equals(p,j))
			{
				Debug.Log("player already exists");
				return false;
			}
		}
		Players.Add (p);
		PlayersInLobby++;
		return true;
	}
	//Adds targets into the game
	static public bool AddTarget(BaseTarget t)
	{
		Targets.Add (t);
		TargetsAlive++;
		return true;
	}
	static public Player CurrentPlayer()
	{
		return (Player)Players [PlayersTurn];
	}
    // Call this to Have the game logic function
	static public void GameLoop()
    {
		if (PlayersTurn > PlayersInLobby)
		{
			AITurn();
			PlayersTurn = PlayersTurn % (PlayersInLobby+1);
		}
		if(LastPlayer != (Player)Players[PlayersTurn])
		{
			LastPlayer.moved = false;
			LastPlayer.mHand.PlayedCard = false;
			LastPlayer = (Player)Players[PlayersTurn];
		}
		if(PlayersTurn <= PlayersInLobby)
		{
			PlayerTurn((Player)Players[PlayersTurn]);
		}
    }
	//this is what the player can do on their turn
	static private void PlayerTurn(Player p)
    {
		if(Input.GetMouseButtonDown (0))
		{
			p.UpdatePlayer();
		}
		if(p.moved)//&& p.mHand.PlayedCard)
		{
			PlayersTurn++;
		}
    }
	//Do AI stuff in this function
	static private void AITurn()
	{
		foreach(BaseTarget t in Targets)
		{
			if(t.UpdateTarget())
			{
				t.TargetTurn = false;
			}
		}
	}
}
