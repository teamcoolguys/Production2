//Created by Dylan Fraser
//November 3, 2014
//Jack Ng
//November 4, 2014
//Wyatt Gibbs
//December 10, 2014

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TileMap))]
[RequireComponent(typeof(TileMapMouse))]
public class Player : MonoBehaviour
{
	TileMap mTileMap;
	TileMapMouse mMouse;
	GameObject mTileMapObject;
	//privates
	private int mMouseX;
	private int mMouseY;
	private Space currentSpace;
	private TestMap mCurrentGrid;
	//private TestMap map;
	 
			//Jack//
	public int mCurrentSpot;
	public baseCharacter mCharacter;
	public int mPositionX;
	public int mPositionY;
	//Tracking current Spot//

	//Wyatt//
	public bool moved;
	public Hand mHand; //made this public so I could reference it in the Game Manager to pass to the HUD 
	//allows game loop to move forwardcurrently//

	//publics
	public Deck mDeck;

	public GameObject Self;

	public int mInfamy = 0;
	public int mRange = 0;

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
		moved = false;
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

		if (Input.GetKey ("escape")) 
		{
			Application.Quit ();
		}
	}

	public bool UpdatePlyer()
	{
		Debug.Log ("Tile: " + mMouse.mMouseHitX + ", " + mMouse.mMouseHitY);
		Debug.Log ("Tile: " + mMouseX + ", " + mMouseY);
		Debug.Log (mTileMap.MapInfo.GetTileType(mMouseX,mMouseY));
		int temp=mTileMap.MapInfo.GetTileType(mMouseX, mMouseY);
		//Random moveMent;
		Debug.Log (temp);
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
			moved = true;
			break;
		case 2:
			Debug.Log ("Target::Wall");
			moved = false;
			break;
		default:
			Debug.Log ("Target::Fuck Off");
			moved = false;
			break;
		}
		return true;
	}

	void Move(Vector3 pos)
	{
		gameObject.transform.position = pos + new Vector3(0.0f, 1.0f, 0.0f);
	}

	public void SetCurrentSpace(Space nextSpace)
	{
		currentSpace = nextSpace;
	}

	public Transform FindCurrentSpace()
	{
		return currentSpace.transform;
	}
}
