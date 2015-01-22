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
		Debug.Log(sPlayersTurn);
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
//		if(sLastPlayer != (Player)sPlayers[sPlayersTurn])
//		{
//			sLastPlayer.mMoved = false;
//			sLastPlayer.mHand.PlayedCard = false;
//			sLastPlayer.mAttacked = false;
//			sLastPlayer = (Player)sPlayers[sPlayersTurn];
//		}
		if(sPlayersTurn <= sPlayersInLobby)
		{
			PlayerTurn((Player)sPlayers[sPlayersTurn]);
		}
		if (sPlayersTurn > sPlayersInLobby)
		{
			AITurn();
			sPlayersTurn = sPlayersTurn % (sPlayersInLobby  );
			Debug.Log(sPlayersTurn);
		}
    }

	//this is what the player can do on their turn
	private void PlayerTurn(Player p)
    {
		if(!p.mAttacked)
		{
			if(Input.GetMouseButtonDown (0))
			{
				if(p.networkView.isMine)
				{
					p.UpdatePlayer();
					Debug.Log(sPlayersTurn);
				}
			}
		}
		else
		{
			p.mAttacked = false;
			p.mMoved = false;
			p.mHand.PlayedCard = false;
			sPlayersTurn++;
			Debug.Log(sPlayersTurn);
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

	void OnPhotonSerializeView(PhotonStream stream,	PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//We own this player: send the others our data
			stream.SendNext(sPlayersInLobby);
			stream.SendNext(sPlayersTurn);
			stream.SendNext(sPlayers);
			stream.SendNext(sTargetsAlive);
			stream.SendNext(sInstaniated);
			stream.SendNext(sTargets);
		}
		else
		{
			//Network player, receive data
			sPlayersInLobby = (int)stream.ReceiveNext();
			sPlayersTurn = (int)stream.ReceiveNext();
			sPlayers = (ArrayList)stream.ReceiveNext();
			sTargetsAlive = (int)stream.ReceiveNext();
			sInstaniated = (bool)stream.ReceiveNext();
			sTargets = (ArrayList)stream.ReceiveNext();
		}
	}
}
