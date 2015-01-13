//Created December 10, 2014
//Updated December 16, 2014
//Copywrite Wyatt 2014
using UnityEngine;
using System.Collections;

static public class GameManager
{
    //publics
	public static int sPlayersInLobby;
	public static int sPlayersTurn = 0;
	public static int sTargetsAlive;
	public static bool sInstaniated = false;
    //privates
	private static ArrayList sPlayers;
	private static ArrayList sTargets;
	private static Player sLastPlayer;
	private static BaseTarget sLastTarget;
	//Call this to restart the lobby
	static public void Init()
	{
		if(sInstaniated != true)
		{
			sPlayers = new ArrayList();
			sTargets = new ArrayList();
			sPlayersInLobby = 0;
			sPlayersTurn = 0;
			sTargetsAlive = 0;
			sInstaniated = true;
		}
	}
	//Adds Players to the game
	static public bool AddPlayer(Player p)
	{
		if(sPlayers.Count == 0)
		{
			sPlayers.Add(p);
			sLastPlayer = p;
			return true;
		}
		foreach(Player j in sPlayers)
		{
			if(Equals(p,j))
			{
				Debug.Log("player already exists");
				return false;
			}
		}
		sPlayers.Add (p);
		sPlayersInLobby++;
		return true;
	}
	//Adds targets into the game
	static public bool AddTarget(BaseTarget t)
	{
		sTargets.Add (t);
		sTargetsAlive++;
		return true;
	}
	static public Player CurrentPlayer()
	{
		return (Player)sPlayers [sPlayersTurn];
	}
    // Call this to Have the game logic function
	static public void GameLoop()
    {
		if (sPlayersTurn > sPlayersInLobby)
		{
			AITurn();
			sPlayersTurn = sPlayersTurn % (sPlayersInLobby+1);
			Debug.Log(sPlayersTurn);
		}
		if(sLastPlayer != (Player)sPlayers[sPlayersTurn])
		{
			sLastPlayer.mMoved = false;
			sLastPlayer.mHand.PlayedCard = false;
			sLastPlayer = (Player)sPlayers[sPlayersTurn];
		}
		if(sPlayersTurn <= sPlayersInLobby)
		{
			PlayerTurn((Player)sPlayers[sPlayersTurn]);
		}
    }
	//this is what the player can do on their turn
	static private void PlayerTurn(Player p)
    {
		if(Input.GetMouseButtonDown (0))
		{
			p.UpdatePlayer();
		}
		if(p.mMoved)//&& p.mHand.PlayedCard)
		{
			sPlayersTurn++;
		}
    }
	//Do AI stuff in this function
	static private void AITurn()
	{
		foreach(BaseTarget t in sTargets)
		{
			if(t.UpdateTarget())
			{
				t.mTargetTurn = false;
			}
		}
	}
}
