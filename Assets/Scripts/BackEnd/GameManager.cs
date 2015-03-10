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
	public BaseTarget[] mTargetsObjects;
	public int[] mTargetSpawnPoint;
	public TileMap mManagerTileMap;
	
	public DTileMap.TileType curDefending;
	public DTileMap.TileType curAttacking;
	
	public bool AttackWorked = false;
	public bool CounterAttackWorked = false;
	public bool HudUpdated;

	public Player[] sPlayers;
	public BaseTarget[] sTargets;

	private bool newPlayerAdded = false;
	private bool GameOver = false;

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

			mManagerTileMap = GameObject.Find("CurrentTileMap").GetComponent<TileMap>();

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
		sTargets.SetValue(t, sTargetsAlive);
		t.mTargetIndex = DTileMap.TileType.Target1 + sTargetsAlive;
		sTargetsAlive++;
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

	public BaseTarget CurrentTargetDefender()
	{
		return sTargets [curDefending - DTileMap.TileType.Target1];
	}

	public void GameLoop()
	{
		if(sTargetsAlive < 2)
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
			if(p.mInfamy >= 5)
			{
			
			}
			if(!p.mTurn)
			{
				if(PhotonNetwork.offlineMode)
				{
					p.UpdatePlayer();
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
				p.ResetWalkRange();

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
				PhotonNetwork.Instantiate(mTargetsObjects[(int)((Random.value * 100) % mTargetsObjects.Length)].name, tempV3, Quaternion.identity, 0);
			}
			else
			{
				Instantiate(mTargetsObjects[(int)((Random.value * 100) % mTargetsObjects.Length)].gameObject, tempV3, Quaternion.identity);
			}
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
			if(CurrentPlayer () == null)
			{
				Debug.Log ("Fucking Broken");
			}
			else
			{
				Debug.Log ("CurrentPlayerNetworkView" + CurrentPlayer().networkView.isMine);
				if(CurrentPlayer().networkView.isMine)
				{
					gameObject.GetPhotonView().RPC("SetPlayersTurn", PhotonTargets.Others, sPlayersTurn);
				}
			}
		}
	}
	void OnGUI()
	{
		if(sPlayersTurn < sPlayersInRoom)
		{
			GUI.TextArea(new Rect(10,400,100 ,50),"Players Turn " + (sPlayersTurn+1).ToString());
		}
		else
		{
			GUI.TextArea(new Rect(10,400,100 ,50),"AI Turn");
			//Debug.Log(mManager.sPlayersTurn.ToString());
		}
		if(CurrentPlayer().mInfamy >= 5)
		{
			GUI.TextArea(new Rect(Screen.width/2, Screen.height/2, 500, 50), CurrentPlayer().mCharacter + " WINS the Game");
			CurrentPlayer().mAudio.clip = CurrentPlayer().DialogWin[Random.Range(0, CurrentPlayer().DialogWin.Length)];
		}
	}

}