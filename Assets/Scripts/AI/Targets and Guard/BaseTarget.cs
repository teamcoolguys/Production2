//Jack Ng
//Nov 5th, 2014


using UnityEngine;
using System.Collections;

public class BaseTarget : MonoBehaviour {

	public int mPositionX;
	public int mPositionY;

	public int mDefense;
	public int mCurDefense;
	
	public int mMovement;
	public int mRunMovement;
	
	public int mInfarmy;
	public int mGold;
	
	public int mDetectionRun;
	public int mDetectionWalk;
	public int mDetectionSee;

	public bool TargetTurn;

	TileMap mTileMap;
	TileMapMouse mMouse;
	GameObject mTileMapObject;
	//privates
	private int mMouseX;
	private int mMouseY;
	// Use this for initialization
	void Start () 
	{
		TargetTurn = false;
		//mMouse = mTileMapObject.GetComponent<TileMapMouse> ();

		mTileMapObject=GameObject.Find("CurrentTileMap");

		mTileMap = mTileMapObject.GetComponent<TileMap>();
		//mMouseX = mMouse.mMouseHitX;
		////fixed for negatvie Z values
		//mMouseY = mMouse.mMouseHitY;
		//Hard fixed for negative Z values
		GameManager.AddTarget (this);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
		mTileMap = mTileMapObject.GetComponent<TileMap>();
		//Debug.Log ("Tile: " + mMouse.mMouseHitX + ", " + mMouse.mMouseHitY);
		//mMouseX = mMouse.mMouseHitX;
		
		//fixed for negatvie Z values
		//mMouseY = mMouse.mMouseHitY;
		//fixed for negatvie Z values
	}
	public bool UpdateTarget()
	{
		bool rc = false;
		//Random movement
		int totalMove = Random.Range(0,mMovement);
		int MoveX = Random.Range (0,totalMove);
		int MoveY = totalMove - MoveX;
		
		int temp=mTileMap.MapInfo.GetTileType(MoveX, MoveX);
		//Random moveMent;
		Debug.Log (temp);
		switch(temp)
		{
		case 1:
			Debug.Log ("Target::Floor");
			mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 1);
			Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(MoveX, MoveY);
			Move(v3Temp);
			mPositionX = MoveX;
			mPositionY = MoveY;
			mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 4);
			TargetTurn = true;
			rc = true;
			break;
		case 2:
			Debug.Log ("Target::Wall");
			break;
		default:
			Debug.Log ("Target::Fuck Off");
			break;
		}
		return rc;
	}
	void Move(Vector3 pos)
	{
		gameObject.transform.position = pos + new Vector3(0.0f, 1.0f, 0.0f);
	}

}
