//Jack Ng
//Nov 5th, 2014


using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(GraphSearch))]
[RequireComponent(typeof(Graph))]
[RequireComponent(typeof(Node))]
public class BaseTarget : MonoBehaviour
{
	//Imformation Needed for the Game
	TileMap mTileMap;
	TileMapMouse mMouse;
	GameObject mTileMapObject;
	GameObject mPlayer;

	//needed to reference the manager
	GameManager mManager;

	////Current Position
	private int mPositionX;
	private int mPositionY;

	//Current Stats
	public int mDefense;
	public int mMovement;
	public int mRunMovement;
	public int mInfamy;

	//3 nodes to move around
	public int mNodeAX;
	public int mNodeAY;
	public int mWeightA;

	public int mNodeBX;
	public int mNodeBY;
	public int mWeightB;

	public int mNodeCX;
	public int mNodeCY;
	public int mWeightC;

	//List to Track Graph
	public List<Node>mCloseList;
	public List<Node>mPath;

	public bool mTargetTurn;


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
		mTargetTurn = false;
		//mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
		//mPlayer=GameObject.Find("Player");
		//mTileMap = mTileMapObject.GetComponent<Player>();


		mTileMapObject=GameObject.Find("CurrentTileMap");
		mManager = GameObject.Find ("GameManager").GetComponent<GameManager>();

		mTileMap = mTileMapObject.GetComponent<TileMap>();
		//mMouseX = mMouse.mMouseHitX;
		////fixed for negatvie Z values
		//mMouseY = mMouse.mMouseHitY;
		//Hard fixed for negative Z values
		mManager.AddTarget (this);
	}

	// Update is called once per frame
	void Update () 
	{
		mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
		mTileMap = mTileMapObject.GetComponent<TileMap>();
		//Debug.Log ("Tile: " + mMouse.mMouseHitX + ", " + mMouse.mMouseHitY);
		mMouseX = mMouse.mMouseHitX;
		
		//fixed for negatvie Z values
		mMouseY = mMouse.mMouseHitY;
		//fixed for negatvie Z values
		if (Input.GetKey ("c")) 
		{
			//mTileMap = GetComponent<Player>();

		}
		if (Input.GetKey ("b"))
		{
			UpdateTarget ();
		}
	}
	public bool UpdateTarget()
	{
		bool rc = false;
		bool walk = false;
			int temp=mTileMap.MapInfo.GetTileType(mMouseX, mMouseY);
			//Random moveMent;
			switch(temp)
			{
				case 1:
					{
						Debug.Log ("Target::Floor");
						mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 1);
						Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(mMouseX, mMouseY);
						Move(v3Temp);
						mPositionX = mMouseX;
						mPositionY = mMouseY;
						mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, 4);
						mTargetTurn = true;
						rc = true;
						walk = true;
						break;
					}
				case 2:
					{
						Debug.Log ("Target::Wall");
						break;
					}
				default:
					{
						Debug.Log ("Target::Fuck Off");
						break;
					}
			}
			if(walk == true)
			{
				Debug.Log("Walking");
			}
		//}
		return rc;
	}
	void Move(Vector3 pos)
	{
		gameObject.transform.position = pos + new Vector3(0.0f, 1.0f, 0.0f);
	}
	void PathFind(int startX, int startY, int endX, int endY)
	{
		GraphSearch mSearch= new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.Run(startX, startY, endX, endY);
		if(mSearch.IsFound())
		{
			mCloseList = mSearch.GetCloseList();
			mPath= mSearch.GetPathList();
		}
		foreach(Node i in mPath)
		{
			mTileMap.MapInfo.SetTileTypeIndex(i.mIndex,1);
		}
	}	
	void ResetPath()
	{
		if (mPath == null) 
		{
			return;
		}
		for (int i=0; i<mPath.Count; i++)
		{
			int x = mPath[i].mIndex;
			mTileMap.MapInfo.SetTileTypeIndex (x,0);
		}
		mPath.Clear ();
	}
}
