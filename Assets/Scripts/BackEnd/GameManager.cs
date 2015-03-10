//Created December 10, 2014
//Updated December 16, 2014
//Copywrite Wyatt 2014
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	//publics
	public int sPlayersInRoom;
	public int sPlayersTurn;
	public int sTargetsAlive;
	public int sInstaniated = 0;
	public BaseTarget[] mTargetsObjects;
	public int[] mTargetSpawnPoint;		// 371, 402, 343, 202, 169, 161, 4, 20
	public TileMap mManagerTileMap;

	public DTileMap.TileType curDefending;
	public DTileMap.TileType curAttacking;
	
	public bool AttackWorked = false;
	public bool CounterAttackWorked = false;
	public bool HudUpdated = false;
	public Player[] sPlayers;
	public BaseTarget[] sTargets;

	private bool newPlayerAdded = false;
	 
	//Call this to restart the lobby
	public void Init()
	{
		if(sInstaniated <= 1)
		{
			Debug.Log("Instantiated");
			sPlayers.Initialize();
			sPlayersInRoom = 0;
			sPlayersTurn = 0;
			sTargetsAlive = 0;
			sInstaniated = 1;
			if(!PhotonNetwork.offlineMode)
			{
				mManagerTileMap = GameObject.Find("CurrentTileMap(Clone)").GetComponent<TileMap>();
			}
			else
			{
				mManagerTileMap = GameObject.Find("CurrentTileMap").GetComponent<TileMap>();
			}

			//HACK
			mTargetSpawnPoint = new int[8];
			mTargetSpawnPoint[0] = 371;
			mTargetSpawnPoint[1] = 402;
			mTargetSpawnPoint[2] = 343;
			mTargetSpawnPoint[3] = 202;
			mTargetSpawnPoint[4] = 169;
			mTargetSpawnPoint[5] = 161;
			mTargetSpawnPoint[6] = 4;
			mTargetSpawnPoint[7] = 20;
			//HACK
		}
	}

	void Start()
	{
		Init ();
	}
	//Adds Players to the game
	public bool AddPlayer(Player p)
	{
		bool rc = true;
		if(sPlayers.Length == 0)
		{
			sPlayers.SetValue(p, 0);
			sPlayersInRoom++;
		}
		else
		{
			foreach(Player j in sPlayers)
			{
				if(Equals(p,j))
				{
					Debug.Log("player already exists");
					rc = false;
				}
			}
			if(rc)
			{
				sPlayers.SetValue (p, sPlayersInRoom);
				sPlayersInRoom++;
			}
		}
		Debug.Log(sPlayersInRoom);
		newPlayerAdded = rc;
		return rc;
	}
	
	//Adds targets into the game
	public bool AddTarget(BaseTarget t)
	{
		if(sTargets[0]==null)
		{
			t.mTargetIndex = DTileMap.TileType.Target1;
			sTargets[0] = t;
			Debug.Log ("SpawnTarget at 1: " + sTargetsAlive);
			return true;
		}
		else if(sTargets[1]==null)
		{
			t.mTargetIndex = DTileMap.TileType.Target2;
			sTargets[1] = t;
			Debug.Log ("SpawnTarget at 2: " + sTargetsAlive);
			return true;
		}
		else if (sTargets[2]==null)
		{
			t.mTargetIndex = DTileMap.TileType.Target3;
			sTargets[2] = t;
			Debug.Log ("SpawnTarget at 3: " + sTargetsAlive);
			return true;
		}
		return true;
	}
	
	public Player CurrentPlayer()
	{
		return (Player)sPlayers [sPlayersTurn];
	}
	
	public Player CurrentPlayerDefender()
	{
		return sPlayers [curDefending - DTileMap.TileType.Player1];
	}

	public void RemoveTarget(BaseTarget targetToRemove)
	{
		DTileMap.TileType temp = targetToRemove.mTargetIndex;
		if(temp == DTileMap.TileType.Target1)
		{
			sTargets[0] = null;
		}
		else if(temp == DTileMap.TileType.Target2)
		{
			sTargets[1] = null;
		}
		else if(temp == DTileMap.TileType.Target3)
		{
			sTargets[2] = null;
		}
		sTargetsAlive--;
	}
	public BaseTarget CurrentTargetDefender()
	{
		return sTargets [curDefending - DTileMap.TileType.Target1];
	}
	// Call this to Have the game logic function
	public void GameLoop()
	{
		if(sTargetsAlive < 3)
		{
			Debug.Log ("Manager:" + sTargetsAlive);
			SpawnTarget();
		}
		if(newPlayerAdded)
		{
			CheckPlayers();
		}
		if(sPlayersTurn < sPlayersInRoom)
		{
			PlayerTurn((Player)sPlayers[sPlayersTurn]);
			//Debug.Log(sPlayersTurn);
		}
		else if (sPlayersTurn >= sPlayersInRoom)
		{
			AITurn();
			sPlayersTurn++;
			//Debug.Log(sPlayersTurn);
			sPlayersTurn = sPlayersTurn % (sPlayersTurn);
			//Debug.Log(sPlayersTurn);
		}
		curAttacking = (DTileMap.TileType)(sPlayersTurn + DTileMap.TileType.Player1);
	}
	
	//this is what the player can do on their turn
	private void PlayerTurn(Player p)
	{
		if(p)
		{
			if (p.mInfamy >= 5)
			{
				Debug.Log("Player " + p.mCharacter + " Wins");
			}
			if(!p.mTurn)
			{
				if(PhotonNetwork.offlineMode)
				{
					p.UpdatePlayer();
					//Debug.Log(sPlayersTurn);
				}
				else
				{
					if(p.networkView.isMine)
					{
						p.UpdatePlayer();
						//Debug.Log(sPlayersTurn);
					}
				}
			}
			else
			{
				p.mAttacked = false;
				p.mMoved = false;
				p.mTurn = false;
				p.mPlayerPhase = Player.PlayerPhase.Start;
				HudUpdated = false;
				AttackWorked = false;
				CounterAttackWorked = false;
				sPlayersTurn++;
				
				gameObject.GetPhotonView().RPC("SetPlayersTurn", PhotonTargets.Others, sPlayersTurn);
				//Debug.Log(sPlayersTurn);
			}
		}
		//Debug.Log ("LoganFuckUP" + curDefending + ("WeFUCkUp") + CurrentPlayer().curTarget);
	}
	
	//Do AI stuff in this function
	private void AITurn()
	{
		foreach(BaseTarget t in sTargets)
		{
			if(t)
			{
				if(t.UpdateTarget())
				{
					t.mTargetTurn = false;
				}
			}
		}
	}
	
	void CheckPlayers()
	{
		Player[] temp = new Player[5];
		int cintd = 0;
		for (int i = 0; i < sPlayers.Length; i++) 
		{
			if (sPlayers[i] != null)
			{
				temp[cintd] = sPlayers[i];
				cintd++;
			}
		}
		for (int i = 0; i < sPlayers.Length; i++) 
		{
			sPlayers[i] = temp[i];
		}
		newPlayerAdded = false;
	}
	
	void SpawnTarget()
	{
		if(mManagerTileMap == null)
		{
			Debug.Log ("mManagerTileMap is null");
		}
		else
		{
			Vector3[] positionToSpawn = new Vector3[8];
			Vector3 tempV3 = new Vector3 (0.0f, 0.0f, 0.0f);
			for(int i = 0; i < 8 ; i++)
			{
				DTileMap.TileType temp = mManagerTileMap.MapInfo.GetTileTypeIndex (i);
				if(temp == DTileMap.TileType.Floor)
				{
					tempV3 = mManagerTileMap.MapInfo.GetTileLocationIndex(i);
				}
			}
			//HACK
			if(!PhotonNetwork.offlineMode)
			{
				GameObject Target = PhotonNetwork.Instantiate(mTargetsObjects[(int)((Random.value * 100) % mTargetsObjects.Length)].name, tempV3, Quaternion.identity, 0);
				AddTarget(Target.GetComponent<BaseTarget>());
			}
			else
			{
				Object Target = Instantiate(mTargetsObjects[(int)((Random.value * 100) % mTargetsObjects.Length)].gameObject, tempV3, Quaternion.identity);
				AddTarget(((GameObject)Target).GetComponent<BaseTarget>());
			}
			sTargetsAlive++;
			//HACK
		}
	}

	[RPC]
	public void SetPlayersInRoom(int iPlayersInRoom)
	{
		sPlayersInRoom = iPlayersInRoom;
	}
	
	[RPC]
	public void SetPlayersTurn(int iPlayersTurn)
	{
		sPlayersTurn = iPlayersTurn;
	}
	
	
	
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//Debug.Log ("Writing");
			
			//We own this player: send the others our data			
			stream.SendNext( sInstaniated );
			stream.SendNext( sPlayersInRoom );
			stream.SendNext( sPlayersTurn );
			stream.SendNext( sTargetsAlive );
		}
		else
		{
			//Debug.Log ("Receiving");
			//Network player, receive data
			sInstaniated = (int)stream.ReceiveNext();
			sPlayersInRoom = (int)stream.ReceiveNext();
			sPlayersTurn = (int)stream.ReceiveNext();
			sTargetsAlive = (int)stream.ReceiveNext();
		}
	}
	
	void Awake()
	{
		if(GameObject.Find("GameClient"))
		{
			PhotonNetwork.offlineMode = false;
		}
		else
		{
			PhotonNetwork.offlineMode = true;
		}
	}
	
	void Update()
	{
		if(PhotonNetwork.offlineMode)
		{
			GameLoop ();
		}
		if(!PhotonNetwork.isMasterClient)
		{
			if(CurrentPlayer().networkView.isMine)
			{
				gameObject.GetPhotonView().RPC("SetPlayersTurn", PhotonTargets.Others, sPlayersTurn);
			}
		}
	}
	void OnGUI()
	{
		if(sPlayersTurn < sPlayersInRoom)
		{
			GUI.TextArea(new Rect(10,500,100 ,50),"Players Turn " + (sPlayersTurn+1).ToString());
		}
		else
		{
			GUI.TextArea(new Rect(10,400,100 ,50),"AI Turn");
			//Debug.Log(mManager.sPlayersTurn.ToString());
		}
	}
}