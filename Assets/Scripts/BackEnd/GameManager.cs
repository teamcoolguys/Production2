//Created December 10, 2014
//Updated December 16, 2014
//Copywrite Wyatt 2014
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //publics
	public int sPlayersInRoom;
	public int sPlayersTurn;
	public int sTargetsAlive;
	public int sInstaniated = 0;
    //privates
	public Player[] sPlayers;
	public BaseTarget[] sTargets;

	//Call this to restart the lobby
	public void Init()
	{
		if(sInstaniated <= 1)
		{
			Debug.Log("Instantiated");
			sPlayers.Initialize();
			sTargets.Initialize();
			sPlayersInRoom = 0;
			sPlayersTurn = 0;
			sTargetsAlive = 0;
			sInstaniated = 1;
		}
	}

	//Adds Players to the game
	public bool AddPlayer(Player p)
	{
		if(sPlayers.Length == 0)
		{
			sPlayers.SetValue(p, 0);
			sPlayersInRoom++;
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
		sPlayers.SetValue(p, sPlayersInRoom);
		sPlayersInRoom++;
		Debug.Log(sPlayersTurn);
		return true;
	}

	//Adds targets into the game
	public bool AddTarget(BaseTarget t)
	{
		sTargets.SetValue(t, sTargetsAlive);
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
		if(sPlayersTurn <= sPlayersInRoom)
		{
			PlayerTurn((Player)sPlayers[sPlayersTurn]);
		}
		if (sPlayersTurn > sPlayersInRoom)
		{
			AITurn();
			sPlayersTurn = sPlayersTurn % (sPlayersInRoom + 1);
			Debug.Log(sPlayersTurn);
		}
    }

	//this is what the player can do on their turn
	private void PlayerTurn(Player p)
    {
		if(!p.mMoved)
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
			//p.mHand.PlayedCard = false;
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

	public byte[] SerializeGameManager(object customObject)
	{
		GameManager gm = (GameManager)customObject;
		int index = 0;
		byte[] playerBytes = ExitGames.Client.Photon.Protocol.Serialize (gm.sPlayers);
		byte[] targetBytes = ExitGames.Client.Photon.Protocol.Serialize (gm.sTargets);
		byte[] bytes = new byte[playerBytes.Length + targetBytes.Length + 16];
		ExitGames.Client.Photon.Protocol.Serialize (playerBytes.Length, bytes, ref index);
		System.Array.Copy (playerBytes, 0, bytes, index, playerBytes.Length);
		index += playerBytes.Length;
		ExitGames.Client.Photon.Protocol.Serialize (targetBytes.Length, bytes, ref index);
		System.Array.Copy (targetBytes, 0, bytes, index, targetBytes.Length);
		index += targetBytes.Length;
		ExitGames.Client.Photon.Protocol.Serialize (gm.sPlayersInRoom, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Serialize (gm.sPlayersTurn, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Serialize (gm.sTargetsAlive, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Serialize (gm.sInstaniated, bytes, ref index);
		return bytes;
	}
	public object DeserializeMethod(byte[] bytes)
	{
		GameManager gm = new GameManager();
		int index = 0;
		int playerBytesLength;
		int targetBytesLength;
		ExitGames.Client.Photon.Protocol.Deserialize (out playerBytesLength, bytes, ref index);
		byte[] playerBytes = new byte[playerBytesLength];
		System.Array.Copy(bytes, index, playerBytes, 0, playerBytesLength);
		gm.sPlayers = ((Player[])ExitGames.Client.Photon.Protocol.Deserialize (playerBytes));
		index += playerBytesLength;
		ExitGames.Client.Photon.Protocol.Deserialize (out targetBytesLength, bytes, ref index);
		byte[] targetBytes = new byte[targetBytesLength];
		System.Array.Copy(bytes, index, targetBytes, 0, targetBytesLength);
		gm.sPlayers = ((Player[])ExitGames.Client.Photon.Protocol.Deserialize(targetBytes));
		index += targetBytesLength;
		ExitGames.Client.Photon.Protocol.Deserialize (out gm.sPlayersInRoom, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Deserialize (out gm.sPlayersTurn, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Deserialize (out gm.sTargetsAlive, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Deserialize (out gm.sInstaniated, bytes, ref index);
		return gm;
	}
	
	void OnPhotonSerializeView(PhotonStream stream,	PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//We own this player: send the others our data
			stream.SendNext(this);
		}
		else
		{
			GameManager networks = (GameManager)stream.ReceiveNext();
			//Network player, receive data
			sInstaniated = networks.sInstaniated;
			sPlayersInRoom = networks.sPlayersInRoom;
			sPlayers = networks.sPlayers;
			sTargets = networks.sTargets;
			sPlayersTurn = networks.sPlayersTurn;
			sTargetsAlive = networks.sTargetsAlive;
		}
	}
}
