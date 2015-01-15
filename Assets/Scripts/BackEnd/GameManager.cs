//Created December 10, 2014
//Updated December 16, 2014
//Copywrite Wyatt 2014
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //publics
	public int sPlayersInLobby;
	public int sPlayersTurn;
	public int sTargetsAlive;
	public bool sInstaniated;
    //privates
	private ArrayList sPlayers;
	private ArrayList sTargets;
	private Player sLastPlayer;
	private BaseTarget sLastTarget;

	//Call this to restart the lobby
	public void Init()
	{
		if(sInstaniated != true)
		{
			Debug.Log("Instantiated");
			sPlayers = new ArrayList();
			sTargets = new ArrayList();
			sPlayersInLobby = 0;
			sPlayersTurn = 0;
			sTargetsAlive = 0;
			sInstaniated = true;
		}
	}

	//Adds Players to the game
	public bool AddPlayer(Player p)
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
	public bool AddTarget(BaseTarget t)
	{
		sTargets.Add (t);
		sTargetsAlive++;
		return true;
	}
	public Player CurrentPlayer()
	{
		return (Player)sPlayers [sPlayersTurn];
	}
    // Call this to Have the game logic function
	public void GameLoop()
    {
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
		if (sPlayersTurn > sPlayersInLobby)
		{
			AITurn();
			sPlayersTurn = sPlayersTurn % (sPlayersInLobby + 1);
			Debug.Log(sPlayersTurn);
		}
    }
	//this is what the player can do on their turn
	private void PlayerTurn(Player p)
    {
		while(!p.mAttacked)
		{
			if(Input.GetMouseButtonDown (0) && !p.mMoved)
			{			
				p.UpdatePlayer();
			}
			if(Input.GetMouseButtonDown(0) && !p.mAttacked)
			{
				p.Attack();
			}
		}
    }
	//Do AI stuff in this function
	private void AITurn()
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
