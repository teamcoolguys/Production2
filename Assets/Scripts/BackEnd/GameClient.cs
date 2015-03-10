 using UnityEngine;
using System.Collections;

public class GameClient : MonoBehaviour 
{
	public GameObject mPlayer1Prefab;
	public GameObject mPlayer2Prefab;
	public GameObject mPlayer3Prefab;
	public GameObject mPlayer4Prefab;
	public GameObject mGameManager;
	public GameObject mTileMap;

	private GameManager mManager;
	private int playersInRoom = 0;
	private GameObject[] players;
	private GameObject[] mTileMaps;

	// Use this for initialization
	void Awake()
	{
		// in case we started this demo with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.connected)
		{
			Application.LoadLevel(GameMenu.SceneNameMenu);
			return;
		}
		if (!PhotonNetwork.offlineMode)
		{
			Debug.Log("GameClient::DestroyingPlayers");
			players = GameObject.FindGameObjectsWithTag("Player");		
			foreach (Object player in players) 
			{
				Destroy(player);
			}
			Debug.Log("GameClient::DestroyingMap");
			mTileMaps = GameObject.FindGameObjectsWithTag("Map");
			foreach (Object player in mTileMaps) 
			{
				Destroy(player);
			}
		}
		if(PhotonNetwork.isMasterClient)
		{
			Debug.Log("GameClient::CreatingTileMap");
			mTileMap = PhotonNetwork.Instantiate(mTileMap.name, transform.position, Quaternion.identity, 0);
			Debug.Log("GameClient::CreatingGameManager");
			mGameManager = PhotonNetwork.Instantiate(mGameManager.name, transform.position, Quaternion.identity, 0);
			mManager = mGameManager.GetComponent<GameManager>();
			mManager.mManagerTileMap = mTileMap.GetComponent<TileMap>();
		}
	}
	void Start () 
	{
		// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		if(PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.Instantiate(mPlayer1Prefab.name, transform.position, Quaternion.identity, 0);
		}
	}
	// Update is called once per frame
	void Update () 
	{
		if(!mTileMap) 
		{
			Debug.Log("GameClient::CreatingTileMap");
			mTileMap = PhotonNetwork.Instantiate(mTileMap.name, transform.position, Quaternion.identity, 0);
		}
		if(!mManager)
		{
			players = GameObject.FindGameObjectsWithTag("Player");		
			foreach (Object player in players) 
			{
				playersInRoom++;
			}
			mGameManager = GameObject.Find("GameManager(Clone)");
			mManager = mGameManager.GetComponent<GameManager>();
			if(playersInRoom == 0)
			{
				PhotonNetwork.Instantiate(mPlayer1Prefab.name, transform.position, Quaternion.identity, 0);
			}
			else if(playersInRoom == 1)
			{
				PhotonNetwork.Instantiate(mPlayer2Prefab.name, transform.position, Quaternion.identity, 0);
			}
			else if(playersInRoom == 2)
			{
				PhotonNetwork.Instantiate(mPlayer3Prefab.name, transform.position, Quaternion.identity, 0);
			}
			else if(playersInRoom == 3)
			{
				PhotonNetwork.Instantiate(mPlayer4Prefab.name, transform.position, Quaternion.identity, 0);
			}
			mManager.mManagerTileMap = mTileMap.GetComponent<TileMap>();
		}
		else //if(mManager)
		{
			if(mManager.sPlayersInRoom > 0)
			{
				mManager.GameLoop ();
			}
		}
	}

	void OnGUI()
	{
		if(mManager)
		{
			if(mManager.sPlayersTurn < mManager.sPlayersInRoom)
			{
				GUI.TextArea(new Rect(10,400,100 ,50),"Players Turn " + (mManager.sPlayersTurn+1).ToString());
			}
			else
			{
				GUI.TextArea(new Rect(10,400,100 ,50),"AI Turn");
				//Debug.Log(mManager.sPlayersTurn.ToString());
			}
			
			if(GUI.Button(new Rect (10, 500, 100, 50), "End Turn"))
			{
				mManager.sPlayersTurn++;
			}
		}
	}
}
