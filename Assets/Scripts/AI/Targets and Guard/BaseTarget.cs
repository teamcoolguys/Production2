//Jack Ng
//Nov 5th, 2014


using UnityEngine;
using System.Collections;

public class BaseTarget : MonoBehaviour {

	//public int mStartingPositionX;
	//public int mStartingPositionY;

	private int mPositionX;
	private int mPositionY;

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
		mPositionX = 0;
		mPositionY = 0;
		
		//Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(mPositionX, mPositionY);
		//Move(v3Temp);
		//mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 4);
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
		bool walk = false;
		while(walk==false)
		{

			//Random movement
			int totalMove = Random.Range(0,mMovement); //4
			//int possibleMoveX = 0;
			//int possibleMoveY = 0;
			//int checkX;
			//int checkY;
			//for(possibleMoveY=0;possibleMoveY<totalMove; possibleMoveY++)
			//{
			//	for(possibleMoveX = 0; possibleMoveX<=totalMove-possibleMoveY; possibleMoveX++)
			//	{
			//		checkX = mPositionX + possibleMoveY;
			//		checkY = mPositionY + possibleMoveY; 
			//		if (possibleMoveX > 9) 
			//		{
			//			possibleMoveX = 9;
			//		}
			//		if(possibleMoveX<0)
			//		{
			//			possibleMoveX = 0;
			//		}
			//		if (possibleMoveY > 9) 
			//		{
			//			possibleMoveY = 9;
			//		}
			//		if(possibleMoveY<0)
			//		{
			//			possibleMoveY = 0;
			//		}
			//		if(mTileMap.MapInfo.GetTileType(possibleMoveX,possibleMoveY)==1)
			//		mTileMap.MapInfo.SetTileType(possibleMoveX,possibleMoveY, 0);
			//	}
			//}
			//Debug.Log ("breakpoint1");
			int MoveX = Random.Range (0,totalMove);
			int MoveY = totalMove - MoveX;
			Debug.Log ("Moved: " + MoveX + ", " + MoveY);
			MoveX = Random.Range (-MoveX, MoveX);
			MoveY = Random.Range (-MoveY, MoveY);
			Debug.Log ("MovedSecond: " + MoveX + ", " + MoveY);
			MoveX += mPositionX;
			MoveY += mPositionY; 
			if (MoveX > 9) 
			{
				MoveX = 9;
			}
			if(MoveX<0)
			{
				MoveX = 0;
			}
			if (MoveY > 9) 
			{
				MoveY = 9;
			}
			if(MoveY<0)
			{
				MoveY = 0;
			}

			int temp=mTileMap.MapInfo.GetTileType(MoveX, MoveX);
			//Random moveMent;

			Debug.Log ("TargetMoved: " + MoveX + ", " + MoveY);
			Debug.Log (temp);
			switch(temp)
			{

			case 1:
				Debug.Log ("Target::Floor");
				Debug.Log ("Target: " + MoveX + ", " + MoveY);
				mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 1);
				Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(MoveX, MoveY);
				Move(v3Temp);
				mPositionX = MoveX;
				mPositionY = MoveY;
				mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 4);
				TargetTurn = true;
				rc = true;
				walk = true;
				break;
			case 2:
				Debug.Log ("Target::Wall");
				break;
			default:
				Debug.Log ("Target::Fuck Off");
				break;
			}
		}
		return rc;
	}
	void Move(Vector3 pos)
	{
		gameObject.transform.position = pos + new Vector3(0.0f, 1.0f, 0.0f);
	}

}
