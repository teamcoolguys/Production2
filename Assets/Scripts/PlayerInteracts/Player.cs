//Created by Dylan Fraser
//November 3, 2014

//Updated by
//Jack Ng
//November 4, 2014
//Wyatt Gibbs
//December 10, 2014
//Jack Ng
//Jan 8th, 2015

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TileMap))]
[RequireComponent(typeof(TileMapMouse))]
public class Player : MonoBehaviour
{

	//Information needed in the game 
	public baseCharacter mCharacter;
	TileMap mTileMap;
	TileMapMouse mMouse;
	GameObject mTileMapObject;

	//Current Stats
	public uint mAttack;
	public uint mDefence;
	public uint mMovement;
	public uint mRange;
	
	//Mouse Info
	private int mMouseX;
	private int mMouseY;

	//Tracking current Spot//
	public int mPositionX;
	public int mPositionY;


	//Wyatt//
	//stuff I am using for Game Loop
	public bool mMoved;
	public Hand mHand;
	private Vector3 syncEndPosition = Vector3.zero;
	//made this public so I could reference it in the Game Manager to pass to the HUD 
	//allows game loop to move forwardcurrently//
	
	public Deck mDeck;
	public GameObject Self;
	public int mInfamy = 0;

	// Use this for initialization
	void Start()
	{
		mTileMapObject=GameObject.Find("CurrentTileMap");
		mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
		mTileMap = mTileMapObject.GetComponent<TileMap>();
		mMouseX = mMouse.mMouseHitX;
		//fixed for negatvie Z values
		mMouseY = mMouse.mMouseHitY;
		//fixed for negative Z values
		//instantiates the objects in this object
		mMoved = false;
		mHand = new Hand();
		mDeck = new Deck ();
		GameManager.AddPlayer (this);//allows gamemanager to know that a new player is active
		Debug.Log ("Player Created");
	}

	void Update()
	{
		mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
		mTileMap = mTileMapObject.GetComponent<TileMap>();
		//Debug.Log ("Tile: " + mMouse.mMouseHitX + ", " + mMouse.mMouseHitY);
		mMouseX = mMouse.mMouseHitX;

		//fixed for negatvie Z values
		mMouseY = mMouse.mMouseHitY;
		//fixed for negatvie Z values

		//if (Input.GetKey ("escape")) 
		//{
		//	Application.Quit ();
		//}
		if (Input.GetMouseButtonDown (0))
		{
			UpdatePlayer ();
		}
	}

	public bool UpdatePlayer()
	{
		if(networkView.isMine)
		{
			//Debug.Log ("Tile: " + mMouse.mMouseHitX + ", " + mMouse.mMouseHitY);
			//Debug.Log ("Tile: " + mMouseX + ", " + mMouseY);
			//Debug.Log (mTileMap.MapInfo.GetTileType(mMouseX,mMouseY));
			int temp=mTileMap.MapInfo.GetTileType(mMouseX, mMouseY);
			//Random moveMent;
			//Debug.Log (temp);
			switch(temp)
			{
			case 1:
				Debug.Log ("Target::Floor");
				mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 1);
				Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(mMouseX, mMouseY);
				Move(v3Temp);
				mPositionX=mMouseX;
				mPositionY=mMouseY;
				mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 3);
				mMoved = true;
				break;
			case 2:
				Debug.Log ("Target::Wall");
				mMoved = false;
				break;
			default:
				//Debug.Log ("Target::Fuck Off");
				mMoved = false;
				break;
			}
		}
		return true;
	}

	void Move(Vector3 pos)
	{
		gameObject.transform.position = pos + new Vector3(0.0f, 1.0f, 0.0f);
	}

	//added this to try to fix some issues
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(rigidbody.position);
		}
		else
		{
			syncEndPosition = (Vector3)stream.ReceiveNext();
			GameManager.sPlayersTurn
				++;
		}
	}
}
